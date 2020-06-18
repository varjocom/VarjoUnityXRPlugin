using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityEditor.XR.Varjo.Tests
{
    class EditorTests
    {
        internal class SmokeTests : TestBaseSetup
        {
            [Test]
            public void SceneIsCreated()
            {
                Assert.IsNotNull(m_Camera, "Camera was not created");
                Assert.IsNotNull(m_Light, "Light was not created");
                Assert.IsNotNull(m_Cube, "Cube was not created");
            }

            [UnityTest]
            public IEnumerator XrSdkAssetsCreated()
            {
                Assert.IsNotNull(m_TrackHead, "Tracking the Head Node was not created");
                yield return null;
                Assert.IsNotNull(m_TrackingRig, "Tracking rig was not created");
                yield return null;
            }
        }
    }
}
