
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
using UnityEngine;
using System;
using System.Diagnostics;

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	public class DirectPreviewControlPanel : EditorWindow
	{
		private const string DEVICE_WIFI_ADDRESS = "wifi_ip_state";
		private const string ENABLE_PREVIEW_IMAGE = "EnablePreviewImage";
		private const string DLL_TRACE_LOG_TO_FILE = "DllTraceLogToFile";
		private const string OUTPUT_IMAGE_TO_FILE = "OutputImagesToFile";
		private const string UPDATE_FREQUENCY = "UpdateFrequency";
		private const string CONNECTTYPE = "ConnectType";
		private const string TARGET_SIZE_RATIO = "TargetSizeRatio";
		private const string TARGET_DEVICE = "DPTargetDevice";

		private string bwifi_ip_textField = "";
		private string wifi_ip_textField = "";
		private bool isRenderImageToggle = false;
		private bool isCurrentRenderImageState = false;

		//private bool isDllLogSavedFlagToggle = false;
		//private bool isCurrentIsDllLogSavedState = false;

		private bool bOutputImagesToFile = false;
		private bool aOutputImagesToFile = false;

		// 	Connection Type
		private string[] ConnectTypeString = new string[] { "USB", "Wi-Fi" };
		private int[] ConnectTypeValue = { 0, 1 };
		private int selectedConnectTypeValue = 1;
		private int prevConnectTypeValue = 1;

		// Update Frequency
		private int selectedFPS = 1;
		private int prevSelectedfps = 1;
		private string[] names = new string[] { "Runtime defined", "15 FPS", "30 FPS", "45 FPS", "60 FPS", "75 FPS" };
		private int[] fps = { 0, 15, 30, 45, 60, 75 };

		// Render target size ratio
		private string[] targetSizeRatioString = new string[] { "1", "0.8", "0.6", "0.4", "0.2" };
		private int[] targetSizeRatio = { 1, 2, 3, 4, 5 };
		private int selectedTargetSizeRatio = 1;
		private int prevTargetSizeRatio = 1;

		// 	Target device
		private string[] TargetDeviceString = new string[] { "Others", "FOCUS+" };
		private int[] TargetDeviceValue = { 0, 1 };
		private int selectedTargetDeviceValue = 1;
		private int prevTargetDeviceValue = 1;

        private bool configFoldout = true;

        private static readonly string PREF_IS_ADB_EXIST = "IsAdbExist";

        public void Awake()
        {
            isAdbExist = EditorPrefs.GetBool(PREF_IS_ADB_EXIST, isAdbExist);
        }

        void ShowConfig()
        {
            if (!EditorPrefs.HasKey(CONNECTTYPE))
            {
                prevConnectTypeValue = 1; // Wifi
            }
            else
            {
                prevConnectTypeValue = EditorPrefs.GetInt(CONNECTTYPE);
            }

            selectedConnectTypeValue = EditorGUILayout.IntPopup("Connect Type: ", prevConnectTypeValue, ConnectTypeString, ConnectTypeValue);

            if (prevConnectTypeValue != selectedConnectTypeValue)
                EditorPrefs.SetInt(CONNECTTYPE, selectedConnectTypeValue);
            if (selectedConnectTypeValue == 0)
            {
                EditorGUILayout.LabelField("Use USB to get data (Pose/Event/...) from device\n" +
                    "Note: HMD will NOT show images.", GUILayout.Height(40));
            }
            else
            {
                EditorGUILayout.LabelField("Use Wi-Fi to get data from device and show images on HMD.\n" +
                    "Suggest to use 5G Wi-Fi to get better performance.", GUILayout.Height(40));

                wifi_ip_textField = EditorPrefs.GetString(DEVICE_WIFI_ADDRESS);
                bwifi_ip_textField = EditorGUILayout.TextField("Device Wi-Fi IP: ", wifi_ip_textField);

                if (!bwifi_ip_textField.Equals(""))
                {
                    EditorPrefs.SetString(DEVICE_WIFI_ADDRESS, bwifi_ip_textField);
                }
                /*
				isCurrentIsDllLogSavedState = EditorPrefs.GetBool(DLL_TRACE_LOG_TO_FILE);

				isDllLogSavedFlagToggle = EditorGUI.Toggle(new Rect(0, 100, position.width, 20), "Save log to file", isCurrentIsDllLogSavedState);

				if (isDllLogSavedFlagToggle)
				{
					EditorPrefs.SetBool(DLL_TRACE_LOG_TO_FILE, true);
				}
				else
				{
					EditorPrefs.SetBool(DLL_TRACE_LOG_TO_FILE, false);
				}*/

                if (!EditorPrefs.HasKey(ENABLE_PREVIEW_IMAGE))
                {
                    isCurrentRenderImageState = true;
                }
                else
                {
                    isCurrentRenderImageState = EditorPrefs.GetBool(ENABLE_PREVIEW_IMAGE);
                }

                
                isRenderImageToggle = EditorGUILayout.Toggle("Enable preview image: ", isCurrentRenderImageState);

                if (isRenderImageToggle)
                {
                    EditorPrefs.SetBool(ENABLE_PREVIEW_IMAGE, true);
                }
                else
                {
                    EditorPrefs.SetBool(ENABLE_PREVIEW_IMAGE, false);
                }

                if (isRenderImageToggle)
                {
                    if (!EditorPrefs.HasKey(UPDATE_FREQUENCY))
                    {
                        prevSelectedfps = 0; // Based on runtime target FPS
                    }
                    else
                    {
                        prevSelectedfps = EditorPrefs.GetInt(UPDATE_FREQUENCY);
                    }

                    selectedFPS = EditorGUILayout.IntPopup("Update frequency: ", prevSelectedfps, names, fps);

                    if (prevSelectedfps != selectedFPS)
                    {
                        UnityEngine.Debug.Log("Set frequency " + selectedFPS);
                        EditorPrefs.SetInt(UPDATE_FREQUENCY, selectedFPS);
                    }
                    // target device
                    if (!EditorPrefs.HasKey(TARGET_DEVICE))
                    {
                        prevTargetDeviceValue = 0; // Based on runtime target FPS
                    }
                    else
                    {
                        prevTargetDeviceValue = EditorPrefs.GetInt(TARGET_DEVICE);
                    }

                    selectedTargetDeviceValue = EditorGUILayout.IntPopup("Render target device: ", prevTargetDeviceValue, TargetDeviceString, TargetDeviceValue);

                    if (prevTargetDeviceValue != selectedTargetDeviceValue)
                    {
                        UnityEngine.Debug.Log("Render target device " + selectedTargetDeviceValue);
                        EditorPrefs.SetInt(TARGET_DEVICE, selectedTargetDeviceValue);
                    }
                    EditorGUILayout.LabelField("Please re-install Device APK if changed target device.");
                    //--

                    if (!EditorPrefs.HasKey(TARGET_SIZE_RATIO))
                    {
                        prevTargetSizeRatio = 4; // Target size ratio = 0.4f
                    }
                    else
                    {
                        prevTargetSizeRatio = EditorPrefs.GetInt(TARGET_SIZE_RATIO);
                    }

                    selectedTargetSizeRatio = EditorGUILayout.IntPopup("Preview image ratio: ", prevTargetSizeRatio, targetSizeRatioString, targetSizeRatio);

                    if (prevTargetSizeRatio != selectedTargetSizeRatio)
                    {
                        EditorPrefs.SetInt(TARGET_SIZE_RATIO, selectedTargetSizeRatio);
                    }

                    bOutputImagesToFile = EditorPrefs.GetBool(OUTPUT_IMAGE_TO_FILE);

                    aOutputImagesToFile = EditorGUILayout.Toggle("Regularly save images: ", bOutputImagesToFile);

                    if (aOutputImagesToFile)
                    {
                        EditorPrefs.SetBool(OUTPUT_IMAGE_TO_FILE, true);
                    }
                    else
                    {
                        EditorPrefs.SetBool(OUTPUT_IMAGE_TO_FILE, false);
                    }
                }
            }
        }

        public static bool isAdbChecked = false;
        public static bool isAdbExist = false;

        public void CheckAdbExist()
        {
            if (isAdbChecked && isAdbExist)
                return;

            isAdbChecked = true;
            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
                myProcess.StartInfo.Arguments = "/c where /Q adb";
                myProcess.Start();
                myProcess.WaitForExit();
                int ExitCode = myProcess.ExitCode;
                if (ExitCode == 0)
                {
                    UnityEngine.Debug.Log("adb.exe exist in path.");
                    isAdbExist = true;
                    EditorPrefs.SetBool(PREF_IS_ADB_EXIST, true);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("adb.exe does not exist in path.");
                    isAdbExist = false;
                    EditorPrefs.SetBool(PREF_IS_ADB_EXIST, false);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        void ShowButtons()
        {
            if (GUILayout.Button("Start streaming server"))
            {
                StreamingServer.StartStreamingServer();
                configFoldout = false;
            }
            if (GUILayout.Button("Stop streaming server"))
            {
                StreamingServer.StopStreamingServer();
            }
            //if (GUILayout.Button("Start Device APK"))
            //{
            //    DirectPreviewAPK.StartSimulator();
            //    configFoldout = false;
            //}
            //if (GUILayout.Button("Stop Device APK"))
            //{
            //    DirectPreviewAPK.StopSimulator();
            //}
            if (GUILayout.Button("Install Device APK"))
            {
                DirectPreviewAPK.InstallSimulator();
            }
        }

        void OnGUI()
		{
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Application is Playing\n" + "Before any DirectPreview operation, please stop playing.", MessageType.None);
                return;
            }

            if (!isAdbChecked)
                CheckAdbExist();

            if (!isAdbExist)
                EditorGUILayout.HelpBox("Adb does not exist.\n" +
                    "Please set \"<your android sdk>\\platform-tools\" into your environment path, and then reboot your PC to take effect."
                    , MessageType.Error);

            //configFoldout = EditorGUILayout.Foldout(configFoldout, "Config");
            //if (configFoldout)
            //{
            //    EditorGUI.indentLevel++;
            //    ShowConfig();
            //    EditorGUI.indentLevel--;
            //}

            ShowButtons();

            this.Repaint();
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}
	}

	[InitializeOnLoad]
	public static class CheckIfSimulatorEnabled
	{
		private const string MENU_NAME = "Wave/DirectPreview/EnableDirectPreview";

		private static bool enabled_;
		private const string DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME = "Wave/DirectPreview/ControlPanel";
		/// Called on load thanks to the InitializeOnLoad attribute
		static CheckIfSimulatorEnabled()
		{
			CheckIfSimulatorEnabled.enabled_ = EditorPrefs.GetBool(CheckIfSimulatorEnabled.MENU_NAME, false);
			//if (CheckIfSimulatorEnabled.enabled_)
			//{
			//	switchGaphicsEmulationInner(true);
			//}

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfSimulatorEnabled.enabled_);
			};
		}

		[MenuItem(CheckIfSimulatorEnabled.MENU_NAME, priority = 601)]
		private static void ToggleAction()
		{
			if (!CheckIfSimulatorEnabled.enabled_)
			{

				if (!EditorPrefs.GetBool(CheckIfSimulatorEnabled.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME))
				{
                    DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
					window.Show();
                    EditorPrefs.SetBool(CheckIfSimulatorEnabled.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME, true);
				}

				//switchGaphicsEmulationInner(true);
			}
			else
			{

				//switchGaphicsEmulationInner(false);
			}
			/// Toggling action
			PerformAction(!CheckIfSimulatorEnabled.enabled_);
		}

		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfSimulatorEnabled.MENU_NAME, enabled);
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfSimulatorEnabled.MENU_NAME, enabled);

			CheckIfSimulatorEnabled.enabled_ = enabled;
		}

		[MenuItem(CheckIfSimulatorEnabled.MENU_NAME, validate = true, priority = 601)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfSimulatorEnabled.MENU_NAME, enabled_);
			return true;
		}

		[MenuItem(CheckIfSimulatorEnabled.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME, priority = 602)]
		private static void OptToggleAction()
		{
            DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
            window.Show();
        }
		//// Switch to emulation mode.
		//public static void switchGaphicsEmulationInner(bool isSwitchGaphicsEmulation)
		//{
		//	//UnityEngine.Debug.Log("switch to multi-pass: " + isSwitchGaphicsEmulation);
		//	//EditorPrefs.SetBool("isMirrorToDevice", isSwitchGaphicsEmulation);
		//	/*try
		//	{
		//		if ( isSwitchGaphicsEmulation == true ) {
		//			UnityEngine.Debug.Log("switchGaphicsEmulationInner to D3D");
		//			// Switch to no emulator
		//			EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
		//			// Switch to multipass
		//			EditorPrefs.SetBool("isMirrorToDevice", true);
		//		} else {
		//			UnityEngine.Debug.Log("switchGaphicsEmulationInner to OpenGL ES 3.0");
		//			// Set Graphic emulation back to OpenGL
		//			EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/OpenGL ES 3.0");
		//			// Set back to auto of singlepass
		//			EditorPrefs.SetBool("isMirrorToDevice", false);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		UnityEngine.Debug.LogError(e);
		//	}*/
		//}
	}
}
#endif