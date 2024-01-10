using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Loader;
using static Wave.XR.Constants;

namespace Wave.XR.Settings
{
    public class SettingsHelper
    {
        internal static void CheckSinglePass()
        {
            WaveXRSettings appSettings = WaveXRSettings.GetInstance();
#if UNITY_2020_1_OR_NEWER
            Utils.DisplaySubsystem.textureLayout =
                appSettings.preferedStereoRenderingPath == WaveXRSettings.StereoRenderingPath.MultiPass ?
                XRDisplaySubsystem.TextureLayout.SeparateTexture2Ds : XRDisplaySubsystem.TextureLayout.Texture2DArray;
#else
            Utils.DisplaySubsystem.singlePassRenderingDisabled = appSettings.preferedStereoRenderingPath == WaveXRSettings.StereoRenderingPath.MultiPass;
#endif
        }

        // If you want to modify the values here, please make sure you know what it is.
        // For cross platform, we don't use enum of WVR.
        internal static void ProcessRenderConfig(WaveXRSettings appSettings)
        {
            if (appSettings == null)
                return;

            bool allowAMC = appSettings.amcMode != WaveXRSettings.AMCMode.Off && appSettings.amcModeConfirm != 0;
            ulong config = 0;
            if (allowAMC)
            {
                switch (appSettings.amcMode)
                {
                    case WaveXRSettings.AMCMode.Auto:
                        config |= (1 << 6) | (1 << 7);
                        break;
                    case WaveXRSettings.AMCMode.Force_UMC:
                        config |= 1 << 6;
                        break;
                    case WaveXRSettings.AMCMode.Force_PMC:
                        config |= 1 << 7;
                        break;
                }
            }

            if (appSettings.fadeOut)
                config |= (1 << 5);

            if (appSettings.enableFSE)
            {
                config |= (1 << 8);
            }

            if (appSettings.allowSpectatorCamera)
            {
                config |= (1L << 33);
            }

            uint highBytes = (uint)((config >> 32) & 0xFFFFFFFF);
            uint lowBytes = (uint)(config & 0xFFFFFFFF);
            SetInt("renderConfigH", highBytes);
            SetInt("renderConfigL", lowBytes);
        }

        internal static void Process(WaveXRLoader loader)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return;
#endif
            WaveXRSettings appSettings = WaveXRSettings.GetInstance();

            if (appSettings == null)
            {
                Debug.LogWarning("WaveXRSettings instance is null");
                return;
            }

            #region common
            uint logFlagForNative = appSettings.overrideLogFlagForNative ?
                appSettings.debugLogFlagForNative : (uint)DebugLogFlag.Default;
            SetInt(NameDebugLogFlagForNative, logFlagForNative);
            //GetInt(NameDebugLogFlagForUnity, ref appSettings.debugLogFlagForUnity);
            //Debug.Log("WaveXRSettingsHelper: Process development build: " + Debug.isDebugBuild);
            if (Debug.isDebugBuild)
            {
                SetBool(NameUseCMPChecker, appSettings.useCMPChecker);
                Debug.Log("WaveXRSettingsHelper: useCMPChecker " + appSettings.useCMPChecker);
            }
            #endregion common

            #region rendering
            CheckSinglePass();

            SetBool("sRGB", QualitySettings.activeColorSpace == ColorSpace.Linear);
            SetInt("qsMSAA", (uint)QualitySettings.antiAliasing);
            SetBool("useDoubleWidth", appSettings.useDoubleWidth);
            SetBool(NameUseRenderMask, appSettings.useRenderMask);
            SetInt(NameEnableTimeWarpStabilizedMode, (uint)appSettings.enableTimeWarpStabilizedMode);

            SetBool(NameUseAdaptiveQuality, appSettings.adaptiveQualityMode != WaveXRSettings.AdaptiveQualityMode.Disabled);
            SetInt(NameAdaptiveQualityMode, (uint)appSettings.adaptiveQualityMode);
            SetBool("AQ_AutoFoveation", appSettings.AQ_AutoFoveation);
            SetBool("AQ_SendQualityEvent", appSettings.AQ_SendQualityEvent);
            SetBool("useAQDynamicResolution", appSettings.useAQDynamicResolution);

            SetInt(NameFoveationMode, (uint)appSettings.foveationMode);
            SetFloat(NameLeftClearVisionFOV, appSettings.leftClearVisionFOV);
            SetFloat(NameRightClearVisionFOV, appSettings.rightClearVisionFOV);
            SetInt(NameLeftPeripheralQuality, (uint)appSettings.leftPeripheralQuality);
            SetInt(NameRightPeripheralQuality, (uint)appSettings.rightPeripheralQuality);

            SetBool(NameOverridePixelDensity, appSettings.overridePixelDensity);
            SetFloat(NamePixelDensity, appSettings.pixelDensity);
            SetFloat(NameResolutionScale, appSettings.resolutionScale);
            SetInt("amcMode", (uint)appSettings.amcMode);
            SetInt("amcModeConfirm", (uint)appSettings.amcModeConfirm);

            ProcessRenderConfig(appSettings);

            for (int i = 0; i < 4; i++)
            {
                WaveXRSettings.DisplayGamut gamut = WaveXRSettings.DisplayGamut.Native;
                if (i < appSettings.displayGamutPreferences.Count)
                    gamut = appSettings.displayGamutPreferences[i];
                SetInt("displayGamutPreference" + i, (uint)gamut);
            }

            SetBool(NameThreadPriority, appSettings.overrideThreadPriority);
            // Only RenderThread need set in C.  RenderThread is unable to use command line to set a custom value.
            //SetInt(NameGameThreadPriority, (uint)appSettings.gameThreadPriority);
            SetInt(NameRenderThreadPriority, (uint)appSettings.renderThreadPriority);
            //SetInt(NameJobWorkerThreadPriority, (uint)appSettings.jobWorkerThreadPriority);

            #endregion rendering

            #region Tracker
            SetBool(WaveXRSettings.EnableTrackerText, appSettings.EnableTracker);
            #endregion

            #region Hand
            SetBool(WaveXRSettings.EnableNaturalHandText, appSettings.EnableNaturalHand);
            SetBool(WaveXRSettings.EnableElectronicHandText, appSettings.EnableElectronicHand);
            #endregion

            #region Eye
            // Expression
            SetBool(WaveXRSettings.EnableEyeExpressionText, appSettings.EnableEyeExpression);
            // Tracking
            SetBool(WaveXRSettings.EnableEyeTrackingText, appSettings.EnableEyeTracking);
            #endregion

            #region Lip Expression
            SetBool(WaveXRSettings.EnableLipExpressionText, appSettings.EnableLipExpression);
            #endregion
        }

        public const string NameSRGB = "sRGB";
        public const string NameUseRenderMask = "useRenderMask";
        public const string NameEnableTimeWarpStabilizedMode = "enableTimeWarpStabilizedMode";
        public const string NameUseAdaptiveQuality = "useAdaptiveQuality";
        public const string NameAdaptiveQualityMode = "adaptiveQualityMode";

        public const string NameFoveationMode = "foveationMode";
        public const string NameLeftClearVisionFOV = "leftClearVisionFOV";
        public const string NameRightClearVisionFOV = "rightClearVisionFOV";
        public const string NameLeftPeripheralQuality = "leftPeripheralQuality";
        public const string NameRightPeripheralQuality = "rightPeripheralQuality";

        public const string NameOverridePixelDensity = "overridePixelDensity";
        public const string NamePixelDensity = "pixelDensity";
        public const string NameResolutionScale = "resolutionScale";

        public const string NameDebugLogFlagForNative = "debugLogFlagForNative";
        public const string NameDebugLogFlagForUnity = "debugLogFlagForUnity";
        public const string NameUseCMPChecker = "useCMPChecker";

        public const string NameThreadPriority = "overrideThreadPriority";
        public const string NameGameThreadPriority = "gameThreadPriority";
        public const string NameRenderThreadPriority = "renderThreadPriority";
        public const string NameJobWorkerThreadPriority = "jobWorkerThreadPriority";

        // Set
        [DllImport("wvrunityxr", EntryPoint = "SettingsSetBool")]
        public static extern ErrorCode SetBool(string name, bool value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsSetInt")]
        public static extern ErrorCode SetInt(string name, uint value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsSetFloat")]
        public static extern ErrorCode SetFloat(string name, float value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsSetString")]
        internal static extern ErrorCode SetFloatArray(string name, [In, Out] float[] array, uint length);
        public static ErrorCode SetFloatArray(string name, float[] array)
        {
            return SetFloatArray(name, array, (uint)array.Length);
        }

        [DllImport("wvrunityxr", EntryPoint = "SettingsSetString")]
        internal static extern ErrorCode SetString(string name, System.IntPtr value, uint length);
        public static ErrorCode SetString(string name, string value)
        {
            System.IntPtr ptrValue = Marshal.StringToHGlobalAnsi(value);
            ErrorCode ret = SetString(name, ptrValue, (uint)value.Length);
            Marshal.FreeHGlobal(ptrValue);
            return ret;
        }

        // Get
        [DllImport("wvrunityxr", EntryPoint = "SettingsGetBool")]
        public static extern ErrorCode GetBool(string name, ref bool value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsGetInt")]
        public static extern ErrorCode GetInt(string name, ref uint value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsGetFloat")]
        public static extern ErrorCode GetFloat(string name, ref float value);

        [DllImport("wvrunityxr", EntryPoint = "SettingsGetFloatArray")]
        internal static extern ErrorCode GetFloatArray(string name, ref float[] array, uint bufferSize);
        public static ErrorCode GetFloatArray(string name, ref float[] array)
        {
            uint length = (uint)array.Length;
            if (length > 0xFFFF)
                return ErrorCode.OutOfRange;
            return GetFloatArray(name, ref array, length);  // Native can handle length validation
        }

        [DllImport("wvrunityxr", EntryPoint = "SettingsSetString")]
        public static extern ErrorCode GetString(string name, System.IntPtr value, uint bufferSize);
        public static ErrorCode GetString(string name, ref string value, uint bufferSize)
        {
            if (bufferSize > 0xFFFF)
                return ErrorCode.OutOfRange;
            System.IntPtr ptrValue = Marshal.AllocHGlobal((int)bufferSize);
            ErrorCode ret = GetString(name, ptrValue, (uint)value.Length);
            value = Marshal.PtrToStringAnsi(ptrValue);
            Marshal.FreeHGlobal(ptrValue);
            return ret;
        }
    }
}
