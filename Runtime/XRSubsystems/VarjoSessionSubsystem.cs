using System;
using System.Runtime.InteropServices;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;


namespace Varjo.XR
{
    /// <summary>
    /// Varjo implementation of the <c>XRSessionSubsystem</c>. Do not create this directly.
    /// Use <c>VarjoSessionSubsystemSubsystemDescriptor.Create()</c> instead.
    /// See <see cref="UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor">XRSessionSubsystemDescriptor</see>.
    /// </summary>
    /// <example>
    /// <code>
    /// var descriptors = new List<XRSessionSubsystemDescriptor>();
    /// SubsystemManager.GetSubsystemDescriptors(descriptors);
    /// subsystem = null;
    /// foreach(var descriptor in descriptors)
    /// {
    ///     if (descriptor.id.Equals(Varjo.XR.VarjoSessionSubsystem.VarjoSessionID))
    ///     {
    ///         subsystem = descriptor.Create() as Varjo.XR.VarjoSessionSubsystem;
    ///         break;
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class VarjoSessionSubsystem : XRSessionSubsystem
    {
        public const string VarjoSessionID = "Varjo Session";

        public VarjoSessionSubsystem() { }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_6000_0_OR_NEWER
            XRSessionSubsystemDescriptor.Register(new XRSessionSubsystemDescriptor.Cinfo()
#else
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo()
#endif
            {
                supportsInstall = false,
                supportsMatchFrameRate = false,
                id = VarjoSessionID,
                providerType = typeof(VarjoSessionProvider),
                subsystemTypeOverride = typeof(VarjoSessionSubsystem)
            });
        }

        private class VarjoSessionProvider : Provider
        {
            [DllImport("VarjoUnityXR")]
            private static extern void StartSession();
            [DllImport("VarjoUnityXR")]
            private static extern void StopSession();
            [DllImport("VarjoUnityXR")]
            private static extern bool IsSessionStarted();


            public VarjoSessionProvider()
            { }


            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                var availability = VarjoMixedReality.IsMRReady() ? (SessionAvailability.Supported | SessionAvailability.Installed) : SessionAvailability.None;
                return Promise<SessionAvailability>.CreateResolvedPromise(availability);
            }


            public override void Start()
            {
                StartSession();
            }

            public override void Stop()
            {
                StopSession();
            }

            public override void Destroy()
            {
                StopSession();
            }


            public override TrackingState trackingState
            {
                get
                {
                    var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);

                    if (device.isValid)
                    {
                        if (device.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState trackingState))
                        {
                            if (trackingState == InputTrackingState.None)
                                return TrackingState.None;
                            else if (trackingState == (InputTrackingState.Position | InputTrackingState.Rotation))
                                return TrackingState.Tracking;
                            else
                                return TrackingState.Limited;
                        }
                    }
                    return TrackingState.None;
                }
            }
        }
    }
}