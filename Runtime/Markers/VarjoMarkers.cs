// Copyright 2020 Varjo Technologies Oy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Varjo.XR
{
    [Flags] public enum VarjoMarkerPoseFlags
    {
        TrackingOk = 1,               //!< Pose is being currently tracked
        TrackingLost = 2,             //!< Pose has been tracked but currently is not detected by the tracking subsystem
        TrackingDisconnected = 4,     //!< Tracking subsystem is not connected and poses cannot be acquired
        HasPosition = 8,              //!< Pose has position information
        HasRotation = 16,             //!< Pose has rotation information
        HasVelocity = 32,             //!< Pose has velocity information
        HasAngularVelocity = 64,      //!< Pose has angular velocity information
        HasAcceleration = 128,        //!< Pose has acceleration information
        HasConfidence = 256,          //!< Pose has confidence information
    }

    public enum VarjoMarkerFlags
    {
        None = 0,
        DoPrediction = 1,             //!< Marker pose is predicted. If not specified, the latest detected pose is used.
    }

    public enum VarjoMarkerError
    {
        None = 0,                     //!< No error
        DuplicateID = 1,              //!< Several markers have the same ID
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VarjoMarker
    {
        public long id;
        public long timestamp;                  //!< Timestamp of the pose. This represents the time the pose has been extrapolated to.
        public float confidence;                //!< Tracker confidence in range [0.0, 1.0] (1.0 is accurate).//!< Id of the object. Valid for the lifetime of the world.
        public Pose pose;                       //!< Pose of the object in the global space.
        public Vector3 velocity;                //!< Linear velocity (m/s).
        public Vector3 angularVelocity;         //!< Angular velocity (radians/s).
        public Vector3 acceleration;            //!< Acceleration (m/s^2).
        public Vector3 size;                    //!< Size of the marker in meters.
        public VarjoMarkerPoseFlags poseFlags;  //!< Bit field value describing pose.
        public VarjoMarkerFlags flags;          //!< Marker flags.
        public VarjoMarkerError error;          //!< Marker error.
    };

    public class VarjoMarkers
    {
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool EnableVarjoMarkers(bool enabled);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool IsVarjoMarkersEnabled();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern int GetVarjoMarkerCount();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern int GetMarkers(VarjoMarker[] markers, int markerCount);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern int GetRemovedVarjoMarkerCount();

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern int GetRemovedMarkerIds(long[] removedIds, int removedMarkerCount);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool SetVarjoMarkerTimeout(long markerId, long durationMilliseconds);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool AddVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags);

        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        public static extern bool RemoveVarjoMarkerFlags(long markerId, VarjoMarkerFlags flags);

        public static int GetVarjoMarkers(out List<VarjoMarker> markers)
        {
            int markerCount = GetVarjoMarkerCount();
            VarjoMarker[] markerArray = new VarjoMarker[markerCount];
            if (markerCount > 0)
            {
                GetMarkers(markerArray, markerCount);
            }
            markers = markerArray.ToList();
            return markerCount;
        }

        public static int GetRemovedVarjoMarkerIds(out List<long> removedIds)
        {
            int removedMarkerCount = GetRemovedVarjoMarkerCount();
            long[] idArray = new long[removedMarkerCount];
            if (removedMarkerCount > 0)
            {
                removedMarkerCount = GetRemovedMarkerIds(idArray, removedMarkerCount);
            }
            removedIds = idArray.ToList();
            return removedMarkerCount;
        }
    }
}
