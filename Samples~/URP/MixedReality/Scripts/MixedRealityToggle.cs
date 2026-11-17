using UnityEngine;
using UnityEngine.InputSystem;
using Varjo.XR;

namespace VarjoExample
{
    public class MixedRealityToggle : MonoBehaviour
    {
        [SerializeField] Key toggleMixedReality = Key.M;

        public bool MixedRealityEnabled { get; private set; }

        private bool originalOpaqueValue;

        void OnEnable()
        {
            SetMixedReality(true);
        }

        private void OnDisable()
        {
            SetMixedReality(false);
        }

        void Update()
        {
            if (Keyboard.current[toggleMixedReality].wasPressedThisFrame)
            {
                SetMixedReality(!MixedRealityEnabled);
            }
        }

        public void SetMixedReality(bool enabled)
        {
            MixedRealityEnabled = enabled && VarjoMixedReality.IsMRReady();
            if (MixedRealityEnabled)
            {
                MixedRealityEnabled = VarjoMixedReality.StartRender();
                if(MixedRealityEnabled)
                { 
                    originalOpaqueValue = VarjoRendering.GetOpaque();
                    VarjoRendering.SetOpaque(false);
                }
            }
            else
            {
                VarjoMixedReality.StopRender();
                VarjoRendering.SetOpaque(originalOpaqueValue);
            }
            Debug.Log("Varjo Mixed reality: " + MixedRealityEnabled, gameObject);
        }
    }
}
