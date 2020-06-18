using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public class VarjoEyeTracking
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct GazeCalibrationParameter
        {
            [MarshalAs(UnmanagedType.LPStr)] public string key;
            [MarshalAs(UnmanagedType.LPStr)] public string value;
        }

        public enum GazeCalibrationMode
        {
            Legacy,
            Fast
        };

        public enum GazeOutputFilterMode
        {
            Standard,
            None
        }

        public enum GazeEyeCalibrationQuality
        {
            INVALID = 0,
            LOW = 1,
            MEDIUM = 2,
            HIGH = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeCalibrationQuality
        {
            public GazeEyeCalibrationQuality left;
            public GazeEyeCalibrationQuality right;
        }

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsGazeAllowed();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsGazeCalibrating();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsGazeCalibrated();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern GazeCalibrationQuality GetGazeCalibrationQuality();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern double GetIPDEstimate();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void RequestGazeCalibrationWithParameters(GazeCalibrationParameter[] parameters, int parametersCount);

        public static void RequestGazeCalibration(GazeCalibrationMode calibrationMode = GazeCalibrationMode.Fast, GazeOutputFilterMode outputFilterMode = GazeOutputFilterMode.Standard)
        {
            string calibrationModeString = "Fast";
            if (calibrationMode == GazeCalibrationMode.Legacy)
                calibrationModeString = "Legacy";

            string outputFilterModeString = "Standard";
            if (outputFilterMode == GazeOutputFilterMode.None)
                outputFilterModeString = "None";

            GazeCalibrationParameter[] parameters = new GazeCalibrationParameter[] {
                new GazeCalibrationParameter { key = "GazeCalibrationType", value = calibrationModeString },
                new GazeCalibrationParameter { key = "OutputFilterType", value = outputFilterModeString }};

            RequestGazeCalibrationWithParameters(parameters, 2);
        }
    }
}