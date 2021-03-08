using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Varjo.XR
{
    public class VarjoEyeTracking
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct GazeRay
        {
            public Vector3 origin;
            public Vector3 forward;
        }

        public enum GazeStatus
        {
            Invalid = 0,
            Adjust = 1,
            Valid = 2
        }

        public enum GazeEyeStatus
        {
            Invalid = 0,
            Visible = 1,
            Compensated = 2,
            Tracked = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeData
        {
            public long frameNumber;
            public long captureTime;
            public GazeStatus status;
            public GazeRay gaze;
            public float focusDistance;
            public float focusStability;
            public GazeEyeStatus leftStatus;
            public GazeRay left;
            public float leftPupilSize;
            public GazeEyeStatus rightStatus;
            public GazeRay right;
            public float rightPupilSize;
        }

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

        public enum GazeOutputFilterType
        {
            None,
            Standard
        }

        public enum GazeOutputFrequency
        {
            MaximumSupported,
            Frequency100Hz,
            Frequency200Hz
        }

        public enum GazeEyeCalibrationQuality
        {
            Invalid = 0,
            Low = 1,
            Medium = 2,
            High = 3
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
        public static extern bool IsGazeAvailable();

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

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool SetGazeOutputFilterType(GazeOutputFilterType outputFilterType);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern GazeOutputFilterType GetGazeOutputFilterType();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool SetGazeOutputFrequency(GazeOutputFrequency outputFrequency);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern GazeOutputFrequency GetGazeOutputFrequency();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern int FetchGazeData();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern bool GetGazeArray(GazeData[] gazeData, int gazeDataCount);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern GazeData GetGaze();

        public static void RequestGazeCalibration(GazeCalibrationMode calibrationMode = GazeCalibrationMode.Fast)
        {
            string calibrationModeValue = "Fast";
            if (calibrationMode == GazeCalibrationMode.Legacy)
                calibrationModeValue = "Legacy";

            GazeCalibrationParameter[] parameters = new GazeCalibrationParameter[] {
                new GazeCalibrationParameter { key = "GazeCalibrationType", value = calibrationModeValue } };

            RequestGazeCalibrationWithParameters(parameters, 1);
        }

        [ObsoleteAttribute("Use GazeOutputFilterType instead.", false)]
        public enum GazeOutputFilterMode
        {
            Standard,
            None
        }

        [ObsoleteAttribute("Use RequestGazeCalibration(GazeCalibrationMode calibrationMode) instead and set the output filter mode with SetOutputFilterMode(GazeOutputFilterMode outputFilterMode).", false)]
        public static void RequestGazeCalibration(GazeCalibrationMode calibrationMode, GazeOutputFilterMode outputFilterMode)
        {
            string calibrationModeValue = "Fast";
            if (calibrationMode == GazeCalibrationMode.Legacy)
                calibrationModeValue = "Legacy";

            string outputFilterModeValue = "Standard";
            if (outputFilterMode == GazeOutputFilterMode.None)
                outputFilterModeValue = "None";

            GazeCalibrationParameter[] parameters = new GazeCalibrationParameter[] {
                new GazeCalibrationParameter { key = "GazeCalibrationType", value = calibrationModeValue },
                new GazeCalibrationParameter { key = "OutputFilterType", value = outputFilterModeValue }};

            RequestGazeCalibrationWithParameters(parameters, 2);
        }

        public static int GetGazeList(out List<GazeData> gazeData)
        {
            int gazeDataCount = FetchGazeData();
            GazeData[] gazeDataArray = new GazeData[gazeDataCount];
            if (gazeDataCount > 0)
            {
                GetGazeArray(gazeDataArray, gazeDataCount);
            }
            gazeData = gazeDataArray.ToList();
            return gazeDataCount;
        }
    }
}