// AssetBundleTool is designed for general Unity environment.
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

namespace AssetBundleTool
{
	public class AssetBundleObjectLoaderBuildProcess :
		IProcessSceneWithReport
	{
		public int callbackOrder { get { return 0; } }

		public void OnProcessScene(Scene scene, BuildReport report)
		{
			OnProcessScene(scene);
		}

		List<string> aboPath = new List<string>();

		public void FindAssetBundleObjectLoader(GameObject obj)
		{
			var abos = obj.GetComponentsInChildren<AssetBundleObjectLoader>();
			foreach (var abo in abos)
			{
				aboPath.Add(abo.assetTypeName);
			}
		}

		public void OnProcessScene(Scene scene)
		{
			GameObject [] rootObjs = scene.GetRootGameObjects();
			foreach (var obj in rootObjs)
			{
				FindAssetBundleObjectLoader(obj);
			}
		}
	}
}

#endif
