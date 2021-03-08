using UnityEngine;

public class Fracture : MonoBehaviour
{
    public GameObject fracturedObject;

    public void Destroy()
    {
        Instantiate(fracturedObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
