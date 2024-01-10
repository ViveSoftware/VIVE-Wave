// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using Wave.XR;
using Wave.XR.Settings;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	public class EssenceSettingsProvider : SettingsProvider
	{
		public static string WaveEssencePath = "Assets/Wave/Essence";
		private static void CheckingWavePackagePath()
		{
			if (!File.Exists("Assets/Wave/Essence/ControllerModel.asset"))
			{
				var guids = AssetDatabase.FindAssets("ControllerModel");

				foreach (string guid in guids)
				{
					var path = AssetDatabase.GUIDToAssetPath(guid);
					if (path.EndsWith("ControllerModel.asset"))
					{
						UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(PackageEssenceAsset));

						if (asset != null && asset.GetType() == typeof(PackageEssenceAsset))
						{
							WaveXRSettings settings;
							EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
							var sub = path.Substring(0, path.Length - "/ControllerModel.asset".Length);
							WaveEssencePath = sub;
							if (settings != null)
								settings.waveEssenceFolder = sub;

							Debug.Log("WaveEssencePath = " + WaveEssencePath);
						}
					}
				}
			}
			else
			{
				WaveXRSettings settings;
				EditorBuildSettings.TryGetConfigObject(Constants.k_SettingsKey, out settings);
				WaveEssencePath = "Assets/Wave/Essence";
				if (settings != null)
					settings.waveEssenceFolder = "Assets/Wave/Essence";
				Debug.Log("WaveEssencePath = " + WaveEssencePath);
			}
		}

		#region Essence.Controller.Model asset
		const string kControllerModelAsset = "/ControllerModel.asset";
		public static void UpdateAssetControllerModel(bool importedControllerModelPackage)
		{
			PackageEssenceAsset asset = null;
			if (File.Exists(WaveEssencePath + kControllerModelAsset))
			{
				asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kControllerModelAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedControllerModelPackage = importedControllerModelPackage;
			}
			else
			{
				asset = ScriptableObject.CreateInstance(typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedControllerModelPackage = importedControllerModelPackage;
				if (!Directory.Exists(WaveEssencePath.Substring(0, WaveEssencePath.Length - "/Essence".Length)))
					Directory.CreateDirectory(WaveEssencePath.Substring(0, WaveEssencePath.Length - "/Essence".Length));
				if (!Directory.Exists(WaveEssencePath))
					Directory.CreateDirectory(WaveEssencePath);
				AssetDatabase.CreateAsset(asset, WaveEssencePath + kControllerModelAsset);
			}
			AssetDatabase.SaveAssets();
			Debug.Log("UpdateAssetControllerModel() " + WaveEssencePath + kControllerModelAsset + ", importedControllerModelPackage: " + asset.importedControllerModelPackage);
		}
		internal static bool IsControllerModelPackageOnceImported()
		{
			if (!File.Exists(WaveEssencePath + kControllerModelAsset))
				return false;

			PackageEssenceAsset asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kControllerModelAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
			return asset.importedControllerModelPackage;
		}
		#endregion

		#region Essence.InputModule asset
		const string kInputModuleAsset = "/InputModule.asset";
		public static void UpdateAssetInputModule(bool importedInputModulePackage)
		{
			PackageEssenceAsset asset = null;
			if (File.Exists(WaveEssencePath + kInputModuleAsset))
			{
				asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kInputModuleAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedInputModulePackage = importedInputModulePackage;
			}
			else
			{
				asset = ScriptableObject.CreateInstance(typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedInputModulePackage = importedInputModulePackage;
				AssetDatabase.CreateAsset(asset, WaveEssencePath + kInputModuleAsset);
			}
			AssetDatabase.SaveAssets();
			Debug.Log("UpdateAssetInputModule() " + WaveEssencePath + kInputModuleAsset + ", importedInputModulePackage: " + asset.importedInputModulePackage);
		}
		internal static bool IsInputModulePackageOnceImported()
		{
			if (!File.Exists(WaveEssencePath + kInputModuleAsset))
				return false;

			PackageEssenceAsset asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kInputModuleAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
			return asset.importedInputModulePackage;
		}
		#endregion

		#region Essence.Hand.Model asset
		const string kHandModelAsset = "/HandModel.asset";
		public static void UpdateAssetHandModel(bool importedHandModelPackage)
		{
			PackageEssenceAsset asset = null;
			if (File.Exists(WaveEssencePath + kHandModelAsset))
			{
				asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kHandModelAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedHandModelPackage = importedHandModelPackage;
			}
			else
			{
				asset = ScriptableObject.CreateInstance(typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedHandModelPackage = importedHandModelPackage;
				AssetDatabase.CreateAsset(asset, WaveEssencePath + kHandModelAsset);
			}
			AssetDatabase.SaveAssets();
			Debug.Log("UpdateAssetHandModel() " + WaveEssencePath + kHandModelAsset + ", importedHandModelPackage: " + asset.importedHandModelPackage);
		}
		internal static bool IsHandModelPackageOnceImported()
		{
			if (!File.Exists(WaveEssencePath + kHandModelAsset))
				return false;

			PackageEssenceAsset asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kHandModelAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
			return asset.importedHandModelPackage;
		}
		#endregion

		#region Essence.Interaction.Mode asset
		const string kInteractionModeAsset = "/InteractionMode.asset";
		public static void UpdateAssetInteractionMode(bool importedInteractionModePackage)
		{
			PackageEssenceAsset asset = null;
			if (File.Exists(WaveEssencePath + kInteractionModeAsset))
			{
				asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kInteractionModeAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedInteractionModePackage = importedInteractionModePackage;
			}
			else
			{
				asset = ScriptableObject.CreateInstance(typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedInteractionModePackage = importedInteractionModePackage;
				AssetDatabase.CreateAsset(asset, WaveEssencePath + kInteractionModeAsset);
			}
			AssetDatabase.SaveAssets();
			Debug.Log("UpdateAssetInteractionMode() " + WaveEssencePath + kInteractionModeAsset + ", importedInteractionModePackage: " + asset.importedInteractionModePackage);
		}
		internal static bool IsInteractionModePackageOnceImported()
		{
			if (!File.Exists(WaveEssencePath + kInteractionModeAsset))
				return false;

			PackageEssenceAsset asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kInteractionModeAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
			return asset.importedInteractionModePackage;
		}
		#endregion

		#region BodyTracking asset
		const string kBodyTrackingAsset = "/BodyTracking.asset";
		public static void UpdateAssetBodyTracking(bool importedBodyTrackingPackage)
		{
			PackageEssenceAsset asset = null;
			if (File.Exists(WaveEssencePath + kBodyTrackingAsset))
			{
				asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kBodyTrackingAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedBodyTrackingPackage = importedBodyTrackingPackage;
			}
			else
			{
				asset = ScriptableObject.CreateInstance(typeof(PackageEssenceAsset)) as PackageEssenceAsset;
				asset.importedBodyTrackingPackage = importedBodyTrackingPackage;
				AssetDatabase.CreateAsset(asset, WaveEssencePath + kBodyTrackingAsset);
			}
			AssetDatabase.SaveAssets();
			Debug.Log("UpdateAssetBodyTracking() " + WaveEssencePath + kBodyTrackingAsset + ", importedBodyTrackingPackage: " + asset.importedBodyTrackingPackage);
		}
		internal static bool IsBodyTrackingPackageOnceImported()
		{
			if (!File.Exists(WaveEssencePath + kBodyTrackingAsset))
				return false;

			PackageEssenceAsset asset = AssetDatabase.LoadAssetAtPath(WaveEssencePath + kBodyTrackingAsset, typeof(PackageEssenceAsset)) as PackageEssenceAsset;
			return asset.importedBodyTrackingPackage;
		}
		#endregion

		private static readonly string[] essenceKeywords = new string[]
		{
			"Wave",
			"Essence",
			"Controller",
			"InputModule",
			"RenderDoc",
			"Hand",
			"Interaction",
			"CompositorLayer",
			"XR",
			"Tracker",
			"ScenePerception",
			"TrackableMarker",
			"URPMaterials",
			"Spectator",
			"BodyTracking",
		};

		internal static UnityEditor.PackageManager.PackageInfo pi = null;

		public EssenceSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
			: base(path, scope, essenceKeywords)
		{
			CheckingWavePackagePath();
			pi = SearchInPackageList(Constants.EssencePackageName);
		}

		internal static void Init()
		{
			CheckingWavePackagePath();
			pi = SearchInPackageList(Constants.EssencePackageName);
		}

		private const string FAKE_VERSION = "0.0.0";

		internal const string kControllerModelPath = "/Controller/Model";
		internal const string kControllerModelPackage = "wave_essence_controller_model.unitypackage";
		internal const string kInputModulePath = "/InputModule";
		internal const string kInputModulePackage = "wave_essence_inputmodule.unitypackage";
		internal const string kHandModelPath = "/Hand/Model";
		internal const string kHandModelPackage = "wave_essence_hand_model.unitypackage";
		internal const string kInteractionModePath = "/Interaction/Mode";
		internal const string kInteractionModePackage = "wave_essence_interaction_mode.unitypackage";
		internal const string kInteractionToolkitPath = "/Interaction/Toolkit";
		internal const string kInteractionToolkitPackage = "wave_essence_interaction_toolkit.unitypackage";
		internal const string kCameraTexturePath = "/CameraTexture/";
		internal const string kCameraTexturePackage = "wave_essence_cameratexture.unitypackage";
		internal const string kCompositorLayerPath = "/CompositorLayer";
		internal const string kCompositorLayerPackage = "wave_essence_compositorlayer.unitypackage";
		internal const string kBundlePreviewPath = "/BundlePreview";
		internal const string kBundlePreviewPackage = "wave_essence_bundlepreview.unitypackage";
		internal const string kRenderDocPath = "/RenderDoc/";
		internal const string kRenderDocPackage = "wave_essence_renderdoc.unitypackage";
		internal const string kTrackerModelPath = "/Tracker/Model";
		internal const string kTrackerModelPackage = "wave_essence_tracker_model.unitypackage";
		internal const string kScenePerceptionPath = "/ScenePerception";
		internal const string kScenePerceptionPackage = "wave_essence_sceneperception.unitypackage";
		internal const string kTrackableMarkerPath = "/TrackableMarker";
		internal const string kTrackableMarkerPackage = "wave_essence_trackablemarker.unitypackage";
        internal const string kURPMaterialsPackage = "wave_essence_urpmaterials.unitypackage";
		internal const string kSpectatorPath = "/Spectator";
		internal const string kSpectatorPackage = "wave_essence_spectator.unitypackage";
		internal const string kBodyTrackingPath = "/BodyTracking";
		internal const string kBodyTrackingPackage = "wave_essence_bodytracking.unitypackage";
		internal const string kVrm1Package = "VRM-0.109.0_7aff.unitypackage";

		internal static bool featureControllerModelImported = false;
		internal static bool featureInputModuleImported = false;
		internal static bool featureHandModelImported = false;
		internal static bool featureInteractionModeImported = false;
		internal static bool featureInteractionToolkitImported = false;
		internal static bool featureCameraTextureImported = false;
		internal static bool featureCompositorLayerImported = false;
		internal static bool featureBundlePreviewImported = false;
		internal static bool featureRenderDocImported = false;
		internal static bool featureTrackerModelImported = false;
		internal static bool featureScenePerceptionImported = false;
		internal static bool featureTrackableMarkerImported = false;
		internal static bool featureSpectatorImported = false;
		internal static bool featureBodyTrackingImported = false;

		internal static bool featureControllerModelNeedUpdate = false;
		internal static bool featureInputModuleNeedUpdate = false;
		internal static bool featureHandModelNeedUpdate = false;
		internal static bool featureInteractionModeNeedUpdate = false;
		internal static bool featureInteractionToolkitNeedUpdate = false;
		internal static bool featureCameraTextureNeedUpdate = false;
		internal static bool featureCompositorLayerNeedUpdate = false;
		internal static bool featureBundlePreviewNeedUpdate = false;
		internal static bool featureRenderDocNeedUpdate = false;
		internal static bool featureTrackerModelNeedUpdate = false;
		internal static bool featureScenePerceptionNeedUpdate = false;
		internal static bool featureTrackableMarkerNeedUpdate = false;
		internal static bool featureSpectatorNeedUpdate = false;
		internal static bool featureBodyTrackingNeedUpdate = false;

		internal static bool hasFeatureNeedUpdate = false;

		internal static bool checkFeaturePackages()
		{
			featureControllerModelImported = Directory.Exists(WaveEssencePath + kControllerModelPath);
			featureInputModuleImported = Directory.Exists(WaveEssencePath + kInputModulePath);
			featureHandModelImported = Directory.Exists(WaveEssencePath + kHandModelPath);
			featureInteractionModeImported = Directory.Exists(WaveEssencePath + kInteractionModePath);
			featureInteractionToolkitImported = Directory.Exists(WaveEssencePath + kInteractionToolkitPath);
			featureCameraTextureImported = Directory.Exists(WaveEssencePath + kCameraTexturePath);
			featureCompositorLayerImported = Directory.Exists(WaveEssencePath + kCompositorLayerPath);
			featureBundlePreviewImported = Directory.Exists(WaveEssencePath + kBundlePreviewPath);
			featureRenderDocImported = Directory.Exists(WaveEssencePath + kRenderDocPath);
			featureTrackerModelImported = Directory.Exists(WaveEssencePath + kTrackerModelPath);
			featureScenePerceptionImported = Directory.Exists(WaveEssencePath + kScenePerceptionPath);
			featureTrackableMarkerImported = Directory.Exists(WaveEssencePath + kTrackableMarkerPath);
			featureSpectatorImported = Directory.Exists(WaveEssencePath + kSpectatorPath);
			featureBodyTrackingImported = Directory.Exists(WaveEssencePath + kBodyTrackingPath);

			if (pi == null)
				return false;

			featureControllerModelNeedUpdate = featureControllerModelImported && !Directory.Exists(WaveEssencePath + kControllerModelPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kControllerModelPath + "/" + FAKE_VERSION);
			featureInputModuleNeedUpdate = featureInputModuleImported && !Directory.Exists(WaveEssencePath + kInputModulePath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kInputModulePath + "/" + FAKE_VERSION);
			featureHandModelNeedUpdate = featureHandModelImported && !Directory.Exists(WaveEssencePath + kHandModelPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kHandModelPath + "/" + FAKE_VERSION);
			featureInteractionModeNeedUpdate = featureInteractionModeImported && !Directory.Exists(WaveEssencePath + kInteractionModePath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kInteractionModePath + "/" + FAKE_VERSION);
			featureInteractionToolkitNeedUpdate = featureInteractionToolkitImported && !Directory.Exists(WaveEssencePath + kInteractionToolkitPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kInteractionToolkitPath + "/" + FAKE_VERSION);
			featureCameraTextureNeedUpdate = featureCameraTextureImported && !Directory.Exists(WaveEssencePath + kCameraTexturePath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kCameraTexturePath + "/" + FAKE_VERSION);
			featureCompositorLayerNeedUpdate = featureCompositorLayerImported && !Directory.Exists(WaveEssencePath + kCompositorLayerPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kCompositorLayerPath + "/" + FAKE_VERSION);
			featureBundlePreviewNeedUpdate = featureBundlePreviewImported && !Directory.Exists(WaveEssencePath + kBundlePreviewPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kBundlePreviewPath + "/" + FAKE_VERSION);
			featureRenderDocNeedUpdate = featureRenderDocImported && !Directory.Exists(WaveEssencePath + kRenderDocPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kRenderDocPath + "/" + FAKE_VERSION);
			featureTrackerModelNeedUpdate = featureTrackerModelImported && !Directory.Exists(WaveEssencePath + kTrackerModelPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kTrackerModelPath + "/" + FAKE_VERSION);
			featureScenePerceptionNeedUpdate = featureScenePerceptionImported && !Directory.Exists(WaveEssencePath + kScenePerceptionPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kScenePerceptionPath + "/" + FAKE_VERSION);
			featureTrackableMarkerNeedUpdate = featureTrackableMarkerImported && !Directory.Exists(WaveEssencePath + kTrackableMarkerPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kTrackableMarkerPath + "/" + FAKE_VERSION);
			featureSpectatorNeedUpdate = featureSpectatorImported && !Directory.Exists(WaveEssencePath + kSpectatorPath + "/" + pi.version) && !Directory.Exists(WaveEssencePath + kSpectatorPath + "/" + FAKE_VERSION);
			featureBodyTrackingNeedUpdate = featureBodyTrackingImported && !Directory.Exists(WaveEssencePath + kBodyTrackingPath + "/" + pi.version) &&
				!Directory.Exists(WaveEssencePath + kBodyTrackingPath + "/" + FAKE_VERSION);

			hasFeatureNeedUpdate = featureControllerModelNeedUpdate || featureInputModuleNeedUpdate || featureHandModelNeedUpdate || featureInteractionModeNeedUpdate || featureInteractionToolkitNeedUpdate ||
				featureCameraTextureNeedUpdate || featureCompositorLayerNeedUpdate || featureBundlePreviewNeedUpdate || featureRenderDocNeedUpdate || featureScenePerceptionNeedUpdate || featureTrackableMarkerNeedUpdate || featureSpectatorNeedUpdate || featureBodyTrackingNeedUpdate;

			return hasFeatureNeedUpdate;
		}

		public static void UpdateAllModules()
		{
			checkFeaturePackages();
			if (featureControllerModelNeedUpdate)
				UpdateModule(WaveEssencePath + kControllerModelPath, kControllerModelPackage);
			if (featureInputModuleNeedUpdate)
				UpdateModule(WaveEssencePath + kInputModulePath, kInputModulePackage);
			if (featureHandModelNeedUpdate)
				UpdateModule(WaveEssencePath + kHandModelPath, kHandModelPackage);
			if (featureInteractionModeNeedUpdate)
				UpdateModule(WaveEssencePath + kInteractionModePath, kInteractionModePackage);
			if (featureCameraTextureNeedUpdate)
				UpdateModule(WaveEssencePath + kCameraTexturePath, kCameraTexturePackage);
			if (featureCompositorLayerNeedUpdate)
				UpdateModule(WaveEssencePath + kCompositorLayerPath, kCompositorLayerPackage);
			if (featureBundlePreviewNeedUpdate)
				UpdateModule(WaveEssencePath + kBundlePreviewPath, kBundlePreviewPackage);
			if (featureRenderDocNeedUpdate)
				UpdateModule(WaveEssencePath + kRenderDocPath, kRenderDocPackage);
			if (featureInteractionToolkitNeedUpdate)
				UpdateModule(WaveEssencePath + kInteractionToolkitPath, kInteractionToolkitPackage);
			if (featureTrackerModelNeedUpdate)
				UpdateModule(WaveEssencePath + kTrackerModelPath, kTrackerModelPackage);
			if (featureScenePerceptionNeedUpdate)
				UpdateModule(WaveEssencePath + kScenePerceptionPath, kScenePerceptionPackage);
			if (featureTrackableMarkerNeedUpdate)
				UpdateModule(WaveEssencePath + kTrackableMarkerPath, kTrackableMarkerPackage);
			if (featureSpectatorNeedUpdate)
				UpdateModule(WaveEssencePath + kSpectatorPath, kSpectatorPackage);
			if (featureBodyTrackingNeedUpdate)
				UpdateModule(WaveEssencePath + kBodyTrackingPath, kBodyTrackingPackage);
		}

		public override void OnGUI(string searchContext)
		{
			bool hasKeyword = false;
			bool showControllerModel = searchContext.Contains("Controller");
			bool showInputModule = searchContext.Contains("InputModule");
			bool showHandModel = searchContext.Contains("Hand");
			bool showInteractionMode = searchContext.Contains("Interaction");
			bool showCameraTexture = searchContext.Contains("CameraTexture");
			bool showCompositorLayer = searchContext.Contains("CompositorLayer");
			bool showBundlePreview = searchContext.Contains("BundlePreview");
			bool showRenderDoc = searchContext.Contains("RenderDoc");
			bool showInteractionToolkit = searchContext.Contains("Interaction");
			bool showTrackerModel = searchContext.Contains("Tracker");
			bool showScenePerception = searchContext.Contains("ScenePerception");
			bool showTrackableMarker = searchContext.Contains("TrackableMarker");
			bool showURPMaterials = searchContext.Contains("URPMaterials");
			bool showSpectator = searchContext.Contains("Spectator");
			bool showBodyTracking = searchContext.Contains("BodyTracking");

			if (showControllerModel ||
				showInputModule ||
				showHandModel ||
				showInteractionMode ||
				showCameraTexture ||
				showCompositorLayer ||
				showBundlePreview ||
				showRenderDoc ||
				showInteractionToolkit ||
				showTrackerModel ||
				showScenePerception ||
				showTrackableMarker ||
				showURPMaterials ||
				showSpectator ||
				showBodyTracking)
			{
				hasKeyword = true;
			}

			/**
             * GUI layout of features.
             * 1. Controller Model
             * 2. Input Module
             * 3. Hand Model
			 * 4. Ineraction Mode
             * 5. Camera Texture
			 * 6. Compositor Layer
			 * 7. BundlePreview
             * 8. RenderDoc
             * 9. Interaction Toolkit
             * 10. Tracker Model
             * 11. Scene Perception
             * 12. Trackable Marker
             * 13. URP Materials
			 * 14. Spectator
             * 15. Body Tracking
             **/

			checkFeaturePackages();

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUILayout.Label("Check Packages", EditorStyles.boldLabel);
				GUILayout.Label("Checking if any packges need update.", EditorStyles.label);
				GUILayout.Space(5f);
				if (GUILayout.Button("Check packages", GUILayout.ExpandWidth(false)))
					EssenseSettingsConfigDialog.ShowDialog();

                bool b = EditorPrefs.GetBool("EssenceNotifyUpdatePackageSkip", false);
                if (GUILayout.Toggle(b, "Do not auto check package update"))
                {
                    EditorPrefs.SetBool("EssenceNotifyUpdatePackageSkip", true);
                }
                else
                {
                    EditorPrefs.SetBool("EssenceNotifyUpdatePackageSkip", false);
                }
                GUILayout.Space(5f);
			}
			GUILayout.EndVertical();

			if (!PackageInfo.IsImporting &&
				!featureControllerModelImported && !IsControllerModelPackageOnceImported())
			{
				PackageInfo.IsImporting = true;
				UpdateAssetControllerModel(true);
				ImportModule(kControllerModelPackage);
			}
			if (showControllerModel || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Controller Model", EditorStyles.boldLabel);
					GUILayout.Label("This feature is imported by default.\n\n" +
						"This package provides features of render model, button effect and controller tips. \n" +
						"Please import XR interaction toolkit and refer Demo scene to check how to use it.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/Controller/Model.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureControllerModelImported || featureControllerModelNeedUpdate;
					if (featureControllerModelNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Controller Model", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kControllerModelPath, kControllerModelPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Controller Model", GUILayout.ExpandWidth(false)))
							ImportModule(kControllerModelPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (!PackageInfo.IsImporting &&
				!featureInputModuleImported && !IsInputModulePackageOnceImported())
			{
				PackageInfo.IsImporting = true;
				UpdateAssetInputModule(true);
				ImportModule(kInputModulePackage);
			}
			if (showInputModule || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Input Module", EditorStyles.boldLabel);
					GUILayout.Label("This feature is imported by default.\n\n" +
						"The Input Module feature provides a controller input module and a gaze input module. In the demo you will see how to interact with scene objects.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/InputModule.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureInputModuleImported || featureInputModuleNeedUpdate;
					if (featureInputModuleNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Input Module", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kInputModulePath, kInputModulePackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Input Module", GUILayout.ExpandWidth(false)))
							ImportModule(kInputModulePackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (!PackageInfo.IsImporting &&
				!featureHandModelImported && !IsHandModelPackageOnceImported())
			{
				PackageInfo.IsImporting = true;
				UpdateAssetHandModel(true);
				ImportModule(kHandModelPackage);
			}
			if (showHandModel || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Hand Model", EditorStyles.boldLabel);
					GUILayout.Label("This feature is imported by default.\n\n" +
						"The Hand Model feature provides the models of hand.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/Hand/Model.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureHandModelImported || featureHandModelNeedUpdate;
					if (featureHandModelNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Hand Model", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kHandModelPath, kHandModelPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Hand Model", GUILayout.ExpandWidth(false)))
							ImportModule(kHandModelPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (!PackageInfo.IsImporting &&
				featureControllerModelImported && featureInputModuleImported && featureHandModelImported &&
				!featureInteractionModeImported && !IsInteractionModePackageOnceImported())
			{
				PackageInfo.IsImporting = true;
				UpdateAssetInteractionMode(true);
				ImportModule(kInteractionModePackage);
			}
			if (showInteractionMode || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Interaction Mode", EditorStyles.boldLabel);
					GUILayout.Label("This feature is imported by default.\n" +
						"If you want to import this feature manually, you have to import \"Controller Model\", \"Input Module\" and \"Hand Model\" first.\n\n" +
						"There are three modes provided by Wave plugin: \n" +
						"- Gaze: A player will use gaze for interaction.\n" +
						"- Controller: A player will use controllers for interaction.\n" +
						"- Hand: A player will use his hands for interaction.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/Interaction/Mode.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = (!featureInteractionModeImported || featureInteractionModeNeedUpdate) && featureControllerModelImported && featureInputModuleImported && featureHandModelImported;
					if (featureInteractionModeNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Interaction Mode", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kInteractionModePath, kInteractionModePackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Interaction Mode", GUILayout.ExpandWidth(false)))
							ImportModule(kInteractionModePackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showCameraTexture || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				GUILayout.Label("CameraTexture", EditorStyles.boldLabel);
				GUILayout.Label("This feature is deprecated and is no longer supported.", new GUIStyle(EditorStyles.label) { wordWrap = true });
				GUILayout.Space(5f);
				if (featureCameraTextureNeedUpdate)
				{
					if (GUILayout.Button("Update Feature - CameraTexture", GUILayout.ExpandWidth(false)))
						UpdateModule(WaveEssencePath + kCameraTexturePath, kCameraTexturePackage);
				}
				else
				{
					if (GUILayout.Button("Import Feature - CameraTexture", GUILayout.ExpandWidth(false)))
						ImportModule(kCameraTexturePackage);
				}
				GUILayout.EndVertical();
			}

			if (showCompositorLayer || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Compositor Layer", EditorStyles.boldLabel);
					GUILayout.Label("This feature leverages the Wave Multi-Layer Rendering Architecture to display textures on layers other than the eye buffer.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/CompositorLayer.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureCompositorLayerImported || featureCompositorLayerNeedUpdate;
					if (featureCompositorLayerNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Compositor Layer", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kCompositorLayerPath, kCompositorLayerPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Compositor Layer", GUILayout.ExpandWidth(false)))
							ImportModule(kCompositorLayerPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showBundlePreview || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Bundle Preview", EditorStyles.boldLabel);
					GUILayout.Label("Bundle Preview allows you to quickly preview project changes by modularizing the project building process. \n" +
						"Select Wave/BundlePreview in the menu to start using this feature.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/BundlePreview.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureBundlePreviewImported || featureBundlePreviewNeedUpdate;
					if (featureBundlePreviewNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - BundlePreview", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kBundlePreviewPath, kBundlePreviewPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - BundlePreview", GUILayout.ExpandWidth(false)))
							ImportModule(kBundlePreviewPackage);
					}
					
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showRenderDoc || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				string renderDocLabel =
					"Developer can check out the graphic's detail problem with RenderDoc profiling tool.  " +
					"This tool is integrated within Wave's XR plugin.  " +
					"In this package, provide a basic class and sample.  " +
					"Because RenderDoc will cost performance, you can remove the imported content after your test.";
				GUILayout.Label("RenderDoc", EditorStyles.boldLabel);
				GUILayout.Label(renderDocLabel, new GUIStyle(EditorStyles.label) { wordWrap = true });
				GUILayout.Space(5f);
				GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/RenderDoc.", EditorStyles.label);
				if (featureRenderDocNeedUpdate)
				{
					if (GUILayout.Button("Update RenderDoc tool", GUILayout.ExpandWidth(false)))
						UpdateModule(WaveEssencePath + kRenderDocPath, kRenderDocPackage);
				}
				else
				{
					if (GUILayout.Button("Import RenderDoc tool", GUILayout.ExpandWidth(false)))
						ImportModule(kRenderDocPackage);
				}
				GUILayout.EndVertical();
			}

			if (showInteractionToolkit || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Interaction Toolkit", EditorStyles.boldLabel);
					GUILayout.Label(
						"The Wave Extension of Unity XR Interaction Toolkit.\n" +
						"Do NOT use this package without Unity XR Interaction Toolkit!\n" +
						"Do NOT use this package without Unity XR Interaction Toolkit!\n" +
						"Do NOT use this package without Unity XR Interaction Toolkit!\n" +
						"Do NOT use this package without Unity XR Interaction Toolkit!\n" +
						"Do NOT use this package without Unity XR Interaction Toolkit!",
						new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/Interaction/Toolkit.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureInteractionToolkitImported | featureInteractionToolkitNeedUpdate;
					if (featureInteractionToolkitNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Interaction Toolkit", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kInteractionToolkitPath, kInteractionToolkitPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Interaction Toolkit", GUILayout.ExpandWidth(false)))
							ImportModule(kInteractionToolkitPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showTrackerModel || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Tracker Model", EditorStyles.boldLabel);
					GUILayout.Label("The Tracker Model package provides the sample and model of Tracker feature.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + "/Tracker/Model.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureTrackerModelImported || featureTrackerModelNeedUpdate;

					if (featureTrackerModelNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Tracker Model", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kTrackerModelPath, kTrackerModelPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Tracker Model", GUILayout.ExpandWidth(false)))
							ImportModule(kTrackerModelPackage);
					}

					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showScenePerception || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Scene Perception", EditorStyles.boldLabel);
					GUILayout.Label("Note: This feature is currenty in Beta.\n" +
									"Scene Perception is feature which facilitates Mixed Reality development by bringing in spatial information from the users' surroundings into the virtual environment.\n" +
									"The aspects of this feature that are currently supported are Scene Planes, Scene Mesh and Spatial Anchors.", new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + kScenePerceptionPath, EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureScenePerceptionImported || featureScenePerceptionNeedUpdate;
					if (featureScenePerceptionNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Scene Perception", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kScenePerceptionPath, kScenePerceptionPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Scene Perception", GUILayout.ExpandWidth(false)))
							ImportModule(kScenePerceptionPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showTrackableMarker || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Trackable Marker", EditorStyles.boldLabel);
					GUILayout.Label("Note: This feature is currenty in Beta.\n" +
									"Trackable Marker is feature which enables you to track markers detected by the device."
									, new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + kTrackableMarkerPath, EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureTrackableMarkerImported || featureTrackableMarkerNeedUpdate;
					if (featureTrackableMarkerNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Trackable Marker", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kTrackableMarkerPath, kTrackableMarkerPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Trackable Marker", GUILayout.ExpandWidth(false)))
							ImportModule(kTrackableMarkerPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

            if (GraphicsSettings.renderPipelineAsset != null && (showURPMaterials || !hasKeyword))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("URP materials", EditorStyles.boldLabel);
                    GUILayout.Label("These materials will overwrite the original contents."
                                    , new GUIStyle(EditorStyles.label) { wordWrap = true });
                    GUILayout.Label("The feature will be imported at everywhere", EditorStyles.label);
                    GUILayout.Space(5f);
                    if (GUILayout.Button("Import Feature - URP Materials", GUILayout.ExpandWidth(false)))
                        ImportModule(kURPMaterialsPackage, true);
                    GUILayout.Space(5f);
                    GUI.enabled = true;
                }
                GUILayout.EndVertical();
            }

			if (showSpectator || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Spectator", EditorStyles.boldLabel);
					GUILayout.Label("Note: This feature is currently in Beta.\n" +
					                "Spectator is a feature that enables you to set the spectator camera to record the VR scene freely.\n"
						, new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("The feature will be imported at " + WaveEssencePath + kSpectatorPath, EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = !featureSpectatorImported || featureSpectatorNeedUpdate;
					if (featureSpectatorNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Spectator", GUILayout.ExpandWidth(false)))
						{
							// To check wherever the Spectator setup correctly in the Wave setting
							WaveXRSettings waveXRSettings = WaveXRSettings.GetInstance();
							if (waveXRSettings != null)
							{
								if (waveXRSettings.allowSpectatorCamera == false)
								{
									waveXRSettings.allowSpectatorCamera = true;
								}
								UpdateModule(WaveEssencePath + kSpectatorPath, kSpectatorPackage);
							}
							else
							{
								Debug.LogError("Import Spectator feature fail because cannot get the WaveXR" +
								               "setting, please try again later.");
							}
						}
					}
					else
					{
						if (GUILayout.Button("Import Feature - Spectator", GUILayout.ExpandWidth(false)))
							ImportModule(kSpectatorPackage);
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}

			if (showBodyTracking || !hasKeyword)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					GUILayout.Label("Body Tracking", EditorStyles.boldLabel);
					GUILayout.Label(
						"The Body Tracking feature depends on Humanoid VRM plugin.\n" +
						"You can import the Humanoid VRM package (VRM-0.109.0_7aff.unitypackage) at\n" +
						"Library > PackageCache > com.htc.upm.wave.essence > UnityPackages~",
						new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label(
						"Note: Must using Unity Editor 2020.3.30f1 or newer version.",
						new GUIStyle(EditorStyles.label) { wordWrap = true });
					GUILayout.Label("This feature will be imported at " + WaveEssencePath + "/BodyTracking.\n" +
						"Import the Tracker Model package first before using this feature.", EditorStyles.label);
					GUILayout.Space(5f);
					GUI.enabled = (!featureBodyTrackingImported || featureBodyTrackingNeedUpdate) && featureTrackerModelImported;
					if (featureBodyTrackingNeedUpdate)
					{
						if (GUILayout.Button("Update Feature - Body Tracking", GUILayout.ExpandWidth(false)))
							UpdateModule(WaveEssencePath + kBodyTrackingPath, kBodyTrackingPackage);
					}
					else
					{
						if (GUILayout.Button("Import Feature - Body Tracking", GUILayout.ExpandWidth(false)))
						{
							//ImportModule(kVrm1Package);
							ImportModule(kBodyTrackingPackage);
						}
					}
					GUILayout.Space(5f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}
        }

		public static void DeleteFolder(string path)
		{
			if (Directory.Exists(path))
			{
				var files = Directory.GetFiles(path);
				var dirs = Directory.GetDirectories(path);
				foreach (var file in files)
				{
					File.Delete(file);
				}
				foreach (var dir in dirs)
				{
					Directory.Delete(dir, true);
				}
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
					else if (Directory.EnumerateFiles(destdir).Any()) // allow two layers
					{
						var dirname = Path.GetFileName(dir);
						var srcpath2 = srcpath + "/" + dirname;
						var destpath2 = destpath + "/" + dirname;
						var srcfiles2 = Directory.GetFiles(srcpath2);
						var srcdirs2 = Directory.GetDirectories(srcpath2);
						foreach (var file2 in srcfiles2)
						{
							string destfile2 = destpath2 + file2.Substring(srcpath2.Length, file2.Length - srcpath2.Length);
							if (!File.Exists(destfile2))
								File.Move(file2, destfile2);
							else // layer 2 force write
							{
								File.Delete(destfile2);
								File.Move(file2, destfile2);
							}
						}
						foreach (var dir2 in srcdirs2)
						{
							string destdir2 = destpath2 + dir2.Substring(srcpath2.Length, dir2.Length - srcpath2.Length);
							if (!Directory.Exists(destdir2))
								Directory.Move(dir2, destdir2);
							else // layer 2 force write
							{
								Directory.Delete(destdir2, true);
								Directory.Move(dir2, destdir2);
							}
						}
						Directory.Delete(dir, true);
						File.Delete(dir + ".meta");
					}
					else
					{
						Directory.Delete(destdir);
						Directory.Move(dir, destdir);
					}
				}
			}
		}

		public static void CleanHouse()
		{
			if (Directory.Exists("Assets/Wave/Essence"))
				Directory.Delete("Assets/Wave/Essence");
			if (File.Exists("Assets/Wave/Essence.meta"))
				File.Delete("Assets/Wave/Essence.meta");
			if (Directory.Exists("Assets/Wave"))
				Directory.Delete("Assets/Wave");
			if (File.Exists("Assets/Wave.meta"))
				File.Delete("Assets/Wave.meta");
		}

		internal static void UpdateModule(string ModelPath, string packagePath)
		{
			DeleteFolder(ModelPath);
			AssetDatabase.Refresh();
			string target = Path.Combine("Packages/" + Constants.EssencePackageName + "/UnityPackages~", packagePath);
			Debug.Log("Import: " + target);
			AssetDatabase.ImportPackage(target, false);
		}

		internal static void ImportModule(string packagePath, bool interactive = false)
		{
			string target = Path.Combine("Packages/" + Constants.EssencePackageName + "/UnityPackages~", packagePath);
			Debug.Log("Import: " + target);
			AssetDatabase.ImportPackage(target, interactive);
		}

		[SettingsProvider]
		static SettingsProvider Create()
		{
			Debug.Log("Create EssenceSettingsProvider");
			return new EssenceSettingsProvider("Project/Wave XR/Essence");
		}

		private static UnityEditor.PackageManager.PackageInfo SearchInPackageList(string packageName)
		{
			var listRequest = Client.List(true);
			do
			{
				if (listRequest.IsCompleted)
				{
					if (listRequest.Result == null)
					{
						Debug.Log("List result: is empty");
						return null;
					}

					foreach (var pi in listRequest.Result)
					{
						//Debug.Log("List has: " + pi.name + " == " + packageName);
						if (pi.name == packageName)
						{
							Debug.Log("Found " + packageName);

							return pi;
						}
					}
					break;
				}
				Thread.Sleep(100);
			} while (true);
			return null;
		}
	} // class EssenceSettingProvider

	[InitializeOnLoad]
	public class PackageInfo : AssetPostprocessor
	{
		public static bool IsImporting = false;

		static PackageInfo()
		{
			Debug.Log("PackageInfo()");
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
			EssenceSettingsProvider.Init();
			EssenceSettingsProvider.checkFeaturePackages();

			if (packagename.Equals("UnityPackages~\\wave_essence_controller_model"))
			{
				if (!EssenceSettingsProvider.featureInputModuleImported && !EssenceSettingsProvider.IsInputModulePackageOnceImported())
				{
					EssenceSettingsProvider.UpdateAssetInputModule(true);
					EssenceSettingsProvider.ImportModule(EssenceSettingsProvider.kInputModulePackage);
				}
				else
					IsImporting = false;
			}
			else if (packagename.Equals("UnityPackages~\\wave_essence_inputmodule"))
			{
				if(!EssenceSettingsProvider.featureHandModelImported && !EssenceSettingsProvider.IsHandModelPackageOnceImported())
				{
					EssenceSettingsProvider.UpdateAssetHandModel(true);
					EssenceSettingsProvider.ImportModule(EssenceSettingsProvider.kHandModelPackage);
				}
				else
					IsImporting = false;
			}
			else if (packagename.Equals("UnityPackages~\\wave_essence_hand_model"))
			{
				if (EssenceSettingsProvider.featureControllerModelImported && EssenceSettingsProvider.featureInputModuleImported &&
					EssenceSettingsProvider.featureHandModelImported && !EssenceSettingsProvider.featureInteractionModeImported &&
					!EssenceSettingsProvider.IsInteractionModePackageOnceImported())
				{
					EssenceSettingsProvider.UpdateAssetInteractionMode(true);
					EssenceSettingsProvider.ImportModule(EssenceSettingsProvider.kInteractionModePackage);
				}
				else
					IsImporting = false;
			}
			else
			    IsImporting = false;
			if (!EssenceSettingsProvider.WaveEssencePath.Equals("Assets/Wave/Essence"))
				MoveEssenceFolder();
		}

		public static void ResetToDefaultPackages()
		{
			EssenceSettingsProvider.UpdateAssetControllerModel(false);
			EssenceSettingsProvider.UpdateAssetInputModule(false);
			EssenceSettingsProvider.UpdateAssetHandModel(false);
			EssenceSettingsProvider.UpdateAssetInteractionMode(false);
			EssenceSettingsProvider.DeleteFolder(EssenceSettingsProvider.WaveEssencePath);
			AssetDatabase.Refresh();			
		}

		private static void MoveEssenceFolder()
		{
			if (Directory.Exists("Assets/Wave/Essence"))
			{
				EssenceSettingsProvider.MoveFolder("Assets/Wave/Essence", EssenceSettingsProvider.WaveEssencePath);
				EssenceSettingsProvider.CleanHouse();
				AssetDatabase.Refresh();
			}
		}
	}

	[InitializeOnLoad]
	public class EssenseSettingsConfigDialog : EditorWindow
	{
		List<Item> items;

		public class Item
		{
			const string currentValue = " (Need update = {0})";

			public delegate bool DelegateIsShow();
			public delegate bool DelegateIsReady();
			public delegate string DelegateGetCurrent();

			public DelegateIsShow IsShow;
			public DelegateIsReady IsReady;
			public DelegateGetCurrent GetCurrent;

			public string title { get; private set; }

			public Item(string title)
			{
				this.title = title;
			}

			// Return true when setting is not ready.
			public bool Show()
			{
				if (IsShow())
					GUILayout.Label(title + string.Format(currentValue, GetCurrent()));
				if (IsReady())
					return false;
				return true;
			}
		}

		static List<Item> GetItems()
		{
			var ControllerModel = new Item("Controller Model")
			{
				IsShow = () => { return EssenceSettingsProvider.featureControllerModelImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureControllerModelNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureControllerModelNeedUpdate.ToString(); },
			};

			var InputModule = new Item("Input Module")
			{
				IsShow = () => { return EssenceSettingsProvider.featureInputModuleImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureInputModuleNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureInputModuleNeedUpdate.ToString(); },
			};

			var HandModel = new Item("Hand Model")
			{
				IsShow = () => { return EssenceSettingsProvider.featureHandModelImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureHandModelNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureHandModelNeedUpdate.ToString(); },
			};

			var InteractionMode = new Item("Interaction Mode")
			{
				IsShow = () => { return EssenceSettingsProvider.featureInteractionModeImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureInteractionModeNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureInteractionModeNeedUpdate.ToString(); },
			};

			var CameraTexture = new Item("Camera Texture")
			{
				IsShow = () => { return EssenceSettingsProvider.featureCameraTextureImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureCameraTextureNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureCameraTextureNeedUpdate.ToString(); },
			};

			var CompositorLayer = new Item("Compositor Layer")
			{
				IsShow = () => { return EssenceSettingsProvider.featureCompositorLayerImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureCompositorLayerNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureCompositorLayerNeedUpdate.ToString(); },
			};

			var BundlePreview = new Item("Bundle Preview")
			{
				IsShow = () => { return EssenceSettingsProvider.featureBundlePreviewImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureBundlePreviewNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureBundlePreviewNeedUpdate.ToString(); },
			};

			var RenderDoc = new Item("Render Doc")
			{
				IsShow = () => { return EssenceSettingsProvider.featureRenderDocImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureRenderDocNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureRenderDocNeedUpdate.ToString(); },
			};

			var InteractionToolkit = new Item("Interaction Toolkit")
			{
				IsShow = () => { return EssenceSettingsProvider.featureInteractionToolkitImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureInteractionToolkitNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureInteractionToolkitNeedUpdate.ToString(); },
			};

			var ScenePerception = new Item("Scene Perception")
			{
				IsShow = () => { return EssenceSettingsProvider.featureCompositorLayerImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureCompositorLayerNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureCompositorLayerNeedUpdate.ToString(); },
			};

			var TrackableMarker = new Item("Trackable Marker")
			{
				IsShow = () => { return EssenceSettingsProvider.featureTrackableMarkerImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureTrackableMarkerNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureTrackableMarkerNeedUpdate.ToString(); },
			};

            var URPMaterials = new Item("URP Materials")
            {
                IsShow = () => { return true; },
                IsReady = () => { return true; },
                GetCurrent = () => { return "Need Manual Update"; },
            };

			var Spectator = new Item("Spectator")
			{
				IsShow = () => { return EssenceSettingsProvider.featureSpectatorImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureSpectatorNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureSpectatorNeedUpdate.ToString(); },
			};

			var BodyTracking = new Item("Body Tracking")
			{
				IsShow = () => { return EssenceSettingsProvider.featureBodyTrackingImported; },
				IsReady = () => { return !EssenceSettingsProvider.featureBodyTrackingNeedUpdate; },
				GetCurrent = () => { return EssenceSettingsProvider.featureBodyTrackingNeedUpdate.ToString(); },
			};

			return new List<Item>()
			{
				ControllerModel,
				InputModule,
				HandModel,
				InteractionMode,
				CameraTexture,
				CompositorLayer,
				BundlePreview,
				RenderDoc,
				InteractionToolkit,
				ScenePerception,
				TrackableMarker,
                URPMaterials,
				Spectator,
				BodyTracking,
			};
		}

		static EssenseSettingsConfigDialog window;

		static EssenseSettingsConfigDialog()
		{
			EditorApplication.update += Update;
		}

		public static void ShowDialog()
		{
			EssenceSettingsProvider.Init();
			EssenceSettingsProvider.checkFeaturePackages();
			var items = GetItems();
			UpdateInner(items, true);
		}

		static void Update()
		{
			Debug.Log("Check for Essense Settings Update.");
			if (EditorPrefs.GetBool("EssenceNotifyUpdatePackageSkip", false))
			{
				Debug.Log("Skip Essense Settings Update.  EssenceNotifyUpdatePackageSkip=true");
				EditorApplication.update -= Update;
				return;
			}
			EssenceSettingsProvider.Init();
			EssenceSettingsProvider.checkFeaturePackages();
			var items = GetItems();
			UpdateInner(items, false);

			EditorApplication.update -= Update;
		}

		public static void UpdateInner(List<Item> items, bool forceShow)
		{
			bool show = forceShow;
			if (!forceShow)
			{
				foreach (var item in items)
				{
					show |= !item.IsReady();
				}
			}

			if (show)
			{
				window = GetWindow<EssenseSettingsConfigDialog>(true);
				window.minSize = new Vector2(480, 240);
				window.items = items;
			}

			if (!EssenceSettingsProvider.featureControllerModelImported && !EssenceSettingsProvider.IsControllerModelPackageOnceImported())
			{
				PackageInfo.IsImporting = true;
				EssenceSettingsProvider.UpdateAssetControllerModel(true);
				EssenceSettingsProvider.ImportModule(EssenceSettingsProvider.kControllerModelPackage);
			}
		}

		Vector2 scrollPosition;

		public void OnGUI()
		{
			if (items == null)
				return;

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			int notReadyItems = 0;
			GUILayout.Label("List imported packages :", EditorStyles.boldLabel);
			foreach (var item in items)
			{
				if(item.Show())
					notReadyItems++;
			}

			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(5f);
				GUILayout.Label("Reset to Default Packages", EditorStyles.boldLabel);
				if (GUILayout.Button("Reset to default packages", GUILayout.ExpandWidth(false)))
				{
					if (EditorUtility.DisplayDialog("Reset to Default Packages", "Are you sure?", "Yes, Reset to Default Packages", "Cancel"))
					{
						PackageInfo.ResetToDefaultPackages();
						Close();
					}
				}
				GUILayout.Space(5f);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				bool b = EditorPrefs.GetBool("EssenceNotifyUpdatePackageSkip", false);
				if (GUILayout.Toggle(b, "Don't show me again"))
				{
					EditorPrefs.SetBool("EssenceNotifyUpdatePackageSkip", true);
				}
				else
				{
					EditorPrefs.SetBool("EssenceNotifyUpdatePackageSkip", false);
				}
			}

			if (notReadyItems > 0)
			{
				if (GUILayout.Button("Update All"))
				{
					EssenceSettingsProvider.UpdateAllModules();
					EditorUtility.DisplayDialog("Update All", "Update all packages!", "Ok");
					Close();
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
}
#endif
