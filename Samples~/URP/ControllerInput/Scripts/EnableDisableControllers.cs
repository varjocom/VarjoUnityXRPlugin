using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace VarjoExample
{
    public class EnableDisableControllers : MonoBehaviour
    {
        [Header("Controllers to hide if device is not valid")]
        [SerializeField] GameObject leftController;
        [SerializeField] GameObject rightController;

        private XRNode XRNodeLeftHand = XRNode.LeftHand;
        private XRNode XRNodeRightHand = XRNode.RightHand;

        private List<InputDevice> devices = new List<InputDevice>();
        private InputDevice device;

        void Update()
        {
            GetDevice(XRNodeLeftHand, leftController);
            GetDevice(XRNodeRightHand, rightController);
        }

        void GetDevice(XRNode Hand, GameObject controller)
        {
            InputDevices.GetDevicesAtXRNode(Hand, devices);
            device = devices.FirstOrDefault();
            if (!device.isValid && controller.activeSelf)
            {
                controller.SetActive(false);
            }
            else if (device.isValid && !controller.activeSelf)
            {
                controller.SetActive(true);
            }
        }
    }
}
