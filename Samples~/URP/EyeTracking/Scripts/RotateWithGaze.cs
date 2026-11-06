using UnityEngine;

namespace VarjoExample
{
    public class RotateWithGaze : MonoBehaviour
    {
        [SerializeField] float forceAmount;
    
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponentInChildren<Rigidbody>();
        }

        // Rotates object hit with gaze tracking raycast
        public void GazeHit()
        {
            rb.AddTorque(Vector3.up * forceAmount * Time.deltaTime, ForceMode.Force);
        }
    }
}
