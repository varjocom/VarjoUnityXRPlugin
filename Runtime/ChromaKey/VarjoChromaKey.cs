// Copyright 2020 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Chroma Key Config Type determines the type of the configuration:
    /// </summary>
    public enum VarjoChromaKeyConfigType
    {
        /** <summary>Chroma key config in the index is not in use.</summary> */
        Disabled = 0,
        /** <summary>Chroma key config in the index is of type HSV.</summary> */
        HSV = 1,
    }

    /// <summary>
    /// Varjo Chroma Key Params contains HSV parameters for the chroma key configuration
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoChromaKeyParams
    {
        /** <summary>Chroma key color tone (range 0.0 .. 1.0).</summary> */
        public float hue;
        /** <summary>HSV tolerances (range 0.0 .. 1.0). x: Tolerance for color variation (H), y: Tolerance for bright and pale areas (S), z: Tolerance for dark and shaded areas (V).</summary> */
        public Vector3 hsvTolerance;
    };

    /// <summary>
    /// Varjo Chroma Key
    /// </summary>
    public class VarjoChromaKey
    {
        /// <summary>
        /// Enable or disable chroma keying.
        /// </summary>
        /// <param name="global">
        /// When <c>true</c> enables video pass through and starts chroma keying for all application layers regardless if they have chroma key
        /// flag set.Varjo system layers are not chroma keyed. This is used as force override to make it possible to run non-MR applications with chroma keying.
        /// Default is <c>false.</c>
        /// </param>
        /// <remarks>
        /// Start chroma keying for the video pass through image. This enables occlusion between VR and MR content
        /// when the VR content is submitted as a layer with chroma key testing enabled.
        /// This setting is ignored unless <see cref="VarjoMixedReality.StartRender"/> has been enabled.
        /// </remarks>
        /// <param name="enabled">Enabled.</param>
        public static bool EnableChromaKey(bool enabled, bool global = false)
        {
            if (global)
            {
                Native.SetChromaKeyGlobal(enabled);
                return VarjoError.CheckError();
            }
            else
            {
                if (!Native.EnableChromaKey(enabled))
                {
                    VarjoError.CheckError();
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Check if chroma key is enabled.
        /// </summary>
        /// <returns>True if enabled.</returns>
        public static bool IsChromaKeyEnabled()
        {
            return Native.IsChromaKeyEnabled();
        }

        /// <summary>
        /// Get the number of possible configuration slots.
        /// </summary>
        /// <returns>Number of supported chroma key configs.</returns>
        public static int GetChromaKeyConfigCount()
        {
            int configCount = Native.GetChromaKeyConfigCount();
            VarjoError.CheckError();
            return configCount;
        }

        /// <summary>
        /// Get currently applied chroma keying configuration for given index.
        /// Use <see cref="GetChromaKeyConfigCount"/> for getting maximum count.
        /// </summary>
        /// <param name="index">Chroma key index.</param>
        /// <returns>Currently applied chroma keying configuration.</returns>
        public static VarjoChromaKeyConfigType GetChromaKeyConfigType(int index)
        {
            VarjoChromaKeyConfigType configType = Native.GetChromaKeyConfigType(index);
            VarjoError.CheckError();
            return configType;
        }

        /// <summary>
        /// Try to lock the chroma key configs.
        /// This call needs to succeed before any calls altering the chroma key config state.
        /// <see cref="UnlockChromaKeyConfigs"/> should be called after the configuration has been changed.
        /// </summary>
        /// <remarks>
        /// The configuration can be locked by one application at a time. Locking fails,
        /// if another client has the configuration locked already.
        /// </remarks>
        /// <returns>True if locked successfully, otherwise False.</returns>
        public static bool LockChromaKeyConfigs()
        {
            if (!Native.LockChromaKeyConfigs())
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unlock a previously locked chroma key configs.
        /// </summary>
        public static void UnlockChromaKeyConfigs()
        {
            Native.UnlockChromaKeyConfigs();
            VarjoError.CheckError();
        }

        /// <summary>
        /// Disable chroma key configuration in the given index.
        /// </summary>
        /// <remarks>
        ///  Use <see cref="GetChromaKeyConfigCount"/> for getting maximum count. Config values will be clamped to valid range.
        ///  Before calling this function, chroma key config has to be locked succesfully with <see cref="LockChromaKeyConfigs"/>
        /// </remarks>
        /// <param name="index">Chroma key index.</param>
        /// <returns>True if no errors.</returns>
        public static bool DisableChromaKeyConfig(int index)
        {
            if (!Native.DisableChromaKeyConfig(index))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set chroma key parameters for the given configuration index.
        /// </summary>
        /// <remarks>
        /// Set video chroma keying configuration to select color matcher and setting parameters for it. Index can be used to
        /// set multiple chroma key configs that will all be evaluated into a single mask.
        /// Use <see cref="GetChromaKeyConfigCount"/> for getting maximum count. Config values will be clamped to valid range.
        /// Before calling this function, chroma key config has to be locked succesfully with <see cref="LockChromaKeyConfigs"/>
        /// </remarks>
        /// <param name="index">Chroma key index.</param>
        /// <param name="parameters">Chroma key configuration to be applied.</param>
        /// <returns></returns>
        public static bool SetChromaKeyParameters(int index, VarjoChromaKeyParams parameters)
        {
            if (!Native.SetChromaKeyParameters(index, parameters))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get currently applied chroma keying configuration for given index.
        /// </summary>
        /// <param name="index">Chroma key index.</param>
        /// <returns>Currently applied chroma keying configuration.</returns>
        public static VarjoChromaKeyParams GetChromaKeyParameters(int index)
        {
            VarjoChromaKeyParams parameters = Native.GetChromaKeyParameters(index);
            VarjoError.CheckError();
            return parameters;
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool EnableChromaKey(bool enabled);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool IsChromaKeyEnabled();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void SetChromaKeyGlobal(bool enabled);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetChromaKeyConfigCount();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern VarjoChromaKeyConfigType GetChromaKeyConfigType(int index);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool LockChromaKeyConfigs();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern void UnlockChromaKeyConfigs();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool DisableChromaKeyConfig(int index);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool SetChromaKeyParameters(int index, VarjoChromaKeyParams parameters);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern VarjoChromaKeyParams GetChromaKeyParameters(int index);
        }
    }
}