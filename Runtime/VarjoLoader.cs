
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


        public override bool Initialize()
        {
            // Send the settings over to the native plugin
            VarjoSettings settings = GetSettings();
            settings.UpdateSettings();

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
            return true;
        }

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
