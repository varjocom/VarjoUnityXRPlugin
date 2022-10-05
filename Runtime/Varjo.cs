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

        public override string ToString() => $"{major}.{minor}.{patch}.{build}";
    };

    public class Varjo
    {
        /// <summary>
        /// Checks whether Varjo system is available.
        /// </summary>
        /// <remarks>
        /// If returns false, it is guaranteed that the session cannot be initialized.
        /// If this returns true, application may try initiating a new session.
        /// </remarks>
        /// <returns>False if Varjo system is not available, true if it is available.</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsVarjoSystemInstalled();

        /// <summary>
        /// Is HMD Connected.
        /// </summary>
        /// <returns>True if connected.</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsHMDConnected();

        /// <summary>
        /// Get Plugin Version.
        /// </summary>
        /// <returns>Plugin Version.</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern VarjoVersion GetPluginVersion();

        /// <summary>
        /// Get Varjo Runtime Version.
        /// </summary>
        /// <returns>Runtime Version.</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern VarjoVersion GetVarjoRuntimeVersion();

        /// <summary>
        /// Get Varjo Session.
        /// </summary>
        /// <returns>Pointer handle to Varjo session.</returns>
        [DllImport("VarjoUnityXR")]
        public static extern IntPtr GetVarjoSession();
    }
}