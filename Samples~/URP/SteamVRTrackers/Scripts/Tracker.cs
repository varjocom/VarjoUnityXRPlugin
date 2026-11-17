using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(TrackedPoseDriver))]
public class Tracker : MonoBehaviour
{
    public float BatteryLevelValue { get; private set; }
    public bool GripPressedValue { get; private set; }
    public bool PrimaryValue { get; private set; }
    public bool SystemValue { get; private set; }
    public bool TrackpadPressedValue { get; private set; }
    public bool TriggerPressedValue { get; private set; }

    private TrackedPoseDriver trackedPoseDriver;
    private InputAction batteryLevel;
    private InputAction gripPressed;
    private InputAction primary;
    private InputAction system;
    private InputAction trackpadPressed;
    private InputAction triggerPressed;
    private bool initialized;

    void Awake()
    {
        trackedPoseDriver = GetComponent<TrackedPoseDriver>();
    }

    public void Initialize(InputDevice device)
    {
        gameObject.name = device.displayName;

        // Set up tracked pose driver
        trackedPoseDriver.positionAction = new InputAction(name: "devicePosition", type: InputActionType.Value, binding: $"<Varjo{device.name}>/devicePosition");
        trackedPoseDriver.rotationAction = new InputAction(name: "deviceRotation", type: InputActionType.Value, binding: $"<Varjo{device.name}>/deviceRotation");

        // Add bindings dynamically for the device, for example "<VarjoSteamVRTrackerKeyboard>/triggerPressed"
        batteryLevel = new InputAction(name: "batteryLevel", type: InputActionType.Value, binding: $"<Varjo{device.name}>/batteryLevel");
        gripPressed = new InputAction(name: "gripPressed", type: InputActionType.Button, binding: $"<Varjo{device.name}>/gripPressed");
        primary = new InputAction(name: "primary", type: InputActionType.Button, binding: $"<Varjo{device.name}>/primary");
        system = new InputAction(name: "system", type: InputActionType.Button, binding: $"<Varjo{device.name}>/system");
        trackpadPressed = new InputAction(name: "trackpadPressed", type: InputActionType.Button, binding: $"<Varjo{device.name}>/trackpadPressed");
        triggerPressed = new InputAction(name: "triggerPressed", type: InputActionType.Button, binding: $"<Varjo{device.name}>/triggerPressed");

        // Enable input actions
        batteryLevel.Enable();
        gripPressed.Enable();
        primary.Enable();
        system.Enable();
        trackpadPressed.Enable();
        triggerPressed.Enable();

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        BatteryLevelValue = batteryLevel.ReadValue<float>();
        GripPressedValue = gripPressed.IsPressed();
        PrimaryValue = primary.IsPressed();
        SystemValue = system.IsPressed();
        TrackpadPressedValue = trackpadPressed.IsPressed();
        TriggerPressedValue = triggerPressed.IsPressed();
    }
}
