using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Varjo.XR;

namespace VarjoExample
{
    public class MixedRealityFeatureExample : MonoBehaviour
    {
        //[Serializable]
        //public class CubemapEvent : UnityEvent { }

        [SerializeField] Camera xrCamera;

        [Header("Keys")]
        [SerializeField] Key depthOcclusionToggleKey = Key.Digit1;
        [SerializeField] Key realTimeReflectionToggleKey = Key.Digit2;
        [SerializeField] Key VREyeOffsetToggleKey = Key.Digit3;

        public bool MixedRealityEnabled { get; private set; }
        public bool DepthOcclusionEnabled { get; private set; }
        public bool EnvironmentReflectionsEnabled { get; private set; }
        public float VREyeOffset { get; private set; }

        [Header("Depth occlusion distance (set on toggle)")]
        [Range(0.0f, 7.0f)]
        [SerializeField] float depthOcclusionDistance = 0.75f;

        private bool originalSubmitDepthValue = false;
        private bool originalDepthSortingValue = false;
        private bool originalDepthTestingValue = false;

        void OnDisable()
        {
            SetDepthOcclusion(false);
            SetEnvironmentReflections(false);
        }

        void Update()
        {
            UpdateToggles();
            UpdateEnvironmentReflections();
        }

        void UpdateToggles()
        {
            if (Keyboard.current[depthOcclusionToggleKey].wasPressedThisFrame)
            {
                SetDepthOcclusion(!DepthOcclusionEnabled, depthOcclusionDistance);
            }
            else if (Keyboard.current[realTimeReflectionToggleKey].wasPressedThisFrame)
            {
                SetEnvironmentReflections(!EnvironmentReflectionsEnabled);
            }
            else if (Keyboard.current[VREyeOffsetToggleKey].wasPressedThisFrame)
            {
                if (Mathf.Approximately(VREyeOffset, 1.0f))
                {
                    SetVREyeOffset(0.0f);
                }
                else
                {
                    SetVREyeOffset(1.0f);
                }
            }
        }

        void UpdateEnvironmentReflections()
        {
            if (EnvironmentReflectionsEnabled)
            {
                if (VarjoMixedReality.environmentCubemapStream.hasNewFrame)
                {
                    RenderSettings.customReflectionTexture = VarjoMixedReality.environmentCubemapStream.GetFrame().cubemap;
                }
            }
        }

        public void SetDepthOcclusion(bool enabled, float distance = 0.75f)
        {
            DepthOcclusionEnabled = enabled;
            if (DepthOcclusionEnabled)
            {
                DepthOcclusionEnabled = VarjoMixedReality.EnableDepthEstimation();
                if(DepthOcclusionEnabled)
                {
                    originalSubmitDepthValue = VarjoRendering.GetSubmitDepth();
                    originalDepthSortingValue = VarjoRendering.GetDepthSorting();
                    originalDepthTestingValue = VarjoRendering.GetDepthTestRangeEnabled();
                    VarjoRendering.SetSubmitDepth(true);
                    VarjoRendering.SetDepthSorting(false);
                    VarjoRendering.SetDepthTestRangeEnabled(true);
                    VarjoRendering.SetDepthTestNearZ(0.0f);
                    VarjoRendering.SetDepthTestFarZ(distance);
                    Debug.Log("Varjo Depth Occlusion Disance: " + distance, gameObject);
                }
            }
            else
            {
                VarjoMixedReality.DisableDepthEstimation();
                VarjoRendering.SetSubmitDepth(originalSubmitDepthValue);
                VarjoRendering.SetDepthSorting(originalDepthSortingValue);
                VarjoRendering.SetDepthTestRangeEnabled(originalDepthTestingValue);
                DepthOcclusionEnabled = false;
            }
            Debug.Log("Varjo Depth Occlusion: " + DepthOcclusionEnabled, gameObject);
        }

        public void SetEnvironmentReflections(bool enabled)
        {
            EnvironmentReflectionsEnabled = enabled;
            if (EnvironmentReflectionsEnabled)
            {
                if (VarjoMixedReality.environmentCubemapStream.IsSupported())
                {
                    EnvironmentReflectionsEnabled = VarjoMixedReality.environmentCubemapStream.Start();
                }
                VarjoMixedReality.SetEnvironmentCubemapMode(VarjoEnvironmentCubemapMode.AutoAdapt);
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            }
            else
            {
                VarjoMixedReality.environmentCubemapStream.Stop();
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            }

            Debug.Log("Varjo Environment Reflections: " + EnvironmentReflectionsEnabled, gameObject);
        }

        public void SetVREyeOffset(float newOffset)
        {
            newOffset = Mathf.Clamp01(newOffset);
            VarjoMixedReality.SetVRViewOffset(newOffset);
            VREyeOffset = newOffset;
            Debug.Log("Varjo View Offset: " + VREyeOffset, gameObject);
        }
    }
}