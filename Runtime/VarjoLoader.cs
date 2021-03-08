
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
#if VARJO_EXPERIMENTAL_FEATURES
        private static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptors =
            new List<XRMeshSubsystemDescriptor>();
#endif

        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
#if VARJO_EXPERIMENTAL_FEATURES
        public XRMeshSubsystem meshSubsystem => GetLoadedSubsystem<XRMeshSubsystem>();

        private int m_MeshSubsystemRefCount = 0;
#endif
        public override bool Initialize()
        {
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
#else
                uds.checkSinglePassValue = 0;
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
#if VARJO_EXPERIMENTAL_FEATURES
            if (!PluginVersionMatchesRuntime())
            {
                VarjoVersion p = Varjo.GetPluginVersion();
                VarjoVersion r = Varjo.GetVarjoRuntimeVersion();
                Debug.LogFormat(LogType.Warning, LogOption.None, this,
                    "Experimental Varjo XR Meshing Subsystem can't be initialized. Experimental features in this version of the plugin can only be used with Varjo Base version {0}.{1}.{2}.\n" +
                    "Varjo Base {3}.{4}.{5} is currently installed. Install Varjo Base {0}.{1}.{2} to use experimental XR Meshing Subsystem.",
                    p.major, p.minor, p.patch, r.major, r.minor, r.patch);
            }
            else
            {
                CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptors, "VarjoMeshing");
            }
#endif
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
#if VARJO_EXPERIMENTAL_FEATURES
            if (m_MeshSubsystemRefCount > 0)
            {
                m_MeshSubsystemRefCount = 0;
                StopSubsystem<XRMeshSubsystem>();
            }
#endif
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRDisplaySubsystem>();

            return true;
        }

        public override bool Deinitialize()
        {
#if VARJO_EXPERIMENTAL_FEATURES
            DestroySubsystem<XRMeshSubsystem>();
#endif
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRDisplaySubsystem>();

            return true;
        }

#if VARJO_EXPERIMENTAL_FEATURES
        internal void StartMeshSubsystem()
        {
            if (!PluginVersionMatchesRuntime())
            {
                VarjoVersion p = Varjo.GetPluginVersion();
                VarjoVersion r = Varjo.GetVarjoRuntimeVersion();
                Debug.LogFormat(LogType.Error, LogOption.None, this,
                    "Failed to start experimental Varjo XR Meshing Subsystem. Experimental features in this version of the plugin can only be used with Varjo Base version {0}.{1}.{2}.\n" +
                    "Varjo Base {3}.{4}.{5} is currently installed. Install Varjo Base {0}.{1}.{2} to use experimental XR Meshing Subsystem.",
                    p.major, p.minor, p.patch, r.major, r.minor, r.patch);
                return;
            }

            m_MeshSubsystemRefCount += 1;
            if (m_MeshSubsystemRefCount == 1)
            {
                StartSubsystem<XRMeshSubsystem>();
            }
        }

        internal void StopMeshSubsystem()
        {
            if (m_MeshSubsystemRefCount == 0)
                return;

            m_MeshSubsystemRefCount -= 1;
            if (m_MeshSubsystemRefCount == 0)
            {
                StopSubsystem<XRMeshSubsystem>();
            }
        }

        private bool PluginVersionMatchesRuntime()
        {
            VarjoVersion p = Varjo.GetPluginVersion();
            VarjoVersion r = Varjo.GetVarjoRuntimeVersion();
            return p.major == r.major && p.minor == r.minor && p.patch == r.patch;
        }

#endif
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
        }

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern void SetUserDefinedSettings(UserDefinedSettings settings);

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
