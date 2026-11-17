using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varjo.XR;

namespace VarjoExample
{
    public class MarkerManager : MonoBehaviour
    {
        private List<VarjoMarker> markers;
        private List<long> markerIds;
        private List<long> absentIds;
        private Dictionary<long, MarkerVisualizer> markerVisualizers;

        [SerializeField] Transform xrRig;
        [SerializeField] GameObject markerPrefab;

        [Header("Marker features (set on found)")]
        [SerializeField] long markerTimeout = 3000;
        [SerializeField] VarjoMarkerFlags markerFlags;

        void Start()
        {
            markers = new List<VarjoMarker>();
            markerIds = new List<long>();
            absentIds = new List<long>();
            markerVisualizers = new Dictionary<long, MarkerVisualizer>();
        }

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
            if (VarjoMarkers.IsVarjoMarkersEnabled())
            {
                markers.Clear();
                markerIds.Clear();
                int foundMarkers = VarjoMarkers.GetVarjoMarkers(out markers);
                if (markers.Count > 0)
                {
                    foreach (var marker in markers)
                    {
                        markerIds.Add(marker.id);
                        if (markerVisualizers.ContainsKey(marker.id))
                        {
                            UpdateMarkerVisualizer(marker);
                        }
                        else
                        {
                            CreateMarkerVisualizer(marker);
                            VarjoMarkers.SetVarjoMarkerTimeout(marker.id, markerTimeout);
                            VarjoMarkers.AddVarjoMarkerFlags(marker.id, markerFlags);
                        }
                    }
                }

                VarjoMarkers.GetRemovedVarjoMarkerIds(out absentIds);

                foreach (var id in absentIds)
                {
                    if (markerVisualizers.ContainsKey(id))
                    {
                        Destroy(markerVisualizers[id].gameObject);
                        markerVisualizers.Remove(id);
                    }
                    markerIds.Remove(id);
                }
                absentIds.Clear();
            }

            if (markerIds.Count == 0 && markerVisualizers.Count > 0)
            {
                var ids = markerVisualizers.Keys.ToArray();
                foreach (var id in ids)
                {
                    Destroy(markerVisualizers[id].gameObject);
                    markerVisualizers.Remove(id);
                }
            }
        }

        void CreateMarkerVisualizer(VarjoMarker marker)
        {
            GameObject go = Instantiate(markerPrefab);
            go.name = marker.id.ToString();
            go.transform.SetParent(xrRig);
            MarkerVisualizer visualizer = go.GetComponent<MarkerVisualizer>();
            markerVisualizers.Add(marker.id, visualizer);
            visualizer.SetMarkerData(marker);
        }

        void UpdateMarkerVisualizer(VarjoMarker marker)
        {
            markerVisualizers[marker.id].SetMarkerData(marker);
        }
    }
}