// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    public class VarjoWBNormalizationData
    {
        public Color wbGains;       //!< White balance gains to convert from 6500K to VST color temperature.
        public Matrix4x4 invCCM;    //!< Inverse CCM for 6500K.
        public Matrix4x4 ccm;       //!< CCM for VST color temperature.
    }

    public class VarjoCameraMetadata
    {
        public double ev { get; internal set; }                        //!< EV (exposure value) at ISO100.
        public double exposureTime { get; internal set; }              //!< Exposure time in seconds.
        public double whiteBalanceTemperature { get; internal set; }   //!< White balance temperature in Kelvin degrees.
        public VarjoWBNormalizationData wbNormalizationData { get; internal set; } // White balance normalization data.
        public Texture2D leftTexture { get; internal set; }            //!< Texture from left camera.
        public Texture2D rightTexture { get; internal set; }           //!< Texture from right camera.
        public double cameraCalibrationConstant { get; internal set; } //!< The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.

        private VarjoDistortedColorData data;

        internal VarjoCameraMetadata (VarjoDistortedColorData distortedColorData)
        {
            data = distortedColorData;
            ev = data.ev;
            exposureTime = data.exposureTime;
            whiteBalanceTemperature = data.whiteBalanceTemperature;
            wbNormalizationData = new VarjoWBNormalizationData();

            if (data.wbNormalizationData.wbGains != null)
            {
                wbNormalizationData.wbGains = new Color(
                    (float)data.wbNormalizationData.wbGains[0],
                    (float)data.wbNormalizationData.wbGains[1],
                    (float)data.wbNormalizationData.wbGains[2]
                );
            }

            if (data.wbNormalizationData.invCCM != null)
            {
                wbNormalizationData.invCCM = new Matrix4x4(
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
                wbNormalizationData.ccm = new Matrix4x4(
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

            cameraCalibrationConstant = data.cameraCalibrationConstant;
        }
    }
}
