using System;
using System.Runtime.InteropServices;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;


namespace Varjo.XR
{
    /// <summary>
    /// Varjo implementation of the <c>XROcclusionSubsystem</c>. Do not create this directly.
    /// Use <c>VarjoOcclusionSubsystemSubsystemDescriptor.Create()</c> instead.
    /// See <see cref="UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor">XROcclusionSubsystemDescriptor</see>.
    /// </summary>
    public sealed class VarjoOcclusionSubsystem : XROcclusionSubsystem
    {
        public const string VarjoOcclusionID = "Varjo Occlusion";

        public VarjoOcclusionSubsystem() { }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_6000_0_OR_NEWER
            XROcclusionSubsystemDescriptor.Register(new XROcclusionSubsystemDescriptor.Cinfo() 
#else
            XROcclusionSubsystem.Register(new XROcclusionSubsystemCinfo()
#endif
            {
                id = VarjoOcclusionID,
                providerType = typeof(VarjoOcclusionProvider),
                subsystemTypeOverride = typeof(VarjoOcclusionSubsystem)
            });
        }

        private class VarjoOcclusionProvider : Provider
        {
            public VarjoOcclusionProvider()
            { }


            public override void Start()
            {
                VarjoMixedReality.EnableDepthEstimation();
            }

            public override void Stop()
            {
                VarjoMixedReality.DisableDepthEstimation();
            }

            public override void Destroy()
            {
                VarjoMixedReality.DisableDepthEstimation();
            }
        }
    }
}