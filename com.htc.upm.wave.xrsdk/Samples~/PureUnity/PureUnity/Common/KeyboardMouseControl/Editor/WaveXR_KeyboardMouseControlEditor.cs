#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Wave.XR.Sample.KMC
{
	[InitializeOnLoad]
	public static class WaveXR_KeyboardMouseControlEditor
	{
		const string KmcEnableKey = "WXR_EnableKMC";

		// register an event handler when the class is initialized
		static WaveXR_KeyboardMouseControlEditor()
		{
			EditorApplication.playModeStateChanged += LogPlayModeState;
			isKMCEnabled = EditorPrefs.GetBool(KmcEnableKey, false);
		}

		private static void LogPlayModeState(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredPlayMode && isKMCEnabled)
			{
				SceneManager.sceneLoaded += OnSceneLoaded;
				Scene scene = SceneManager.GetActiveScene();
				if (scene.isLoaded)
					OnSceneLoaded(scene, LoadSceneMode.Single);
			}
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Additive)
				return;
			var obj = new GameObject("KMC");
			obj.AddComponent<WaveXR_KeyboardMouseControl>();
		}

		static bool isKMCEnabled = false;
		const string kmcMenuPath = "Wave/Enable KeyboardMouseControl";

		[MenuItem(kmcMenuPath, true)]
		private static bool AllowKeyboardMouseControlValidate()
		{
			isKMCEnabled = EditorPrefs.GetBool(KmcEnableKey, false);
			Menu.SetChecked(kmcMenuPath, isKMCEnabled);

			if (isKMCEnabled)
				EditorApplication.playModeStateChanged += LogPlayModeState;
			else
				EditorApplication.playModeStateChanged -= LogPlayModeState;
			return true;
		}

		[MenuItem(kmcMenuPath, false, 2)]
		private static void AllowKeyboardMouseControl()
		{
			isKMCEnabled = !isKMCEnabled;
			Menu.SetChecked(kmcMenuPath, isKMCEnabled);
			EditorPrefs.SetBool(KmcEnableKey, isKMCEnabled);
		}
	}
}
#endif
