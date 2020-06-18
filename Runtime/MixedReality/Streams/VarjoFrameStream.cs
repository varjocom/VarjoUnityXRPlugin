// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public abstract class VarjoFrameStream
    {
        protected readonly object mutex;
        private VarjoStreamCallback callback;

        protected VarjoFrameStream()
        {
            mutex = new object();
        }

        /// <summary>
        /// Check if device supports the frame stream.
        /// </summary>
        /// <returns>True, if device supports this type of frame stream.</returns>
        public bool IsSupported()
        {
            return VarjoMixedReality.SupportsDataStream(StreamType);
        }

        /// <summary>
        /// Starts the frame stream.
        /// </summary>
        /// <returns>True, if frame stream supported and successfully started.</returns>
        public bool Start()
        {
            if (callback != null)
            {
                return true;
            }
            callback = new VarjoStreamCallback(NewFrameCallback);
            return VarjoMixedReality.StartDataStream(StreamType, callback);
        }

        /// <summary>
        /// Stops the frame stream.
        /// </summary>
        public void Stop()
        {
            VarjoMixedReality.StopDataStream(StreamType);
            callback = null;
        }

        internal abstract void NewFrameCallback(VarjoStreamFrame data, IntPtr userdata);
        internal abstract VarjoStreamType StreamType { get; }
    }
}
