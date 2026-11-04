using System.Collections.Generic;
using UnityEngine;

namespace VarjoExample
{
    public class Teleporter : MonoBehaviour
    {
        // Public variables to customize teleport, set in inspector
        [Header("Variables")]
        [SerializeField] Color validTeleportColor = Color.cyan;
        [SerializeField] Color invalidTeleportColor = Color.red;

        [SerializeField] LayerMask teleportableLayers;        // Collision layers for teleport arc
        [SerializeField] float maxTargetSurfaceAngle = 10.0f;   // Prevent teleporting in walls

        [SerializeField] float velocity = 5.0f;                 // Velocity for simulated trajectory
        [SerializeField] float gravity = 25.0f;                 // Gravity for simulated trajectory
        [SerializeField] float controllerPitchFactor = 1.0f;    // How much controller angle affects the velocity
        [SerializeField] int pointsOnArc = 30;                // Affects performance and visual quality

        // Public scene references, set in inspector
        [Header("References")]
        [SerializeField] Transform teleportPointer;           // Teleport pointer
        [SerializeField] Transform xrRig;                     // What should we move when teleporting
        [SerializeField] Transform mainCamera;                // Where is our head
        [SerializeField] GameObject teleportTargetPrefab;     // Visual representation of teleport target

        private List<Vector3> arc = new List<Vector3>();    // List of points on the arc
        private LineRenderer teleportArcRenderer;           // Arc renderer
        private Transform teleportTarget;                   // Instantiated target visuals

        private bool buttonDown;
        private bool canTeleport;                           // Can we teleport to pointer target
        private RaycastHit hit;
        private Vector3 lastPoint;                          // Previous point on arc
        private Vector3 hitPoint;                           // The point where arc hit a collider
        private Vector3 forwardVelocity;                    // Calculated velocity for simulated trajectory
        private Vector3 trackingSpaceGravity;               // Tracking space gravity for simulated trajectory
        private Vector3 controllerPitchEffect;              // Calculated effect of controller pitch to simulated trajectory
        private Controller controller;


        // Get required references, sanity check references, instansiate required visuals
        void Awake()
        {
            controller = GetComponent<Controller>();
            teleportArcRenderer = teleportPointer.GetComponent<LineRenderer>();
            teleportTarget = Instantiate(teleportTargetPrefab).transform;
        }

        void OnValidate()
        {
            if (pointsOnArc < 3)
            {
                pointsOnArc = 3;
                Debug.LogWarning("The arc should have at least three points.");
            }
        }

        // Shoot teleport arc when holding primary button and teleport when released if possible
        void Update()
        {
            if (controller.PrimaryButton)
            {
                if (!buttonDown)
                {
                    buttonDown = true;
                }

                arc.Clear();
                arc.Add(teleportPointer.position);

                lastPoint = teleportPointer.position;
                forwardVelocity = teleportPointer.forward * velocity;
                trackingSpaceGravity = xrRig.up * -gravity;
                controllerPitchEffect = Mathf.Max((Vector3.Dot(xrRig.up, teleportPointer.up)), 0f) * controllerPitchFactor * forwardVelocity;

                for (int i = 1; i < pointsOnArc; i++)
                {
                    float t = i * 1f / pointsOnArc;
                    Vector3 point = GetPointFromArc(teleportPointer.position, forwardVelocity + controllerPitchEffect, trackingSpaceGravity, t);

                    if (Physics.Linecast(lastPoint, point, out hit, teleportableLayers))
                    {
                        arc.Add(hit.point);
                        hitPoint = hit.point;
                        canTeleport = AngleAllowed(hit.normal);
                        break;
                    }
                    else
                    {
                        arc.Add(point);
                        canTeleport = false;
                    }
                    lastPoint = point;
                }
                // Draw arc with color indicating if the current target is valid
                // Valid target is also indicated with on the ground
                if (canTeleport)
                {
                    ShowArc(validTeleportColor);
                    ShowTarget();
                }
                else
                {
                    ShowArc(invalidTeleportColor);
                    teleportTarget.gameObject.SetActive(false);
                }
            }
            else if (!controller.PrimaryButton && buttonDown)
            {
                buttonDown = false;
                Teleport();
            }
            else
            {
                teleportPointer.gameObject.SetActive(false);
                teleportTarget.gameObject.SetActive(false);
            }
        }

        void ShowArc(Color color)
        {
            teleportPointer.gameObject.SetActive(true);
            teleportArcRenderer.positionCount = arc.Count;
            teleportArcRenderer.SetPositions(arc.ToArray());
            teleportArcRenderer.startColor = color;
            teleportArcRenderer.endColor = color;
        }

        void ShowTarget()
        {
            teleportTarget.gameObject.SetActive(true);
            teleportTarget.position = hitPoint;
            teleportTarget.rotation = Quaternion.identity;
        }

        void Teleport()
        {
            // Teleport relative to head position to avoid spatial confusion
            Vector3 userOffsetFromTrackingOrigin = xrRig.position - mainCamera.position;
            userOffsetFromTrackingOrigin.y = 0;
            xrRig.position = hitPoint + userOffsetFromTrackingOrigin;

            teleportTarget.gameObject.SetActive(false);
            canTeleport = false;
        }

        // Checks if angle of the surface is smaller or equal to set maximum angle
        bool AngleAllowed(Vector3 angle)
        {
            return Vector3.Angle(Vector3.up, angle) <= maxTargetSurfaceAngle;
        }

        // Returns point from a simulated trajectory
        public Vector3 GetPointFromArc(Vector3 start, Vector3 velocity, Vector3 gravity, float time)
        {
            Vector3 point = new Vector3();
            point.x = start.x + velocity.x * time + 0.5f * gravity.x * time * time;
            point.y = start.y + velocity.y * time + 0.5f * gravity.y * time * time;
            point.z = start.z + velocity.z * time + 0.5f * gravity.z * time * time;
            return point;
        }
    }
}
