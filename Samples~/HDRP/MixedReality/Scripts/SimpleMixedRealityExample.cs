using UnityEngine;
using Varjo.XR;

public class SimpleMixedRealityExample : MonoBehaviour
{
    public bool mixedReality;
    public KeyCode MixedRealityToggle = KeyCode.Space;

    private bool mixedRealityEnabled = false;
    private bool originalOpaqueValue;

    void Start()
    {
        if (mixedReality)
        {
            VarjoMixedReality.StartRender();
            mixedRealityEnabled = true;
            originalOpaqueValue = VarjoRendering.GetOpaque();
            VarjoRendering.SetOpaque(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(MixedRealityToggle))
        {
            mixedReality = !mixedReality;
        }

        if (mixedReality != mixedRealityEnabled)
        {
            if (mixedReality)
            {
                VarjoMixedReality.StartRender();
                originalOpaqueValue = VarjoRendering.GetOpaque();
                VarjoRendering.SetOpaque(false);
            }
            else
            {
                VarjoMixedReality.StopRender();
                VarjoRendering.SetOpaque(originalOpaqueValue);
            }

            mixedRealityEnabled = mixedReality;
        }
    }
}
