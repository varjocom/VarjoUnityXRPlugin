// Copyright 2022 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Cubemap Metadata.
    /// </summary>
    public class VarjoCubemapMetadata
    {
        /** <summary>Current environment cubemap mode.</summary> */
        public VarjoEnvironmentCubemapMode mode { get; internal set; }
        /** <summary>White balance temperature in Kelvin degrees.</summary> */
        public double whiteBalanceTemperature { get; internal set; }
        /** <summary>White balance normalization data.</summary> */
        public VarjoWBNormalizationData wbNormalizationData { get; internal set; }

        /** <summary></summary> */
        public double brightnessNormalizationGain { get; internal set; }

        internal VarjoCubemapMetadata(VarjoEnvironmentCubemapData cubemapData)
        {
            VarjoEnvironmentCubemapData data = cubemapData;
            mode = data.mode;
            whiteBalanceTemperature = data.whiteBalanceTemperature;
            wbNormalizationData = new VarjoWBNormalizationData(data.wbNormalizationData);
            brightnessNormalizationGain = data.brightnessNormalizationGain;
        }
    }
}
