#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Varjo.XR.Input
{
    /// <summary>
    /// A Varjo headset.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "Varjo Headset")]
    public class VarjoHMD : XRHMD
    {
        [Preserve, InputControl]
        public new IntegerControl trackingState { get; private set; }

        [Preserve, InputControl]
        public new ButtonControl isTracked { get; private set; }

        [Preserve, InputControl(aliases = new[] { "HeadPosition" })]
        public new Vector3Control devicePosition { get; private set; }

        [Preserve, InputControl(aliases = new[] { "HeadRotation" })]
        public new QuaternionControl deviceRotation { get; private set; }

        [Preserve, InputControl]
        public new Vector3Control leftEyePosition { get; private set; }

        [Preserve, InputControl]
        public new QuaternionControl leftEyeRotation { get; private set; }

        [Preserve, InputControl]
        public new Vector3Control rightEyePosition { get; private set; }

        [Preserve, InputControl]
        public new QuaternionControl rightEyeRotation { get; private set; }

        [Preserve, InputControl]
        public new Vector3Control centerEyePosition { get; private set; }

        [Preserve, InputControl]
        public new QuaternionControl centerEyeRotation { get; private set; }

        [Preserve, InputControl(aliases = new[] { "PrimaryButton" })]
        public ButtonControl applicationButton { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Gaze" })]
        public EyesControl eyeGaze { get; private set; }

        [Preserve, InputControl]
        public ButtonControl userPresence { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            trackingState = GetChildControl<IntegerControl>("trackingState");
            isTracked = GetChildControl<ButtonControl>("isTracked");
            devicePosition = GetChildControl<Vector3Control>("devicePosition");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
            leftEyePosition = GetChildControl<Vector3Control>("leftEyePosition");
            leftEyeRotation = GetChildControl<QuaternionControl>("leftEyeRotation");
            rightEyePosition = GetChildControl<Vector3Control>("rightEyePosition");
            rightEyeRotation = GetChildControl<QuaternionControl>("rightEyeRotation");
            centerEyePosition = GetChildControl<Vector3Control>("centerEyePosition");
            centerEyeRotation = GetChildControl<QuaternionControl>("centerEyeRotation");
            applicationButton = GetChildControl<ButtonControl>("applicationButton");
            eyeGaze = GetChildControl<EyesControl>("eyeGaze");
            userPresence = GetChildControl<ButtonControl>("userPresence");
        }
    }

    /// <summary>
    /// A Varjo controller.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "Varjo Controller", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class VarjoController : XRControllerWithRumble
    {
        [Preserve, InputControl]
        public ButtonControl gripPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primary { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primaryTouched { get; private set; }

        [Preserve, InputControl]
        public ButtonControl secondary { get; private set; }

        [Preserve, InputControl]
        public ButtonControl secondaryTouched { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "joystick" })]
        public Vector2Control thumbstick { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisClick", "joystickClicked" })]
        public ButtonControl thumbstickPressed { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisTouch", "joystickTouched" })]
        public ButtonControl thumbstickTouched { get; private set; }

        [Preserve, InputControl]
        public AxisControl trigger { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerTouched { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceAngularVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public AxisControl batteryLevel { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            primary = GetChildControl<ButtonControl>("primary");
            primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
            secondary = GetChildControl<ButtonControl>("secondary");
            secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
            thumbstickPressed = GetChildControl<ButtonControl>("thumbstickPressed");
            thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
            thumbstick = GetChildControl<Vector2Control>("thumbstick");
            trigger = GetChildControl<AxisControl>("trigger");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
            triggerTouched = GetChildControl<ButtonControl>("triggerTouched");

            deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
            deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
            batteryLevel = GetChildControl<AxisControl>("batteryLevel");
        }
    }

    /// <summary>
    /// A Valve Index controller.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "Index Controller (Varjo)", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class VarjoIndexController : XRControllerWithRumble
    {
        [Preserve, InputControl]
        public AxisControl grip { get; private set; }

        [Preserve, InputControl]
        public ButtonControl gripPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primary { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primaryTouched { get; private set; }

        [Preserve, InputControl]
        public ButtonControl secondary { get; private set; }

        [Preserve, InputControl]
        public ButtonControl secondaryTouched { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Secondary2DAxisClick", "touchpadClicked"})]
        public ButtonControl trackpadPressed { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Secondary2DAxisTouch", "touchpadTouched"})]
        public ButtonControl trackpadTouched { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Secondary2DAxis", "touchpad" })]
        public Vector2Control trackpad { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisClick", "joystickClicked" })]
        public ButtonControl thumbstickPressed { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisTouch", "joystickTouched" })]
        public ButtonControl thumbstickTouched { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "joystick" })]
        public Vector2Control thumbstick { get; private set; }

        [Preserve, InputControl]
        public AxisControl trigger { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerTouched { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceAngularVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public AxisControl batteryLevel { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            grip = GetChildControl<AxisControl>("grip");
            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            primary = GetChildControl<ButtonControl>("primary");
            primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
            secondary = GetChildControl<ButtonControl>("secondary");
            secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
            trackpadPressed = GetChildControl<ButtonControl>("trackpadPressed");
            trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
            trackpad = GetChildControl<Vector2Control>("trackpad");
            thumbstickPressed = GetChildControl<ButtonControl>("thumbstickPressed");
            thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
            thumbstick = GetChildControl<Vector2Control>("thumbstick");
            trigger = GetChildControl<AxisControl>("trigger");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
            triggerTouched = GetChildControl<ButtonControl>("triggerTouched");

            deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
            deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
            batteryLevel = GetChildControl<AxisControl>("batteryLevel");
        }
    }

    /// <summary>
    /// An HTC Vive Wand controller.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "Vive Wand (Varjo)", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class VarjoViveWand : XRControllerWithRumble
    {
        [Preserve, InputControl]
        public AxisControl grip { get; private set; }

        [Preserve, InputControl]
        public ButtonControl gripPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primary { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisClick", "joystickOrPadPressed", "touchpadClicked" })]
        public ButtonControl trackpadPressed { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisTouch", "joystickOrPadTouched", "touchpadTouched" })]
        public ButtonControl trackpadTouched { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "touchpad" })]
        public Vector2Control trackpad { get; private set; }

        [Preserve, InputControl]
        public AxisControl trigger { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerPressed { get; private set; }


        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceAngularVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public AxisControl batteryLevel { get; private set; }


        protected override void FinishSetup()
        {
            base.FinishSetup();

            grip = GetChildControl<AxisControl>("grip");
            primary = GetChildControl<ButtonControl>("primary");
            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            trackpadPressed = GetChildControl<ButtonControl>("trackpadPressed");
            trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
            trackpad = GetChildControl<Vector2Control>("trackpad");
            trigger = GetChildControl<AxisControl>("trigger");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");

            deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
            deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
            batteryLevel = GetChildControl<AxisControl>("batteryLevel");
        }
    }

    /// <summary>
    /// A SteamVR tracker.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker (Varjo)")]
    public class VarjoSteamVRTracker : TrackedDevice
    {
        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public Vector3Control deviceAngularVelocity { get; private set; }

        [Preserve, InputControl(noisy = true)]
        public AxisControl batteryLevel { get; private set; }


        protected override void FinishSetup()
        {
            base.FinishSetup();

            deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
            deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
            batteryLevel = GetChildControl<AxisControl>("batteryLevel");
        }
    }

    /// <summary>
    /// A SteamVR tracker with buttons.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker With Buttons (Varjo)")]
    public class VarjoSteamVRTrackerWithButtons : VarjoSteamVRTracker
    {
        [Preserve, InputControl]
        public AxisControl grip { get; private set; }

        [Preserve, InputControl]
        public ButtonControl gripPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl primary { get; private set; }

        [Preserve, InputControl(aliases = new[] { "Primary2DAxisClick", "joystickOrPadPressed", "touchpadClicked" })]
        public ButtonControl trackpadPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl triggerPressed { get; private set; }

        [Preserve, InputControl]
        public ButtonControl system { get; private set; }


        protected override void FinishSetup()
        {
            grip = GetChildControl<AxisControl>("grip");
            primary = GetChildControl<ButtonControl>("primary");
            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            trackpadPressed = GetChildControl<ButtonControl>("trackpadPressed");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
            system = GetChildControl<ButtonControl>("system");

            base.FinishSetup();
        }
    }

    /// <summary>
    /// A handed SteamVR tracker.
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Handed (Varjo)", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class VarjoSteamVRTrackerHanded : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Camera).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Camera (Varjo)")]
    public class VarjoSteamVRTrackerCamera : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Chest).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Chest (Varjo)")]
    public class VarjoSteamVRTrackerChest : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Keyboard).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Keyboard (Varjo)")]
    public class VarjoSteamVRTrackerKeyboard : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Left Elbow).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Left Elbow (Varjo)")]
    public class VarjoSteamVRTrackerLeftElbow : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Left Foot).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Left Foot (Varjo)")]
    public class VarjoSteamVRTrackerLeftFoot : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Left Knee).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Left Knee (Varjo)")]
    public class VarjoSteamVRTrackerLeftKnee : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Left Shoulder).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Left Shoulder (Varjo)")]
    public class VarjoSteamVRTrackerLeftShoulder : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Right Elbow).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Right Elbow (Varjo)")]
    public class VarjoSteamVRTrackerRightElbow : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Right Foot).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Right Foot (Varjo)")]
    public class VarjoSteamVRTrackerRightFoot : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Right Knee).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Right Knee (Varjo)")]
    public class VarjoSteamVRTrackerRightKnee : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Right Shoulder).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Right Shoulder (Varjo)")]
    public class VarjoSteamVRTrackerRightShoulder : VarjoSteamVRTrackerWithButtons
    {
    }

    /// <summary>
    /// A SteamVR tracker (Waist).
    /// </summary>
    [Preserve, InputControlLayout(displayName = "SteamVR Tracker Waist (Varjo)")]
    public class VarjoSteamVRTrackerWaist : VarjoSteamVRTrackerWithButtons
    {
    }
}
#endif //#if UNITY_INPUT_SYSTEM
