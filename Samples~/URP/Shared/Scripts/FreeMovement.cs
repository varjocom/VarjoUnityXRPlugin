using UnityEngine;
using UnityEngine.InputSystem;

namespace VarjoExample
{
    public class FreeMovement : MonoBehaviour
    {
        [SerializeField] Transform rig;
        [SerializeField] Transform head;

        [Header("Use WASD/QE/ZX to move in Play mode")]
        [SerializeField] float moveSpeed = 10.0f;
        [SerializeField] float turnSpeed = 200.0f;

        [Header("Lock movement to horizontal place")]
        [SerializeField] bool moveOnXZPlane = true;

        void Start()
        {
            if(rig == null || head == null)
            {
                Debug.LogWarning("No rig or head assigned", gameObject);
                enabled = false;
            }
        }

        void Update()
        {
            float turnLeft = Keyboard.current.zKey.isPressed ? -1.0f : 0.0f;
            float turnRight = Keyboard.current.xKey.isPressed ? 1.0f : 0.0f;

            float forward = Keyboard.current.wKey.isPressed ? 1.0f : 0.0f;
            float backward = Keyboard.current.sKey.isPressed ? 1.0f : 0.0f;

            float left = Keyboard.current.aKey.isPressed ? 1.0f : 0.0f;
            float right = Keyboard.current.dKey.isPressed ? 1.0f : 0.0f;

            float up = Keyboard.current.eKey.isPressed ? 1.0f : 0.0f;
            float down = Keyboard.current.qKey.isPressed ? 1.0f : 0.0f;

            rig.RotateAround(head.position, Vector3.up, turnSpeed * turnLeft * Time.deltaTime);
            rig.RotateAround(head.position, Vector3.up, turnSpeed * turnRight * Time.deltaTime);

            rig.Translate(rig.up * moveSpeed * up * Time.deltaTime, Space.World);
            rig.Translate(-rig.up * moveSpeed * down * Time.deltaTime, Space.World);

            if (moveOnXZPlane)
            {
                rig.Translate(VectorYToZero(head.forward) * moveSpeed * forward * Time.deltaTime, Space.World);
                rig.Translate(VectorYToZero(-head.forward) * moveSpeed * backward * Time.deltaTime, Space.World);

                rig.Translate(VectorYToZero(head.right) * moveSpeed * right * Time.deltaTime, Space.World);
                rig.Translate(VectorYToZero(-head.right) * moveSpeed * left * Time.deltaTime, Space.World);
            }
            else
            {
                rig.Translate(head.forward * moveSpeed * forward * Time.deltaTime, Space.World);
                rig.Translate(-head.forward * moveSpeed * backward * Time.deltaTime, Space.World);

                rig.Translate(head.right * moveSpeed * right * Time.deltaTime, Space.World);
                rig.Translate(-head.right * moveSpeed * left * Time.deltaTime, Space.World);
            }
        }

        Vector3 VectorYToZero(Vector3 v)
        {
            return new Vector3(v.x, 0.0f, v.z);
        }
    }
}

