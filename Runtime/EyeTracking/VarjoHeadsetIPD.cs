using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public class VarjoHeadsetIPD
    {
        /// <summary>
        /// Interpupillary Distance (IPD) Adjustment Mode
        /// </summary>
        public enum IPDAdjustmentMode
        {
            Manual = 0,
            Automatic = 1
        }

        /// <summary>
        /// Get current headset interpupillary distance
        /// </summary>
        /// <returns>Current headset IPD or 0.0 if headset is not connected</returns>
        public static float GetDistance() { return Native.GetHeadsetIPD(); }

        /// <summary>
        /// Get interpupillary distance adjustment mode
        /// </summary>
        /// <returns>Current IPD adjustment mode.</returns>
        public static IPDAdjustmentMode GetAdjustmentMode()
        {
            string ipdAdjustmentModeValue = Native.GetHeadsetIPDAdjustmentMode();
            switch (ipdAdjustmentModeValue)
            {
                case "Manual": return IPDAdjustmentMode.Manual;
                default: return IPDAdjustmentMode.Automatic;
            }
        }

        /// <summary>
        /// Sets interpupillary distance (IPD) parameters for HMD.
        /// </summary>
        /// <param name="ipdAdjustmentMode">Interpupillary distance adjustment mode.</param>
        /// <param name="requestedPositionInMM">Requested interpupillary distance position in
        /// millimeters in manual adjustment mode.</param>
        /// <returns>True if interpupillary distance paramters were succesfully set</returns>
        public static bool SetInterPupillaryDistanceParameters(
            IPDAdjustmentMode ipdAdjustmentMode,
            float? requestedPositionInMM = null)
        {
            string ipdAdjustmentModeValue = "Automatic";
            switch (ipdAdjustmentMode)
            {
                case IPDAdjustmentMode.Manual:
                    ipdAdjustmentModeValue = "Manual";
                    break;
                default:
                    break;
            }

            var parameters = new List<Native.InterPupillaryDistanceParameter> {
                new Native.InterPupillaryDistanceParameter { key = "AdjustmentMode", value = ipdAdjustmentModeValue }
            };

            if (requestedPositionInMM.HasValue)
            {
                string requestedPositionValue = requestedPositionInMM.Value.ToString("F", CultureInfo.InvariantCulture);
                parameters.Add(new Native.InterPupillaryDistanceParameter { key = "RequestedPositionInMM", value = requestedPositionValue });
            }

            Native.SetInterPupillaryDistanceParameters(parameters.ToArray(), parameters.Count);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.BStr)]
            public static extern string GetHeadsetIPDAdjustmentMode();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern float GetHeadsetIPD();

            [StructLayout(LayoutKind.Sequential)]
            public struct InterPupillaryDistanceParameter
            {
                [MarshalAs(UnmanagedType.LPStr)] public string key;
                [MarshalAs(UnmanagedType.LPStr)] public string value;
            }

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetInterPupillaryDistanceParameters(InterPupillaryDistanceParameter[] parameters, int parametersCount);
        }
    }
}