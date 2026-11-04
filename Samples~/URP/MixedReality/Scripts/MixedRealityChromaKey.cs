using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using Varjo.XR;

namespace VarjoExample
{
    public class MixedRealityChromaKey : MonoBehaviour
    {
        [SerializeField] Color chromaKeyHue = Color.green;
        [SerializeField][Range(0.0f, 1.0f)] float hueTolerance = 0.9f;
        [SerializeField][Range(0.0f, 1.0f)] float valueTolerance = 0.9f;
        [SerializeField][Range(0.0f, 1.0f)] float saturationTolerance = 0.9f;

        void Start()
        {
            VarjoChromaKeyParams chromaKeyParams;
            Color.RGBToHSV(chromaKeyHue, out float hue, out float sat, out float val);
            chromaKeyParams.hue = hue;
            chromaKeyParams.hsvTolerance = new Vector3(hueTolerance, valueTolerance, saturationTolerance);
            VarjoChromaKey.SetChromaKeyParameters(0, chromaKeyParams);
            VarjoChromaKey.EnableChromaKey(true, false);
        }
    }
}
