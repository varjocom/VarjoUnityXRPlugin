using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

using XRCpuImageHandle = System.Int32;

using static UnityEngine.XR.ARSubsystems.XRCpuImage;


namespace Varjo.XR
{
    /// <summary>
    /// Channels of color stream.
    /// </summary>
    public enum VarjoStreamChannel
    {
        Left,
        Right
    }


    public partial class VarjoCameraSubsystem
    {
        /// <summary>
        /// An API of XRCpuImage
        /// </summary>
        private sealed class VarjoCpuImageAPI : XRCpuImage.Api
        {
            #region AsyncRequests
            private static Dictionary<int, asyncRequest> asyncRequests = new Dictionary<XRCpuImageHandle, asyncRequest>();
            private static int NextRequestId = 1;

            private static (int, asyncRequest) CreateAsyncRequest(NativeArray<byte> data, AsyncConversionStatus status)
            {
                int requestId = NextRequestId++;
                var request = new asyncRequest(data, status);
                asyncRequests.Add(requestId, request);
                return (requestId, request);
            }

            private class asyncRequest : IDisposable
            {
                public unsafe IntPtr DataPtr => (IntPtr)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(data);

                public NativeArray<byte> data;
                public int dataSizeBytes;
                public XRCpuImage.AsyncConversionStatus status;

                public void Dispose()
                {
                    if (status != XRCpuImage.AsyncConversionStatus.Disposed)
                    {
                        status = XRCpuImage.AsyncConversionStatus.Disposed;
                        data.Dispose();
                        dataSizeBytes = -1;
                    }
                }

                public asyncRequest(NativeArray<byte> data, XRCpuImage.AsyncConversionStatus status)
                {
                    this.data = data;
                    this.dataSizeBytes = data.Length;
                    this.status = status;
                }
            }
            #endregion

            private int GetUnityTextureBytesPerPixel(TextureFormat format)  //for unsupported formats return -1
            {
                switch (format)
                {
                    case TextureFormat.RGBA32:
                        return 4;
                    case TextureFormat.R8:
                        return 1;
                    default:
                        return -1;
                }
            }


            public override bool FormatSupported(XRCpuImage image, TextureFormat format) => FormatSupported(format);

            public bool FormatSupported(TextureFormat format) => GetUnityTextureBytesPerPixel(format) != -1;

            public override void DisposeImage(XRCpuImageHandle nativeHandle)
            {
                CPUImages.GetImage(nativeHandle)?.Dispose();
                CPUImages.RemoveImage(nativeHandle);
            }

            public override bool NativeHandleValid(int nativeHandle)
            {
                //image is not valid if it is disposed or not contained in CPUImages collection:
                return !CPUImages.GetImage(nativeHandle)?.IsDisposed ?? false;
            }

            public override bool TryGetPlane(int nativeHandle, int planeIndex, out XRCpuImage.Plane.Cinfo planeCinfo)
            {
                VarjoCpuImage image = CPUImages.GetImage(nativeHandle);
                if (image is null)
                {
                    planeCinfo = default;
                    return false;
                }
                else
                {
                    unsafe
                    {
                        ref readonly var metadata = ref image.leftBufferMetadata;
                        NativeArray<byte> data = image.leftBuffer;

                        if (Channel == VarjoStreamChannel.Right)
                        {
                            metadata = ref image.rightBufferMetadata;
                            data = image.rightBuffer;
                        }

                        switch (planeIndex)
                        {
                            case 0: //Y plane:
                                {
                                    void* YPtr = NativeArrayUnsafeUtility.GetUnsafePtr<byte>(data);
                                    int YPlaneSizeBytes = metadata.height * metadata.rowStride; //1 byte per pixel
                                    planeCinfo = new XRCpuImage.Plane.Cinfo((IntPtr)YPtr, YPlaneSizeBytes, metadata.rowStride, pixelStride: 1);
                                    return true;
                                }

                            case 1: //UV plane:
                                {
                                    byte* UVPtr = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(data);
                                    UVPtr += metadata.height * metadata.rowStride;  //UV plane locates after Y plane

                                    int UVPlaneSizeBytes = metadata.height * metadata.rowStride;

                                    if (metadata.textureFormat == VarjoTextureFormat.NV12)
                                        UVPlaneSizeBytes >>= 1;

                                    planeCinfo = new XRCpuImage.Plane.Cinfo((IntPtr)UVPtr, UVPlaneSizeBytes, metadata.rowStride, pixelStride: 2);    //UV plane is 8+8bit interleaved in half resolution
                                    return true;
                                }

                            default:
                                planeCinfo = default;
                                return false;
                        }
                    }
                }
            }

            public override bool TryGetConvertedDataSize(XRCpuImageHandle nativeHandle, Vector2Int dimensions, TextureFormat format, out int size)
            {
                int bytesPerPixel = GetUnityTextureBytesPerPixel(format);   //return -1 for unsupported formats
                size = bytesPerPixel * dimensions.x * dimensions.y;
                return size > 0;
            }

            public override bool TryConvert(XRCpuImageHandle nativeHandle, ConversionParams conversionParams, IntPtr destinationBuffer, int bufferLength)
            {
                return TryConvert(nativeHandle, conversionParams, destinationBuffer, bufferLength, Channel);
            }

            public bool TryConvert(XRCpuImageHandle nativeHandle, ConversionParams conversionParams, IntPtr destinationBuffer, int bufferLength, VarjoStreamChannel channel)
            {
                var cpuImage = CPUImages.GetImage(nativeHandle);

                Debug.Assert(cpuImage != null);
                Debug.Assert(!cpuImage.IsDisposed);
                Debug.Assert(destinationBuffer != IntPtr.Zero);

                if (conversionParams.transformation != Transformation.None)
                {
                    return false;
                }

                var buffer = cpuImage.GetBuffer(channel);
                var metadata = cpuImage.GetMetadata(channel);

                switch (conversionParams.outputFormat)
                {
                    case TextureFormat.R8:
                        colorStream.GetYPlane(buffer, in metadata, destinationBuffer, bufferLength);
                        return true;

                    case TextureFormat.RGBA32:
                        {
                            if (metadata.textureFormat == VarjoTextureFormat.NV12)
                                colorStream.ConvertNV12ToRGBA32(buffer, in metadata, destinationBuffer, bufferLength);
                            else
                                throw new NotSupportedException($"Format \"{metadata.textureFormat}\" is not supported");
                        }
                        return true;

                    default:
                        return false;
                }
            }

            public override void ConvertAsync(XRCpuImageHandle nativeHandle, ConversionParams conversionParams, OnImageRequestCompleteDelegate callback, IntPtr context)
            {
                Debug.Assert(callback != null);

                if (!TryGetConvertedDataSize(nativeHandle, conversionParams.outputDimensions, conversionParams.outputFormat, out int channelSizeBytes))
                {
                    callback(AsyncConversionStatus.Failed, conversionParams, IntPtr.Zero, dataLength: -1, context);
                    return;
                }

                var task = new Task(() => {
                    //size to store both left and right channels:
                    using (var buffer = new NativeArray<byte>(channelSizeBytes << 1, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                    {
                        IntPtr dataPtr;
                        unsafe {
                            dataPtr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(buffer);
                        }

                        bool result = TryConvert(nativeHandle, conversionParams, dataPtr, channelSizeBytes, VarjoStreamChannel.Left) &
                                      TryConvert(nativeHandle, conversionParams, dataPtr + channelSizeBytes, channelSizeBytes, VarjoStreamChannel.Right);

                        if (result)
                            callback.Invoke(AsyncConversionStatus.Ready, conversionParams, dataPtr, buffer.Length, context);
                        else
                            callback.Invoke(AsyncConversionStatus.Failed, conversionParams, IntPtr.Zero, dataLength: -1, context);
                    }
                });

                task.Start();
            }

            public override int ConvertAsync(int nativeHandle, ConversionParams conversionParams)
            {
                if (!TryGetConvertedDataSize(nativeHandle, conversionParams.outputDimensions, conversionParams.outputFormat, out int channelSizeBytes))
                {
                    return -1;
                }

                (int requestId, var request) = CreateAsyncRequest(
                        //size to store both left and right channels:
                        data: new NativeArray<byte>(channelSizeBytes << 1, AsyncRequestAllocator, NativeArrayOptions.UninitializedMemory),
                        status: AsyncConversionStatus.Pending
                    );

                var task = new Task(() => {
                    request.status = AsyncConversionStatus.Processing;

                    bool result = TryConvert(nativeHandle, conversionParams, request.DataPtr, channelSizeBytes, VarjoStreamChannel.Left) &
                                  TryConvert(nativeHandle, conversionParams, request.DataPtr + channelSizeBytes, channelSizeBytes, VarjoStreamChannel.Right);

                    if (result)
                        request.status = AsyncConversionStatus.Ready;
                    else
                        request.status = AsyncConversionStatus.Failed;
                });

                task.Start();

                return requestId;
            }

            public override bool TryGetAsyncRequestData(int requestId, out IntPtr dataPtr, out int dataLength)
            {
                if (asyncRequests.TryGetValue(requestId, out var request))
                {
                    dataPtr = request.DataPtr;
                    dataLength = request.dataSizeBytes;
                    return true;
                }
                else
                {
                    dataPtr = IntPtr.Zero;
                    dataLength = -1;
                    return false;
                }
            }

            public override AsyncConversionStatus GetAsyncRequestStatus(int requestId)
            {
                if (asyncRequests.TryGetValue(requestId, out var request))
                {
                    return request.status;
                }
                else
                    return AsyncConversionStatus.Disposed;
            }

            public override void DisposeAsyncRequest(int requestId)
            {
                if (asyncRequests.TryGetValue(requestId, out var request))
                {
                    request.Dispose();
                    asyncRequests.Remove(requestId);
                }
            }
        }
    }
}
