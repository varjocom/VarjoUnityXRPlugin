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
        /// <summary>
        /// GazeRay struct contains data about eye position coordinates in meters [origin (x, y, z)]
        /// and a normalized direction vector [forward (x, y, z)].
        /// Gaze data is given in the left-hand coordinate system and is relative to head pose.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GazeRay
        {
            /** <summary>Eye position coordinates in meters [origin (x, y, z)].</summary> */
            public Vector3 origin;
            /** <summary>Normalized direction vector [forward (x, y, z)].</summary> */
            public Vector3 forward;
        }

        /// <summary>
        /// GazeStatus is a value for the eye tracking status of the headset
        /// </summary>
        public enum GazeStatus
        {
            /** <summary>Data unavailable: User is not wearing the headset or eyes cannot be located</summary> */
            Invalid = 0,
            /** <summary>User is wearing the headset, but gaze tracking is being calibrated</summary> */
            Adjust = 1,
            /** <summary>Data is valid</summary> */
            Valid = 2
        }

        public enum GazeEyeStatus
        {
            /** <summary>Eye is not tracked and not visible (e.g., the eye is shut).</summary> */
            Invalid = 0,
            /** <summary>Eye is visible but not reliably tracked (e.g., during a saccade or blink).</summary> */
            Visible = 1,
            /** <summary>Eye is tracked but quality is compromised (e.g., the headset has moved after calibration).</summary> */
            Compensated = 2,
            /** <summary>Eye is tracked.</summary> */
            Tracked = 3
        }

        /// <summary>
        /// GazeData contains eye tracking data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GazeData
        {
            /** <summary>A unique identifier of the frame at the time when the data was recorded.</summary> */
            public long frameNumber;
            /** <summary>A timestamp, in nanoseconds, of when the video frame was recorded by the eye tracking cameras.</summary> */
            public long captureTime;
            /** <summary>A status for eye tracking.</summary> */
            public GazeStatus status;
            /** <summary>Gaze ray combined from both eyes.</summary> */
            public GazeRay gaze;
            /** <summary>The distance between eye and focus point in meters. Values are between 0 and 2 meters.</summary> */
            public float focusDistance;
            /** <summary>Stability of the user’s focus. Value are between 0.0 and 1.0, where 0.0 indicates least stable focus and 1.0 most stable.</summary> */
            public float focusStability;
            /** <summary>A status for the left eye.</summary> */
            public GazeEyeStatus leftStatus;
            /** <summary>Gaze ray for the left eye.</summary> */
            public GazeRay left;
            /** <summary>Pupil size for the left eye, calculated according to the pupil size range detected by the headset. Values are between 0 and 1.</summary> */
            public float leftPupilSize;
            /** <summary>A status for the right eye.</summary> */
            public GazeEyeStatus rightStatus;
            /** <summary>Gaze ray for the right eye.</summary> */
            public GazeRay right;
            /** <summary>Pupil size for the right eye, calculated according to the pupil size range detected by the headset. Values are between 0 and 1.</summary> */
            public float rightPupilSize;
        }

        /// <summary>
        /// Gaze Calibration Mode
        /// </summary>
        public enum GazeCalibrationMode
        {
            Legacy,
            Fast
        }

        /// <summary>
        /// Gaze OutputFilter Type
        /// </summary>
        public enum GazeOutputFilterType
        {
            None,
            Standard
        }

        /// <summary>
        /// Gaze Output Frequency
        /// </summary>
        public enum GazeOutputFrequency
        {
            MaximumSupported,
            Frequency100Hz,
            Frequency200Hz
        }

        /// <summary>
        /// Gaze Eye Calibration Quality
        /// </summary>
        public enum GazeEyeCalibrationQuality
        {
            Invalid = 0,
            Low = 1,
            Medium = 2,
            High = 3
        }

        /// <summary>
        /// Gaze Calibration Quality
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GazeCalibrationQuality
        {
            public GazeEyeCalibrationQuality left;
            public GazeEyeCalibrationQuality right;
        }

        /// <summary>
        /// Is the HMD gaze tracker allowed.
        /// </summary>
        /// <returns>True if gaze is allowed to be used, False otherwise</returns>
        public static bool IsGazeAllowed() { return Native.IsGazeAllowed(); }

        /// <summary>
        /// Is Gaze Available.
        /// </summary>
        /// <returns>True if available.</returns>
        public static bool IsGazeAvailable() { return Native.IsGazeAvailable(); }

        /// <summary>
        /// Is system currently calibrating the HMD gaze tracker.
        /// </summary>
        public static bool IsGazeCalibrating() { return Native.IsGazeCalibrating(); }

        /// <summary>
        /// Is the HMD gaze tracker calibrated.
        /// </summary>
        public static bool IsGazeCalibrated() { return Native.IsGazeCalibrated(); }

        /// <summary>
        /// Get Gaze Calibration Quality.
        /// </summary>
        /// <returns>Gaze Calibration Quality.</returns>
        public static GazeCalibrationQuality GetGazeCalibrationQuality() { return Native.GetGazeCalibrationQuality(); }

        /// <summary>
        /// Get IPD distance estimate in millimeters.
        /// </summary>
        /// <returns>IPD distance estimate in millimeters.</returns>
        public static double GetIPDEstimate() { return Native.GetIPDEstimate(); }

        /// <summary>
        /// Get Gaze Output FilterType;
        /// </summary>
        /// <returns>Gaze Output Filter Type</returns>
        public static GazeOutputFilterType GetGazeOutputFilterType() { return Native.GetGazeOutputFilterType(); }

        /// <summary>
        /// Sets GazeOutputFilterType.
        /// </summary>
        public static bool SetGazeOutputFilterType(GazeOutputFilterType outputFilterType)
        {
            Native.SetGazeOutputFilterType(outputFilterType);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Get Gaze Output Frequency.
        /// </summary>
        /// <returns>Gaze Output Frequency.</returns>
        public static GazeOutputFrequency GetGazeOutputFrequency() { return Native.GetGazeOutputFrequency(); }

        /// <summary>
        /// Set Gaze Output Frequency.
        /// </summary>
        /// <param name="outputFrequency">Gaze Output Frequency</param>
        public static bool SetGazeOutputFrequency(GazeOutputFrequency outputFrequency)
        {
            Native.SetGazeOutputFrequency(outputFrequency);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Returns the latest gaze data frame.
        /// </summary>
        /// <returns>The latest gaze data frame.</returns>
        public static GazeData GetGaze() { return Native.GetGaze(); }

        /// <summary>
        /// Requests a HMD gaze calibration with provided parameters.
        /// </summary>
        /// <remarks>
        /// This attempts to trigger the gaze calibration sequence if the user has
        /// allowed gaze tracking from Varjo settings and Varjo system is in a state
        /// where it can bring up the calibration UI.
        /// </remarks>
        /// <param name="calibrationMode">Gaze calibration mode.</param>
        /// <returns>True if gaze calibration was succesfully requested</returns>
        public static bool RequestGazeCalibration(GazeCalibrationMode calibrationMode = GazeCalibrationMode.Fast)
        {
            string calibrationModeValue = "Fast";
            if (calibrationMode == GazeCalibrationMode.Legacy)
                calibrationModeValue = "Legacy";

            Native.GazeCalibrationParameter[] parameters = new Native.GazeCalibrationParameter[] {
                new Native.GazeCalibrationParameter { key = "GazeCalibrationType", value = calibrationModeValue } };

            Native.RequestGazeCalibrationWithParameters(parameters, parameters.Length);
            return VarjoError.CheckError();
        }

        [ObsoleteAttribute("Use GazeOutputFilterType instead.", false)]
        public enum GazeOutputFilterMode
        {
            Standard,
            None
        }

        [ObsoleteAttribute("Use RequestGazeCalibration(GazeCalibrationMode calibrationMode) instead and set the output filter mode with SetOutputFilterMode(GazeOutputFilterMode outputFilterMode).", false)]
        public static bool RequestGazeCalibration(GazeCalibrationMode calibrationMode, GazeOutputFilterMode outputFilterMode)
        {
            string calibrationModeValue = "Fast";
            if (calibrationMode == GazeCalibrationMode.Legacy)
                calibrationModeValue = "Legacy";

            string outputFilterModeValue = "Standard";
            if (outputFilterMode == GazeOutputFilterMode.None)
                outputFilterModeValue = "None";

            Native.GazeCalibrationParameter[] parameters = new Native.GazeCalibrationParameter[] {
                new Native.GazeCalibrationParameter { key = "GazeCalibrationType", value = calibrationModeValue },
                new Native.GazeCalibrationParameter { key = "OutputFilterType", value = outputFilterModeValue }};

            Native.RequestGazeCalibrationWithParameters(parameters, parameters.Length);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Get Gaze List.
        /// </summary>
        /// <param name="gazeData">Gaze Data list to fill.</param>
        /// <returns>Size of gazeData list</returns>
        public static int GetGazeList(out List<GazeData> gazeData)
        {
            int gazeDataCount = Native.FetchGazeData();
            if (gazeDataCount == 0)
            {
                VarjoError.CheckError();
                gazeData = new List<GazeData>();
                return 0;
            }

            GazeData[] gazeDataArray = new GazeData[gazeDataCount];
            Native.GetGazeArray(gazeDataArray, gazeDataCount);
            gazeData = gazeDataArray.ToList();
            return gazeDataCount;
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
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
            public static extern GazeOutputFilterType GetGazeOutputFilterType();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto, EntryPoint = "SetGazeOutputFilterType")]
            public static extern bool SetGazeOutputFilterType(GazeOutputFilterType outputFilterType);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern GazeOutputFrequency GetGazeOutputFrequency();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto, EntryPoint = "SetGazeOutputFrequency")]
            public static extern bool SetGazeOutputFrequency(GazeOutputFrequency outputFrequency);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern GazeData GetGaze();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int FetchGazeData();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool GetGazeArray(GazeData[] gazeData, int gazeDataCount);

            [StructLayout(LayoutKind.Sequential)]
            public struct GazeCalibrationParameter
            {
                [MarshalAs(UnmanagedType.LPStr)] public string key;
                [MarshalAs(UnmanagedType.LPStr)] public string value;
            }

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void RequestGazeCalibrationWithParameters(GazeCalibrationParameter[] parameters, int parametersCount);
        }
    }
}