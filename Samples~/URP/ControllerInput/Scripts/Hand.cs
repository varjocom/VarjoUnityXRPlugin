using System.Collections.Generic;
using UnityEngine;

namespace VarjoExample
{
    [RequireComponent(typeof(Controller))]
    [RequireComponent(typeof(FixedJoint))]
    public class Hand : MonoBehaviour
    {
        [SerializeField] Transform xrRig;
        private List<Interactable> contactedInteractables = new List<Interactable>();
        private bool triggerDown;
        private FixedJoint fixedJoint;
        private Interactable currentInteractable;
        private Rigidbody heldObjectBody;

        Controller controller;

        void Awake()
        {
            controller = GetComponent<Controller>();
            fixedJoint = GetComponent<FixedJoint>();
        }

        // Update is called once per frame
        void Update()
        {
            if (controller.TriggerButton)
            {
                if (!triggerDown)
                {
                    triggerDown = true;
                    Pick();
                }
            }
            else if (!controller.Primary2DAxisClick && triggerDown)
            {
                triggerDown = false;
                Drop();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Interactable interactable = other.GetComponent<Interactable>();
            if(interactable != null)
            {
                contactedInteractables.Add(interactable);
            }
        }

        void OnTriggerExit(Collider other)
        {
            Interactable interactable = other.GetComponent<Interactable>();
            if (interactable != null)
            {
                contactedInteractables.Remove(interactable);
            }
        }

        public void Pick()
        {
            currentInteractable = GetNearestInteractable();

            if (!currentInteractable)
            {
                return;
            }

            // Drop interactable if already held
            if (currentInteractable.activeHand)
            {
                currentInteractable.activeHand.Drop();
            }

            // Attach
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            fixedJoint.connectedBody = heldObjectBody;

            // Set active hand
            currentInteractable.activeHand = this;
        }

        public void Drop()
        {
            if (!currentInteractable)
                return;

            // Detach
            fixedJoint.connectedBody = null;

            // Apply velocity
            heldObjectBody = currentInteractable.GetComponent<Rigidbody>();
            heldObjectBody.linearVelocity = xrRig.TransformVector(controller.DeviceVelocity);
            heldObjectBody.angularVelocity = xrRig.TransformDirection(controller.DeviceAngularVelocity);

            // Clear
            currentInteractable.activeHand = null;
            currentInteractable = null;
        }

        Interactable GetNearestInteractable()
        {
            Interactable nearest = null;
            float minDistance = float.MaxValue;
            float distance = 0.0f;

            foreach (Interactable interactable in contactedInteractables)
            {
                if (interactable)
                {
                    distance = (interactable.transform.position - transform.position).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = interactable;
                    }
                }
            }
            return nearest;
        }
    }
}