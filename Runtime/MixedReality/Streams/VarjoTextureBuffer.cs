// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo.XR
{
    internal class VarjoTextureBuffer
    {
        private readonly object mutex;
        private Texture2D texture;
        private VarjoBufferMetadata metadata;
        private IntPtr cpuData;
        private byte[] data;
        private byte[] yData;
        private bool changed;
        private bool invertRowOrder;
        private int byteSize;

        internal VarjoTextureBuffer(bool invertRowOrder)
        {
            this.invertRowOrder = invertRowOrder;
            mutex = new object();
            changed = true;
        }

        // Thread safe
        internal void UpdateBuffer(long bufferId)
        {
            lock (mutex)
            {
                bool success = VarjoMixedReality.LockDataStreamBuffer(bufferId);
                if (!success)
                {
                    Debug.LogErrorFormat("Failed to lock data stream buffer {0}", bufferId);
                    return;
                }

                success = VarjoMixedReality.GetBufferMetadata(bufferId, out metadata);
                if (!success)
                {
                    Debug.LogErrorFormat("Failed to get buffer metadata {0}", bufferId);
                    return;
                }

                success = VarjoMixedReality.GetBufferCPUData(bufferId, out cpuData);
                if (!success)
                {
                    Debug.LogErrorFormat("Failed to get buffer CPU data {0}", bufferId);
                    return;
                }

                byteSize = metadata.width * metadata.height * GetUnityTextureBytesPerPixel(metadata.textureFormat);

                if (data == null || data.Length != byteSize)
                {
                    data = new byte[byteSize];
                }

                CopyCpuData(metadata, cpuData, invertRowOrder);
                VarjoMixedReality.UnlockDataStreamBuffer(bufferId);
                changed = true;
            }
        }

        // Not thread safe - may be called from main thread only
        internal Texture2D GetTexture2D()
        {
            lock (mutex)
            {
                if (!changed)
                {
                    return texture;
                }
                changed = false;

                if (data == null)
                {
                    return null;
                }

                TextureFormat format = GetTextureFormat(metadata.textureFormat);
                int width = (int)metadata.width;
                int height = (int)metadata.height;

                if (texture == null)
                {
                    texture = new Texture2D(width, height, format, false);
                }
                else if (texture.width != width || texture.height != height)
                {
#if UNITY_2021_2_OR_NEWER
                    texture.Reinitialize(width, height, format, false);
#else
                    texture.Resize(width, height, format, false);
#endif
                }

                LoadTextureData();

                return texture;
            }
        }

        private TextureFormat GetTextureFormat(VarjoTextureFormat varjoTextureFormat)
        {
            switch (varjoTextureFormat)
            {
                case VarjoTextureFormat.R8G8B8A8_SRGB:
                    return TextureFormat.RGBA32;
                case VarjoTextureFormat.B8G8R8A8_SRGB:
                    return TextureFormat.BGRA32;
                case VarjoTextureFormat.D32_FLOAT:
                    return TextureFormat.RFloat;
                case VarjoTextureFormat.A8_UNORM:
                    return TextureFormat.Alpha8;
                case VarjoTextureFormat.YUV422:
                    return TextureFormat.R8;
                case VarjoTextureFormat.RGBA16_FLOAT:
                    return TextureFormat.RGBAHalf;
                case VarjoTextureFormat.R8G8B8A8_UNORM:
                    return TextureFormat.RGBA32;
                case VarjoTextureFormat.R32_FLOAT:
                    return TextureFormat.RFloat;
                case VarjoTextureFormat.NV12:
                    return TextureFormat.R8; // We extract only Y channel from the YUV420 for now.
                default:
                    Debug.LogErrorFormat("Texture format {0} not supported", varjoTextureFormat);
                    return TextureFormat.Alpha8;
            }
        }

        private int GetUnityTextureBytesPerPixel(VarjoTextureFormat varjoTextureFormat)
        {
            TextureFormat textureFormat = GetTextureFormat(varjoTextureFormat);
            switch (textureFormat)
            {
                case TextureFormat.RGBA32:
                case TextureFormat.BGRA32:
                case TextureFormat.RFloat:
                    return 4;
                case TextureFormat.RGBAHalf:
                    return 8;
                case TextureFormat.Alpha8:
                case TextureFormat.R8:
                default:
                    return 1;
            }
        }

        private void CopyCpuData(VarjoBufferMetadata metadata, IntPtr cpuBuffer, bool invertRowOrder)
        {
            int height = metadata.height;
            int rowStride = metadata.rowStride;
            int destRowStride = byteSize / height;

            if (invertRowOrder)
            {
                for (int srcRow = 0; srcRow < height; ++srcRow)
                {
                    long srcOffset = cpuBuffer.ToInt64() + srcRow * rowStride;
                    int destOffset = (height - srcRow - 1) * destRowStride;
                    Marshal.Copy(new IntPtr(srcOffset), data, destOffset, destRowStride);
                }
            }
            else
            {
                if (rowStride == destRowStride)
                {
                    Marshal.Copy(cpuBuffer, data, 0, byteSize);
                }
                else
                {
                    // If row strides are not equal length, copy row by row.
                    for (int srcRow = 0; srcRow < height; ++srcRow)
                    {
                        long srcOffset = cpuBuffer.ToInt64() + srcRow * rowStride;
                        int destOffset = srcRow * destRowStride;
                        Marshal.Copy(new IntPtr(srcOffset), data, destOffset, destRowStride);
                    }
                }
            }
        }

        private void LoadTextureData()
        {
            // Extract Y from NV12.
            if (metadata.textureFormat == VarjoTextureFormat.NV12 && texture.format == TextureFormat.R8)
            {
                // Allocate working buffer for y data.
                if (yData == null || yData.Length != byteSize)
                {
                    yData = new byte[byteSize];
                }

                // Copy data row by row since we need to change the stride.
                for (int row = 0; row < metadata.height; ++row)
                {
                    int srcOffset = row * metadata.rowStride;
                    int destOffset = row * metadata.width;
                    Buffer.BlockCopy(data, srcOffset, yData, destOffset, metadata.width);
                }

                texture.LoadRawTextureData(yData);
            }
            else
            {
                // Other formats can be copied as-is.
                texture.LoadRawTextureData(data);
            }
            texture.Apply();
        }
    }
}
