// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Wave.Native
{
#if UNITY_EDITOR && UNITY_ANDROID
	public class WVR_DirectPreview : Wave.Native.Interop.WVR_Base
	{
#pragma warning disable
		private static string LOG_TAG = "WVR_DirectPreview";
#pragma warning enable
		public static string wifi_ip_tmp;
		public static string wifi_ip_state = "";
#pragma warning disable
		bool isLeftReady = false;
		bool isRightReady = false;
#pragma warning enable
		//bool isTimeUp = false;
		//bool isClientConnected = false;
		RenderTexture rt_L;
		RenderTexture rt_R;
		IntPtr[] rt = new IntPtr[2];
		int mFPS = 60;
		long lastUpdateTime = 0;
		bool enablePreview = false;
		bool saveLog = false;
		bool saveImage = false;
		int connectType = 0;  // USB

		public enum SIM_InitError
		{
			SIM_InitError_None = 0,
			SIM_InitError_WSAStartUp_Failed = 1,
			SIM_InitError_Already_Inited = 2,
			SIM_InitError_Device_Not_Found = 3,
			SIM_InitError_Can_Not_Connect_Server = 4,
			SIM_InitError_IPAddress_Null = 5,
		}

		public enum SIM_ConnectType
		{
			SIM_ConnectType_USB = 0,
			SIM_ConnectType_Wifi = 1,
		}

		public delegate void debugcallback(string z);

		#region wvr_hand.h

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetInteractionMode_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_InteractionMode WVR_GetInteractionMode_S();
		public override WVR_InteractionMode GetInteractionMode()
		{
			return WVR_GetInteractionMode_S();
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetHandGestureInfo_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandGestureInfo_S(ref WVR_HandGestureInfo_t info);
		public override WVR_Result GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
		{
			return WVR_GetHandGestureInfo_S(ref info);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_StartHandGesture_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartHandGesture_S(ulong demands);
		public override WVR_Result StartHandGesture(ulong demands)
		{
			return WVR_StartHandGesture_S(demands);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_StopHandGesture_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopHandGesture_S();
		public override void StopHandGesture()
		{
			WVR_StopHandGesture_S();
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetHandGestureData_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandGestureData_S(ref WVR_HandGestureData_t data);
		public override WVR_Result GetHandGestureData(ref WVR_HandGestureData_t data)
		{
			return WVR_GetHandGestureData_S(ref data);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_StartHandTracking_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartHandTracking_S(WVR_HandTrackerType type);
		public override WVR_Result StartHandTracking(WVR_HandTrackerType type)
		{
			return WVR_StartHandTracking_S(type);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_StopHandTracking_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopHandTracking_S(WVR_HandTrackerType type);
		public override void StopHandTracking(WVR_HandTrackerType type)
		{
			WVR_StopHandTracking_S(type);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetHandJointCount_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandJointCount_S(WVR_HandTrackerType type, ref uint jointCount);
		public override WVR_Result GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount)
		{
			return WVR_GetHandJointCount_S(type, ref jointCount);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetHandTrackerInfo_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandTrackerInfo_S(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info);
		public override WVR_Result GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info)
		{
			return WVR_GetHandTrackerInfo_S(type, ref info);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetHandTrackingData_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandTrackingData_S(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_DPHandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t pose);
		public override WVR_Result GetHandTrackingData(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_HandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t pose)
		{
			WVR_DPHandTrackingData_t handTrackerDPData = new WVR_DPHandTrackingData_t();
			WVR_Result result = WVR_GetHandTrackingData_S(trackerType, modelType, originModel, ref handTrackerDPData, ref pose);
			if (result == WVR_Result.WVR_Success)
			{
				handTrackerData.right.isValidPose = handTrackerDPData.right.isValidPose;
				handTrackerData.right.confidence = handTrackerDPData.right.confidence;
				handTrackerData.right.jointCount = handTrackerDPData.right.jointCount;
				handTrackerData.right.joints = handTrackerDPData.right.joints;
				handTrackerData.right.scale = handTrackerDPData.right.scale;
				handTrackerData.right.wristLinearVelocity = handTrackerDPData.right.wristLinearVelocity;
				handTrackerData.right.wristAngularVelocity = handTrackerDPData.right.wristAngularVelocity;
				handTrackerData.left.isValidPose = handTrackerDPData.left.isValidPose;
				handTrackerData.left.confidence = handTrackerDPData.left.confidence;
				handTrackerData.left.jointCount = handTrackerDPData.left.jointCount;
				handTrackerData.left.joints = handTrackerDPData.left.joints;
				handTrackerData.left.scale = handTrackerDPData.left.scale;
				handTrackerData.left.wristLinearVelocity = handTrackerDPData.left.wristLinearVelocity;
				handTrackerData.left.wristAngularVelocity = handTrackerDPData.left.wristAngularVelocity;
			}
			return result;
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_ControllerSupportElectronicHand_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_ControllerSupportElectronicHand_S();
		public override bool ControllerSupportElectronicHand()
		{
			return WVR_ControllerSupportElectronicHand_S();
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_EnhanceHandStable_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_EnhanceHandStable_S(bool wear);
		public override void EnhanceHandStable(bool wear)
		{
			WVR_EnhanceHandStable_S(wear);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_IsEnhanceHandStable_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsEnhanceHandStable_S();
		public override bool IsEnhanceHandStable()
		{
			return WVR_IsEnhanceHandStable_S();
		}
		#endregion

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Render_Image_S")]
		public static extern void WVR_Render_Image_S(bool isRenderImageState, bool isRenderImageStateUpdate);

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetSupportedFeatures_S", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong WVR_GetSupportedFeatures_S();
		public override ulong GetSupportedFeatures()
		{
			return WVR_GetSupportedFeatures_S();
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Init_S")]
		public static extern SIM_InitError WVR_Init_S(int a, System.IntPtr ip, bool enablePreview, bool saveLogToFile, bool saveImage);

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Quit_S")]
		public static extern void WVR_Quit_S();

		// Button press and touch state.
		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetInputDeviceState_S")]
		public static extern bool WVR_GetInputDeviceState_S(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
			[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount);
		public override bool GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
			[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
		{
			return WVR_GetInputDeviceState_S(type, inputMask, ref buttons, ref touches, analogArray, analogArrayCount);
		}

		// Count of specified button type.
		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetInputTypeCount_S")]
		public static extern int WVR_GetInputTypeCount_S(WVR_DeviceType type, WVR_InputType inputType);
		public override int GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
		{
			return WVR_GetInputTypeCount_S(type, inputType);
		}


		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetInputButtonState_S")]
		public static extern bool WVR_GetInputButtonState_S(int WVR_DeviceType, int WVR_InputId);
		public override bool GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputButtonState_S((int)type, (int)id);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetInputTouchState_S")]
		public static extern bool WVR_GetInputTouchState_S(int WVR_DeviceType, int WVR_InputId);
		public override bool GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputTouchState_S((int)type, (int)id);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetInputAnalogAxis_S")]
		public static extern WVR_Axis_t WVR_GetInputAnalogAxis_S(int WVR_DeviceType, int WVR_InputId);
		public override WVR_Axis_t GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputAnalogAxis_S((int)type, (int)id);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_IsDeviceConnected_S")]
		public static extern bool WVR_IsDeviceConnected_S(int WVR_DeviceType);
		public override bool IsDeviceConnected(WVR_DeviceType type)
		{
			return WVR_IsDeviceConnected_S((int)type);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetDegreeOfFreedom_S")]
		public static extern int WVR_GetDegreeOfFreedom_S(int WVR_DeviceType);
		public override WVR_NumDoF GetDegreeOfFreedom(WVR_DeviceType type)
		{
			if (WVR_GetDegreeOfFreedom_S((int)type) == 0)
				return WVR_NumDoF.WVR_NumDoF_3DoF;
			return WVR_NumDoF.WVR_NumDoF_6DoF;
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetDeviceBatteryPercentage_S")]
		public static extern float WVR_GetDeviceBatteryPercentage_S(int type);
		public override float GetDeviceBatteryPercentage(WVR_DeviceType type)
		{
			return WVR_GetDeviceBatteryPercentage_S((int)type);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetSyncPose_S")]
		//[return : MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.SysInt , SizeConst = 10)]
		public static extern void WVR_GetSyncPose_S(int WVR_PoseOriginModel, [In, Out] WVR_DevicePosePair_t[] poseArray, int PoseCount);
		public override void GetSyncPose(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
		{
			WVR_GetSyncPose_S((int)originModel, poseArray, (int)pairArrayCount);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetParameters_S")]
		//[return : MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.SysInt , SizeConst = 10)]
		public static extern int WVR_GetParameters_S(int WVR_DeviceType, System.IntPtr pchValue, System.IntPtr retValue, uint unBufferSize);
		public override uint GetParameters(WVR_DeviceType type, IntPtr pchValue, IntPtr retValue, uint unBufferSize)
		{
			return (uint)WVR_GetParameters_S((int)type, pchValue, retValue, unBufferSize);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_PollEventQueue_S")]
		//[return : MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.SysInt , SizeConst = 10)]
		public static extern bool WVR_PollEventQueue_S(ref WVR_Event_t t);
		public override bool PollEventQueue(ref WVR_Event_t e)
		{
			return WVR_PollEventQueue_S(ref e);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetRenderTargetSize_S")]
		public static extern void WVR_GetRenderTargetSize_S(ref uint width, ref uint height);
		public override void GetRenderTargetSize(ref uint width, ref uint height)
		{
			WVR_GetRenderTargetSize_S(ref width, ref height);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetTransformFromEyeToHead_S")]
		public static extern WVR_Matrix4f_t WVR_GetTransformFromEyeToHead_S(WVR_Eye eye, WVR_NumDoF dof);
		public override WVR_Matrix4f_t GetTransformFromEyeToHead(WVR_Eye eye, WVR_NumDoF dof)
		{
			return WVR_GetTransformFromEyeToHead_S(eye, dof);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetClippingPlaneBoundary_S")]
		public static extern void WVR_GetClippingPlaneBoundary_S(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom);
		public override void GetClippingPlaneBoundary(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom)
		{
			WVR_GetClippingPlaneBoundary_S(eye, ref left, ref right, ref top, ref bottom);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetRenderProps_S")]
		public static extern bool WVR_GetRenderProps_S(ref WVR_RenderProps_t props);
		public override bool GetRenderProps(ref WVR_RenderProps_t props)
		{
			return WVR_GetRenderProps_S(ref props);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_GetCurrentControllerModel_S")]
		public static extern WVR_Result WVR_GetCurrentControllerModel_S(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone);
		public override WVR_Result GetCurrentControllerModel(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone)
		{
			return WVR_GetCurrentControllerModel_S(type, ref ctrlerModel, isOneBone);
		}

		[DllImportAttribute("wvr_plugins_directpreview", EntryPoint = "WVR_ReleaseControllerModel_S")]
		public static extern void WVR_ReleaseControllerModel_S(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */);
		public override void ReleaseControllerModel(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */)
		{
			WVR_ReleaseControllerModel_S(ref ctrlerModel /* WVR_CtrlerModel* */);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetPrintCallback")]
		public static extern void WVR_SetPrintCallback_S(debugcallback callback);

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetRenderImageHandles")]
		public static extern bool WVR_SetRenderImageHandles(IntPtr[] ttPtr);

		public static void PrintLog(string msg)
		{
			UnityEngine.Debug.Log("WVR_DirectPreview : " + msg);
		}
		// wvr.h
		public override WVR_InitError Init(WVR_AppType eType)
		{
			return WVR_InitError.WVR_InitError_None;
		}

		public override void PostInit()
		{
			UnityEngine.Debug.LogWarning("WaveVR_Render Instance is null, skip PostInit");
		}

		public override void Quit()
		{
			WVR_Quit_S();
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_TriggerVibrationScale_S")]
		public static extern void WVR_TriggerVibrationScale_S(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, float amplitude);
		public override void TriggerVibrationScale(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, float amplitude)
		{
			WVR_TriggerVibrationScale_S(type, id, durationMicroSec, frequency, amplitude);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_TriggerVibration_S")]
		public static extern void WVR_TriggerVibration_S(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity);
		public override void TriggerVibration(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity)
		{
			WVR_TriggerVibration_S(type, id, durationMicroSec, frequency, intensity);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetControllerPoseMode_S")]
		public static extern bool WVR_SetControllerPoseMode_S(WVR_DeviceType type, WVR_ControllerPoseMode mode);
		public override bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			return WVR_SetControllerPoseMode_S(type, mode);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetControllerPoseMode_S")]
		public static extern bool WVR_GetControllerPoseMode_S(WVR_DeviceType type, ref WVR_ControllerPoseMode mode);
		public override bool GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
		{
			return WVR_GetControllerPoseMode_S(type, ref mode);
		}

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_GetControllerPoseModeOffset_S")]
		public static extern bool WVR_GetControllerPoseModeOffset_S(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion);
		public override bool GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
		{
			return WVR_GetControllerPoseModeOffset_S(type, mode, ref translation, ref quaternion);
		}

		bool leftCall = false;
		bool rightCall = false;

		public static long getCurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
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
	}
#endif
}
