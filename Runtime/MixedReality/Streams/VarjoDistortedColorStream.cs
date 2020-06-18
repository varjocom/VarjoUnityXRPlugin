// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    public class VarjoDistortedColorStream : VarjoFrameStream
    {
        public class VarjoWBNormalizationData
        {
            public Color wbGains;       //!< White balance gains to convert from 6500K to VST color temperature.
            public Matrix4x4 invCCM;    //!< Inverse CCM for 6500K.
            public Matrix4x4 ccm;       //!< CCM for VST color temperature.
        }

        public class VarjoDistortedColorFrame
        {
            public long timestamp { get; internal set; }                   //!< Timestamp at end of exposure.
            public double ev { get; internal set; }                        //!< EV (exposure value) at ISO100.
            public double exposureTime { get; internal set; }              //!< Exposure time in seconds.
            public double whiteBalanceTemperature { get; internal set; }   //!< White balance temperature in Kelvin degrees.
            public VarjoWBNormalizationData wbNormalizationData { get; internal set; } // White balance normalization data.
            public Texture2D leftTexture { get; internal set; }            //!< Texture from left camera.
            public Texture2D rightTexture { get; internal set; }           //!< Texture from right camera.
            public double cameraCalibrationConstant { get; internal set; } //!< The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.
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
                var frame = new VarjoDistortedColorFrame();
                frame.timestamp = data.timestamp;
                frame.ev = data.ev;
                frame.exposureTime = data.exposureTime;
                frame.whiteBalanceTemperature = data.whiteBalanceTemperature;
                frame.wbNormalizationData = new VarjoWBNormalizationData();

                if (data.wbNormalizationData.wbGains != null)
                {
                    frame.wbNormalizationData.wbGains = new Color(
                        (float)data.wbNormalizationData.wbGains[0],
                        (float)data.wbNormalizationData.wbGains[1],
                        (float)data.wbNormalizationData.wbGains[2]
                    );
                }

                if (data.wbNormalizationData.invCCM != null)
                {
                    frame.wbNormalizationData.invCCM = new Matrix4x4(
                        new Vector4(
                            (float)data.wbNormalizationData.invCCM[0],
                            (float)data.wbNormalizationData.invCCM[1],
                            (float)data.wbNormalizationData.invCCM[2],
                            0.0f),
                        new Vector4(
                            (float)data.wbNormalizationData.invCCM[3],
                            (float)data.wbNormalizationData.invCCM[4],
                            (float)data.wbNormalizationData.invCCM[5],
                            0.0f),
                        new Vector4(
                            (float)data.wbNormalizationData.invCCM[6],
                            (float)data.wbNormalizationData.invCCM[7],
                            (float)data.wbNormalizationData.invCCM[8],
                            0.0f),
                        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                    );
                }

                if (data.wbNormalizationData.ccm != null)
                {
                    frame.wbNormalizationData.ccm = new Matrix4x4(
                        new Vector4(
                            (float)data.wbNormalizationData.ccm[0],
                            (float)data.wbNormalizationData.ccm[1],
                            (float)data.wbNormalizationData.ccm[2],
                            0.0f),
                        new Vector4(
                            (float)data.wbNormalizationData.ccm[3],
                            (float)data.wbNormalizationData.ccm[4],
                            (float)data.wbNormalizationData.ccm[5],
                            0.0f),
                        new Vector4(
                            (float)data.wbNormalizationData.ccm[6],
                            (float)data.wbNormalizationData.ccm[7],
                            (float)data.wbNormalizationData.ccm[8],
                            0.0f),
                        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                    );
                }

                frame.cameraCalibrationConstant = data.cameraCalibrationConstant;
                frame.leftTexture = leftBuffer.GetTexture2D();
                frame.rightTexture = rightBuffer.GetTexture2D();
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
            }
        }

        internal override VarjoStreamType StreamType { get { return VarjoStreamType.DistortedColor; } }
    }
}
