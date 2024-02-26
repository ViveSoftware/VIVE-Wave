using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.XR.Function;

namespace Wave.XR
{
    public sealed class WaveXRSpectatorCameraHandle : MonoBehaviour
    {
        // These fields is used to control spectator in editor
        public bool debugStartCamera = false;
        public bool debugRenderFrame = false;
        public int debugFPS = 30;
        private float debugAccTime = 0;
        private Material debugMaterial;

        private static WaveXRSpectatorCameraHandle instance;

        // Camera state
        private bool changedST = false; // change start
        private bool changedSR = false; // change should render
        private bool run = false; // camera is running or not
        private bool shouldRenderFrame = false; // should render the frame in current time

        // Unity component
        private Camera spectatorCamera;
        private GameObject spectatorCameraObject;
        private WaveXRSpectatorCamera spectatorCameraComponent;
        private readonly StringBuilder sb = new StringBuilder(300);

        // Texture
        private System.IntPtr textureId;
        private RenderTexture renderTexture;

        // Override pose variables
        private bool hasOverridePose = false;
        private Vector3 overridePosition;
        private Quaternion overrideRotation;

        // Override fov variables
        private bool hasOverrideFov = false;
        private float overrideFov = 90.0f;

        // Override culling mask variables
        private bool hasOverrideCullingMask = false;
        private int overrideCullingMask = 0x0;

        // Current frame render parameters of the spectator
        private RenderParameters renderParameters = new RenderParameters();

        // Render thread task
        private readonly RenderThreadTask task = new RenderThreadTask(
            // receiver
            (queue) =>
            {
                //Debug.Log("RunInRenderThread");
                System.IntPtr nativeTexId;
                lock (queue)
                {
                    // Run in RenderThread
                    var msg = (RTData)queue.Dequeue();
                    nativeTexId = msg.TextureId;
                    queue.Release(msg);
                }
#if UNITY_EDITOR
                if (Application.isEditor) return;
#endif
                setSpectatorTexture(nativeTexId);
            }
        );

        // Callback function when spectator start/stop
        public OnSpectatorStartDelegate OnSpectatorStart;
        public OnSpectatorStopDelegate OnSpectatorStop;

        private static IsSpectatorStartedDelegate isSpectatorStarted;
        private static ShouldSpectatorRenderFrameDelegate shouldSpectatorRenderFrame;
        private static SetSpectatorTextureDelegate setSpectatorTexture;
        private static GetSpectatorRenderParametersDelegate getSpectatorRenderParameters;

        // Shader property
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        #region Public function of spectator handler
        
        /// <summary>
        /// Get the instance of WaveXRSpectatorCameraHandle
        /// </summary>
        /// <returns>WaveXRSpectatorCameraHandle component</returns>
        public static WaveXRSpectatorCameraHandle GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Get spectator camera
        /// </summary>
        /// <returns>Camera component</returns>
        public Camera GetCamera()
        {
            return spectatorCamera;
        }

        /// <summary>
        /// The event function that Unity calls after spectator camera renders the scene.
        /// This will update the spectator camera texture and then submit to the render thread.
        /// </summary>
        public void OnCameraPostRender()
        {
            if (textureId == System.IntPtr.Zero && renderTexture != null)
            {
                textureId = renderTexture.GetNativeTexturePtr();
            }

            SubmitInRenderThread();
        }

        /// <summary>
        /// Set the spectator camera pose by specific input position and rotation.
        /// </summary>
        /// <param name="position">The position you want to set to the spectator camera.</param>
        /// <param name="rotation">The rotation you want to set to the spectator camera.</param>
        public void SetFixedPose(Vector3 position, Quaternion rotation)
        {
            hasOverridePose = true;
            overridePosition = position;
            overrideRotation = rotation;
        }

        /// <summary>
        /// Reset the spectator camera pose and set it to the main camera pose.
        /// </summary>
        public void ClearFixedPose()
        {
            hasOverridePose = false;
        }

        /// <summary>
        /// Set the spectator camera fov by specific input value.
        /// </summary>
        /// <param name="fov">The FOV value you want to set to the spectator camera.</param>
        public void SetFixedFOV(float fov)
        {
            hasOverrideFov = true;
            overrideFov = Mathf.Clamp(fov, 5, 130);
        }

        /// <summary>
        /// Reset the spectator camera fov value.
        /// </summary>
        public void ClearFixedFOV()
        {
            hasOverrideFov = false;
        }

        /// <summary>
        /// Set the spectator camera culling mask by specific input value.
        /// </summary>
        /// <param name="cullingMask">The culling mask you want to set to the spectator camera.</param>
        public void SetCullingMask(int cullingMask)
        {
            hasOverrideCullingMask = true;
            overrideCullingMask = cullingMask;
        }

        /// <summary>
        /// Reset the spectator camera culling mask.
        /// </summary>
        public void ClearCullingMask()
        {
            hasOverrideCullingMask = false;
        }
        
        #endregion Public function of spectator handler

        #region Private function of spectator handler internal usage in its lifecycle

        private static void Init()
        {
            if (instance != null)
            {
                return;
            }

            var initOnLoad = WaveXRRuntimeInitializeOnLoad.GetInstance();
            instance = initOnLoad.gameObject.AddComponent<WaveXRSpectatorCameraHandle>();

            if (!GetFunctions())
            {
                instance.enabled = false;
            }
        }

        private void DestroyRenderTexture()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture, 0.5f);
                renderTexture = null;
            }

            textureId = System.IntPtr.Zero;
        }

        private void DestroyCamera()
        {
            if (spectatorCamera == null)
            {
                return;
            }

            spectatorCamera.targetTexture = null;
            spectatorCamera = null;
            spectatorCameraComponent = null;
            Destroy(spectatorCameraObject, 0.5f);
            spectatorCameraObject = null;
        }

        private void ReleaseResource()
        {
            DestroyRenderTexture();
            DestroyCamera();
            if (debugMaterial)
            {
                debugMaterial.SetTexture(MainTex, null);
            }
        }

        private bool CheckCamera()
        {
            if (Camera.main == null)
            {
                if (spectatorCamera == null)
                {
                    return false;
                }
            }

            if (spectatorCamera == null)
            {
                spectatorCameraObject = new GameObject("Spectator");
                spectatorCamera = spectatorCameraObject.AddComponent<Camera>();
                spectatorCameraComponent = spectatorCameraObject.AddComponent<WaveXRSpectatorCamera>();
                spectatorCameraComponent.SetHandle(this);
                DontDestroyOnLoad(spectatorCameraObject);
                spectatorCamera.enabled = false;
                spectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;

                CopyMainCameraParameters();
            }

            return true;
        }

        // Called when parameters are validate.
        private void CheckTexture()
        {
            var rp = renderParameters;
            if (renderTexture != null)
            {
                bool changed = renderTexture.width != (int)rp.RenderParametersNative.width ||
                               renderTexture.height != (int)rp.RenderParametersNative.height;
                if (changed)
                {
                    DestroyRenderTexture();
                }
            }

            if (renderTexture == null)
            {
                renderTexture = new RenderTexture((int)rp.RenderParametersNative.width,
                    (int)rp.RenderParametersNative.height, 32);
            }

            if (!renderTexture.IsCreated())
            {
                textureId = System.IntPtr.Zero;
                renderTexture.Create();
                if (debugMaterial)
                {
                    debugMaterial.SetTexture(MainTex, renderTexture);
                }
            }
        }

        private void CheckStatus()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                run = debugStartCamera;
                if (!run)
                {
                    shouldRenderFrame = false;
                    ReleaseResource();
                    return;
                }
                if (debugRenderFrame)
                {
                    debugAccTime += Time.unscaledDeltaTime;
                    if (debugAccTime > (1.0f / debugFPS))
                    {
                        // This frame should render
                        shouldRenderFrame = true;
                        debugAccTime = 0;
                    }
                    else
                    {
                        // This frame should not render due to fps setting
                        shouldRenderFrame = false;
                    }
                }
                else
                {
                    // This frame should not render due to the "fps" flag statement (equal to false)
                    shouldRenderFrame = false;
                }

                return;
            }
#endif

            bool isStarted = isSpectatorStarted();
            changedST = run != isStarted;
            changedSR = false;
            run = isStarted;
            if (changedST)
            {
                try
                {
                    if (isStarted)
                    {
                        OnSpectatorStart();
                    }
                    else
                    {
                        OnSpectatorStop();
                    }
                }
                catch
                {
                    // ignored
                }

                shouldRenderFrame = false;
            }

            if (changedST && !run)
            {
                // state change to stop
                ReleaseResource();
            }

            if (run)
            {
                bool shouldRender = shouldSpectatorRenderFrame();
                changedSR = shouldRenderFrame != shouldRender;
                shouldRenderFrame = shouldRender;
            }
            else
            {
                shouldRenderFrame = false;
            }

            if (changedST || changedSR)
            {
                var t = Application.GetStackTraceLogType(LogType.Log);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                Debug.Log("Spectator isStarted=" + isStarted + ", shouldRenderFrame=" + shouldRenderFrame);
                Application.SetStackTraceLogType(LogType.Log, t);
            }
        }

        // The Matrix4x4.Frustum will imply the z-flip matrix. Use custom projection instead.
        private static Matrix4x4 Frustum(float left, float right, float top, float bottom, float near, float far)
        {
            float x = 2.0F / (right - left);
            float y = 2.0F / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }

        private void CopyMainCameraParameters()
        {
            Camera main = Camera.main;
            if (hasOverridePose)
            {
                var spectatorCameraTransform = spectatorCamera.transform;

                spectatorCameraTransform.position = overridePosition;
                spectatorCameraTransform.rotation = overrideRotation;
            }
            else
            {
                if (!(main is null))
                {
                    var mainCameraTransform = main.transform;
                    var spectatorCameraTransform = spectatorCamera.transform;

                    spectatorCameraTransform.position = mainCameraTransform.position;
                    spectatorCameraTransform.rotation = mainCameraTransform.rotation;
                }
            }

            if (!(main is null))
            {
                spectatorCamera.farClipPlane = main.farClipPlane;
                spectatorCamera.nearClipPlane = main.nearClipPlane;
                spectatorCamera.allowMSAA = main.allowMSAA;
                spectatorCamera.backgroundColor = main.backgroundColor;
                spectatorCamera.clearFlags = main.clearFlags;
                spectatorCamera.depth =
                    main.depth +
                    99; // No matter what depth we set, the mono camera always run before stereo camera. See Profiler to check.
                spectatorCamera.depthTextureMode = main.depthTextureMode;
                spectatorCamera.useOcclusionCulling = main.useOcclusionCulling;
                spectatorCamera.cullingMask = hasOverrideCullingMask ? overrideCullingMask : main.cullingMask;
            }

            spectatorCamera.allowHDR = false;
            spectatorCamera.allowDynamicResolution = false;
            spectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }

        private void UpdateRenderParameters()
        {
            RenderParametersNative n = new RenderParametersNative();
#if UNITY_EDITOR
            n.isParametersValid = true;
            n.isStarted = true;
            n.shouldRender = true;
            n.hasPose = false;
            n.l = -1.77777778f;
            n.r = 1.77777778f;
            n.t = 1;
            n.b = -1;
            n.width = 1920;
            n.height = 1080;
            if (!Application.isEditor)
            {
#endif
                // Get the render parameters from native
                n.isParametersValid =
                    getSpectatorRenderParameters(ref n.width, ref n.height, ref n.l, ref n.r, ref n.t, ref n.b);
#if UNITY_EDITOR
            }
#endif

            renderParameters.Set(n);

            if (changedSR)
            {
                var t = Application.GetStackTraceLogType(LogType.Log);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                sb.Clear()
                    .Append("Spectator parameters isValid=").Append(n.isParametersValid)
                    .Append(", w=").Append(n.width)
                    .Append(", h=").Append(n.height)
                    .Append(", l=").Append(n.l)
                    .Append(", r=").Append(n.r)
                    .Append(", t=").Append(n.t)
                    .Append(", b=").Append(n.b);
                Debug.Log(sb.ToString());
                Application.SetStackTraceLogType(LogType.Log, t);
            }

            if (renderParameters.RenderParametersNative.isParametersValid)
            {
                CheckTexture();
                CopyMainCameraParameters();
                if (hasOverrideFov)
                {
                    float aspect = renderParameters.RenderParametersNative.width /
                                   (float)renderParameters.RenderParametersNative.height;
                    spectatorCamera.projectionMatrix = Matrix4x4.Perspective(overrideFov, aspect,
                        spectatorCamera.nearClipPlane, spectatorCamera.farClipPlane);
                }
                else
                {
                    renderParameters.CalculateProj(spectatorCamera.nearClipPlane, spectatorCamera.farClipPlane);
                    spectatorCamera.projectionMatrix = renderParameters.Proj;
                }

                spectatorCamera.targetTexture = renderTexture;
                spectatorCamera.enabled = true;
            }
        }

        private RenderParameters GetRenderParameters()
        {
            return renderParameters;
        }

        private static bool GetFunctions()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return true;
#endif
            bool error = true;
            do
            {
                var ptr = FunctionsHelper.GetFuncPtr("IsSpectatorStarted");
                if (Equals(ptr, default(System.IntPtr)) || ptr == System.IntPtr.Zero)
                {
                    break;
                }

                isSpectatorStarted = Marshal.GetDelegateForFunctionPointer<IsSpectatorStartedDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("ShouldSpectatorRenderFrame");
                if (Equals(ptr, default(System.IntPtr)) || ptr == System.IntPtr.Zero)
                {
                    break;
                }

                shouldSpectatorRenderFrame =
                    Marshal.GetDelegateForFunctionPointer<ShouldSpectatorRenderFrameDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("SetSpectatorTexture");
                if (Equals(ptr, default(System.IntPtr)) || ptr == System.IntPtr.Zero)
                {
                    break;
                }

                setSpectatorTexture = Marshal.GetDelegateForFunctionPointer<SetSpectatorTextureDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("GetSpectatorRenderParameters");
                if (Equals(ptr, default(System.IntPtr)) || ptr == System.IntPtr.Zero)
                {
                    break;
                }

                getSpectatorRenderParameters =
                    Marshal.GetDelegateForFunctionPointer<GetSpectatorRenderParametersDelegate>(ptr);

                error = false;
            } while (false);

            return !error;
        }

        #region Render thread submit

        private void SubmitInRenderThread()
        {
#if UNITY_EDITOR && !UNITY_2021_3
            // Older version will hang after run script in render thread.
            if (Application.isEditor)
                return;
#endif
#if !UNITY_ANDROID
			if (Application.platform != RuntimePlatform.Android)
				return;
#endif

            var queue = task.Queue;

            lock (queue)
            {
                var msg = queue.Obtain<RTData>();
                msg.TextureId = textureId;
                queue.Enqueue(msg);
            }

            task.IssueEvent();
        }

        #endregion Render thread submit

        #endregion Private function of spectator handler internal usage in its lifecycle
        
        #region Public callback function
        
        /// <summary>
        /// The callback function registration when the spectator camera is started.
        /// </summary>
        public delegate void OnSpectatorStartDelegate();

        /// <summary>
        /// The callback function registration when the spectator camera is stopped.
        /// </summary>
        public delegate void OnSpectatorStopDelegate();
        
        #endregion Public callback function

        #region Callback function for internal usage of spectator handler
        
        private delegate bool IsSpectatorStartedDelegate();

        private delegate bool ShouldSpectatorRenderFrameDelegate();

        private delegate void SetSpectatorTextureDelegate(System.IntPtr ptr);

        private delegate bool GetSpectatorRenderParametersDelegate(
            ref uint w,
            ref uint h,
            ref float l,
            ref float r,
            ref float t,
            ref float b);

        #endregion Callback function for internal usage of spectator handler

        #region Unity Lifecycle functions

        private void OnEnable()
        {
            // This only available if you put this material in Resources folder.  We put it in our sample.
            debugMaterial = Resources.Load<Material>("Spectator/SpectatorCaptured");
        }

        private void OnDisable()
        {
            ReleaseResource();
        }

        // Update is called once per frame
        private void Update()
        {
            CheckStatus();

            if (!run)
            {
                return;
            }

            // Prepare camera before rendering
            if (!CheckCamera())
            {
                // No main camera found
                return;
            }

            spectatorCamera.enabled = false;

            // Check if we need to render this frame (related to fps setting)
            if (!shouldRenderFrame)
            {
                // This frame skipped if "fps" flag is false
                return;
            }

            UpdateRenderParameters();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                ReleaseResource();
            }
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

        private void OnDestroy()
        {
            ReleaseResource();
        }

        #endregion Unity Lifecycle functions

        #region Structrue/Class definition

        private class RTData : Message
        {
            public System.IntPtr TextureId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RenderParametersNative
        {
            public bool isStarted;
            public bool shouldRender;
            public bool isParametersValid;
            public bool hasPose;
            public uint width;
            public uint height;
            public float l;
            public float r;
            public float t;
            public float b;
            public Vector3 position;
            public Quaternion rotation;
        }

        private struct RenderParameters
        {
            public RenderParametersNative RenderParametersNative;
            public Matrix4x4 Proj;

            private float _n;
            private float _f;

            public void Set(RenderParametersNative na)
            {
                RenderParametersNative = na;
                Proj = WaveXRSpectatorCameraHandle.Frustum(
                    RenderParametersNative.l,
                    RenderParametersNative.r,
                    RenderParametersNative.t,
                    RenderParametersNative.b,
                    _n,
                    _f);
            }

            public void CalculateProj(float near, float far)
            {
                _n = near;
                _f = far;
                if (RenderParametersNative.isParametersValid)
                {
                    Proj = WaveXRSpectatorCameraHandle.Frustum(
                        RenderParametersNative.l,
                        RenderParametersNative.r,
                        RenderParametersNative.t,
                        RenderParametersNative.b,
                        _n,
                        _f);
                }
            }
        }

        #endregion Structrue/Class definition
    }
}