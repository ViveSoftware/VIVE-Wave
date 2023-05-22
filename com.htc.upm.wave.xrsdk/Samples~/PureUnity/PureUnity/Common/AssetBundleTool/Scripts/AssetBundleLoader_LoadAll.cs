// AssetBundleTool is designed for general Unity environment.

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundleTool
{
	public class AssetBundleLoader_LoadAll : MonoBehaviour
	{
		public string assetBundleRootPath = "";
		public string loadingSceneName = "";

		public GameObject LoadingIndicator = null;

		private bool hasSceneLoaded = false;
        
		public bool IsLoadingScenes { get; set; }

		private void Awake()
		{
			if (string.IsNullOrEmpty(assetBundleRootPath))
				assetBundleRootPath = Path.Combine(Application.persistentDataPath, "AssetBundles/Scenes");
			if (!Directory.Exists(assetBundleRootPath))
				Directory.CreateDirectory(assetBundleRootPath);
			Debug.Log("ABL: AssetBundlePathName: " + assetBundleRootPath);
			IsLoadingScenes = false;
		}

		private IEnumerator LoadSceneInAssetBundle(AssetBundle assetBundle)
		{
			StringBuilder sb = new StringBuilder("ABL: LoadSceneInAssetBundle: Scenes:");
			string[] scenePaths = assetBundle.GetAllScenePaths();
			foreach (string scenePath in scenePaths)
			{
				sb.AppendLine();
				sb.Append(scenePath);
			}
			Debug.Log(sb.ToString());

			foreach (string scenePath in scenePaths)
			{
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);
				var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				IsLoadingScenes = true;
				yield return async;
				hasSceneLoaded = true;
			}
		}

		static Dictionary<string, AssetBundle> dictionary = new Dictionary<string, AssetBundle>();
		//static List<AssetBundle> list = new List<AssetBundle>();

		private IEnumerator LoadingSceneControl(bool loadAction)
		{
			if (loadAction)
				yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
			else
				yield return SceneManager.UnloadSceneAsync(loadingSceneName);
		}

		IEnumerator LoadAll()
		{
			IsLoadingScenes = true;
			yield return LoadingSceneControl(true);

			Debug.Log("ABL: Start LoadAll()");
#if false
			StringBuilder sb = new StringBuilder("ABL: List all scenes:");
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				sb.AppendLine();
				var scene = SceneManager.GetSceneAt(i);
				sb.Append(i).Append(": ").Append(scene.name).Append(" loaded: ").Append(scene.isLoaded);
			}
			Debug.Log(sb.ToString());
#endif

			string[] files = Directory.GetFiles(assetBundleRootPath);
			if (files.Length == 0)
				Debug.Log("ABL: No assetbundle found");

			foreach (string name in files)
			{
				AssetBundle ab;
				bool loaded = dictionary.TryGetValue(name, out ab);
				if (!loaded)
				{
					if (!name.EndsWith(".assetbundle", true, CultureInfo.CurrentCulture))
						continue;
					Debug.Log("ABL: Start loading assetbundle: " + name);

					if (File.Exists(name))
					{
						var request = AssetBundle.LoadFromFileAsync(name);
						yield return request;
						ab = request.assetBundle;
					}

					if (ab != null)
					{
						Debug.Log("ABL: Assetbundle " + name + " is loaded");
						dictionary.Add(name, ab);
					}
					loaded = true;
				}

				if (ab == null)
					continue;

				yield return LoadSceneInAssetBundle(ab);
			}

			// Stop showing Loading...
			LoadingIndicatorControl(false);

			if (hasSceneLoaded)
			{
				yield return LoadingSceneControl(false);
			}
			IsLoadingScenes = false;
		}

		void LoadingIndicatorControl(bool activate)
		{
			if (LoadingIndicator)
			{
				if (activate)
				{
					if (!LoadingIndicator.activeInHierarchy)
						LoadingIndicator.SetActive(true);
				}
				else
				{
					if (LoadingIndicator.activeInHierarchy)
						LoadingIndicator.SetActive(false);
				}
			}
		}

		void Start()
		{
			StartCoroutine(LoadAll());
			LoadingIndicatorControl(true);
		}
	}
}
