// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

namespace Wave.Generic.Sample
{
	public class MasterSceneManager : MonoBehaviour
	{
		//private static string LOG_TAG = "MasterSceneManager";
		public static Stack previouslevel;
		public static MasterSceneManager Instance;
		public static GameObject bs, hs;

		private List<SceneData> Scenes {
			get {
				return VRTestAppScenes.Instance.scenesData;
			}
		}

		private static int scene_idx = 0;

		// Using C# reflection to add Interaction Toolkit's UI component.
		private void AddTrackedDeviceGraphicRaycaster(GameObject obj)
		{
			Type t = Type.GetType("UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster, Unity.XR.Interaction.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			if (t == null)
				return;
			Component renderModel = obj.AddComponent(t);
		}

		private void Awake()
		{
			while (!Scenes[scene_idx].isEntry)
			{
				scene_idx++;
			}

			if (Instance == null)
			{
				DontDestroyOnLoad(this);
				Instance = this;
				previouslevel = new Stack();
				bs = GameObject.Find("BackButton");
				if (bs != null)
				{
					AddTrackedDeviceGraphicRaycaster(bs);
					DontDestroyOnLoad(bs);
					bs.SetActive(false);
				}
				hs = GameObject.Find("HelpButton");
				if (hs != null)
				{
					AddTrackedDeviceGraphicRaycaster(hs);
					DontDestroyOnLoad(hs);
					hs.SetActive(false);
				}
			}
			else
			{
				previouslevel.Clear();
				if (bs != null)
					bs.SetActive (false);
				if (hs != null)
					hs.SetActive (false);
				GameObject dd = GameObject.Find("BackButton");
				if (dd != null)
					dd.SetActive (false);
				dd = GameObject.Find("HelpButton");
				if (dd != null)
					dd.SetActive (false);
			}

			GameObject ts = GameObject.Find("SceneText");
			if (ts != null)
			{
				Text sceneText = ts.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}

			GameObject ts2 = GameObject.Find("SceneText2");
			if (ts2 != null)
			{
				Text sceneText = ts2.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}

			{
				GameObject vrstring = GameObject.Find("UnityBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "Unity version: " + Application.unityVersion;
					}
				}
			}


			try
			{
				GameObject vrstring = GameObject.Find("CommitInfo");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						var commitInfo = Wave.Generic.CommitInfo.LoadFromResource();
						if (commitInfo != null)
							sceneText.text = "Commit Info: " + commitInfo.abbreviated_commit;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}


#if UNITY_EDITOR
			if (Application.isEditor)
				return;
#endif

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.BuildConfig");
				String buildtime = jc.GetStatic<String>("VR_VERSION");
				GameObject vrstring = GameObject.Find("VRBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "VR Client AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.permission.client.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("PermissionBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "Permission AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.ime.client.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("IMEBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "IME AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.unity.pidemo.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("PiDemoBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "PiDemo AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.unity.resdemo.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("ResDemoBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "ResDemo AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.unity.resindicator.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("ResIndicatorBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "ResIndicator AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			try
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.htc.vr.unity.BuildConfig");
				String buildtime = jc.GetStatic<String>("AAR_BUILDTIME");
				GameObject vrstring = GameObject.Find("UnityPluginBuildTime");
				if (vrstring != null)
				{
					Text sceneText = vrstring.GetComponent<Text>();
					if (sceneText != null)
					{
						sceneText.text = "Unity Plugin AAR: " + buildtime;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		public void ChangeToNext()
		{
			do
			{
				scene_idx++;
				if (scene_idx >= Scenes.Count)
					scene_idx = 0;
			} while (!Scenes[scene_idx].isEntry);

			GameObject ts = GameObject.Find("SceneText");
			if (ts != null)
			{
				Text sceneText = ts.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}

			GameObject ts2 = GameObject.Find("SceneText2");
			if (ts2 != null)
			{
				Text sceneText = ts2.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}
		}

		public void ChangeToPrevious()
		{
			do
			{
				scene_idx--;
				if (scene_idx < 0)
					scene_idx = Scenes.Count - 1;
			} while (!Scenes[scene_idx].isEntry);

			GameObject ts = GameObject.Find("SceneText");
			if (ts != null)
			{
				Text sceneText = ts.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}

			GameObject ts2 = GameObject.Find("SceneText2");
			if (ts2 != null)
			{
				Text sceneText = ts2.GetComponent<Text>();
				if (sceneText != null)
				{
					sceneText.text = Scenes[scene_idx].name;
				}
			}
		}

		public void LoadPrevious()
		{
			if (previouslevel.Count > 0)
			{
				string scene_name = previouslevel.Pop().ToString();
				if (previouslevel.Count != 0)
				{
					hs.SetActive (true);
				}
				SceneManager.LoadScene(scene_name);
			}
		}

		public void LoadScene()
		{
			string scene_path = Scenes[scene_idx].path;
			string scene = System.IO.Path.GetFileNameWithoutExtension(scene_path);
			bs.SetActive (true);
			LoadNext(scene, scene_path);
		}

		public void loadHelpScene()
		{
			string help_scene_name = SceneManager.GetActiveScene().name + "_Help";
			string help_scene_path = SceneManager.GetActiveScene().path + "_Help";
			LoadNext(help_scene_name, help_scene_path);
		}

		private void LoadNext(string scene, string scene_path)
		{
			Debug.Log("Load Scene:" + scene + " path:" + scene_path);
			previouslevel.Push(SceneManager.GetActiveScene().name);
			int buildId = SceneUtility.GetBuildIndexByScenePath(scene_path);
			if (buildId == -1)
				return;
			hs.SetActive(scene_path.Contains("_Help"));
			SceneManager.LoadScene(buildId);
		}

		private void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Single)
			{
				if (bs != null)
					bs.GetComponent<Canvas>().worldCamera = Camera.main;
				if (hs != null)
					hs.GetComponent<Canvas>().worldCamera = Camera.main;
			}
		}


		public void ChooseQuit()
		{
			Application.Quit();
		}
	}
}
