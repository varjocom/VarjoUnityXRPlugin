// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Varjo.XR
{
    public abstract class VarjoFrameStream
    {
        private VarjoStreamConfig _config;
        protected readonly object mutex;
        private VarjoStreamCallback callback;
        private protected bool hasReceivedData;
        private readonly int instanceIndex;


        public ref readonly VarjoStreamConfig configRef => ref _config;
        public bool isActive { get; internal set; }
        public bool hasNewFrame { get; internal set; }


        protected VarjoFrameStream()
        {
            mutex = new object();

            //search for free space in the streams:
            lock (s_streamsInstances)
            {
                int length = s_streamsInstances.Count;
                for (int i = 0; i < length; ++i)
                {
                    //insert reference to the instance in the list
                    if (s_streamsInstances[i] == null)
                    {
                        s_streamsInstances[i] = this;
                        instanceIndex = i;
                        return;
                    }
                }
                //if no free references available, insert in the end:
                s_streamsInstances.Add(this);
                instanceIndex = s_streamsInstances.Count - 1;
            }
        }
        ~VarjoFrameStream()
        {
            lock (s_streamsInstances)
            {
                if (s_streamsInstances.Count > instanceIndex)
                {
                    s_streamsInstances[instanceIndex] = null;                
                }

                //trim empty space:
                int lastActiveIndex = s_streamsInstances.FindLastIndex((VarjoFrameStream instance) => instance != null);
                if (lastActiveIndex > -1)
                {
                    s_streamsInstances.RemoveRange(lastActiveIndex, s_streamsInstances.Count - lastActiveIndex);
                }
            }
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
            callback = s_NewFrameCallback;
            _config = VarjoMixedReality.GetStreamConfig(StreamType);
            isActive = VarjoMixedReality.StartDataStream(StreamType, callback, (IntPtr)instanceIndex);
            if (!isActive)
            {
                VarjoError.CheckError();
            }
            hasNewFrame = false;
            return isActive;
        }

        /// <summary>
        /// Stops the frame stream.
        /// </summary>
        public void Stop()
        {
            VarjoMixedReality.StopDataStream(StreamType);
            callback = null;
            isActive = false;
            hasNewFrame = false;
            hasReceivedData = false;
        }

        internal abstract void NewFrameCallback(VarjoStreamFrame data);
        internal abstract VarjoStreamType StreamType { get; }

        [AOT.MonoPInvokeCallback(typeof(VarjoFrameStream))]
        private static void s_NewFrameCallback(VarjoStreamFrame data, IntPtr userdata)
        {
            int instanceIndex = (int)userdata;
            s_streamsInstances[instanceIndex].NewFrameCallback(data);
        }

        private static List<VarjoFrameStream> s_streamsInstances = new List<VarjoFrameStream>();
    }
}
