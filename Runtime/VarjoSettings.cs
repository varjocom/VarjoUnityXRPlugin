
// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Varjo.XR
{
    [System.Serializable]
    [XRConfigurationData("Varjo", "Varjo.XR.Settings")]
    public class VarjoSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public VarjoStereoRenderingMode StereoRenderingMode = VarjoStereoRenderingMode.TwoPass;

        [SerializeField, Tooltip("Perform a separate culling pass for focus displays")]
        public bool SeparateCullPass = false;

        [SerializeField, Tooltip("Use foveated rendering")]
        public bool FoveatedRendering = true;

        [SerializeField, Tooltip("Scale factor applied to the context display resolution")]
        [Range(0.1f, 1.0f)]
        public float ContextScalingFactor = 1.0f;

        [SerializeField, Tooltip("Scale factor applied to the focus display resolution")]
        [Range(0.1f, 1.0f)]
        public float FocusScalingFactor = 1.0f;

        [SerializeField, Tooltip("The compositor should not perform alpha blending with the submitted frame")]
        public bool Opaque = true;

        [SerializeField, Tooltip("If enabled, the compositor disables warping for the layer. Use only if the camera is face-locked.")]
        public bool FaceLocked = false;

        [SerializeField, Tooltip("Flip the Y axis of the displays")]
        public bool FlipY = false;

        [SerializeField, Tooltip("Use occlusion mesh")]
        public bool OcclusionMesh = true;

        [SerializeField, Tooltip("Sessions with higher priority render on top of lower ones")]
        public int SessionPriority = 0;

        [SerializeField, Tooltip("Submit depth surfaces to the compositor")]
        public bool SubmitDepth = true;

        [SerializeField, Tooltip("Participate in depth sorting (Submit Depth should be enabled)")]
        public bool DepthSorting = false;

        [SerializeField, Tooltip("If true, the depth test range is enabled. Use Depth Test Near Z and Far Z to control the range inside which the depth test will be evaluated.")]
        public bool DepthTestRange = false;

        [SerializeField, Tooltip("Minimum depth included in the depth test range (meters)")]
        public double DepthTestNearZ = 0.0f;

        [SerializeField, Tooltip("Maximum depth included in the depth test range (meters)")]
        public double DepthTestFarZ = 1.0f;



#if !UNITY_EDITOR
        public static VarjoSettings s_Settings;

        public void Awake()
        {
            s_Settings = this;
        }
#endif
    }
}
