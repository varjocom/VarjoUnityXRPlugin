using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace VarjoExample
{
    public class Controller : MonoBehaviour
    {
        [Header("Select hand")]
        [SerializeField] XRNode XRNode = XRNode.LeftHand;

        [Header("Controllers parts")]
        [SerializeField] GameObject controller;
        [SerializeField] GameObject bodyGameobject;
        [SerializeField] MeshRenderer touchPadGameobject;
        [SerializeField] MeshRenderer menubuttonGameobject;
        [SerializeField] MeshRenderer triggerGameobject;
        [SerializeField] MeshRenderer systemButtonGameobject;
        [SerializeField] MeshRenderer gripButtonGameobject;

        [Header("Controller material")]
        [SerializeField] Material controllerMaterial;

        [Header("Controller button highlight material")]
        [SerializeField] Material buttonPressedMaterial;
        [SerializeField] Material touchpadTouchedMaterial;

        private List<InputDevice> devices = new List<InputDevice>();
        private InputDevice device;

        private Quaternion deviceRotation;
        private Vector3 devicePosition;
        private Vector3 deviceAngularVelocity;
        private Vector3 deviceVelocity;
        private Vector3 triggerRotation;

        private bool triggerButton;
        private bool gripButton;
        private bool primary2DAxisTouch;
        private bool primary2DAxisClick;
        private bool primaryButton;
        private float trigger;

        public bool TriggerButton { get { return triggerButton; } }
        public bool GripButton { get { return gripButton; } }
        public bool Primary2DAxisTouch { get { return primary2DAxisTouch; } }
        public bool Primary2DAxisClick { get { return primary2DAxisClick; } }
        public bool PrimaryButton { get { return primaryButton; } }
        public Vector3 DeviceVelocity { get { return deviceVelocity; } }
        public Vector3 DeviceAngularVelocity { get { return deviceAngularVelocity; } }
        public float Trigger { get { return trigger; } }

        void OnEnable()
        {
            if (!device.isValid)
            {
                GetDevice();
            }
        }

        void Update()
        {
            if (!device.isValid)
            {
                GetDevice();
            }

            // Get values for device position, rotation and buttons.
            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
            {
                transform.localPosition = devicePosition;
            }

            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
            {
                transform.localRotation = deviceRotation;
            }

            if (device.TryGetFeatureValue(CommonUsages.trigger, out trigger))
            {
                ControllerInput();
            }

            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton))
            {
                ControllerInput();
            }

            if (device.TryGetFeatureValue(CommonUsages.gripButton, out gripButton))
            {
                ControllerInput();
            }

            if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out primary2DAxisTouch))
            {
                ControllerInput();
            }

            if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primary2DAxisClick))
            {
                ControllerInput();
            }

            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton))
            {
                ControllerInput();
            }

            device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out deviceAngularVelocity);
            device.TryGetFeatureValue(CommonUsages.deviceVelocity, out deviceVelocity);
        }

        void GetDevice()
        {
            InputDevices.GetDevicesAtXRNode(XRNode, devices);
            device = devices.FirstOrDefault();
        }

        void ControllerInput()
        {
            // Set trigger rotation from input
            triggerRotation.Set(trigger * -30.0f, 0, 0);
            triggerGameobject.transform.localRotation = Quaternion.Euler(triggerRotation);

            // Set controller button materials from inputs
            triggerGameobject.material = triggerButton ? buttonPressedMaterial : controllerMaterial;
            gripButtonGameobject.material = gripButton ? buttonPressedMaterial : controllerMaterial;
            menubuttonGameobject.material = primaryButton ? buttonPressedMaterial : controllerMaterial;

            if (!primary2DAxisTouch)
            {
                touchPadGameobject.material = controllerMaterial;
            }
            else if (primary2DAxisTouch && primary2DAxisClick)
            {
                touchPadGameobject.material = buttonPressedMaterial;
            }
            else if (primary2DAxisTouch)
            {
                touchPadGameobject.material = touchpadTouchedMaterial;
            }
        }

        public void SendHapticImpulse(float amplitude = 1.0f, float duration = 1.0f)
        {
            if (!device.isValid)
            {
                GetDevice();
            }

            device.SendHapticImpulse(0, amplitude, duration);
        }
    }
}