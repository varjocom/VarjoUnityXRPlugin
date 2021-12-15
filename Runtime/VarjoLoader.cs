
// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public class VarjoLoader : XRLoaderHelper
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        public override bool Initialize()
        {
            InitializePluginInstance();

            // Send the settings over to the native plugin
            VarjoSettings settings = GetSettings();
            if (settings != null)
            {
                UserDefinedSettings uds;
                uds.stereoRenderingMode = (ushort)settings.StereoRenderingMode;
                uds.separateCullPass = (ushort)(settings.SeparateCullPass ? 1 : 0);
                uds.foveatedRendering = (ushort)(settings.FoveatedRendering ? 1 : 0);
                uds.contextScalingFactor = settings.ContextScalingFactor;
                uds.focusScalingFactor = settings.FocusScalingFactor;
                uds.opaque = (ushort)(settings.Opaque ? 1 : 0);
                uds.faceLocked = (ushort)(settings.FaceLocked ? 1 : 0);
                uds.flipY = (ushort)(settings.FlipY ? 1 : 0);
                uds.occlusionMesh = (ushort)(settings.OcclusionMesh ? 1 : 0);
                uds.sessionPriority = settings.SessionPriority;
                uds.submitDepth = (ushort)(settings.SubmitDepth ? 1 : 0);
                uds.depthSorting = (ushort)(settings.DepthSorting ? 1 : 0);
                uds.depthTestRange = (ushort)(settings.DepthTestRange ? 1 : 0);
                uds.depthTestNearZ = settings.DepthTestNearZ;
                uds.depthTestFarZ = settings.DepthTestFarZ;

#if !HDRP || HDRP_8_OR_NEWER
                uds.checkSinglePassValue = 1;
                uds.flipOcclusionMesh = 0;
#else
                uds.checkSinglePassValue = 0;
                uds.flipOcclusionMesh = 1;
#endif

                switch (settings.StereoRenderingMode)
                {
                    default:
                    case VarjoStereoRenderingMode.MultiPass:
                        uds.useTextureArrays = 0;
                        break;
                    case VarjoStereoRenderingMode.TwoPass:
                        uds.useTextureArrays = 1;
                        break;
                }
                SetUserDefinedSettings(uds);
            }

            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "VarjoDisplay");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "VarjoInput");
            return true;
        }

        public override bool Start()
        {
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRDisplaySubsystem>();

            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRDisplaySubsystem>();

            ShutdownPluginInstance();

            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UserDefinedSettings
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
            public ushort checkSinglePassValue;
            public ushort useTextureArrays;
            public ushort flipOcclusionMesh;
        }

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetUserDefinedSettings(UserDefinedSettings settings);

        [DllImport("VarjoUnityXR")]
        private static extern void InitializePluginInstance();

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
