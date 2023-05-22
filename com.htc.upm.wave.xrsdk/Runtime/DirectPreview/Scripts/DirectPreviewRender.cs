using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR && UNITY_ANDROID
namespace Wave.XR.DirectPreview
{
	public class DirectPreviewRender : MonoBehaviour
	{
		private static string TAG = "DirectPreviewRender:";

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetRenderImageHandles")]
		public static extern bool WVR_SetRenderImageHandles(IntPtr[] ttPtr);

		static bool leftCall = false;
		static bool rightCall = false;
		static bool isLeftReady = false;
		static bool isRightReady = false;
		static RenderTexture rt_L;
		static RenderTexture rt_R;
		static IntPtr[] rt = new IntPtr[2];
		static int mFPS = 60;
		static long lastUpdateTime = 0;
		int frame = 0;
		new Camera camera;
		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Quit_S")]
		//public static extern void WVR_Quit_S();

		//public delegate void debugcallback(int l, string z);
		//[DllImport("wvrunityxr", EntryPoint = "SetPrintCallback")]
		//public static extern void RegisterDebugCallback(debugcallback callback);

		//[DllImport("wvrunityxr", EntryPoint = "GetFirstEyePtr")]
		//public static extern IntPtr GetFirstEyePtr();

		//[DllImport("wvrunityxr", EntryPoint = "GetSecondEyePtr")]
		//public static extern IntPtr GetSecondEyePtr();

		//public static void PrintLog(int l, string msg)
		//{
		//	switch (l)
		//	{
		//		case 0: // error
		//			UnityEngine.Debug.LogError(msg);
		//			break;
		//		case 1: // assert
		//			UnityEngine.Debug.LogAssertion(msg);
		//			break;
		//		case 2: // warning
		//			UnityEngine.Debug.LogWarning(msg);
		//			break;
		//		case 3: // log
		//			UnityEngine.Debug.Log(msg);
		//			break;
		//		case 4: // exception
		//			UnityEngine.Debug.LogError(msg);
		//			break;
		//		case 5:
		//			UnityEngine.Debug.Log(msg);
		//			break;
		//		default:
		//			UnityEngine.Debug.Log(msg);
		//			break;
		//	}

		//}

		//public enum SIM_InitError
		//{
		//	SIM_InitError_None = 0,
		//	SIM_InitError_WSAStartUp_Failed = 1,
		//	SIM_InitError_Already_Inited = 2,
		//	SIM_InitError_Device_Not_Found = 3,
		//	SIM_InitError_Can_Not_Connect_Server = 4,
		//	SIM_InitError_IPAddress_Null = 5,
		//}

		//public enum SIM_ConnectType
		//{
		//	SIM_ConnectType_USB = 0,
		//	SIM_ConnectType_Wifi = 1,
		//}

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Init_S")]
		//public static extern SIM_InitError WVR_Init_S(int a, System.IntPtr ip, bool enablePreview, bool saveLogToFile, bool saveImage);


		//bool enablePreview = false;
		//static bool saveLog = false;
		//static bool saveImage = false;
		//static int connectType = 0;  // USB


		//public delegate void printcallback(string z);

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetPrintCallback")]
		//public static extern void WVR_SetPrintCallback_S(printcallback callback);

		public static long getCurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
		}

		private static void PrintError(string msg)
		{
			Debug.LogError(TAG + ": " + msg);
		}

		private static void PrintDebug(string msg)
		{
			Debug.Log(TAG + ": " + msg);
		}
		Material mat;

		private void Start()
		{
			camera = GetComponent<Camera>();
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
		}

		private void Update()
		{
			frame++;
			//PrintDebug("update: " + frame);
		}

        //public void OnPostRender(Camera cam)
        //{
        //	Debug.Log(" native ptr: " + cam.activeTexture.GetNativeTexturePtr());
        //}

        readonly RenderTexture[] temp = new RenderTexture[2];

		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			//Debug.Log("vrUsage=" + src.vrUsage + ", width=" + src.width + ", height=" + src.height + ", name=" + src.name + ", frame=" + frame + ", eye=" + camera.stereoActiveEye);
			//Debug.Log("src native ptr: " + src.GetNativeTexturePtr() + ", eye=" + camera.stereoActiveEye);

			Graphics.Blit(src, dest);

			var height = src.height;
			if ((height % 2) != 0)
			{
				UnityEngine.Debug.LogWarning("RenderTexture height is odd, skip.");
				return;
			}

			long currentTime = getCurrentTimeMillis();
			if (currentTime - lastUpdateTime >= (1000 / mFPS))
			{
				if (!isLeftReady && camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
				{
					var desc = src.descriptor;
					desc.msaaSamples = 1;
					desc.depthBufferBits = 0;
					desc.useMipMap = false;
					temp[0] = RenderTexture.GetTemporary(desc);
					Graphics.Blit(src, temp[0], mat);
					isLeftReady = true;
				}

				if (isLeftReady && !isRightReady && camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
				{
					var desc = src.descriptor;
					desc.msaaSamples = 1;
					desc.depthBufferBits = 0;
					desc.useMipMap = false;
					temp[1] = RenderTexture.GetTemporary(desc);
					Graphics.Blit(src, temp[1], mat);
					isRightReady = true;
				}

				if (isLeftReady && isRightReady)
				{
					rt[0] = temp[0].GetNativeTexturePtr();
					rt[1] = temp[1].GetNativeTexturePtr();
					lastUpdateTime = currentTime;
					if (WVR_SetRenderImageHandles(rt))
					{
						//Debug.LogWarning("callback successfully");
					}
					else
					{
						//UnityEngine.Debug.LogWarning("WVR_SetRenderImageHandles fail");
					}
					isLeftReady = false;
					isRightReady = false;
					RenderTexture.ReleaseTemporary(temp[0]);
					RenderTexture.ReleaseTemporary(temp[1]);
					temp[0] = null;
					temp[1] = null;
				}
			}

			if (isLeftReady && isRightReady)
			{
			}
		}
	}
}
#endif