using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.XR.Management;
using Varjo.XR;

public class MixedRealityExample : MonoBehaviour
{
    [Serializable]
    public class CubemapEvent : UnityEvent { }

    public Camera xrCamera;

    [Header("Mixed Reality Features")]
    public bool videoSeeThrough = true;
    public bool depthEstimation = false;
    [Range(0f, 1.0f)]
    public float VREyeOffset = 1.0f;

    [Header("Real Time Environment")]
    public bool environmentReflections = false;
    public int reflectionRefreshRate = 30;
    public VolumeProfile m_skyboxProfile = null;
    public Cubemap defaultSky = null;
    public CubemapEvent onCubemapUpdate = new CubemapEvent();

    private bool videoSeeThroughEnabled = false;
    private bool environmentReflectionsEnabled = false;
    private bool depthEstimationEnabled = false;
    private float currentVREyeOffset = 1f;

    private bool metadataStreamEnabled = false;
    private VarjoCameraMetadataStream.VarjoCameraMetadataFrame metadataFrame;

    private VarjoEnvironmentCubemapStream.VarjoEnvironmentCubemapFrame cubemapFrame;

    private bool originalOpaqueValue = false;
    private bool originalSubmitDepthValue = false;
    private bool originalDepthSortingValue = false;

    private bool defaultSkyActive = false;
    private bool cubemapEventListenerSet = false;

    private HDRISky volumeSky = null;
    private Exposure volumeExposure = null;
    private VSTWhiteBalance volumeVSTWhiteBalance = null;

    private HDAdditionalCameraData HDCameraData;

    private VarjoCameraSubsystem cameraSubsystem;

    private void Start()
    {
        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            var loader = XRGeneralSettings.Instance.Manager.activeLoader as Varjo.XR.VarjoLoader;
            cameraSubsystem = loader.cameraSubsystem as VarjoCameraSubsystem;
        }

        if (cameraSubsystem != null)
        {
            cameraSubsystem.Start();
        }
        originalOpaqueValue = VarjoRendering.GetOpaque();
        VarjoRendering.SetOpaque(false);
        cubemapEventListenerSet = onCubemapUpdate.GetPersistentEventCount() > 0;
        HDCameraData = xrCamera.GetComponent<HDAdditionalCameraData>();

        if (!m_skyboxProfile.TryGet(out volumeSky))
        {
            volumeSky = m_skyboxProfile.Add<HDRISky>(true);
        }

        if (!m_skyboxProfile.TryGet(out volumeExposure))
        {
            volumeExposure = m_skyboxProfile.Add<Exposure>(true);
        }

        if (!m_skyboxProfile.TryGet(out volumeVSTWhiteBalance))
        {
            volumeVSTWhiteBalance = m_skyboxProfile.Add<VSTWhiteBalance>(true);
        }
    }

    void Update()
    {
        UpdateMRFeatures();
    }

    void UpdateMRFeatures()
    {
        UpdateVideoSeeThrough();
        UpdateDepthEstimation();
        UpdateVREyeOffSet();
        UpdateEnvironmentReflections();
    }

    void UpdateVideoSeeThrough()
    {
        if (videoSeeThrough != videoSeeThroughEnabled)
        {
            if (videoSeeThrough)
            {
                videoSeeThrough = VarjoMixedReality.StartRender();
                if (HDCameraData)
                    HDCameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            }
            else
            {
                VarjoMixedReality.StopRender();
                if (HDCameraData)
                    HDCameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
            }
            videoSeeThroughEnabled = videoSeeThrough;
        }
    }

    void UpdateDepthEstimation()
    {
        if (depthEstimation != depthEstimationEnabled)
        {
            if (depthEstimation)
            {
                depthEstimation = VarjoMixedReality.EnableDepthEstimation();
                originalSubmitDepthValue = VarjoRendering.GetSubmitDepth();
                originalDepthSortingValue = VarjoRendering.GetDepthSorting();
                VarjoRendering.SetSubmitDepth(true);
                VarjoRendering.SetDepthSorting(true);
            }
            else
            {
                VarjoMixedReality.DisableDepthEstimation();
                VarjoRendering.SetSubmitDepth(originalSubmitDepthValue);
                VarjoRendering.SetDepthSorting(originalDepthSortingValue);
            }
            depthEstimationEnabled = depthEstimation;
        }
    }

    void UpdateVREyeOffSet()
    {
        if (VREyeOffset != currentVREyeOffset)
        {
            VarjoMixedReality.SetVRViewOffset(VREyeOffset);
            currentVREyeOffset = VREyeOffset;
        }
    }

    void UpdateEnvironmentReflections()
    {
        if (environmentReflections != environmentReflectionsEnabled)
        {
            if (environmentReflections)
            {
                if (VarjoMixedReality.environmentCubemapStream.IsSupported())
                {
                    environmentReflections = VarjoMixedReality.environmentCubemapStream.Start();
                }

                if (!cameraSubsystem.IsMetadataStreamEnabled)
                {
                    cameraSubsystem.EnableMetadataStream();
                }
                metadataStreamEnabled = cameraSubsystem.IsMetadataStreamEnabled;
            }
            else
            {
                VarjoMixedReality.environmentCubemapStream.Stop();
                cameraSubsystem.DisableMetadataStream();
            }
            environmentReflectionsEnabled = environmentReflections;
        }

        if (environmentReflectionsEnabled && metadataStreamEnabled)
        {
            if (VarjoMixedReality.environmentCubemapStream.hasNewFrame && cameraSubsystem.MetadataStream.hasNewFrame)
            {
                cubemapFrame = VarjoMixedReality.environmentCubemapStream.GetFrame();

                metadataFrame = cameraSubsystem.MetadataStream.GetFrame();
                float exposureValue = (float)metadataFrame.metadata.ev + Mathf.Log((float)metadataFrame.metadata.cameraCalibrationConstant, 2f);
                volumeExposure.fixedExposure.Override(exposureValue);

                volumeSky.hdriSky.Override(cubemapFrame.cubemap);
                volumeSky.updateMode.Override(EnvironmentUpdateMode.Realtime);
                volumeSky.updatePeriod.Override(1f / (float)reflectionRefreshRate);
                defaultSkyActive = false;

                volumeVSTWhiteBalance.intensity.Override(1f);

                // Set white balance normalization values
                Shader.SetGlobalColor("_CamWBGains", metadataFrame.metadata.wbNormalizationData.wbGains);
                Shader.SetGlobalMatrix("_CamInvCCM", metadataFrame.metadata.wbNormalizationData.invCCM);
                Shader.SetGlobalMatrix("_CamCCM", metadataFrame.metadata.wbNormalizationData.ccm);

                if (cubemapEventListenerSet)
                {
                    onCubemapUpdate.Invoke();
                }
            }
        }
        else if (!defaultSkyActive)
        {
            volumeSky.hdriSky.Override(defaultSky);
            volumeSky.updateMode.Override(EnvironmentUpdateMode.OnChanged);
            volumeExposure.fixedExposure.Override(6.5f);
            volumeVSTWhiteBalance.intensity.Override(0f);
            defaultSkyActive = true;
        }
    }

    void OnDisable()
    {
        videoSeeThrough = false;
        depthEstimation = false;
        environmentReflections = false;
        UpdateMRFeatures();
        VarjoRendering.SetOpaque(originalOpaqueValue);
    }
}