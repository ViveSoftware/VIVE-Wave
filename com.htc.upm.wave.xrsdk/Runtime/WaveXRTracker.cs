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
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Settings;

namespace Wave.OpenXR
{
	public static class InputDeviceTracker
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceTracker";
		static void DEBUG(string msg) { UnityEngine.Debug.Log(LOG_TAG + " " + msg); }

		#region Wave XR Interface
		public static void ActivateTracker(bool active)
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null && settings.EnableTracker != active)
			{
				settings.EnableTracker = active;
				string caller = "TBD";
				var frame = new StackFrame(1, true);
				if (frame != null)
				{
					var method = frame.GetMethod();
					if (method != null)
						caller = method.Name;
					else
						caller = "No method.";
				}
				DEBUG("ActivateTracker() " + (settings.EnableTracker ? "Activate." : "Deactivate.") + " from " + caller);
				SettingsHelper.SetBool(WaveXRSettings.EnableTrackerText, settings.EnableTracker);
			}
		}
		#endregion

		#region Unity XR Tracker definitions
		const string kTracker0Name = "Wave Tracker0";
		const string kTracker1Name = "Wave Tracker1";
		const string kTracker2Name = "Wave Tracker2";
		const string kTracker3Name = "Wave Tracker3";
		const string kTracker4Name = "Wave Tracker4";
		const string kTracker5Name = "Wave Tracker5";
		const string kTracker6Name = "Wave Tracker6";
		const string kTracker7Name = "Wave Tracker7";
		const string kTracker8Name = "Wave Tracker8";

		const string kTracker0SN = "HTC-211012-Tracker0";
		const string kTracker1SN = "HTC-211012-Tracker1";
		const string kTracker2SN = "HTC-211012-Tracker2";
		const string kTracker3SN = "HTC-211012-Tracker3";
		const string kTracker4SN = "HTC-220801-Tracker4";
		const string kTracker5SN = "HTC-220801-Tracker5";
		const string kTracker6SN = "HTC-220801-Tracker6";
		const string kTracker7SN = "HTC-220801-Tracker7";
		const string kTracker8SN = "HTC-220801-Tracker8";

		const string kTracker0Role = "TrackerRole0";
		const string kTracker1Role = "TrackerRole1";
		const string kTracker2Role = "TrackerRole2";
		const string kTracker3Role = "TrackerRole3";
		const string kTracker4Role = "TrackerRole4";
		const string kTracker5Role = "TrackerRole5";
		const string kTracker6Role = "TrackerRole6";
		const string kTracker7Role = "TrackerRole7";
		const string kTracker8Role = "TrackerRole8";

		/// <summary> Standalone Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kAloneTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice
		);
		/// <summary> Right Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kRightTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Right
		);
		/// <summary> Left Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kLeftTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Left
		);
		#endregion

		public enum TrackerId : UInt32
		{
			Tracker0 = 0,
			Tracker1 = 1,
			Tracker2 = 2,
			Tracker3 = 3,
			Tracker4 = 4,
			Tracker5 = 5,
			Tracker6 = 6,
			Tracker7 = 7,
			Tracker8 = 8,
		}

		public static string Name(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return kTracker0Name; }
			if (trackerId == TrackerId.Tracker1) { return kTracker1Name; }
			if (trackerId == TrackerId.Tracker2) { return kTracker2Name; }
			if (trackerId == TrackerId.Tracker3) { return kTracker3Name; }
			if (trackerId == TrackerId.Tracker4) { return kTracker4Name; }
			if (trackerId == TrackerId.Tracker5) { return kTracker5Name; }
			if (trackerId == TrackerId.Tracker6) { return kTracker6Name; }
			if (trackerId == TrackerId.Tracker7) { return kTracker7Name; }
			if (trackerId == TrackerId.Tracker8) { return kTracker8Name; }
			return kTracker0Name;
		}
		public static string SerialNumber(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return kTracker0SN; }
			if (trackerId == TrackerId.Tracker1) { return kTracker1SN; }
			if (trackerId == TrackerId.Tracker2) { return kTracker2SN; }
			if (trackerId == TrackerId.Tracker3) { return kTracker3SN; }
			if (trackerId == TrackerId.Tracker4) { return kTracker4SN; }
			if (trackerId == TrackerId.Tracker5) { return kTracker5SN; }
			if (trackerId == TrackerId.Tracker6) { return kTracker6SN; }
			if (trackerId == TrackerId.Tracker7) { return kTracker7SN; }
			if (trackerId == TrackerId.Tracker8) { return kTracker8SN; }
			return kTracker0SN;
		}
		public static string RoleKey(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return kTracker0Role; }
			if (trackerId == TrackerId.Tracker1) { return kTracker1Role; }
			if (trackerId == TrackerId.Tracker2) { return kTracker2Role; }
			if (trackerId == TrackerId.Tracker3) { return kTracker3Role; }
			if (trackerId == TrackerId.Tracker4) { return kTracker4Role; }
			if (trackerId == TrackerId.Tracker5) { return kTracker5Role; }
			if (trackerId == TrackerId.Tracker6) { return kTracker6Role; }
			if (trackerId == TrackerId.Tracker7) { return kTracker7Role; }
			if (trackerId == TrackerId.Tracker8) { return kTracker8Role; }
			return kTracker0Role;
		}

		public enum TrackerRole : UInt32
		{
			Undefined   = 0,
			Standalone  = 1,
			Pair1_Right = 2,
			Pair1_Left  = 3,

			Upper_Arm_Right = 32,
			Forearm_Right   = 33,
			Wrist_Right     = 34,
			Thigh_Right     = 35,
			Calf_Right      = 36,
			Ankle_Right     = 37,

			Upper_Arm_Left  = 47,
			Forearm_Left    = 48,
			Wrist_Left      = 49,
			Thigh_Left      = 50,
			Calf_Left       = 51,
			Ankle_Left      = 52,

			Chest = 62,
			Waist = 63
		}
		internal static TrackerRole[] s_TrackerRoles = new TrackerRole[]
		{
			TrackerRole.Undefined,	 // 0
			TrackerRole.Standalone,	 // 1
			TrackerRole.Pair1_Right, // 2
			TrackerRole.Pair1_Left,	 // 3

			TrackerRole.Upper_Arm_Right, // 32
			TrackerRole.Forearm_Right,   // 33
			TrackerRole.Wrist_Right,     // 34
			TrackerRole.Thigh_Right,     // 35
			TrackerRole.Calf_Right,      // 36
			TrackerRole.Ankle_Right,     // 37

			TrackerRole.Upper_Arm_Left,  // 47
			TrackerRole.Forearm_Left,    // 48
			TrackerRole.Wrist_Left,      // 49
			TrackerRole.Thigh_Left,      // 50
			TrackerRole.Calf_Left,       // 51
			TrackerRole.Ankle_Left,      // 52

			TrackerRole.Chest, // 62
			TrackerRole.Waist, // 63
		};

		public static bool IsTrackerDevice(InputDevice input, TrackerId trackerId)
		{
			if (input.name.Equals(trackerId.Name()) && input.serialNumber.Equals(trackerId.SerialNumber()))
				return true;

			return false;
		}

		internal static List<InputDevice> s_InputDevices = new List<InputDevice>();

		public static bool IsAvailable()
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker0) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker1) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker2) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker3) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker4) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker5) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker6) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker7) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker8)
					)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAvailable(TrackerId trackerId)
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (IsTrackerDevice(s_InputDevices[i], trackerId)) { return true; }
			}
			return false;
		}

		public static bool IsTracked(TrackerId trackerId)
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
				{
					return isTracked;
				}
			}
			return false;
		}

		public static bool GetTrackingState(TrackerId trackerId, out InputTrackingState state)
		{
			state = InputTrackingState.None;

			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState value))
				{
					state = value;
					return true;
				}
			}

			return false;
		}

		public static bool GetPosition(TrackerId trackerId, out Vector3 position)
		{
			position = Vector3.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.devicePosition, out position);
			}

			return false;
		}
		public static bool GetRotation(TrackerId trackerId, out Quaternion rotation)
		{
			rotation = Quaternion.identity;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			}

			return false;
		}
		public static bool GetVelocity(TrackerId trackerId, out Vector3 velocity)
		{
			velocity = Vector3.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity);
			}

			return false;
		}
		public static bool GetAngularVelocity(TrackerId trackerId, out Vector3 angularVelocity)
		{
			angularVelocity = Vector3.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out angularVelocity);
			}

			return false;
		}

		public static TrackerRole GetRole(TrackerId trackerId)
		{
			UInt32 roleId = 0;
			SettingsHelper.GetInt(trackerId.RoleKey(), ref roleId);
			for (int i = 0; i < s_TrackerRoles.Length; i++)
			{
				if (roleId == (UInt32)s_TrackerRoles[i])
					return s_TrackerRoles[i];
			}

			/*for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				if (s_InputDevices[i].characteristics.Equals(kLeftTrackerCharacteristics))
					return TrackerRole.Pair1_Left;

				if (s_InputDevices[i].characteristics.Equals(kRightTrackerCharacteristics))
					return TrackerRole.Pair1_Right;

				if (s_InputDevices[i].characteristics.Equals(kAloneTrackerCharacteristics))
					return TrackerRole.Standalone;
			}*/


			return TrackerRole.Undefined;
		}

		public static bool ButtonDown(TrackerId trackerId, InputFeatureUsage<bool> button, out bool down)
		{
			down = false;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out down);
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<float> button, out float axis)
		{
			axis = 0;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out axis);
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<Vector2> button, out Vector2 axis)
		{
			axis = Vector2.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out axis);
			}

			return false;
		}

		public static bool BatteryLevel(TrackerId trackerId, out float level)
		{
			level = 0;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.batteryLevel, out level);
			}

			return false;
		}

		public static bool HapticPulse(TrackerId trackerId, UInt32 durationMicroSec = 500000, UInt32 frequency = 0, float amplitude = 0.5f)
		{
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				float durationSec = durationMicroSec / 1000000;
				string caller = "TBD";
				var frame = new StackFrame(1, true);
				if (frame != null)
				{
					var method = frame.GetMethod();
					if (method != null)
						caller = method.Name;
					else
						caller = "No method.";
				}
				DEBUG("HapticPulse() " + trackerId
					+ "[" + trackerId.Name() + "]"
					+ "[" + trackerId.SerialNumber() + "]"
					+ ": " + durationSec.ToString() + ", " + amplitude
					+ " from " + caller);
				return s_InputDevices[i].SendHapticImpulse(0, amplitude, durationSec);
			}

			return false;
		}
	}
}