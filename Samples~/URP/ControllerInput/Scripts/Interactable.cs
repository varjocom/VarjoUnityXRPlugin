using UnityEngine;

namespace VarjoExample
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {
        [HideInInspector]
        public Hand activeHand = null;
    }
}