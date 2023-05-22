// AssetBundleTool is designed for general Unity environment.
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetBundleTool
{
	public class AssetBundleObjectLoader : MonoBehaviour, AssetBundleResourceLoader.IResourceLoaderCallback
	{
		[Tooltip("Get name from what you drag a asset from project and drop it to the target field below.")]
		public string assetTypeName = "";

		[Tooltip("This field will be always empty")]
		public GameObject target;

		private GameObject originalObject = null;
		private Object [] originalObjects = null;
		private GameObject instance = null;

		//static Dictionary<string, AssetBundle> dictionary = new Dictionary<string, AssetBundle>();

		//private AssetBundle[] abs;

		private void OnEnable()
		{
#if UNITY_EDITOR
			var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetTypeName);
			if (obj != null)
			{
				instance = Instantiate(obj, transform.position, transform.rotation, transform.parent);
				return;
			}
#endif
			AssetBundleResourceLoader.RegisterCallback(this);
		}

		private void OnDisable()
		{
			AssetBundleResourceLoader.UnregisterCallback(this);
		}

		public void OnResourceLoaded(AssetBundle[] abs)
		{
			//this.abs = abs;
			if (originalObject == null)
			{
				foreach (var ab in abs)
				{
					originalObjects = ab.LoadAssetWithSubAssets(assetTypeName);

					if (originalObjects != null && originalObjects[0] != null)
					{
						instance = Instantiate((GameObject)originalObjects[0], transform.position, transform.rotation, transform.parent);
						break;
					}
				}
			}
		}

		public bool BeforeResourceUnload(AssetBundle ab)
		{
			Destroy(instance);
			Destroy(originalObject);
			return false;
		}

		public void OnResourceUnloaded(AssetBundle ab)
		{
		}

		public void OnValidate()
		{
#if UNITY_EDITOR
			if (target != null)
			{
				assetTypeName = AssetDatabase.GetAssetPath(target);
				target = null;
			}
#endif
		}
	}
}
