using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.XR;

#if UNITY_EDITOR && UNITY_ANDROID
namespace Wave.XR.DirectPreview
{
	public class DirectPreviewRender : MonoBehaviour
	{
		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetRenderImageHandles")]
		public static extern bool WVR_SetRenderImageHandles3(IntPtr ttPtr);

		private class RTTask : RenderThreadTask
		{
			public RTTask(Receiver render) : base(render) {}

			public void IssueEvent(System.IntPtr l, System.IntPtr r)
			{
				var rtts = Queue.Obtain<RTTextures>();
				rtts.left = l;
				rtts.right = r;
				Queue.Enqueue(rtts);
				IssueEvent();
			}
		}

		RTTask renderThreadTask;

		static bool isLeftReady = false;
		static bool isRightReady = false;
		static int mFPS = 60;
		static long lastUpdateTime = 0;

		Camera cam;


		public static long getCurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
		}

		Material mat;
		List<XRDisplaySubsystem> subsystems = new List<XRDisplaySubsystem>();
		XRDisplaySubsystem displaySubsystem;
        XRDisplaySubsystem GetDisplaySubsystem()
        {
            SubsystemManager.GetInstances(subsystems);
            if (subsystems != null && subsystems.Count > 0)
            {
                for (int i = 0; i < subsystems.Count; i++)
                {
                    if (subsystems[i].SubsystemDescriptor.id == Constants.k_DisplaySubsystemId)
                    {
                        return subsystems[i];
                    }
                }
            }
            return null;
        }

        private void Start()
		{
			cam = GetComponent<Camera>();
			mat = new Material(Shader.Find("Unlit/DP2BlitShader"));
			lastUpdateTime = 0;

			if (mFPS == 0)
			{
				if (Application.targetFrameRate > 0 && Application.targetFrameRate < 99)
				{
					mFPS = Application.targetFrameRate;
				}
				else
				{
					mFPS = 75;
				}
				UnityEngine.Debug.LogWarning("mFPS is changed to " + mFPS);
			}

			RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

			ptrSize = Marshal.SizeOf(typeof(System.IntPtr));
			// Create Marshal memory for a IntPtr[2] array
			ptrArray = Marshal.AllocHGlobal(ptrSize * 2);

			renderThreadTask = new RTTask(RenderThreadReceiver);
		}

		class RTTextures : Message
		{
			public IntPtr left;
			public IntPtr right;
		}

		int ptrSize;
		IntPtr ptrArray;

		void RenderThreadReceiver(PreAllocatedQueue queue)
		{
			var msg = queue.Dequeue();
			if (msg == null) return;
			var rts = (RTTextures)msg;
			if (rts != null)
			{
				// Assign IntPtr to ptrArray[0]
				Marshal.WriteIntPtr(ptrArray, 0, rts.left);
				// Assign IntPtr to ptrArray[1]
				Marshal.WriteIntPtr(ptrArray, ptrSize, rts.right);
				// Call native function
				WVR_SetRenderImageHandles3(ptrArray);
			}
		}

		// Use Triple Buffer
		const int BufferCount = 3;
		int width = -1, height = -1;
		int bufferId = -1;

		class FrameBuffer
		{
			public readonly RenderTexture[] rt = new RenderTexture[2];
			public readonly System.IntPtr[] ptr = new IntPtr[2];

			static int id = 0;

			public RenderTexture Left { get { return rt[0]; } }
			public RenderTexture Right { get { return rt[1]; } }
			public System.IntPtr LeftPtr { get { return ptr[0]; } }
			public System.IntPtr RightPtr { get { return ptr[1]; } }

			public void Create(int w, int h)
			{
				for (int e = 0; e < 2; e++)
				{
					rt[e] = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
					rt[e].name = "DP2_RT_" + (e == 0 ? "L" : "R") + "_" + id;
					id = ++id % 60;
					rt[e].Create();
					ptr[e] = rt[e].GetNativeTexturePtr();
				}
			}

			public bool NeedCreate(int w, int h)
			{
				return rt[0] == null || rt[0].width != w || rt[0].height != h;
			}

			public void Destroy()
			{
				for (int e = 0; e < 2; e++)
				{
					if (rt[e] != null)
						rt[e].Release();
					rt[e] = null;
					ptr[e] = System.IntPtr.Zero;
				}
			}
		}

		readonly FrameBuffer[] fbs = new FrameBuffer[BufferCount];

		void CheckRenderTarget(int width, int height)
		{
			bool needCreate = this.width != width || this.height != height;
			needCreate |= fbs[0] == null || fbs[0].NeedCreate(width, height);

			if (!needCreate) return;

			this.width = width;
			this.height = height;

			for (int bId = 0; bId < BufferCount; bId++)
			{
				if (fbs[bId] == null)
					fbs[bId] = new FrameBuffer();
				fbs[bId].Destroy();
				fbs[bId].Create(width, height);
			}
		}

		// This only work for universal render pipeline
		void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (!DirectPreviewCore.IsDPInited) return;

			if (camera != cam) return;
			displaySubsystem = GetDisplaySubsystem();
			if (displaySubsystem == null) return;

			var currentTime = getCurrentTimeMillis();
			bool doCopy = (currentTime - lastUpdateTime) >= (1000 / mFPS);
			if (!doCopy) return;


			int pc = displaySubsystem.GetRenderPassCount();
			if (pc == 0) return;

			displaySubsystem.GetRenderPass(0, out var pass);
			int w = (int)(pass.renderTargetDesc.width / XRSettings.eyeTextureResolutionScale);
			int h = (int)(pass.renderTargetDesc.height / XRSettings.eyeTextureResolutionScale);

			if (w <= 0 || h <= 0) return;

			// New frame will render on another buffer
			bufferId = ++bufferId % BufferCount;

			CheckRenderTarget(w, h);

			lastUpdateTime = currentTime;

			bool isSinglePass = displaySubsystem.GetRenderPassCount() == 1;
			bool isMultiPass = displaySubsystem.GetRenderPassCount() == 2;
			if (isSinglePass)
			{
				var src = pass.renderTarget;
				var cmdBuf = new CommandBuffer();
				cmdBuf.name = "RT_to_DP2";
				cmdBuf.Blit(src, fbs[bufferId].Left, 0, 0);
				cmdBuf.Blit(src, fbs[bufferId].Right, 1, 0);
				context.ExecuteCommandBuffer(cmdBuf);
#if UNITY_2020_3_OR_NEWER
				if (Time.frameCount > mFPS)
#endif
				context.Submit();
				if (renderThreadTask != null)
					renderThreadTask.IssueEvent(fbs[bufferId].LeftPtr, fbs[bufferId].RightPtr);
			}

			if (isMultiPass)
			{
				displaySubsystem.GetRenderPass(1, out var passR);
				var srcL = pass.renderTarget;
				var srcR = passR.renderTarget;

				var cmdBuf = new CommandBuffer();
				cmdBuf.name = "RT_to_DP2";
				cmdBuf.Blit(srcL, fbs[bufferId].Left);
				cmdBuf.Blit(srcR, fbs[bufferId].Right);
				context.ExecuteCommandBuffer(cmdBuf);
#if UNITY_2020_3_OR_NEWER
				if (Time.frameCount > mFPS)
#endif
				context.Submit();
				if (renderThreadTask != null)
					renderThreadTask.IssueEvent(fbs[bufferId].LeftPtr, fbs[bufferId].RightPtr);
			}
		}

		void OnDestroy()
		{
			RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

			for (int bId = 0; bId < BufferCount; bId++)
			{
				if (fbs == null) continue;
				fbs[bId].Destroy();
				fbs[bId] = null;
			}

			// Free Marshal memory
			if (ptrArray != IntPtr.Zero);
				Marshal.FreeHGlobal(ptrArray);
			ptrArray = IntPtr.Zero;
		}

		bool ShouldCopy(long currentTime)
		{
			return (currentTime - lastUpdateTime) >= (1000 / (mFPS * 1.05f));
		}

		bool mpIsRendered = false;
		int frameCount = 0;

		// This only work for standard render pipeline
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			// Copy to editor display output first
			Graphics.Blit(src, dest);

			if (!DirectPreviewCore.IsDPInited) return;

			// Do DP2 copy
			var currentTime = getCurrentTimeMillis();
			bool doCopy = ShouldCopy(currentTime);
			if (!doCopy) return;

			//if (Camera.current != cam) return;  // always false
			displaySubsystem = GetDisplaySubsystem();
			if (displaySubsystem == null) return;

			displaySubsystem.GetRenderPass(0, out var pass);
			// Not allow to change the resolution scale to DP
			// TODO: should not allow change renderTargetDesc.width/height in wvrunityxr.dll
			int w = (int)(pass.renderTargetDesc.width / XRSettings.eyeTextureResolutionScale);
			int h = (int)(pass.renderTargetDesc.height / XRSettings.eyeTextureResolutionScale);
			CheckRenderTarget(w, h);

			if (frameCount != Time.frameCount)
			{
				frameCount = Time.frameCount;
				// New frame will render on another buffer
				bufferId = ++bufferId % BufferCount;

				isLeftReady = false;
				isRightReady = false;
				mpIsRendered = false;
			}

			bool isSinglePass = displaySubsystem.GetRenderPassCount() == 1;
			if (isSinglePass)
			{
				Graphics.Blit(src, fbs[bufferId].Left, new Vector2(1, -1), new Vector2(0, 1), 0, 0);
				Graphics.Blit(src, fbs[bufferId].Right, new Vector2(1, -1), new Vector2(0, 1), 1, 0);

				lastUpdateTime = currentTime;

				if (renderThreadTask != null)
					renderThreadTask.IssueEvent(fbs[bufferId].LeftPtr, fbs[bufferId].RightPtr);
			}
			else
			{
				if (!isLeftReady && cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
				{
					Graphics.Blit(src, fbs[bufferId].Left, new Vector2(1, -1), new Vector2(0, 1));
					isLeftReady = true;
				}

				if (isLeftReady && !isRightReady && cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
				{
					Graphics.Blit(src, fbs[bufferId].Right, new Vector2(1, -1), new Vector2(0, 1));
					isRightReady = true;
				}

				if (isLeftReady && isRightReady && !mpIsRendered)
				{
					mpIsRendered = true;
					lastUpdateTime = currentTime;
					if (renderThreadTask != null)
						renderThreadTask.IssueEvent(fbs[bufferId].LeftPtr, fbs[bufferId].RightPtr);
				}
			}
		}
	}
}
#endif
