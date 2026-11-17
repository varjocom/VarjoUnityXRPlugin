using UnityEngine;

namespace VarjoExample
{
    public class ConstantRotate : MonoBehaviour
    {
        [SerializeField] Vector3 rotateAxis;
        [SerializeField] float rotateSpeed;

        void Update()
        {
            transform.Rotate(rotateAxis, Time.deltaTime * rotateSpeed);
        }
    }
}
