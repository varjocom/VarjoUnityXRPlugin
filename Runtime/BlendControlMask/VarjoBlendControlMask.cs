using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Masking Mode
    /// </summary>
    public enum VarjoMaskingMode
    {
        /** <summary>No masking. Masking layer is not added to composited scenes. Regular rendering is enabled for this application.</summary> */
        Off = 0,
        /** <summary>Use chroma keying in masked area (or plain VST if not active), elsewhere show VR content. Disables regular rendering for this application.</summary> */
        Restricted = 1,
        /** <summary>Show VR content in masked area, elsewhere use chroma keying (or plain VST if not active). Disables regular rendering for this application.</summary> */
        Extended = 2,
        /** <summary>Show VST content in masked area, elsewhere use chroma keying (or plain VST if not active). Disables regular rendering for this application.</summary> */
        Reduced = 3,
        /** <summary>Do VST depth test in masked area, elsewhere always fail depth test (show VST). Disables regular rendering for this application.</summary> */
        DepthTestOrAlwaysFail = 4,
        /** <summary>Do VST depth test in masked area, elsewhere always pass depth test (show VR). Normal rendering is disabled.</summary> */
        DepthTestOrAlwaysPass = 5
    }

    /// <summary>
    /// Varjo Blend Control Mask allows a client to submit a masking layer that is used to control the blending between video see-through and application layer images.
    /// As Varjo XR plugin currently supports submitting only one layer per client, enabling blend control mask disables regular rendering for the application.
    /// Varjo Blend Control Mask is intended for multi-app cases where one client controls the masking layer and separate client handles the rendering.
    //  Blend Control Mask is only supported on D3D11. 
    /// </summary>
    public class VarjoBlendControlMask
    {
        private enum VarjoMaskingSettingID
        {
            MaskingMode = 25,
            MaskingDebugMode = 26,
            MaskingForceGlobalViewOffset = 27,
            MaskingFrameSkip = 28,
        }

        /// <summary>
        /// Is Blend Control Mask supported.
        /// </summary>
        /// <returns>True if blend control mask is supported with the current system.</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsBlendControlMaskSupported();

        /// <summary>
        /// Set Masking mode.
        /// </summary>
        /// <param name="value">Masking mode to set.</param>
        public static void SetMaskingMode(VarjoMaskingMode value)
        {
            Native.SetRenderSettingIntValue(VarjoMaskingSettingID.MaskingMode, (int)value);
        }

        /// <summary>
        /// Get Masking mode.
        /// </summary>
        /// <returns>Masking mode.</returns>
        public static VarjoMaskingMode GetMaskingMode()
        {
            return (VarjoMaskingMode)Native.GetRenderSettingIntValue(VarjoMaskingSettingID.MaskingMode);
        }

        /// <summary>
        /// Set Masking Debug Mode Enabled. If enabled, mask alpha value is visualized.
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetMaskingDebugModeEnabled(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoMaskingSettingID.MaskingDebugMode, value);
        }

        /// <summary>
        /// Get Masking Debug Mode Enabled.
        /// </summary>
        /// <returns>Masking Debug Mode Enabled.</returns>
        public static bool GetMaskingDebugModeEnabled()
        {
            return Native.GetRenderSettingBoolValue(VarjoMaskingSettingID.MaskingDebugMode);
        }

        /// <summary>
        /// Set Masking Frame Skip. When masking is enabled, skips the set amount of frames between mask layer updates.
        /// </summary>
        /// <param name="value"></param>
        public static void SetMaskingFrameSkip(int value)
        {
            Native.SetRenderSettingIntValue(VarjoMaskingSettingID.MaskingFrameSkip, value);
        }

        /// <summary>
        /// Get Masking Frame Skip.
        /// </summary>
        /// <returns>Masking Frame Skip.</returns>
        public static int GetMaskingFrameSkip()
        {
            return Native.GetRenderSettingIntValue(VarjoMaskingSettingID.MaskingFrameSkip);
        }

        /// <summary>
        /// Set Force Global View Offset Enabled.
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetForceGlobalViewOffsetEnabled(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoMaskingSettingID.MaskingForceGlobalViewOffset, value);
        }

        /// <summary>
        /// Get Force Global View Offset Enabled.
        /// </summary>
        public static bool GetForceGlobalViewOffsetEnabled()
        {
            return Native.GetRenderSettingBoolValue(VarjoMaskingSettingID.MaskingForceGlobalViewOffset);
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetRenderSettingIntValue(VarjoMaskingSettingID setting, int value);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetRenderSettingIntValue(VarjoMaskingSettingID setting);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetRenderSettingBoolValue(VarjoMaskingSettingID setting, bool value);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool GetRenderSettingBoolValue(VarjoMaskingSettingID setting);
        }
    }
}