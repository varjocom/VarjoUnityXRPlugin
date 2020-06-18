using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Varjo.XR;

public class EyeTrackingExample : MonoBehaviour
{
    [Header("Gaze calibration settings")]
    public VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Fast;
    public VarjoEyeTracking.GazeOutputFilterMode gazeFilterMode = VarjoEyeTracking.GazeOutputFilterMode.Standard;
    public XRNode XRNode = XRNode.CenterEye;
    public KeyCode calibrationRequestKey = KeyCode.Space;

    [Header("Toggle gaze target visibility")]
    public KeyCode toggleGazeTarget = KeyCode.Return;

    [Header("Debug Gaze")]
    public KeyCode checkGazeAllowed = KeyCode.PageUp;


    public KeyCode checkGazeCalibrated = KeyCode.PageDown;

    [Header("Toggle fixation point indicator visibility")]
    public bool showFixationPoint = true;
    public Transform fixationPointTransform;
    public Transform leftEyeTransform;
    public Transform rightEyeTransform;

    [Header("VR camera")]
    public Camera vrCamera;

    [Header("Gaze point indicator")]
    public GameObject gazeTarget;

    [Header("Gaze ray radius")]
    public float gazeRadius = 0.01f;

    [Header("Gaze point distance if not hit anything")]
    public float floatingGazeTargetDistance = 5f;

    [Header("Gaze target offset towards viewer")]
    public float targetOffset = 0.2f;

    [Header("Amout of force give to freerotating objects at point where user is looking")]
    public float hitForce = 5f;

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice device;
    private Eyes eyes;
    private Vector3 leftEyePosition;
    private Vector3 rightEyePosition;
    private Quaternion leftEyeRotation;
    private Quaternion rightEyeRotation;
    private Vector3 fixationPoint;
    private Vector3 direction;
    private Vector3 rayOrigin;
    private RaycastHit hit;
    private float distance;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(XRNode, devices);
        device = devices.FirstOrDefault();
    }

    void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }

    private void Start()
    {
        //Hiding the gazetarget if gaze is not available or if the gaze calibration is not done
        if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
        {
            gazeTarget.SetActive(true);
        }
        else
        {
            gazeTarget.SetActive(false);
        }

        if (showFixationPoint)
        {
            fixationPointTransform.gameObject.SetActive(true);
        }
        else
        {
            fixationPointTransform.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //Requesting gaze calibration with default settings
        if (Input.GetKeyDown(calibrationRequestKey))
        {
            VarjoEyeTracking.RequestGazeCalibration(gazeCalibrationMode, gazeFilterMode);
        }

        if (Input.GetKeyDown(checkGazeAllowed))// Check if gaze is allowed
        {
            Debug.Log("Gaze allowed: " + VarjoEyeTracking.IsGazeAllowed());

        }
        else if (Input.GetKeyDown(checkGazeCalibrated))  // Check if gaze calibration is done

        {
            Debug.Log("Gaze calibrated: " + VarjoEyeTracking.IsGazeCalibrated());
        }

        //toggle gaze target visibility
        if (Input.GetKeyDown(toggleGazeTarget))
        {
            gazeTarget.GetComponentInChildren<MeshRenderer>().enabled = !gazeTarget.GetComponentInChildren<MeshRenderer>().enabled;
        }

        // if gaze is allowed and calibrated we can get gaze data
        if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
        {
            //Get device if not valid
            if (!device.isValid)
            {
                GetDevice();
            }

            //show gaze target
            gazeTarget.SetActive(true);

            //Get data for eyes position, rotation and fixation point.
            if (device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
            {
                if (eyes.TryGetLeftEyePosition(out leftEyePosition))
                {
                    leftEyeTransform.localPosition = leftEyePosition;
                }

                if (eyes.TryGetLeftEyeRotation(out leftEyeRotation))
                {
                    leftEyeTransform.localRotation = leftEyeRotation;
                }

                if (eyes.TryGetRightEyePosition(out rightEyePosition))
                {
                    rightEyeTransform.localPosition = rightEyePosition;
                }

                if (eyes.TryGetRightEyeRotation(out rightEyeRotation))
                {
                    rightEyeTransform.localRotation = rightEyeRotation;
                }

                if (eyes.TryGetFixationPoint(out fixationPoint))
                {
                    fixationPointTransform.localPosition = fixationPoint;
                }
            }
        }

        // Set raycast origin point to vr camera position
        rayOrigin = vrCamera.transform.position;

        // Direction from VR camer towards eyes fixation point;
        direction = (fixationPointTransform.position - vrCamera.transform.position).normalized;

        //RayCast to world from VR Camera position towards Eyes fixation point
        if (Physics.SphereCast(rayOrigin, gazeRadius, direction, out hit))
        {
            //put target on gaze raycast position with offset towards user
            gazeTarget.transform.position = hit.point - direction * targetOffset;

            //make gaze target to point towards user
            gazeTarget.transform.LookAt(vrCamera.transform.position, Vector3.up);

            // Scale gazetarget with distance so it apperas to be always same size
            distance = hit.distance;
            gazeTarget.transform.localScale = Vector3.one * distance;

            // Use layers or tags preferably to identify looked objects in your application.
            // This is done here via GetComponent for clarity's sake as example.
            RotateWithGaze rotateWithGaze = hit.collider.gameObject.GetComponent<RotateWithGaze>();
            if (rotateWithGaze != null)
            {
                rotateWithGaze.RayHit();
            }

            // alternative way to check if you hit object with tag
            if (hit.transform.CompareTag("FreeRotating"))
            {
                AddForceAtHitPosition();
            }
        }
        else
        {
            //If not hit anything, the gaze target is shown at fixed distance
            gazeTarget.transform.position = vrCamera.transform.position + direction * floatingGazeTargetDistance;
            gazeTarget.transform.LookAt(vrCamera.transform.position, Vector3.up);
            gazeTarget.transform.localScale = Vector3.one * floatingGazeTargetDistance;
        }
    }

    void AddForceAtHitPosition()
    {
        //Get rigidbody form hit object and add force on hit position.
        Rigidbody rb = hit.rigidbody;
        if (rb != null)
        {
            rb.AddForceAtPosition(direction * hitForce, hit.point, ForceMode.Force);
        }
    }
}