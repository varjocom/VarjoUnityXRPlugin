#if XR_MGMT_320
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management.Metadata;
using Varjo.XR;

class VarjoPackage : IXRPackage
{
    private class VarjoLoaderMetadata : IXRLoaderMetadata
    {
        public string loaderName { get; set; }
        public string loaderType { get; set; }
        public List<BuildTargetGroup> supportedBuildTargets { get; set; }
    }

    private class VarjoPackageMetadata : IXRPackageMetadata
    {
        public string packageName { get; set; }
        public string packageId { get; set; }
        public string settingsType { get; set; }
        public List<IXRLoaderMetadata> loaderMetadata { get; set; }
    }

    private static IXRPackageMetadata s_Metadata = new VarjoPackageMetadata()
    {
        packageName = "Varjo XR Plugin",
        packageId = "com.varjo.xr",
        settingsType = "Varjo.XR.VarjoSettings",
        loaderMetadata = new List<IXRLoaderMetadata>() {
                new VarjoLoaderMetadata() {
                        loaderName = "Varjo",
                        loaderType = "Varjo.XR.VarjoLoader",
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Standalone
                        }
                    },
                }
    };

    public IXRPackageMetadata metadata => s_Metadata;

    public bool PopulateNewSettingsInstance(ScriptableObject obj)
    {
        return true;
    }
}
#endif