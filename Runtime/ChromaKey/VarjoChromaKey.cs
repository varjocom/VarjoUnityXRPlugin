// Copyright 2020 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo.XR
{
    public enum VarjoChromaKeyConfigType
    {
        Disabled = 0,        //!< Chroma key config in the index is not in use.
        HSV = 1,             //!< Chroma key config in the index is of type HSV.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoChromaKeyParams
    {
        public float hue;                    //!< Chroma key color tone (range 0.0 .. 1.0).
        public Vector3 hsvTolerance;         //!< HSV tolerances (range 0.0 .. 1.0). x: Tolerance for color variation (H), y: Tolerance for bright and pale areas (S), z: Tolerance for dark and shaded areas (V).
    };

    public class VarjoChromaKey
    {
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool EnableChromaKey(bool enabled);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsChromaKeyEnabled();

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