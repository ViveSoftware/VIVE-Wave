// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Wave.XR;
using Wave.XR.Loader;

#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace Wave.Essence.Editor
{
	[InitializeOnLoad]
	public static class Preferences
	{
		const string LOG_TAG = "Wave.Essence.Editor.Preferences";
		static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

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

		static Preferences()
		{
			EditorApplication.update += OnUpdate;
		}

		// needs XR Plugin Management version > 4.2.1
		internal static bool ViveWaveXRAndroidAssigned {
			get {
#if XR_PLUGIN_MANAGEMENT_4_2_1
				return XRPackageMetadataStore.IsLoaderAssigned("Wave.XR.Loader.WaveXRLoader", BuildTargetGroup.Android);
#else
				return true;
#endif
			}
		}

		const string DEFINE_USE_VRM_0_x = "USE_VRM_0_x";
		static readonly ScriptingDefinedSettings m_ScriptDefineSettingVrm0 = new ScriptingDefinedSettings(
			new string[] { DEFINE_USE_VRM_0_x, },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);

		const string DEFINE_WAVE_BODY_TRACKING = "WAVE_BODY_TRACKING";
		static readonly ScriptingDefinedSettings m_ScriptDefineSettingBodyTracking = new ScriptingDefinedSettings(
			new string[] { DEFINE_WAVE_BODY_TRACKING, },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);

		const string DEFINE_WAVE_BODY_IK = "WAVE_BODY_IK";
		const string DEFINE_WAVE_BODY_CALIBRATION = "WAVE_BODY_CALIBRATION";
		const string DEFINE_WAVE_BODY_CALIBRATION_INCONTENT = "WAVE_BODY_CALIBRATION_INCONTENT";
		static readonly ScriptingDefinedSettings m_ScriptDefineSettingBodyTrackingConfiguration = new ScriptingDefinedSettings(
			new string[] { DEFINE_WAVE_BODY_CALIBRATION_INCONTENT },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);
		public static void ConfigureBodyTracking(bool enable)
		{
			if (enable)
			{
				if (!HasScriptingDefineSymbols(m_ScriptDefineSettingBodyTrackingConfiguration))
				{
					DEBUG("OnUpdate() Adds m_ScriptDefineSettingBodyTrackingConfiguration.");
					AddScriptingDefineSymbols(m_ScriptDefineSettingBodyTrackingConfiguration);
				}
			}
			else
			{
				if (HasScriptingDefineSymbols(m_ScriptDefineSettingBodyTrackingConfiguration))
				{
					DEBUG("OnUpdate() Removes m_ScriptDefineSettingBodyTrackingConfiguration.");
					RemoveScriptingDefineSymbols(m_ScriptDefineSettingBodyTrackingConfiguration);
				}
			}
		}

		const string DEFINE_FACIAL_TRACKING = "WAVE_FACIAL_TRACKING";
		static readonly ScriptingDefinedSettings m_ScriptDefineSettingFacialTracking = new ScriptingDefinedSettings(
			new string[] { DEFINE_FACIAL_TRACKING, },
			new BuildTargetGroup[] { BuildTargetGroup.Android, }
		);

		static void OnUpdate()
		{
			if (!ViveWaveXRAndroidAssigned) { return; }

			CheckBodyTrackingAsset();

			if (m_AssetAvatar)
			{
				// Adds the script symbol if VRM0 is imported.
				if (File.Exists(PreferenceAvatarAsset.kVrm0Asset))
				{
					if (!HasScriptingDefineSymbols(m_ScriptDefineSettingVrm0))
					{
						DEBUG("OnUpdate() Adds m_ScriptDefineSettingVrm0.");
						AddScriptingDefineSymbols(m_ScriptDefineSettingVrm0);
					}
					m_AssetAvatar.SupportVrm0 = true;
				}
				else
				{
					if (HasScriptingDefineSymbols(m_ScriptDefineSettingVrm0))
					{
						DEBUG("OnUpdate() Removes m_ScriptDefineSettingVrm0.");
						RemoveScriptingDefineSymbols(m_ScriptDefineSettingVrm0);
					}
					m_AssetAvatar.SupportVrm0 = false;
				}

				m_AssetAvatar.SupportVrm1 = File.Exists(PreferenceAvatarAsset.kVrm1Asset);

				// Adds the script symbol if Body Tracking is imported.
				if (File.Exists(PreferenceAvatarAsset.kBodyTrackingAsset))
				{
					if (!HasScriptingDefineSymbols(m_ScriptDefineSettingBodyTracking))
					{
						DEBUG("OnUpdate() Adds m_ScriptDefineSettingBodyTracking.");
						AddScriptingDefineSymbols(m_ScriptDefineSettingBodyTracking);

						ConfigureBodyTracking(true);
					}
				}
				else
				{
					if (HasScriptingDefineSymbols(m_ScriptDefineSettingBodyTracking))
					{
						DEBUG("OnUpdate() Removes m_ScriptDefineSettingBodyTracking.");
						RemoveScriptingDefineSymbols(m_ScriptDefineSettingBodyTracking);

						ConfigureBodyTracking(false);
					}
				}

				// Adds the script symbol if Facial Tracking is imported.
				if (File.Exists(PreferenceAvatarAsset.kFacialExpressionMakerAsset))
				{
					if (!HasScriptingDefineSymbols(m_ScriptDefineSettingFacialTracking))
					{
						DEBUG("OnUpdate() Adds m_ScriptDefineSettingFacialTracking.");
						AddScriptingDefineSymbols(m_ScriptDefineSettingFacialTracking);
					}
				}
				else
				{
					if (HasScriptingDefineSymbols(m_ScriptDefineSettingFacialTracking))
					{
						DEBUG("OnUpdate() Removes m_ScriptDefineSettingFacialTracking.");
						RemoveScriptingDefineSymbols(m_ScriptDefineSettingFacialTracking);
					}
				}
			}
		}

		internal static class PackageManagerHelper
		{
			private static bool s_wasPreparing;
			private static bool m_wasAdded;
			private static bool s_wasRemoved;
			private static ListRequest m_listRequest;
			private static AddRequest m_addRequest;
			private static RemoveRequest m_removeRequest;
			private static string s_fallbackIdentifier;

			public static bool isPreparingList
			{
				get
				{
					if (m_listRequest == null) { return s_wasPreparing = true; }

					switch (m_listRequest.Status)
					{
						case StatusCode.InProgress:
							return s_wasPreparing = true;
						case StatusCode.Failure:
							if (!s_wasPreparing)
							{
								Debug.LogError("Something wrong when adding package to list. error:" + m_listRequest.Error.errorCode + "(" + m_listRequest.Error.message + ")");
							}
							break;
						case StatusCode.Success:
							break;
					}

					return s_wasPreparing = false;
				}
			}

			public static bool isAddingToList
			{
				get
				{
					if (m_addRequest == null) { return m_wasAdded = false; }

					switch (m_addRequest.Status)
					{
						case StatusCode.InProgress:
							return m_wasAdded = true;
						case StatusCode.Failure:
							if (!m_wasAdded)
							{
								AddRequest request = m_addRequest;
								m_addRequest = null;
								if (string.IsNullOrEmpty(s_fallbackIdentifier))
								{
									Debug.LogError("Something wrong when adding package to list. error:" + request.Error.errorCode + "(" + request.Error.message + ")");
								}
								else
								{
									Debug.Log("Failed to install package: \"" + request.Error.message + "\". Retry with fallback identifier \"" + s_fallbackIdentifier + "\"");
									AddToPackageList(s_fallbackIdentifier);
								}

								s_fallbackIdentifier = null;
							}
							break;
						case StatusCode.Success:
							if (!m_wasAdded)
							{
								m_addRequest = null;
								s_fallbackIdentifier = null;
								ResetPackageList();
							}
							break;
					}

					return m_wasAdded = false;
				}
			}

			public static bool isRemovingFromList
			{
				get
				{
					if (m_removeRequest == null) { return s_wasRemoved = false; }

					switch (m_removeRequest.Status)
					{
						case StatusCode.InProgress:
							return s_wasRemoved = true;
						case StatusCode.Failure:
							if (!s_wasRemoved)
							{
								var request = m_removeRequest;
								m_removeRequest = null;
								Debug.LogError("Something wrong when removing package from list. error:" + m_removeRequest.Error.errorCode + "(" + m_removeRequest.Error.message + ")");
							}
							break;
						case StatusCode.Success:
							if (!s_wasRemoved)
							{
								m_removeRequest = null;
								ResetPackageList();
							}
							break;
					}

					return s_wasRemoved = false;
				}
			}

			public static void PreparePackageList()
			{
				if (m_listRequest != null) { return; }
				m_listRequest = Client.List(true, true);
			}

			public static void ResetPackageList()
			{
				s_wasPreparing = false;
				m_listRequest = null;
			}

			public static bool IsPackageInList(string name, out UnityEditor.PackageManager.PackageInfo packageInfo)
			{
				packageInfo = null;
				if (m_listRequest == null || m_listRequest.Result == null) return false;

				foreach (var package in m_listRequest.Result)
				{
					if (package.name.Equals(name))
					{
						packageInfo = package;
						return true;
					}
				}
				return false;
			}

			public static void AddToPackageList(string identifier, string fallbackIdentifier = null)
			{
				Debug.Assert(m_addRequest == null);

				m_addRequest = Client.Add(identifier);
				s_fallbackIdentifier = fallbackIdentifier;
			}

			public static void RemovePackage(string identifier)
			{
				Debug.Assert(m_removeRequest == null);

				m_removeRequest = Client.Remove(identifier);
			}

			public static PackageCollection GetPackageList()
			{
				if (m_listRequest == null || m_listRequest.Result == null)
				{
					return null;
				}

				return m_listRequest.Result;
			}
		}

		static PreferenceAvatarAsset m_AssetAvatar = null;

		internal static void ImportModule(string packagePath, bool interactive = false)
		{
			string target = Path.Combine("Packages/" + Constants.EssencePackageName + "/UnityPackages~", packagePath);
			Debug.Log("Import: " + target);
			AssetDatabase.ImportPackage(target, interactive);
		}

		const string kPreferenceName = "Wave Settings";
		private static GUIContent m_Vrm0Option = new GUIContent("VRM 0", "Avatar format.");
		private static GUIContent m_Vrm1Option = new GUIContent("VRM 1", "Avatar format.");

		static bool avatarOption = true;
#pragma warning disable 0618
		[PreferenceItem(kPreferenceName)]
#pragma warning restore 0618
		private static void OnPreferencesGUI()
		{
			if (EditorApplication.isCompiling)
			{
				EditorGUILayout.LabelField("Compiling...");
				return;
			}
			if (PackageManagerHelper.isAddingToList)
			{
				EditorGUILayout.LabelField("Installing packages...");
				return;
			}
			if (PackageManagerHelper.isRemovingFromList)
			{
				EditorGUILayout.LabelField("Removing packages...");
				return;
			}

			PackageManagerHelper.PreparePackageList();
			if (PackageManagerHelper.isPreparingList)
			{
				EditorGUILayout.LabelField("Checking Packages...");
				return;
			}

			CheckBodyTrackingAsset();

			GUIStyle sectionTitleStyle = new GUIStyle(EditorStyles.label);
			sectionTitleStyle.fontSize = 16;
			sectionTitleStyle.richText = true;

			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.Label("Essence Settings", sectionTitleStyle);
			GUILayout.EndHorizontal();

			#region Vive Wave XR plugin
			GUIStyle foldoutStyle = EditorStyles.foldout;
			foldoutStyle.fontSize = 15;
			foldoutStyle.fontStyle = FontStyle.Bold;

			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			avatarOption = EditorGUILayout.Foldout(avatarOption, "Supported Avatar Format", foldoutStyle);
			GUILayout.EndHorizontal();

			foldoutStyle.fontSize = 12;
			foldoutStyle.fontStyle = FontStyle.Normal;

			if (m_AssetAvatar && avatarOption)
			{
				/// VRM 0
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				GUILayout.Space(35);
				if (!m_AssetAvatar.SupportVrm0)
				{
					bool toggled = EditorGUILayout.ToggleLeft(m_Vrm0Option, false, GUILayout.Width(230f));
					if (toggled)
					{
						DEBUG("OnPreferencesGUI() Adds " + PreferenceAvatarAsset.kVrm0Package);
						ImportModule(PreferenceAvatarAsset.kVrm0Package);
					}
				}
				else
				{
					EditorGUILayout.ToggleLeft(m_Vrm0Option, true, GUILayout.Width(230f));
				}
				GUILayout.EndHorizontal();

				/// VRM 1
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				GUILayout.Space(35);
				if (!m_AssetAvatar.SupportVrm1)
				{
					bool toggled = EditorGUILayout.ToggleLeft(m_Vrm1Option, false, GUILayout.Width(230f));
					if (toggled)
					{
						DEBUG("OnPreferencesGUI() Adds " + PreferenceAvatarAsset.kVrm1Package);
						ImportModule(PreferenceAvatarAsset.kVrm1Package);
					}
				}
				else
				{
					EditorGUILayout.ToggleLeft(m_Vrm1Option, true, GUILayout.Width(230f));
				}
				GUILayout.EndHorizontal();
			}
#endregion
		}

		static void CheckBodyTrackingAsset()
		{
			if (File.Exists(PreferenceAvatarAsset.AssetPath))
			{
				m_AssetAvatar = AssetDatabase.LoadAssetAtPath(PreferenceAvatarAsset.AssetPath, typeof(PreferenceAvatarAsset)) as PreferenceAvatarAsset;
			}
			else
			{
				string folderPath = PreferenceAvatarAsset.AssetPath.Substring(0, PreferenceAvatarAsset.AssetPath.LastIndexOf('/'));
				DirectoryInfo folder = Directory.CreateDirectory(folderPath);
				DEBUG("CheckBodyTrackingAsset() Creates folder: Assets/" + folder.Name);

				m_AssetAvatar = ScriptableObject.CreateInstance(typeof(PreferenceAvatarAsset)) as PreferenceAvatarAsset;
				m_AssetAvatar.SupportVrm0 = false;
				m_AssetAvatar.SupportVrm1 = false;

				DEBUG("CheckBodyTrackingAsset() Creates the asset: " + PreferenceAvatarAsset.AssetPath);
				AssetDatabase.CreateAsset(m_AssetAvatar, PreferenceAvatarAsset.AssetPath);
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
	}
}
#endif
