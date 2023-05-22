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

			bool hasKeyword = false;
			bool showAndroidManifest = false;
			showAndroidManifest = searchContext.Contains("AndroidManifest");
			if(showAndroidManifest)
				hasKeyword = true;

			/**
             * GUI layout of features.
             * 1. AndroidManifest
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

		private void ImportModule(string packagePath)
		{
			string target = Path.Combine("Packages/com.htc.upm.wave.xrsdk/UnityPackages~", packagePath);
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
