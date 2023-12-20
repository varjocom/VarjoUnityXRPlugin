// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Camera property types that can be accessed to control video pass through cameras.
    /// </summary>
    public enum VarjoCameraPropertyType : long
    {
        /** <summary>Exposure time.</summary> */
        ExposureTime = 1,
        /** <summary>ISO value.</summary> */
        ISOValue = 2,
        /** <summary>White balance.</summary> */
        WhiteBalance = 3,
        /** <summary>Flicker compensation (50Hz and 60Hz).</summary> */
        FlickerCompensation = 4,
        /** <summary>Sharpness filter mode.</summary> */
        Sharpness = 5,
        /** <summary>Eye reprojection mode. This offers two modes:
         * When mode is AUTO, the view is automatically reprojected based on the depth map.
         * When mode is MANUAL, the view is reprojected into a certain static distance. Distance is controllable
         * with the manual value.</summary> */
        EyeReprojection = 6,
        /** <summary>Auto exposure behavior. See VarjoAutoExposureBehavior for available values.</summary> */
        AutoExposureBehavior = 7,
    }

    /// <summary>
    /// Varjo Camera property modes that can be set to control video pass through cameras.
    /// </summary>
    public enum VarjoCameraPropertyMode : long
    {
        /** <summary>Off mode.</summary> */
        Off = 0,
        /** <summary>Automatic mode.</summary> */
        Auto = 1,
        /** <summary>Manual value mode.</summary> */
        Manual = 2,
    }

    /// <summary>
    /// Varjo Camera property data types.
    /// </summary>
    public enum VarjoCameraPropertyDataType : long
    {
        /** <summary>Integer.</summary> */
        Int = 1,
        /** <summary>Floating point.</summary> */
        Double = 2,
        /** <summary>Boolean.</summary> */
        Bool = 3,
    }

    /// <summary>
    /// Varjo Camera property config type.
    /// </summary>
    public enum VarjoCameraPropertyConfigType : long
    {
        /** <summary>List of discrete values.</summary> */
        List = 1,
        /** <summary>Range (min, max).</summary> */
        Range = 2,
    }

    /// <summary>
    /// <para>Wrapper for different camera property values.</para>
    /// Note that this is a representation of C++ union:
    /// <c>doubleValue</c>, <c>intValue</c> and <c>boolValue</c> are mutually exclusive.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VarjoCameraPropertyValue
    {
        /// <summary>
        /// Data type for this value.
        /// </summary>
        [FieldOffset(0)]
        public VarjoCameraPropertyDataType type;
        /// <summary>
        /// Floating point value.
        /// </summary>
        [FieldOffset(8)]
        public double doubleValue;
        /// <summary>
        /// Integer value.
        /// </summary>
        [FieldOffset(8)]
        public long intValue;
        /// <summary>
        /// Boolean value.
        /// </summary>
        [FieldOffset(8)]
        public int boolValue;
    }

    /// <summary>
    /// Available options for VarjoCameraPropertyType.AutoExposureBehavior property.
    /// </summary>
    public enum VarjoAutoExposureBehavior : long
    {
        /** <summary>Normal (legacy) auto exposure behavior.</summary> */
        Default = 0,
        /** <summary>More aggressive behavior to prevent any oversaturation in the image.
         * Good for example for ensuring that even small displays stay readable.</summary> */
        PreventOverexposure = 1,
    };
}
