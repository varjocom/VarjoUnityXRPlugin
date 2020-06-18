// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public enum VarjoCameraPropertyType : long
    {
        ExposureTime = 1,
        ISOValue = 2,
        WhiteBalance = 3,
        FlickerCompensation = 4,
        Sharpness = 5,
    }

    public enum VarjoCameraPropertyMode : long
    {
        Off = 0,
        Auto = 1,
        Manual = 2,
    }

    public enum VarjoCameraPropertyDataType : long
    {
        Int = 1,
        Double = 2,
        Bool = 3,
    }

    public enum VarjoCameraPropertyConfigType : long
    {
        List = 1,
        Range = 2,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct VarjoCameraPropertyValue
    {
        [FieldOffset(0)]
        public VarjoCameraPropertyDataType type;
        [FieldOffset(8)]
        public double doubleValue;
        [FieldOffset(8)]
        public long intValue;
        [FieldOffset(8)]
        public int boolValue;
    }
}