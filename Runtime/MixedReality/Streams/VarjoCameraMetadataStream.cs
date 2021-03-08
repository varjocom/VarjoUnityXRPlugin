// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    public class VarjoCameraMetadataStream : VarjoFrameStream
    {
        public class VarjoCameraMetadataFrame
        {
            public long timestamp { get; internal set; }                   //!< Timestamp at end of exposure.
            public VarjoCameraMetadata metadata { get; internal set; }     //!< Camera metadata
        }

        private VarjoDistortedColorData data;

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
                var frame = new VarjoCameraMetadataFrame();
                frame.timestamp = data.timestamp;
                frame.metadata = new VarjoCameraMetadata(data);
                return frame;
            }
        }

        internal override void NewFrameCallback(VarjoStreamFrame streamData, IntPtr userdata)
        {
            lock (mutex)
            {
                Debug.Assert(streamData.type == VarjoStreamType.DistortedColor);
                data = streamData.metadata.distortedColorData;
            }
        }

        internal override VarjoStreamType StreamType { get { return VarjoStreamType.CameraMetadata; } }
    }
}
