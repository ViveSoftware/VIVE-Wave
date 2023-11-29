using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using static Wave.XR.Constants;

using Wave.XR.Settings;
using Wave.XR.Function;
using UnityEngine.Rendering;
#if UNITY_EDITOR && UNITY_XR_MANAGEMENT_320
using UnityEditor;
#endif

namespace Wave.XR.Loader
{
    // This class name here will be shown in the supported VR list in XR Plugin Management 
    public class WaveXRLoader : XRLoaderHelper
#if UNITY_EDITOR && UNITY_XR_MANAGEMENT_320
        , IXRLoaderPreInit
#endif
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();
        private static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptors =
            new List<XRMeshSubsystemDescriptor>();


        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }

        public XRMeshSubsystem meshSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRMeshSubsystem>();
            }
        }

        public override bool Initialize()
        {
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, k_DisplaySubsystemId);
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, k_InputSubsystemId);
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptors, k_MeshSubsystemId);

            if (displaySubsystem == null)
                Debug.LogError("Failed to load display subsystem.");

            if (inputSubsystem == null)
                Debug.LogError("Failed to load input subsystem.");

            if (displaySubsystem == null || inputSubsystem == null)
            {
                Debug.LogError("Unable to start Wave XRSDK Plugin.");
                return false;
            }

            if (Application.isEditor)
            {
                SettingsHelper.CheckSinglePass();
            }
            else
            {
                FunctionsHelper.Process(this);
                SettingsHelper.Process(this);
            }

            return true;
        }

        public override bool Start()
        {

            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            StartSubsystem<XRMeshSubsystem>();

            SettingsHelper.CheckSinglePass();

            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRMeshSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRMeshSubsystem>();
            return true;
        }

#if UNITY_EDITOR && UNITY_XR_MANAGEMENT_320
        public override List<GraphicsDeviceType> GetSupportedGraphicsDeviceTypes(bool buildingPlayer)
        {
            Debug.Log("SystemInfo.graphicsDeviceType=" + SystemInfo.graphicsDeviceType);
            return new List<GraphicsDeviceType>() { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.OpenGLES3 };
        }

        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            if (buildTarget == BuildTarget.Android && buildTargetGroup == BuildTargetGroup.Android)
                return "wvrunityxr";
            return "";
        }
#endif
    }
}
