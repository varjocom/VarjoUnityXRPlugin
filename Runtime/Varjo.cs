using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public class Varjo
    {
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsVarjoSystemInstalled();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsHMDConnected();
    }
}