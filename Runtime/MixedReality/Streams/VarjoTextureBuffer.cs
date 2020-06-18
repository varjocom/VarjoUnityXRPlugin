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

                if (data == null || data.Length != metadata.byteSize)
                {
                    data = new byte[metadata.byteSize];
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
                    texture.Resize(width, height, format, false);
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
                    return TextureFormat.R8; // We extract only Y channel from the YUV422 for now.
                case VarjoTextureFormat.RGBA16_FLOAT:
                    return TextureFormat.RGBAHalf;
                default:
                    Debug.LogErrorFormat("Texture format {0} not supported", varjoTextureFormat);
                    return TextureFormat.Alpha8;
            }
        }

        private void CopyCpuData(VarjoBufferMetadata metadata, IntPtr cpuBuffer, bool invertRowOrder)
        {
            int height = (int)metadata.height;
            int rowStride = (int)metadata.rowStride;
            int byteSize = (int)metadata.byteSize;

            if (invertRowOrder)
            {
                for (int srcRow = 0; srcRow < height; ++srcRow)
                {
                    long srcOffset = cpuBuffer.ToInt64() + srcRow * rowStride;
                    int destOffset = (height - srcRow - 1) * rowStride;
                    Marshal.Copy(new IntPtr(srcOffset), data, destOffset, rowStride);
                }

                // YUV422 contains a second plane for UV.
                if (metadata.textureFormat == VarjoTextureFormat.YUV422)
                {
                    for (int srcRow = 0; srcRow < height; ++srcRow)
                    {
                        long srcOffset = cpuBuffer.ToInt64() + (srcRow + height) * rowStride;
                        int destOffset = (2 * height - srcRow - 1) * rowStride;
                        Marshal.Copy(new IntPtr(srcOffset), data, destOffset, rowStride);
                    }
                }
            }
            else
            {
                Marshal.Copy(cpuBuffer, data, 0, byteSize);
            }
        }

        private void LoadTextureData()
        {
            // Extract Y from YUV422.
            if (metadata.textureFormat == VarjoTextureFormat.YUV422 && texture.format == TextureFormat.R8)
            {
                // Allocate working buffer for y data.
                if (yData == null || yData.Length != metadata.width * metadata.height * sizeof(byte))
                {
                    yData = new byte[metadata.width * metadata.height * sizeof(byte)];
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
