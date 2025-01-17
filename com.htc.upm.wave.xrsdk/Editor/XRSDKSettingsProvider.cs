using System.IO;
using UnityEditor;
using UnityEngine;
using Wave.XR.Settings;

namespace Wave.XR
{
	internal class XRSDKSettingsProvider : SettingsProvider
    {
		internal static string WaveXRPath = "Assets/Wave/XR";

		private static void CheckingWavePackagePath()
		{
			if (!File.Exists("Assets/Wave/XR/WaveXRDummy.unity"))
			{
				var guids = AssetDatabase.FindAssets("WaveXRDummy");

				foreach (string guid in guids)
				{
					var path = AssetDatabase.GUIDToAssetPath(guid);
					if (path.EndsWith("WaveXRDummy.unity"))
					{
						WaveXRSettings settings;
						EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
						var sub = path.Substring(0, path.Length - "/WaveXRDummy.unity".Length);
						WaveXRPath = sub;
						if (settings != null)
							settings.waveXRFolder = sub;
						Debug.Log("WaveXRPath = " + WaveXRPath);
					}
				}
			}
			else
			{
				WaveXRSettings settings;
				EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
				WaveXRPath = "Assets/Wave/XR";
				if (settings != null)
					settings.waveXRFolder = "Assets/Wave/XR";
				Debug.Log("WaveXRPath = " + WaveXRPath);
			}
		}

		private static readonly string[] xrsdkKeywords = new string[]
		{
			"Wave",
			"XR",
			"AndroidManifest",
			"MRTKSupport",
		};

		public XRSDKSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope, xrsdkKeywords)
        {
			CheckingWavePackagePath();
		}

		internal static void Init()
		{
			CheckingWavePackagePath();
		}

		public override void OnGUI(string searchContext)
        {
			bool m_FeatureAndroidManifestImported = Directory.Exists(WaveXRPath + "/Platform/Android");
			bool m_FeatureMRTKSupportImported = Directory.Exists(WaveXRPath + "/MRTKSupport");

			bool hasKeyword = false;
			bool showAndroidManifest = false;
			bool showMRTKSupport = false;
			showAndroidManifest = searchContext.Contains("AndroidManifest");
			showMRTKSupport = searchContext.Contains("MRTKSupport");
			if (showAndroidManifest || showMRTKSupport)
				hasKeyword = true;

			/**
             * GUI layout of features.
             * 1. AndroidManifest
             * 2. MRTK Support
            **/
			if (showAndroidManifest || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Custom AndroidManifest", EditorStyles.boldLabel);
					GUILayout.Label("This package provides features of custom android manifest.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveXRPath + "/Platform/Android.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !m_FeatureAndroidManifestImported;
					if (GUILayout.Button("Import Feature - Custom Android Manifest", GUILayout.ExpandWidth(false)))
						ImportModule("wave_xrsdk_androidmanifest.unitypackage");
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showMRTKSupport || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("MRTK Support", EditorStyles.boldLabel);
					GUILayout.Label("MRTK support is a feature that enables you to run apps built with MRTK on VIVE.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveXRPath + "/MRTKSupport.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !m_FeatureMRTKSupportImported;
					if (GUILayout.Button("Import Feature - MRTK 2.8.3 Support", GUILayout.ExpandWidth(false)))
						ImportModule("Packages/com.htc.upm.wave.xrsdk/Runtime/MRTKSupport/2.8.3/wave_xrsdk_mrtksupport.unitypackage", false);
					GUILayout.Space(5f);
					if (GUILayout.Button("Import Feature - MRTK 3.0.0 Support", GUILayout.ExpandWidth(false)))
						ImportModule("Packages/com.htc.upm.wave.xrsdk/Runtime/MRTKSupport/3.0.0/wave_xrsdk_mrtksupport.unitypackage", false);
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}
#if !UNITY_2022_2_OR_NEWER
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Update Android Gradle Plugin and Gradle Version [2024.April]", EditorStyles.boldLabel);
					GUILayout.Label("Unity developers using Unity 2022.3 or later can now use the default AGP and gradle settings.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Space(5f);
					GUILayout.Label("Our SDK is built with Java 11 libraries. To avoid errors when building Android APKs, please use our recommended and verified development environment: AGP 4.2.0 and gradle version 6.9.2.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Space(5f);
					GUILayout.Label("How to Update", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Space(5f);
					GUILayout.Label("1.Download and extract the gradle 6.9.2 zip file.)", new GUIStyle(EditorStyles.label) { wordWrap = true });
					if (GUILayout.Button("Download URL", GUILayout.ExpandWidth(false)))
						Application.OpenURL("https://gradle.org/releases/");
					GUILayout.Space(5f);
					GUILayout.Label("2.In Edit > Preferences > External Tools, specify the path to the extracted gradle folder.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					if (GUILayout.Button("Setting in the Preferences", GUILayout.ExpandWidth(false)))
						SettingsService.OpenUserPreferences("Preferences/External Tools");
					GUILayout.Space(5f);
					GUILayout.Label("3.In Edit > Project Settings > Player > Publishing Settings, check the \"Custom Base Gradle Template\" box. This will generate the Assets > Plugins > Android > baseProjectTemplate.gradle file.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					if (GUILayout.Button("Player Settings", GUILayout.ExpandWidth(false)))
						SettingsService.OpenProjectSettings("Project/Player");
					GUILayout.Space(5f);
					GUILayout.Label("4.Edit the baseProjectTemplate.gradle file and set \"com.android.tools.build:gradle\" to 4.2.0.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Space(5f);
					bool b = EditorPrefs.GetBool("CustomGradleVersionSkip", false);
					if (GUILayout.Toggle(b, "Ignore the warning dialog."))
					{
						EditorPrefs.SetBool("CustomGradleVersionSkip", true);
					}
					else
					{
						EditorPrefs.SetBool("CustomGradleVersionSkip", false);
					}
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}
#endif
		}

		public static void MoveFolder(string srcpath, string destpath)
		{
			if (Directory.Exists(srcpath))
			{
				var srcfiles = Directory.GetFiles(srcpath);
				var srcdirs = Directory.GetDirectories(srcpath);
				foreach (var file in srcfiles)
				{
					string destfile = destpath + file.Substring(srcpath.Length, file.Length - srcpath.Length);
					if (!File.Exists(destfile))
						File.Move(file, destfile);
				}
				foreach (var dir in srcdirs)
				{
					string destdir = destpath + dir.Substring(srcpath.Length, dir.Length - srcpath.Length);
					if (!Directory.Exists(destdir))
						Directory.Move(dir, destdir);
				}
			}
		}

		public static void CleanHouse()
		{
			if (Directory.Exists("Assets/Wave/XR"))
				Directory.Delete("Assets/Wave/XR");
			if (File.Exists("Assets/Wave/XR.meta"))
				File.Delete("Assets/Wave/XR.meta");
			if (Directory.Exists("Assets/Wave"))
				Directory.Delete("Assets/Wave");
			if (File.Exists("Assets/Wave.meta"))
				File.Delete("Assets/Wave.meta");
		}

		private void ImportModule(string packagePath, bool padding = true)
		{
			string target = padding == true? Path.Combine("Packages/com.htc.upm.wave.xrsdk/UnityPackages~", packagePath) : packagePath;
			Debug.Log("Import: " + target);
			AssetDatabase.ImportPackage(target, false);
		}

		[SettingsProvider]
        static SettingsProvider Create()
        {
            return new XRSDKSettingsProvider("Project/Wave XR/XRSDK");
        }
    }

	[InitializeOnLoad]
	public class PackageInfo : AssetPostprocessor
	{
		static PackageInfo()
		{
			AssetDatabase.importPackageStarted += OnImportPackageStarted;
			AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
		}

		private static void OnImportPackageStarted(string packagename)
		{
			Debug.Log("OnImportPackageStarted() " + packagename);
		}

		private static void OnImportPackageCompleted(string packagename)
		{
			Debug.Log("OnImportPackageCompleted() " + packagename);
			XRSDKSettingsProvider.Init();

			if (!XRSDKSettingsProvider.WaveXRPath.Equals("Assets/Wave/XR"))
				MoveXRFolder();
		}

		private static void MoveXRFolder()
		{
			if (Directory.Exists("Assets/Wave/XR"))
			{
				if (!Directory.Exists(XRSDKSettingsProvider.WaveXRPath.Substring(0, XRSDKSettingsProvider.WaveXRPath.Length - "/XR".Length)))
					Directory.CreateDirectory(XRSDKSettingsProvider.WaveXRPath.Substring(0, XRSDKSettingsProvider.WaveXRPath.Length - "/XR".Length));
				if (!Directory.Exists(XRSDKSettingsProvider.WaveXRPath))
					Directory.CreateDirectory(XRSDKSettingsProvider.WaveXRPath);
				XRSDKSettingsProvider.MoveFolder("Assets/Wave/XR", XRSDKSettingsProvider.WaveXRPath);
				XRSDKSettingsProvider.CleanHouse();
				AssetDatabase.Refresh();
			}
		}
	}
}
