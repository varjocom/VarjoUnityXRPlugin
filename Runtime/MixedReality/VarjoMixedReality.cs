// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo.XR
{
    public class VarjoMixedReality
    {
        private const long VARJO_LOCKTYPE_CAMERA = 1;

        /// <summary>
        /// Environmental lighting cubemap stream.
        /// </summary>
        public static readonly VarjoEnvironmentCubemapStream environmentCubemapStream = new VarjoEnvironmentCubemapStream();

        /// <summary>
        /// Is Mixed Reality capable hardware present.
        /// </summary>
        /// <returns>True if present.</returns>
        public static bool IsMRAvailable() { return Native.IsMRAvailable(); }

        /// <summary>
        /// Is Mixed Reality Ready
        /// </summary>
        /// <returns>True if Plugin Instance was initialized and MixedReality hardware is available.</returns>
        public static bool IsMRReady()
        {
            if (Varjo.GetVarjoSession() == IntPtr.Zero)
            {
                return false;
            }

            if (!IsMRAvailable())
            {
                Debug.LogError("Mixed reality hardware not available.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Starts video see-through rendering.
        /// </summary>
        /// <returns>True, if VST rendering was started successfully.</returns>
        public static bool StartRender()
        {
            if (!IsMRReady()) return false;
            Native.varjo_MRSetVideoRender(Varjo.GetVarjoSession(), true);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Stops video see-through rendering.
        /// </summary>
        public static void StopRender()
        {
            if (!IsMRReady()) return;
            Native.varjo_MRSetVideoRender(Varjo.GetVarjoSession(), false);
        }

        /// <summary>
        /// Enables depth estimation from VST images.
        /// </summary>
        /// <returns>True, if depth estimation was enabled successfully.</returns>
        public static bool EnableDepthEstimation()
        {
            if (!IsMRReady()) return false;
            if (!Native.SetDepthEstimation(true))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disables depth estimation from VST images.
        /// </summary>
        public static void DisableDepthEstimation()
        {
            if (!IsMRReady()) return;
            if (!Native.SetDepthEstimation(false))
            {
                VarjoError.CheckError();
            }
        }

        /// <summary>
        /// Try to lock the environment cubemap configuration.
        /// This call needs to succeed before any calls altering the chroma key config state.
        /// <see cref="UnlockEnvironmentCubemapConfig"/> should be called after the configuration has been changed.
        /// </summary>
        /// <remarks>
        /// The configuration can be locked by one application at a time. Locking fails,
        /// if another client has the configuration locked already.
        /// </remarks>
        /// <returns>True if locked successfully, otherwise False.</returns>
        public static bool LockEnvironmentCubemapConfig()
        {
            return Native.LockEnvironmentCubemapConfig();
        }

        /// <summary>
        /// Unlock a previously locked environment cubemap configuration.
        /// </summary>
        public static void UnlockEnvironmentCubemapConfig()
        {
            Native.UnlockEnvironmentCubemapConfig();
        }

        /// <summary>
        /// Set environment cubemap mode.
        /// </summary>
        /// <remarks>
        /// Set environment cubemap mode to decide how to do color correction in the application.
        /// In Fixed6500K mode, cubemap is given in a fixed 6500K color temperature with values normalized to EV100.
        /// This means application must perform proper color correction as a post process step to convert colors to match VST image.
        /// In AutoAdapt mode, colors and brightness of the cubemap has been automatically adapted to match VST image.
        /// Before calling this function, environment cubemap configuration has to be locked succesfully with <see cref="LockChromaKeyConfigs"/>
        /// </remarks>
        /// <param name="mode">Environment cubemap mode to apply.</param>
        /// <returns>True if mode was successfully changed. Otherwise false.</returns>
        public static bool SetEnvironmentCubemapMode(VarjoEnvironmentCubemapMode mode)
        {
            return Native.SetEnvironmentCubemapMode(mode);
        }

        /// <summary>
        /// Get currently set environment cubemap mode.
        /// </summary>
        /// <returns>Currently set environment cubemap mode.</returns>
        public static VarjoEnvironmentCubemapMode GetEnvironmentCubemapMode()
        {
            return Native.GetEnvironmentCubemapMode();
        }

        /// <summary>
        /// Change virtual camera rendering position between users eyes and video see through cameras.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The scene is rendered twice from the position of the users eyes. In full VR the eye position corresponds to the physical
        /// position of the users eyes. When using video see through there is a physical offset between the users eyes and the
        /// stereo camera pair. So in contrast to full VR, when rendering in MR mode: To remove stereo disparity problems between the
        /// real and virtual world and prevent 'floating' of the VR objects anchored to the real world, the scene should be rendered
        /// from the physical position of the camera pair in most cases. This is the default mode and corresponds to setting eye offset
        /// 'percentage' to 1.0.
        /// </para>
        /// <para>
        /// But there can be scenes that are predominantly VR where it makes sense to render the scene using the VR eye position.
        /// e.g. The scene only contains a small 'magic window' to view the real world or the real world is only shown as a backdrop.
        /// </para>
        /// <para>
        /// This function can be used to switch the rendering position. It can be used for smooth interpolated change in case it
        /// needs to be done the middle of the scene.
        /// </para>
        /// <para>
        /// Note: This setting is ignored if eye reprojection is enabled (#varjo_CameraPropertyType_EyeReprojection). In this case
        /// the rendering is always done from the users eye position (full VR position, corresponds to 'percentage' 0.0).
        /// </para>
        /// </remarks>
        /// <param name="percentage">
        ///  [0.0, 1.0] Linear interpolation of the rendering position between the position of HMD users eyes and the video see through camera position.
        /// </param>
        public static void SetVRViewOffset(double percentage)
        {
            if (!IsMRReady()) return;
            Native.varjo_MRSetVRViewOffset(Varjo.GetVarjoSession(), percentage);
        }

        /// <summary>
        /// Locks camera configuration. This is required for the client application to be able to modify
        /// camera parameters.
        /// </summary>
        /// <returns>True, if lock acquired successfully. Otherwise false.</returns>
        public static bool LockCameraConfig()
        {
            if (!IsMRReady()) return false;
            return (Native.varjo_Lock(Varjo.GetVarjoSession(), VARJO_LOCKTYPE_CAMERA) == 1);
        }

        /// <summary>
        /// Unlocks camera configuration. Client should use this when it no longer want to change camera
        /// parameters.
        /// </summary>
        public static void UnlockCameraConfig()
        {
            if (!IsMRReady()) return;
            Native.varjo_Unlock(Varjo.GetVarjoSession(), VARJO_LOCKTYPE_CAMERA);
        }

        /// <summary>
        /// Retrieves the number of available modes for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <returns>The number of modes for the camera property.</returns>
        public static int GetCameraPropertyModeCount(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return 0;
            return Native.varjo_MRGetCameraPropertyModeCount(Varjo.GetVarjoSession(), type);
        }

        /// <summary>
        /// Retrieves all available modes for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="modes">The available modes for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyModes(VarjoCameraPropertyType type, out List<VarjoCameraPropertyMode> modes)
        {
            if (!IsMRReady())
            {
                modes = new List<VarjoCameraPropertyMode>();
                return false;
            }

            int count = GetCameraPropertyModeCount(type);
            VarjoCameraPropertyMode[] modesArray = new VarjoCameraPropertyMode[count];
            Native.varjo_MRGetCameraPropertyModes(Varjo.GetVarjoSession(), type, modesArray, count);
            modes = modesArray.ToList();
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Retrieves the current mode for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="mode">The current mode for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyMode(VarjoCameraPropertyType type, out VarjoCameraPropertyMode mode)
        {
            if (!IsMRReady())
            {
                mode = VarjoCameraPropertyMode.Off;
                return false;
            }

            mode = (VarjoCameraPropertyMode)Native.varjo_MRGetCameraPropertyMode(Varjo.GetVarjoSession(), type);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Sets the current mode for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="mode">The mode to set.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool SetCameraPropertyMode(VarjoCameraPropertyType type, VarjoCameraPropertyMode mode)
        {
            if (!IsMRReady()) return false;
            Native.varjo_MRSetCameraPropertyMode(Varjo.GetVarjoSession(), type, mode);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Retrieves the camera property's configuration type, i.e. does it accept only selected list values or
        /// a range of values.
        /// </summary>
        /// <param name="prop">The camera property type.</param>
        /// <param name="configType">The camera property's configuration type.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyConfigType(VarjoCameraPropertyType prop, out VarjoCameraPropertyConfigType configType)
        {
            if (!IsMRReady())
            {
                configType = VarjoCameraPropertyConfigType.List;
                return false;
            }

            configType = (VarjoCameraPropertyConfigType)Native.varjo_MRGetCameraPropertyConfigType(Varjo.GetVarjoSession(), prop);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Retrieves the number of possible values for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <returns>The number of possible values for the camera property.</returns>
        public static int GetCameraPropertyValueCount(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return 0;
            return Native.varjo_MRGetCameraPropertyValueCount(Varjo.GetVarjoSession(), type);
        }

        /// <summary>
        /// Retrieves all possible values for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="values">The list of possible values for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyValues(VarjoCameraPropertyType type, out List<VarjoCameraPropertyValue> values)
        {
            if (!IsMRReady())
            {
                values = new List<VarjoCameraPropertyValue>();
                return false;
            }

            int count = GetCameraPropertyValueCount(type);
            VarjoCameraPropertyValue[] valueArray = new VarjoCameraPropertyValue[count];
            Native.varjo_MRGetCameraPropertyValues(Varjo.GetVarjoSession(), type, valueArray, count);
            values = valueArray.ToList();
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Retrieves the current value for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="value">The current value of the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyValue(VarjoCameraPropertyType type, out VarjoCameraPropertyValue value)
        {
            if (!IsMRReady())
            {
                value = new VarjoCameraPropertyValue();
                return false;
            }

            value = Native.varjo_MRGetCameraPropertyValue(Varjo.GetVarjoSession(), type);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Sets the current value for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool SetCameraPropertyValue(VarjoCameraPropertyType type, VarjoCameraPropertyValue value)
        {
            if (!IsMRReady()) return false;
            Native.varjo_MRSetCameraPropertyValue(Varjo.GetVarjoSession(), type, ref value);
            return VarjoError.CheckError();
        }

        /// <summary>
        /// Resets the given camera property back to its default.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        public static void ResetCameraProperty(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return;
            Native.varjo_MRResetCameraProperty(Varjo.GetVarjoSession(), type);
        }

        /// <summary>
        /// Resets all camera properties back to their defaults.
        /// </summary>
        public static void ResetCameraProperties()
        {
            if (!IsMRReady()) return;
            Native.varjo_MRResetCameraProperties(Varjo.GetVarjoSession());
        }

        internal static bool GetDataStreamBufferId(long streamId, long frameNumber, long channelIndex, out long bufferId)
        {
            if (!IsMRReady())
            {
                bufferId = -1;
                return false;
            }
            bufferId = Native.varjo_GetBufferId(Varjo.GetVarjoSession(), streamId, frameNumber, channelIndex);
            return VarjoError.CheckError();
        }

        internal static bool LockDataStreamBuffer(long id)
        {
            if (!IsMRReady())
            {
                return false;
            }
            Native.varjo_LockDataStreamBuffer(Varjo.GetVarjoSession(), id);
            return VarjoError.CheckError();
        }

        internal static bool GetBufferMetadata(long id, out VarjoBufferMetadata metadata)
        {
            if (!IsMRReady())
            {
                metadata = new VarjoBufferMetadata();
                return false;
            }
            metadata = Native.varjo_GetBufferMetadata(Varjo.GetVarjoSession(), id);
            return VarjoError.CheckError();
        }

        internal static bool GetBufferCPUData(long id, out IntPtr cpuData)
        {
            if (!IsMRReady())
            {
                cpuData = IntPtr.Zero;
                return false;
            }
            cpuData = Native.varjo_GetBufferCPUData(Varjo.GetVarjoSession(), id);
            return VarjoError.CheckError();
        }

        internal static bool GetBufferGPUData(long id, out VarjoTexture gpuData)
        {
            if (!IsMRReady())
            {
                gpuData = new VarjoTexture();
                return false;
            }
            gpuData = Native.varjo_GetBufferGPUData(Varjo.GetVarjoSession(), id);
            return VarjoError.CheckError();
        }

        internal static void UnlockDataStreamBuffer(long id)
        {
            if (!IsMRReady()) return;
            Native.varjo_UnlockDataStreamBuffer(Varjo.GetVarjoSession(), id);
        }

        internal static bool SupportsDataStream(VarjoStreamType type)
        {
            if (!IsMRReady()) return false;
            return Native.MRSupportsDataStream(type);
        }

        internal static bool StartDataStream(VarjoStreamType type, VarjoStreamCallback callback, IntPtr userdata)
        {
            if (!IsMRReady()) return false;
            var customAttribs = callback.Method.CustomAttributes.Where(a => a.AttributeType.FullName.Equals("AOT.MonoPInvokeCallbackAttribute"));
            if (customAttribs.Count() == 0)
            {
                Debug.LogError($"{callback.Method} is missing the [AOT.MonoPInvokeCallback] attribute");
                return false;
            }
            return Native.MRStartDataStream(type, callback, userdata);
        }

        internal static void StopDataStream(VarjoStreamType type)
        {
            if (!IsMRReady()) return;
            Native.MRStopDataStream(type);
        }

        internal static VarjoStreamConfig GetStreamConfig(VarjoStreamType streamType)
        {
            return Native.GetStreamConfig(streamType);
        }

        internal static VarjoCameraIntrinsics GetCameraIntrinsics(long id, long frameNumber, long channelIndex)
        {
            return Native.varjo_GetCameraIntrinsics(Varjo.GetVarjoSession(), id, frameNumber, channelIndex);
        }

        internal static VarjoMatrix GetCameraExtrinsics(long id, long frameNumber, long channelIndex)
        {
            return Native.varjo_GetCameraExtrinsics(Varjo.GetVarjoSession(), id, frameNumber, channelIndex);
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR")]
            public static extern bool IsMRAvailable();

            [DllImport("VarjoUnityXR")]
            public static extern bool MRSupportsDataStream(VarjoStreamType streamType);

            [DllImport("VarjoUnityXR")]
            public static extern bool MRStartDataStream(VarjoStreamType streamType, VarjoStreamCallback callback, IntPtr userdata);

            [DllImport("VarjoUnityXR")]
            public static extern void MRStopDataStream(VarjoStreamType streamType);

            [DllImport("VarjoUnityXR")]
            public static extern VarjoStreamConfig GetStreamConfig(VarjoStreamType streamType);

            [DllImport("VarjoUnityXR")]
            public static extern bool SetDepthEstimation(bool enabled);

            // Environment cubemap functions

            [DllImport("VarjoUnityXR")]
            public static extern bool LockEnvironmentCubemapConfig();

            [DllImport("VarjoUnityXR")]
            public static extern void UnlockEnvironmentCubemapConfig();

            [DllImport("VarjoUnityXR")]
            public static extern bool SetEnvironmentCubemapMode(VarjoEnvironmentCubemapMode mode);

            [DllImport("VarjoUnityXR")]
            public static extern VarjoEnvironmentCubemapMode GetEnvironmentCubemapMode();

            // Functions imported directly from VarjoLib

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRSetVideoRender(IntPtr session, bool enabled);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRSetVideoDepthEstimation(IntPtr session, bool enabled);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRSetVRViewOffset(IntPtr session, double percentage);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_LockDataStreamBuffer(IntPtr session, long id);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_UnlockDataStreamBuffer(IntPtr session, long id);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern VarjoCameraIntrinsics varjo_GetCameraIntrinsics(IntPtr session, long id, long frameNumber, long channelIndex);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern VarjoMatrix varjo_GetCameraExtrinsics(IntPtr session, long id, long frameNumber, long index);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern long varjo_GetBufferId(IntPtr session, long id, long frameNumber, long index);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern VarjoBufferMetadata varjo_GetBufferMetadata(IntPtr session, long id);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern VarjoTexture varjo_GetBufferGPUData(IntPtr session, long id);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern IntPtr varjo_GetBufferCPUData(IntPtr session, long id);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern int varjo_Lock(IntPtr session, long lockType);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_Unlock(IntPtr session, long lockType);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern int varjo_MRGetCameraPropertyModeCount(IntPtr session, VarjoCameraPropertyType prop);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRGetCameraPropertyModes(IntPtr session, VarjoCameraPropertyType prop, [In, Out] VarjoCameraPropertyMode[] modes, int maxSize);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern long varjo_MRGetCameraPropertyMode(IntPtr session, VarjoCameraPropertyType type);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRSetCameraPropertyMode(IntPtr session, VarjoCameraPropertyType type, VarjoCameraPropertyMode mode);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern long varjo_MRGetCameraPropertyConfigType(IntPtr session, VarjoCameraPropertyType prop);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern int varjo_MRGetCameraPropertyValueCount(IntPtr session, VarjoCameraPropertyType prop);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern int varjo_MRGetCameraPropertyValues(IntPtr session, VarjoCameraPropertyType prop, [In, Out] VarjoCameraPropertyValue[] values, int maxSize);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern VarjoCameraPropertyValue varjo_MRGetCameraPropertyValue(IntPtr session, VarjoCameraPropertyType type);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRSetCameraPropertyValue(IntPtr session, VarjoCameraPropertyType type, ref VarjoCameraPropertyValue value);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRResetCameraProperty(IntPtr session, VarjoCameraPropertyType type);

            [DllImport("VarjoLib", CharSet = CharSet.Auto)]
            public static extern void varjo_MRResetCameraProperties(IntPtr session);
        }
    }
}
