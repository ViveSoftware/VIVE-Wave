using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundleTool
{
    public class AssetBundleLoader : MonoBehaviour
    {

        public string assetBundlePath = "";

        private void Awake()
        {
            assetBundlePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "scenes.assetbundle";
        }

        void Start()
        {
            Debug.Log("AssetBundlePathName: " + assetBundlePath);
            AssetBundle assetBundle = null;
            if (File.Exists(assetBundlePath))
                assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (assetBundle == null)
            {
                Debug.LogWarning("Load fail");
                return;
            }

            if (assetBundle.isStreamedSceneAssetBundle)
            {
                string[] scenePaths = assetBundle.GetAllScenePaths();
                foreach (string scene in scenePaths)
                {
                    Debug.Log("Scene: " + scenePaths);
                }
                string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}