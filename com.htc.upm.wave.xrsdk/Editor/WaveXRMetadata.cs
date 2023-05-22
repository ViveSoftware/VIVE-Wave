#if UNITY_XR_MANAGEMENT_320

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using Wave.XR.Settings;
using Wave.XR.Loader;
using Wave.XR;

class WaveXRPackage : IXRPackage
{
    private class WaveXRPackageMetadata : IXRPackageMetadata
    {
        public string packageName { get; set; }
        public string packageId { get; set; }
        public string settingsType { get; set; }
        public List<IXRLoaderMetadata> loaderMetadata { get; set; }
    }

    private class WaveXRLoaderMetadata : IXRLoaderMetadata
    {
        public string loaderName { get; set; }
        public string loaderType { get; set; }
        public List<BuildTargetGroup> supportedBuildTargets { get; set; }
    }

    static class WaveXRMetadata
    {
        static WaveXRPackageMetadata s_Metadata = null;

        internal static WaveXRPackageMetadata CreateAndGetMetadata()
        {
            if (s_Metadata == null)
            {
                s_Metadata = new WaveXRPackageMetadata();
                s_Metadata.packageName = "Wave XRSDK Package";
                s_Metadata.packageId = Constants.SDKPackageName;
                s_Metadata.settingsType = typeof(WaveXRSettings).FullName;

                s_Metadata.loaderMetadata = new List<IXRLoaderMetadata>() {
                    new WaveXRLoaderMetadata() {
                        loaderName = "WaveXR",
                        loaderType = typeof(WaveXRLoader).FullName,
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Standalone,
                            BuildTargetGroup.Android,
                        }
                    }
                };
            }

            return s_Metadata;
        }
    }

    public IXRPackageMetadata metadata => WaveXRMetadata.CreateAndGetMetadata();

    public bool PopulateNewSettingsInstance(ScriptableObject obj)
    {
        WaveXRSettings settings = obj as WaveXRSettings;
        if (settings != null)
        {
            //Populate Settings values here
            return true;
        }

        return false;
    }
}

#endif
