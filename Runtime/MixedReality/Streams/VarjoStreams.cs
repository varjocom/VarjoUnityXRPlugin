// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo.XR
{
    internal delegate void VarjoStreamCallback(VarjoStreamFrame frame, IntPtr userdata);

    /// <summary>
    /// Varjo Stream Type.
    /// </summary>
    public enum VarjoStreamType : long
    {
        CameraMetadata = 0,
        /** <summary>Distorted (i.e. uncorrected) color data stream from visible light RGB camera.</summary>*/
        DistortedColor = 1,
        /** <summary>Lighting estimate stream as a cubemap.</summary> */
        EnvironmentCubemap = 2,
    }

    /// <summary>
    /// Varjo Channel Flags.
    /// </summary>
    [Flags]
    public enum VarjoChannelFlags : ulong
    {
        None = 0,
        First = 1,
        Second = 2,
        All = ulong.MaxValue
    }

    /// <summary>
    /// Varjo Buffer Type.
    /// </summary>
    public enum VarjoBufferType : long
    {
        CPU = 1,
        GPU = 2,
    }

    /// <summary>
    /// Varjo Calibration Model.
    /// </summary>
    public enum VarjoCalibrationModel : long
    {
        /** <summary>Omnidir calibration model.</summary> */
        Omnidir = 1,
    }

    /// <summary>
    /// Varjo Texture Format
    /// </summary>
    public enum VarjoTextureFormat : long
    {
        R8G8B8A8_SRGB = 1,
        B8G8R8A8_SRGB = 2,
        D32_FLOAT = 3,
        A8_UNORM = 4,
        YUV422 = 5,
        RGBA16_FLOAT = 6,
        R8G8B8A8_UNORM = 9,
        R32_FLOAT = 10,
        NV12 = 13,
    }

    /// <summary>
    /// Varjo environment cubemap modes.
    /// </summary>
    public enum VarjoEnvironmentCubemapMode : long
    {
        Fixed6500K = 0,
        AutoAdapt = 1,
    }

    /// <summary>
    /// Varjo Stream Config.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoStreamConfig
    {
        /** <summary>Id.</summary> */
        public long streamId;
        /** <summary>Bitfield of provided camera channels.</summary> */
        public VarjoChannelFlags channelFlags;
        /** <summary>Stream type.</summary> */
        public VarjoStreamType streamType;
        /** <summary>Buffer type: CPU or GPU memory buffer.</summary> */
        public VarjoBufferType bufferType;
        /** <summary>Texture format.</summary> */
        public VarjoTextureFormat format;
        /** <summary>Transform from HMD pose center to stream origin in view coordinates.</summary> */
        public VarjoMatrix streamTransform;
        /** <summary>Frame rate in frames per second</summary> */
        public int frameRate;
        /** <summary>Texture width.</summary> */
        public int width;
        /** <summary>Texture height;</summary> */
        public int height;
        /** <summary>Buffer row stride in bytes.</summary> */
        public int rowStride;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct VarjoStreamFrameMetadata
    {
        [FieldOffset(0)]
        internal DistortedColorFrameMetadata distortedColorData;
        [FieldOffset(0)]
        internal VarjoEnvironmentCubemapData environmentCubemapData;
    }

    [Flags]
    internal enum VarjoDataFlags : ulong
    {
        Buffer = 1,
        Intrinsics = 2,
        Extrinsics = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoStreamFrame
    {
        internal VarjoStreamType type; //!< Type of the stream.
        internal long id;              //!< Id of the stream.
        internal long frameNumber;     //!< Monotonically increasing frame number.
        internal VarjoChannelFlags channelFlags;    //!< Channels that this frame contains.
        internal VarjoDataFlags dataFlags;       //!< Data that this frame contains.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal double[] hmdPose;     //!< Pose at the time when the frame was produced.
        internal VarjoStreamFrameMetadata metadata; //!< Frame data. Use 'type' to determine which element to access.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WBNormalizationData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        internal double[] wbGains;                              //!< White balance gains to convert from 6500K to VST color temperature.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        internal double[] invCCM;                               //!< Inverse CCM for 6500K color temperature.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        internal double[] ccm;                                  //!< CCM for VST color temperature.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DistortedColorFrameMetadata
    {
        internal long timestamp;                                //!< Timestamp at end of exposure.
        internal double ev;                                     //!< EV (exposure value) at ISO100.
        internal double exposureTime;                           //!< Exposure time in seconds.
        internal double whiteBalanceTemperature;                //!< White balance temperature in Kelvin degrees.
        internal WBNormalizationData wbNormalizationData;       //!< White balance normalization data.
        internal double cameraCalibrationConstant;              //!< The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoEnvironmentCubemapData
    {
        internal long timestamp;                           //!< Timestamp when the cubemap was last updated.
        internal VarjoEnvironmentCubemapMode mode;         //!< Current cubemap mode.
        internal double whiteBalanceTemperature;           //!< White balance temperature in Kelvin degrees.
        internal double brightnessNormalizationGain;       //!< Normalization gain to convert cubemap brightness to match VST image.
        internal WBNormalizationData wbNormalizationData;  //!< White balance normalization data.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoBufferMetadata
    {
        internal VarjoTextureFormat textureFormat;  //!< Texture format.
        internal VarjoBufferType bufferType;        //!< CPU or GPU.
        internal int byteSize;                      //!< Buffer size in bytes.
        internal int rowStride;                     //!< Buffer row stride in bytes.
        internal int width;                         //!< Image width.
        internal int height;                        //!< Image height.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoCameraIntrinsics
    {
        internal VarjoCalibrationModel model;       //!< Intrisics calibration model.
        internal double principalPointX;            //!< Camera principal point X.
        internal double principalPointY;            //!< Camera principal point Y.
        internal double focalLengthX;               //!< Camera focal length X.
        internal double focalLengthY;               //!< Camera focal length Y.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        internal double[] distortionCoefficients;   //!< Intrinsics model coefficients. For omnidir: 2 radial, skew, xi, 2 tangential.
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoTexture
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal long[] reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoMatrix
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public double[] value;
    };

    //!< Intrinsics model coefficients. For omnidir: 2 radial, skew, xi, 2 tangential.
    public struct VarjoDistortionCoefficients
    {
        public Vector4 K { get; }
        public Vector2 Kr { get; }
        public Vector2 P { get; }

        public VarjoDistortionCoefficients(Vector4 K, Vector2 Kr, Vector2 P)
        {
            this.K = K;
            this.Kr = Kr;
            this.P = P;
        }
    };

}
