
// Copyright 2019 Varjo Technologies Oy. All rights reserved.
using System.IO;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using Varjo.XR.Input;
#endif

namespace Varjo.XR
{
#if UNITY_INPUT_SYSTEM
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    static class InputLayoutLoader
    {
        static InputLayoutLoader()
        {
            RegisterInputLayouts();
        }

        public static void RegisterInputLayouts()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<VarjoHMD>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithManufacturer("Varjo")
                    .WithProduct("^(XR-|VR-|AERO).*$")
            );
            InputSystem.RegisterLayout<VarjoController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithManufacturer("Varjo")
                    .WithProduct(@"(((Varjo Controller)).*)")
            );
            InputSystem.RegisterLayout<VarjoViveWand>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithManufacturer("HTC")
                    .WithProduct(@"(((SteamVR Controller \(Vive Wand)).*)")
            );
            InputSystem.RegisterLayout<VarjoIndexController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithManufacturer("Valve")
                    .WithProduct(@"(((SteamVR Controller \(Index Controller)).*)")
            );
               InputSystem.RegisterLayout<VarjoSteamVRTrackerWithButtons>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Undefined)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerHanded>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \(((Left)|(Right)).Hand\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerCamera>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Camera)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerChest>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Chest)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerKeyboard>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Keyboard)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerLeftElbow>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Left Elbow)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerLeftFoot>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Left Foot)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerLeftKnee>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Left Knee)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerLeftShoulder>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Left Shoulder)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerRightElbow>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Right Elbow)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerRightFoot>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Right Foot)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerRightKnee>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Right Knee)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerRightShoulder>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Right Shoulder)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTrackerWaist>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct(@"(SteamVR Tracker \((Waist)\))")
            );
            InputSystem.RegisterLayout<VarjoSteamVRTracker>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("^(SteamVR Tracker)$")
            );
        }
    }
#endif

    public class VarjoLoader : XRLoaderHelper
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();
        private static List<XROcclusionSubsystemDescriptor> s_OcclusionSubsystemDescriptors =
            new List<XROcclusionSubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();
        private static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors =
            new List<XRSessionSubsystemDescriptor>();
        private static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors =
            new List<XRCameraSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();
        public XRCameraSubsystem cameraSubsystem => GetLoadedSubsystem<XRCameraSubsystem>();
        public XROcclusionSubsystem occlusionSubsystem => GetLoadedSubsystem<XROcclusionSubsystem>();

        public override bool Initialize()
        {
            if (!Varjo.IsVarjoSystemInstalled())
            {
                return false;
            }

#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif

#if UNITY_EDITOR
            string actionManifestPath = Path.GetFullPath("Packages/com.varjo.xr/Runtime/Input/Configs/actions.json");
#else
            string actionManifestPath = Path.GetFullPath(Application.streamingAssetsPath + "/Varjo/Input/Configs/actions.json");
#endif
            SetActionManifestPath(actionManifestPath);

            // Send the settings over to the native plugin
            VarjoSettings settings = GetSettings();
            if (settings != null)
            {
                NativePluginSettings nps;
                nps.stereoRenderingMode = (ushort)settings.StereoRenderingMode;
                nps.separateCullPass = (ushort)(settings.SeparateCullPass ? 1 : 0);
                nps.foveatedRendering = (ushort)(settings.FoveatedRendering ? 1 : 0);
                nps.contextScalingFactor = settings.ContextScalingFactor;
                nps.focusScalingFactor = settings.FocusScalingFactor;
                nps.opaque = (ushort)(settings.Opaque ? 1 : 0);
                nps.faceLocked = (ushort)(settings.FaceLocked ? 1 : 0);
                nps.flipY = (ushort)(settings.FlipY ? 1 : 0);
                nps.occlusionMesh = (ushort)(settings.OcclusionMesh ? 1 : 0);
                nps.sessionPriority = settings.SessionPriority;
                nps.submitDepth = (ushort)(settings.SubmitDepth ? 1 : 0);
                nps.depthSorting = (ushort)(settings.DepthSorting ? 1 : 0);
                nps.depthTestRange = (ushort)(settings.DepthTestRange ? 1 : 0);
                nps.depthTestNearZ = settings.DepthTestNearZ;
                nps.depthTestFarZ = settings.DepthTestFarZ;

#if UNITY_2021_2_OR_NEWER
                nps.supportsDX12 = 1;
#else
                nps.supportsDX12 = 0;
#endif

                SetNativePluginSettings(nps);
            }

            if (!InitializePluginInstance())
            {
                return false;
            }

            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "VarjoDisplay");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "VarjoInput");
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, VarjoSessionSubsystem.VarjoSessionID);
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, VarjoCameraSubsystem.VarjoCameraID);
            CreateSubsystem<XROcclusionSubsystemDescriptor, XROcclusionSubsystem>(s_OcclusionSubsystemDescriptors, VarjoOcclusionSubsystem.VarjoOcclusionID);
            return VarjoError.CheckError();
        }

        public override bool Start()
        {
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            StartSubsystem<XRSessionSubsystem>();
            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRSessionSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRSessionSubsystem>();

            ShutdownPluginInstance();

            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePluginSettings
        {
            public ushort stereoRenderingMode;
            public ushort separateCullPass;
            public ushort foveatedRendering;
            public float contextScalingFactor;
            public float focusScalingFactor;
            public ushort opaque;
            public ushort faceLocked;
            public ushort flipY;
            public ushort occlusionMesh;
            public int sessionPriority;
            public ushort submitDepth;
            public ushort depthSorting;
            public ushort depthTestRange;
            public double depthTestNearZ;
            public double depthTestFarZ;
            public ushort supportsDX12;
        }

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetActionManifestPath([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetNativePluginSettings(NativePluginSettings settings);

        [DllImport("VarjoUnityXR")]
        private static extern bool InitializePluginInstance();

        [DllImport("VarjoUnityXR")]
        private static extern void ShutdownPluginInstance();

        public VarjoSettings GetSettings()
        {
            VarjoSettings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<VarjoSettings>("Varjo.XR.Settings", out settings);
#else
            settings = VarjoSettings.s_Settings;
#endif
            if (settings == null)
                settings = CreateInstance<VarjoSettings>();
            return settings;
        }
    }
}
