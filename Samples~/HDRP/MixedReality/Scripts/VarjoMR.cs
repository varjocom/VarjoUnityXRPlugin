// Copyright 2019 Varjo Technologies Oy. All rights reserved.

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

/// <summary>
/// Controls for mixed reality related functionalities.
/// </summary>
public class VarjoMR : MonoBehaviour
{
    [Serializable]
    public class CubemapEvent : UnityEvent { }

    public Camera XRCamera;

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

    private bool distortedColorStreamEnabled = false;
    private VarjoDistortedColorStream.VarjoDistortedColorFrame cameraFrame;

    private VarjoEnvironmentCubemapStream.VarjoEnvironmentCubemapFrame cubemapFrame;
    private VarjoSettings settings = null;

    private bool originalOpaqueValue = false;
    private bool originalSubmitDepthValue = false;
    private bool originalDepthSortingValue = false;

    private bool defaultSkyActive = false;
    private bool cubemapEventListenerSet = false;

    private HDRISky volumeSky = null;
    private Exposure volumeExposure = null;
    private VSTWhiteBalance volumeVSTWhiteBalance = null;

    private HDAdditionalCameraData HDCameraData;

    private void Start()
    {
        StartCoroutine(GetSettingsWhenInitialized());
        cubemapEventListenerSet = onCubemapUpdate.GetPersistentEventCount() > 0;
        HDCameraData = XRCamera.GetComponent<HDAdditionalCameraData>();

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

    IEnumerator GetSettingsWhenInitialized()
    {
        while (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("Waiting for init..");
            yield return new WaitForSeconds(0.1f);
        }
        VarjoLoader varjoLoader = XRGeneralSettings.Instance.Manager.ActiveLoaderAs<VarjoLoader>();
        settings = varjoLoader.GetSettings();
        Initialize();
    }

    void Initialize()
    {
        if (settings != null)
        {
            originalOpaqueValue = settings.opaque;
            settings.opaque = false;
            settings.UpdateSettings();
        }
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
                if (settings != null)
                {
                    originalSubmitDepthValue = settings.submitDepth;
                    originalDepthSortingValue = settings.depthSorting;
                    settings.submitDepth = true;
                    settings.depthSorting = true;
                    settings.UpdateSettings();
                }
            }
            else
            {
                VarjoMixedReality.DisableDepthEstimation();
                if (settings != null)
                {
                    settings.submitDepth = originalSubmitDepthValue;
                    settings.depthSorting = originalDepthSortingValue;
                    settings.UpdateSettings();
                }
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

                if (VarjoMixedReality.distortedColorStream.IsSupported())
                {
                    distortedColorStreamEnabled = VarjoMixedReality.distortedColorStream.Start();
                }
                else
                {
                    distortedColorStreamEnabled = false;
                }
            }
            environmentReflectionsEnabled = environmentReflections;
        }

        if (environmentReflectionsEnabled && distortedColorStreamEnabled)
        {
            cubemapFrame = VarjoMixedReality.environmentCubemapStream.GetFrame();

            cameraFrame = VarjoMixedReality.distortedColorStream.GetFrame();
            float exposureValue = (float)cameraFrame.ev + Mathf.Log((float)cameraFrame.cameraCalibrationConstant, 2f);
            volumeExposure.fixedExposure.Override(exposureValue);

            volumeSky.hdriSky.Override(cubemapFrame.cubemap);
            volumeSky.updateMode.Override(EnvironmentUpdateMode.Realtime);
            volumeSky.updatePeriod.Override(1f / (float)reflectionRefreshRate);
            defaultSkyActive = false;

            volumeVSTWhiteBalance.intensity.Override(1f);

            // Set white balance normalization values
            Shader.SetGlobalColor("_CamWBGains", cameraFrame.wbNormalizationData.wbGains);
            Shader.SetGlobalMatrix("_CamInvCCM", cameraFrame.wbNormalizationData.invCCM);
            Shader.SetGlobalMatrix("_CamCCM", cameraFrame.wbNormalizationData.ccm);

            if (cubemapEventListenerSet)
            {
                onCubemapUpdate.Invoke();
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
        settings.opaque = originalOpaqueValue;
        settings.UpdateSettings();
    }
}