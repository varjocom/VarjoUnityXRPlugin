using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    public Vector3 rotateAxis;
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateAxis, Time.deltaTime * rotateSpeed);
    }
}
