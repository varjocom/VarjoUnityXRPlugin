using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByPosition : MonoBehaviour
{
    public float destroyPositionY = -10;
    void Update()
    {
        if(transform.position.y < destroyPositionY) 
        {
            Destroy(this.gameObject); //Destroy gameobject if fallen 10 meters below ground level
        }
    }
}
