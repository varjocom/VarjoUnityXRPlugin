
using System;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;

namespace VarjoExample
{
    public class MarkerMask : MonoBehaviour
    {
        [Serializable]
        public struct TrackedObject
        {
            public long markerId;
            public GameObject target;
            public bool controlVisibility;
            public long markerTimeout;
            public VarjoMarkerFlags markerFlags;
            public bool trackPosition;
            public bool trackRotation;
            public bool trackScale;
        }

        // A array for all the tracked objects
        [SerializeField] TrackedObject[] trackedObjects = new TrackedObject[1];

        // A list for found markers
        private List<VarjoMarker> markers = new List<VarjoMarker>();

        // A list for IDs of removed markers
        private List<long> removedMarkerIds = new List<long>();

        void OnEnable()
        {
            VarjoMarkers.EnableVarjoMarkers(true);
        }

        void OnDisable()
        {
            VarjoMarkers.EnableVarjoMarkers(false);
        }

        void Update()
        {
            // Check if Varjo Marker tracking is enabled and functional
            if (VarjoMarkers.IsVarjoMarkersEnabled())
            {
                // Get a list of markers with up-to-date data
                VarjoMarkers.GetVarjoMarkers(out markers);

                // Loop through found markers and update gameobjects matching the marker ID in the array
                foreach (var marker in markers)
                {
                    for (var i = 0; i < trackedObjects.Length; i++)
                    {
                        if (trackedObjects[i].markerId == marker.id)
                        {
                            if(trackedObjects[i].controlVisibility)
                            { 
                                trackedObjects[i].target.SetActive(true);
                            }

                            if (trackedObjects[i].trackPosition)
                            {
                                trackedObjects[i].target.transform.localPosition = marker.pose.position;
                            }

                            if(trackedObjects[i].trackRotation)
                            {
                                trackedObjects[i].target.transform.localRotation = marker.pose.rotation;
                            }

                            if (trackedObjects[i].trackScale)
                            {
                                trackedObjects[i].target.transform.localScale = new Vector3(marker.size.x, marker.size.x, marker.size.z);
                            }

                            VarjoMarkers.SetVarjoMarkerTimeout(marker.id, trackedObjects[i].markerTimeout);
                            VarjoMarkers.AddVarjoMarkerFlags(marker.id, trackedObjects[i].markerFlags);
                        }
                    }
                }

                // Get a list of IDs of removed markers
                VarjoMarkers.GetRemovedVarjoMarkerIds(out removedMarkerIds);

                // Loop through removed marker IDs and deactivate gameobjects matching the marker IDs in the array
                foreach (var id in removedMarkerIds)
                {
                    for (var i = 0; i < trackedObjects.Length; i++)
                    {
                        if (trackedObjects[i].markerId == id)
                        {
                            if (trackedObjects[i].controlVisibility)
                            {
                                trackedObjects[i].target.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
