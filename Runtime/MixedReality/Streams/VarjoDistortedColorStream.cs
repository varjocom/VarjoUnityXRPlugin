// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Distorted Color Stream
    /// </summary>
    public class VarjoDistortedColorStream : VarjoFrameStream
    {
        /// <summary>
        /// Varjo Distorted Color Frame
        /// </summary>
        public class VarjoDistortedColorFrame
        {
            /** <summary>Timestamp at end of exposure.</summary> */
            public long timestamp { get; internal set; }
            /** <summary>Camera metadata</summary> */
            public VarjoCameraMetadata metadata { get; internal set; }
            /** <summary>Texture from left camera.</summary> */
            public Texture2D leftTexture { get; internal set; }
            /** <summary>Texture from right camera.</summary> */
            public Texture2D rightTexture { get; internal set; }
        }

        private VarjoDistortedColorData data;
        private VarjoTextureBuffer leftBuffer;
        private VarjoTextureBuffer rightBuffer;

        internal VarjoDistortedColorStream() : base()
        {
            leftBuffer = new VarjoTextureBuffer(true);
            rightBuffer = new VarjoTextureBuffer(true);
        }

        /// <summary>
        /// Gets latest frame from the frame stream.
        /// Frames update only if stream has been started.
        /// May be called from main thread only.
        /// </summary>
        /// <returns>Latest Distorted color stream frame.</returns>
        public VarjoDistortedColorFrame GetFrame()
        {
            lock (mutex)
            {
                if (!hasReceivedData) return new VarjoDistortedColorFrame();

                var frame = new VarjoDistortedColorFrame();
                frame.timestamp = data.timestamp;
                frame.metadata = new VarjoCameraMetadata(data);
                frame.leftTexture = leftBuffer.GetTexture2D();
                frame.rightTexture = rightBuffer.GetTexture2D();

                hasNewFrame = false;
                return frame;
            }
        }

        internal override void NewFrameCallback(VarjoStreamFrame streamData, IntPtr userdata)
        {
            lock (mutex)
            {
                Debug.Assert(streamData.type == StreamType);
                data = streamData.metadata.distortedColorData;

                long leftBufferId = 0;
                if (!VarjoMixedReality.GetDataStreamBufferId(streamData.id, streamData.frameNumber, 0 /* varjo_ChannelIndex_Left */, out leftBufferId))
                {
                    Debug.LogErrorFormat("Failed to get distorted color left buffer id {0}", streamData.frameNumber);
                    return;
                }

                long rightBufferId = 0;
                if (!VarjoMixedReality.GetDataStreamBufferId(streamData.id, streamData.frameNumber, 1/* varjo_ChannelIndex_Right */, out rightBufferId))
                {
                    Debug.LogErrorFormat("Failed to get distorted color right buffer id {0}", streamData.frameNumber);
                    return;
                }

                leftBuffer.UpdateBuffer(leftBufferId);
                rightBuffer.UpdateBuffer(rightBufferId);
                hasReceivedData = true;
                hasNewFrame = true;
            }
        }

        internal override VarjoStreamType StreamType { get { return VarjoStreamType.DistortedColor; } }
    }
}
