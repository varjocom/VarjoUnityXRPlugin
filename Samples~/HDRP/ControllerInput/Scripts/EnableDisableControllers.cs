using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class EnableDisableControllers : MonoBehaviour
{
    [Header("Controllers to hide if device is not valid")]
    public GameObject leftController;
    public GameObject rightController;

    XRNode XRNodeLeftHand = XRNode.LeftHand;
    XRNode XRNodeRightHand = XRNode.RightHand;

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
        if (!device.isValid)
        {
            controller.SetActive(false);
        }
        else
        {
            controller.SetActive(true);
        }
    }
}
