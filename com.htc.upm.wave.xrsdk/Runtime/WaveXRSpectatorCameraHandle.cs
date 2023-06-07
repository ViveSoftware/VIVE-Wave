using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Wave.XR.Function;

namespace Wave.XR
{
    public sealed class WaveXRSpectatorCameraHandle : MonoBehaviour
    {
        static readonly string TAG = "WXRSpecCamH";
        static WaveXRSpectatorCameraHandle instance;
        uint frameCount = 0;
        bool changedST = false;  // change start
        bool changedSR = false;  // change should render
        bool run = false;
        bool shouldRenderFrame = false;
        Camera spectatorCamera;
        GameObject spectatorCameraObject;
        WaveXRSpectatorCamera spectatorCameraComponent;
        StringBuilder sb = new StringBuilder(300);

        System.IntPtr textureId;
        RenderTexture renderTexture;
        Material debugMaterial;

        // These fields is used to control spectator in editor
        public bool debugStartCamera = false;
        public bool debugRenderFrame = false;
        public float debugAccTime = 0;
        public int debugFPS = 30;

        private bool hasOverridePose = false;
        private Vector3 overridePosition;
        private Quaternion overrideRotation;

        private bool hasOverrideFov = false;
        private float overrideFov = 90.0f;

        private bool hasOverrideCullingMask = false;
        private int overrideCullingMask = 0x0;

        [StructLayout(LayoutKind.Sequential)]
        public struct RenderParametersNative
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

        public struct RenderParameters
        {
            public bool isStarted;
            public bool shouldRender;
            public bool isParametersValid;
            public bool hasPose;
            public int width;
            public int height;
            public float l;
            public float r;
            public float t;
            public float b;
            public Vector3 position;
            public Quaternion rotation;
            public float n;
            public float f;
            public Matrix4x4 proj;

            public void Set(RenderParametersNative na)
            {
                isStarted = na.isStarted;
                shouldRender= na.shouldRender;
                isParametersValid = na.isParametersValid;
                hasPose = na.hasPose;
                width = (int)na.width;
                height = (int)na.height;
                l = na.l;
                r = na.r;
                t = na.t;
                b = na.b;
                proj = WaveXRSpectatorCameraHandle.Frustum(l, r, t, b, n, f);
            }

            public void CalculateProj(float near, float far)
            {
                n = near;
                f = far;
                if (this.isParametersValid)
                    proj = WaveXRSpectatorCameraHandle.Frustum(l, r, t, b, n, f);
            }
        }

        RenderParameters renderParameters = new RenderParameters();

        public static WaveXRSpectatorCameraHandle GetInstance()
        {
            return instance;
        }

        private static void InitCheck()
        {
            if (instance != null) return;
            var initOnLoad = WaveXRRuntimeInitializeOnLoad.GetInstance();
            instance = initOnLoad.gameObject.AddComponent<WaveXRSpectatorCameraHandle>();
            if (!GetFunctions())
                instance.enabled = false;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitCheck();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                ReleaseResource();
            }
            else
            {
            }
        }

        public void OnEnable()
        {
            // This only available if you put this material in Resources folder.  We put it in our sample.
            debugMaterial = Resources.Load<Material>("Spectator/SpectatorCaptured");
        }

        public void OnDisable()
        {
            ReleaseResource();
        }


        // Update is called once per frame
        void Update()
        {
            StatusCheck();
            if (!run) return;
            // Prepare camera before rending
            if (!CheckCamera()) return;  // No main camera found
            spectatorCamera.enabled = false;
            if (!shouldRenderFrame) return;  // this frame skipped.
            UpdateRenderParameters();
            frameCount++;
        }

        private void OnDestroy()
        {
            ReleaseResource();
        }

        private void CopyMainCameraParameters()
        {
            Camera main = Camera.main;
            if (hasOverridePose)
            {
                spectatorCamera.transform.position = overridePosition;
                spectatorCamera.transform.rotation = overrideRotation;
            }
            else
            {
                spectatorCamera.transform.position = main.transform.position;
                spectatorCamera.transform.rotation = main.transform.rotation;
            }
            spectatorCamera.farClipPlane = main.farClipPlane;
            spectatorCamera.nearClipPlane = main.nearClipPlane;
            spectatorCamera.allowHDR = false;
            spectatorCamera.allowDynamicResolution = false;
            spectatorCamera.allowMSAA = main.allowMSAA;
            spectatorCamera.backgroundColor = main.backgroundColor;
            spectatorCamera.clearFlags = main.clearFlags;
            spectatorCamera.cullingMask = hasOverrideCullingMask ? overrideCullingMask : main.cullingMask;
            // No matter what depth we set, the mono camera always run before stereo camera.  See Profiler to check.
            spectatorCamera.depth = Camera.main.depth += 99;
            spectatorCamera.depthTextureMode = main.depthTextureMode;
            spectatorCamera.useOcclusionCulling = main.useOcclusionCulling;
            spectatorCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }

        // Called when parameters are validate.
        private void CheckTexture()
        {
            var rp = renderParameters;
            bool changed = false;
            if (renderTexture != null)
            {
                changed = renderTexture.width != rp.width || renderTexture.height != rp.height;
                if (changed)
                    DestroyRenderTexture();
            }

            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(rp.width, rp.height, 32);
                changed = true;
            }

            if (!renderTexture.IsCreated())
            {
                textureId = System.IntPtr.Zero;
                renderTexture.Create();
                if (debugMaterial)
                    debugMaterial.SetTexture("_MainTex", renderTexture);
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
            if (spectatorCamera == null) return;
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
                debugMaterial.SetTexture("_MainTex", null);
        }

        private bool CheckCamera()
        {
            if (Camera.main == null)
            {
                if (spectatorCamera == null)
                return false;
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

        private void StatusCheck()
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
                        shouldRenderFrame = true;
                        debugAccTime = 0;
                    }
                }
                return;
            }
#endif

            bool isStarted = IsSpectatorStarted();
            changedST = run != isStarted;
            changedSR = false;
            run = isStarted;
            if (changedST)
            {
                try
                {
                    if (isStarted)
                        onSpectatorStart();
                    else
                        onSpectatorStop();
                }
                catch (System.Exception e) { }

                shouldRenderFrame = false;
                frameCount = 0;
            }
            if (changedST && !run)
                ReleaseResource();

            if (run)
            {
                bool shouldRender = ShouldSpectatorRenderFrame();
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

        // The Matrix4x4.Frustum will imply the z-flip matrix.  Use custom projection instead.
        static Matrix4x4 Frustum(float left, float right, float top, float bottom, float near, float far)
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

        // Set pose in world space
        public void SetFixedPose(Vector3 position, Quaternion rotation)
        {
            hasOverridePose = true;
            overridePosition = position;
            overrideRotation = rotation;
        }

        public void ClearFixedPose()
        {
            hasOverridePose = false;
        }

        public void SetFixedFOV(float fov)
        {
            hasOverrideFov = true;
            overrideFov = Mathf.Clamp(fov, 5, 130);
        }

        public void ClearFixedFOV()
        {
            hasOverrideFov = false;
        }

        public void SetCullingMask(int cullingMask)
        {
            hasOverrideCullingMask = true;
            overrideCullingMask = cullingMask;
        }

        public void ClearCullingMask()
        {
            hasOverrideCullingMask = false;
        }

        public RenderParameters GetRenderParameters()
        {
            return renderParameters;
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
#endif
                n.isParametersValid = GetSpectatorRenderParameters(ref n.width, ref n.height, ref n.l, ref n.r, ref n.t, ref n.b);
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

            if (renderParameters.isParametersValid)
            {
                CheckTexture();
                CopyMainCameraParameters();
                if (hasOverrideFov)
                {
                    float aspect = renderParameters.width / (float)renderParameters.height;
                    spectatorCamera.projectionMatrix = Matrix4x4.Perspective(overrideFov, aspect, spectatorCamera.nearClipPlane, spectatorCamera.farClipPlane);
                }
                else
                {
                    renderParameters.CalculateProj(spectatorCamera.nearClipPlane, spectatorCamera.farClipPlane);
                    spectatorCamera.projectionMatrix = renderParameters.proj;
                }
                spectatorCamera.targetTexture = renderTexture;
                spectatorCamera.enabled = true;
            }
        }

        // Callback from Camera
        public void OnCameraPostRender()
        {
            if (textureId == System.IntPtr.Zero && renderTexture != null)
            {
                Profiler.BeginSample("SpecTexGetNativePtr");
                textureId = renderTexture.GetNativeTexturePtr();
                Profiler.EndSample();
            }
            SubmitInRenderThread();
        }

        public Camera GetCamera()
        {
            return spectatorCamera;
        }


#region render_thread_submit
        internal class RTData : Message
        {
            public System.IntPtr textureId;
        }

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
                msg.textureId = textureId;
                queue.Enqueue(msg);
            }

            task.IssueEvent();
        }

        RenderThreadTask task = new RenderThreadTask(
            // receiver
            (queue) => {
                //Debug.Log("RunInRenderThread");
                System.IntPtr nativeTexId;
                lock (queue)
                {
                    // Run in RenderThread
                    var msg = (RTData)queue.Dequeue();
                    nativeTexId = msg.textureId;
                    queue.Release(msg);
                }
#if UNITY_EDITOR
                if (Application.isEditor) return;
#endif
                SetSpectatorTexture(nativeTexId);
            }
        );
#endregion render_thread_submit

#region callback
        public delegate void OnSpectatorStartDelegate();
        public delegate void OnSpectatorStopDelegate();

        public OnSpectatorStartDelegate onSpectatorStart;
        public OnSpectatorStopDelegate onSpectatorStop;
#endregion


#region functions
        delegate bool IsSpectatorStartedDelegate();
        delegate bool ShouldSpectatorRenderFrameDelegate();
        delegate void SetSpectatorTextureDelegate(System.IntPtr ptr);
        delegate bool GetSpectatorRenderParametersDelegate(ref uint w, ref uint h, ref float l, ref float r, ref float t, ref float b);

        static IsSpectatorStartedDelegate IsSpectatorStarted;
        static ShouldSpectatorRenderFrameDelegate ShouldSpectatorRenderFrame;
        static SetSpectatorTextureDelegate SetSpectatorTexture;
        static GetSpectatorRenderParametersDelegate GetSpectatorRenderParameters;

        static bool GetFunctions()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return true;
#endif
            bool error = true;
            do
            {
                System.IntPtr ptr;
                ptr = FunctionsHelper.GetFuncPtr("IsSpectatorStarted");
                if (ptr == null) break;
                IsSpectatorStarted = Marshal.GetDelegateForFunctionPointer<IsSpectatorStartedDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("ShouldSpectatorRenderFrame");
                if (ptr == null) break;
                ShouldSpectatorRenderFrame = Marshal.GetDelegateForFunctionPointer<ShouldSpectatorRenderFrameDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("SetSpectatorTexture");
                if (ptr == null) break;
                SetSpectatorTexture = Marshal.GetDelegateForFunctionPointer<SetSpectatorTextureDelegate>(ptr);

                ptr = FunctionsHelper.GetFuncPtr("GetSpectatorRenderParameters");
                if (ptr == null) break;
                GetSpectatorRenderParameters = Marshal.GetDelegateForFunctionPointer<GetSpectatorRenderParametersDelegate>(ptr);

                error = false;
            } while (false);
            return !error;
        }
#endregion functions
    }
}