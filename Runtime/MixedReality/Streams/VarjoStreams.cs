// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    internal delegate void VarjoStreamCallback(VarjoStreamFrame frame, IntPtr userdata);

    internal enum VarjoStreamType : long
    {
        DistortedColor = 1,        //!< Distorted (i.e. uncorrected) color data stream from visible light RGB camera.
        EnvironmentCubemap = 2,    //!< Lighting estimate stream as a cubemap.
    }

    internal enum VarjoCalibrationModel : long
    {
        Omnidir = 1,        //!< Omnidir calibration model.
    }

    internal enum VarjoTextureFormat : long
    {
        R8G8B8A8_SRGB = 1,
        B8G8R8A8_SRGB = 2,
        D32_FLOAT = 3,
        A8_UNORM = 4,
        YUV422 = 5,
        RGBA16_FLOAT = 6,
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct VarjoStreamFrameMetadata
    {
        [FieldOffset(0)]
        internal VarjoDistortedColorData distortedColorData;
        [FieldOffset(0)]
        internal VarjoEnvironmentCubemapData environmentCubemapData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoStreamFrame
    {
        internal VarjoStreamType type; //!< Type of the stream.
        internal long id;              //!< Id of the stream.
        internal long frameNumber;     //!< Monotonically increasing frame number.
        internal long channelFlags;    //!< Channels that this frame contains.
        internal long dataFlags;       //!< Data that this frame contains.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal double[] hmdPose;     //!< Pose at the time when the frame was produced.
        internal VarjoStreamFrameMetadata metadata; //!< Frame data. Use 'type' to determine which element to access.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoWBNormalizationData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        internal double[] wbGains;                              //!< White balance gains to convert from 6500K to VST color temperature.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        internal double[] invCCM;                               //!< Inverse CCM for 6500K color temperature.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        internal double[] ccm;                                  //!< CCM for VST color temperature.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoDistortedColorData
    {
        internal long timestamp;                                //!< Timestamp at end of exposure.
        internal double ev;                                     //!< EV (exposure value) at ISO100.
        internal double exposureTime;                           //!< Exposure time in seconds.
        internal double whiteBalanceTemperature;                //!< White balance temperature in Kelvin degrees.
        internal VarjoWBNormalizationData wbNormalizationData;  //!< White balance normalization data.
        internal double cameraCalibrationConstant;              //!< The luminance (in cd/m^2) which saturates a pixel is equal to 2^ev * cameraCalibrationConstant.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoEnvironmentCubemapData
    {
        internal readonly long timestamp;     //!< Timestamp when the cubemap was last updated.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VarjoBufferMetadata
    {
        internal VarjoTextureFormat textureFormat;  //!< Texture format.
        internal long bufferType;                   //!< CPU or GPU.
        internal int byteSize;                      //!< Buffer size in bytes.
        internal int rowStride;                     //!< Buffer row stride in bytes.
        internal int width;                         //!< Image width.
        internal int height;                        //!< Image height.
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VarjoCameraIntrinsics
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
}
