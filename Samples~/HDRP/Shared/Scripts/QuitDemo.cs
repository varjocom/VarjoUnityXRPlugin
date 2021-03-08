using UnityEngine;

public class QuitDemo : MonoBehaviour
{
    public KeyCode quitDemo = KeyCode.Escape;
    void Update()
    {
        if (Input.GetKey(quitDemo))
        {
            Application.Quit();
        }
    }
}