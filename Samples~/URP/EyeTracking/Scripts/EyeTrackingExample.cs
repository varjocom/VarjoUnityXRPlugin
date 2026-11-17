using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Varjo.XR;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;
using Unity.XR.CoreUtils;

namespace VarjoExample
{
    public enum GazeDataSource
    {
        InputSubsystem,
        GazeAPI
    }

    public class EyeTrackingExample : MonoBehaviour
    {
        [Header("Gaze data")]
        [SerializeField] GazeDataSource gazeDataSource = GazeDataSource.InputSubsystem;

        [Header("Gaze calibration settings")]
        [SerializeField] VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Fast;
        [SerializeField] Key calibrationRequestKey = Key.Space;

        [Header("Gaze output filter settings")]
        [SerializeField] VarjoEyeTracking.GazeOutputFilterType gazeOutputFilterType = VarjoEyeTracking.GazeOutputFilterType.Standard;
        [SerializeField] Key setOutputFilterTypeKey = Key.RightShift;

        [Header("Gaze data output frequency")]
        [SerializeField] VarjoEyeTracking.GazeOutputFrequency frequency;

        [Header("Toggle gaze target visibility")]
        [SerializeField] Key toggleGazeTarget = Key.Enter;

        [Header("Debug Gaze")]
        [SerializeField] Key checkGazeAllowed = Key.PageUp;
        [SerializeField] Key checkGazeCalibrated = Key.PageDown;

        [Header("Toggle fixation point indicator visibility")]
        [SerializeField] bool showFixationPoint = true;

        [Header("Visualization Transforms")]
        [SerializeField] Transform fixationPointTransform;
        [SerializeField] Transform leftEyeTransform;
        [SerializeField] Transform rightEyeTransform;

        [Header("XR camera")]
        [SerializeField] Camera xrCamera;

        [Header("Gaze point indicator")]
        [SerializeField] GameObject gazeTarget;

        [Header("Gaze ray radius")]
        [SerializeField] float gazeRadius = 0.01f;

        [Header("Layers that hit message is sent to")]
        [SerializeField] LayerMask targetLayers;

        [Header("Gaze point distance if not hit anything")]
        [SerializeField] float floatingGazeTargetDistance = 5f;

        [Header("Gaze target offset towards viewer")]
        [SerializeField] float targetOffset = 0.2f;

        [Header("Gaze data logging")]
        [SerializeField] Key loggingToggleKey = Key.RightCtrl;

        [Header("Default path is Logs under application data path.")]
        [SerializeField] bool useCustomLogPath = false;
        [SerializeField] string customLogPath = "";

        [Header("Print gaze data framerate while logging.")]
        [SerializeField] bool printFramerate = false;

        private List<InputDevice> devices = new List<InputDevice>();
        private InputDevice device;
        private Eyes eyes;
        private VarjoEyeTracking.GazeData gazeData;
        private List<VarjoEyeTracking.GazeData> dataSinceLastUpdate;
        private List<VarjoEyeTracking.EyeMeasurements> eyeMeasurementsSinceLastUpdate;
        private Vector3 leftEyePosition;
        private Vector3 rightEyePosition;
        private Quaternion leftEyeRotation;
        private Quaternion rightEyeRotation;
        private Vector3 fixationPoint;
        private Vector3 direction;
        private Vector3 rayOrigin;
        private RaycastHit hit;
        private float distance;
        private StreamWriter writer = null;
        private bool logging = false;

        private static readonly string[] ColumnNames = { "Frame", "CaptureTime", "LogTime", "HMDPosition", "HMDRotation", "GazeStatus", "CombinedGazeForward", "CombinedGazePosition", "InterPupillaryDistanceInMM", "LeftEyeStatus", "LeftEyeForward", "LeftEyePosition", "LeftPupilIrisDiameterRatio", "LeftPupilDiameterInMM", "LeftIrisDiameterInMM", "RightEyeStatus", "RightEyeForward", "RightEyePosition", "RightPupilIrisDiameterRatio", "RightPupilDiameterInMM", "RightIrisDiameterInMM", "FocusDistance", "FocusStability" };
        private const string ValidString = "VALID";
        private const string InvalidString = "INVALID";

        private int gazeDataCount = 0;
        private float gazeTimer = 0f;

        void GetDevice()
        {
            InputDevices.GetDevicesAtXRNode(XRNode.CenterEye, devices);
            device = devices.FirstOrDefault();
        }

        void OnEnable()
        {
            if (!device.isValid)
            {
                GetDevice();
            }
        }

        void Start()
        {
            VarjoEyeTracking.SetGazeOutputFrequency(frequency);
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
            if (logging && printFramerate)
            {
                gazeTimer += Time.deltaTime;
                if (gazeTimer >= 1.0f)
                {
                    Debug.Log("Gaze data rows per second: " + gazeDataCount);
                    gazeDataCount = 0;
                    gazeTimer = 0f;
                }
            }

            // Request gaze calibration
            if (Keyboard.current[calibrationRequestKey].wasPressedThisFrame)
            {
                VarjoEyeTracking.RequestGazeCalibration(gazeCalibrationMode);
            }

            // Set output filter type
            if (Keyboard.current[setOutputFilterTypeKey].wasPressedThisFrame)
            {
                VarjoEyeTracking.SetGazeOutputFilterType(gazeOutputFilterType);
                Debug.Log("Gaze output filter type is now: " + VarjoEyeTracking.GetGazeOutputFilterType());
            }

            // Check if gaze is allowed
            if (Keyboard.current[checkGazeAllowed].wasPressedThisFrame)
            {
                Debug.Log("Gaze allowed: " + VarjoEyeTracking.IsGazeAllowed());
            }

            // Check if gaze is calibrated
            if (Keyboard.current[checkGazeCalibrated].wasPressedThisFrame)
            {
                Debug.Log("Gaze calibrated: " + VarjoEyeTracking.IsGazeCalibrated());
            }

            // Toggle gaze target visibility
            if (Keyboard.current[toggleGazeTarget].wasPressedThisFrame)
            {
                gazeTarget.GetComponentInChildren<MeshRenderer>().enabled = !gazeTarget.GetComponentInChildren<MeshRenderer>().enabled;
            }

            // Get gaze data if gaze is allowed and calibrated
            if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
            {
                //Get device if not valid
                if (!device.isValid)
                {
                    GetDevice();
                }

                // Show gaze target
                gazeTarget.SetActive(true);

                if (gazeDataSource == GazeDataSource.InputSubsystem)
                {
                    // Get data for eye positions, rotations and the fixation point
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

                    // Set raycast origin point to VR camera position
                    rayOrigin = xrCamera.transform.position;

                    // Direction from VR camera towards fixation point
                    direction = (fixationPointTransform.position - xrCamera.transform.position).normalized;

                } else
                {
                    gazeData = VarjoEyeTracking.GetGaze();

                    if (gazeData.status != VarjoEyeTracking.GazeStatus.Invalid)
                    {
                        // GazeRay vectors are relative to the HMD pose so they need to be transformed to world space
                        if (gazeData.leftStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                        {
                            leftEyeTransform.position = xrCamera.transform.TransformPoint(gazeData.left.origin);
                            leftEyeTransform.rotation = Quaternion.LookRotation(xrCamera.transform.TransformDirection(gazeData.left.forward));
                        }

                        if (gazeData.rightStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                        {
                            rightEyeTransform.position = xrCamera.transform.TransformPoint(gazeData.right.origin);
                            rightEyeTransform.rotation = Quaternion.LookRotation(xrCamera.transform.TransformDirection(gazeData.right.forward));
                        }

                        // Set gaze origin as raycast origin
                        rayOrigin = xrCamera.transform.TransformPoint(gazeData.gaze.origin);

                        // Set gaze direction as raycast direction
                        direction = xrCamera.transform.TransformDirection(gazeData.gaze.forward);

                        // Fixation point can be calculated using ray origin, direction and focus distance
                        fixationPointTransform.position = rayOrigin + direction * gazeData.focusDistance;
                    }
                }
            }

            // Raycast to world from VR Camera position towards fixation point
            if (Physics.SphereCast(rayOrigin, gazeRadius, direction, out hit))
            {
                // Put target on gaze raycast position with offset towards user
                gazeTarget.transform.position = hit.point - direction * targetOffset;

                // Make gaze target point towards user
                gazeTarget.transform.LookAt(rayOrigin, Vector3.up);

                // Scale gazetarget with distance so it apperas to be always same size
                distance = hit.distance;
                gazeTarget.transform.localScale = Vector3.one * distance;

                // Prefer layers or tags to identify looked objects in your application
                if (targetLayers.Contains(hit.transform.gameObject.layer))
                {
                    if(hit.transform == null)
                    {
                        Debug.Log("HIT NULL");
                            return;
                    }
                    else
                    {
                        hit.transform.SendMessage("GazeHit", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                // If gaze ray didn't hit anything, the gaze target is shown at fixed distance
                gazeTarget.transform.position = rayOrigin + direction * floatingGazeTargetDistance;
                gazeTarget.transform.LookAt(rayOrigin, Vector3.up);
                gazeTarget.transform.localScale = Vector3.one * floatingGazeTargetDistance;
            }

            if (Keyboard.current[loggingToggleKey].wasPressedThisFrame)
            {
                if (!logging)
                {
                    StartLogging();
                }
                else
                {
                    StopLogging();
                }
                return;
            }

            if (logging)
            {
                int dataCount = VarjoEyeTracking.GetGazeList(out dataSinceLastUpdate, out eyeMeasurementsSinceLastUpdate);
                if (printFramerate) gazeDataCount += dataCount;
                for (int i = 0; i < dataCount; i++)
                {
                    LogGazeData(dataSinceLastUpdate[i], eyeMeasurementsSinceLastUpdate[i]);
                }
            }
        }

        void LogGazeData(VarjoEyeTracking.GazeData data, VarjoEyeTracking.EyeMeasurements eyeMeasurements)
        {
            string[] logData = new string[23];

            // Gaze data frame number
            logData[0] = data.frameNumber.ToString();

            // Gaze data capture time (nanoseconds)
            logData[1] = data.captureTime.ToString();

            // Log time (milliseconds)
            logData[2] = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();

            // HMD
            logData[3] = xrCamera.transform.localPosition.ToString("F3");
            logData[4] = xrCamera.transform.localRotation.ToString("F3");

            // Combined gaze
            bool invalid = data.status == VarjoEyeTracking.GazeStatus.Invalid;
            logData[5] = invalid ? InvalidString : ValidString;
            logData[6] = invalid ? "" : data.gaze.forward.ToString("F3");
            logData[7] = invalid ? "" : data.gaze.origin.ToString("F3");

            // IPD
            logData[8] = invalid ? "" : eyeMeasurements.interPupillaryDistanceInMM.ToString("F3");

            // Left eye
            bool leftInvalid = data.leftStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
            logData[9] = leftInvalid ? InvalidString : ValidString;
            logData[10] = leftInvalid ? "" : data.left.forward.ToString("F3");
            logData[11] = leftInvalid ? "" : data.left.origin.ToString("F3");
            logData[12] = leftInvalid ? "" : eyeMeasurements.leftPupilIrisDiameterRatio.ToString("F3");
            logData[13] = leftInvalid ? "" : eyeMeasurements.leftPupilDiameterInMM.ToString("F3");
            logData[14] = leftInvalid ? "" : eyeMeasurements.leftIrisDiameterInMM.ToString("F3");

            // Right eye
            bool rightInvalid = data.rightStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
            logData[15] = rightInvalid ? InvalidString : ValidString;
            logData[16] = rightInvalid ? "" : data.right.forward.ToString("F3");
            logData[17] = rightInvalid ? "" : data.right.origin.ToString("F3");
            logData[18] = rightInvalid ? "" : eyeMeasurements.rightPupilIrisDiameterRatio.ToString("F3");
            logData[19] = rightInvalid ? "" : eyeMeasurements.rightPupilDiameterInMM.ToString("F3");
            logData[20] = rightInvalid ? "" : eyeMeasurements.rightIrisDiameterInMM.ToString("F3");

            // Focus
            logData[21] = invalid ? "" : data.focusDistance.ToString();
                logData[22] = invalid ? "" : data.focusStability.ToString();

                Log(logData);
        }

        // Write given values in the log file
        void Log(string[] values)
        {
            if (!logging || writer == null)
                return;

            string line = "";
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = values[i].Replace("\r", "").Replace("\n", ""); // Remove new lines so they don't break csv
                line += values[i] + (i == (values.Length - 1) ? "" : ";"); // Do not add semicolon to last data string
            }
            writer.WriteLine(line);
        }

        public void StartLogging()
        {
            if (logging)
            {
                Debug.LogWarning("Logging was on when StartLogging was called. No new log was started.");
                return;
            }

            logging = true;

            string logPath = useCustomLogPath ? customLogPath : Application.dataPath + "/Logs/";
            Directory.CreateDirectory(logPath);

            DateTime now = DateTime.Now;
            string fileName = string.Format("{0}-{1:00}-{2:00}-{3:00}-{4:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute);

            string path = logPath + fileName + ".csv";
            writer = new StreamWriter(path);

            Log(ColumnNames);
            Debug.Log("Log file started at: " + path);
        }

        public void StopLogging()
        {
            if (!logging)
                return;

            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer = null;
            }
            logging = false;
            Debug.Log("Logging ended");
        }

        void OnApplicationQuit()
        {
            StopLogging();
        }
    }
}