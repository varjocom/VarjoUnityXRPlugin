using UnityEngine;
using Varjo.XR;

namespace VarjoExample
{
    public class MarkerVisualizer : MonoBehaviour
    {
        [SerializeField] TextMesh idText;
        [SerializeField] bool updatePosition = true;
        [SerializeField] bool updateRotation = true;
        [SerializeField] bool updateScale = true;

        public void SetMarkerData(VarjoMarker marker)
        {
            if(idText)
            { 
                idText.text = marker.id.ToString();
            }

            if (updatePosition)
            {
                transform.localPosition = marker.pose.position;
            }

            if(updateRotation)
            {
                transform.localRotation = marker.pose.rotation;
            }

            if(updateScale)
            {
                transform.localScale = new Vector3(marker.size.x, marker.size.x, marker.size.z);
            }
        }
    }
}