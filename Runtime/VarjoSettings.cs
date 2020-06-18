
// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    [System.Serializable]
    [XRConfigurationData("Varjo", "Varjo.XR.Settings")]
    public class VarjoSettings : ScriptableObject
    {
        public enum StereoRenderingModes
        {
            MultiPass = 0,      // 4 passes, one for each display
            TwoPass,        // 2 passes in single-pass stereo rendering, one for context and one for focus displays
                            //            SinglePass,     // 1 pass, quad-wide, does not quite yet work due to Unity limitations
        }

        [SerializeField, Tooltip("Perform a separate culling pass for focus displays")]
        public bool separateCullPassForFocusDisplay = false;

        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public StereoRenderingModes stereoRenderingMode = StereoRenderingModes.TwoPass;

        [SerializeField, Tooltip("Scale factor applied to the context display resolution")]
        [Range(0.1f, 1.0f)]
        public float contextScalingFactor = 1.0f;

        [SerializeField, Tooltip("Scale factor applied to the focus display resolution")]
        [Range(0.1f, 1.0f)]
        public float focusScalingFactor = 1.0f;

        [SerializeField, Tooltip("Flip the Y axis of the displays")]
        public bool flipY = false;

        [SerializeField, Tooltip("Submit depth surfaces to the compositor")]
        public bool submitDepth = true;

        [SerializeField, Tooltip("The compositor should not perform alpha blending with the submitted frame")]
        public bool opaque = true;

        [SerializeField, Tooltip("Use occlusion mesh")]
        public bool useOcclusionMesh = true;

        [SerializeField, Tooltip("Participate in depth sorting (Submit Depth should be enabled)")]
        public bool depthSorting = false;

        [SerializeField, Tooltip("If true, the depth test range is enabled. Use Depth Test Near Z and Far Z to control the range inside which the depth test will be evaluated.")]
        public bool depthTestRangeEnabled = false;

        [SerializeField, Tooltip("Minimum depth included in the depth test range")]
        [Range(0.0f, 50.0f)]
        public double depthTestNearZ = 0.0f;

        [SerializeField, Tooltip("Maximum depth included in the depth test range")]
        [Range(0.0f, 50.0f)]
        public double depthTestFarZ = 50.0f;

        [SerializeField, Tooltip("Sessions with higher priority render on top of lower ones")]
        public int sessionPriority = 0;

#if !UNITY_EDITOR
        public static VarjoSettings s_Settings;

        public void Awake()
        {
            s_Settings = this;
            s_Settings.UpdateSettings();
        }
#endif
        // NOTE: Keep in sync with native code counterpart!
        [StructLayout(LayoutKind.Sequential)]
        private struct VarjoNativeSettings
        {
            public float contextScalingFactor;
            public float focusScalingFactor;
            public ushort separateCullPass;
            public ushort stereoRenderingMode;
            public ushort allowSinglePass;
            public ushort flipY;
            public ushort useTextureArrays;
            public ushort submitDepth;
            public ushort depthSorting;
            public ushort opaque;
            public ushort useOcclusionMesh;
            public double depthTestNearZ;
            public double depthTestFarZ;
            public ushort depthTestRangeEnabled;
            public int sessionPriority;
        }

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetVarjoNativeSettings(VarjoNativeSettings settings);

        public void UpdateSettings()
        {
            contextScalingFactor = Mathf.Clamp(contextScalingFactor, .1f, 1f);
            focusScalingFactor = Mathf.Clamp(focusScalingFactor, .1f, 1f);
            depthTestNearZ = (double)Mathf.Clamp((float)depthTestNearZ, 0.0f, (float)depthTestFarZ - Mathf.Epsilon);
            depthTestFarZ = (double)Mathf.Clamp((float)depthTestFarZ, (float)depthTestNearZ + Mathf.Epsilon, 50.0f);

            VarjoNativeSettings vns;
            vns.contextScalingFactor = contextScalingFactor;
            vns.focusScalingFactor = focusScalingFactor;
            vns.stereoRenderingMode = (ushort)stereoRenderingMode;
            vns.flipY = (ushort)(flipY ? 1 : 0);
            vns.submitDepth = (ushort)(submitDepth ? 1 : 0);
            vns.opaque = (ushort)(opaque ? 1 : 0);
            vns.useOcclusionMesh = (ushort)(useOcclusionMesh ? 1 : 0);
            vns.depthSorting = (ushort)(depthSorting ? 1 : 0);
            vns.allowSinglePass = 1;
            vns.depthTestNearZ = depthTestNearZ;
            vns.depthTestFarZ = depthTestFarZ;
            vns.depthTestRangeEnabled = (ushort)(depthTestRangeEnabled ? 1 : 0);
            vns.sessionPriority = sessionPriority;

            switch (stereoRenderingMode)
            {
                default:
                case StereoRenderingModes.MultiPass:
                    vns.separateCullPass = (ushort)(separateCullPassForFocusDisplay ? 1 : 0);
                    vns.useTextureArrays = 0;
                    break;
                case StereoRenderingModes.TwoPass:
                    vns.separateCullPass = (ushort)(separateCullPassForFocusDisplay ? 1 : 0);
                    vns.useTextureArrays = 1;
                    break;
                    /*                 case StereoRenderingModes.SinglePass:
                                        vns.separateCullPass = 0;
                                        vns.useTextureArrays = 1;
                                        break;
                    */
            }

            SetVarjoNativeSettings(vns);
        }

        void OnValidate()
        {
            UpdateSettings();
        }
    }
}
