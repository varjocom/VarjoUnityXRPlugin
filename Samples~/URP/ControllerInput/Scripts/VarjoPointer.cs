using UnityEngine;
using UnityEngine.EventSystems;

namespace VarjoExample
{
    public class VarjoPointer : MonoBehaviour
    {
        [Header("Input module for canvas ui hits")]
        [SerializeField] VarjoInputModule varjoInputModule;

        [Header("Controller for ui interaction")]
        [SerializeField] Controller controller;

        [Header("Object to show where user is aiming")]
        [SerializeField] GameObject pointerVisual;

        [Header("How far visuals should hover from hit position")]
        [SerializeField] float distanceFromHit = 0.01f;

        [Header("Decrease to smooth out micro movements")]
        [SerializeField] float responsiveness = 10.0f;

        void Awake()
        {
            if (varjoInputModule == null)
            {
                Debug.LogError("Varjo pointer is missing input module reference. Disabling pointer.", gameObject);
                enabled = false;
                return;
            }

            if (pointerVisual == null)
            {
                Debug.LogWarning("Varjo pointer is missing pointer visuals.", gameObject);
            }
        }

        void Update()
        {
            if (pointerVisual != null)
            {
                // By default do not show the pointer
                bool showPointer = false;

                // Show pointer on canvas if possible
                if (varjoInputModule.PointerHitCanvas)
                {
                    showPointer = true;
                    Vector3 canvasNormal = varjoInputModule.targetCanvas.transform.forward;
                    Vector3 targetPosition = varjoInputModule.PointerWorldPosition - canvasNormal * distanceFromHit;
                    pointerVisual.transform.position = Vector3.Lerp(pointerVisual.transform.position, targetPosition, responsiveness * Time.smoothDeltaTime);
                    pointerVisual.transform.forward = canvasNormal;
                }

                // Update pointer visibility if needed
                if (pointerVisual.activeSelf != showPointer)
                {
                    pointerVisual.SetActive(showPointer);
                }
            }
        }
    }
}