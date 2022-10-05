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

        internal VarjoWBNormalizationData(WBNormalizationData data)
        {
            if (data.wbGains != null)
            {
                wbGains = new Color(
                    (float)data.wbGains[0],
                    (float)data.wbGains[1],
                    (float)data.wbGains[2]
                );
            }

            if (data.invCCM != null)
            {
                invCCM = new Matrix4x4(
                    new Vector4(
                        (float)data.invCCM[0],
                        (float)data.invCCM[1],
                        (float)data.invCCM[2],
                        0.0f),
                    new Vector4(
                        (float)data.invCCM[3],
                        (float)data.invCCM[4],
                        (float)data.invCCM[5],
                        0.0f),
                    new Vector4(
                        (float)data.invCCM[6],
                        (float)data.invCCM[7],
                        (float)data.invCCM[8],
                        0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
            }

            if (data.ccm != null)
            {
                ccm = new Matrix4x4(
                    new Vector4(
                        (float)data.ccm[0],
                        (float)data.ccm[1],
                        (float)data.ccm[2],
                        0.0f),
                    new Vector4(
                        (float)data.ccm[3],
                        (float)data.ccm[4],
                        (float)data.ccm[5],
                        0.0f),
                    new Vector4(
                        (float)data.ccm[6],
                        (float)data.ccm[7],
                        (float)data.ccm[8],
                        0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
                );
            }
        }
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

        /** <summary>The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.</summary> */
        public double cameraCalibrationConstant { get; internal set; }

        internal VarjoCameraMetadata (DistortedColorFrameMetadata distortedColorData)
        {
            DistortedColorFrameMetadata data = distortedColorData;
            ev = data.ev;
            exposureTime = data.exposureTime;
            whiteBalanceTemperature = data.whiteBalanceTemperature;
            wbNormalizationData = new VarjoWBNormalizationData(data.wbNormalizationData);
            cameraCalibrationConstant = data.cameraCalibrationConstant;
        }
    }
}
