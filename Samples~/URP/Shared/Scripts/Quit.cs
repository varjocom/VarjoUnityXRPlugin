using UnityEngine;
using UnityEngine.InputSystem;

namespace VarjoExample
{
    public class Quit : MonoBehaviour
    {
        [SerializeField] Key quitKey = Key.Escape;

        void Update()
        {
            if (Keyboard.current[quitKey].wasPressedThisFrame)
            {
                Application.Quit();
            }
        }
    }
}