using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    public Vector3 rotateAxis;
    public float rotateSpeed;

    void Update()
    {
        transform.Rotate(rotateAxis, Time.deltaTime * rotateSpeed);
    }
}
