// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo WB Normalization Data
    /// </summary>
    public class VarjoWBNormalizationData
    {
        /** <summary>White balance gains to convert from 6500K to VST color temperature.</summary> */
        public Color wbGains;
        /** <summary>Inverse CCM for 6500K.</summary> */
        public Matrix4x4 invCCM;
        /** <summary>CCM for VST color temperature.</summary> */
        public Matrix4x4 ccm;
    }

    /// <summary>
    /// Varjo Camera Metadata.
    /// </summary>
    public class VarjoCameraMetadata
    {
        /** <summary>EV (exposure value) at ISO100.</summary> */
        public double ev { get; internal set; }
        /** <summary>Exposure time in seconds.</summary> */
        public double exposureTime { get; internal set; }
        /** <summary>White balance temperature in Kelvin degrees.</summary> */
        public double whiteBalanceTemperature { get; internal set; }
        /** <summary>White balance normalization data.</summary> */
        public VarjoWBNormalizationData wbNormalizationData { get; internal set; }
        /** <summary>Texture from left camera.</summary> */
        public Texture2D leftTexture { get; internal set; }
        /** <summary>Texture from right camera.</summary> */
        public Texture2D rightTexture { get; internal set; }
        /** <summary>The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.</summary> */
        public double cameraCalibrationConstant { get; internal set; }

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
