using UnityEngine;

public class DestroyByPosition : MonoBehaviour
{
    public float destroyPositionY = -10;
    void Update()
    {
        if(transform.position.y < destroyPositionY)
        {
            Destroy(this.gameObject);
        }
    }
}
