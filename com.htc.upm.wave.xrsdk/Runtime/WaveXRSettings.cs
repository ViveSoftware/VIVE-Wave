using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using static Wave.XR.Constants;

namespace Wave.XR.Settings
{
    [XRConfigurationData("WaveXRSettings", k_SettingsKey)]
    [System.Serializable]
    public class WaveXRSettings : ScriptableObject
    {
        static WaveXRSettings s_RuntimeInstance = null;

        public enum StereoRenderingPath
        {
            MultiPass,
            SinglePass
        }

        public enum AdaptiveQualityMode
        {
            Disabled,
            QualityOrientedMode,
            PerformanceOrientedMode,
            CustomizationMode
        }
        public enum FoveationMode
        {
            Disable,
            Enable,
            Default
        }

        public enum FoveatedRenderingPeripheralQuality
        {
            Low,
            Middle,
            High,
        }

        public enum AMCMode
        {
            Off,
            Force_UMC,
            Auto,
            Force_PMC
        }

        public enum DisplayGamut
        {
            Native,
            sRGB,
            DisplayP3
        }

        public enum SupportedFPS
        {
            HMD_Default,
            _120
        }

        [SerializeField, Tooltip("In URP, only SinglePass can be used.")]
        public StereoRenderingPath preferedStereoRenderingPath = StereoRenderingPath.SinglePass;

        [SerializeField, Tooltip("Use Double Width texture.  On Android, only MultiPass can have double width texture.")]
        public bool useDoubleWidth = false;

        [SerializeField, Tooltip("Enable RenderMask (Occlusion Mesh).")]
        public bool useRenderMask = true;

        public enum TimeWarpStabilizedMode
        {
            Off,
            On,
            Auto,
        }

        [SerializeField, Tooltip("If you select On, it will reduce jitter in some case. Please refer developer guide.")]
        public TimeWarpStabilizedMode enableTimeWarpStabilizedMode = TimeWarpStabilizedMode.Auto;

        [SerializeField, Tooltip("Adaptive Quality Mode.")]
        public AdaptiveQualityMode adaptiveQualityMode = AdaptiveQualityMode.QualityOrientedMode;

        [SerializeField, Tooltip("Allow set quality strategy is send quality event. SendQualityEvent = false if quality strategy use default.")]
        public bool AQ_SendQualityEvent = true;

        [SerializeField, Tooltip("Allow set auto foveation quality strategy. AutoFoveation = false if quality strategy disable foveation.")]
        public bool AQ_AutoFoveation = false;

        [SerializeField, Tooltip("Enable Dynamic Resolution based on Adaptive Quality event.")]
        public bool useAQDynamicResolution = true;

        [Tooltip("You can choose one of resolution scale from this list as a default resolution scale by setting the default index.")]
        [SerializeField]
        public int DR_DefaultIndex = 0;

        [Tooltip("The unit used for measuring text size here is dmm (Distance-Independent Millimeter). The method of conversion from Unity text size into dmm can be found in the documentation of the SDK.")]
        [SerializeField]
        [Range(20, 40)]
        public int DR_TextSize = 20;

        [SerializeField]
        public List<float> DR_ResolutionScaleList = new List<float>();

        [SerializeField, Tooltip("Set foveationMode.  Choose default mode will apply device's config.")]
        public FoveationMode foveationMode = FoveationMode.Default;

        [SerializeField, Tooltip("Set LeftClearVisionFOV")]
        [Range(1, 179)]
        public float leftClearVisionFOV = 38;

        [SerializeField, Tooltip("Set RightClearVisionFOV")]
        [Range(1, 179)]
        public float rightClearVisionFOV = 38;

        [SerializeField, Tooltip("Set LeftPeripheralQuality")]
        public FoveatedRenderingPeripheralQuality leftPeripheralQuality = FoveatedRenderingPeripheralQuality.High;

        [SerializeField, Tooltip("Set RightPeripheralQuality")]
        public FoveatedRenderingPeripheralQuality rightPeripheralQuality = FoveatedRenderingPeripheralQuality.High;


        [SerializeField, Tooltip("Enabled to override system's PixelDensity.  Disabled to use system's PixelDensity.")]
        public bool overridePixelDensity = false;

        [SerializeField, Tooltip("PixelDensity is a scale to the real display's width and height.  It is use to set a default eye buffer size.  The default eye buffer size will be calculated by (display w, display h) * pixelDensity.")]
        [Range(0.1f, 2)]
        public float pixelDensity = 1;

        [SerializeField, Tooltip("ResolutionScale is a scale to the default eye buffer size which is decided by PixelDensity.  Default is 1.  You can also use XRSettings.eyeTextureResolutionScale to configure it in the runtime.  The final eye buffer size will be calculated by (eye buffer w, eye buffer h) * resolutionScale.")]
        [Range(0.1f, 1)]
        public float resolutionScale = 1;

        [SerializeField, Tooltip("Override Unity's thread priority.  WARNING: This setting is experimental and may not have good effect.  Use it at your own risk.")]
        public bool overrideThreadPriority = false;

        [SerializeField, Tooltip("Game thread priority default is 0.  Its range is from -20 to 19, and the lower number the higher priority.")]
        [Range(-20, 19)]
        public int gameThreadPriority = 0;

        [SerializeField, Tooltip("Render thread priority default is -2.  Its range is from -20 to 19, and the lower number the higher priority.")]
        [Range(-20, 19)]
        public int renderThreadPriority = -2;

        [SerializeField, Tooltip("Job Worker thread priority default is 0.  Its range is from -20 to 19, and the lower number the higher priority.")]
        [Range(-20, 19)]
        public int jobWorkerThreadPriority = 0;

        [SerializeField, Tooltip("Debug log flag which native XR plugin should follow.")]
        public uint debugLogFlagForNative = (uint)(DebugLogFlag.BasicMask | DebugLogFlag.LifecycleMask | DebugLogFlag.RenderMask | DebugLogFlag.InputMask);

        [SerializeField, Tooltip("Debug log flag which Unity Player should follow.")]
        public uint debugLogFlagForUnity = (uint)(1 << (int)DebugLogFlag.Debug1 | 1 << (int)DebugLogFlag.Debug2 | 1 << (int)DebugLogFlag.Debug3);

        #region common
        [SerializeField, Tooltip("Override the LogFlag for native.")]
        public bool overrideLogFlagForNative = false;

        [SerializeField, Tooltip("Enable Compatibility Checker in development build.")]
        public bool useCMPChecker = false;
        #endregion

        [SerializeField, Tooltip("Set the mode of Adaptive Motion Compensator (AMC).  You can choose to force enable one of Universal Motion Compensator (UMC) and Positional Motion Compensator (PMC).  Or use auto UMC.  Default is off")]
        public AMCMode amcMode = AMCMode.Off;

        [SerializeField, Tooltip("0. not accept, 1. accept, 2. accept and don't ask again")]
        public int amcModeConfirm = 0;

        [SerializeField, Tooltip("Default is off.  Before you enable this option, please see online document \"FadeOut effect\" chapter first.")]
        public bool fadeOut = false;

        [SerializeField, Tooltip("Disabled by default. Enable this option to enable the Frame Sharpness Enhancement feature.\nWhen enabled, the final image will be sharpened which can improve things like text sharpness.")]
        public bool enableFSE = false;

        [SerializeField, Tooltip("Set the enhancement level of the Frame Sharpness Enhancement feature.\nThe higher the level, the stronger the sharpening effect.")]
        [Range(0f, 1f)]
        public float FSE_Level = 0.5f;

        [SerializeField, Tooltip("Set the App supported FPS. (Experimental function)\nHMD Default:\nApp will run with HMD default FPS.\n120 FPS:\nDeclare the App can support up to 120 FPS and the HMD display will switch to 120 FPS to run your App if it can support.\nNotice : You must to tune your App to make it suitable for running at 120FPS, or App might get worse rendering performance and cause jitter phenomenon.")]
        public SupportedFPS supportedFPS = SupportedFPS.HMD_Default;

        [SerializeField, Tooltip("Wave XR Feature Package folder location")]
        public string waveXRFolder = "Assets/Wave/XR";

        [SerializeField, Tooltip("Wave Essence Feature Package folder location")]
        public string waveEssenceFolder = "Assets/Wave/Essence";

        [SerializeField, Tooltip("Display gamut preferences.  Wave will try to apply gamut from the top of the list.  When the earier gamut is accepted, Wave will not try the next.  Not all gamut preference here can be accepted.  It's depended on device.  The sRGB is default acceptable for all device.")]
        public List<DisplayGamut> displayGamutPreferences = new List<DisplayGamut>() { DisplayGamut.Native, DisplayGamut.sRGB, DisplayGamut.DisplayP3 };

        [SerializeField, Tooltip("Select to enable auto fallback when using Multi-Layer (i.e. Layers that exceed the maximum layer count will be rendered in-game).")]
        private bool enableAutoFallbackForMultiLayer = true;

        [SerializeField, Tooltip("Allow this plugin to generate a spectator camera to produce images for Screenshot, Recording, or Broadcast usages.  This camera will consume more performance when it is activated, but captured image will have better FOV result.")]
        public bool allowSpectatorCamera = false;
        
        [SerializeField, Tooltip("Allow the spectator camera to capture 360 image.")]
        public bool allowSpectatorCameraCapture360Image = false;

        #region Tracker
        [SerializeField, Tooltip("Select to enable the Tracker feature when AP starts.")]
        public bool EnableTracker = false;
        public const string EnableTrackerText = "EnableTracker";
        #endregion

		#region Hand
		[SerializeField, Tooltip("Select to enable the Natural Hand feature when AP starts.")]
        public bool EnableNaturalHand = false;
        public const string EnableNaturalHandText = "EnableNaturalHand";

        [SerializeField, Tooltip("Select to enable the Electronic Hand feature when AP starts.")]
        public bool EnableElectronicHand = false;
        public const string EnableElectronicHandText = "EnableElectronicHand";
        #endregion

        #region Eye
        // Tracking
        [SerializeField, Tooltip("Select to enable the Eye Tracking feature when AP starts.")]
        public bool EnableEyeTracking = false;
        public const string EnableEyeTrackingText = "EnableEyeTracking";
        // Expression
        [SerializeField, Tooltip("Select to enable the Eye Expression feature when AP starts.")]
        public bool EnableEyeExpression = false;
        public const string EnableEyeExpressionText = "EnableEyeExpression";
        #endregion

        #region Lip Expression
        [SerializeField, Tooltip("Select to enable the Lip Expression feature when AP starts.")]
        public bool EnableLipExpression = false;
        public const string EnableLipExpressionText = "EnableLipExpression";
		#endregion

		#region Body
		// Tracking
		[SerializeField, Tooltip("Select to enable the Body Tracking feature when AP starts.")]
		public bool EnableBodyTracking = false;
		#endregion

		#region Scene Perception
		[SerializeField, Tooltip("Select to enable the Scene Perception feature.")]
        public bool EnableScenePerception = false;

        [SerializeField, Tooltip("Enable this option if you need to use Scene Mesh.")]
        public bool EnableSceneMesh = false;
        #endregion

        #region Marker
        [SerializeField, Tooltip("Select to enable the Marker feature.")]
        public bool EnableMarker = false;
        #endregion

        public bool enableAutoFallbackForMultiLayerProperty
        {
            get
            {
                return enableAutoFallbackForMultiLayer;
            }
        }

        void Awake()
        {
            Debug.Log("Wave.XR.Settings.WaveXRSettings Awake() debugLogFlagForUnity: " + debugLogFlagForUnity);
#if UNITY_EDITOR
            if (Application.isEditor)
                return;
#endif
            s_RuntimeInstance = this;
        }

        public static WaveXRSettings GetInstance()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                Object obj = null;
                UnityEditor.EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out obj);
                if (obj == null || !(obj is WaveXRSettings))
                    return null;
                return (WaveXRSettings)obj;
            }
#endif
            if (s_RuntimeInstance == null)
                s_RuntimeInstance = new WaveXRSettings();
            return s_RuntimeInstance;
        }
    }
}
