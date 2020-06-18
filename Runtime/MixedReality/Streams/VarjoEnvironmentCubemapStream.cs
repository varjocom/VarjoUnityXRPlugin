// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    public class VarjoEnvironmentCubemapStream : VarjoFrameStream
    {
        public class VarjoEnvironmentCubemapFrame
        {
            public long timestamp { get; internal set; }  // Timestamp at end of exposure.
            public Cubemap cubemap { get; internal set; } // Environmental lighting cubemap.
        }

        private VarjoEnvironmentCubemapData data;
        private VarjoTextureBuffer buffer;
        private Cubemap cubemap;

        internal VarjoEnvironmentCubemapStream() : base()
        {
            buffer = new VarjoTextureBuffer(false);
        }

        /// <summary>
        /// Gets latest frame from the frame stream.
        /// Frames update only if stream has been started.
        /// May be called from main thread only.
        /// </summary>
        /// <returns>Latest environmental lighting cubemap.</returns>
        public VarjoEnvironmentCubemapFrame GetFrame()
        {
            lock (mutex)
            {
                var frame = new VarjoEnvironmentCubemapFrame();
                frame.timestamp = data.timestamp;
                UpdateCubemap();
                frame.cubemap = cubemap;
                return frame;
            }
        }

        internal override void NewFrameCallback(VarjoStreamFrame streamData, IntPtr userdata)
        {
            lock (mutex)
            {
                Debug.Assert(streamData.type == StreamType);
                data = streamData.metadata.environmentCubemapData;

                long bufferId = 0;
                if (!VarjoMixedReality.GetDataStreamBufferId(streamData.id, streamData.frameNumber, 0 /* varjo_ChannelIndex_First */, out bufferId))
                {
                    Debug.LogErrorFormat("Failed to get cubemap buffer id {0}", streamData.frameNumber);
                    return;
                }
                buffer.UpdateBuffer(bufferId);
            }
        }

        internal override VarjoStreamType StreamType { get { return VarjoStreamType.EnvironmentCubemap; } }

        private void UpdateCubemap()
        {
            Texture2D texture = buffer.GetTexture2D();
            if (texture == null)
            {
                cubemap = null;
                return;
            }
            int resolution = texture.width;
            if (!cubemap || cubemap.width != resolution)
            {
                UnityEngine.Object.Destroy(cubemap);
                cubemap = new Cubemap(resolution, TextureFormat.RGBAHalf, false);
            }
            for (int faceIdx = 0; faceIdx < 6; ++faceIdx)
            {
                Graphics.CopyTexture(
                    src: texture, srcElement: 0, srcMip: 0, srcX: 0, srcY: faceIdx * resolution, srcWidth: resolution, srcHeight: resolution,
                    dst: cubemap, dstElement: faceIdx, dstMip: 0, dstX: 0, dstY: 0
                );
            }
        }
    }
}
