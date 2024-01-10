// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Wave.XR.Constants;

namespace Wave.XR.Settings
{
    public class AMCDialog
    {
        public static readonly string amcDialogTitleNoEssence = "AMC Mode";
        public static readonly string amcDialogContentNoEssence =
            "Before using AMC, you need import the\n" +
            "\"VIVE Wave XR Plugin - Essence\" package.\n" +
            "Without it, the AMC mode keep off.";
        public static readonly string amcDialogTitle = "Information about AMC Mode";
        public static readonly string amcDialogContent =
            "Some impacts and requirement you should know before choosing any AMC Mode:\n\n" +

            "AMC means Adaptive Motion Compensator.  " +
            "When AMC is activated, the FPS may drop but the motion of objects on screen may look smoother after the compensation.  " +
            "However the compensated result may have some graphic artifacts.  " +
            "Please note that AMC is an experimental feature and might cause certian APPs to run in abnormal behaviors.  " +
            "See document for more detail.\n\n" +

            "* Mode 'Off'.  AMC is disabled.\n" +
            "* Mode 'Auto'.  AMC is allowed to be activated automatically when the FPS is lower than a certain level.  In this mode, UMC or PMC methods will be choosen automatically.\n" +
            "* Mode 'Force_UMC' and 'Force_PMC'.  AMC is always activated in the choosen method no matter what the current FPS is.  This mode should be used for test only.\n\n" +

            "If you choose a mode which PMC method is possibly applied, WaveSDK will automatically attach a script, WaveXR_AMCProcess, to your main camera's GameObject in the runtime.  The attaching will happen when scene is loaded.  The script will add a CommandBuffer to the render pipeline and help AMC do the depth texture process.  This script require the camera's 'Target Eye' is 'Both' and the Camera's 'depthTextureMode' is not 'DepthNormals' or 'MotionVectors'.  If the requirement are not met, the PMC method will not be activate.\n\n" +

            "When the AQ (Adaptive Quality) is enabled, AMC will be controlled by AQ, but you still have to select mode 'auto' to let AQ include this feature.\n\n";

        public static readonly string amcDialogOkBtn = "Acknowledge";
        public static readonly string amcDialogCancelBtn = "Cancel";
        public static readonly string amcDialogOk2Btn = "Acknowledge and don't ask me again";
    }

    public class DisplayGamutPreference
    {
        private ReorderableList reorderableList;
        private SerializedProperty property;
        public DisplayGamutPreference(SerializedObject serializedObject)
        {
            property = serializedObject.FindProperty("displayGamutPreferences");

            reorderableList = new ReorderableList(serializedObject, property, true, true, false, false);
            reorderableList.drawHeaderCallback = DrawHeader;
            reorderableList.drawElementCallback = DrawListItems;
        }

        public void Draw()
        {
            reorderableList.DoLayoutList();
        }

        void DrawHeader(Rect rect)
        {
            string tooltip = "Display gamut preferences.  Wave will try to apply gamut for the device from the top of the list.  When the first gamut is accepted, Wave will stop try the left.  Not all kind of gamut preference list here can be accepted by device.  The default value is Native.  The Native gamut will use the full capable of display's color.  This is the enterprise feature.";
            EditorGUI.LabelField(rect, new GUIContent("Display Gamut", tooltip));
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

            EditorGUI.LabelField(rect, new GUIContent(((WaveXRSettings.DisplayGamut)element.intValue).ToString()));
        }
    }

    /// <summary>
    /// Simple custom editor used to show how to enable custom UI for XR Management
    /// configuraton data.
    /// </summary>
    [CustomEditor(typeof(WaveXRSettings))]
    public class WaveXRSettingsEditor : UnityEditor.Editor
    {
        static string PropertyName_PreferedStereoRenderingPath = "preferedStereoRenderingPath";
        static GUIContent Label_PreferedStereoRenderingPath = new GUIContent("Render Mode");
        SerializedProperty Property_PreferedStereoRenderingPath;

        static string PropertyName_UseDoubleWidth = "useDoubleWidth";
        static GUIContent Label_UseDoubleWidth = new GUIContent("Use DoubleWidth eye texture");
        SerializedProperty Property_UseDoubleWidth;

        static string PropertyName_AdaptiveQualityMode = "adaptiveQualityMode";
        static GUIContent Label_AdaptiveQualityMode = new GUIContent("Adaptive Quality Mode");
        SerializedProperty Property_AdaptiveQualityMode;

        static string PropertyName_AQSendEvent = "AQ_SendQualityEvent";
        static GUIContent Label_AQSendEvent = new GUIContent("Send Quality Event");
        SerializedProperty Property_AQSendEvent;

        static string PropertyName_AQAutoFoveation = "AQ_AutoFoveation";
        static GUIContent Label_AQAutoFoveation = new GUIContent("Auto Foveation");
        SerializedProperty Property_AQAutoFoveation;

        static string PropertyName_UseRenderMask = "useRenderMask";
        static GUIContent Label_UseRenderMask = new GUIContent("RenderMask");
        SerializedProperty Property_UseRenderMask;

        static string PropertyName_EnableTimeWarpStabilizedMode = "enableTimeWarpStabilizedMode";
        static GUIContent Label_EnableTimeWarpStabilizedMode = new GUIContent("Enable TimeWarp Stabilized Mode");
        SerializedProperty Property_EnableTimeWarpStabilizedMode;

        static string PropertyName_UseAQDR = "useAQDynamicResolution";
        static GUIContent Label_UseAQDR = new GUIContent("Dynamic Resolution");
        SerializedProperty Property_UseAQDR;

        static string PropertyName_DefaultIndex = "DR_DefaultIndex";
        static GUIContent Label_DefaultIndex = new GUIContent("Default Index");
        SerializedProperty Property_DefaultIndex;

        static string PropertyName_TextSize = "DR_TextSize";
        static GUIContent Label_TextSize = new GUIContent("Text Size");
        SerializedProperty Property_TextSize;

        static string PropertyName_ResolutionScaleList = "DR_ResolutionScaleList";
        static GUIContent Label_ResolutionScaleList = new GUIContent("Resolution List");
        SerializedProperty Property_ResolutionScaleList;

        static string PropertyName_FoveationMode = "foveationMode";
        static GUIContent Label_FoveationMode = new GUIContent("Foveation mode");
        SerializedProperty Property_FoveationMode;

        static string PropertyName_LeftClearVisionFOV = "leftClearVisionFOV";
        static GUIContent Label_LeftClearVisionFOV = new GUIContent("LeftClearVisionFOV");
        SerializedProperty Property_LeftClearVisionFOV;

        static string PropertyName_RightClearVisionFOV = "rightClearVisionFOV";
        static GUIContent Label_RightClearVisionFOV = new GUIContent("RightClearVisionFOV");
        SerializedProperty Property_RightClearVisionFOV;

        static string PropertyName_LeftPeripheralQuality = "leftPeripheralQuality";
        static GUIContent Label_LeftPeripheralQuality = new GUIContent("LeftPeripheralQuality");
        SerializedProperty Property_LeftPeripheralQuality;

        static string PropertyName_RightPeripheralQuality = "rightPeripheralQuality";
        static GUIContent Label_RightPeripheralQuality = new GUIContent("RightPeripheralQuality");
        SerializedProperty Property_RightPeripheralQuality;

        static string PropertyName_OverridePixelDensity = "overridePixelDensity";
        static GUIContent Label_OverridePixelDensity = new GUIContent("Override system PixelDensity");
        SerializedProperty Property_OverridePixelDensity;

        static string PropertyName_PixelDensity = "pixelDensity";
        static GUIContent Label_PixelDensity = new GUIContent("PixelDensity");
        SerializedProperty Property_PixelDensity;

        static string PropertyName_ResolutionScale = "resolutionScale";
        static GUIContent Label_ResolutionScale = new GUIContent("ResolutionScale");
        SerializedProperty Property_ResolutionScale;

        static string PropertyName_ThreadPriority = "overrideThreadPriority";
        static string Warning_ThreadPriority = "WARNING: This setting is experimental and may not have good effect.  Use it at your own risk.";
        static GUIContent Label_ThreadPriority = new GUIContent("Override Thread Priority", Warning_ThreadPriority);
        SerializedProperty Property_ThreadPriority;

        static string PropertyName_GameThreadPriority = "gameThreadPriority";
        static GUIContent Label_GameThreadPriority = new GUIContent("Game thread priority", "Default is 0.  Its range is from -20 to 19, and the lower number the higher priority.");
        SerializedProperty Property_GameThreadPriority;

        static string PropertyName_RenderThreadPriority = "renderThreadPriority";
        static GUIContent Label_RenderThreadPriority = new GUIContent("Render thread priority", "Default is -2.  Its range is from -20 to 19, and the lower number the higher priority.");
        SerializedProperty Property_RenderThreadPriority;

        static string PropertyName_JobWorkerThreadPriority = "jobWorkerThreadPriority";
        static GUIContent Label_JobWorkerThreadPriority = new GUIContent("Job Worker thread priority", "Default is 0.  Its range is from -20 to 19, and the lower number the higher priority.");
        SerializedProperty Property_JobWorkerThreadPriority;

        static string PropertyName_AMCMode = "amcMode";
        static GUIContent Label_AMCMode = new GUIContent("AMC mode");
        SerializedProperty Property_AMCMode;
        static string PropertyName_AMCModeConfirm = "amcModeConfirm";
        SerializedProperty Property_AMCModeConfirm;

        static string PropertyName_FadeOut = "fadeOut";
        static GUIContent Label_FadeOut = new GUIContent("Enable FadeOut", "Default is off.  Before you enable this option, please see online document \"FadeOut effect\" chapter first.");
        SerializedProperty Property_FadeOut;

        static string PropertyName_EnableFSE = "enableFSE";
        static GUIContent Label_EnableFSE = new GUIContent("Enable FSE", "Disabled by default. Enable this option to enable the Frame Sharpness Enhancement feature.\nWhen enabled, the final image will be sharpened which can improve things like text sharpness.");
        SerializedProperty Property_EnableFSE;

        static string PropertyName_FSELevel = "FSE_Level";
        static GUIContent Label_FSELevel = new GUIContent("FSE Level", "Set the enhancement level of the Frame Sharpness Enhancement feature.\nThe higher the level, the stronger the sharpening effect.");
        SerializedProperty Property_FSELevel;

        static string PropertyName_SupportedFPS = "supportedFPS";
        static GUIContent Label_SupportedFPS = new GUIContent("Supported FPS");
        SerializedProperty Property_SupportedFPS;

        static string PropertyName_EnableAutoFallbackForMultiLayer = "enableAutoFallbackForMultiLayer";
        static GUIContent Label_EnableAutoFallbackForMultiLayer = new GUIContent("Multi-Layer auto fallback");
        SerializedProperty Property_EnableAutoFallbackForMultiLayer;

        static string PropertyName_WaveXRFolder = "waveXRFolder";
        static GUIContent Label_WaveXRFolder = new GUIContent("Wave XR Folder");
        SerializedProperty Property_WaveXRFolder;

        static string PropertyName_WaveEssenceFolder = "waveEssenceFolder";
        static GUIContent Label_WaveEssenceFolder = new GUIContent("Wave Essence Folder");
        SerializedProperty Property_WaveEssenceFolder;

        DisplayGamutPreference displayGamutPreference = null;

        static string PropertyName_debugLogFlagForUnity = "debugLogFlagForUnity";
        static GUIContent Label_debugLogFlagForUnity = new GUIContent("Log Flag for Unity");
        SerializedProperty Property_debugLogFlagForUnity;

        static string PropertyName_LogFlagForNative = "debugLogFlagForNative";
        static GUIContent Label_LogFlagForNative = new GUIContent("LogFlagForNative");
        SerializedProperty Property_LogFlagForNative;

        static string PropertyName_OverrideLogFlag = "overrideLogFlagForNative";
        static GUIContent Label_OverrideLogFlag = new GUIContent("Override Log Flag for Native");
        SerializedProperty Property_OverrideLogFlag;

        static string PropertyName_UseCMPChecker = "useCMPChecker";
        static GUIContent Label_UseCMPChecker = new GUIContent("CompatibilityChecker");
        SerializedProperty Property_UseCMPChecker;

        #region Tracker
        static string PropertyName_EnableTracker = "EnableTracker";
        static GUIContent Label_EnableTracker = new GUIContent("Enable Tracker");
        SerializedProperty Property_EnableTracker = null;
        #endregion

        #region Hand
        static string PropertyName_EnableNaturalHand = "EnableNaturalHand";
        static GUIContent Label_EnableNaturalHand = new GUIContent("Enable Natural Hand");
        SerializedProperty Property_EnableNaturalHand = null;
        static string PropertyName_EnableElectronicHand = "EnableElectronicHand";
        static GUIContent Label_EnableElectronicHand = new GUIContent("Enable Electronic Hand");
        SerializedProperty Property_EnableElectronicHand = null;
        #endregion

        #region Eye
        // Tracking
        static string PropertyName_EnableEyeTracking = "EnableEyeTracking";
        static GUIContent Label_EnableEyeTracking = new GUIContent("Enable Eye Tracking");
        SerializedProperty Property_EnableEyeTracking = null;
        // Expression
        static string PropertyName_EnableEyeExpression = "EnableEyeExpression";
        static GUIContent Label_EnableEyeExpression = new GUIContent("Enable Eye Expression");
        SerializedProperty Property_EnableEyeExpression = null;
        #endregion

        #region Lip Expression
        static string PropertyName_EnableLipExpression = "EnableLipExpression";
        static GUIContent Label_EnableLipExpression = new GUIContent("Enable Lip Expression");
        SerializedProperty Property_EnableLipExpression = null;
		#endregion

		#region Body
		static string PropertyName_EnableBodyTracking = "EnableBodyTracking";
		static GUIContent Label_EnableBodyTracking = new GUIContent("Enable Body Tracking");
		SerializedProperty Property_EnableBodyTracking = null;
		#endregion

		#region Scene Perception
		static string PropertyName_EnableScenePerception = "EnableScenePerception";
        static GUIContent Label_EnableScenePerception = new GUIContent("Enable Scene Perception");
        SerializedProperty Property_EnableScenePerception = null;

        static string PropertyName_EnableSceneMesh = "EnableSceneMesh";
        static GUIContent Label_EnableSceneMesh = new GUIContent("Enable Scene Mesh");
        SerializedProperty Property_EnableSceneMesh = null;
        #endregion

        #region Marker
        static string PropertyName_EnableMarker = "EnableMarker";
        static GUIContent Label_EnableMarker = new GUIContent("Enable Marker");
        SerializedProperty Property_EnableMarker = null;
        #endregion

        static string PropertyName_AllowSpectatorCamera = "allowSpectatorCamera";
        static GUIContent Label_AllowSpectatorCamera = new GUIContent("Allow Spectator Camera", "Allow this plugin to generate a spectator camera to produce images for Screenshot, Recording, or Broadcast usages.This camera will consume more performance when it is activated, but captured image will have better FOV result.");
        SerializedProperty Property_AllowSpectatorCamera;
        
        static string PropertyName_AllowSpectatorCameraCapture360Image = 
	        "allowSpectatorCameraCapture360Image";
        static GUIContent Label_AllowSpectatorCameraCapture360Image = new GUIContent(
	        "Allow Capture 360 Image",
	        "Allow the spectator camera to capture 360 images. Addition Request:\n" +
	        "1.) Open the \"enable360StereoCapture\" in the Unity Player Setting " +
	        "Page");
        SerializedProperty Property_AllowSpectatorCameraCapture360Image;


        enum Platform
        {
            Standalone,
            Android
        }

        /// <summary>Override of Editor callback.</summary>
        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                if (EditorGUIUtility.currentViewWidth > 200)
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.25f;
                AndroidSettings();
            }

            if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
            {
                EditorGUILayout.LabelField("Standalone specific things");
            }

            EditorGUILayout.EndBuildTargetSelectionGrouping();

            if (GUILayout.Button("Remove Settings"))
                RemoveSettings();
        }

        bool foldoutRendering = true;
        bool foldoutTracker = true;
        bool foldoutHand = true;
        bool foldoutEye = true;
		bool foldoutBody = true;
		bool foldoutLipExpression = true;
        bool foldoutScenePerception = true;
        bool foldoutMarker = true;
        bool foldoutCommon = true;

        public void AndroidSettings()
        {
            if (Property_PreferedStereoRenderingPath == null) Property_PreferedStereoRenderingPath = serializedObject.FindProperty(PropertyName_PreferedStereoRenderingPath);
            if (Property_UseDoubleWidth == null) Property_UseDoubleWidth = serializedObject.FindProperty(PropertyName_UseDoubleWidth);
            if (Property_AdaptiveQualityMode == null) Property_AdaptiveQualityMode = serializedObject.FindProperty(PropertyName_AdaptiveQualityMode);
            if (Property_AQSendEvent == null) Property_AQSendEvent = serializedObject.FindProperty(PropertyName_AQSendEvent);
            if (Property_AQAutoFoveation == null) Property_AQAutoFoveation = serializedObject.FindProperty(PropertyName_AQAutoFoveation);
            if (Property_UseRenderMask == null) Property_UseRenderMask = serializedObject.FindProperty(PropertyName_UseRenderMask);
            if (Property_EnableTimeWarpStabilizedMode == null) Property_EnableTimeWarpStabilizedMode = serializedObject.FindProperty(PropertyName_EnableTimeWarpStabilizedMode);
            if (Property_UseAQDR == null) Property_UseAQDR = serializedObject.FindProperty(PropertyName_UseAQDR);
            if (Property_ResolutionScaleList == null) Property_ResolutionScaleList = serializedObject.FindProperty(PropertyName_ResolutionScaleList);
            if (Property_TextSize == null) Property_TextSize = serializedObject.FindProperty(PropertyName_TextSize);
            if (Property_DefaultIndex == null) Property_DefaultIndex = serializedObject.FindProperty(PropertyName_DefaultIndex);

            if (Property_FoveationMode == null) Property_FoveationMode = serializedObject.FindProperty(PropertyName_FoveationMode);
            if (Property_LeftClearVisionFOV == null) Property_LeftClearVisionFOV = serializedObject.FindProperty(PropertyName_LeftClearVisionFOV);
            if (Property_RightClearVisionFOV == null) Property_RightClearVisionFOV = serializedObject.FindProperty(PropertyName_RightClearVisionFOV);
            if (Property_LeftPeripheralQuality == null) Property_LeftPeripheralQuality = serializedObject.FindProperty(PropertyName_LeftPeripheralQuality);
            if (Property_RightPeripheralQuality == null) Property_RightPeripheralQuality = serializedObject.FindProperty(PropertyName_RightPeripheralQuality);

            if (Property_OverridePixelDensity == null) Property_OverridePixelDensity = serializedObject.FindProperty(PropertyName_OverridePixelDensity);
            if (Property_PixelDensity == null) Property_PixelDensity = serializedObject.FindProperty(PropertyName_PixelDensity);
            if (Property_ResolutionScale == null) Property_ResolutionScale = serializedObject.FindProperty(PropertyName_ResolutionScale);

            if (Property_ThreadPriority == null) Property_ThreadPriority = serializedObject.FindProperty(PropertyName_ThreadPriority);
            if (Property_GameThreadPriority == null) Property_GameThreadPriority = serializedObject.FindProperty(PropertyName_GameThreadPriority);
            if (Property_RenderThreadPriority == null) Property_RenderThreadPriority = serializedObject.FindProperty(PropertyName_RenderThreadPriority);
            if (Property_JobWorkerThreadPriority == null) Property_JobWorkerThreadPriority = serializedObject.FindProperty(PropertyName_JobWorkerThreadPriority);

            if (Property_AMCMode == null) Property_AMCMode = serializedObject.FindProperty(PropertyName_AMCMode);
            if (Property_AMCModeConfirm == null) Property_AMCModeConfirm = serializedObject.FindProperty(PropertyName_AMCModeConfirm);

            if (Property_FadeOut == null) Property_FadeOut = serializedObject.FindProperty(PropertyName_FadeOut);

            if (Property_EnableFSE == null) Property_EnableFSE = serializedObject.FindProperty(PropertyName_EnableFSE);
            if (Property_FSELevel == null) Property_FSELevel = serializedObject.FindProperty(PropertyName_FSELevel);

            if (Property_AllowSpectatorCamera == null) Property_AllowSpectatorCamera = serializedObject.FindProperty(PropertyName_AllowSpectatorCamera);
			if (Property_AllowSpectatorCameraCapture360Image == null) Property_AllowSpectatorCameraCapture360Image = serializedObject.FindProperty(PropertyName_AllowSpectatorCameraCapture360Image);
            
			if (Property_SupportedFPS == null) Property_SupportedFPS = serializedObject.FindProperty(PropertyName_SupportedFPS);
            if (Property_WaveXRFolder == null) Property_WaveXRFolder = serializedObject.FindProperty(PropertyName_WaveXRFolder);
            if (Property_WaveEssenceFolder == null) Property_WaveEssenceFolder = serializedObject.FindProperty(PropertyName_WaveEssenceFolder);

            if (Property_EnableAutoFallbackForMultiLayer == null) Property_EnableAutoFallbackForMultiLayer = serializedObject.FindProperty(PropertyName_EnableAutoFallbackForMultiLayer);

            if (Property_debugLogFlagForUnity == null) Property_debugLogFlagForUnity = serializedObject.FindProperty(PropertyName_debugLogFlagForUnity);
            if (Property_LogFlagForNative == null) Property_LogFlagForNative = serializedObject.FindProperty(PropertyName_LogFlagForNative);
            if (Property_OverrideLogFlag == null) Property_OverrideLogFlag = serializedObject.FindProperty(PropertyName_OverrideLogFlag);
            if (Property_UseCMPChecker == null) Property_UseCMPChecker = serializedObject.FindProperty(PropertyName_UseCMPChecker);


            #region Tracker
            if (Property_EnableTracker == null) Property_EnableTracker = serializedObject.FindProperty(PropertyName_EnableTracker);
            #endregion

            #region Hand
            if (Property_EnableNaturalHand == null) Property_EnableNaturalHand = serializedObject.FindProperty(PropertyName_EnableNaturalHand);
            if (Property_EnableElectronicHand == null) Property_EnableElectronicHand = serializedObject.FindProperty(PropertyName_EnableElectronicHand);
            #endregion

            #region Eye
            // Expression
            if (Property_EnableEyeExpression == null) Property_EnableEyeExpression = serializedObject.FindProperty(PropertyName_EnableEyeExpression);
            // Tracking
            if (Property_EnableEyeTracking == null) Property_EnableEyeTracking = serializedObject.FindProperty(PropertyName_EnableEyeTracking);
            #endregion

            #region Lip Expression
            if (Property_EnableLipExpression == null) Property_EnableLipExpression = serializedObject.FindProperty(PropertyName_EnableLipExpression);
			#endregion

			#region Body
			// Tracking
			if (Property_EnableBodyTracking == null) Property_EnableBodyTracking = serializedObject.FindProperty(PropertyName_EnableBodyTracking);
			#endregion

			#region Scene Perception
			if (Property_EnableScenePerception == null) Property_EnableScenePerception = serializedObject.FindProperty(PropertyName_EnableScenePerception);
            if (Property_EnableSceneMesh == null) Property_EnableSceneMesh = serializedObject.FindProperty(PropertyName_EnableSceneMesh);
            #endregion

            #region Marker
            if (Property_EnableMarker == null) Property_EnableMarker = serializedObject.FindProperty(PropertyName_EnableMarker);
            #endregion

            if (displayGamutPreference == null)
            {
                displayGamutPreference = new DisplayGamutPreference(serializedObject);
            }

            bool guiEnableLastCond = false;

            foldoutRendering = EditorGUILayout.Foldout(foldoutRendering, "Rendering");
            if (foldoutRendering)
            {
                EditorGUI.indentLevel++;

                //displayGamutPreference.Draw();

                EditorGUILayout.PropertyField(Property_PreferedStereoRenderingPath, Label_PreferedStereoRenderingPath);

#if false
                // Double Width is an experimental feature.
                if (((WaveXRSettings.StereoRenderingPath)Property_PreferedStereoRenderingPath.intValue) == WaveXRSettings.StereoRenderingPath.MultiPass)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(Property_UseDoubleWidth, Label_UseDoubleWidth);
                    EditorGUI.indentLevel--;
                }
#endif

                AdaptiveQualityGUI();

                EditorGUILayout.PropertyField(Property_UseRenderMask, Label_UseRenderMask);
                EditorGUILayout.PropertyField(Property_EnableTimeWarpStabilizedMode, Label_EnableTimeWarpStabilizedMode);
                EditorGUILayout.PropertyField(Property_FoveationMode, Label_FoveationMode);
                guiEnableLastCond = GUI.enabled;
                GUI.enabled = Property_FoveationMode.enumValueIndex == (int)WaveXRSettings.FoveationMode.Enable && guiEnableLastCond;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_LeftClearVisionFOV, Label_LeftClearVisionFOV);
                EditorGUILayout.PropertyField(Property_RightClearVisionFOV, Label_RightClearVisionFOV);
                EditorGUILayout.PropertyField(Property_LeftPeripheralQuality, Label_LeftPeripheralQuality);
                EditorGUILayout.PropertyField(Property_RightPeripheralQuality, Label_RightPeripheralQuality);
                EditorGUI.indentLevel--;
                GUI.enabled = true;

                EditorGUILayout.PropertyField(Property_OverridePixelDensity, Label_OverridePixelDensity);
                GUI.enabled = Property_OverridePixelDensity.boolValue;
                EditorGUILayout.PropertyField(Property_PixelDensity, Label_PixelDensity);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(Property_ResolutionScale, Label_ResolutionScale);

                AMCGUI();

                GUI.enabled = true;
                EditorGUILayout.PropertyField(Property_FadeOut, Label_FadeOut);
                EditorGUILayout.PropertyField(Property_EnableFSE, Label_EnableFSE);

                FSEGUI();

                //EditorGUILayout.PropertyField(Property_SupportedFPS, Label_SupportedFPS);

                GUI.enabled = false;
                EditorGUILayout.PropertyField(Property_WaveXRFolder, Label_WaveXRFolder);
                EditorGUILayout.PropertyField(Property_WaveEssenceFolder, Label_WaveEssenceFolder);

                GUI.enabled = true;

                MultiLayerGUI();

                #region Spectator Camera Setting
                
                const string acceptButtonString = 
	                "OK";
                const string cancelButtonString = 
	                "Cancel";
                const string openCapture360ImageAdditionRequestTitle =
	                "Additional Request of Capturing 360 Image throughout the Spectator Camera";
                const string openCapture360ImageAdditionRequestDescription =
	                "Allow the spectator camera to capture 360 images. Addition Request:\n" +
	                "1.) Open the \"enable360StereoCapture\" in the Unity Player Setting " +
	                "Page.";
                
                EditorGUILayout.PropertyField(Property_AllowSpectatorCamera, Label_AllowSpectatorCamera);
                var mySettings = target as WaveXRSettings;
                if (mySettings.allowSpectatorCamera)
                {
	                EditorGUI.indentLevel++;

	                EditorGUI.BeginChangeCheck();
	                EditorGUILayout.PropertyField(
		                Property_AllowSpectatorCameraCapture360Image,
		                Label_AllowSpectatorCameraCapture360Image);
	                bool currentValue = mySettings.allowSpectatorCameraCapture360Image;
	                if (EditorGUI.EndChangeCheck())
	                {
		                // currentValue change to True
		                if (!currentValue)
		                {
			                bool isEnable360StereoCapture = PlayerSettings.enable360StereoCapture;
                            if (!isEnable360StereoCapture)
                            {
                                bool acceptDialog1 = EditorUtility.DisplayDialog(
                                    openCapture360ImageAdditionRequestTitle,
                                    openCapture360ImageAdditionRequestDescription,
                                    acceptButtonString,
                                    cancelButtonString);

                                if (acceptDialog1)
                                {
                                    PlayerSettings.enable360StereoCapture = true;
                                }
                                else
                                {
                                    Property_AllowSpectatorCameraCapture360Image.boolValue = false;
                                }
                            }
		                }
	                }

	                EditorGUI.indentLevel--;
                }
                else
                {
	                // if allowSpectatorCamera is disabled,
	                // the attribute of allowSpectatorCameraCapture360Image
	                // should be disabled too.
	                Property_AllowSpectatorCameraCapture360Image.boolValue = false;
                }
                
                #endregion

                ThreadPriorityGUI();

                // Put this at final
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            #region Tracker
            foldoutTracker = EditorGUILayout.Foldout(foldoutTracker, "Tracker");
            if (foldoutTracker)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableTracker, Label_EnableTracker);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            #endregion

            #region Hand
            foldoutHand = EditorGUILayout.Foldout(foldoutHand, "Hand");
            if (foldoutHand)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableNaturalHand, Label_EnableNaturalHand);
                //EditorGUILayout.PropertyField(Property_EnableElectronicHand, Label_EnableElectronicHand);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            #endregion

            #region Eye Expression
            foldoutEye = EditorGUILayout.Foldout(foldoutEye, "Eye");
            if (foldoutEye)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableEyeTracking, Label_EnableEyeTracking);
                WaveXRSettings mySettings = target as WaveXRSettings;
                if (mySettings.EnableEyeTracking) { EditorGUILayout.PropertyField(Property_EnableEyeExpression, Label_EnableEyeExpression); }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            #endregion

            #region Lip Expression
            foldoutLipExpression = EditorGUILayout.Foldout(foldoutLipExpression, "Lip Expression");
            if (foldoutLipExpression)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableLipExpression, Label_EnableLipExpression);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
			#endregion

			#region Body
			foldoutBody = EditorGUILayout.Foldout(foldoutBody, "Body");
			if (foldoutBody)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(Property_EnableBodyTracking, Label_EnableBodyTracking);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();
			#endregion

			#region Scene Perception
			foldoutScenePerception = EditorGUILayout.Foldout(foldoutScenePerception, "Scene Perception");
            if (foldoutScenePerception)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableScenePerception, Label_EnableScenePerception);
                WaveXRSettings mySettings = target as WaveXRSettings;
                if (mySettings.EnableScenePerception) { EditorGUILayout.PropertyField(Property_EnableSceneMesh, Label_EnableSceneMesh); }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            #endregion

            #region Marker
            foldoutMarker = EditorGUILayout.Foldout(foldoutMarker, "Marker");
            if (foldoutMarker)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_EnableMarker, Label_EnableMarker);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            #endregion

            foldoutCommon = EditorGUILayout.Foldout(foldoutCommon, "Common");
            if (foldoutCommon)
            {
                EditorGUI.indentLevel++;
                LogFlagGUI();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        void AdaptiveQualityGUI()
        {
            bool guiEnableLastCond = false;

            // AQ
            EditorGUILayout.PropertyField(Property_AdaptiveQualityMode, Label_AdaptiveQualityMode);
            if (Property_AdaptiveQualityMode.intValue == (int)WaveXRSettings.AdaptiveQualityMode.QualityOrientedMode)
            {
                Property_AQSendEvent.boolValue = true;
                Property_AQAutoFoveation.boolValue = true;
                Property_UseAQDR.boolValue = true;
                Property_ResolutionScaleList.arraySize = 5;
                Property_ResolutionScaleList.GetArrayElementAtIndex(0).floatValue = 1.4f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(1).floatValue = 1.3f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(2).floatValue = 1.2f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(3).floatValue = 1.1f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(4).floatValue = 1.0f;
            }
            else if (Property_AdaptiveQualityMode.intValue == (int)WaveXRSettings.AdaptiveQualityMode.PerformanceOrientedMode)
            {
                Property_AQSendEvent.boolValue = true;
                Property_AQAutoFoveation.boolValue = true;
                Property_UseAQDR.boolValue = true;
                Property_ResolutionScaleList.arraySize = 5;
                Property_ResolutionScaleList.GetArrayElementAtIndex(0).floatValue = 1.0f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(1).floatValue = 0.9f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(2).floatValue = 0.8f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(3).floatValue = 0.7f;
                Property_ResolutionScaleList.GetArrayElementAtIndex(4).floatValue = 0.6f;
            }
            else if (Property_AdaptiveQualityMode.intValue == (int)WaveXRSettings.AdaptiveQualityMode.Disabled)
            {
                return;
            }
            GUI.enabled = Property_AdaptiveQualityMode.intValue == (int)WaveXRSettings.AdaptiveQualityMode.CustomizationMode;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Property_AQSendEvent, Label_AQSendEvent);
            EditorGUILayout.PropertyField(Property_AQAutoFoveation, Label_AQAutoFoveation);

            // DR
            guiEnableLastCond = GUI.enabled;
#if WAVE_ESSENCE
            GUI.enabled = Property_AQSendEvent.boolValue && guiEnableLastCond;
#else
            GUI.enabled = false;
            Property_UseAQDR.boolValue = false;
#endif
            if (Property_AQSendEvent.boolValue == false)
                Property_UseAQDR.boolValue = false;
            EditorGUILayout.PropertyField(Property_UseAQDR, Label_UseAQDR);

            if (Property_UseAQDR.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Property_TextSize, Label_TextSize);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(Property_ResolutionScaleList, Label_ResolutionScaleList, true);
                //Validate values of resolution scale list
                if (EditorGUI.EndChangeCheck())
                {
                    if (Property_ResolutionScaleList.arraySize < 2)
                    {
                        Property_ResolutionScaleList.arraySize = 2;
                        Property_ResolutionScaleList.GetArrayElementAtIndex(0).floatValue = 1f;
                        Property_ResolutionScaleList.GetArrayElementAtIndex(1).floatValue = 0.1f;
                    }
                }

                Property_DefaultIndex.intValue = 0;
                EditorGUI.indentLevel--;
            }
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }

        void AMCGUI()
        {
            EditorGUI.BeginChangeCheck();
            int lastValue = Property_AMCMode.intValue;
            EditorGUILayout.PropertyField(Property_AMCMode, Label_AMCMode);

            if (!EditorGUI.EndChangeCheck() || Property_AMCMode.intValue == 0)
                return;

            bool hasEssencePackage = false;
#if WAVE_ESSENCE
            hasEssencePackage = true;
#endif

            if (!hasEssencePackage && Property_AMCMode.intValue > 1 /* Auto or PMC */)
            {
                // No Essence package.  Fallback to Off.
                Property_AMCMode.intValue = 0;
                Property_AMCModeConfirm.intValue = 0;
                EditorUtility.DisplayDialog(AMCDialog.amcDialogTitleNoEssence, AMCDialog.amcDialogContentNoEssence, "OK");
                return;
            }

            int isConfirmed = Property_AMCModeConfirm.intValue;
            if (isConfirmed != 2 && lastValue == 0)
            {
                int opt = EditorUtility.DisplayDialogComplex(AMCDialog.amcDialogTitle, AMCDialog.amcDialogContent, AMCDialog.amcDialogOkBtn, AMCDialog.amcDialogCancelBtn, AMCDialog.amcDialogOk2Btn);

                switch (opt)
                {
                    case 0:
                        Property_AMCModeConfirm.intValue = 1; // Accept
                        break;

                    case 1:
                        // Cancel.  Fallback to Off.
                        Property_AMCMode.intValue = 0;
                        Property_AMCModeConfirm.intValue = 0; // Not Accept
                        break;

                    case 2:
                        Property_AMCModeConfirm.intValue = 2; // Accept and don't ask again
                        break;
                }
            }
        }

        void ThreadPriorityGUI()
        {
            EditorGUILayout.PropertyField(Property_ThreadPriority, Label_ThreadPriority);
            if (Property_ThreadPriority.boolValue)
            {
                EditorGUI.indentLevel++;
                //EditorUtility.DisplayDialog("Override Thread Priority", Warning_ThreadPriority, "OK");
                EditorGUILayout.PropertyField(Property_GameThreadPriority, Label_GameThreadPriority);
                EditorGUILayout.PropertyField(Property_RenderThreadPriority, Label_RenderThreadPriority);
                EditorGUILayout.PropertyField(Property_JobWorkerThreadPriority, Label_JobWorkerThreadPriority);
                EditorGUI.indentLevel--;
            }
        }

        void MultiLayerGUI()
        {
            bool hasEssencePackage = false;
#if WAVE_ESSENCE
            hasEssencePackage = true;
#endif

            if (hasEssencePackage)
            {
                EditorGUILayout.PropertyField(Property_EnableAutoFallbackForMultiLayer, Label_EnableAutoFallbackForMultiLayer);
                serializedObject.ApplyModifiedProperties();
            }
        }

        void FSEGUI()
        {
            bool hasEssencePackage = false;
#if WAVE_ESSENCE
            hasEssencePackage = true;
#endif

            if (hasEssencePackage && Property_EnableFSE.boolValue)
            {
                EditorGUILayout.PropertyField(Property_FSELevel, Label_FSELevel);
                serializedObject.ApplyModifiedProperties();
            }
        }

        static string[] s_LogTitleUnity = { "Warning", "Info", "Debug", "Verbose" };
        static int[] s_TitleLengthUnity = { 70, 40, 60, 70 };
        static bool foldoutLogUnity = false;

        static string[] LogTitleList =
        {
            "Basic", "Debug", "Lifecyle", "Render", "Input",
        };
        static bool foldoutLog = false;

        void LogFlagGUI()
        {
            foldoutLogUnity = EditorGUILayout.Foldout(foldoutLogUnity, "Log Flag for Unity");
            if (foldoutLogUnity)
            {
                int flags = Property_debugLogFlagForUnity.intValue;

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < s_LogTitleUnity.Length; i++)
                {
                    EditorGUILayout.LabelField(s_LogTitleUnity[i], GUILayout.Width(s_TitleLengthUnity[i]));

                    int bit = 4 + i;
                    int mask = 1 << bit;
                    bool flag = (flags & mask) > 0;
                    bool ret = GUILayout.Button(flag ? "X" : " ", GUILayout.Width(20));
                    if (ret)
                        flags = !flag ? (flags | mask) : (flags & ~mask);
                }
                EditorGUILayout.EndHorizontal();

                Property_debugLogFlagForUnity.intValue = flags;
                EditorGUILayout.PropertyField(Property_debugLogFlagForUnity, Label_debugLogFlagForUnity);
            }

            EditorGUILayout.PropertyField(Property_OverrideLogFlag, Label_OverrideLogFlag);
            foldoutLog = EditorGUILayout.Foldout(foldoutLog, "Log Flag for Native");
            if (foldoutLog)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = Property_OverrideLogFlag.boolValue;
                int flags = Property_LogFlagForNative.intValue;

                for (int i = 0; i < 5; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(LogTitleList[i], GUILayout.Width(80));
                    for (int j = 0; j < 4; j++)
                    {
                        int bit = i * 4 + j;
                        int mask = 1 << bit;
                        bool flag = (flags & mask) > 0;
                        bool ret = GUILayout.Button(flag ? "X" : " ", GUILayout.Width(20));
                        if (ret)
                            flags = !flag ? (flags | mask) : (flags & ~mask);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                Property_LogFlagForNative.intValue = flags;
                EditorGUILayout.PropertyField(Property_LogFlagForNative, Label_LogFlagForNative);
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(Property_UseCMPChecker, Label_UseCMPChecker);
        }

        public static void RemoveSettings()
        {
            // I don't know why the system will remember the old asset in memory.  Use this method to clean them.
            AssetDatabase.DeleteAsset("Assets/XR/Settings/Wave XR Settings.asset");
            EditorBuildSettings.RemoveConfigObject(k_SettingsKey);
        }

#if false
        [MenuItem("WaveVR/XRSDK/Create WaveXRSettings Assets")]
        public static void CreateWaveXRSettingsAssets()
        {
            string path = EditorUtility.SaveFilePanel("Create WaveXRSettings Asset", "Assets/", "WaveXRSettings", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);
            WaveXRSettings instance = CreateInstance<WaveXRSettings>();

            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
        }
#endif
    }

}
