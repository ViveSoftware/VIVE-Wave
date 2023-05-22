using UnityEngine;
using UnityEngine.Rendering;
using Wave.XR.Function;

/**
 * 
 * WaveXR_AMCProcess will be triggered by WaveXR_RuntimeInitializeOnLoad.
 * And only do FindMainCamera once when scene loaded.
 * Change Camera.main will stop this script.
 * 
**/

public class WaveXR_AMCProcess : MonoBehaviour
{
    // For GL.IssuePluginEvent.
    private System.IntPtr pluginEventHandlerPtr;
    private delegate int GetPluginEventIdDelegate(string eventName);
    private GetPluginEventIdDelegate GetPluginEventId;
    private Camera cam = null;
    private bool useCommandBuffer = false;
    private CommandBuffer commandBuffer = null;

    public static void FindMainCamera()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.Log("AMC cannot find the main camera");
            return;
        }

        if (mainCamera == null || mainCamera.stereoTargetEye == StereoTargetEyeMask.None)
        {
            Debug.Log("AMC cannot find the stereo camera");
            return;
        }

        var instance = mainCamera.GetComponent<WaveXR_AMCProcess>();
        if (instance == null) {
            instance = mainCamera.gameObject.AddComponent<WaveXR_AMCProcess>();
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!Application.isEditor)
#endif
        {
            pluginEventHandlerPtr = FunctionsHelper.GetFuncPtr("PluginEventHandler");
            GetPluginEventId = FunctionsHelper.GetFuncPtr<GetPluginEventIdDelegate>("GetPluginEventId");
        }

        cam = GetComponent<Camera>();

        if (commandBuffer == null)
        {
            commandBuffer = new CommandBuffer() { name = "CopyDepth" };

            int id = 1;
            if (GetPluginEventId != null)
                id = GetPluginEventId("CopyDepthTexture");

            // work
            if (pluginEventHandlerPtr != null && pluginEventHandlerPtr != System.IntPtr.Zero)
            {
                commandBuffer.IssuePluginEvent(pluginEventHandlerPtr, id);
            }
            else
            {
                //commandBuffer = null;
                //enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        // used to show enable checkbox in inspector
    }

    private void OnDisable()
    {
        RemoveCommandBuffer();
    }

    void AddCommandBuffer()
    {
        cam.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        useCommandBuffer = true;
    }

    void RemoveCommandBuffer()
    {
        if (useCommandBuffer)
            cam.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        useCommandBuffer = false;
    }

    private void Update()
    {
        if (cam == null)
            return;

        if (cam.depthTextureMode != DepthTextureMode.Depth &&
            cam.depthTextureMode != DepthTextureMode.None)
        {
            RemoveCommandBuffer();
            return;
        }

        // Force update CommandBuffer
        RemoveCommandBuffer();
        AddCommandBuffer();
    }
}
