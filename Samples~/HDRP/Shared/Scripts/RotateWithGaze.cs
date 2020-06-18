using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithGaze : MonoBehaviour
{
    public GameObject demoObject;
    public float forceAmount;
    public Material flatMaterial;
    public Material objectMaterial;
    Rigidbody rb;
    MeshRenderer mr;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        mr = demoObject.GetComponent<MeshRenderer>();
    }


    void Update()
    {
        //Check if object is rotating and change material accordingly
        if (!rb.IsSleeping())
        {
            mr.material = objectMaterial;
        }
        else
        {
            mr.material = flatMaterial;
        }
    }

    public void RayHit() // Rotates object hit with gaze tracking raycast
    {
        rb.AddTorque(Vector3.up * forceAmount * Time.deltaTime, ForceMode.Force);
    }
}
