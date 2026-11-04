using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace VarjoExample
{
    public class Recenter : MonoBehaviour
    {
        [SerializeField] Key recenterKey = Key.R;

        void Update()
        {
            if (Keyboard.current[recenterKey].wasPressedThisFrame)
            {
                XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>().TryRecenter();
            }
        }
    }
}
