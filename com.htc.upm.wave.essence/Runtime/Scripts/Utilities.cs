// "Wave SDK
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR;
using Wave.Native;
using Wave.OpenXR;
using Wave.XR;
using Wave.XR.Function;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.Diagnostics;

namespace Wave.Essence
{
	public static class ClientInterface
	{
		const string LOG_TAG = "Wave.Essence.ClientInterface";
		static List<UnityEngine.XR.InputDevice> m_Devices = new List<UnityEngine.XR.InputDevice>();
		public static void DeviceInfo()
		{
			InputDevices.GetDevices(m_Devices);
			foreach (var dev in m_Devices)
			{
				bool connected = false;
				if (dev.TryGetFeatureValue(XR_Feature.ValidPose, out bool value))
					connected = value;
				Log.d(LOG_TAG, "DeviceInfo() dev: " + dev.name + ", valid? " + dev.isValid + ", connected? " + connected, true);
			}
		}

		public static bool SetOrigin(WVR_PoseOriginModel wvrOrigin)
		{
			TrackingOriginModeFlags originMode = TrackingOriginModeFlags.Unknown;
			if (wvrOrigin == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround)
				originMode = TrackingOriginModeFlags.Floor;
			if (wvrOrigin == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead)
				originMode = TrackingOriginModeFlags.Device;
			if (wvrOrigin == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnTrackingObserver)
				originMode = TrackingOriginModeFlags.TrackingReference;

			if (originMode == TrackingOriginModeFlags.Unknown)
				return false;

			return Utils.InputSubsystem.TrySetTrackingOriginMode(originMode);
		}
		public static bool GetOrigin(ref WVR_PoseOriginModel wvrOrigin)
		{
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				wvrOrigin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
				return true;
			}
			else
#endif
			{
				return GetOrigin(Utils.InputSubsystem.GetTrackingOriginMode(), ref wvrOrigin);
			}
		}
		public static bool GetOrigin(in TrackingOriginModeFlags xrOrigin, ref WVR_PoseOriginModel wvrOrigin)
		{
			bool result = true;
			switch (xrOrigin)
			{
				case TrackingOriginModeFlags.Device:
					wvrOrigin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
					break;
				case TrackingOriginModeFlags.Floor:
					wvrOrigin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround;
					break;
				case TrackingOriginModeFlags.TrackingReference:
					wvrOrigin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnTrackingObserver;
					break;
				default:
					result = false;
					break;
			}

			return result;
		}

		public static string GetCurrentRenderModelName(WVR_DeviceType type)
		{
			Profiler.BeginSample("GetCurrentRenderModelName");
			string parameterName = "GetRenderModelName";
			IntPtr ptrParameterName = Marshal.StringToHGlobalAnsi(parameterName);
			IntPtr ptrResult = Marshal.AllocHGlobal(64);
			uint resultVertLength = 64;
			string tmprenderModelName = "";
			uint retOfRenderModel = Interop.WVR_GetParameters(type, ptrParameterName, ptrResult, resultVertLength);

			if (retOfRenderModel > 0)
				tmprenderModelName = Marshal.PtrToStringAnsi(ptrResult);

			Log.d(LOG_TAG, "Type: " + type + ", current render model name: " + tmprenderModelName);

			Marshal.FreeHGlobal(ptrParameterName);
			Marshal.FreeHGlobal(ptrResult);
			Profiler.EndSample();
			return tmprenderModelName;
		}

		static int m_ForegroundFrameCount = 0;
		private static bool m_IsFocused = false;
		/// <summary> Means the system focus is captured by current scene or not. </summary>
		public static bool IsFocused
		{
			get
			{
				if (m_ForegroundFrameCount != Time.frameCount)
				{
					m_ForegroundFrameCount = Time.frameCount;
					bool focused = (Interop.WVR_IsInputFocusCapturedBySystem() ? false : true);
					if (m_IsFocused != focused)
					{
						m_IsFocused = focused;
						Log.d(LOG_TAG, "IsFocused() " + m_IsFocused, true);
					}
				}
				return m_IsFocused;
			}
		}

		public static XR_InteractionMode InteractionMode
		{
			get { return WaveEssence.Instance.GetInteractionMode(); }
			set { }
		}
	} // class ClientInterface

	public static class Numeric
	{
		public static bool IsBoolean(string value)
		{
			try
			{
				bool i = Convert.ToBoolean(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsFloat(string value)
		{
			try
			{
				float i = Convert.ToSingle(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsNumeric(string value)
		{
			try
			{
				int i = Convert.ToInt32(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsValid(this Quaternion quat)
		{
			if (quat.x > 1 || quat.x < -1 || float.IsNaN(quat.x)) { return false; }
			if (quat.y > 1 || quat.y < -1 || float.IsNaN(quat.y)) { return false; }
			if (quat.z > 1 || quat.z < -1 || float.IsNaN(quat.z)) { return false; }
			if (quat.w > 1 || quat.w < -1 || float.IsNaN(quat.w)) { return false; }
			return true;
		}
	} // class Numeric

	public class RenderFunctions
	{
		public delegate void SetPoseUsedOnSubmitDelegate([In, Out] WVR_PoseState_t[] poseState);
		private static SetPoseUsedOnSubmitDelegate setPoseUsedOnSubmit = null;
		public static SetPoseUsedOnSubmitDelegate SetPoseUsedOnSubmit
		{
			get
			{
				if (setPoseUsedOnSubmit == null)
					setPoseUsedOnSubmit = FunctionsHelper.GetFuncPtr<SetPoseUsedOnSubmitDelegate>("SetPoseUsedOnSubmit");
				return setPoseUsedOnSubmit;
			}
		}

		public delegate void NotifyQualityLevelChangeDelegate();
		private static NotifyQualityLevelChangeDelegate notifyQualityLevelChange = null;
		public static NotifyQualityLevelChangeDelegate NotifyQualityLevelChange
		{
			get
			{
				if (notifyQualityLevelChange == null)
					notifyQualityLevelChange = FunctionsHelper.GetFuncPtr<NotifyQualityLevelChangeDelegate>("NotifyQualityLevelChange");
				return notifyQualityLevelChange;
			}
		}
	} // class RenderFunctions

	public static class WXRDevice
	{
		public static bool IsLeftHanded {
			get {
				return WaveEssence.Instance.IsLeftHanded;
			}
		}

		public static bool IsConnected(WVR_DeviceType deviceType)
		{
			return WaveEssence.Instance.IsConnected(deviceType);
		}
		private static XR_Device GetAdaptiveDevice(XR_Device device, bool adaptiveHanded = false)
		{
			if (adaptiveHanded)
			{
				if (device == XR_Device.Left)
				{
					return (IsLeftHanded ? XR_Device.Right : XR_Device.Left);
				}
				if (device == XR_Device.Right)
				{
					return (IsLeftHanded ? XR_Device.Left : XR_Device.Right);
				}
			}

			return device;
		}
		public static bool IsConnected(XR_Device device, bool adaptiveHanded = false)
		{
#if UNITY_EDITOR
			if (Application.isEditor)
				return true;
#endif
			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.IsConnected(InputDeviceControl.kHMDCharacteristics); }
			if (device == XR_Device.Left) { return InputDeviceControl.IsConnected(InputDeviceControl.kControllerLeftCharacteristics); }
			if (device == XR_Device.Right) { return InputDeviceControl.IsConnected(InputDeviceControl.kControllerRightCharacteristics); }

			return false;
		}
		public static bool IsTracked(XR_Device device, bool adaptiveHanded = false)
		{
#if UNITY_EDITOR
			if (Application.isEditor)
				return true;
#endif
			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.IsTracked(InputDeviceControl.kHMDCharacteristics); }
			if (device == XR_Device.Left) { return InputDeviceControl.IsTracked(InputDeviceControl.kControllerLeftCharacteristics); }
			if (device == XR_Device.Right) { return InputDeviceControl.IsTracked(InputDeviceControl.kControllerRightCharacteristics); }

			return false;
		}

		public static WVR_DeviceType GetRoleType(this XR_Device device, bool adaptiveHanded = false)
		{
			switch (device)
			{
				case XR_Device.Head:
					return WVR_DeviceType.WVR_DeviceType_HMD;
				case XR_Device.Dominant:
					if (adaptiveHanded)
						return IsLeftHanded ? WVR_DeviceType.WVR_DeviceType_Controller_Left : WVR_DeviceType.WVR_DeviceType_Controller_Right;
					else
						return WVR_DeviceType.WVR_DeviceType_Controller_Right;
				case XR_Device.NonDominant:
					if (adaptiveHanded)
						return IsLeftHanded ? WVR_DeviceType.WVR_DeviceType_Controller_Right : WVR_DeviceType.WVR_DeviceType_Controller_Left;
					else
						return WVR_DeviceType.WVR_DeviceType_Controller_Left;
				default:
					break;
			}

			return WVR_DeviceType.WVR_DeviceType_Invalid;
		}

		public static InputDevice GetRoleDevice(XR_Device device, bool adaptiveHanded = false)
		{
			if (device == XR_Device.Dominant)
			{
				if (adaptiveHanded)
					return IsLeftHanded ? InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) : InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
				else
					return InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
			}
			if (device == XR_Device.NonDominant)
			{
				if (adaptiveHanded)
					return IsLeftHanded ? InputDevices.GetDeviceAtXRNode(XRNode.RightHand) : InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
				else
					return InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			}

			return InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		static WVR_DeviceType GetRoleDevice(WVR_DeviceType device, bool adaptiveHanded = false)
		{
			if (adaptiveHanded && IsLeftHanded)
			{
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
					return WVR_DeviceType.WVR_DeviceType_Controller_Left;
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
					return WVR_DeviceType.WVR_DeviceType_Controller_Right;

			}
			return device;
		}

		#region Unity XR Buttons
		public static bool KeyDown(XR_Device device, InputFeatureUsage<bool> button, bool adaptiveHanded = false)
		{
			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.KeyDown(InputDeviceControl.kHMDCharacteristics, button); }
			if (device == XR_Device.Left) { return InputDeviceControl.KeyDown(InputDeviceControl.kControllerLeftCharacteristics, button); }
			if (device == XR_Device.Right) { return InputDeviceControl.KeyDown(InputDeviceControl.kControllerRightCharacteristics, button); }

			return false;
		}
		public static float KeyAxis1D(XR_Device device, InputFeatureUsage<float> button, bool adaptiveHanded = false)
		{
			float axis = 0;

			if (KeyAxis1D(device, button, out float value)) { axis = value; }

			return axis;
		}
		public static bool KeyAxis1D(XR_Device device, InputFeatureUsage<float> button, out float axis1d, bool adaptiveHanded = false)
		{
			axis1d = 0;

			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.KeyAxis1D(InputDeviceControl.kHMDCharacteristics, button, out axis1d); }
			if (device == XR_Device.Left) { return InputDeviceControl.KeyAxis1D(InputDeviceControl.kControllerLeftCharacteristics, button, out axis1d); }
			if (device == XR_Device.Right) { return InputDeviceControl.KeyAxis1D(InputDeviceControl.kControllerRightCharacteristics, button, out axis1d); }

			return false;
		}
		public static Vector2 KeyAxis2D(XR_Device device, InputFeatureUsage<Vector2> button, bool adaptiveHanded = false)
		{
			Vector2 axis = Vector2.zero;

			if (KeyAxis2D(device, button, out Vector2 value)) { axis = value; }

			return axis;
		}
		public static bool KeyAxis2D(XR_Device device, InputFeatureUsage<Vector2> button, out Vector2 axis2d, bool adaptiveHanded = false)
		{
			axis2d = Vector2.zero;

			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.KeyAxis2D(InputDeviceControl.kHMDCharacteristics, button, out axis2d); }
			if (device == XR_Device.Left) { return InputDeviceControl.KeyAxis2D(InputDeviceControl.kControllerLeftCharacteristics, button, out axis2d); }
			if (device == XR_Device.Right) { return InputDeviceControl.KeyAxis2D(InputDeviceControl.kControllerRightCharacteristics, button, out axis2d); }

			return false;
		}
		#endregion
		#region Wave Buttons
		public static bool ButtonPress(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonPress(adaptive_device, id);
		}
		public static bool ButtonHold(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonHold(adaptive_device, id);
		}
		public static bool ButtonRelease(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonRelease(adaptive_device, id);
		}
		public static bool ButtonTouch(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonTouch(adaptive_device, id);
		}
		public static bool ButtonTouching(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonTouching(adaptive_device, id);
		}
		public static bool ButtonUntouch(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonUntouch(adaptive_device, id);
		}
		public static Vector2 ButtonAxis(WVR_DeviceType device, WVR_InputId id, bool adaptiveHanded = false)
		{
			WVR_DeviceType adaptive_device = GetRoleDevice(device);
			return WaveEssence.Instance.ButtonAxis(adaptive_device, id);
		}
		#endregion

		#region XR Device Vibration
		static readonly HapticCapabilities emptyHapticCapabilities = new HapticCapabilities();
		public static bool TryGetHapticCapabilities(XR_Device device, out HapticCapabilities hapticCaps, bool adaptiveHanded = false)
		{
			hapticCaps = emptyHapticCapabilities;

			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.TryGetHapticCapabilities(InputDeviceControl.kHMDCharacteristics, out hapticCaps); }
			if (device == XR_Device.Left) { return InputDeviceControl.TryGetHapticCapabilities(InputDeviceControl.kControllerLeftCharacteristics, out hapticCaps); }
			if (device == XR_Device.Right) { return InputDeviceControl.TryGetHapticCapabilities(InputDeviceControl.kControllerRightCharacteristics, out hapticCaps); }

			return false;
		}

#pragma warning disable
		static HapticCapabilities m_HapticCaps = new HapticCapabilities();
#pragma warning enable
		public static bool SendHapticImpulse(XR_Device device, float amplitude, float duration, bool adaptiveHanded = false)
		{
			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.SendHapticImpulse(InputDeviceControl.kHMDCharacteristics, amplitude, duration); }
			if (device == XR_Device.Left) { return InputDeviceControl.SendHapticImpulse(InputDeviceControl.kControllerLeftCharacteristics, amplitude, duration); }
			if (device == XR_Device.Right) { return InputDeviceControl.SendHapticImpulse(InputDeviceControl.kControllerRightCharacteristics, amplitude, duration); }

			return false;
		}
		#endregion

		public static bool GetPosition(XR_Device device, ref Vector3 position, bool adaptiveHanded = false)
		{
			if (device == XR_Device.Head)
			{
				if (!IsTracked(device, adaptiveHanded))
					return false;

#if UNITY_EDITOR
				if (Application.isEditor)
				{
					position = WaveEssence.Instance.GetPosition(WVR_DeviceType.WVR_DeviceType_HMD);
					return true;
				}
				else
#endif
				{
					return InputDeviceControl.GetPosition(InputDeviceControl.kHMDCharacteristics, out position);
				}
			}
			if (device == XR_Device.Right)
				return GetControllerPosition(XR_Hand.Right, ref position, adaptiveHanded);
			if (device == XR_Device.Left)
				return GetControllerPosition(XR_Hand.Left, ref position, adaptiveHanded);

			return false;
		}
		public static bool GetRotation(XR_Device device, ref Quaternion rotation, bool adaptiveHanded = false)
		{
			if (device == XR_Device.Head)
			{
				if (!IsTracked(device, adaptiveHanded))
					return false;

#if UNITY_EDITOR
				if (Application.isEditor)
				{
					rotation = WaveEssence.Instance.GetRotation(WVR_DeviceType.WVR_DeviceType_HMD);
					return true;
				}
				else
#endif
				{
					return InputDeviceControl.GetRotation(InputDeviceControl.kHMDCharacteristics, out rotation);
				}
			}
			if (device == XR_Device.Right)
				return GetControllerRotation(XR_Hand.Right, ref rotation, adaptiveHanded);
			if (device == XR_Device.Left)
				return GetControllerRotation(XR_Hand.Left, ref rotation, adaptiveHanded);

			return false;
		}
		public static bool GetVelocity(XR_Device device, ref Vector3 velocity, bool adaptiveHanded = false)
		{
			device = GetAdaptiveDevice(device, adaptiveHanded);
			return InputDeviceControl.GetVelocity(device.InputDevice(), out velocity);
		}
		public static bool GetAngularVelocity(XR_Device device, ref Vector3 angularVelocity, bool adaptiveHanded = false)
		{
			device = GetAdaptiveDevice(device, adaptiveHanded);
			return InputDeviceControl.GetAngularVelocity(device.InputDevice(), out angularVelocity);
		}

		#region Controller Pose Mode
		public static bool GetControllerPosition(XR_Hand hand, ref Vector3 position, bool adaptiveHanded = false)
		{
			return GetControllerPosition(hand, XR_ControllerPoseMode.Raw, ref position, adaptiveHanded);
		}
		public static bool GetControllerPosition(XR_Hand hand, XR_ControllerPoseMode mode, ref Vector3 position, bool adaptiveHanded = false)
		{
			if (!IsTracked((XR_Device)hand, adaptiveHanded))
				return false;

			Vector3 pos = Vector3.zero;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				pos = WaveEssence.Instance.GetPosition(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded));
			} else
#endif
			{
				var device = GetAdaptiveDevice((XR_Device)hand, adaptiveHanded);
				if (device == XR_Device.Left)
				{
					if (InputDeviceControl.GetPosition(InputDeviceControl.kControllerLeftCharacteristics, out Vector3 value))
						pos = value;
				}
				if (device == XR_Device.Right)
				{
					if (InputDeviceControl.GetPosition(InputDeviceControl.kControllerRightCharacteristics, out Vector3 value))
						pos = value;
				}
			}

			Vector3 offset = GetControllerPositionOffset(hand, mode, adaptiveHanded);
			position = pos + offset;

			return true;
		}
		public static bool GetControllerRotation(XR_Hand hand, ref Quaternion rotation, bool adaptiveHanded = false)
		{
			return GetControllerRotation(hand, XR_ControllerPoseMode.Raw, ref rotation, adaptiveHanded);
		}
		public static bool GetControllerRotation(XR_Hand hand, XR_ControllerPoseMode mode, ref Quaternion rotation, bool adaptiveHanded = false)
		{
			if (!IsTracked((XR_Device)hand, adaptiveHanded))
				return false;

			Quaternion rot = Quaternion.identity;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				rot = WaveEssence.Instance.GetRotation(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded));
			}
			else
#endif
			{
				var device = GetAdaptiveDevice((XR_Device)hand, adaptiveHanded);
				if (device == XR_Device.Left)
				{
					if (InputDeviceControl.GetRotation(InputDeviceControl.kControllerLeftCharacteristics, out Quaternion value))
						rot = value;
				}
				if (device == XR_Device.Right)
				{
					if (InputDeviceControl.GetRotation(InputDeviceControl.kControllerRightCharacteristics, out Quaternion value))
						rot = value;
				}
			}

			Quaternion offset = GetControllerRotationOffset(hand, mode, adaptiveHanded);
			rotation = rot * offset;

			return true;
		}

		public static bool SetControllerPoseMode(XR_Hand hand, XR_ControllerPoseMode mode, bool adaptiveHanded = false)
		{
			return WaveEssence.Instance.SetControllerPoseMode(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded), (WVR_ControllerPoseMode)mode);
		}
		public static bool GetControllerPoseMode(XR_Hand hand, out XR_ControllerPoseMode mode, bool adaptiveHanded = false)
		{
			mode = XR_ControllerPoseMode.Raw;
			if (WaveEssence.Instance.GetControllerPoseMode(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded), out WVR_ControllerPoseMode wvrMode))
			{
				mode = wvrMode.XrMode();
				return true;
			}
			return false;
		}
		public static Vector3 GetCurrentControllerPositionOffset(XR_Hand hand, bool adaptiveHanded = false)
		{
			return WaveEssence.Instance.GetCurrentControllerPositionOffset(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded));
		}
		public static Vector3 GetControllerPositionOffset(XR_Hand hand, XR_ControllerPoseMode mode, bool adaptiveHanded = false)
		{
			return WaveEssence.Instance.GetControllerPositionOffset(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded), (WVR_ControllerPoseMode)mode);
		}
		public static Quaternion GetCurrentControllerRotationOffset(XR_Hand hand, bool adaptiveHanded = false)
		{
			return WaveEssence.Instance.GetCurrentControllerRotationOffset(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded));
		}
		public static Quaternion GetControllerRotationOffset(XR_Hand hand, XR_ControllerPoseMode mode, bool adaptiveHanded = false)
		{
			return WaveEssence.Instance.GetControllerRotationOffset(GetRoleDevice((WVR_DeviceType)hand, adaptiveHanded), (WVR_ControllerPoseMode)mode);
		}
		public static XR_ControllerPoseMode XrMode(this WVR_ControllerPoseMode mode)
		{
			if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger) { return XR_ControllerPoseMode.Trigger; }
			if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel) { return XR_ControllerPoseMode.Panel; }
			if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle) { return XR_ControllerPoseMode.Handle; }
			return XR_ControllerPoseMode.Raw;
		}
		#endregion

		/// <summary>
		/// Retrieves a device's battery life with a valid value between 0~1 where 1 means full capacity or an invalid value of -1.
		/// </summary>
		public static float GetBatteryLevel(XR_Device device, bool adaptiveHanded = false)
		{
			device = GetAdaptiveDevice(device, adaptiveHanded);
			if (device == XR_Device.Head) { return InputDeviceControl.GetBatteryLevel(InputDeviceControl.kHMDCharacteristics); }
			if (device == XR_Device.Left) { return InputDeviceControl.GetBatteryLevel(InputDeviceControl.kControllerLeftCharacteristics); }
			if (device == XR_Device.Right) { return InputDeviceControl.GetBatteryLevel(InputDeviceControl.kControllerRightCharacteristics); }

			return 0;
		}

		/// <summary>
		/// When user wears the head mounted device, CommonUsages.userPresence is true.
		/// </summary>
		public static bool IsUserPresence()
		{
			return InputDeviceControl.IsUserPresence();
		}

		#region Table Static
		public static bool IsTableStatic(XR_Hand hand)
		{
			return WaveEssence.Instance.IsTableStatic(hand);
		}
		#endregion
	} // class WXRDevice

	public static class NotifyDevice
	{
		const string LOG_TAG = "Wave.Essence.NotifyDevice";
		static void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		static void ERROR(string msg) { Log.e(LOG_TAG, msg, true); }

		const UInt32 MAX_DATA_LENGTH = 64;
		static void AssertLength(ref UInt32 length)
		{
			length = length > MAX_DATA_LENGTH ? MAX_DATA_LENGTH : length;
		}

		static Dictionary<WVR_DeviceType, UInt32> s_Results = new Dictionary<WVR_DeviceType, UInt32>()
		{
			{ WVR_DeviceType.WVR_DeviceType_HMD, 0 },
			{ WVR_DeviceType.WVR_DeviceType_Controller_Right, 0 },
			{ WVR_DeviceType.WVR_DeviceType_Controller_Left, 0 },
			{ WVR_DeviceType.WVR_DeviceType_Camera, 0 },
			{ WVR_DeviceType.WVR_DeviceType_EyeTracking, 0 },
			{ WVR_DeviceType.WVR_DeviceType_HandGesture_Right, 0 },
			{ WVR_DeviceType.WVR_DeviceType_HandGesture_Left, 0 },
			{ WVR_DeviceType.WVR_DeviceType_NaturalHand_Right, 0 },
			{ WVR_DeviceType.WVR_DeviceType_NaturalHand_Left, 0 },
			{ WVR_DeviceType.WVR_DeviceType_ElectronicHand_Right, 0 },
			{ WVR_DeviceType.WVR_DeviceType_ElectronicHand_Left, 0 },
			{ WVR_DeviceType.WVR_DeviceType_Tracker, 0 },
		};

		public static WVR_Result Start(WVR_DeviceType type, UInt32 unBufferSize = MAX_DATA_LENGTH)
		{
			if (!s_Results.ContainsKey(type)) { return WVR_Result.WVR_Error_InvalidArgument; }
			if (unBufferSize <= 0) { return WVR_Result.WVR_Error_InvalidArgument; }
			if (!WXRDevice.IsConnected(type)) { return WVR_Result.WVR_Error_DeviceDisconnected; }
			if (s_Results[type] > 0) { return WVR_Result.WVR_Success; }

			AssertLength(ref unBufferSize);

			var result = Interop.WVR_StartNotifyDeviceInfo(type, unBufferSize);
			DEBUG("Start() " + type + ", " + unBufferSize + ", result: " + result);

			if (result == WVR_Result.WVR_Success) { s_Results[type] = unBufferSize; }
			return result;
		}
		public static void Stop(WVR_DeviceType type)
		{
			if (s_Results.ContainsKey(type) && s_Results[type] > 0)
			{
				Interop.WVR_StopNotifyDeviceInfo(type);
				s_Results[type] = 0;
			}
		}
		const string PREFIX = "OUTDATA-";
		public static void Send(WVR_DeviceType type, string dataValue)
		{
			if (!s_Results.ContainsKey(type))
			{
				ERROR("Send() " + type + " is invalid.");
				return;
			}
			if (s_Results[type] <= 0)
			{
				ERROR("Send() " + type + " is not started.");
				return;
			}
			if (dataValue == null || dataValue.Length <= 0)
			{
				ERROR("Send() invalid data.");
				return;
			}

			uint type_id = ((uint)type);
			string info = PREFIX + type_id + dataValue;

			if (info.Length > s_Results[type])
			{
				ERROR("Send() info " + info + " length " + info.Length + " > buffer size " + s_Results[type]);
				return;
			}

			DEBUG("Send() " + type + ", " + info);

			IntPtr ptrParameterName = Marshal.StringToHGlobalAnsi(info);
			Interop.WVR_UpdateNotifyDeviceInfo(type, ptrParameterName);
			Marshal.FreeHGlobal(ptrParameterName);
		}
	}

	public static class Misc
	{
		public static string GetCaller()
		{
			string caller = "none";

			var frame = new StackFrame(2, true);
			if (frame != null)
			{
				var method = frame.GetMethod();
				if (method != null)
					caller = method.Name;
				else
					caller = "No method.";
			}

			return caller;
		}
	}
}
