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
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Settings;

namespace Wave.OpenXR
{
	public static class InputDeviceTracker
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceTracker ";
		private static StringBuilder m_sb = null;
		private static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg)
		{
			msg.Insert(0, LOG_TAG);
			UnityEngine.Debug.Log(msg);
		}

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

				sb.Clear(); sb.Append("ActivateTracker() ").Append((settings.EnableTracker ? "Activate." : "Deactivate.")).Append(" from ").Append(caller);
				DEBUG(sb);
				SettingsHelper.SetBool(WaveXRSettings.EnableTrackerText, settings.EnableTracker);
			}
		}
		#endregion

		#region Wave XR Constants
		const string kTracker0Name = "Wave Tracker0";
		const string kTracker1Name = "Wave Tracker1";
		const string kTracker2Name = "Wave Tracker2";
		const string kTracker3Name = "Wave Tracker3";
		const string kTracker4Name = "Wave Tracker4";
		const string kTracker5Name = "Wave Tracker5";
		const string kTracker6Name = "Wave Tracker6";
		const string kTracker7Name = "Wave Tracker7";
		const string kTracker8Name = "Wave Tracker8";
		const string kTracker9Name = "Wave Tracker9";
		const string kTracker10Name = "Wave Tracker10";
		const string kTracker11Name = "Wave Tracker11";
		const string kTracker12Name = "Wave Tracker12";
		const string kTracker13Name = "Wave Tracker13";
		const string kTracker14Name = "Wave Tracker14";
		const string kTracker15Name = "Wave Tracker15";
		const string kWristTrackerName = "Vive_Wrist_Tracker";
		const string kInsideOutTrackerName = "Vive_Self_Tracker";
		const string kInsideOutTrackerIMName = "Vive_Self_Tracker_IM";

		const string kTracker0SN = "HTC-211012-Tracker0";
		const string kTracker1SN = "HTC-211012-Tracker1";
		const string kTracker2SN = "HTC-211012-Tracker2";
		const string kTracker3SN = "HTC-211012-Tracker3";
		const string kTracker4SN = "HTC-220801-Tracker4";
		const string kTracker5SN = "HTC-220801-Tracker5";
		const string kTracker6SN = "HTC-220801-Tracker6";
		const string kTracker7SN = "HTC-220801-Tracker7";
		const string kTracker8SN = "HTC-220801-Tracker8";
		const string kTracker9SN = "HTC-230503-Tracker9";
		const string kTracker10SN = "HTC-230503-Tracker10";
		const string kTracker11SN = "HTC-230503-Tracker11";
		const string kTracker12SN = "HTC-230503-Tracker12";
		const string kTracker13SN = "HTC-230503-Tracker13";
		const string kTracker14SN = "HTC-230503-Tracker14";
		const string kTracker15SN = "HTC-230503-Tracker15";

		const string kTracker0Role = "TrackerRole0";
		const string kTracker1Role = "TrackerRole1";
		const string kTracker2Role = "TrackerRole2";
		const string kTracker3Role = "TrackerRole3";
		const string kTracker4Role = "TrackerRole4";
		const string kTracker5Role = "TrackerRole5";
		const string kTracker6Role = "TrackerRole6";
		const string kTracker7Role = "TrackerRole7";
		const string kTracker8Role = "TrackerRole8";
		const string kTracker9Role = "TrackerRole9";
		const string kTracker10Role = "TrackerRole10";
		const string kTracker11Role = "TrackerRole11";
		const string kTracker12Role = "TrackerRole12";
		const string kTracker13Role = "TrackerRole13";
		const string kTracker14Role = "TrackerRole14";
		const string kTracker15Role = "TrackerRole15";

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
			Tracker9 = 9,
			Tracker10 = 10,
			Tracker11 = 11,
			Tracker12 = 12,
			Tracker13 = 13,
			Tracker14 = 14,
			Tracker15 = 15,
		}
		internal readonly static TrackerId[] s_TrackerIds = new TrackerId[]
		{
			TrackerId.Tracker0,
			TrackerId.Tracker1,
			TrackerId.Tracker2,
			TrackerId.Tracker3,
			TrackerId.Tracker4,
			TrackerId.Tracker5,
			TrackerId.Tracker6,
			TrackerId.Tracker7,
			TrackerId.Tracker8,
			TrackerId.Tracker9,
			TrackerId.Tracker10,
			TrackerId.Tracker11,
			TrackerId.Tracker12,
			TrackerId.Tracker13,
			TrackerId.Tracker14,
			TrackerId.Tracker15,
		};

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
			if (trackerId == TrackerId.Tracker9) { return kTracker9Name; }
			if (trackerId == TrackerId.Tracker10) { return kTracker10Name; }
			if (trackerId == TrackerId.Tracker11) { return kTracker11Name; }
			if (trackerId == TrackerId.Tracker12) { return kTracker12Name; }
			if (trackerId == TrackerId.Tracker13) { return kTracker13Name; }
			if (trackerId == TrackerId.Tracker14) { return kTracker14Name; }
			if (trackerId == TrackerId.Tracker15) { return kTracker15Name; }

			return "";
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
			if (trackerId == TrackerId.Tracker9) { return kTracker9SN; }
			if (trackerId == TrackerId.Tracker10) { return kTracker10SN; }
			if (trackerId == TrackerId.Tracker11) { return kTracker11SN; }
			if (trackerId == TrackerId.Tracker12) { return kTracker12SN; }
			if (trackerId == TrackerId.Tracker13) { return kTracker13SN; }
			if (trackerId == TrackerId.Tracker14) { return kTracker14SN; }
			if (trackerId == TrackerId.Tracker15) { return kTracker15SN; }
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
			if (trackerId == TrackerId.Tracker9) { return kTracker9Role; }
			if (trackerId == TrackerId.Tracker10) { return kTracker10Role; }
			if (trackerId == TrackerId.Tracker11) { return kTracker11Role; }
			if (trackerId == TrackerId.Tracker12) { return kTracker12Role; }
			if (trackerId == TrackerId.Tracker13) { return kTracker13Role; }
			if (trackerId == TrackerId.Tracker14) { return kTracker14Role; }
			if (trackerId == TrackerId.Tracker15) { return kTracker15Role; }
			return kTracker0Role;
		}

		public enum TrackerRole : UInt32
		{
			Undefined   = 0,
			Standalone  = 1,
			Pair1_Right = 2,
			Pair1_Left  = 3,

			Shoulder_Right = 32,
			Upper_Arm_Right = 33,
			Elbow_Right = 34,
			Forearm_Right   = 35,
			Wrist_Right     = 36,
			Hand_Right = 37,
			Thigh_Right     = 38,
			Knee_Right = 39,
			Calf_Right      = 40,
			Ankle_Right     = 41,
			Foot_Right = 42,

			Shoulder_Left = 47,
			Upper_Arm_Left  = 48,
			Elbow_Left = 49,
			Forearm_Left    = 50,
			Wrist_Left      = 51,
			Hand_Left = 52,
			Thigh_Left      = 53,
			Knee_Left = 54,
			Calf_Left       = 55,
			Ankle_Left      = 56,
			Foot_Left = 57,

			Chest = 62,
			Waist = 63,

			Camera = 71,
			Keyboard = 72,
		}
		internal static TrackerRole[] s_TrackerRoles = new TrackerRole[]
		{
			TrackerRole.Undefined,	 // 0
			TrackerRole.Standalone,	 // 1
			TrackerRole.Pair1_Right, // 2
			TrackerRole.Pair1_Left,	 // 3

			TrackerRole.Shoulder_Right, //32
			TrackerRole.Upper_Arm_Right, // 33
			TrackerRole.Elbow_Right, // 34
			TrackerRole.Forearm_Right,   // 35
			TrackerRole.Wrist_Right,     // 36
			TrackerRole.Hand_Right, // 37
			TrackerRole.Thigh_Right,     // 38
			TrackerRole.Knee_Right, // 39
			TrackerRole.Calf_Right,      // 40
			TrackerRole.Ankle_Right,     // 41
			TrackerRole.Foot_Right, // 42

			TrackerRole.Shoulder_Left, // 47
			TrackerRole.Upper_Arm_Left,  // 48
			TrackerRole.Elbow_Left, // 49
			TrackerRole.Forearm_Left,    // 50
			TrackerRole.Wrist_Left,      // 51
			TrackerRole.Hand_Left, // 52
			TrackerRole.Thigh_Left,      // 53
			TrackerRole.Knee_Left, // 54
			TrackerRole.Calf_Left,       // 55
			TrackerRole.Ankle_Left,      // 56
			TrackerRole.Foot_Left, // 57

			TrackerRole.Chest, // 62
			TrackerRole.Waist, // 63

			TrackerRole.Camera, // 71
			TrackerRole.Keyboard, // 72
		};

		public static bool IsTrackerDevice(InputDevice input, TrackerId trackerId)
		{
			if (input.serialNumber.Equals(trackerId.SerialNumber()))
				return true;

			return false;
		}

		internal static List<InputDevice> s_InputDevices = new List<InputDevice>();
		internal static int inputDeviceFrame = -1;
		private static void UpdateInputDevices()
		{
			if (inputDeviceFrame != Time.frameCount)
			{
				inputDeviceFrame = Time.frameCount;
				InputDevices.GetDevices(s_InputDevices);
			}
		}

		private static readonly InputDevice kInputDeviceDefault;
		private static bool GetTrackerDevice(TrackerId trackerId, out InputDevice device)
		{
			UpdateInputDevices();
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (IsTrackerDevice(s_InputDevices[i], trackerId))
				{
					device = s_InputDevices[i];
					return true;
				}
			}

			device = kInputDeviceDefault;
			return false;
		}

		public static bool IsAvailable()
		{
			UpdateInputDevices();
			for (int i = 0; s_InputDevices != null && i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				for (int id = 0; id < s_TrackerIds.Length; id++)
				{
					if (IsTrackerDevice(s_InputDevices[i], s_TrackerIds[id]))
						return true;
				}
			}
			return false;
		}
		public static bool IsAvailable(TrackerId trackerId)
		{
			return GetTrackerDevice(trackerId, out InputDevice device);
		}

		public static bool IsTracked(TrackerId trackerId)
		{
			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
					return isTracked;
			}
			return false;
		}
		public static bool GetTrackingState(TrackerId trackerId, out InputTrackingState state)
		{
			state = InputTrackingState.None;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState value))
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

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 value))
				{
					position = value;
					return true;
				}
			}

			return false;
		}
		public static bool GetRotation(TrackerId trackerId, out Quaternion rotation)
		{
			rotation = Quaternion.identity;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion value))
				{
					rotation = value;
					return true;
				}
			}

			return false;
		}
		public static bool GetVelocity(TrackerId trackerId, out Vector3 velocity)
		{
			velocity = Vector3.zero;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 value))
				{
					velocity = value;
					return true;
				}
			}

			return false;
		}
		public static bool GetAngularVelocity(TrackerId trackerId, out Vector3 angularVelocity)
		{
			angularVelocity = Vector3.zero;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 value))
				{
					angularVelocity = value;
					return true;
				}
			}

			return false;
		}
		public static bool GetAcceleration(TrackerId trackerId, out Vector3 acceleration)
		{
			acceleration = Vector3.zero;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.deviceAcceleration, out Vector3 value))
				{
					acceleration = value;
					return true;
				}
			}

			return false;
		}
		public static bool GetAngularAcceleration(TrackerId trackerId, out Vector3 angularAcceleration)
		{
			angularAcceleration = Vector3.zero;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out Vector3 value))
				{
					angularAcceleration = value;
					return true;
				}
			}

			return false;
		}
		public static bool BatteryLevel(TrackerId trackerId, out float level)
		{
			level = 0;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(CommonUsages.batteryLevel, out float value))
				{
					level = value;
					return true;
				}
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

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(button, out bool value))
				{
					down = value;
					return true;
				}
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<float> button, out float axis)
		{
			axis = 0;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(button, out float value))
				{
					axis = value;
					return true;
				}
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<Vector2> button, out Vector2 axis)
		{
			axis = Vector2.zero;

			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				if (device.TryGetFeatureValue(button, out Vector2 value))
				{
					axis = value;
					return true;
				}
			}

			return false;
		}

		public static bool HapticPulse(TrackerId trackerId, UInt32 durationMicroSec = 500000, UInt32 frequency = 0, float amplitude = 0.5f)
		{
			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
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

				sb.Clear().Append("HapticPulse() ").Append(trackerId.Name())
					.Append("[").Append(device.name).Append("]")
					.Append("[").Append(device.serialNumber).Append("]")
					.Append(" durationSec: ").Append(durationSec).Append(", amplitude: ").Append(amplitude)
					.Append(" from ").Append(caller);
				DEBUG(sb);
				return device.SendHapticImpulse(0, amplitude, durationSec);
			}

			return false;
		}

		public static bool GetTrackerDeviceName(TrackerId trackerId, out string trackerName)
		{
			if (GetTrackerDevice(trackerId, out InputDevice device))
			{
				trackerName = device.name;
				return true;
			}

			trackerName = "";
			return false;
		}
	}
}