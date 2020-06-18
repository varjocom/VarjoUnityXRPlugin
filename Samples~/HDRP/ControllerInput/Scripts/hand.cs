using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VarjoExample
{
    public class hand : MonoBehaviour
    {
        [Header("Select root of XR rig")]
        public Transform xrRig;

        Controller controller;
        public List<Interactable> contactedInteractables = new List<Interactable>();
        private bool triggerDown;
        private FixedJoint fixedJoint = null;
        private Interactable currentInteractable;
        private Rigidbody heldObjectBody;

        void Awake()
        {
            controller = GetComponent<Controller>();
            fixedJoint = GetComponent<FixedJoint>();
        }

        // Update is called once per frame
        void Update()
        {
            if (controller.triggerButton)
            {
                if (!triggerDown)
                {
                    //Debug.Log("triggerButton Pressed");
                    triggerDown = true;
                    Pick();
                }
                else
                {
                    //Debug.Log("triggerButton Down");
                }
            }
            else if (!controller.primary2DAxisClick && triggerDown)
            {
                //Debug.Log("triggerButton Released");
                triggerDown = false;
                Drop();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Pickable") || other.gameObject.CompareTag("Fracture"))
            {
                contactedInteractables.Add(other.gameObject.GetComponent<Interactable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Pickable") || other.gameObject.CompareTag("Fracture"))
            {
                contactedInteractables.Remove(other.gameObject.GetComponent<Interactable>());
            }
        }

        public void Pick()
        {

            //Get nearest object contacted
            currentInteractable = GetNearestInteractable();

            //NullCheck current interactable
            if (!currentInteractable)
            {
                return;
            }

            // Check if alreadyu held
            if (currentInteractable.activeHand)
            {
                currentInteractable.activeHand.Drop();
            }

            //attach 
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            fixedJoint.connectedBody = heldObjectBody;

            //set active hand
            currentInteractable.activeHand = this;
        }

        public void Drop()
        {
            // Null check
            if (!currentInteractable)
                return;

            //Detach
            fixedJoint.connectedBody = null;

            //apply velocity
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            heldObjectBody.velocity = xrRig.TransformVector(controller.DeviceVelocity);
            heldObjectBody.angularVelocity = xrRig.TransformDirection(controller.DeviceAngularVelocity);

            //clear
            currentInteractable.activeHand = null;
            currentInteractable = null;
        }

        private Interactable GetNearestInteractable()
        {
            Interactable nearest = null;
            float minDistance = float.MaxValue;
            float distance = 0.0f;

            foreach (Interactable interactable in contactedInteractables)
            {
                distance = (interactable.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = interactable;
                }
            }
            return nearest;
        }
    }
}