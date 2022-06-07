// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Camera Metadata Stream
    /// </summary>
    public class VarjoCameraMetadataStream : VarjoFrameStream
    {
        /// <summary>
        /// Varjo Camera Metadata Frame
        /// </summary>
        public class VarjoCameraMetadataFrame
        {
            /** <summary>Timestamp at end of exposure.</summary> */
            public long timestamp { get; internal set; }
            /** <summary>Camera metadata.</summary> */
            public VarjoCameraMetadata metadata { get; internal set; }
        }

        private DistortedColorFrameMetadata data;

        /// <summary>
        /// Gets latest frame from the frame stream.
        /// Frames update only if stream has been started.
        /// May be called from main thread only.
        /// </summary>
        /// <returns>Latest metadata stream frame.</returns>
        public VarjoCameraMetadataFrame GetFrame()
        {
            lock (mutex)
            {
                if (!hasReceivedData) return new VarjoCameraMetadataFrame();

                var frame = new VarjoCameraMetadataFrame();
                frame.timestamp = data.timestamp;
                frame.metadata = new VarjoCameraMetadata(data);

                hasNewFrame = false;
                return frame;
            }
        }

        internal override void NewFrameCallback(VarjoStreamFrame streamData)
        {
            lock (mutex)
            {
                Debug.Assert(streamData.type == VarjoStreamType.DistortedColor);
                data = streamData.metadata.distortedColorData;
                hasReceivedData = true;
                hasNewFrame = true;
            }
        }

        internal override VarjoStreamType StreamType { get { return VarjoStreamType.CameraMetadata; } }
    }
}
