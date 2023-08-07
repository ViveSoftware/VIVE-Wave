// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Runtime.InteropServices;

namespace Wave.Native
{
#if UNITY_EDITOR
	public class WVR_Editor : Wave.Native.Interop.WVR_Base
	{
		private static WVR_EditorImpl system
		{
			get
			{
				return WVR_EditorImpl.Instance;
			}
		}

		~WVR_Editor()
		{
		}

		#region [Event]
		public override bool PollEventQueue(ref WVR_Event_t e)
		{
			return system.PollEventQueue(ref e);
		}

		public override int GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
		{
			return system.GetInputTypeCount(type, inputType);
		}

		public override bool GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
		{
			return system.GetInputButtonState(type, id);
		}

		public override bool GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
		{
			return system.GetInputTouchState(type, id);
		}

		public override WVR_Axis_t GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
		{
			return system.GetInputAnalogAxis(type, id);
		}

		public override bool GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
				[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
		{
			return system.GetInputDeviceState(type, inputMask, ref buttons, ref touches, analogArray, analogArrayCount);
		}
		#endregion

		#region Device Pose
		public override void GetSyncPose(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
		{
			system.GetSyncPose(originModel, poseArray, pairArrayCount);
		}
		#endregion

		#region Simulation Pose
		public override void SetArmModel(WVR_SimulationType simulationType)
		{
			system.SetArmModel(simulationType);
		}

		public override void SetArmSticky(bool stickyArm)
		{
			system.SetArmSticky(stickyArm);
		}

		public override void SetNeckModelEnabled(bool enabled)
		{
			system.SetNeckModelEnabled(enabled);
		}
		#endregion

		#region Key Mapping
		public override bool SetInputRequest(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size)
		{
			return system.SetInputRequest(type, request, size);
		}

		public override uint GetInputMappingTable(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size)
		{
			return system.GetInputMappingTable(type, table, size);
		}

		public override bool GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair)
		{
			return system.GetInputMappingPair(type, destination, ref pair);
		}
		#endregion

		#region Interaction Mode
		[System.Obsolete("This is an obsolete function.", true)]
		public override bool SetInteractionMode(WVR_InteractionMode mode)
		{
			return system.SetInteractionMode(mode);
		}

		public override WVR_InteractionMode GetInteractionMode()
		{
			return system.GetInteractionMode();
		}

		public override bool SetGazeTriggerType(WVR_GazeTriggerType type)
		{
			return system.SetGazeTriggerType(type);
		}
		public override WVR_GazeTriggerType GetGazeTriggerType()
		{
			return system.GetGazeTriggerType();
		}
		#endregion

		#region Arena
		[System.Obsolete("This API is deprecated and is no longer supported.", true)]
		public override bool SetArena(ref WVR_Arena_t arena)
		{
			return system.SetArena(ref arena);
		}

		public override WVR_Arena_t GetArena()
		{
			return system.GetArena();
		}

		public override bool IsOverArenaRange()
		{
			return system.IsOverArenaRange();
		}

		public override void SetArenaVisible(WVR_ArenaVisible config)
		{
			system.SetArenaVisible(config);
		}

		public override WVR_ArenaVisible GetArenaVisible()
		{
			return system.GetArenaVisible();
		}
		#endregion

		#region Focused Controller
		public override WVR_DeviceType GetFocusedController()
		{
			return system.GetFocusedController();
		}

		public override void SetFocusedController(WVR_DeviceType focusController)
		{
			system.SetFocusedController(focusController);
		}
		#endregion

		#region Eye Tracking
		public override WVR_Result StartEyeTracking()
		{
			return system.StartEyeTracking();
		}

		public override void StopEyeTracking()
		{
			system.StopEyeTracking();
		}

		public override WVR_Result GetEyeTracking(ref WVR_EyeTracking_t data, WVR_CoordinateSystem space)
		{
			return system.GetEyeTracking(ref data);
		}
		#endregion

		#region Hand
		public override WVR_Result GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
		{
			return system.GetHandGestureInfo(ref info);
		}
		public override WVR_Result StartHandGesture(ulong demands)
		{
			return system.StartHandGesture(demands);
		}
		public override void StopHandGesture()
		{
			system.StopHandGesture();
		}
		public override WVR_Result GetHandGestureData(ref WVR_HandGestureData_t data)
		{
			return system.GetHandGestureData(ref data);
		}
		public override WVR_Result StartHandTracking(WVR_HandTrackerType type)
		{
			return system.StartHandTracking(type);
		}
		public override void StopHandTracking(WVR_HandTrackerType type)
		{
			system.StopHandTracking(type);
		}
		public override WVR_Result GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount)
		{
			return system.GetHandJointCount(type, ref jointCount);
		}
		public override WVR_Result GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info)
		{
			return system.GetHandTrackerInfo(type, ref info);
		}
		public override WVR_Result GetHandTrackingData(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_HandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t pose)
		{
			return system.GetHandTrackingData(trackerType, modelType, originModel, ref handTrackerData, ref pose);
		}
		public override bool ControllerSupportElectronicHand() { return system.ControllerSupportElectronicHand(); }
		public override void EnhanceHandStable(bool wear) { system.EnhanceHandStable(wear); }
		public override bool IsEnhanceHandStable() { return system.IsEnhanceHandStable(); }

		public override void SetMixMode(bool enable)
		{
			system.SetMixMode(enable);
		}
		#endregion

		#region Controller Pose Mode
		public override bool GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
		{
			return system.GetControllerPoseModeOffset(type, mode, ref translation, ref quaternion);
		}
		public override bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			return system.SetControllerPoseMode(type, mode);
		}
		public override bool GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
		{
			return system.GetControllerPoseMode(type, ref mode);
		}
		#endregion

		#region Tracker
		public override WVR_Result StartTracker()
		{
			return system.StartTracker();
		}
		public override void StopTracker()
		{
			system.StopTracker();
		}
		public override bool IsTrackerConnected(WVR_TrackerId trackerId)
		{
			return system.IsTrackerConnected(trackerId);
		}
		public override WVR_TrackerRole GetTrackerRole(WVR_TrackerId trackerId)
		{
			return system.GetTrackerRole(trackerId);
		}
		public override WVR_Result GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities)
		{
			return system.GetTrackerCapabilities(trackerId, ref capabilities);
		}
		public override WVR_Result GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			return system.GetTrackerPoseState(trackerId, originModel, predictedMilliSec, ref poseState);
		}
		public override Int32 GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType)
		{
			return system.GetTrackerInputDeviceCapability(trackerId, inputType);
		}
		public override WVR_AnalogType GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return system.GetTrackerInputDeviceAnalogType(trackerId, id);
		}
		public override bool GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return system.GetTrackerInputButtonState(trackerId, id);
		}
		public override bool GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return system.GetTrackerInputTouchState(trackerId, id);
		}
		public override WVR_Axis_t GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return system.GetTrackerInputAnalogAxis(trackerId, id);
		}
		public override float GetTrackerBatteryLevel(WVR_TrackerId trackerId)
		{
			return system.GetTrackerBatteryLevel(trackerId);
		}
		public override WVR_Result TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f)
		{
			return system.TriggerTrackerVibration(trackerId, durationMicroSec, frequency, amplitude);
		}
		public override IntPtr GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp)
		{
			return system.GetTrackerExtendedData(trackerId, ref exDataSize, ref timestamp);
		}
		public override WVR_Result GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, ref IntPtr deviceName)
		{
			return system.GetTrackerDeviceName(trackerId, ref nameSize, ref deviceName);
		}
		public override void SetFocusedTracker(int focusedTracker)
		{
			system.SetFocusedTracker(focusedTracker);
		}
		public override int GetFocusedTracker()
		{
			return system.GetFocusedTracker();
		}
		#endregion

		#region wvr_notifydeviceinfo.h
		public override WVR_Result StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize)
		{
			return system.StartNotifyDeviceInfo(type, unBufferSize);
		}
		public override void StopNotifyDeviceInfo(WVR_DeviceType type)
		{
			system.StopNotifyDeviceInfo(type);
		}
		public override void UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue)
		{
			system.UpdateNotifyDeviceInfo(type, dataValue);
		}
		#endregion

		#region Lip Expression
		public override WVR_Result StartLipExp()
		{
			return system.StartLipExp();
		}
		public override WVR_Result GetLipExpData([In, Out] float[] value)
		{
			return system.GetLipExpData(value);
		}
		public override void StopLipExp()
		{
			system.StopLipExp();
		}
		#endregion

		public override bool IsDeviceConnected(WVR_DeviceType type)
		{
			return system.IsDeviceConnected(type);
		}

		public override bool IsInputFocusCapturedBySystem()
		{
			return system.IsInputFocusCapturedBySystem();
		}

		public override void InAppRecenter(WVR_RecenterType recenterType)
		{
			system.InAppRecenter(recenterType);
		}

		public override ulong GetSupportedFeatures()
		{
			return system.GetSupportedFeatures();
		}

		public override bool IsDeviceTableStatic(WVR_DeviceType type)
		{
			return system.IsDeviceTableStatic(type);
		}
	}
#endif
}
