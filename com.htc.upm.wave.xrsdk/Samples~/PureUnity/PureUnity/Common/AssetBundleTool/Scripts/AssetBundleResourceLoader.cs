// AssetBundleTool is designed for general Unity environment.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetBundleTool
{
	// Please not to load scene when AssetBUndleResourceLoader still loading.  Use IsLoading or IsLoaded to check.
	public class AssetBundleResourceLoader : MonoBehaviour
	{
		[Tooltip("For external AssetBundles, put them into this specified external folder at runtime.")]
		public string assetBundleRootPath = "";
		[Tooltip("For internal AssetBundles, put them into Assets/StreamingAssets.")]
		public List<string> streamingAssetBundle = new List<string>();

		public interface IResourceLoaderCallback {
			// All resource loaded
			void OnResourceLoaded(AssetBundle[] abs);

			// If still want to hold the resources in this ab, return true.
			bool BeforeResourceUnload(AssetBundle ab);

			void OnResourceUnloaded(AssetBundle ab);
		}

		public bool IsLoading { get; private set; }
		public static bool IsLoaded { get; private set; }

		private void Awake()
		{
			IsLoading = false;
			if (string.IsNullOrEmpty(assetBundleRootPath))
				assetBundleRootPath = Path.Combine(Application.persistentDataPath, "AssetBundles/Resources");
			if (!Directory.Exists(assetBundleRootPath))
				Directory.CreateDirectory(assetBundleRootPath);
			Debug.Log("ABL: AssetBundlePathName: " + assetBundleRootPath);
		}

		private void OnApplicationQuit()
		{
			foreach (var ab in dictionary.Values)
			{
				ab.Unload(true);
			}
		}

		static Dictionary<string, AssetBundle> dictionary = new Dictionary<string, AssetBundle>();

		static List<IResourceLoaderCallback> callbacks = new List<IResourceLoaderCallback>();

		public static void RegisterCallback(IResourceLoaderCallback callback) {
			if (callback == null)
				return;
			callbacks.Add(callback);

			if (IsLoaded)
			{
				AssetBundle[] assetBundles = new AssetBundle[dictionary.Count];
				dictionary.Values.CopyTo(assetBundles, 0);
				callback.OnResourceLoaded(assetBundles);
			}
		}

		public static void UnregisterCallback(IResourceLoaderCallback callback)
		{
			if (callback != null)
				callbacks.Remove(callback);
		}


#if UNITY_ANDROID
		IEnumerator LoadStreamingAsset()
		{
			Debug.Log("ABL: Start LoadStreamingAsset()");

			if (streamingAssetBundle.Count == 0)
				Debug.Log("ABL: No assetbundle found");

			IsLoading = true;
			IsLoaded = false;
			foreach (var name in streamingAssetBundle)
			{
				Debug.Log("ABL: try load AB: " + name);
				AssetBundle ab;
				bool loaded = dictionary.TryGetValue(name, out ab);
				if (!loaded)
				{
					if (!name.EndsWith(".assetbundle", true, CultureInfo.CurrentCulture))
						continue;

					var url = Path.Combine(Application.streamingAssetsPath, name);
					Debug.Log("ABL: Start loading assetbundle in url: " + url);
#if UNITY_2018_1_OR_NEWER
					using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
#else
					using (var request = UnityWebRequest.GetAssetBundle(url))
#endif
					{
						yield return request.SendWebRequest();

						if (request.isHttpError || request.isNetworkError)
							continue;
						ab = DownloadHandlerAssetBundle.GetContent(request);
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
			}
			IsLoading = false;
			IsLoaded = true;

			AssetBundle[] assetBundles = new AssetBundle[dictionary.Count];
			dictionary.Values.CopyTo(assetBundles, 0);

			foreach (var callback in callbacks)
			{
				try
				{
					// Even ab is null
					callback.OnResourceLoaded(assetBundles);
				}
				catch (Exception )
				{
					callbacks.Remove(callback);
				}
			}

			Debug.Log("ABL: End LoadStreamingAsset()");
		}
#endif

		void LoadResource()
		{
			Debug.Log("ABL: Start LoadResource()");

			string[] files = Directory.GetFiles(assetBundleRootPath);
			if (files.Length == 0)
				Debug.Log("ABL: No assetbundle found");

			IsLoading = true;
			IsLoaded = false;
			foreach (string name in files)
			{
				Debug.Log("ABL: found name " + name);
				AssetBundle ab;
				bool loaded = dictionary.TryGetValue(name, out ab);
				if (!loaded)
				{
					if (!name.EndsWith(".assetbundle", true, CultureInfo.CurrentCulture))
						continue;
					Debug.Log("ABL: Start loading assetbundle: " + name);

					if (File.Exists(name))
						ab = AssetBundle.LoadFromFile(name);

					if (ab != null)
					{
						Debug.Log("ABL: Assetbundle " + name + " is loaded");
						dictionary.Add(name, ab);
					}
					loaded = true;
				}

				if (ab == null)
					continue;
			}
			IsLoading = false;
			IsLoaded = true;

			AssetBundle[] assetBundles = new AssetBundle[dictionary.Count];
			dictionary.Values.CopyTo(assetBundles, 0);
			foreach (var callback in callbacks)
			{
				try
				{
					// Even ab is null
					callback.OnResourceLoaded(assetBundles);
				}
				catch (Exception )
				{
					callbacks.Remove(callback);
				}
			}

			Debug.Log("ABL: End LoadResource()");
		}

#if UNITY_ANDROID
		IEnumerator Start()
		{
			yield return LoadStreamingAsset();
		}
#else
		void Start()
		{
			LoadResource();
		}
#endif
	}
}
