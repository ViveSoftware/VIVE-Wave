using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

#if UNITY_EDITOR && UNITY_ANDROID
namespace Wave.XR.DirectPreview
{
	public class DirectPreviewCore
	{
		//public enum SIM_InitError
		//{
		//	SIM_InitError_None = 0,
		//	SIM_InitError_WSAStartUp_Failed = 1,
		//	SIM_InitError_Already_Inited = 2,
		//	SIM_InitError_Device_Not_Found = 3,
		//	SIM_InitError_Can_Not_Connect_Server = 4,
		//	SIM_InitError_IPAddress_Null = 5,
		//}

		public enum SIM_ConnectType
		{
			SIM_ConnectType_USB = 0,
			SIM_ConnectType_Wifi = 1,
		}

		public delegate void LogCallbackDelegate(string z);

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetPrintCallback")]
		public static extern void WVR_SetPrintCallback_S(System.IntPtr callback);

		private static IntPtr GetFunctionPointerForDelegate(Delegate del)
		{
			return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(del);
		}

		private static readonly LogCallbackDelegate handle = new LogCallbackDelegate(NativeLogCallback);
		private static readonly IntPtr handlePtr = GetFunctionPointerForDelegate(handle);


		public static void PrintLog(string msg)
		{
			UnityEngine.Debug.Log("WVR_DirectPreview: " + msg);
		}

		[DllImport("wvrunityxr", EntryPoint = "EnableDP")]
		public static extern void EnableDP(bool enable, SIM_ConnectType type, IntPtr addr, bool preview, bool printLog, bool saveImage);

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Quit_S")]
		//public static extern void WVR_Quit_S();

		//[DllImport("wvrunityxr", EntryPoint = "GetFirstEyePtr")]
		//public static extern IntPtr GetFirstEyePtr();

		//[DllImport("wvrunityxr", EntryPoint = "GetSecondEyePtr")]
		//public static extern IntPtr GetSecondEyePtr();

		//private static string TAG = "DirectPreviewCore:";
		public static bool EnableDirectPreview = false;

		private static string TAG = "DirectPreviewCore";
#pragma warning disable
		private static Camera camera = null;
		bool enablePreview = false;
		static string wifi_ip_tmp;
#pragma warning enable
		static string wifi_ip_state = "";
		static bool saveLog = false;
		static bool saveImage = false;
		static int connectType = 0;  // USB

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetRenderImageHandles")]
		//public static extern bool WVR_SetRenderImageHandles(IntPtr[] ttPtr);

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Print")]
		//public static extern void WVR_Print(string msg);

		//public delegate void debugcallback(int l, string z);

		//[DllImport("wvrunityxr", EntryPoint = "SetPrintCallback")]
		//public static extern void RegisterDebugCallback(debugcallback callback);

		//class PrintClass
		//{

		//}


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
		//PrintClass pc;

		[InitializeOnEnterPlayMode]
		static void OnEnterPlayModeMethod(EnterPlayModeOptions options)
		{
			EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			PrintDebug("OnEnterPlayModeMethod: " + EnableDirectPreview);

			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

			if (EnableDirectPreview)
			{
				PrintDebug("Enable direct preview and add delegate to sceneLoaded");
				SceneManager.sceneLoaded += OnSceneLoaded;
				EditorApplication.wantsToQuit += WantsToQuit;

				PrintDebug("DirectPreviewCore.DP_Init");
				DP_Init();
			} else
			{
				EnableDP(false, (SIM_ConnectType)SIM_ConnectType.SIM_ConnectType_USB, IntPtr.Zero, false, false, false);
				PrintDebug("Enable Direct Preview: " + false);
				IsDPInited = false;
			}
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
				EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
				WVR_SetPrintCallback_S(System.IntPtr.Zero);
				IsDPInited = false;
			}
		}

		public static bool dpServerProcessChecker()
		{
			bool flag = false;
			Process[] processlist = Process.GetProcesses();
			foreach (Process theProcess in processlist)
			{
				if (theProcess.ProcessName == "dpServer")
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public static bool IsDPInited { get; set; } = false;

		public static void DP_Init()
		{
			EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			
			wifi_ip_state = EditorPrefs.GetString("wifi_ip_state");
			bool tPreview = EditorPrefs.GetBool("EnablePreviewImage", true);
			saveLog = EditorPrefs.GetBool("DllTraceLogToFile", false);
			saveImage = EditorPrefs.GetBool("OutputImagesToFile", false);
			connectType = EditorPrefs.GetInt("ConnectType", 1);
			string ipaddr = wifi_ip_state;
			System.IntPtr ptrIPaddr = Marshal.StringToHGlobalAnsi(ipaddr);

			if (EnableDirectPreview)
			{
				PrintDebug("Register direct preview print callback");
				WVR_SetPrintCallback_S(handlePtr);

				//if (connectType == 1)
				//{
				//	if (dpServerProcessChecker())
				//		UnityEngine.Debug.Log("dpServer.exe is running in task list.");
				//	else
				//		UnityEngine.Debug.LogWarning("There's no dpServer.exe running in task list.");
				//}

				EnableDP(true, (SIM_ConnectType)connectType, ptrIPaddr, tPreview, saveLog, saveImage);
				PrintDebug("Enable Direct Preview: " + true + ", connection: " + connectType + ", IP: " + ipaddr + ", preview: " + tPreview + ", log: " + saveLog + ", image: " + saveImage);
				IsDPInited = true;
			}
		}

		//public static long getCurrentTimeMillis()
		//{
		//	DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		//	return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
		//}

		//public static void OnPostRender(Camera cam)
		//{
		//	//bool isSinglePass = !displaySubsystem.singlePassRenderingDisabled;
		//	//WVR_Print("Print from DP - OnPostRender");

		//	//DisplaySubsystem.

		//	if (cam.tag.Equals("MainCamera"))
		//	{
		//		//RenderTexture rt = cam.activeTexture;
		//		//IntPtr nTexture = IntPtr.Zero;

		//		//if (rt != null)
		//		//{
		//		//	nTexture = rt.GetNativeTexturePtr();
		//		////	rt.graphicsFormat


		//		//	PrintDebug(cam.tag + " OnPostRender: " + nTexture + " : " + cam.stereoActiveEye +":" + rt.graphicsFormat + ":" + rt.width + ":" + rt.height);
		//		//}
		//		//IntPtr firstPtr = GetFirstEyePtr();
		//		//IntPtr SecondPtr = GetSecondEyePtr();
		//		//PrintDebug(" OnPostRender - 1st: " + firstPtr.ToString() + " 2nd: " + SecondPtr.ToString());

		//		if (!isLeftReady && cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)//eye == WVR_Eye.WVR_Eye_Left)
		//		{
		//			//rt_L =  cam.activeTexture ; //wvrCamera.GetCamera().targetTexture;

		//			//cam.text

		//			//if (rt_L != null)
		//			{
		//				//rt[0] = firstPtr;// rt_L.GetNativeTexturePtr();
		//				UnityEngine.Debug.LogWarning("rt[0] : " + rt[0]);
		//				isLeftReady = true;
		//				lastUpdateTime = 0;
		//			}
		//			//else
		//			//{
		//			//	UnityEngine.Debug.LogWarning("rt_L == null");
		//			//}
		//		}

		//		if (!isRightReady && cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)//eye == WVR_Eye.WVR_Eye_Right)
		//		{
		//			//rt_R = cam.activeTexture;

		//			//if (rt_R != null)
		//			{
		//				//rt[1] = SecondPtr;// rt_R.GetNativeTexturePtr();

		//				UnityEngine.Debug.LogWarning("rt[1] : " + rt[1]);
		//				isRightReady = true;
		//				lastUpdateTime = 0;
		//			}
		//			//else
		//			//{
		//			//	UnityEngine.Debug.LogWarning("rt_R == null");
		//			//}
		//		}

		//		if (isLeftReady && isRightReady)
		//		{
		//			if (mFPS == 0)
		//			{
		//				if (Application.targetFrameRate > 0 && Application.targetFrameRate < 99)
		//				{
		//					mFPS = Application.targetFrameRate;
		//				}
		//				else
		//				{
		//					mFPS = 75;
		//				}
		//				UnityEngine.Debug.LogWarning("mFPS is changed to " + mFPS);
		//			}

		//			long currentTime = getCurrentTimeMillis();
		//			if (currentTime - lastUpdateTime >= (1000 / mFPS))
		//			{
		//				if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
		//				{
		//					//var height = wvrCamera.GetCamera().targetTexture.height;
		//					//if ((height % 2) != 0)
		//					//{
		//					//	UnityEngine.Debug.LogWarning("RenderTexture height is odd, skip.");
		//					//	return;
		//					//}

		//					//var l1 = cam.activeTexture.GetNativeTexturePtr();
		//					//if (l1 != rt[0])
		//					//{
		//					//	UnityEngine.Debug.LogWarning("left native pointer changed");
		//					//	isLeftReady = false;
		//					//	isRightReady = false;
		//					//	return;
		//					//}
		//					leftCall = true;
		//				}

		//				if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
		//				{
		//					//var height = wvrCamera.GetCamera().targetTexture.height;
		//					//if ((height % 2) != 0)
		//					//{
		//					//	UnityEngine.Debug.LogWarning("RenderTexture height is odd, skip.");
		//					//	return;
		//					//}

		//					//var r1 = cam.activeTexture.GetNativeTexturePtr();
		//					//if (r1 != rt[1])
		//					//{
		//					//	UnityEngine.Debug.LogWarning("right native pointer changed");
		//					//	isLeftReady = false;
		//					//	isRightReady = false;
		//					//	return;
		//					//}
		//					rightCall = true;
		//				}

		//				if (leftCall && rightCall)
		//				{
		//					lastUpdateTime = currentTime;
		//					if (WVR_SetRenderImageHandles(rt))
		//					{
		//						// Debug.LogWarning("callback successfully");
		//					}
		//					else
		//					{
		//						UnityEngine.Debug.LogWarning("" +
		//							"callback fail");
		//					}
		//					leftCall = false;
		//					rightCall = false;
		//				}
		//			}
		//		}
		//	}
		//}

		private static void PrintError(string msg)
		{
			UnityEngine.Debug.LogError(TAG + ": " + msg);
		}

		private static void PrintDebug(string msg)
		{
			UnityEngine.Debug.Log(TAG + ": " + msg);
		}

		[MonoPInvokeCallback(typeof(LogCallbackDelegate))]
		private static void NativeLogCallback(string msg)
		{
			UnityEngine.Debug.Log("DirectPreviewCore(NCB): " + msg);
		}

		public class DirectPreviewRendererHooker : MonoBehaviour
        {
			// Only MonoBehaviour has coroutine
			IEnumerator Start()
            {
				while (Camera.main == null)
					yield return null;
				if (Camera.main.gameObject.GetComponent<DirectPreviewRender>() == null)
					Camera.main.gameObject.AddComponent<DirectPreviewRender>();
				Destroy(this.gameObject);
			}
        }

		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (EnableDirectPreview)
			{
				var obj = new GameObject("DPRendererHooker");
				obj.AddComponent<DirectPreviewRendererHooker>();
			}
		}

		static bool WantsToQuit()
		{
			UnityEngine.Debug.Log("Editor prevented from quitting. --------");
			SceneManager.sceneLoaded -= OnSceneLoaded;

			IsDPInited = false;

			return true;
		}
	}
}
#endif
