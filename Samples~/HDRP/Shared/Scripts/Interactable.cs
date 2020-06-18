using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VarjoExample
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {
        [HideInInspector]
        public hand activeHand = null;
    }
}