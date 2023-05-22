// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using Wave.Essence.Render;
using Wave.XR.Settings;
using Wave.XR.Loader;
using Wave.Native;

namespace Wave.Essence
{
	public class WaveXR_RuntimeInitializeOnLoad : MonoBehaviour
	{
		static readonly string TAG = "WaveXRRuntimeOnInitialize";
		static bool isFirstScene = true;

		[RuntimeInitializeOnLoadMethod]
		static void OnRuntimeMethodLoad()
		{
			Debug.Log(TAG + ": OnRuntimeMethodLoad");
			if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null || XRGeneralSettings.Instance.Manager.activeLoader == null || XRGeneralSettings.Instance.Manager.activeLoader.GetType() != typeof(WaveXRLoader))
				return; //Don't create GO and script instance if active loader is not found or is not WaveXRLoader

			GameObject obj = new GameObject(TAG, typeof(WaveXR_RuntimeInitializeOnLoad));
			DontDestroyOnLoad(obj);
			isFirstScene = true;
		}

		private void Awake()
		{
			Debug.Log(TAG + ": Awake");
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneLoadActions();
			Debug.Log(TAG + ": OnSceneLoaded: " + scene.name);
		}

		private void SceneLoadActions()
		{
#if UNITY_EDITOR
            if (Application.isEditor)
                return;
#endif
            var settings = WaveXRSettings.GetInstance();
            if (settings == null)
            {
                Debug.Log(TAG + ": WaveXR settings instance is null");
                return;
            }

            if (settings.adaptiveQualityMode != WaveXRSettings.AdaptiveQualityMode.Disabled &&
                settings.AQ_SendQualityEvent && settings.useAQDynamicResolution)
			{
				if (DynamicResolution.instance == null)
					new GameObject("DynamicResolution", typeof(DynamicResolution));
			}

            if ((settings.amcMode == WaveXRSettings.AMCMode.Auto ||
                settings.amcMode == WaveXRSettings.AMCMode.Force_PMC) &&
                settings.amcModeConfirm > 0)
                WaveXR_AMCProcess.FindMainCamera();

			if (settings.enableFSE)
			{
				Debug.Log(TAG + ": WVR_SetFrameSharpnessEnhancementLevel to: " + settings.FSE_Level);
				Interop.WVR_SetFrameSharpnessEnhancementLevel(settings.FSE_Level);
			}

		}

		private void OnEnable()
		{
			Debug.Log(TAG + ": OnEnable");
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void Start()
		{
			Debug.Log(TAG + ": Start");
			if (isFirstScene) //Manually run SceneLoadActions actions in first scene as hooking OnSceneLoaded to delegate happens after first scene load
			{
				SceneLoadActions();
				isFirstScene = false;
			}
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Debug.Log(TAG + ": OnDisable");
		}
	}
}
