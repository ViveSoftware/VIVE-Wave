using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using Wave.XR.Loader;
using Wave.XR.Settings;

namespace Wave.XR
{
    public class WaveXRRuntimeInitializeOnLoad : MonoBehaviour
    {
        static readonly string TAG = "WaveXR_InitOnLoad";
        static WaveXRRuntimeInitializeOnLoad instance;
        static bool isFirstScene = true;

        public static WaveXRRuntimeInitializeOnLoad GetInstance()
        {
            return instance;
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            Debug.Log(TAG + ": OnRuntimeMethodLoad");
            if (XRGeneralSettings.Instance == null ||
                XRGeneralSettings.Instance.Manager == null ||
                XRGeneralSettings.Instance.Manager.activeLoader == null ||
                XRGeneralSettings.Instance.Manager.activeLoader.GetType() != typeof(WaveXRLoader))
            {
                Debug.Log(TAG + ": Not XR. Disabled.");
                return;
            }
#if false // When debug Set false to run in editor
            if (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null || XRGeneralSettings.Instance.Manager.activeLoader == null || XRGeneralSettings.Instance.Manager.activeLoader.GetType() != typeof(WaveXRLoader))
                return; //Don't create GO and script instance if active loader is not found or is not WaveXRLoader
#endif
            GameObject obj = new GameObject(TAG, typeof(WaveXRRuntimeInitializeOnLoad));
            DontDestroyOnLoad(obj);
            isFirstScene = true;
            instance = obj.GetComponent<WaveXRRuntimeInitializeOnLoad>();
        }

        private void Awake()
        {
            Debug.Log(TAG + ": Awake");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneLoadActions(scene, mode);
            Debug.Log(TAG + ": OnSceneLoaded: " + scene.name);
        }

        private void SceneLoadActions(Scene scene, LoadSceneMode mode)
        {
            var settings = WaveXRSettings.GetInstance();
            if (settings == null)
            {
                Debug.Log(TAG + ": WaveXR settings instance is null");
                return;
            }
            if (settings.allowSpectatorCamera)
            {
                try
                {
                    WaveXRSpectatorCameraHandle.OnSceneLoaded(scene, mode);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        void Update()
        {

        }

        private void OnEnable()
        {
            Debug.Log(TAG + ": OnEnable");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static RenderThreadTask renderThreadRenameTask;
        private static void RenameRenderThreadReceiver(PreAllocatedQueue _)
        {
            try
            {
                Thread thread = Thread.CurrentThread;
                thread.Name = "UnityGfx";
                renderThreadRenameTask = null;
            }
            catch (System.Exception) { /* Thread name can only set once.  Avoid exception if set by others. */}
        }

        private void Start()
        {
            Debug.Log(TAG + ": Start");
            if (isFirstScene) //Manually run SceneLoadActions actions in first scene as hooking OnSceneLoaded to delegate happens after first scene load
            {
                SceneLoadActions(SceneManager.GetActiveScene(), LoadSceneMode.Single);
                isFirstScene = false;
            }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (!Application.isEditor)
#endif
            {
                renderThreadRenameTask = new RenderThreadTask(RenameRenderThreadReceiver);
                if (renderThreadRenameTask != null)
                    renderThreadRenameTask.IssueEvent();
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log(TAG + ": OnDisable");
        }
    }
}