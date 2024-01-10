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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wave.Native
{
	public class WVR_Android : Wave.Native.Interop.WVR_Base
	{
		#region wvr_events.h
		// Events: swipe, battery status.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_PollEventQueue", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_PollEventQueue_Android(ref WVR_Event_t e);
		public override bool PollEventQueue(ref WVR_Event_t e)
		{
			return WVR_PollEventQueue_Android(ref e);
		}
		#endregion

		#region wvr_device.h
		// Button types for which device is capable.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputDeviceCapability", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WVR_GetInputDeviceCapability_Android(WVR_DeviceType type, WVR_InputType inputType);
		public override int GetInputDeviceCapability(WVR_DeviceType type, WVR_InputType inputType)
		{
			return WVR_GetInputDeviceCapability_Android(type, inputType);
		}

		// Get analog type for which device.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputDeviceAnalogType", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_AnalogType WVR_GetInputDeviceAnalogType_Android(WVR_DeviceType type, WVR_InputId id);
		public override WVR_AnalogType GetInputDeviceAnalogType(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputDeviceAnalogType_Android(type, id);
		}

		// Button press and touch state.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputDeviceState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetInputDeviceState_Android(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
			[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount);
		public override bool GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
			[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
		{
			return WVR_GetInputDeviceState_Android(type, inputMask, ref buttons, ref touches, analogArray, analogArrayCount);
		}

		// Count of specified button type.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputTypeCount", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WVR_GetInputTypeCount_Android(WVR_DeviceType type, WVR_InputType inputType);
		public override int GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
		{
			return WVR_GetInputTypeCount_Android(type, inputType);
		}

		// Button press state.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputButtonState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetInputButtonState_Android(WVR_DeviceType type, WVR_InputId id);
		public override bool GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputButtonState_Android(type, id);
		}

		// Button touch state.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputTouchState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetInputTouchState_Android(WVR_DeviceType type, WVR_InputId id);
		public override bool GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputTouchState_Android(type, id);
		}

		// Axis of analog button: touchpad (x, y), trigger (x only)
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputAnalogAxis", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Axis_t WVR_GetInputAnalogAxis_Android(WVR_DeviceType type, WVR_InputId id);
		public override WVR_Axis_t GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_GetInputAnalogAxis_Android(type, id);
		}

		// Get transform of specified device.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetPoseState", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetPoseState_Android(WVR_DeviceType type, WVR_PoseOriginModel originModel, uint predictedMilliSec, ref WVR_PoseState_t poseState);
		public override void GetPoseState(WVR_DeviceType type, WVR_PoseOriginModel originModel, uint predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			WVR_GetPoseState_Android(type, originModel, predictedMilliSec, ref poseState);
		}

		// Get all attributes of pose of all devices.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSyncPose", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetSyncPose_Android(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount);
		public override void GetSyncPose(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
		{
			WVR_GetSyncPose_Android(originModel, poseArray, pairArrayCount);
		}

		// Device connection state.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsDeviceConnected", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsDeviceConnected_Android(WVR_DeviceType type);
		public override bool IsDeviceConnected(WVR_DeviceType type)
		{
			return WVR_IsDeviceConnected_Android(type);
		}

		// Make device vibration.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_TriggerVibration", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_TriggerVibration_Android(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity);
		public override void TriggerVibration(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity)
		{
			WVR_TriggerVibration_Android(type, id, durationMicroSec, frequency, intensity);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_TriggerVibrationScale", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_TriggerVibrationScale_Android(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, float amplitude);
		public override void TriggerVibrationScale(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, float amplitude)
		{
			WVR_TriggerVibrationScale_Android(type, id, durationMicroSec, frequency, amplitude);
		}

		// Recenter the "Virtual World" in current App.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_InAppRecenter", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_InAppRecenter_Android(WVR_RecenterType recenterType);
		public override void InAppRecenter(WVR_RecenterType recenterType)
		{
			WVR_InAppRecenter_Android(recenterType);
		}

		// Enables or disables screen protection
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetScreenProtection", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetScreenProtection_Android(bool enabled);
		public override void SetScreenProtection(bool enabled)
		{
			WVR_SetScreenProtection_Android(enabled);
		}

		// Enables or disables use of the neck model for 3-DOF head tracking
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetNeckModelEnabled", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetNeckModelEnabled_Android(bool enabled);
		public override void SetNeckModelEnabled(bool enabled)
		{
			WVR_SetNeckModelEnabled_Android(enabled);
		}

		// Decide Neck Model on/off/3dofOn
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetNeckModel", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetNeckModel_Android(WVR_SimulationType simulationType);
		public override void SetNeckModel(WVR_SimulationType simulationType)
		{
			WVR_SetNeckModel_Android(simulationType);
		}

		// Decide Arm Model on/off/3dofOn
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetArmModel", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetArmModel_Android(WVR_SimulationType simulationType);
		public override void SetArmModel(WVR_SimulationType simulationType)
		{
			WVR_SetArmModel_Android(simulationType);
		}

		// Decide Arm Model behaviors
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetArmSticky", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetArmSticky_Android(bool stickyArm);
		public override void SetArmSticky(bool stickyArm)
		{
			WVR_SetArmSticky_Android(stickyArm);
		}

		// bool WVR_SetInputRequest(WVR_DeviceType type, const WVR_InputAttribute* request, uint32_t size);
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetInputRequest", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetInputRequest_Android(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size);
		public override bool SetInputRequest(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size)
		{
			return WVR_SetInputRequest_Android(type, request, size);
		}

		// bool WVR_GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, WVR_InputMappingPair* pair);
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputMappingPair", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetInputMappingPair_Android(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair);
		public override bool GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair)
		{
			return WVR_GetInputMappingPair_Android(type, destination, ref pair);
		}

		// uint32_t WVR_GetInputMappingTable(WVR_DeviceType type, WVR_InputMappingPair* table, uint32_t size);
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInputMappingTable", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint WVR_GetInputMappingTable_Android(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size);
		public override uint GetInputMappingTable(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size)
		{
			return WVR_GetInputMappingTable_Android(type, table, size);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetControllerPoseMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode);
		public override bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			return WVR_SetControllerPoseMode(type, mode);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetControllerPoseMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode);
		public override bool GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
		{
			return WVR_GetControllerPoseMode(type, ref mode);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetControllerPoseModeOffset", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion);
		public override bool GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
		{
			return WVR_GetControllerPoseModeOffset(type, mode, ref translation, ref quaternion);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsDeviceTableStatic", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsDeviceTableStatic(WVR_DeviceType type);
		public override bool IsDeviceTableStatic(WVR_DeviceType type)
		{
			return WVR_IsDeviceTableStatic(type);
		}
		#endregion

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSpectatorRenderTargetSize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetSpectatorRenderTargetSize(ref uint width, ref uint height);
		public override void GetSpectatorRenderTargetSize(ref uint width, ref uint height)
		{
			WVR_GetSpectatorRenderTargetSize(ref width, ref height);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSpectatorClippingPlaneBoundary", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetSpectatorClippingPlaneBoundary(ref float l, ref float r, ref float t, ref float b);
		public override void GetSpectatorClippingPlaneBoundary(ref float l, ref float r, ref float t, ref float b)
		{
			WVR_GetSpectatorClippingPlaneBoundary(ref l, ref r, ref t, ref b);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_PreSpectatorRender", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_PreSpectatorRender(ref WVR_SpectatorState state);
		public override bool PreSpectatorRender(ref WVR_SpectatorState state)
		{
			return WVR_PreSpectatorRender(ref state);
		}

		#region wvr_tracker.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartTracker", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartTracker();
		public override WVR_Result StartTracker()
		{
			return WVR_StartTracker();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopTracker", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopTracker();
		public override void StopTracker()
		{
			WVR_StopTracker();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsTrackerConnected", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsTrackerConnected(WVR_TrackerId trackerId);
		public override bool IsTrackerConnected(WVR_TrackerId trackerId)
		{
			return WVR_IsTrackerConnected(trackerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerRole", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_TrackerRole WVR_GetTrackerRole(WVR_TrackerId trackerId);
		public override WVR_TrackerRole GetTrackerRole(WVR_TrackerId trackerId)
		{
			return WVR_GetTrackerRole(trackerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerCapabilities", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities);
		public override WVR_Result GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities)
		{
			return WVR_GetTrackerCapabilities(trackerId, ref capabilities);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerPoseState", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState);
		public override WVR_Result GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			return WVR_GetTrackerPoseState(trackerId, originModel, predictedMilliSec, ref poseState);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerInputDeviceCapability", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 WVR_GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType);
		public override Int32 GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType)
		{
			return WVR_GetTrackerInputDeviceCapability(trackerId, inputType);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerInputDeviceAnalogType", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_AnalogType WVR_GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id);
		public override WVR_AnalogType GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_GetTrackerInputDeviceAnalogType(trackerId, id);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerInputButtonState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id);
		public override bool GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_GetTrackerInputButtonState(trackerId, id);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerInputTouchState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id);
		public override bool GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_GetTrackerInputTouchState(trackerId, id);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerInputAnalogAxis", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Axis_t WVR_GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id);
		public override WVR_Axis_t GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_GetTrackerInputAnalogAxis(trackerId, id);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerBatteryLevel", CallingConvention = CallingConvention.Cdecl)]
		public static extern float WVR_GetTrackerBatteryLevel(WVR_TrackerId trackerId);
		public override float GetTrackerBatteryLevel(WVR_TrackerId trackerId)
		{
			return WVR_GetTrackerBatteryLevel(trackerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_TriggerTrackerVibration", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f);
		public override WVR_Result TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f)
		{
			return WVR_TriggerTrackerVibration(trackerId, durationMicroSec, frequency, amplitude);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerExtendedData", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WVR_GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp);
		public override IntPtr GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp)
		{
			return WVR_GetTrackerExtendedData(trackerId, ref exDataSize, ref timestamp);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackerDeviceName", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, IntPtr deviceName);
		public override WVR_Result GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, ref IntPtr deviceName)
		{
			return WVR_GetTrackerDeviceName(trackerId, ref nameSize, deviceName);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RegisterTrackerInfoCallback", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_RegisterTrackerInfoCallback(ref WVR_TrackerInfoNotify notify);
		public override WVR_Result RegisterTrackerInfoCallback(ref WVR_TrackerInfoNotify notify)
		{
			return WVR_RegisterTrackerInfoCallback(ref notify);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_UnregisterTrackerInfoCallback", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_UnregisterTrackerInfoCallback();
		public override WVR_Result UnregisterTrackerInfoCallback()
		{
			return WVR_UnregisterTrackerInfoCallback();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetFocusedTracker", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetFocusedTracker(int focusedTracker);
		public override void SetFocusedTracker(int focusedTracker)
		{
			WVR_SetFocusedTracker(focusedTracker);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetFocusedTracker", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WVR_GetFocusedTracker();
		public override int GetFocusedTracker()
		{
			return WVR_GetFocusedTracker();
		}
		#endregion

		#region wvr_arena.h
		// Get current attributes of arena.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetArena", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Arena_t WVR_GetArena_Android();
		public override WVR_Arena_t GetArena()
		{
			return WVR_GetArena_Android();
		}

		// Set up arena.
		[Obsolete("This API is deprecated and is no longer supported.", true)]
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetArena", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetArena_Android(ref WVR_Arena_t arena);
		[Obsolete("This API is deprecated and is no longer supported.", true)]
		public override bool SetArena(ref WVR_Arena_t arena)
		{
			return WVR_SetArena_Android(ref arena);
		}

		// Get visibility type of arena.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetArenaVisible", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_ArenaVisible WVR_GetArenaVisible_Android();
		public override WVR_ArenaVisible GetArenaVisible()
		{
			return WVR_GetArenaVisible_Android();
		}

		// Set visibility type of arena.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetArenaVisible", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetArenaVisible_Android(WVR_ArenaVisible config);
		public override void SetArenaVisible(WVR_ArenaVisible config)
		{
			WVR_SetArenaVisible_Android(config);
		}

		// Check if player is over range of arena.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsOverArenaRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsOverArenaRange_Android();
		public override bool IsOverArenaRange()
		{
			return WVR_IsOverArenaRange_Android();
		}
		#endregion

		#region wvr_status.h
		// Battery electricity (%).
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetDeviceBatteryPercentage", CallingConvention = CallingConvention.Cdecl)]
		public static extern float WVR_GetDeviceBatteryPercentage_Android(WVR_DeviceType type);
		public override float GetDeviceBatteryPercentage(WVR_DeviceType type)
		{
			return WVR_GetDeviceBatteryPercentage_Android(type);
		}

		// Battery life status.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetBatteryStatus", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_BatteryStatus WVR_GetBatteryStatus_Android(WVR_DeviceType type);
		public override WVR_BatteryStatus GetBatteryStatus(WVR_DeviceType type)
		{
			return WVR_GetBatteryStatus_Android(type);
		}

		// Battery is charging or not.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetChargeStatus", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_ChargeStatus WVR_GetChargeStatus_Android(WVR_DeviceType type);
		public override WVR_ChargeStatus GetChargeStatus(WVR_DeviceType type)
		{
			return WVR_GetChargeStatus_Android(type);
		}

		// Whether battery is overheat.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetBatteryTemperatureStatus", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_BatteryTemperatureStatus WVR_GetBatteryTemperatureStatus_Android(WVR_DeviceType type);
		public override WVR_BatteryTemperatureStatus GetBatteryTemperatureStatus(WVR_DeviceType type)
		{
			return WVR_GetBatteryTemperatureStatus_Android(type);
		}

		// Battery temperature.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetBatteryTemperature", CallingConvention = CallingConvention.Cdecl)]
		public static extern float WVR_GetBatteryTemperature_Android(WVR_DeviceType type);
		public override float GetBatteryTemperature(WVR_DeviceType type)
		{
			return WVR_GetBatteryTemperature_Android(type);
		}
		#endregion

		#region wvr_eyetracking.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartEyeTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartEyeTracking_Android();
		public override WVR_Result StartEyeTracking()
		{
			return WVR_StartEyeTracking_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopEyeTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopEyeTracking_Android();
		public override void StopEyeTracking()
		{
			WVR_StopEyeTracking_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetEyeTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetEyeTracking_Android(ref WVR_EyeTracking_t data, WVR_CoordinateSystem space);
		public override WVR_Result GetEyeTracking(ref WVR_EyeTracking_t data, WVR_CoordinateSystem space)
		{
			return WVR_GetEyeTracking_Android(ref data, space);
		}
		#endregion

		#region wvr_hand.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetHandGestureInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandGestureInfo(ref WVR_HandGestureInfo_t info);
		public override WVR_Result GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
		{
			return WVR_GetHandGestureInfo(ref info);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartHandGesture", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartHandGesture(ulong demands);
		public override WVR_Result StartHandGesture(ulong demands)
		{
			return WVR_StartHandGesture(demands);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopHandGesture", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopHandGesture();
		public override void StopHandGesture()
		{
			WVR_StopHandGesture();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetHandGestureData", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandGestureData(ref WVR_HandGestureData_t data);
		public override WVR_Result GetHandGestureData(ref WVR_HandGestureData_t data)
		{
			return WVR_GetHandGestureData(ref data);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartHandTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartHandTracking(WVR_HandTrackerType type);
		public override WVR_Result StartHandTracking(WVR_HandTrackerType type)
		{
			return WVR_StartHandTracking(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopHandTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopHandTracking(WVR_HandTrackerType type);
		public override void StopHandTracking(WVR_HandTrackerType type)
		{
			WVR_StopHandTracking(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetHandJointCount", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount);
		public override WVR_Result GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount)
		{
			return WVR_GetHandJointCount(type, ref jointCount);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetHandTrackerInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info);
		public override WVR_Result GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info)
		{
			return WVR_GetHandTrackerInfo(type, ref info);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetHandTrackingData", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetHandTrackingData(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_HandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t pose);
		public override WVR_Result GetHandTrackingData(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_HandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t pose)
		{
			return WVR_GetHandTrackingData(trackerType, modelType, originModel, ref handTrackerData, ref pose);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ControllerSupportElectronicHand", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_ControllerSupportElectronicHand();
		public override bool ControllerSupportElectronicHand()
		{
			return WVR_ControllerSupportElectronicHand();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnhanceHandStable", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_EnhanceHandStable(bool wear);
		public override void EnhanceHandStable(bool wear)
		{
			WVR_EnhanceHandStable(wear);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsEnhanceHandStable", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsEnhanceHandStable();
		public override bool IsEnhanceHandStable()
		{
			return WVR_IsEnhanceHandStable();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetMixMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetMixMode(bool enable);
		public override void SetMixMode(bool enable)
		{
			WVR_SetMixMode(enable);
		}
		#endregion

		#region wvr_notifydeviceinfo.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartNotifyDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize);
		public override WVR_Result StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize)
		{
			return WVR_StartNotifyDeviceInfo(type, unBufferSize);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopNotifyDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopNotifyDeviceInfo(WVR_DeviceType type);
		public override void StopNotifyDeviceInfo(WVR_DeviceType type)
		{
			WVR_StopNotifyDeviceInfo(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_UpdateNotifyDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue);
		public override void UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue)
		{
			WVR_UpdateNotifyDeviceInfo(type, dataValue);
		}
		#endregion

		#region wvr_lip_expression.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartLipExp", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartLipExp();
		public override WVR_Result StartLipExp()
		{
			return WVR_StartLipExp();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetLipExpData", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetLipExpData([In, Out] float[] value);
		public override WVR_Result GetLipExpData([In, Out] float[] value)
		{
			return WVR_GetLipExpData(value);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopLipExp", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopLipExp();
		public override void StopLipExp()
		{
			WVR_StopLipExp();
		}
		#endregion

		#region wvr_scene.h

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartScene", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartScene();
		public override WVR_Result StartScene()
		{
			return WVR_StartScene();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopScene", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopScene();

		public override void StopScene()
		{
			WVR_StopScene();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartScenePerception", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartScenePerception(WVR_ScenePerceptionTarget target);
		public override WVR_Result StartScenePerception(WVR_ScenePerceptionTarget target)
		{
			return WVR_StartScenePerception(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopScenePerception", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopScenePerception(WVR_ScenePerceptionTarget target);
		public override WVR_Result StopScenePerception(WVR_ScenePerceptionTarget target)
		{
			return WVR_StopScenePerception(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetScenePerceptionState", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetScenePerceptionState(WVR_ScenePerceptionTarget target, ref WVR_ScenePerceptionState state /* WVR_ScenePerceptionState* */);
		public override WVR_Result GetScenePerceptionState(WVR_ScenePerceptionTarget target, ref WVR_ScenePerceptionState state /* WVR_ScenePerceptionState* */)
		{
			return WVR_GetScenePerceptionState(target, ref state);
		
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetScenePlanes", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetScenePlanes([In, Out] WVR_ScenePlaneFilter[] planeFilter /* WVR_ScenePlaneFilter*,nullptr if no need filter. */, UInt32 planeCapacityInput, out UInt32 planeCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr planes /* WVR_ScenePlane* */);
		public override WVR_Result GetScenePlanes([In, Out] WVR_ScenePlaneFilter[] planeFilter /* WVR_ScenePlaneFilter*,nullptr if no need filter. */, UInt32 planeCapacityInput, out UInt32 planeCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr planes /* WVR_ScenePlane* */)
		{
			return WVR_GetScenePlanes(planeFilter, planeCapacityInput, out planeCountOutput, originModel, planes);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSceneMeshes", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetSceneMeshes(WVR_SceneMeshType meshType, UInt32 meshCapacityInput, out UInt32 meshCountOutput /* uint32_t* */, IntPtr meshes /* WVR_SceneMesh* */);
		public override WVR_Result GetSceneMeshes(WVR_SceneMeshType meshType, UInt32 meshCapacityInput, out UInt32 meshCountOutput /* uint32_t* */, IntPtr meshes /* WVR_SceneMesh* */)
		{
			return WVR_GetSceneMeshes(meshType, meshCapacityInput, out meshCountOutput, meshes);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSceneMeshBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetSceneMeshBuffer(UInt64 meshBufferId, WVR_PoseOriginModel originModel, ref WVR_SceneMeshBuffer sceneMeshBuffer /* WVR_SceneMeshBuffer* */);
		public override WVR_Result GetSceneMeshBuffer(UInt64 meshBufferId, WVR_PoseOriginModel originModel, ref WVR_SceneMeshBuffer sceneMeshBuffer /* WVR_SceneMeshBuffer* */)
		{
			return WVR_GetSceneMeshBuffer(meshBufferId, originModel, ref sceneMeshBuffer);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSceneObjects", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetSceneObjects(UInt32 objectCapacityInput, out UInt32 objectCountOutput, WVR_PoseOriginModel originModel, IntPtr objects /* WVR_SceneObject* */);
		public override WVR_Result GetSceneObjects(UInt32 objectCapacityInput, out UInt32 objectCountOutput, WVR_PoseOriginModel originModel, IntPtr objects)
		{
			return WVR_GetSceneObjects(objectCapacityInput, out objectCountOutput, originModel, objects);
		}

		#endregion

		#region wvr_anchor.h

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CreateSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_CreateSpatialAnchor([In, Out] WVR_SpatialAnchorCreateInfo[] createInfo /* WVR_SpatialAnchorCreateInfo* */, out UInt64 anchor /* WVR_SpatialAnchor* */);
		public override WVR_Result CreateSpatialAnchor([In, Out] WVR_SpatialAnchorCreateInfo[] createInfo /* WVR_SpatialAnchorCreateInfo* */, out UInt64 anchor /* WVR_SpatialAnchor* */)
		{
			return WVR_CreateSpatialAnchor(createInfo, out anchor);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_DestroySpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_DestroySpatialAnchor(UInt64 anchor /* WVR_SpatialAnchor */);
		public override WVR_Result DestroySpatialAnchor(UInt64 anchor /* WVR_SpatialAnchor */)
		{
			return WVR_DestroySpatialAnchor(anchor);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnumerateSpatialAnchors", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_EnumerateSpatialAnchors(UInt32 anchorCapacityInput, out UInt32 anchorCountOutput /* uint32_t* */, out UInt64 anchors /* WVR_SpatialAnchor* */);
		public override WVR_Result EnumerateSpatialAnchors(UInt32 anchorCapacityInput, out UInt32 anchorCountOutput /* uint32_t* */, out UInt64 anchors /* WVR_SpatialAnchor* */)
		{
			return WVR_EnumerateSpatialAnchors(anchorCapacityInput, out anchorCountOutput, out anchors);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSpatialAnchorState", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetSpatialAnchorState(UInt64 anchor /* WVR_SpatialAnchor */, WVR_PoseOriginModel originModel, out WVR_SpatialAnchorState anchorState /* WVR_SpatialAnchorState* */);
		public override WVR_Result GetSpatialAnchorState(UInt64 anchor /* WVR_SpatialAnchor */, WVR_PoseOriginModel originModel, out WVR_SpatialAnchorState anchorState /* WVR_SpatialAnchorState* */)
		{
			return WVR_GetSpatialAnchorState(anchor, originModel, out anchorState);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CacheSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_CacheSpatialAnchor(ref WVR_SpatialAnchorCacheInfo spatialAnchorPersistInfo);
		public override WVR_Result CacheSpatialAnchor(ref WVR_SpatialAnchorCacheInfo spatialAnchorPersistInfo)
		{
			return WVR_CacheSpatialAnchor(ref spatialAnchorPersistInfo);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_UncacheSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_UncacheSpatialAnchor(ref WVR_SpatialAnchorName cachedSpatialAnchorName);
		public override WVR_Result UncacheSpatialAnchor(ref WVR_SpatialAnchorName cachedSpatialAnchorName)
		{
			return WVR_UncacheSpatialAnchor(ref cachedSpatialAnchorName);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnumerateCachedSpatialAnchorNames", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_EnumerateCachedSpatialAnchorNames(
			UInt32 cachedSpatialAnchorNamesCapacityInput,
			out UInt32 cachedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] cachedSpatialAnchorNames);
		public override WVR_Result EnumerateCachedSpatialAnchorNames(
			UInt32 cachedSpatialAnchorNamesCapacityInput,
			out UInt32 cachedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] cachedSpatialAnchorNames)
		{
			return WVR_EnumerateCachedSpatialAnchorNames(cachedSpatialAnchorNamesCapacityInput, out cachedSpatialAnchorNamesCountOutput, cachedSpatialAnchorNames);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ClearCachedSpatialAnchors", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ClearCachedSpatialAnchors();

		public override WVR_Result ClearCachedSpatialAnchors()
		{
			return WVR_ClearCachedSpatialAnchors();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CreateSpatialAnchorFromCacheName", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_CreateSpatialAnchorFromCacheName(ref WVR_SpatialAnchorFromCacheNameCreateInfo createInfo, out UInt64 anchor /* WVR_SpatialAnchor */);
		public override WVR_Result CreateSpatialAnchorFromCacheName(ref WVR_SpatialAnchorFromCacheNameCreateInfo createInfo, out UInt64 anchor /* WVR_SpatialAnchor */)
		{
			anchor = 0;
			return WVR_CreateSpatialAnchorFromCacheName(ref createInfo, out anchor);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_PersistSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_PersistSpatialAnchor(ref WVR_SpatialAnchorPersistInfo spatialAnchorPersistInfo);
		public override WVR_Result PersistSpatialAnchor(ref WVR_SpatialAnchorPersistInfo spatialAnchorPersistInfo)
		{
			return WVR_PersistSpatialAnchor(ref spatialAnchorPersistInfo);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_UnpersistSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_UnpersistSpatialAnchor(ref WVR_SpatialAnchorName persistedSpatialAnchorName);

		public override WVR_Result UnpersistSpatialAnchor(ref WVR_SpatialAnchorName persistedSpatialAnchorName)
		{
			return WVR_UnpersistSpatialAnchor(ref persistedSpatialAnchorName);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnumeratePersistedSpatialAnchorNames", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_EnumeratePersistedSpatialAnchorNames(
			UInt32 persistedSpatialAnchorNamesCapacityInput,
			out UInt32 persistedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] persistedSpatialAnchorNames);
		public override WVR_Result EnumeratePersistedSpatialAnchorNames(
			UInt32 persistedSpatialAnchorNamesCapacityInput,
			out UInt32 persistedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] persistedSpatialAnchorNames)
		{
			return WVR_EnumeratePersistedSpatialAnchorNames(
				persistedSpatialAnchorNamesCapacityInput,
				out persistedSpatialAnchorNamesCountOutput,
				persistedSpatialAnchorNames);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ClearPersistedSpatialAnchors", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ClearPersistedSpatialAnchors();
        public override WVR_Result ClearPersistedSpatialAnchors()
        {
            return WVR_ClearPersistedSpatialAnchors();
        }


		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetPersistedSpatialAnchorCount", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetPersistedSpatialAnchorCount(ref WVR_PersistedSpatialAnchorCountGetInfo getInfo);

		public override WVR_Result GetPersistedSpatialAnchorCount(
			ref WVR_PersistedSpatialAnchorCountGetInfo getInfo)
		{
			return WVR_GetPersistedSpatialAnchorCount(ref getInfo);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CreateSpatialAnchorFromPersistenceName", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_CreateSpatialAnchorFromPersistenceName(
			ref WVR_SpatialAnchorFromPersistenceNameCreateInfo createInfo,
			out UInt64 anchor /* WVR_SpatialAnchor* */);

		public override WVR_Result CreateSpatialAnchorFromPersistenceName(
			ref WVR_SpatialAnchorFromPersistenceNameCreateInfo createInfo,
			out UInt64 anchor /* WVR_SpatialAnchor* */)
		{
			return WVR_CreateSpatialAnchorFromPersistenceName(ref createInfo, out anchor);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ExportPersistedSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ExportPersistedSpatialAnchor(
			ref WVR_SpatialAnchorName persistedSpatialAnchorName,
			UInt32 dataCapacityInput,
			out UInt32 dataCountOutput,
			[Out] byte[] data);

		public override WVR_Result ExportPersistedSpatialAnchor(
			ref WVR_SpatialAnchorName persistedSpatialAnchorName,
			UInt32 dataCapacityInput,
			out UInt32 dataCountOutput,
			[Out] byte[] data)
		{
			return WVR_ExportPersistedSpatialAnchor(ref persistedSpatialAnchorName, dataCapacityInput, out dataCountOutput, data);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ImportPersistedSpatialAnchor", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ImportPersistedSpatialAnchor(
			UInt32 dataCount,
			[In] byte[] data);

		public override WVR_Result ImportPersistedSpatialAnchor(
			UInt32 dataCount,
			[In] byte[] data)
		{
			return WVR_ImportPersistedSpatialAnchor(dataCount, data);
		}

		#endregion

		#region wvr_marker.h

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartMarker", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartMarker();
		public override WVR_Result StartMarker()
		{
			return WVR_StartMarker();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopMarker", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopMarker();
		public override void StopMarker()
		{
			WVR_StopMarker();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartMarkerObserver", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartMarkerObserver(WVR_MarkerObserverTarget target);
		public override WVR_Result StartMarkerObserver(WVR_MarkerObserverTarget target)
		{
			return WVR_StartMarkerObserver(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopMarkerObserver", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopMarkerObserver(WVR_MarkerObserverTarget target);
		public override WVR_Result StopMarkerObserver(WVR_MarkerObserverTarget target)
		{
			return WVR_StopMarkerObserver(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetMarkerObserverState", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetMarkerObserverState(WVR_MarkerObserverTarget target, out WVR_MarkerObserverState state);
		public override WVR_Result GetMarkerObserverState(WVR_MarkerObserverTarget target, out WVR_MarkerObserverState state)
		{
			return WVR_GetMarkerObserverState(target, out state);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartMarkerDetection", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartMarkerDetection(WVR_MarkerObserverTarget target);
		public override WVR_Result StartMarkerDetection(WVR_MarkerObserverTarget target)
		{
			return WVR_StartMarkerDetection(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopMarkerDetection", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopMarkerDetection(WVR_MarkerObserverTarget target);
		public override WVR_Result StopMarkerDetection(WVR_MarkerObserverTarget target)
		{
			return WVR_StopMarkerDetection(target);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetArucoMarkers", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetArucoMarkers(UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr markers /* WVR_ArucoMarker* */);
		public override WVR_Result GetArucoMarkers(UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr markers /* WVR_ArucoMarker* */)
		{
			return WVR_GetArucoMarkers(markerCapacityInput, out markerCountOutput, originModel, markers);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnumerateTrackableMarkers", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_EnumerateTrackableMarkers(WVR_MarkerObserverTarget target, UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, IntPtr markerIds /* WVR_Uuid* */);
		public override WVR_Result EnumerateTrackableMarkers(WVR_MarkerObserverTarget target, UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, IntPtr markerIds /* WVR_Uuid* */)
		{
			return WVR_EnumerateTrackableMarkers(target, markerCapacityInput, out markerCountOutput, markerIds);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CreateTrackableMarker", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_CreateTrackableMarker([In, Out] WVR_TrackableMarkerCreateInfo[] createInfo /* WVR_TrackableMarkerCreateInfo* */);
		public override WVR_Result CreateTrackableMarker([In, Out] WVR_TrackableMarkerCreateInfo[] createInfo /* WVR_TrackableMarkerCreateInfo* */)
		{
			return WVR_CreateTrackableMarker(createInfo);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_DestroyTrackableMarker", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_DestroyTrackableMarker(WVR_Uuid markerId);
		public override WVR_Result DestroyTrackableMarker(WVR_Uuid markerId)
		{
			return WVR_DestroyTrackableMarker(markerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StartTrackableMarkerTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StartTrackableMarkerTracking(WVR_Uuid markerId);
		public override WVR_Result StartTrackableMarkerTracking(WVR_Uuid markerId)
		{
			return WVR_StartTrackableMarkerTracking(markerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_StopTrackableMarkerTracking", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_StopTrackableMarkerTracking(WVR_Uuid markerId);
		public override WVR_Result StopTrackableMarkerTracking(WVR_Uuid markerId)
		{
			return WVR_StopTrackableMarkerTracking(markerId);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTrackableMarkerState", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetTrackableMarkerState(WVR_Uuid markerId, WVR_PoseOriginModel originModel, out WVR_TrackableMarkerState state /* WVR_TrackableMarkerState* */);
		public override WVR_Result GetTrackableMarkerState(WVR_Uuid markerId, WVR_PoseOriginModel originModel, out WVR_TrackableMarkerState state /* WVR_TrackableMarkerState* */)
		{
			return WVR_GetTrackableMarkerState(markerId, originModel, out state);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetArucoMarkerData", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetArucoMarkerData(WVR_Uuid markerId, out WVR_ArucoMarkerData data /* WVR_ArucoMarkerData* */);
		public override WVR_Result GetArucoMarkerData(WVR_Uuid markerId, out WVR_ArucoMarkerData data /* WVR_ArucoMarkerData* */)
		{
			return WVR_GetArucoMarkerData(markerId, out data);
		}

		#endregion

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetSupportedFeatures", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong WVR_GetSupportedFeatures();
		public override ulong GetSupportedFeatures()
		{
			return WVR_GetSupportedFeatures();
		}

		// wvr.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_Init", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_InitError WVR_Init_Android(WVR_AppType eType);
		public override WVR_InitError Init(WVR_AppType eType)
		{
			return WVR_Init_Android(eType);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_Quit", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_Quit_Android();
		public override void Quit()
		{
			WVR_Quit_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInitErrorString", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WVR_GetInitErrorString_Android(WVR_InitError error);
		public override IntPtr GetInitErrorString(WVR_InitError error)
		{
			return WVR_GetInitErrorString_Android(error);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetWaveRuntimeVersion", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint WVR_GetWaveRuntimeVersion_Android();
		public override uint GetWaveRuntimeVersion()
		{
			return WVR_GetWaveRuntimeVersion_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetWaveSDKVersion", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint WVR_GetWaveSDKVersion_Android();
		public override uint GetWaveSDKVersion()
		{
			return WVR_GetWaveSDKVersion_Android();
		}

		// wvr_system.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsInputFocusCapturedBySystem", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsInputFocusCapturedBySystem_Android();
		public override bool IsInputFocusCapturedBySystem()
		{
			return WVR_IsInputFocusCapturedBySystem_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RenderInit", CallingConvention = CallingConvention.Cdecl)]
		internal static extern WVR_RenderError WVR_RenderInit_Android(ref WVR_RenderInitParams_t param);
		internal override WVR_RenderError RenderInit(ref WVR_RenderInitParams_t param)
		{
			return WVR_RenderInit_Android(ref param);
		}

		// Set CPU and GPU performance level.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetPerformanceLevels", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool WVR_SetPerformanceLevels_Android(WVR_PerfLevel cpuLevel, WVR_PerfLevel gpuLevel);
		internal override bool SetPerformanceLevels(WVR_PerfLevel cpuLevel, WVR_PerfLevel gpuLevel)
		{
			return WVR_SetPerformanceLevels_Android(cpuLevel, gpuLevel);
		}

		// Allow WaveVR SDK runtime to adjust render quality and CPU/GPU perforamnce level automatically.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnableAdaptiveQuality", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool WVR_EnableAdaptiveQuality_Android(bool enable, uint flags);
		internal override bool EnableAdaptiveQuality(bool enable, uint flags)
		{
			return WVR_EnableAdaptiveQuality_Android(enable, flags);
		}

		// Check if adaptive quailty enabled.
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsAdaptiveQualityEnabled", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool WVR_IsAdaptiveQualityEnabled_Android();
		internal override bool IsAdaptiveQualityEnabled()
		{
			return WVR_IsAdaptiveQualityEnabled_Android();
		}

        // wvr_camera.h
        [DllImportAttribute("wvr_api", EntryPoint = "WVR_StartCamera", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_StartCamera_Android(ref WVR_CameraInfo_t info);
        public override bool StartCamera(ref WVR_CameraInfo_t info)
		{
			return WVR_StartCamera_Android(ref info);
		}

        [DllImportAttribute("wvr_api", EntryPoint = "WVR_StopCamera", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_StopCamera_Android();
        public override void StopCamera()
		{
			WVR_StopCamera_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_UpdateTexture", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_UpdateTexture_Android(uint textureid);
		public override bool UpdateTexture(IntPtr textureid)
		{
			return WVR_UpdateTexture_Android((uint)textureid);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetCameraIntrinsic", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetCameraIntrinsic_Android(WVR_CameraPosition position, ref WVR_CameraIntrinsic_t intrinsic);
		public override bool GetCameraIntrinsic(WVR_CameraPosition position, ref WVR_CameraIntrinsic_t intrinsic)
		{
			return WVR_GetCameraIntrinsic_Android(position, ref intrinsic);
		}

        [DllImportAttribute("wvr_api", EntryPoint = "WVR_GetCameraFrameBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetCameraFrameBuffer_Android(IntPtr pFramebuffer, uint frameBufferSize);
        public override bool GetCameraFrameBuffer(IntPtr pFramebuffer, uint frameBufferSize)
		{
			return WVR_GetCameraFrameBuffer_Android(pFramebuffer, frameBufferSize);
		}

        [DllImportAttribute("camerautility", EntryPoint = "GetFrameBufferWithPoseState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetFrameBufferWithPoseState_Android(IntPtr pFramebuffer, uint frameBufferSize, WVR_PoseOriginModel origin, uint predictInMs, ref WVR_PoseState_t poseState);
        public override bool GetFrameBufferWithPoseState(IntPtr pFramebuffer, uint frameBufferSize, WVR_PoseOriginModel origin, uint predictInMs, ref WVR_PoseState_t poseState)
		{
			return GetFrameBufferWithPoseState_Android(pFramebuffer, frameBufferSize, origin, predictInMs, ref poseState);
		}

        [DllImportAttribute("camerautility", EntryPoint = "ReleaseAll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ReleaseCameraTexture_Android();
        public override void ReleaseCameraTexture()
		{
			ReleaseCameraTexture_Android();
		}

		[DllImportAttribute("camerautility", EntryPoint = "DrawTextureWithBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool DrawTextureWithBuffer_Android(uint textureId, WVR_CameraImageFormat imgFormat, IntPtr frameBuffer, uint size, uint width, uint height, bool enableCropping, bool clearClampRegion);
		public override bool DrawTextureWithBuffer(IntPtr textureId, WVR_CameraImageFormat imgFormat, IntPtr frameBuffer, uint size, uint width, uint height, bool enableCropping, bool clearClampRegion)
		{
			return DrawTextureWithBuffer_Android((uint)textureId, imgFormat, frameBuffer, size, width, height,enableCropping, clearClampRegion);
		}

		// wvr_device.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsDeviceSuspend", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsDeviceSuspend_Android(WVR_DeviceType type);
		public override bool IsDeviceSuspend(WVR_DeviceType type)
		{
			return WVR_IsDeviceSuspend_Android(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ConvertMatrixQuaternion", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_ConvertMatrixQuaternion_Android(ref WVR_Matrix4f_t mat, ref WVR_Quatf_t quat, bool m2q);
		public override void ConvertMatrixQuaternion(ref WVR_Matrix4f_t mat, ref WVR_Quatf_t quat, bool m2q)
		{
			WVR_ConvertMatrixQuaternion_Android(ref mat, ref quat, m2q);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetDegreeOfFreedom", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_NumDoF WVR_GetDegreeOfFreedom_Android(WVR_DeviceType type);
		public override WVR_NumDoF GetDegreeOfFreedom(WVR_DeviceType type)
		{
			return WVR_GetDegreeOfFreedom_Android(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetParameters", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetParameters_Android(WVR_DeviceType type, IntPtr pchValue);
		public override void SetParameters(WVR_DeviceType type, IntPtr pchValue)
		{
			WVR_SetParameters_Android(type, pchValue);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetParameters", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint WVR_GetParameters_Android(WVR_DeviceType type, IntPtr pchValue, IntPtr retValue, uint unBufferSize);
		public override uint GetParameters(WVR_DeviceType type, IntPtr pchValue, IntPtr retValue, uint unBufferSize)
		{
			return WVR_GetParameters_Android(type, pchValue, retValue, unBufferSize);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetDefaultControllerRole", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_DeviceType WVR_GetDefaultControllerRole_Android();
		public override WVR_DeviceType GetDefaultControllerRole()
		{
			return WVR_GetDefaultControllerRole_Android();
		}

		/*[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetInteractionMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetInteractionMode_Android(WVR_InteractionMode mode);
		public override bool SetInteractionMode(WVR_InteractionMode mode)
		{
			return WVR_SetInteractionMode_Android(mode);
		}*/

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetInteractionMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_InteractionMode WVR_GetInteractionMode_Android();
		public override WVR_InteractionMode GetInteractionMode()
		{
			return WVR_GetInteractionMode_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetGazeTriggerType", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetGazeTriggerType_Android(WVR_GazeTriggerType type);
		public override bool SetGazeTriggerType(WVR_GazeTriggerType type)
		{
			return WVR_SetGazeTriggerType_Android(type);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetGazeTriggerType", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_GazeTriggerType WVR_GetGazeTriggerType_Android();
		public override WVR_GazeTriggerType GetGazeTriggerType()
		{
			return WVR_GetGazeTriggerType_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetDeviceErrorState", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetDeviceErrorState_Android(WVR_DeviceType dev_type, WVR_DeviceErrorStatus error_type);
		public override bool GetDeviceErrorState(WVR_DeviceType dev_type, WVR_DeviceErrorStatus error_type)
		{
			return WVR_GetDeviceErrorState_Android(dev_type, error_type);
		}

		// TODO
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetRenderTargetSize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetRenderTargetSize_Android(ref uint width, ref uint height);
		public override void GetRenderTargetSize(ref uint width, ref uint height)
		{
			WVR_GetRenderTargetSize_Android(ref width, ref height);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetProjection", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Matrix4f_t WVR_GetProjection_Android(WVR_Eye eye, float near, float far);
		public override WVR_Matrix4f_t GetProjection(WVR_Eye eye, float near, float far)
		{
			return WVR_GetProjection_Android(eye, near, far);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetClippingPlaneBoundary", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_GetClippingPlaneBoundary_Android(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom);
		public override void GetClippingPlaneBoundary(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom)
		{
			WVR_GetClippingPlaneBoundary_Android(eye, ref left, ref right, ref top, ref bottom);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetOverfillRatio", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetOverfillRatio_Android(float ratioX, float ratioY);
		public override void SetOverfillRatio(float ratioX, float ratioY)
		{
			WVR_SetOverfillRatio_Android(ratioX, ratioY);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTransformFromEyeToHead", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Matrix4f_t WVR_GetTransformFromEyeToHead_Android(WVR_Eye eye, WVR_NumDoF dof);
		public override WVR_Matrix4f_t GetTransformFromEyeToHead(WVR_Eye eye, WVR_NumDoF dof)
		{
			return WVR_GetTransformFromEyeToHead_Android(eye, dof);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SubmitCompositionLayers", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_SubmitError WVR_SubmitCompositionLayers_Android([In, Out] WVR_LayerSetParams_t[] param);
		public override WVR_SubmitError SubmitCompositionLayers([In, Out] WVR_LayerSetParams_t[] param)
		{
			return WVR_SubmitCompositionLayers_Android(param);
		}

        [DllImportAttribute("wvr_api", EntryPoint = "WVR_GetMaxCompositionLayerCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint WVR_GetMaxCompositionLayerCount_Android();
        public override uint GetMaxCompositionLayerCount()
        {
            return WVR_GetMaxCompositionLayerCount_Android();
        }

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_CreateAndroidSurface", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WVR_CreateAndroidSurface_Android(int width, int height, bool isProtected, [In, Out] WVR_TextureParams_t[] param);
		public override IntPtr CreateAndroidSurface(int width, int height, bool isProtected, [In, Out] WVR_TextureParams_t[] param)
		{
			return WVR_CreateAndroidSurface_Android(width, height, isProtected, param);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_DeleteAndroidSurface", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_DeleteAndroidSurface_Android();
		public override void DeleteAndroidSurface()
		{
			WVR_DeleteAndroidSurface_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SubmitFrame", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_SubmitError WVR_SubmitFrame_Android(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod);
		public override WVR_SubmitError SubmitFrame(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod)
		{
			return WVR_SubmitFrame_Android(eye, param, pose, extendMethod);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_PreRenderEye", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_PreRenderEye_Android(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_RenderFoveationParams[] foveationParams);
		public override void PreRenderEye(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			WVR_PreRenderEye_Android(eye, param, foveationParams);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RequestScreenshot", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_RequestScreenshot_Android(uint width, uint height, WVR_ScreenshotMode mode, IntPtr filename);
		public override bool RequestScreenshot(uint width, uint height, WVR_ScreenshotMode mode, IntPtr filename)
		{
			return WVR_RequestScreenshot_Android(width, height, mode, filename);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RenderMask", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_RenderMask_Android(WVR_Eye eye);
		public override void RenderMask(WVR_Eye eye)
		{
			WVR_RenderMask_Android(eye);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetRenderProps", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_GetRenderProps_Android(ref WVR_RenderProps_t props);
		public override bool GetRenderProps(ref WVR_RenderProps_t props)
		{
			return WVR_GetRenderProps_Android(ref props);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ObtainTextureQueue", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr WVR_ObtainTextureQueue_Android(WVR_TextureTarget target, WVR_TextureFormat format, WVR_TextureType type, uint width, uint height, int level);
		public override IntPtr ObtainTextureQueue(WVR_TextureTarget target, WVR_TextureFormat format, WVR_TextureType type, uint width, uint height, int level)
		{
			return WVR_ObtainTextureQueue_Android(target, format, type, width, height, level);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTextureQueueLength", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint WVR_GetTextureQueueLength_Android(IntPtr handle);
		public override uint GetTextureQueueLength(IntPtr handle)
		{
			return WVR_GetTextureQueueLength_Android(handle);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetTexture", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_TextureParams_t WVR_GetTexture_Android(IntPtr handle, int index);
		public override WVR_TextureParams_t GetTexture(IntPtr handle, int index)
		{
			return WVR_GetTexture_Android(handle, index);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetAvailableTextureIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern int WVR_GetAvailableTextureIndex_Android(IntPtr handle);
		public override int GetAvailableTextureIndex(IntPtr handle)
		{
			return WVR_GetAvailableTextureIndex_Android(handle);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ReleaseTextureQueue", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_ReleaseTextureQueue_Android(IntPtr handle);
		public override void ReleaseTextureQueue(IntPtr handle)
		{
			WVR_ReleaseTextureQueue_Android(handle);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsRenderFoveationSupport", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsRenderFoveationSupport_Android();
		public override bool IsRenderFoveationSupport()
		{
			return WVR_IsRenderFoveationSupport_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RenderFoveation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_RenderFoveation_Android(bool enable);
		public override void RenderFoveation(bool enable)
		{
			WVR_RenderFoveation_Android(enable);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_RenderFoveationMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_RenderFoveationMode_Android(WVR_FoveationMode mode);
		public override WVR_Result RenderFoveationMode(WVR_FoveationMode mode)
		{
			return WVR_RenderFoveationMode_Android(mode);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetFoveationConfig", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetFoveationConfig_Android(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams);
		public override WVR_Result SetFoveationConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			return WVR_SetFoveationConfig_Android(eye, foveationParams);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetFoveationDefaultConfig", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetFoveationDefaultConfig_Android(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams);
		public override WVR_Result GetFoveationDefaultConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			return WVR_GetFoveationDefaultConfig_Android(eye, foveationParams);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsRenderFoveationEnabled", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsRenderFoveationEnabled_Android();
		public override bool IsRenderFoveationEnabled()
		{
			return WVR_IsRenderFoveationEnabled_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsRenderFoveationDefaultOn", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsRenderFoveationDefaultOn_Android();
		public override bool IsRenderFoveationDefaultOn()
		{
			return WVR_IsRenderFoveationDefaultOn_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetPosePredictEnabled", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetPosePredictEnabled_Android(WVR_DeviceType type, bool enabled_position_predict, bool enable_rotation_predict);
		public override void SetPosePredictEnabled(WVR_DeviceType type, bool enabled_position_predict, bool enable_rotation_predict)
		{
			WVR_SetPosePredictEnabled_Android(type, enabled_position_predict, enable_rotation_predict);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ShowPassthroughOverlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_ShowPassthroughOverlay_Android(bool show, bool delaySubmit, bool showIndicator);
		public override bool ShowPassthroughOverlay(bool show, bool delaySubmit = false, bool showIndicator = false)
		{
			return WVR_ShowPassthroughOverlay_Android(show, delaySubmit, showIndicator);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetPassthroughOverlayAlpha", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetPassthroughOverlayAlpha_Android(float alpha);
		public override WVR_Result SetPassthroughOverlayAlpha(float alpha)
		{
			return WVR_SetPassthroughOverlayAlpha_Android(alpha);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ShowPassthroughUnderlay", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ShowPassthroughUnderlay_Android(bool show);
		public override WVR_Result ShowPassthroughUnderlay(bool show)
		{
			return WVR_ShowPassthroughUnderlay_Android(show);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_EnableAutoPassthrough", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_EnableAutoPassthrough_Android(bool enable);
		public override void EnableAutoPassthrough(bool enable)
		{
			WVR_EnableAutoPassthrough_Android(enable);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_IsPassthroughOverlayVisible", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_IsPassthroughOverlayVisible_Android();
		public override bool IsPassthroughOverlayVisible()
		{
			return WVR_IsPassthroughOverlayVisible_Android();
		}

		// wvr_compatibility.h
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetChecker", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetChecker_Android(bool enable);
		public override bool SetChecker(bool enable)
		{
			return WVR_SetChecker_Android(enable);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetProjectedPassthroughPose", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetProjectedPassthroughPose_Android(ref WVR_Pose_t pose);
		public override WVR_Result SetProjectedPassthroughPose(ref WVR_Pose_t pose)
		{
			return WVR_SetProjectedPassthroughPose_Android(ref pose);
		}
		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetProjectedPassthroughMesh", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetProjectedPassthroughMesh_Android(float[] vertexBuffer, uint vertextCount, uint[] indices, uint indexCount);
		public override WVR_Result SetProjectedPassthroughMesh(float[] vertexBuffer, uint vertextCount, uint[] indices, uint indexCount)
		{
			return WVR_SetProjectedPassthroughMesh_Android(vertexBuffer, vertextCount, indices, indexCount);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetProjectedPassthroughAlpha", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetProjectedPassthroughAlpha_Android(float alpha);
		public override WVR_Result SetProjectedPassthroughAlpha(float alpha)
		{
			return WVR_SetProjectedPassthroughAlpha_Android(alpha);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ShowProjectedPassthrough", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_ShowProjectedPassthrough_Android(bool show);
		public override WVR_Result ShowProjectedPassthrough(bool show)
		{
			return WVR_ShowProjectedPassthrough_Android(show);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetAMCMode", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetAMCMode_Android(WVR_AMCMode mode);
		public override WVR_Result SetAMCMode(WVR_AMCMode mode)
		{
			return WVR_SetAMCMode_Android(mode);
		}

        [DllImportAttribute("wvr_api", EntryPoint = "WVR_GetAMCMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern WVR_AMCMode WVR_GetAMCMode_Android();
        public override WVR_AMCMode GetAMCMode()
        {
            return WVR_GetAMCMode_Android();
        }

        [DllImportAttribute("wvr_api", EntryPoint = "WVR_GetAMCState", CallingConvention = CallingConvention.Cdecl)]
        public static extern WVR_AMCState WVR_GetAMCState_Android();
        public override WVR_AMCState GetAMCState()
        {
            return WVR_GetAMCState_Android();
        }

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetFrameSharpnessEnhancementLevel", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetFrameSharpnessEnhancementLevel_Android(float level);
		public override WVR_Result SetFrameSharpnessEnhancementLevel(float level)
		{
			return WVR_SetFrameSharpnessEnhancementLevel_Android(level);
		}

		#region Internal
		public override string DeployRenderModelAssets(int deviceIndex, string renderModelName)
		{
			const string VRACTIVITY_CLASSNAME = "com.htc.vr.unity.WVRUnityVRActivity";
			const string FILEUTILS_CLASSNAME = "com.htc.vr.unity.FileUtils";

			AndroidJavaClass ajc = new AndroidJavaClass(VRACTIVITY_CLASSNAME);

			if (ajc == null || deviceIndex == -1)
			{
				//PrintWarningLog("AndroidJavaClass vractivity is null, deviceIndex" + deviceIndex);
				return "";
			}
			else
			{
				AndroidJavaObject activity = ajc.CallStatic<AndroidJavaObject>("getInstance");
				if (activity != null)
				{
					AndroidJavaObject afd = activity.Call<AndroidJavaObject>("getControllerModelFileDescriptor", deviceIndex);
					if (afd != null)
					{
						AndroidJavaObject fileUtisObject = new AndroidJavaObject(FILEUTILS_CLASSNAME, activity, afd);

						if (fileUtisObject != null)
						{
							string retUnzip = fileUtisObject.Call<string>("deployRenderModelAssets", renderModelName);

							if (retUnzip == "")
							{
								//PrintWarningLog("doUnZIPAndDeploy failed");
							}
							else
							{
								//PrintInfoLog("doUnZIPAndDeploy success");
								ajc = null;
								return retUnzip;
							}
						}
						else
						{
							//PrintWarningLog("fileUtisObject is null");
						}
					}
					else
					{
						//PrintWarningLog("get fd failed");
					}
				}
				else
				{
					//PrintWarningLog("getInstance failed");
				}
			}
			ajc = null;
			return "";
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_SetFocusedController", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_SetFocusedController_Android(WVR_DeviceType focusController);
		public override void SetFocusedController(WVR_DeviceType focusController)
		{
			WVR_SetFocusedController_Android(focusController);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetFocusedController", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_DeviceType WVR_GetFocusedController_Android();
		public override WVR_DeviceType GetFocusedController()
		{
			return WVR_GetFocusedController_Android();
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetResolutionMaxScale", CallingConvention = CallingConvention.Cdecl)]
		public static extern float WVR_GetResolutionMaxScale_Android();
		public override float GetResolutionMaxScale()
		{
			return WVR_GetResolutionMaxScale_Android();
		}

		private const string PERMISSION_MANAGER_CLASSNAME = "com.htc.vr.permission.client.PermissionManager";
		private static WVR_RequestCompleteCallback mCallback = null;
		private static WVR_RequestUsbCompleteCallback mUsbCallback = null;
		private AndroidJavaObject permissionsManager = null;

		private AndroidJavaObject javaArrayFromCS(string[] values)
		{
			AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
			AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Length);
			for (int i = 0; i < values.Length; ++i)
			{
				arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
			}

			return arrayObject;
		}

		public override bool IsPermissionInitialed()
		{
			bool ret = false;
			if (permissionsManager == null)
			{
				AndroidJavaClass ajc = new AndroidJavaClass(PERMISSION_MANAGER_CLASSNAME);

				if (ajc != null)
				{
					permissionsManager = ajc.CallStatic<AndroidJavaObject>("getInstance");
				}
			}

			if (permissionsManager != null)
			{
				ret = permissionsManager.Call<bool>("isInitialized");
			}

			return ret;
		}

		public override bool ShowDialogOnScene()
		{
			if (!IsPermissionInitialed())
			{
				return false;
			}

			return permissionsManager.Call<bool>("showDialogOnVRScene");
		}

		public override bool IsPermissionGranted(string permission)
		{
			if (!IsPermissionInitialed())
			{
				return false;
			}

			return permissionsManager.Call<bool>("isPermissionGranted", permission);
		}

		public override bool ShouldGrantPermission(string permission)
		{
			if (!IsPermissionInitialed())
			{
				return false;
			}

			return permissionsManager.Call<bool>("shouldGrantPermission", permission);
		}

		public override void RequestPermissions(string[] permissions, WVR_RequestCompleteCallback cb)
		{
			//Log.d(LOG_TAG, "requestPermission");

			if (!IsPermissionInitialed())
			{
				//Log.e(LOG_TAG, "requestPermissions failed because permissionsManager doesn't initialize");
				return;
			}

			mCallback = cb;
			if (!permissionsManager.Call<bool>("isShow2D"))
			{
				permissionsManager.Call("requestPermissions", javaArrayFromCS(permissions), new RequestCompleteHandler());
			}
			else
			{
				using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						jo.Call("setRequestPermissionCallback", new RequestCompleteHandler());
					}
				}
				permissionsManager.Call("requestPermissions", javaArrayFromCS(permissions), new RequestCompleteHandler());
			}
		}

		public override void RequestUsbPermission(WVR_RequestUsbCompleteCallback cb)
		{

			if (!IsPermissionInitialed())
			{
				return;
			}

			mUsbCallback = cb;
			permissionsManager.Call("requestUsbPermission", new RequestUsbCompleteHandler());
		}



		class RequestCompleteHandler : AndroidJavaProxy
		{
			internal RequestCompleteHandler() : base(new AndroidJavaClass("com.htc.vr.permission.client.PermissionCallback"))
			{
			}

			public void onRequestCompletedwithObject(AndroidJavaObject resultObject)
			{
				//Log.i(LOG_TAG, "unity callback with result object");
				if (mCallback == null)
				{
					// Log.w(LOG_TAG, "unity callback but user callback is null ");
				}

				List<WVR_RequestResult> permissionResults = new List<WVR_RequestResult>();
				bool[] resultArray = null;
				AndroidJavaObject boolbuffer = resultObject.Get<AndroidJavaObject>("result");
				if ((boolbuffer) != null && (boolbuffer.GetRawObject() != IntPtr.Zero))
				{
					try
					{
#if UNITY_2018
						resultArray = AndroidJNI.FromBooleanArray(boolbuffer.GetRawObject());
#else
						resultArray = AndroidJNIHelper.ConvertFromJNIArray<bool[]>(boolbuffer.GetRawObject());
#endif
						//  Log.i(LOG_TAG, "ConvertFromJNIArray to bool array : " + resultArray.Length);
					}
					catch (Exception)
					{
						//  Log.e(LOG_TAG, "ConvertFromJNIArray failed: " + e.ToString());
					}
				}

				string[] permissionArray = null;

				AndroidJavaObject stringbuffer = resultObject.Get<AndroidJavaObject>("requestPermissions");
				if ((stringbuffer) != null && (stringbuffer.GetRawObject() != IntPtr.Zero))
				{
					permissionArray = AndroidJNIHelper.ConvertFromJNIArray<string[]>(stringbuffer.GetRawObject());
					// Log.i(LOG_TAG, "ConvertFromJNIArray to string array : " + permissionArray.Length);
				}

				if (permissionArray != null && resultArray != null)
				{
					for (int i = 0; i < permissionArray.Length; i++)
					{
						WVR_RequestResult rr;
						rr.mPermission = permissionArray[i];
						rr.mGranted = resultArray[i];
						permissionResults.Add(rr);
					}
				}

				mCallback(permissionResults);
			}
		}

		class RequestUsbCompleteHandler : AndroidJavaProxy
		{
			internal RequestUsbCompleteHandler() : base(new AndroidJavaClass("com.htc.vr.permission.client.UsbPermissionCallback"))
			{
			}
			public void onRequestCompletedwithObject(AndroidJavaObject resultObject)
			{
				//Log.i(LOG_TAG, "unity callback with result object");
				if (mUsbCallback == null)
				{
					// Log.w(LOG_TAG, "unity callback but user callback is null ");
				}
				bool resut = resultObject.Get<bool>("result");
				mUsbCallback(resut);
			}
		}

		private const string RESOURCE_WRAPPER_CLASSNAME = "com.htc.vr.unity.ResourceWrapper";
		private AndroidJavaObject ResourceWrapper = null;

		private bool initializeResourceObject()
		{
			if (ResourceWrapper == null)
			{
				AndroidJavaClass ajc = new AndroidJavaClass(RESOURCE_WRAPPER_CLASSNAME);

				if (ajc != null)
				{
					// Get the PermissionManager object
					ResourceWrapper = ajc.CallStatic<AndroidJavaObject>("getInstance");
				}
			}
			return (ResourceWrapper == null) ? false : true;
		}

		public override string GetStringBySystemLanguage(string stringName)
		{
			string retString = "";
			//Log.d(LOG_TAG, "getString, string " + stringName);
			if (initializeResourceObject())
			{
				retString = ResourceWrapper.Call<string>("getStringByName", stringName);
			}
			//Log.d(LOG_TAG, "getString, return string " + retString);
			return retString;
		}

		public override string GetStringByLanguage(string stringName, string lang, string country)
		{
			string retString = "";
			//Log.d(LOG_TAG, "getPreferredStringByName, string = " + stringName + ", lang = " + lang + ", country = " + country);
			if (initializeResourceObject())
			{
				retString = ResourceWrapper.Call<string>("getPreferredStringByName", stringName, lang, country);
			}
			//Log.d(LOG_TAG, "getPreferredStringByName, return string " + retString);
			return retString;
		}

		public override string GetSystemLanguage()
		{
			string retString = "";
			if (initializeResourceObject())
			{
				retString = ResourceWrapper.Call<string>("getSystemLanguage");
			}
			//Log.d(LOG_TAG, "getSystemLanguage, return string " + retString);
			return retString;
		}

		public override string GetSystemCountry()
		{
			string retString = "";
			if (initializeResourceObject())
			{
				retString = ResourceWrapper.Call<string>("getSystemCountry");
			}
			//Log.d(LOG_TAG, "getSystemCountry, return string " + retString);
			return retString;
		}

		private const string OEM_CONFIG_CLASSNAME = "com.htc.vr.unity.WVRUnityVRActivity";
		private static WVR_OnOEMConfigChanged OEMChangedCallback = null;
		private static AndroidJavaObject mOEMConfig = null;

		public static OEMConfigCallback mOEMCallback = new OEMConfigCallback();

		public class OEMConfigCallback : AndroidJavaProxy
		{
			internal OEMConfigCallback() : base(new AndroidJavaClass("com.htc.vr.unity.WVRUnityVRActivity$OEMConfigCallback"))
			{
			}

			public void onConfigChanged()
			{
				if (OEMChangedCallback != null)
				{
					OEMChangedCallback();
				}
			}
		}

		private static void initAJC()
		{
			if (mOEMConfig == null)
			{
				AndroidJavaClass ajc = new AndroidJavaClass(OEM_CONFIG_CLASSNAME);

				if (ajc == null)
				{
					// Log.e(LOG_TAG, "AndroidJavaClass is null");
					return;
				}
				// Get the OEMConfig object
				mOEMConfig = ajc.CallStatic<AndroidJavaObject>("getInstance");

				mOEMConfig.Call("setOEMChangedCB", mOEMCallback);
			}
		}

		public override string GetOEMConfigByKey(string key)
		{
			string getString = "";
			initAJC();

			if (mOEMConfig != null)
			{
				getString = mOEMConfig.Call<string>("getJsonRawData", key);
#if false
				const int charPerLine = 200;
				int len = (getString.Length / charPerLine);

				Log.d(LOG_TAG, "len = " + len + ", length of string = " + getString.Length);
				Log.d(LOG_TAG, key + ": raw data = ");

				for (int i = 0; i < len; i++)
				{
					string substr = getString.Substring(i * charPerLine, charPerLine);
					Log.d(LOG_TAG, substr);
				}

				int remainLen = getString.Length - (len * charPerLine);
				string remainstr = getString.Substring(len * charPerLine, remainLen);
				Log.d(LOG_TAG, remainstr);
#endif
			}

			getString.Trim(' ');
			if (getString.Length == 0 || getString[0] != '{' || getString[getString.Length - 1] != '}')
				return "";
			return getString;
		}

		public override void SetOEMConfigChangedCallback(WVR_OnOEMConfigChanged cb)
		{
			OEMChangedCallback = cb;
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetCurrentControllerModel", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetCurrentControllerModel_Android(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone);
		public override WVR_Result GetCurrentControllerModel(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone)
		{
			return WVR_GetCurrentControllerModel_Android(type, ref ctrlerModel, isOneBone);
		}

	    [DllImportAttribute("wvr_api", EntryPoint = "WVR_ReleaseControllerModel", CallingConvention = CallingConvention.Cdecl)]
	    public static extern void WVR_ReleaseControllerModel_Android(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */);
	    public override void ReleaseControllerModel(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */)
	    {
		    WVR_ReleaseControllerModel_Android(ref ctrlerModel /* WVR_CtrlerModel* */);
	    }

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetCtrlerModelAnimNodeData", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetCtrlerModelAnimNodeData_Android(WVR_DeviceType type, ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */);
		public override WVR_Result GetCtrlerModelAnimNodeData(WVR_DeviceType type, ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
		{
			return WVR_GetCtrlerModelAnimNodeData_Android(type, ref ctrlModelAnimData);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ReleaseCtrlerModelAnimNodeData", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_ReleaseCtrlerModelAnimNodeData_Android(ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */);
		public override void ReleaseCtrlerModelAnimNodeData(ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
		{
			WVR_ReleaseCtrlerModelAnimNodeData_Android(ref ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_GetCurrentNaturalHandModel", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetCurrentNaturalHandModel_Android(ref IntPtr handModel /* WVR_HandRenderModel* */);
		public override WVR_Result GetCurrentNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
		{
			return WVR_GetCurrentNaturalHandModel_Android(ref handModel);
		}

		[DllImportAttribute("wvr_api", EntryPoint = "WVR_ReleaseNatureHandModel", CallingConvention = CallingConvention.Cdecl)]
		public static extern void WVR_ReleaseNaturalHandModel_Android(ref IntPtr handModel /* WVR_HandRenderModel* */);
		public override void ReleaseNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
		{
			WVR_ReleaseNaturalHandModel_Android(ref handModel);
		}
		#endregion

		[DllImport("wvr_api", EntryPoint = "WVR_GetAvailableFrameRates", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetAvailableFrameRates_Android(uint frameRateCapacityInput, ref uint frameRateCountOutput, [In, Out] uint[] frameRates);
		public override WVR_Result WVR_GetAvailableFrameRates(out uint[] frameRates)
		{
			frameRates = null;
			uint frameRateCountOutput = 0;
			WVR_Result result = WVR_GetAvailableFrameRates_Android(0, ref frameRateCountOutput, null);
			if (result != WVR_Result.WVR_Success)
				return result;

			if (frameRateCountOutput == 0)
			{
				frameRates = null;
				return result;
			}

			frameRates = new uint[frameRateCountOutput];
			result = WVR_GetAvailableFrameRates_Android(frameRateCountOutput, ref frameRateCountOutput, frameRates);
			return result;
		}

		[DllImport("wvr_api", EntryPoint = "WVR_GetFrameRate", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_GetFrameRate_Android(ref uint frameRate);
		public override WVR_Result WVR_GetFrameRate(ref uint frameRate)
		{
			return WVR_GetFrameRate_Android(ref frameRate);
		}

		[DllImport("wvr_api", EntryPoint = "WVR_SetFrameRate", CallingConvention = CallingConvention.Cdecl)]
		public static extern WVR_Result WVR_SetFrameRate_Android(uint frameRate);
		public override WVR_Result WVR_SetFrameRate(uint frameRate)
		{
			return WVR_SetFrameRate_Android(frameRate);
		}

		[DllImport("wvr_api", EntryPoint = "WVR_SetPassthroughImageQuality", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetPassthroughImageQuality_Android(WVR_PassthroughImageQuality quality);
		public override bool WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality quality)
		{
			return WVR_SetPassthroughImageQuality_Android(quality);
		}

		[DllImport("wvr_api", EntryPoint = "WVR_SetPassthroughImageFocus", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WVR_SetPassthroughImageFocus_Android(WVR_PassthroughImageFocus focus);
		public override bool WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus focus)
		{
			return WVR_SetPassthroughImageFocus_Android(focus);
		}

        [DllImport("wvr_api", EntryPoint = "WVR_SetPassthroughImageRate", CallingConvention = CallingConvention.Cdecl)]
        public static extern WVR_Result WVR_SetPassthroughImageRate_Android(WVR_PassthroughImageRate rate);

        public override WVR_Result SetPassthroughImageRate(WVR_PassthroughImageRate rate)
        {
            return WVR_SetPassthroughImageRate_Android(rate);
        }


        [DllImport("wvr_api", EntryPoint = "WVR_EnableHandleDisplayChanged", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WVR_EnableHandleDisplayChanged_Android(bool enable);
        public override void EnableHandleDisplayChanged(bool enable)
        {
            WVR_EnableHandleDisplayChanged_Android(enable);
        }
    }
}
