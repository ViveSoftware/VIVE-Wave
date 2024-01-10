// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

#if UNITY_EDITOR
using System.IO;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;

using Wave.XR.Loader;
using UnityEngine;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
using System.Xml;
using Wave.XR.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wave.XR.BuildCheck
{
	static class CustomBuildProcessor
	{
		private static string WaveXRPath = "Assets/Wave/XR";

		const string CustomAndroidManifestPathSrc = "/Platform/Android/AndroidManifest.xml";
		const string AndroidManifestPathSrc = "Packages/" + Constants.SDKPackageName + "/Runtime/Android/AndroidManifest.xml";
		const string Aar2017PathSrc = "Packages/" + Constants.SDKPackageName + "/Runtime/Android/wvr_unity_plugin_2017.aar";
		const string Aar2022PathSrc = "Packages/" + Constants.SDKPackageName + "/Runtime/Android/wvr_unity_plugin_2022.aar";
		const string AndroidManifestPathDest = "Assets/Plugins/Android/AndroidManifest.xml";
		const string AndroidManifestScriptCreatedPath = "Assets/Plugins/Android/AndroidManifest.IsCreatedByScript";
		const string Aar2017PathDest = "Assets/Plugins/Android/wvr_unity_plugin_2017.aar";
		const string Aar2022PathDest = "Assets/Plugins/Android/wvr_unity_plugin_2022.aar";
		const string CustomGradle1Src = "Packages/" + Constants.SDKPackageName + "/Runtime/Android/launcherTemplate.gradle";
		const string CustomGradle2Src = "Packages/" + Constants.SDKPackageName + "/Runtime/Android/mainTemplate.gradle";
		const string CustomGradle1Dest = "Assets/Plugins/Android/launcherTemplate.gradle";
		const string CustomGradle1CreatedPath = "Assets/Plugins/Android/launcherTemplate.IsCreatedByScript";
		const string CustomGradle2Dest = "Assets/Plugins/Android/mainTemplate.gradle";
		const string CustomGradle2CreatedPath = "Assets/Plugins/Android/mainTemplate.IsCreatedByScript";
		const string ForceBuildWVR = "ForceBuildWVR.txt";

		internal static void AddHandTrackingAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkHandtrackingFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						handtracking: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkHandtrackingFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						handtracking: true);
				}
			}
		}

		internal static void AddTrackerAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkTrackerFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						tracker: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkTrackerFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						tracker: true);
				}
			}
		}

		internal static void AddSimultaneousInteractionAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkSimultaneousInteractionFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						simultaneous_interaction: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkSimultaneousInteractionFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						simultaneous_interaction: true);
				}
			}
		}

		internal static void AddEyeTrackingAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkEyeTrackingFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						eyetracking: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkEyeTrackingFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						eyetracking: true);
				}
			}
		}

		internal static void AddBodyTrackingAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkBodyTrackingFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						bodytracking: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkBodyTrackingFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						bodytracking: true);
				}
			}
		}

		internal static void AddLipExpressionAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkLipExpressionFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						lipexpression: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkLipExpressionFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						lipexpression: true);
				}
			}
		}

		internal static void AddScenePerceptionAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkScenePerceptionFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						scenePerception: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkScenePerceptionFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						scenePerception: true);
				}
			}
		}

		internal static void AddSceneMeshAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkSceneMeshPermission(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						sceneMesh: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkSceneMeshPermission(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						sceneMesh: true);
				}
			}
		}

		internal static void AddMarkerAndroidManifest()
		{
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (File.Exists(AndroidManifestPathDest))
			{
				if (!checkMarkerFeature(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						marker: true);
				}
			}
			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				if (!checkMarkerFeature(WaveXRPath + CustomAndroidManifestPathSrc))
				{
					appendFile(
						filename: WaveXRPath + CustomAndroidManifestPathSrc,
						marker: true);
				}
			}
		}

		static void RemoveCreatedAndroidManifest()
		{
			bool isAndroidManifestCreatedByScript = File.Exists(AndroidManifestScriptCreatedPath);
			if (isAndroidManifestCreatedByScript)
			{
				File.Delete(AndroidManifestScriptCreatedPath);
				File.Delete(AndroidManifestScriptCreatedPath + ".meta");
				if (File.Exists(AndroidManifestPathDest))
				{
					File.Delete(AndroidManifestPathDest);
					File.Delete(AndroidManifestPathDest + ".meta");
				}
			}
		}

		static void RemoveWaveAARs()
        {
#if UNITY_2022_2_OR_NEWER
			if (File.Exists(Aar2022PathDest))
				File.Delete(Aar2022PathDest);
			if (File.Exists(Aar2022PathDest + ".meta"))
				File.Delete(Aar2022PathDest + ".meta");
#else
			if (File.Exists(Aar2017PathDest))
				File.Delete(Aar2017PathDest);
			if (File.Exists(Aar2017PathDest + ".meta"))
				File.Delete(Aar2017PathDest + ".meta");
#endif
			AssetDatabase.Refresh();
		}

		static void RemoveGradles()
		{
#if !UNITY_2020_1_OR_NEWER
			bool isCustomGradle1CreatedPath = File.Exists(CustomGradle1CreatedPath);
			if (isCustomGradle1CreatedPath)
			{
				File.Delete(CustomGradle1CreatedPath);
				File.Delete(CustomGradle1CreatedPath + ".meta");
				if (File.Exists(CustomGradle1Dest))
					File.Delete(CustomGradle1Dest);
				if (File.Exists(CustomGradle1Dest + ".meta"))
					File.Delete(CustomGradle1Dest + ".meta");
			}
			bool isCustomGradle2CreatedPath = File.Exists(CustomGradle2CreatedPath);
			if (isCustomGradle2CreatedPath)
			{
				File.Delete(CustomGradle2CreatedPath);
				File.Delete(CustomGradle2CreatedPath + ".meta");
				if (File.Exists(CustomGradle2Dest))
					File.Delete(CustomGradle2Dest);
				if (File.Exists(CustomGradle2Dest + ".meta"))
					File.Delete(CustomGradle2Dest + ".meta");
			}
#endif
		}

		static void CopyAndroidManifest()
		{
			const string PluginAndroidPath = "Assets/Plugins/Android";
			WaveXRSettings settings;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
				WaveXRPath = settings.waveXRFolder;

			if (!Directory.Exists(PluginAndroidPath))
				Directory.CreateDirectory(PluginAndroidPath);
			bool isAndroidManifestPathDestExisted = File.Exists(AndroidManifestPathDest);
			if (isAndroidManifestPathDestExisted)
			{
				Debug.Log("Using the Android Manifest at Assets/Plugins/Android");
				if (settings != null && settings.supportedFPS != WaveXRSettings.SupportedFPS.HMD_Default && !checkDefSupportedFPS(AndroidManifestPathDest))
				{
					appendFile(
						filename: AndroidManifestPathDest,
						supportedFPS: settings.supportedFPS);
				}
				return; // not to overwrite existed AndroidManifest.xml
			}

			// Create the dummy file
			if (!isAndroidManifestPathDestExisted)
				File.Create(AndroidManifestScriptCreatedPath).Dispose();

			if (File.Exists(WaveXRPath + CustomAndroidManifestPathSrc))
			{
				Debug.Log("Using the Android Manifest at " + WaveXRPath + "/Platform/Android");
				File.Copy(WaveXRPath + CustomAndroidManifestPathSrc, AndroidManifestPathDest, false);
			}
			else if (File.Exists(AndroidManifestPathSrc))
			{
				Debug.Log("Using the Android Manifest at Packages/com.htc.upm.wave.xrsdk/Runtime/Android");
				File.Copy(AndroidManifestPathSrc, AndroidManifestPathDest, false);
			}

			bool addHandTracking  = false;
			bool addTracker	      = false;
			bool addEyeTracking	  = false;
			bool addBodyTracking = false;
			bool addLipExpression = false;
			bool addScenePerception = false;
			bool addSceneMesh = false;
			bool addMarker = false;
			bool addSimultaneousInteraction = (EditorPrefs.GetBool(CheckIfSimultaneousInteractionEnabled.MENU_NAME, false) && !checkSimultaneousInteractionFeature(AndroidManifestPathDest));

			if (settings != null)
			{
				addHandTracking = (settings.EnableNaturalHand || settings.EnableElectronicHand) && !checkHandtrackingFeature(AndroidManifestPathDest);
				addTracker = settings.EnableTracker && !checkTrackerFeature(AndroidManifestPathDest);
				addEyeTracking = settings.EnableEyeTracking && !checkEyeTrackingFeature(AndroidManifestPathDest);
				addBodyTracking = settings.EnableBodyTracking && !checkBodyTrackingFeature(AndroidManifestPathDest);
				addLipExpression = settings.EnableLipExpression && !checkLipExpressionFeature(AndroidManifestPathDest);
				addScenePerception = settings.EnableScenePerception && !checkScenePerceptionFeature(AndroidManifestPathDest);
				addMarker = settings.EnableMarker && !checkMarkerFeature(AndroidManifestPathDest);
				addSceneMesh = settings.EnableSceneMesh && !checkSceneMeshPermission(AndroidManifestPathDest);

				appendFile(
					filename: AndroidManifestPathDest,
					handtracking: addHandTracking,
					settings.supportedFPS,
					tracker: addTracker,
					simultaneous_interaction: addSimultaneousInteraction,
					eyetracking: addEyeTracking,
					bodytracking: addBodyTracking,
					lipexpression: addLipExpression,
					scenePerception: addScenePerception,
					sceneMesh: addSceneMesh,
					marker: addMarker);
			}
			else
			{
				appendFile(
					filename: AndroidManifestPathDest,
					handtracking: addHandTracking,
					tracker: addTracker,
					simultaneous_interaction: addSimultaneousInteraction,
					eyetracking: addEyeTracking,
					bodytracking: addBodyTracking,
					lipexpression: addLipExpression,
					scenePerception: addScenePerception,
					sceneMesh: addSceneMesh,
					marker: addMarker);
			}
		}

		static void CopyWaveAARs()
        {
#if UNITY_2022_2_OR_NEWER
			if (!File.Exists(Aar2022PathDest))
				File.Copy(Aar2022PathSrc, Aar2022PathDest, false);
			if (File.Exists(Aar2017PathDest))
				File.Delete(Aar2017PathDest);
			if (File.Exists(Aar2017PathDest + ".meta"))
				File.Delete(Aar2017PathDest + ".meta");
			PluginImporter plugin = AssetImporter.GetAtPath(Aar2022PathDest) as PluginImporter;
			if (plugin != null)
				plugin.SetCompatibleWithPlatform(BuildTarget.Android, true);
			AssetDatabase.Refresh();
#else
			if (!File.Exists(Aar2017PathDest))
				File.Copy(Aar2017PathSrc, Aar2017PathDest, false);
			if (File.Exists(Aar2022PathDest))
				File.Delete(Aar2022PathDest);
			if (File.Exists(Aar2022PathDest + ".meta"))
				File.Delete(Aar2022PathDest + ".meta");
			PluginImporter plugin = AssetImporter.GetAtPath(Aar2017PathDest) as PluginImporter;
			if (plugin != null)
				plugin.SetCompatibleWithPlatform(BuildTarget.Android, true);
			AssetDatabase.Refresh();
#endif
		}

		static void CopyGradles()
		{
#if !UNITY_2020_1_OR_NEWER
			if (!File.Exists(CustomGradle1Dest))
			{
				File.Create(CustomGradle1CreatedPath).Dispose();
				File.Copy(CustomGradle1Src, CustomGradle1Dest, false);
			}
			if (!File.Exists(CustomGradle2Dest))
			{
				File.Create(CustomGradle2CreatedPath).Dispose();
				File.Copy(CustomGradle2Src, CustomGradle2Dest, false);
			}
#endif
		}

		static void appendFile(string filename
			, bool handtracking = false
			, WaveXRSettings.SupportedFPS supportedFPS = WaveXRSettings.SupportedFPS.HMD_Default
			, bool tracker = false
			, bool simultaneous_interaction = false
			, bool eyetracking = false
			, bool bodytracking = false
			, bool lipexpression = false
			, bool scenePerception = false
			, bool sceneMesh = false
			, bool marker = false)
		{
			string line;

			// Read the file and display it line by line.  
			StreamReader file1 = new StreamReader(filename);
			StreamWriter file2 = new StreamWriter(filename + ".tmp");
			bool appendFPS120 = supportedFPS == WaveXRSettings.SupportedFPS._120;
			while ((line = file1.ReadLine()) != null)
			{
				if (line.Contains("</application>") && appendFPS120)
				{
					file2.WriteLine("		<meta-data android:name=\"com.htc.vr.content.SupportedFPS\" android:value=\"120\" />");
				}
				if (line.Contains("</manifest>") && handtracking)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.handtracking\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && tracker)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.tracker\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && simultaneous_interaction)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.simultaneous_interaction\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && eyetracking)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.eyetracking\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && bodytracking)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.bodytracking\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && lipexpression)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.lipexpression\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && scenePerception)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.sceneperception\" android:required=\"true\" />");
				}
				if (line.Contains("</manifest>") && sceneMesh)
				{
					file2.WriteLine("	<uses-permission android:name=\"wave.permission.GET_SCENE_MESH\" />");
				}
				if (line.Contains("</manifest>") && marker)
				{
					file2.WriteLine("	<uses-feature android:name=\"wave.feature.marker\" android:required=\"true\" />");
				}
				file2.WriteLine(line);
			}

			file1.Close();
			file2.Close();
			File.Delete(filename);
			File.Move(filename + ".tmp", filename);
		}

		static bool checkHandtrackingFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.handtracking"))
						return true;
				}
			}
			return false;
		}

		static bool checkSimultaneousInteractionFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.simultaneous_interaction"))
						return true;
				}
			}
			return false;
		}

		static bool checkEyeTrackingFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.eyetracking"))
						return true;
				}
			}
			return false;
		}

		static bool checkBodyTrackingFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.bodytracking"))
						return true;
				}
			}
			return false;
		}

		static bool checkTrackerFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.tracker"))
						return true;
				}
			}
			return false;
		}

		static bool checkLipExpressionFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.lipexpression"))
						return true;
				}
			}
			return false;
		}

		static bool checkScenePerceptionFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.sceneperception"))
						return true;
				}
			}
			return false;
		}

		static bool checkSceneMeshPermission(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-permission");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;

					if (name != null && name.Equals("wave.permission.GET_SCENE_MESH"))
						return true;
				}
			}
			return false;
		}

		static bool checkMarkerFeature(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/uses-feature");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;
					string required = metadataNode.Attributes["android:required"].Value;

					if (name != null && name.Equals("wave.feature.marker"))
						return true;
				}
			}
			return false;
		}

		static bool checkDefSupportedFPS(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/application/meta-data");

			if (metadataNodeList != null)
			{
				foreach (XmlNode metadataNode in metadataNodeList)
				{
					string name = metadataNode.Attributes["android:name"].Value;

					if (name != null && name.Equals("com.htc.vr.content.SupportedFPS"))
						return true;
				}
			}
			return false;
		}

		static void DelAndroidManifest()
		{
			if (File.Exists(AndroidManifestPathDest))
				File.Delete(AndroidManifestPathDest);

			string AndroidManifestMetaPathDest = AndroidManifestPathDest + ".meta";
			if (File.Exists(AndroidManifestMetaPathDest))
				File.Delete(AndroidManifestMetaPathDest);
		}

		static bool SetBuildingWave()
		{
			var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
			var androidXRSettings = androidGenericSettings.AssignedSettings;
			
			if (androidXRSettings == null)
			{
				androidXRSettings = ScriptableObject.CreateInstance<XRManagerSettings>() as XRManagerSettings;
			}
			var didAssign = XRPackageMetadataStore.AssignLoader(androidXRSettings, "Wave.XR.Loader.WaveXRLoader", BuildTargetGroup.Android);
			if (!didAssign)
			{
				Debug.LogError("Fail to add android WaveXRLoader.");
			}
			return didAssign;
		}

		static bool CheckIsBuildingVR()
        {
			var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
			if (androidGenericSettings == null)
				return false;

			var androidXRMSettings = androidGenericSettings.AssignedSettings;
			if (androidXRMSettings == null)
				return false;

#pragma warning disable
            var loaders = androidXRMSettings.loaders;
#pragma warning enable
            return loaders.Count > 0;
		}

		static bool CheckIsBuildingWave()
		{
			var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
			if (androidGenericSettings == null)
				return false;

			var androidXRMSettings = androidGenericSettings.AssignedSettings;
			if (androidXRMSettings == null)
				return false;

#pragma warning disable
			var loaders = androidXRMSettings.loaders;
#pragma warning enable
			foreach (var loader in loaders)
			{
				if (loader.GetType() == typeof(WaveXRLoader))
				{
					return true;
				}
			}
			return false;
		}

		// Copy from XR.Management.XRGeneralBuildProcess
		internal class BootConfig
		{
			static readonly string kXrBootSettingsKey = "xr-boot-settings";
			Dictionary<string, string> bootConfigSettings;

			BuildReport buildReport;
			string bootConfigPath;

			internal BootConfig(BuildReport report)
			{
				buildReport = report;
			}

			internal void ReadBootConfig()
			{
				bootConfigSettings = new Dictionary<string, string>();

				string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
				string xrBootSettings = UnityEditor.EditorUserBuildSettings.GetPlatformSettings(buildTargetName, kXrBootSettingsKey);
				if (!String.IsNullOrEmpty(xrBootSettings))
				{
					// boot settings string format
					// <boot setting>:<value>[;<boot setting>:<value>]*
					var bootSettings = xrBootSettings.Split(';');
					foreach (var bootSetting in bootSettings)
					{
						var setting = bootSetting.Split(':');
						if (setting.Length == 2 && !String.IsNullOrEmpty(setting[0]) && !String.IsNullOrEmpty(setting[1]))
						{
							bootConfigSettings.Add(setting[0], setting[1]);
						}
					}
				}
			}

			internal void SetValueForKey(string key, string value, bool replace = false)
			{
				if (bootConfigSettings.ContainsKey(key))
				{
					bootConfigSettings[key] = value;
				}
				else
				{
					bootConfigSettings.Add(key, value);
				}
			}

			internal bool GetValueForKey(string key, out string value)
			{
				if (bootConfigSettings.ContainsKey(key))
				{
					value = bootConfigSettings[key];
					return true;
				}
				value = "";
				return false;
			}

			internal void WriteBootConfig()
			{
				// boot settings string format
				// <boot setting>:<value>[;<boot setting>:<value>]*
				bool firstEntry = true;
				var sb = new System.Text.StringBuilder();
				foreach (var kvp in bootConfigSettings)
				{
					if (!firstEntry)
					{
						sb.Append(";");
					}
					sb.Append($"{kvp.Key}:{kvp.Value}");
					firstEntry = false;
				}

				string buildTargetName = BuildPipeline.GetBuildTargetName(buildReport.summary.platform);
				EditorUserBuildSettings.SetPlatformSettings(buildTargetName, kXrBootSettingsKey, sb.ToString());
			}
		}

		private class CustomPreprocessor : IPreprocessBuildWithReport
        {
            public int callbackOrder { get { return 0; } }

			public void OnPreprocessBuild(BuildReport report)
			{
				// Remove any script created files before build
				{
					RemoveCreatedAndroidManifest();
					RemoveGradles();
					RemoveWaveAARs();
				}

				if (report.summary.platform == BuildTarget.Android && !CheckIsBuildingVR())
				{
					Debug.Log("WaveXRBuildCheck: Not building WaveXR.");
					// if we want to build non-VR Android apk, we need clean the pre-init library because the XR.Management didn't clean it.
					string key = "xrsdk-pre-init-library";
					BootConfig bootConfig = new BootConfig(report);
					bootConfig.ReadBootConfig();
					if (bootConfig.GetValueForKey(key, out string value) && value == "wvrunityxr")
					{
						Debug.Log("WaveXRBuildCheck: Clean Wave's PreInitLibrary.");
						bootConfig.SetValueForKey(key, "");
						bootConfig.WriteBootConfig();
					}
					return;
				}

				if (File.Exists(ForceBuildWVR))
				{
					//SetBuildingWave();
					AddHandTrackingAndroidManifest();
					AddTrackerAndroidManifest();
					AddEyeTrackingAndroidManifest();
					AddLipExpressionAndroidManifest();
					AddScenePerceptionAndroidManifest();
					AddSceneMeshAndroidManifest();
					AddMarkerAndroidManifest();
					CopyAndroidManifest();
					CopyGradles();
					CopyWaveAARs();
					return;
				}
				else if (report.summary.platform == BuildTarget.Android && CheckIsBuildingWave())
				{
					CopyAndroidManifest();
					CopyGradles();
					CopyWaveAARs();
					return;
				}
			}
		}

        private class CustomPostprocessor : IPostprocessBuildWithReport
        {
            public int callbackOrder { get { return 0; } }

            public void OnPostprocessBuild(BuildReport report)
            {
				RemoveCreatedAndroidManifest();
				RemoveGradles();
				RemoveWaveAARs();
			}
        }
	}

	[Obsolete("CheckIfHandTrackingEnabled is deprecated. Please use WaveXRSettings EnableNaturalHand instead", false)]
	public static class CheckIfHandTrackingEnabled
	{
		internal const string MENU_NAME = "Wave/Hand Tracking/Enable Hand Tracking";

		private static bool enabled_ = false;
		static CheckIfHandTrackingEnabled()
		{
			WaveXRSettings settings = null;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
			{
				CheckIfHandTrackingEnabled.enabled_ = settings.EnableNaturalHand;
			}

			EditorPrefs.SetBool(CheckIfHandTrackingEnabled.MENU_NAME, CheckIfHandTrackingEnabled.enabled_);
		}

		[Obsolete("CheckIfHandTrackingEnabled is obsolete. Please use WaveXRSettings EnableNaturalHand instead", false)]
		public static void PerformAction(bool enabled)
		{
			WaveXRSettings settings = null;
			EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
			if (settings != null)
			{
				settings.EnableNaturalHand = enabled;
			}
			if (enabled)
				CustomBuildProcessor.AddHandTrackingAndroidManifest();
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfHandTrackingEnabled.MENU_NAME, enabled);

			CheckIfHandTrackingEnabled.enabled_ = enabled;
		}

		[Obsolete("CheckIfHandTrackingEnabled is obsolete. Please use WaveXRSettings EnableNaturalHand instead", false)]
		public static bool ValidateEnabled()
		{
			return true;
		}
	}

	[InitializeOnLoad]
	public static class CheckIfWaveEnabled
	{
		const string LOG_TAG = "Wave.XR.CheckIfWaveEnabled";
		static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
		const string VERSION_DEFINE_WAVE_XR = "USE_VIVE_WAVE_XR_5_3_1";
		internal struct ScriptingDefinedSettings
		{
			public string[] scriptingDefinedSymbols;
			public BuildTargetGroup[] targetGroups;

			public ScriptingDefinedSettings(string[] symbols, BuildTargetGroup[] groups)
			{
				scriptingDefinedSymbols = symbols;
				targetGroups = groups;
			}
		}
		static readonly ScriptingDefinedSettings m_ScriptDefineSettingWaveXR = new ScriptingDefinedSettings(
			new string[] { VERSION_DEFINE_WAVE_XR, },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);
		const string XR_LOADER_WAVE_XR_NAME = "Wave.XR.Loader.WaveXRLoader";
		internal static bool ViveWaveXRAndroidAssigned {
			get {
				var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
				if (androidGenericSettings == null)
					return false;

				var androidXRMSettings = androidGenericSettings.AssignedSettings;
				if (androidXRMSettings == null)
					return false;

#pragma warning disable
				var loaders = androidXRMSettings.loaders;
#pragma warning enable
				foreach (var loader in loaders)
				{
					if (loader.GetType() == typeof(WaveXRLoader))
					{
						return true;
					}
				}
				return false;
			}
		}
		static void AddScriptingDefineSymbols(ScriptingDefinedSettings setting)
		{
			for (int group_index = 0; group_index < setting.targetGroups.Length; group_index++)
			{
				var group = setting.targetGroups[group_index];
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
				List<string> allDefines = definesString.Split(';').ToList();
				for (int symbol_index = 0; symbol_index < setting.scriptingDefinedSymbols.Length; symbol_index++)
				{
					if (!allDefines.Contains(setting.scriptingDefinedSymbols[symbol_index]))
					{
						DEBUG("AddDefineSymbols() " + setting.scriptingDefinedSymbols[symbol_index] + " to group " + group);
						allDefines.Add(setting.scriptingDefinedSymbols[symbol_index]);
					}
					else
					{
						DEBUG("AddDefineSymbols() " + setting.scriptingDefinedSymbols[symbol_index] + " already existed.");
					}
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					group,
					string.Join(";", allDefines.ToArray())
				);
			}
		}
		static void RemoveScriptingDefineSymbols(ScriptingDefinedSettings setting)
		{
			for (int group_index = 0; group_index < setting.targetGroups.Length; group_index++)
			{
				var group = setting.targetGroups[group_index];
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
				List<string> allDefines = definesString.Split(';').ToList();
				for (int symbol_index = 0; symbol_index < setting.scriptingDefinedSymbols.Length; symbol_index++)
				{
					if (allDefines.Contains(setting.scriptingDefinedSymbols[symbol_index]))
					{
						DEBUG("RemoveDefineSymbols() " + setting.scriptingDefinedSymbols[symbol_index] + " from group " + group);
						allDefines.Remove(setting.scriptingDefinedSymbols[symbol_index]);
					}
					else
					{
						DEBUG("RemoveDefineSymbols() " + setting.scriptingDefinedSymbols[symbol_index] + " already existed.");
					}
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					group,
					string.Join(";", allDefines.ToArray())
				);
			}
		}
		static bool HasScriptingDefineSymbols(ScriptingDefinedSettings setting)
		{
			for (int group_index = 0; group_index < setting.targetGroups.Length; group_index++)
			{
				var group = setting.targetGroups[group_index];
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
				List<string> allDefines = definesString.Split(';').ToList();
				for (int symbol_index = 0; symbol_index < setting.scriptingDefinedSymbols.Length; symbol_index++)
				{
					if (!allDefines.Contains(setting.scriptingDefinedSymbols[symbol_index]))
					{
						return false;
					}
				}
			}

			return true;
		}
		static void OnUpdate()
		{
			// Adds the script symbol if Vive Wave XR Plugin is imported and assigned in XR Plugin-in Management.
			if (ViveWaveXRAndroidAssigned)
			{
				if (!HasScriptingDefineSymbols(m_ScriptDefineSettingWaveXR))
				{
					DEBUG("OnUpdate() Adds m_ScriptDefineSettingWaveXR.");
					AddScriptingDefineSymbols(m_ScriptDefineSettingWaveXR);
				}
			}
			// Removes the script symbol if Vive Wave XR Plugin is uninstalled.
			else
			{
				if (HasScriptingDefineSymbols(m_ScriptDefineSettingWaveXR))
				{
					DEBUG("OnUpdate() Removes m_ScriptDefineSettingWaveXR.");
					RemoveScriptingDefineSymbols(m_ScriptDefineSettingWaveXR);
				}
			}
		}
		static CheckIfWaveEnabled()
		{
			EditorApplication.update += OnUpdate;
		}
	}

	[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableTracker instead.", false)]
	public static class CheckIfTrackerEnabled
	{
		internal const string MENU_NAME = "Wave/Tracker/Enable Tracker";

		private static bool enabled_ = false;
		static CheckIfTrackerEnabled()
		{
			CheckIfTrackerEnabled.enabled_ = EditorPrefs.GetBool(CheckIfTrackerEnabled.MENU_NAME, false);

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfTrackerEnabled.enabled_);
			};
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableTracker instead.", false)]
		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfTrackerEnabled.MENU_NAME, enabled);
			if (enabled)
				CustomBuildProcessor.AddTrackerAndroidManifest();
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfTrackerEnabled.MENU_NAME, enabled);

			CheckIfTrackerEnabled.enabled_ = enabled;
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableTracker instead.", false)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfTrackerEnabled.MENU_NAME, enabled_);
			return true;
		}
	}

	[InitializeOnLoad]
	public static class CheckIfSimultaneousInteractionEnabled
	{
		internal const string MENU_NAME = "Wave/Interaction Mode/Enable Simultaneous Interaction";

		private static bool enabled_;
		static CheckIfSimultaneousInteractionEnabled()
		{
			CheckIfSimultaneousInteractionEnabled.enabled_ = EditorPrefs.GetBool(CheckIfSimultaneousInteractionEnabled.MENU_NAME, false);

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfSimultaneousInteractionEnabled.enabled_);
			};
		}

		[MenuItem(CheckIfSimultaneousInteractionEnabled.MENU_NAME, priority = 603)]
		private static void ToggleAction()
		{
			/// Toggling action
			PerformAction(!CheckIfSimultaneousInteractionEnabled.enabled_);
		}

		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfSimultaneousInteractionEnabled.MENU_NAME, enabled);
			if (enabled)
				CustomBuildProcessor.AddSimultaneousInteractionAndroidManifest();
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfSimultaneousInteractionEnabled.MENU_NAME, enabled);

			CheckIfSimultaneousInteractionEnabled.enabled_ = enabled;
		}

		[MenuItem(CheckIfSimultaneousInteractionEnabled.MENU_NAME, validate = true, priority = 603)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfSimultaneousInteractionEnabled.MENU_NAME, enabled_);
			return true;
		}
	}

	[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableEyeTracking instead.", false)]
	public static class CheckIfEyeTrackingEnabled
	{
		internal const string MENU_NAME = "Wave/Eye/Enable Eye Tracking";

		private static bool enabled_ = false;
		static CheckIfEyeTrackingEnabled()
		{
			CheckIfEyeTrackingEnabled.enabled_ = EditorPrefs.GetBool(CheckIfEyeTrackingEnabled.MENU_NAME, false);

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfEyeTrackingEnabled.enabled_);
			};
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableEyeTracking instead.", false)]
		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfEyeTrackingEnabled.MENU_NAME, enabled);
			if (enabled)
				CustomBuildProcessor.AddEyeTrackingAndroidManifest();
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfEyeTrackingEnabled.MENU_NAME, enabled);

			CheckIfEyeTrackingEnabled.enabled_ = enabled;
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableEyeTracking instead.", false)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfEyeTrackingEnabled.MENU_NAME, enabled_);
			return true;
		}
	}

	[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableLipExpression instead.", false)]
	public static class CheckIfLipExpressionEnabled
	{
		internal const string MENU_NAME = "Wave/Lip/Enable Lip Expression";

		private static bool enabled_ = false;
		static CheckIfLipExpressionEnabled()
		{
			CheckIfLipExpressionEnabled.enabled_ = EditorPrefs.GetBool(CheckIfLipExpressionEnabled.MENU_NAME, false);

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfLipExpressionEnabled.enabled_);
			};
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableLipExpression instead.", false)]
		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfLipExpressionEnabled.MENU_NAME, enabled);
			if (enabled)
				CustomBuildProcessor.AddLipExpressionAndroidManifest();
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfLipExpressionEnabled.MENU_NAME, enabled);

			CheckIfLipExpressionEnabled.enabled_ = enabled;
		}

		[Obsolete("CheckIfTrackerEnabled is deprecated. Please use WaveXRSettings EnableLipExpression instead.", false)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfLipExpressionEnabled.MENU_NAME, enabled_);
			return true;
		}
	}
	/*
	[InitializeOnLoad]
	public static class MenuAvatarIKHandler
	{
		internal const string MENU_NAME = "Wave/Body Tracking/Enable Avatar IK";

		const string LOG_TAG = "Wave.XR.MenuAvatarIKHandler ";
		static StringBuilder m_sb = null;
		static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg)
		{
			msg.Insert(0, LOG_TAG);
			Debug.Log(msg);
		}

		#region Scripting Symbol
		const string DEFINE_AVATAR_IK = "WAVE_AVATAR_IK";
		internal struct ScriptingDefinedSettings
		{
			public string[] scriptingDefinedSymbols;
			public BuildTargetGroup[] targetGroups;

			public ScriptingDefinedSettings(string[] symbols, BuildTargetGroup[] groups)
			{
				scriptingDefinedSymbols = symbols;
				targetGroups = groups;
			}
		}
		static readonly ScriptingDefinedSettings m_SettingAvatarIK = new ScriptingDefinedSettings(
			new string[] { DEFINE_AVATAR_IK, },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);
		static void AddScriptingDefineSymbols(ScriptingDefinedSettings setting)
		{
			for (int group_index = 0; group_index < setting.targetGroups.Length; group_index++)
			{
				var group = setting.targetGroups[group_index];
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
				List<string> allDefines = definesString.Split(';').ToList();
				for (int symbol_index = 0; symbol_index < setting.scriptingDefinedSymbols.Length; symbol_index++)
				{
					if (!allDefines.Contains(setting.scriptingDefinedSymbols[symbol_index]))
					{
						sb.Clear(); sb.Append("AddScriptingDefineSymbols() ").Append(setting.scriptingDefinedSymbols[symbol_index]).Append(" to group ").Append(group);
						DEBUG(sb);
						allDefines.Add(setting.scriptingDefinedSymbols[symbol_index]);
					}
					else
					{
						sb.Clear(); sb.Append("AddScriptingDefineSymbols() ").Append(setting.scriptingDefinedSymbols[symbol_index]).Append(" already existed.");
						DEBUG(sb);
					}
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					group,
					string.Join(";", allDefines.ToArray())
				);
			}
		}
		static void RemoveScriptingDefineSymbols(ScriptingDefinedSettings setting)
		{
			for (int group_index = 0; group_index < setting.targetGroups.Length; group_index++)
			{
				var group = setting.targetGroups[group_index];
				string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
				List<string> allDefines = definesString.Split(';').ToList();
				for (int symbol_index = 0; symbol_index < setting.scriptingDefinedSymbols.Length; symbol_index++)
				{
					if (allDefines.Contains(setting.scriptingDefinedSymbols[symbol_index]))
					{
						sb.Clear(); sb.Append("RemoveScriptingDefineSymbols() ").Append(setting.scriptingDefinedSymbols[symbol_index]).Append(" from group ").Append(group);
						DEBUG(sb);
						allDefines.Remove(setting.scriptingDefinedSymbols[symbol_index]);
					}
					else
					{
						sb.Clear(); sb.Append("RemoveScriptingDefineSymbols() ").Append(setting.scriptingDefinedSymbols[symbol_index]).Append(" already existed.");
						DEBUG(sb);
					}
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup(
					group,
					string.Join(";", allDefines.ToArray())
				);
			}
		}
		#endregion

		private static bool m_Enabled = false;
		static MenuAvatarIKHandler()
		{
			MenuAvatarIKHandler.m_Enabled = EditorPrefs.GetBool(MenuAvatarIKHandler.MENU_NAME, false);

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(MenuAvatarIKHandler.m_Enabled);
			};
		}

		[MenuItem(MenuAvatarIKHandler.MENU_NAME, priority = 604)]
		private static void ToggleAction()
		{
			/// Toggling action
			PerformAction(!MenuAvatarIKHandler.m_Enabled);
		}

		private static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(MenuAvatarIKHandler.MENU_NAME, enabled);
			if (enabled)
			{
				AddScriptingDefineSymbols(m_SettingAvatarIK);
			}
			else
			{
				RemoveScriptingDefineSymbols(m_SettingAvatarIK);
			}
			/// Saving editor state
			EditorPrefs.SetBool(MenuAvatarIKHandler.MENU_NAME, enabled);

			MenuAvatarIKHandler.m_Enabled = enabled;
		}

		[MenuItem(MenuAvatarIKHandler.MENU_NAME, validate = true, priority = 604)]
		private static bool ValidateEnabled()
		{
			Menu.SetChecked(MenuAvatarIKHandler.MENU_NAME, m_Enabled);
			return true;
		}
	}
	*/
}
#endif
