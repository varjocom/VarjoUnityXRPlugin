// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

using Unity.Collections;


namespace Varjo.XR
{
    internal class VarjoDistortedColorStreamInternal
    {
        private VarjoStreamConfig _config = default;

        private bool cameraParametersSet = false;

        private VarjoCameraIntrinsics[] intrinsics = new VarjoCameraIntrinsics[2];

        private VarjoMatrix[] extrinsics = new VarjoMatrix[2];

        internal ref readonly VarjoStreamConfig ConfigRef => ref _config;

        internal VarjoDistortedColorStreamInternal() { }

        internal bool IsStarted() => Native.MRDistortedColorStream_IsStarted();

        internal bool HasReceivedData() => Native.MRDistortedColorStream_HasReceivedData();

        internal bool HasFetchedCameraParameters() => Native.MRDistortedColorStream_HasFetchedCameraParameters();

        internal bool HasNewFrame() => Native.MRDistortedColorStream_HasNewFrame();

        internal bool Start()
        {
            _config = Native.MRDistortedColorStream_GetStreamConfig();
            return Native.MRDistortedColorStream_Start();
        }

        internal void Stop() => Native.MRDistortedColorStream_Stop();

        internal bool GetCameraParameters()
        {
            if (!HasFetchedCameraParameters()) return false;

            intrinsics[0] = Native.MRDistortedColorStream_GetIntrinsics(VarjoStreamChannel.Left);
            intrinsics[1] = Native.MRDistortedColorStream_GetIntrinsics(VarjoStreamChannel.Right);
            extrinsics[0] = Native.MRDistortedColorStream_GetExtrinsics(VarjoStreamChannel.Left);
            extrinsics[1] = Native.MRDistortedColorStream_GetExtrinsics(VarjoStreamChannel.Right);
            cameraParametersSet = true;
            return true;
        }
        internal VarjoCameraIntrinsics GetCameraIntrinsics(VarjoStreamChannel channel)
        {
            if (!cameraParametersSet) GetCameraParameters();
            return intrinsics[(int)channel];
        }

        internal VarjoMatrix GetCameraExtrinsics(VarjoStreamChannel channel)
        {
            if (!cameraParametersSet) GetCameraParameters();
            return extrinsics[(int)channel];
        }

        internal bool IsReadyToReturnImage() => IsStarted() && HasReceivedData();

        internal bool ObtainCPUDataCopy(Allocator allocator, out NativeArray<byte> left, out NativeArray<byte> right, out VarjoBufferMetadata leftBufferMetadata, out VarjoBufferMetadata rightBufferMetadata, out DistortedColorFrameMetadata frameMetadata)
        {
            if (!IsReadyToReturnImage())
            {
                left = right = default;
                leftBufferMetadata = rightBufferMetadata = default;
                frameMetadata = default;
                return false;
            }

            Native.MRDistortedColorStream_Lock();

            frameMetadata = Native.MRDistortedColorStream_GetLastFrameMetadata();

            leftBufferMetadata = Native.MRDistortedColorStream_GetLastFrameBufferMetadata(VarjoStreamChannel.Left);
            left = new NativeArray<byte>(leftBufferMetadata.byteSize, allocator, NativeArrayOptions.UninitializedMemory);

            rightBufferMetadata = Native.MRDistortedColorStream_GetLastFrameBufferMetadata(VarjoStreamChannel.Right);
            right = new NativeArray<byte>(rightBufferMetadata.byteSize, allocator, NativeArrayOptions.UninitializedMemory);

            unsafe {
                void* pleft = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(left);
                void* pright = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(right);

                Native.MRDistortedColorStream_GetLastFrameCPUDataCopy((IntPtr)pleft, leftBufferMetadata.byteSize, (IntPtr)pright, rightBufferMetadata.byteSize);
            }

            Native.MRDistortedColorStream_Unlock();

            return true;
        }

        internal void ConvertNV12ToRGBA32(byte[] cpuData, in VarjoBufferMetadata metadata, IntPtr destination, int destinationSize)
        {
            GCHandle handle = GCHandle.Alloc(cpuData, GCHandleType.Pinned);
            Native.MRConvertNV12ToRGBA32(handle.AddrOfPinnedObject(), in metadata, destination, destinationSize);
            handle.Free();
        }

        internal void ConvertNV12ToRGBA32(NativeArray<byte> cpuData, in VarjoBufferMetadata metadata, IntPtr destination, int destinationSize)
        {
            unsafe {
                void* pdata = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(cpuData);
                Native.MRConvertNV12ToRGBA32((IntPtr)pdata, in metadata, destination, destinationSize);
            }
        }

        internal void GetYPlane(NativeArray<byte> cpuData, in VarjoBufferMetadata metadata, IntPtr destination, int destinationSize)
        {
            unsafe {
                void* pdata = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(cpuData);
                Native.MRGetYPlane((IntPtr)pdata, in metadata, destination, destinationSize);
            }
        }


        private static class Native
        {
            [DllImport("VarjoUnityXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool MRDistortedColorStream_IsStarted();

            [DllImport("VarjoUnityXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool MRDistortedColorStream_HasNewFrame();

            [DllImport("VarjoUnityXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool MRDistortedColorStream_HasReceivedData();

            [DllImport("VarjoUnityXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool MRDistortedColorStream_HasFetchedCameraParameters();

            [DllImport("VarjoUnityXR")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool MRDistortedColorStream_Start();

            [DllImport("VarjoUnityXR")]
            public static extern void MRDistortedColorStream_Stop();

            [DllImport("VarjoUnityXR")]
            public static extern DistortedColorFrameMetadata MRDistortedColorStream_GetLastFrameMetadata();

            [DllImport("VarjoUnityXR")]
            public static extern VarjoBufferMetadata MRDistortedColorStream_GetLastFrameBufferMetadata(VarjoStreamChannel channel);

            [DllImport("VarjoUnityXR")]
            public static extern VarjoCameraIntrinsics MRDistortedColorStream_GetIntrinsics(VarjoStreamChannel channel);

            [DllImport("VarjoUnityXR")]
            public static extern VarjoMatrix MRDistortedColorStream_GetExtrinsics(VarjoStreamChannel channel);


            [DllImport("VarjoUnityXR")]
            public static extern VarjoStreamConfig MRDistortedColorStream_GetStreamConfig();

            [DllImport("VarjoUnityXR")]
            public static extern void MRDistortedColorStream_GetLastFrameCPUDataCopy(IntPtr leftBuffer, int leftBufferSize, IntPtr rightBuffer, int rightBufferSize);

            [DllImport("VarjoUnityXR")]
            public static extern void MRDistortedColorStream_Lock();

            [DllImport("VarjoUnityXR")]
            public static extern void MRDistortedColorStream_Unlock();

            [DllImport("VarjoUnityXR")]
            public static extern void MRConvertNV12ToRGBA32(IntPtr cpuData, in VarjoBufferMetadata buffer, IntPtr destination, int destinationSize);

            [DllImport("VarjoUnityXR")]
            public static extern void MRGetYPlane(IntPtr cpuData, in VarjoBufferMetadata buffer, IntPtr destination, int destinationSize);
        }
    }
}
