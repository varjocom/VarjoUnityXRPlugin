using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;

public class MarkerVisualizer : MonoBehaviour
{
    public TextMesh idText;

    public void SetMarkerData(VarjoMarker marker)
    {
        transform.localPosition = marker.pose.position;
        transform.localRotation = marker.pose.rotation;
        transform.localScale = new Vector3(marker.size.x, marker.size.x, marker.size.z);
        idText.text = marker.id.ToString();
    }
}
