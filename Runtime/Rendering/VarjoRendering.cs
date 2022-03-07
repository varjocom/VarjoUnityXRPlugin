using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    /// <summary>
    /// The stereo rendering mode.
    /// </summary>
    public enum VarjoStereoRenderingMode
    {
        /** <summary>The scene is rendered in four separate passes: one for each view (left context, right context, left focus, right focus).</summary> */
        MultiPass = 0,
        /** <summary>The scene is rendered in two passes: one two-wide instanced stereo rendering pass for the context displays and another for the focus displays.</summary> */
        TwoPass = 1,
        /** <summary>The scene is rendered in one two-wide instanced stereo rendering pass for the context displays. Focus views are not rendered and foveated rendering is disabled.</summary> */
        Stereo = 3
    }

    public class VarjoRendering
    {
        private enum VarjoSettingID
        {
            StereoRenderingMode = 0,
            SeparateCullPass = 1,
            FoveatedRendering = 2,
            ContextScalingFactor = 3,
            FocusScalingFactor = 4,
            Opaque = 5,
            FaceLocked = 6,
            FlipY = 7,
            OcclusionMesh = 8,
            SessionPriority = 9,
            SubmitDepth = 10,
            DepthSorting = 11,
            DepthTestRange = 12,
            DepthTestNearZ = 13,
            DepthTestFarZ = 14,
        }

        /// <summary>
        /// Set Stereo rendering mode.
        /// </summary>
        /// <param name="value">Stereo rendering mode to set.</param>
        public static void SetStereoRenderingMode(VarjoStereoRenderingMode value)
        {
            Native.SetRenderSettingIntValue(VarjoSettingID.StereoRenderingMode, (int)value);
        }

        /// <summary>
        /// Get Stereo rendering mode.
        /// </summary>
        /// <returns>Stereo rendering mode.</returns>
        public static VarjoStereoRenderingMode GetStereoRenderingMode()
        {
            return (VarjoStereoRenderingMode)Native.GetRenderSettingIntValue(VarjoSettingID.StereoRenderingMode);
        }

        /// <summary>
        /// Set Separate Cull Pass For Focus Displays.
        /// <para>
        /// When enabled, Unity will perform an extra culling pass for the focus displays.
        /// Otherwise the culling results for the context displays will be reused for the focus displays (the default and recommended setting).
        /// </para>
        /// Default: false
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetSeparateCullPass(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.SeparateCullPass, value);
        }

        /// <summary>
        /// Get Set Separate Cull Pass For Focus Displays.
        /// <para>See remarks for <see cref="SetSeparateCullPass(bool)"/>.</para>
        /// </summary>
        public static bool GetSeparateCullPass()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.SeparateCullPass);
        }

        /// <summary>
        /// Set Foveated Rendering.
        /// <para>
        /// When enabled, foveated rendering is enabled for the application.
        /// </para>
        /// <para>
        /// Default: true
        /// </para>
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetFoveatedRendering(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.FoveatedRendering, value);
        }

        /// <summary>
        /// <para>Get Foveated Rendering.</para>
        /// See remarks for <see cref="SetFoveatedRendering(bool)"/>.
        /// </summary>
        /// <returns></returns>
        public static bool GetFoveatedRendering()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.FoveatedRendering);
        }

        /// <summary>
        /// <para>Set Context Scaling Factor.</para>
        /// <para>A scaling factor that can be used to reduce the rendering resolution for the context display surfaces.</para>
        /// Range: 0.1-1.0 Default: 1.0
        ///</summary>
        ///<param name="value">Scaling factor (in range 0.1-1.0).</param>
        /// <returns>True if value was set successfully.</returns>
        public static bool SetContextScalingFactor(float value)
        {
            Native.SetRenderSettingFloatValue(VarjoSettingID.ContextScalingFactor, value);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Get Context Scaling Factor.
        /// </summary>
        /// <returns>Context Scaling Factor.</returns>
        public static float GetContextScalingFactor()
        {
            return Native.GetRenderSettingFloatValue(VarjoSettingID.ContextScalingFactor);
        }

        /// <summary>
        /// Set Focus Scaling Factor.
        /// <para>A scaling factor that can be used to reduce the rendering resolution for the focus display surfaces.</para>
        /// <para>Range: 0.1-1.0 Default: 1.0</para>
        /// </summary>
        /// <param name="value">Focus Scaling Factor (in range 0.1-1.0).</param>
        /// <returns>True if value was set successfully.</returns>
        public static bool SetFocusScalingFactor(float value)
        {
            Native.SetRenderSettingFloatValue(VarjoSettingID.FocusScalingFactor, value);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// <para>Get Focus Scaling Factor.</para>
        /// See remarks for <see cref="SetFocusScalingFactor(float)"/>
        /// </summary>
        /// <returns>Focus Scaling Factor.</returns>
        public static float GetFocusScalingFactor()
        {
            return Native.GetRenderSettingFloatValue(VarjoSettingID.FocusScalingFactor);
        }

        /// <summary>
        /// <para>Set Opaque.</para>
        /// <para>Used to tell the Varjo Compositor that the submitted surfaces are meant to be opaque, and that the compositor
        /// should not perform any alpha blending against possible background applications when rendering the surfaces.</para>
        /// Default: true
        /// </summary>
        /// <param name="value">Opaque.</param>
        public static void SetOpaque(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.Opaque, value);
        }

        /// <summary>
        /// Get Opaque.
        /// </summary>
        /// <returns>Opaque.</returns>
        public static bool GetOpaque()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.Opaque);
        }

        /// <summary>
        /// <para>Set Face-locked</para>
        /// <para>When enabled, the compositor disables warping for the layer. Use only if the camera is face-locked.</para>
        /// Default: false
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetFaceLocked(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.FaceLocked, value);
        }

        /// <summary>
        /// Get Face-locked.
        /// </summary>
        /// <returns>Face-locked.</returns>
        public static bool GetFaceLocked()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.FaceLocked);
        }

        /// <summary>
        /// <para>Set Flip Y.</para>
        /// <para>When enabled, the rendering results are flipped upside down before they are submitted to the compositor.
        /// Enable this if your scene appears upside down in the HMD.</para>
        /// Daefault: false
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetFlipY(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.FlipY, value);
        }

        /// <summary>
        /// Get Flip Y.
        /// </summary>
        /// <returns>Flip Y.</returns>
        public static bool GetFlipY()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.FlipY);
        }

        /// <summary>
        /// Set Occlusion Mesh Enabled.
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetOcclusionMeshEnabled(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.OcclusionMesh, value);
        }

        /// <summary>
        /// Get Occlusion Mesh Enabled.
        /// </summary>
        /// <returns>Occlusion Mesh Enabled.</returns>
        public static bool GetOcclusionMeshEnabled()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.OcclusionMesh);
        }

        /// <summary>
        /// <para>Set Session Priority.</para>
        /// <para>Used when multiple clients are running at the same time.
        /// Sessions with higher priority are rendered on top of lower ones.</para>
        /// Default: 0
        /// </summary>
        /// <param name="value"></param>
        public static void SetSessionPriority(int value)
        {
            Native.SetRenderSettingIntValue(VarjoSettingID.SessionPriority, value);
        }

        /// <summary>
        /// Get Session Priority.
        /// </summary>
        /// <returns>Session Priority.</returns>
        public static int GetSessionPriority()
        {
            return Native.GetRenderSettingIntValue(VarjoSettingID.SessionPriority);
        }

        /// <summary>
        /// <para>Set Submit Depth.</para>
        /// <para>When enabled, the application will pass depth surfaces to the compositor (alongside color surfaces).
        /// This allows the compositor to use Positional TimeWarp to improve rendering quality.</para>
        /// Default: true
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetSubmitDepth(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.SubmitDepth, value);
        }

        /// <summary>
        /// Get Submit Depth.
        /// </summary>
        /// <returns>Value.</returns>
        public static bool GetSubmitDepth()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.SubmitDepth);
        }

        /// <summary>
        /// <para>Set Depth Sorting.</para>
        /// <para>Used to tell the Varjo Compositor that the application wants its contents to participate in per-pixel depth sorting.
        /// With depth sorting, if other applications (or the video pass-through view in XR headsets) have pixels closer to the camera than this application,
        /// those pixels are displayed instead of pixels from this application. When this option is in use, Submit Depth should be enabled as well.</para>
        /// Default: false
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetDepthSorting(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.DepthSorting, value);
        }

        /// <summary>
        /// Get Depth Sorting.
        /// </summary>
        /// <returns>Depth Sorting</returns>
        public static bool GetDepthSorting()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.DepthSorting);
        }

        /// <summary>
        /// <para>Set Depth Test Range Enabled</para>
        /// <para>Enable the depth test range. Use Depth Test Near Z and Depth Test Far Z to control the range for which the depth test is evaluated.</para>
        /// Default: false
        /// </summary>
        /// <param name="value">Value.</param>
        public static void SetDepthTestRangeEnabled(bool value)
        {
            Native.SetRenderSettingBoolValue(VarjoSettingID.DepthTestRange, value);
        }

        /// <summary>
        /// Get Depth Test Range Enabled.
        /// </summary>
        /// <returns>Value.</returns>
        public static bool GetDepthTestRangeEnabled()
        {
            return Native.GetRenderSettingBoolValue(VarjoSettingID.DepthTestRange);
        }

        /// <summary>
        /// <para>Set Depth Test Near Z.</para>
        /// <para>The minimum depth included in the depth test range (in meters).</para>
        /// <para>Range: 0.0-50.0 Default: 0.0</para>
        /// </summary>
        /// <param name="value">Depth Test Near Z (range: 0.0-50.0).</param>
        /// <returns>True if value was set successfully.</returns>
        public static bool SetDepthTestNearZ(float value)
        {
            Native.SetRenderSettingFloatValue(VarjoSettingID.DepthTestNearZ, value);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Get Depth Test Near Z.
        /// </summary>
        /// <returns>Value.</returns>
        public static float GetDepthTestNearZ()
        {
            return Native.GetRenderSettingFloatValue(VarjoSettingID.DepthTestNearZ);
        }

        /// <summary>
        /// Set Depth Test Far Z.
        /// <para>The maximum depth included in the depth test range (in meters).</para>
        /// Range: 0.0-50.0 Default: 1.0
        /// </summary>
        /// <param name="value">Depth Test Far Z (range: 0.0-50.0).</param>
        /// <returns>True if value was set successfully.</returns>
        public static bool SetDepthTestFarZ(float value)
        {
            Native.SetRenderSettingFloatValue(VarjoSettingID.DepthTestFarZ, value);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Get Depth Test Far Z.
        /// </summary>
        /// <returns>Depth Test Far Z.</returns>
        public static float GetDepthTestFarZ()
        {
            return Native.GetRenderSettingFloatValue(VarjoSettingID.DepthTestFarZ);
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetRenderSettingFloatValue(VarjoSettingID setting, float value);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern float GetRenderSettingFloatValue(VarjoSettingID setting);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetRenderSettingIntValue(VarjoSettingID setting, int value);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetRenderSettingIntValue(VarjoSettingID setting);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetRenderSettingBoolValue(VarjoSettingID setting, bool value);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool GetRenderSettingBoolValue(VarjoSettingID setting);
        }
    }
}