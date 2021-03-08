using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public enum VarjoStereoRenderingMode
    {
        MultiPass = 0,          // 4 passes, one for each display
        TwoPass = 1,            // 2 passes in single-pass stereo rendering, one for context and one for focus displays
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

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetRenderSettingFloatValue(VarjoSettingID setting, float value);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern float GetRenderSettingFloatValue(VarjoSettingID setting);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetRenderSettingIntValue(VarjoSettingID setting, int value);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern int GetRenderSettingIntValue(VarjoSettingID setting);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetRenderSettingBoolValue(VarjoSettingID setting, bool value);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern bool GetRenderSettingBoolValue(VarjoSettingID setting);


        public static void SetStereoRenderingMode(VarjoStereoRenderingMode value)
        {
            SetRenderSettingIntValue(VarjoSettingID.StereoRenderingMode, (int)value);
        }

        public static VarjoStereoRenderingMode GetStereoRenderingMode()
        {
            return (VarjoStereoRenderingMode)GetRenderSettingIntValue(VarjoSettingID.StereoRenderingMode);
        }

        public static void SetSeparateCullPass(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.SeparateCullPass, value);
        }

        public static bool GetSeparateCullPass()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.SeparateCullPass);
        }

        public static void SetFoveatedRendering(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.FoveatedRendering, value);
        }

        public static bool GetFoveatedRendering()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.FoveatedRendering);
        }

        public static void SetContextScalingFactor(float value)
        {
            SetRenderSettingFloatValue(VarjoSettingID.ContextScalingFactor, value);
        }

        public static float GetContextScalingFactor()
        {
            return GetRenderSettingFloatValue(VarjoSettingID.ContextScalingFactor);
        }

        public static void SetFocusScalingFactor(float value)
        {
            SetRenderSettingFloatValue(VarjoSettingID.FocusScalingFactor, value);
        }

        public static float GetFocusScalingFactor()
        {
            return GetRenderSettingFloatValue(VarjoSettingID.FocusScalingFactor);
        }

        public static void SetOpaque(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.Opaque, value);
        }

        public static bool GetOpaque()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.Opaque);
        }

        public static void SetFaceLocked(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.FaceLocked, value);
        }

        public static bool GetFaceLocked()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.FaceLocked);
        }

        public static void SetFlipY(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.FlipY, value);
        }

        public static bool GetFlipY()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.FlipY);
        }

        public static void SetOcclusionMeshEnabled(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.OcclusionMesh, value);
        }

        public static bool GetOcclusionMeshEnabled()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.OcclusionMesh);
        }

        public static void SetSessionPriority(int value)
        {
            SetRenderSettingIntValue(VarjoSettingID.SessionPriority, value);
        }

        public static int GetSessionPriority()
        {
            return GetRenderSettingIntValue(VarjoSettingID.SessionPriority);
        }

        public static void SetSubmitDepth(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.SubmitDepth, value);
        }

        public static bool GetSubmitDepth()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.SubmitDepth);
        }

        public static void SetDepthSorting(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.DepthSorting, value);
        }

        public static bool GetDepthSorting()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.DepthSorting);
        }

        public static void SetDepthTestRangeEnabled(bool value)
        {
            SetRenderSettingBoolValue(VarjoSettingID.DepthTestRange, value);
        }

        public static bool GetDepthTestRangeEnabled()
        {
            return GetRenderSettingBoolValue(VarjoSettingID.DepthTestRange);
        }

        public static void SetDepthTestNearZ(float value)
        {
            SetRenderSettingFloatValue(VarjoSettingID.DepthTestNearZ, value);
        }

        public static float GetDepthTestNearZ()
        {
            return GetRenderSettingFloatValue(VarjoSettingID.DepthTestNearZ);
        }

        public static void SetDepthTestFarZ(float value)
        {
            SetRenderSettingFloatValue(VarjoSettingID.DepthTestFarZ, value);
        }

        public static float GetDepthTestFarZ()
        {
            return GetRenderSettingFloatValue(VarjoSettingID.DepthTestFarZ);
        }
    }
}