// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public class WaveXRPlayerSettingsConfigDialog : EditorWindow
{
	List<Item> items;

	public class Item
	{
		const string ignore = "ignore.";
		const string useRecommended = "Use recommended ({0})";
		const string currentValue = " (current = {0})";

		public delegate bool DelegateIsReady();
		public delegate string DelegateGetCurrent();
		public delegate void DelegateSet();

		public DelegateIsReady IsReady;
		public DelegateGetCurrent GetCurrent;
		public DelegateSet Set;

		public string title { get; private set; }
		public string recommended { get; private set; }

		public Item(string title, string recommended)
		{
			this.title = title;
			this.recommended = recommended;
		}

		public bool IsIgnored { get { return EditorPrefs.HasKey(ignore + title); } }

		public void Ignore()
		{
			EditorPrefs.SetBool(ignore + title, true);
		}

		public void CleanIgnore()
		{
			EditorPrefs.DeleteKey(ignore + title);
		}

		// Return true when setting is not ready.
		public bool Show()
		{
			bool ignored = IsIgnored;
			GUILayout.Label(title + string.Format(currentValue, GetCurrent()) + (IsIgnored ? " (ignored)" : ""));
			if (ignored || IsReady())
				return false;

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(string.Format(useRecommended, recommended)))
				Set();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Ignore"))
				Ignore();
			GUILayout.EndHorizontal();
			return true;
		}
	}

	#region version_compatible
#if !UNITY_2020_2_OR_NEWER
#pragma warning disable CS0618 // Type or member is obsolete
	public static bool GetVirtualRealitySupported(BuildTargetGroup group)
	{
        return PlayerSettings.GetVirtualRealitySupported(group);
    }

	public static void SetVirtualRealitySupported(BuildTargetGroup group, bool set)
	{
		PlayerSettings.SetVirtualRealitySupported(group, set);
	}

	public static string[] GetVirtualRealitySDKs(BuildTargetGroup group)
	{
		return PlayerSettings.GetVirtualRealitySDKs(group);
	}

	public static void SetVirtualRealitySDKs(BuildTargetGroup group, string[] devices)
	{
		PlayerSettings.SetVirtualRealitySDKs(group, devices);
	}
#pragma warning restore CS0618 // Type or member is obsolete
#endif

	public static bool GetMobileMTRendering(BuildTargetGroup group)
	{
		return PlayerSettings.GetMobileMTRendering(group);
	}

	public static void SetMobileMTRendering(BuildTargetGroup group, bool set)
	{
		PlayerSettings.SetMobileMTRendering(group, set);
	}
	#endregion

	public static List<string> GetDefineSymbols(BuildTargetGroup group)
	{
		//https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/Editor/AddDefineSymbols.cs
		var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
		return symbols.Split(';').ToList();
	}

	static List<Item> GetItems() {
		var builtTarget = new Item("Build target", BuildTarget.Android.ToString())
		{
			IsReady = () => { return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android; },
			GetCurrent = () => { return EditorUserBuildSettings.activeBuildTarget.ToString(); },
			Set = () => { EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android); }
		};

		var defaultOrigin = new Item("Default orientation", UIOrientation.LandscapeLeft.ToString())
		{
			IsReady = () => { return PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeLeft; },
			GetCurrent = () => { return PlayerSettings.defaultInterfaceOrientation.ToString(); },
			Set = () => { PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft; }
		};

		var enableMTRendering = new Item("Enable multi-threading rendering", true.ToString())
		{
			IsReady = () => { return GetMobileMTRendering(BuildTargetGroup.Android); },
			GetCurrent = () => { return GetMobileMTRendering(BuildTargetGroup.Android).ToString(); },
			Set = () => { SetMobileMTRendering(BuildTargetGroup.Android, true); }
		};

		var autoGraphicsAPi = new Item("Set Auto Graphics Api", false.ToString())
		{
			IsReady = () => { return (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android) == false); },
			GetCurrent = () => { return PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android).ToString(); },
			Set = () => { PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false); }
		};

		UnityEngine.Rendering.GraphicsDeviceType[] apis = { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 };
		var graphicsApis = new Item("Graphics Apis", UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3.ToString())
		{
			IsReady = () => {
				var curapi = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
				return curapi.Length == 1 && curapi[0].Equals(UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3) &&
					PlayerSettings.openGLRequireES31 == PlayerSettings.openGLRequireES31AEP == PlayerSettings.openGLRequireES32 == true;
			},
			GetCurrent = () => { var curapi = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);  return (curapi.Length > 0) ? curapi[0].ToString() : "null"; },
			Set = () => {
				PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, apis);
				PlayerSettings.openGLRequireES31 = PlayerSettings.openGLRequireES31AEP = PlayerSettings.openGLRequireES32 = true;
			}
		};

		var AndroidMinSDK = new Item("Android Min SDK version", AndroidSdkVersions.AndroidApiLevel25.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel25; },
			GetCurrent = () => { return PlayerSettings.Android.minSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25; }
		};

#if UNITY_2019_4_OR_NEWER
#if  UNITY_2019_4_0 || UNITY_2019_4_1 || UNITY_2019_4_2 || UNITY_2019_4_3 || UNITY_2019_4_4 || UNITY_2019_4_5 || UNITY_2019_4_6 || UNITY_2019_4_7 || UNITY_2019_4_8 || UNITY_2019_4_9 || UNITY_2019_4_10 || UNITY_2019_4_11 || UNITY_2019_4_12 || UNITY_2019_4_13
		var AndroidTargetSDK = new Item("Android Target SDK version", AndroidSdkVersions.AndroidApiLevel26.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.targetSdkVersion >= AndroidSdkVersions.AndroidApiLevel26; },
			GetCurrent = () => { return PlayerSettings.Android.targetSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26; }
		};
#else
		var AndroidTargetSDK = new Item("Android Target SDK version", AndroidSdkVersions.AndroidApiLevel29.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.targetSdkVersion >= AndroidSdkVersions.AndroidApiLevel29; },
			GetCurrent = () => { return PlayerSettings.Android.targetSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel29; }
		};
#endif
#else
		var AndroidTargetSDK = new Item("Android Target SDK version", AndroidSdkVersions.AndroidApiLevel26.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.targetSdkVersion >= AndroidSdkVersions.AndroidApiLevel26; },
			GetCurrent = () => { return PlayerSettings.Android.targetSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel26; }
		};
#endif

		var gpuSkinning = new Item("GPU Skinning", false.ToString())
		{
			IsReady = () => { return !PlayerSettings.gpuSkinning; },
			GetCurrent = () => { return PlayerSettings.gpuSkinning.ToString(); },
			Set = () => { PlayerSettings.gpuSkinning = false; }
		};

		var scriptingBackend = new Item("ScriptingBackend", ScriptingImplementation.IL2CPP.ToString())
		{
			IsReady = () => { return PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP; },
			GetCurrent = () => { return PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android).ToString(); },
			Set = () => { PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP); }
		};

		var androidArchitecture = new Item("AndroidArchitecture", AndroidArchitecture.ARM64.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.targetArchitectures == AndroidArchitecture.ARM64; },
			GetCurrent = () => { return PlayerSettings.Android.targetArchitectures.ToString(); },
			Set = () => { PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; }
		};

		return new List<Item>()
		{
			builtTarget,
			defaultOrigin,
			enableMTRendering,
			gpuSkinning,
			autoGraphicsAPi,
			graphicsApis,
			AndroidMinSDK,
			AndroidTargetSDK,
			scriptingBackend,
			androidArchitecture
		};
	}

	static WaveXRPlayerSettingsConfigDialog window;

	static WaveXRPlayerSettingsConfigDialog()
	{
		EditorApplication.update += Update;
	}

	//[MenuItem("Wave/Show PlayerSettings Dialog", priority = 0)]
	public static void ShowDialog()
	{
		var items = GetItems();
		UpdateInner(items, true);
	}

	static void Update()
	{
		Debug.Log("Check for Wave prefered editor settings");
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
		{
			var items = GetItems();
			UpdateInner(items, false);
		}
		EditorApplication.update -= Update;
	}

	public static void UpdateInner(List<Item> items, bool forceShow)
	{
		bool show = forceShow;
		if (!forceShow)
		{
			foreach (var item in items)
			{
				show |= !item.IsIgnored && !item.IsReady();
			}
		}

		if (show)
		{
			window = GetWindow<WaveXRPlayerSettingsConfigDialog>(true);
			window.minSize = new Vector2(640, 320);
			window.items = items;
		}
	}

	Vector2 scrollPosition;

	string GetResourcePath()
	{
		var ms = MonoScript.FromScriptableObject(this);
		var path = AssetDatabase.GetAssetPath(ms);
		path = Path.GetDirectoryName(path);
		return path.Substring(0, path.Length - "Editor".Length) + "Runtime/Textures/";
	}

	public void OnGUI()
	{
		var resourcePath = GetResourcePath();
		var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "WaveViveLogoFlat.png");
		var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
		if (logo)
			GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

		EditorGUILayout.HelpBox("Recommended project settings for Wave:", MessageType.Info);

		if (items == null)
			return;

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		int notReadyItems = 0;
		foreach(var item in items)
		{
			if (item.Show())
				notReadyItems++;
		}

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Clear All Ignores"))
		{
			foreach (var item in items)
			{
				item.CleanIgnore();
			}
		}

		GUILayout.EndHorizontal();

		GUILayout.EndScrollView();

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		if (notReadyItems > 0)
		{
			if (GUILayout.Button("Accept All"))
			{
				foreach (var item in items)
				{
					// Only set those that have not been explicitly ignored.
					if (!item.IsIgnored)
						item.Set();
				}

				EditorUtility.DisplayDialog("Accept All", "You made the right choice!", "Ok");

				Close();
			}

			if (GUILayout.Button("Ignore All"))
			{
				if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
				{
					foreach (var item in items)
					{
						// Only ignore those that do not currently match our recommended settings.
						if (!item.IsReady())
							item.Ignore();
					}

					Close();
				}
			}
		}
		else 
		{
			if (GUILayout.Button("Close"))
				Close();
		}
		GUILayout.EndHorizontal();
	}
}
