// Copyright 2020 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Varjo.XR
{
    /// <summary>
    /// Varjo Marker Pose Flags
    /// </summary>
    [Flags] public enum VarjoMarkerPoseFlags
    {
        /** <summary>Pose is being currently tracked.</summary> */
        TrackingOk = 1,
        /** <summary>Pose has been tracked but currently is not detected by the tracking subsystem.</summary> */
        TrackingLost = 2,
        /** <summary>Tracking subsystem is not connected and poses cannot be acquired.</summary> */
        TrackingDisconnected = 4,
        /** <summary>Pose has position information.</summary> */
        HasPosition = 8,
        /** <summary>Pose has rotation information.</summary> */
        HasRotation = 16,
        /** <summary>Pose has velocity information.</summary> */
        HasVelocity = 32,
        /** <summary>Pose has angular velocity information.</summary> */
        HasAngularVelocity = 64,
        /** <summary>Pose has acceleration information.</summary> */
        HasAcceleration = 128,
        /** <summary>Pose has confidence information.</summary> */
        HasConfidence = 256,
    }

    /// <summary>
    /// Varjo Marker Flags
    /// </summary>
    public enum VarjoMarkerFlags
    {
        None = 0,
        /** <summary>Marker pose is predicted. If not specified, the latest detected pose is used.</summary> */
        DoPrediction = 1,
    }

    /// <summary>
    /// Varjo Marker Error
    /// </summary>
    public enum VarjoMarkerError
    {
        /** <summary>No error.</summary> */
        None = 0,
        /** <summary>Several markers have the same ID.</summary> */
        DuplicateID = 1,
    }

    /// <summary>
    /// Varjo Unity Object Marker.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoMarker
    {
        /** <summary>Id of the object. Valid for the lifetime of the world.</summary> */
        public long id;
        /** <summary>Timestamp of the pose. This represents the time the pose has been extrapolated to.</summary> */
        public long timestamp;
        /** <summary>Tracker confidence in range [0.0, 1.0] (1.0 is accurate).</summary> */
        public float confidence;
        /** <summary>Pose of the object in the global space.</summary> */
        public Pose pose;
        /** <summary>Linear velocity (m/s).</summary> */
        public Vector3 velocity;
        /** <summary>Angular velocity (radians/s).</summary> */
        public Vector3 angularVelocity;
        /** <summary>Acceleration (m/s^2).</summary> */
        public Vector3 acceleration;
        /** <summary>Size of the marker in meters.</summary> */
        public Vector3 size;
        /** <summary>Bit field value describing pose.</summary> */
        public VarjoMarkerPoseFlags poseFlags;
        /** <summary>Marker flags.</summary> */
        public VarjoMarkerFlags flags;
        /** <summary>Marker error.</summary> */
        public VarjoMarkerError error;
    };

    public class VarjoMarkers
    {
        /// <summary>
        /// Enable Varjo Marker tracking.
        /// </summary>
        /// <param name="enabled">Value.</param>
        /// <returns>True if markers is enabled.</returns>
        public static bool EnableVarjoMarkers(bool enabled)
        {
            if (!VarjoMixedReality.IsMRReady()) return false;
            if (!Native.EnableVarjoMarkers(enabled))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if Varjo Marker tracking is enabled and functional.
        /// </summary>
        /// <returns>True if enabled.</returns>
        public static bool IsVarjoMarkersEnabled() { return Native.IsVarjoMarkersEnabled(); }

        /// <summary>
        /// Get Varjo Marker Count.
        /// </summary>
        /// <returns>Count.</returns>
        public static int GetVarjoMarkerCount()
        {
            int count = Native.GetVarjoMarkerCount();
            VarjoError.CheckError();
            return count;
        }

        /// <summary>
        /// Get Removed Varjo Marker Count.
        /// </summary>
        /// <returns>Removed Varjo Marker Count.</returns>
        public static int GetRemovedVarjoMarkerCount()
        {
            int count = Native.GetRemovedVarjoMarkerCount();
            VarjoError.CheckError();
            return count;
        }

        /// <summary>
        /// Sets a desired lifetime duration for specified marker.
        /// </summary>
        /// <param name="markerId">Marker.</param>
        /// <param name="durationMilliseconds">Duration in milliseconds.</param>
        public static bool SetVarjoMarkerTimeout(long markerId, long durationMilliseconds)
        {
            if (!Native.SetVarjoMarkerTimeout(markerId, durationMilliseconds))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets flags for specifed markers.
        /// </summary>
        /// <param name="markerId">Id of the marker to set the flags for.</param>
        /// <param name="flags">Marker flags to set.</param>
        /// <returns>True if succeeded.</returns>
        public static bool AddVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags)
        {
            if (!Native.AddVarjoMarkerFlags(markerId, flags))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Remove flags for specifed markers.
        /// </summary>
        /// <param name="markerId">Id of the marker to remove the flags for.</param>
        /// <param name="flags">Marker flags to set.</param>
        /// <returns>True if succeeded.</returns>
        public static bool RemoveVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags)
        {
            if (!Native.RemoveVarjoMarkerFlags(markerId, flags))
            {
                VarjoError.CheckError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a list of markers with up-to-date data.
        /// </summary>
        /// <param name="markers">List of markers to get.</param>
        /// <returns>Number of markers in the list.</returns>
        public static int GetVarjoMarkers(out List<VarjoMarker> markers)
        {
            int markerCount = Native.GetVarjoMarkerCount();
            VarjoMarker[] markerArray = new VarjoMarker[markerCount];
            if (markerCount > 0)
            {
                Native.GetMarkers(markerArray, markerCount);
            }

            if (!VarjoError.CheckError())
            {
                markers = new List<VarjoMarker>();
                return 0;
            }

            markers = markerArray.ToList();
            return markerCount;
        }

        /// <summary>
        /// Get a list of IDs of removed markers.
        /// </summary>
        /// <param name="removedIds">List of IDs to get.</param>
        /// <returns>Number of markers in the list.</returns>
        public static int GetRemovedVarjoMarkerIds(out List<long> removedIds)
        {
            int removedMarkerCount = Native.GetRemovedVarjoMarkerCount();
            long[] idArray = new long[removedMarkerCount];
            if (removedMarkerCount > 0)
            {
                removedMarkerCount = Native.GetRemovedMarkerIds(idArray, removedMarkerCount);
            }

            if (!VarjoError.CheckError())
            {
                removedIds = new List<long>();
                return 0;
            }

            removedIds = idArray.ToList();
            return removedMarkerCount;
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool EnableVarjoMarkers(bool enabled);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool IsVarjoMarkersEnabled();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetVarjoMarkerCount();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetMarkers(VarjoMarker[] markers, int markerCount);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetRemovedVarjoMarkerCount();

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern int GetRemovedMarkerIds(long[] removedIds, int removedMarkerCount);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool SetVarjoMarkerTimeout(long markerId, long durationMilliseconds);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool AddVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags);

            [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
            public static extern bool RemoveVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags);
        }
    }
}
