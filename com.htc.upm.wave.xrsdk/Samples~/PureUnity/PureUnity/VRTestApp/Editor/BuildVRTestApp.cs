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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Wave.Generic.Sample.Editor
{
	public class BuildVRTestApp
	{
		static BuildVRTestApp()
		{
			//isBuildAndRun = EditorPrefs.GetBool(BuildAndRunKey, true);
			//Menu.SetChecked(BuildAndRunMenuPath, isBuildAndRun);

			isBuildOnlyScript = EditorPrefs.GetBool(BuildOnlyScriptKey, false);
			Menu.SetChecked(BuildOnlyScriptMenuPath, isBuildOnlyScript);

			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);
			EditorPrefs.SetInt(BuildConfigurationKey, (int)buildConfiguration);

			Menu.SetChecked(BuildForVerificationMenuPath, (buildConfiguration & SceneBuildConfiguration.Verification) != 0);
			Menu.SetChecked(BuildForReleaseMenuPath, (buildConfiguration & SceneBuildConfiguration.Release) != 0);
			Menu.SetChecked(BuildForDevelopmentMenuPath, (buildConfiguration & SceneBuildConfiguration.Development) != 0);

			EditorApplication.delayCall += DelayCall;
		}

		// This delay call isn't always work.  Use validate function for sure.
		public static void DelayCall()
		{
			//Menu.SetChecked(BuildAndRunMenuPath, isBuildAndRun);
			isBuildOnlyScript = EditorPrefs.GetBool(BuildOnlyScriptKey, false);
			Menu.SetChecked(BuildOnlyScriptMenuPath, isBuildOnlyScript);

			UpdateBuildConfiguration();
		}

		private static string _destinationPath;
		private static void CustomizedCommandLine()
		{
			Dictionary<string, Action<string>> cmdActions = new Dictionary<string, Action<string>>
			{
				{
					"-destinationPath", delegate(string argument)
					{
						_destinationPath = argument;
					}
				}
			};

			Action<string> actionCache;
			string[] cmdArguments = Environment.GetCommandLineArgs();

			for (int count = 0; count < cmdArguments.Length; count++)
			{
				if (cmdActions.ContainsKey(cmdArguments[count]))
				{
					actionCache = cmdActions[cmdArguments[count]];
					actionCache(cmdArguments[count + 1]);
				}
			}

			if (string.IsNullOrEmpty(_destinationPath))
			{
				_destinationPath = Path.GetDirectoryName(Application.dataPath);
			}
		}

		private static void GeneralSettings()
		{
			PlayerSettings.Android.bundleVersionCode = 1;
			PlayerSettings.bundleVersion = "2.0.0";
			PlayerSettings.companyName = "HTC Corp.";
			PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
			PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
		}

		private static void GeneralSettingsForWindows()
		{
			PlayerSettings.bundleVersion = "2.0.0";
			PlayerSettings.companyName = "HTC Corp.";
			PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		}

		private static string SearchScenePath(string sceneName)
		{
			sceneName = Path.GetFileNameWithoutExtension(sceneName);
			var list = AssetDatabase.FindAssets(sceneName + " t:scene");
			foreach (var sceneGUID in list)
			{
				var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
				if (Path.GetFileNameWithoutExtension(scenePath) == sceneName)
					return scenePath;
			}
			return default;
		}

		private static string VRTestAppBuildScenesAssetName = "VRTestAppBuildScenes.asset";

		private static string GetBuildAssetsPath()
		{
			var VRTestAppPath = SearchScenePath("VRTestApp");
			if (string.IsNullOrEmpty(VRTestAppPath))
			{
				Debug.LogError("Fail to find VRTestApp scene");
				return default;
			}

			var VRTestAppDir = Path.GetDirectoryName(VRTestAppPath);
			return Path.Combine(VRTestAppDir, VRTestAppBuildScenesAssetName);
		}

		private static VRTestAppScenes CreateBuildAssets(string VRTestAppBuildSceneAssetPath)
		{
			var scenesAsset = ScriptableObject.CreateInstance(typeof(VRTestAppScenes)) as VRTestAppScenes;
			CheckSceneList(VRTestAppScenes.GetPredefinedScenesData(), ref scenesAsset.pathes, ref scenesAsset.scenesData);
			AssetDatabase.CreateAsset(scenesAsset, VRTestAppBuildSceneAssetPath);
			AssetDatabase.SaveAssets();
			return scenesAsset;
		}

		private static VRTestAppScenes scenesData = null;
		public static VRTestAppScenes ScenesData
		{
			get
			{
				if (scenesData == null)
				{
					var VRTestAppBuildSceneAssetPath = GetBuildAssetsPath();
					if (!File.Exists(VRTestAppBuildSceneAssetPath))
						scenesData = CreateBuildAssets(VRTestAppBuildSceneAssetPath);
					else
						scenesData = AssetDatabase.LoadAssetAtPath(VRTestAppBuildSceneAssetPath, typeof(VRTestAppScenes)) as VRTestAppScenes;
				}
				return scenesData;
			}
		}

		private static void UpdateBuildConfiguration()
		{
			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

			bool isVerfication = (buildConfiguration & SceneBuildConfiguration.Verification) != 0;
			bool isRelease = (buildConfiguration & SceneBuildConfiguration.Release) != 0;
			bool isDevelopment = (buildConfiguration & SceneBuildConfiguration.Development) != 0;

			Menu.SetChecked(BuildForVerificationMenuPath, isVerfication);
			Menu.SetChecked(BuildForReleaseMenuPath, isRelease);
			Menu.SetChecked(BuildForDevelopmentMenuPath, isDevelopment);
		}

		private static void UpdateDefineSymbols()
		{
			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

			bool isVerfication = (buildConfiguration & SceneBuildConfiguration.Verification) != 0;

			string originalDefineSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
			string defineSymbolString = originalDefineSymbolString;

			if (isVerfication)
				defineSymbolString = AddPlayerSettingsDefineSymbol(defineSymbolString, BuildForVerificationDefine);
			else
				defineSymbolString = RemovePlayerSettingsDefineSymbol(defineSymbolString, BuildForVerificationDefine);

			bool isRelease = (buildConfiguration & SceneBuildConfiguration.Release) != 0;

			if (isRelease)
				defineSymbolString = AddPlayerSettingsDefineSymbol(defineSymbolString, BuildForReleaseDefine);
			else
				defineSymbolString = RemovePlayerSettingsDefineSymbol(defineSymbolString, BuildForReleaseDefine);

			bool isDevelopment = (buildConfiguration & SceneBuildConfiguration.Development) != 0;

			if (isDevelopment)
				defineSymbolString = AddPlayerSettingsDefineSymbol(defineSymbolString, BuildForDevelopmentDefine);
			else
				defineSymbolString = RemovePlayerSettingsDefineSymbol(defineSymbolString, BuildForDevelopmentDefine);

			if (defineSymbolString != originalDefineSymbolString)
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbolString);
		}

		private static void CleanPrebuildAssets()
		{
			UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
			List<UnityEngine.Object> assets;
			if (preloadedAssets == null)
				return;
			assets = preloadedAssets.ToList();

			var oldSettings = from s in preloadedAssets
							  where s != null && s.GetType() == typeof(VRTestAppScenes)
							  select s;

			if (oldSettings != null && oldSettings.Any())
			{
				foreach (var s in oldSettings)
				{
					assets.Remove(s);
				}
			}
			PlayerSettings.SetPreloadedAssets(assets.ToArray());
		}

		//[MenuItem("Wave/Build VRTestApp/Update VRTestApp Build Asset", false, priority = 541)]
		private static void ResetPrebuildAssets()
		{
			scenesData = null;
			CleanPrebuildAssets();
			var VRTestAppBuildSceneAssetPath = GetBuildAssetsPath();
			if (File.Exists(VRTestAppBuildSceneAssetPath))
				AssetDatabase.DeleteAsset(VRTestAppBuildSceneAssetPath);
			if (ScenesData == null)
				Debug.LogError("Fail to create VRTestApp Build Asset:" + VRTestAppBuildSceneAssetPath);
		}

		private static void SetPrebuildAssets()
		{
			UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
			List<UnityEngine.Object> assets;
			if (preloadedAssets == null)
				assets = new List<UnityEngine.Object>();
			else
				assets = preloadedAssets.ToList();

			var oldSettings = from s in preloadedAssets
							  where s != null && s.GetType() == typeof(VRTestAppScenes)
							  select s;

			if (oldSettings != null && oldSettings.Any())
			{
				foreach (var s in oldSettings)
				{
					assets.Remove(s);
				}
			}
			assets.Add(ScenesData);
			PlayerSettings.SetPreloadedAssets(assets.ToArray());
		}

		private static void CheckSceneList(List<SceneData> scenes, ref List<string> checkedPathes, ref List<SceneData> checkedScenesData)
		{
			foreach (var sceneData in scenes)
			{
				string titleName = sceneData.name;
				bool onPC = sceneData.onPC;
				bool isEntry = sceneData.isEntry;
				bool hasHelp = sceneData.hasHelp;

				var buildConfig = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

				bool isVerification = (buildConfig & SceneBuildConfiguration.Verification) != 0;
				bool isRelease = (buildConfig & SceneBuildConfiguration.Release) != 0;
				bool isDevelopment = (buildConfig & SceneBuildConfiguration.Development) != 0;

				string path = SearchScenePath(sceneData.path);
				if (!File.Exists(path))
				{
					Debug.LogWarning("Drop lost scene: " + sceneData.path);
					continue;
				}

				bool sceneIsVerification = (sceneData.buildConfig & SceneBuildConfiguration.Verification) != 0;
				bool sceneIsRelease = (sceneData.buildConfig & SceneBuildConfiguration.Release) != 0;
				bool sceneIsDevelopment = (sceneData.buildConfig & SceneBuildConfiguration.Development) != 0;
				bool sceneIsAlways = (sceneData.buildConfig & SceneBuildConfiguration.AlwaysBuild) != 0;
				bool canBuild =
					   (sceneIsVerification && isVerification)
					|| (sceneIsRelease && isRelease)
					|| (sceneIsDevelopment && isDevelopment)
					|| sceneIsAlways;

				if (!canBuild)
				{
					Debug.LogWarning("Drop scene for not matching configuration.");
					continue;
				}

				checkedPathes.Add(path);

				if (sceneData.hasHelp)
				{
					string helpPath = path.Remove(path.Length - 6);
					helpPath += "_Help.unity";
					if (!File.Exists(helpPath))
					{
						Debug.LogWarning("Drop lost scene: " + sceneData.path + "_Help");
						hasHelp = false;
					}
					else
					{
						checkedPathes.Add(helpPath);
					}
				}

				checkedScenesData.Add(new SceneData(isEntry, titleName, path, hasHelp, onPC));
			}
		}

		static void ApplyVRTestAppPlayerSettingsForWindows()
		{
			Debug.Log("Apply VRTestApp's PlayerSettings");

			GeneralSettingsForWindows();

			PlayerSettings.productName = "VRTestApp";

			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.vrm.unity.VRTestApp");

			PlayerSettings.gpuSkinning = false;
			PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Standalone, true);
			PlayerSettings.graphicsJobs = true;

			// This can help check the Settings by text editor
			EditorSettings.serializationMode = SerializationMode.ForceText;
			AssetDatabase.SaveAssets();
		}

		static void BuildApkAll()
		{
			CustomizedCommandLine();
			BuildApk(_destinationPath + "/", false, false, true, true, true);
			BuildApk(_destinationPath + "/armv7", false, false);
			//BuildApk(_destinationPath + "/arm64", false, false, true, false, true);
		}

		//static bool isBuildAndRun = true;
		//const string BuildAndRunMenuPath = "Wave/Build VRTestApp/Build and Run";
		//const string BuildAndRunKey = "VRTestApp_BuildAndRun";

		static bool isBuildOnlyScript = false;
		const string BuildOnlyScriptMenuPath = "Wave/Build VRTestApp/Build Only Script";
		const string BuildOnlyScriptKey = "VRTestApp_BuildOnlyScript";

		//[MenuItem(BuildAndRunMenuPath, false, priority = 500)]
		//static void CheckerBuildAndRun()
		//{
		//	isBuildAndRun = !isBuildAndRun;

		//	EditorPrefs.SetBool(BuildAndRunKey, isBuildAndRun);
		//	Menu.SetChecked(BuildAndRunMenuPath, isBuildAndRun);
		//}


		static string AddPlayerSettingsDefineSymbol(string defineSymbolString, string define)
		{
			var defineSymbols = defineSymbolString.Split(';');
			foreach (var defineSymbol in defineSymbols)
			{
				// already exist
				if (define == defineSymbol)
					return defineSymbolString;
			}
			return defineSymbolString = defineSymbolString + ";" + define;
		}

		static string RemovePlayerSettingsDefineSymbol(string defineSymbolString, string define)
		{
			var defineSymbols = defineSymbolString.Split(';');
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (var defineSymbol in defineSymbols)
			{
				if (define == defineSymbol)
				{
					first = false;
					continue;
				}

				if (!first)
					sb.Append(";");

				first = false;
				sb.Append(defineSymbol);
			}
			return sb.ToString();
		}

		[MenuItem(BuildOnlyScriptMenuPath, true)]
		static bool CheckerBuildOnlyScriptValidate()
		{
			isBuildOnlyScript = EditorPrefs.GetBool(BuildOnlyScriptKey, false);
			Menu.SetChecked(BuildOnlyScriptMenuPath, isBuildOnlyScript);
			return true;
		}

		[MenuItem(BuildOnlyScriptMenuPath, false, priority = 501)]
		static void CheckerBuildOnlyScript()
		{
			isBuildOnlyScript = !isBuildOnlyScript;

			EditorPrefs.SetBool(BuildOnlyScriptKey, isBuildOnlyScript);
			Menu.SetChecked(BuildOnlyScriptMenuPath, isBuildOnlyScript);
		}

		static SceneBuildConfiguration buildConfiguration = SceneBuildConfiguration.VerificationAndRelease;
		const string BuildConfigurationKey = "VRTestApp_BuildConfiguration";

		const string BuildForVerificationMenuPath = "Wave/Build VRTestApp/Build For Verification";
		const string BuildForReleaseMenuPath = "Wave/Build VRTestApp/Build For Release";
		const string BuildForDevelopmentMenuPath = "Wave/Build VRTestApp/Build For Development";

		const string BuildForVerificationDefine = "WAVE_BUILD_SAMPLE_VERIFICATION";
		const string BuildForReleaseDefine = "WAVE_BUILD_SAMPLE_RELEASE";
		const string BuildForDevelopmentDefine = "WAVE_BUILD_SAMPLE_DEVELOPMENT";

		[MenuItem(BuildForVerificationMenuPath, false, priority = 502)]
		static void CheckerBuildForVerification()
		{
			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

			const SceneBuildConfiguration v = SceneBuildConfiguration.Verification;
			bool isVerfication = (buildConfiguration & v) == 0;  // Inverted
			buildConfiguration = isVerfication ? (buildConfiguration | v) : (buildConfiguration & (~v));

			EditorPrefs.SetInt(BuildConfigurationKey, (int)buildConfiguration);

			UpdateBuildConfiguration();
			UpdateDefineSymbols();
		}

		[MenuItem(BuildForReleaseMenuPath, true)]
		static bool CheckerBuildForReleaseValidate()
		{
			UpdateBuildConfiguration();
			return true;
		}

		[MenuItem(BuildForReleaseMenuPath, false, priority = 503)]
		static void CheckerBuildForRelease()
		{
			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

			const SceneBuildConfiguration r = SceneBuildConfiguration.Release;
			bool isRelease = (buildConfiguration & r) == 0;  // Inverted
			buildConfiguration = isRelease ? (buildConfiguration | r) : (buildConfiguration & (~r));

			EditorPrefs.SetInt(BuildConfigurationKey, (int)buildConfiguration);

			UpdateBuildConfiguration();
			UpdateDefineSymbols();
		}

		[MenuItem(BuildForDevelopmentMenuPath, false, priority = 504)]
		static void CheckerBuildForDevelopment()
		{
			buildConfiguration = (SceneBuildConfiguration)EditorPrefs.GetInt(BuildConfigurationKey, (int)SceneBuildConfiguration.VerificationAndRelease);

			const SceneBuildConfiguration d = SceneBuildConfiguration.Development;
			bool isDevelopment = (buildConfiguration & d) == 0;  // Inverted
			buildConfiguration = isDevelopment ? (buildConfiguration | d) : (buildConfiguration & (~d));

			EditorPrefs.SetInt(BuildConfigurationKey, (int)buildConfiguration);

			UpdateBuildConfiguration();
			UpdateDefineSymbols();
		}

		[MenuItem("Wave/Build VRTestApp/Build 32+64bit", priority = 520)]
		static void BuildApk()
		{
			CustomizedCommandLine();
			buildConfiguration = SceneBuildConfiguration.Release;
			BuildApk(_destinationPath, false, false, true, true, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build 32bit", priority = 520)]
		static void BuildApk32()
		{
			CustomizedCommandLine();
			BuildApk(_destinationPath + "/armv7", false, false);
		}

		[MenuItem("Wave/Build VRTestApp/Build 64bit", priority = 520)]
		static void BuildApk64()
		{
			CustomizedCommandLine();
			BuildApk(_destinationPath, false, false, true, false, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Run 32+64bit", priority = 520)]
		static void BuildAndRunApk()
		{
			BuildApk(null, true, false, true, true, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Run 32bit", priority = 520)]
		static void BuildAndRunApk32()
		{
			BuildApk("armv7", true, false);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Run 64bit", priority = 520)]
		static void BuildAndRunApk64()
		{
			BuildApk(null, true, false, true, false, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Dev+Run 32+64bit", priority = 520)]
		static void BuildDevAndRunApk()
		{
			BuildApk(null, true, true, true, true, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Dev+Run 32bit", priority = 520)]
		static void BuildDevAndRunApk32()
		{
			BuildApk("armv7", true, true);
		}

		[MenuItem("Wave/Build VRTestApp/Build+Dev+Run 64bit", priority = 520)]
		static void BuildDevAndRunApk64()
		{
			BuildApk(null, true, true, true, false, true);
		}

		//[MenuItem("Wave/Build Windows Exe/Build+Run wvr_unity_vrtestapp.exe")]
		//static void BuildAndRunExe()
		//{
		//	BuildExe(null, true);
		//}

		public static void BuildExe(string destPath, bool run)
		{
			UpdateDefineSymbols();
			ResetPrebuildAssets();
			SetPrebuildAssets();
			List<string> levels = ScenesData.pathes;
			BuildApkInnerWindows(destPath, run, levels.ToArray());
		}

		public static void BuildApk(string destPath, bool run, bool development, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
		{
			UpdateDefineSymbols();
			ResetPrebuildAssets();
			SetPrebuildAssets();
			List<string> levels = ScenesData.pathes;
			BuildApkInner(destPath, run, development, levels.ToArray(), isIL2CPP, isSupport32, isSupport64);
		}

		// Independent this function because the command-line need run this eariler than buildApk to take effect.
		[MenuItem("Wave/Build VRTestApp/Apply VRTestApp PlayerSettings", priority = 540)]
		static void ApplyVRTestAppPlayerSettingsDefault()
		{
			ApplyVRTestAppPlayerSettings();
		}

		[MenuItem("Wave/Build VRTestApp/Add scenes to BuildSettings", priority = 541)]
		static void ApplyVRTestAppSettings()
		{
			UpdateDefineSymbols();
			// Every time you clicke the menu.  Update the assets once.
			ResetPrebuildAssets();

			List<string> levels = ScenesData.pathes;
			List<EditorBuildSettingsScene> editorScenes = new List<EditorBuildSettingsScene>();
			StringBuilder sb = new StringBuilder("Add scenes to BuildSettings").AppendLine();
			foreach (var level in levels)
			{
				sb.Append(level).AppendLine();
				editorScenes.Add(new EditorBuildSettingsScene(level, true));
			}
			Debug.Log(sb.ToString());
			EditorBuildSettings.scenes = editorScenes.ToArray();
		}


		static void ApplyVRTestAppPlayerSettings(bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
		{
			Debug.Log("ApplyVRTestAppPlayerSettings");

			GeneralSettings();

			if (!isSupport32 && !isSupport64)
				isSupport32 = true;

			PlayerSettings.productName = "VRTestApp";

			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.vrm.unity.VRTestApp");
			var VRTestAppPath = SearchScenePath("VRTestApp");
			var VRTestAppDir = Path.GetDirectoryName(VRTestAppPath);
			var IconPath = Directory.GetParent(VRTestAppDir).ToString().Replace("\\", "/") + "/Textures/test.png";
			Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(IconPath, typeof(Texture2D));
			if (icon == null)
				Debug.LogError("Fail to read app icon");

			Texture2D[] group = { icon, icon, icon, icon, icon, icon };

			PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, group);
			PlayerSettings.gpuSkinning = false;
			PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);
			PlayerSettings.graphicsJobs = true;

			// This can help check the Settings by text editor
			EditorSettings.serializationMode = SerializationMode.ForceText;

#pragma warning disable CS0618 // Type or member is obsolete
			PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Android, false);
#pragma warning restore CS0618 // Type or member is obsolete

			// Force use GLES31
			PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
			UnityEngine.Rendering.GraphicsDeviceType[] apis = { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 };
			PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, apis);
			PlayerSettings.openGLRequireES31 = true;
			PlayerSettings.openGLRequireES31AEP = true;

			if (isIL2CPP)
				PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
			else
				PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
			if (isSupport32)
			{
				if (isSupport64)
					PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
				else
					PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
			}
			else
			{
				PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
			}

			PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
#if UNITY_2022_1_OR_NEWER
			PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
#else
#if UNITY_2019_4_OR_NEWER
#if UNITY_2019_4_0 || UNITY_2019_4_1 || UNITY_2019_4_2 || UNITY_2019_4_3 || UNITY_2019_4_4 || UNITY_2019_4_5 || UNITY_2019_4_6 || UNITY_2019_4_7 || UNITY_2019_4_8 || UNITY_2019_4_9 || UNITY_2019_4_10 || UNITY_2019_4_11 || UNITY_2019_4_12 || UNITY_2019_4_13
			PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
#else
			PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
#endif
#else
			PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
#endif
#endif
			AssetDatabase.SaveAssets();
		}

		private static void BuildApkInner(string destPath, bool run, bool development, string[] levels, bool isIL2CPP = false, bool isSupport32 = true, bool isSupport64 = false)
		{
			var apkName = "wvr_unity_vrtestapp.apk";
			ApplyVRTestAppPlayerSettings(isIL2CPP, isSupport32, isSupport64);

			string outputFilePath = string.IsNullOrEmpty(destPath) ? apkName : destPath + "/" + apkName;
			BuildOptions extraFlags = BuildOptions.None;
			if (isBuildOnlyScript)
				extraFlags = BuildOptions.BuildScriptsOnly;
			BuildOptions buildOptions = (run ? BuildOptions.AutoRunPlayer : BuildOptions.None) | (development ? BuildOptions.Development : BuildOptions.None) | extraFlags;

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
			{
				//assetBundleManifestPath = "",
				options = buildOptions,
				target = BuildTarget.Android,
				scenes = levels,
				targetGroup = BuildTargetGroup.Android,
				locationPathName = outputFilePath
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}

		private static void BuildApkInnerWindows(string destPath, bool run, string[] levels)
		{
			var apkName = "wvr_unity_vrtestapp.exe";
			ApplyVRTestAppPlayerSettingsForWindows();

			string outputFilePath = string.IsNullOrEmpty(destPath) ? apkName : destPath + "/" + apkName;
			BuildOptions buildOptions = (run ? BuildOptions.AutoRunPlayer : BuildOptions.None) | (BuildOptions.None);

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
			{
				//assetBundleManifestPath = "",
				options = buildOptions,
				target = BuildTarget.StandaloneWindows64,
				scenes = levels,
				targetGroup = BuildTargetGroup.Standalone,
				locationPathName = outputFilePath
			};
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
	}
}

#endif
