using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoVersion
    {
        public int major;
        public int minor;
        public int patch;
        public int build;
    };

    public class Varjo
    {
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsVarjoSystemInstalled();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsHMDConnected();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern VarjoVersion GetPluginVersion();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern VarjoVersion GetVarjoRuntimeVersion();
    }
}