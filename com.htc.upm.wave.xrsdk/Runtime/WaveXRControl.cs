// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace Wave.OpenXR
{
	public static class InputDeviceControl
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceControl ";
		static StringBuilder m_sb = null;
		static StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg)
		{
			msg.Insert(0, LOG_TAG);
			Debug.Log(msg);
		}

		public enum ControlDevice
		{
			Head = 1,
			Right = 2,
			Left = 3,
		}

		/// <summary> Wave Head Mounted Device Characteristics </summary>
		public const InputDeviceCharacteristics kHMDCharacteristics = (
			InputDeviceCharacteristics.HeadMounted |
			InputDeviceCharacteristics.Camera |
			InputDeviceCharacteristics.TrackedDevice
		);
		public const uint kHMDCharacteristicsValue = (uint)kHMDCharacteristics;
		/// <summary> Wave Left Controller Characteristics </summary>
		public const InputDeviceCharacteristics kControllerLeftCharacteristics = (
			InputDeviceCharacteristics.Left |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.HeldInHand
		);
		public const uint kControllerLeftCharacteristicsValue = (uint)kControllerLeftCharacteristics;
		/// <summary> Wave Right Controller Characteristics </summary>
		public const InputDeviceCharacteristics kControllerRightCharacteristics = (
			InputDeviceCharacteristics.Right |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.HeldInHand
		);
		public const uint kControllerRightCharacteristicsValue = (uint)kControllerRightCharacteristics;
		private static uint GetIndex(uint value)
		{
			if (value == kHMDCharacteristicsValue) { return 1; }
			if (value == kControllerLeftCharacteristicsValue) { return 2; }
			if (value == kControllerRightCharacteristicsValue) { return 3; }
			return 0;
		}

		public static InputDeviceCharacteristics characteristic(this ControlDevice cd)
		{
			return (
				cd == ControlDevice.Head ? kHMDCharacteristics : (
					cd == ControlDevice.Right ? kControllerRightCharacteristics : kControllerLeftCharacteristics
				)
			);
		}

		internal static List<InputDevice> m_InputDevices = new List<InputDevice>();
		internal static int inputDeviceFrame = -1;
		private static void UpdateInputDevices()
		{
			if (inputDeviceFrame != Time.frameCount)
			{
				inputDeviceFrame = Time.frameCount;
				InputDevices.GetDevices(m_InputDevices);
			}
		}

		/// Tracking state
		private static bool[] s_IsConnected = new bool[4] { false, false, false, false };
		private static int[] isConnectedFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateConnectedDevice(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (isConnectedFrame[index] != Time.frameCount)
			{
				isConnectedFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool IsConnected(InputDeviceCharacteristics device)
		{
			if (!UpdateConnectedDevice(device, out uint index)) { return s_IsConnected[index]; }

			UpdateInputDevices();
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						s_IsConnected[index] = true;
						return s_IsConnected[index];
					}
				}
			}

			s_IsConnected[index] = false;
			return s_IsConnected[index];
		}
		public static bool IsConnected(ControlDevice device) { return IsConnected(device.characteristic()); }

		private static bool[] s_IsTracked = new bool[4] { false, false, false, false };
		private static int[] isTrackedFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateTrackedDevice(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (isTrackedFrame[index] != Time.frameCount)
			{
				isTrackedFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool IsTracked(InputDeviceCharacteristics device)
		{
			if (!UpdateTrackedDevice(device, out uint index)) { return s_IsTracked[index]; }
			if (!IsConnected(device)) { return false; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out s_IsTracked[index]))
							return s_IsTracked[index];
					}
				}
			}

			return false;
		}
		public static bool IsTracked(ControlDevice device) { return IsTracked(device.characteristic()); }

		/// Button
		public static bool KeyDown(InputDeviceCharacteristics device, InputFeatureUsage<bool> button)
		{
			if (!IsConnected(device)) { return false; }

			bool isDown = false;
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(button, out bool value))
							isDown = value;
					}
				}
			}

			return isDown;
		}
		public static bool KeyDown(ControlDevice device, InputFeatureUsage<bool> button) { return KeyDown(device.characteristic(), button); }
		public static bool KeyAxis1D(InputDeviceCharacteristics device, InputFeatureUsage<float> button, out float axis1d)
		{
			axis1d = 0;
			if (!IsConnected(device)) { return false; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(button, out float value))
						{
							axis1d = value;
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool KeyAxis1D(ControlDevice device, InputFeatureUsage<float> button, out float axis1d) { return KeyAxis1D(device.characteristic(), button, out axis1d); }
		public static bool KeyAxis2D(InputDeviceCharacteristics device, InputFeatureUsage<Vector2> button, out Vector2 axis2d)
		{
			axis2d = Vector2.zero;
			if (!IsConnected(device)) { return false; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(button, out Vector2 value))
						{
							axis2d = value;
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool KeyAxis2D(ControlDevice device, InputFeatureUsage<Vector2> button, out Vector2 axis2d) { return KeyAxis2D(device.characteristic(), button, out axis2d); }

		/// Haptic
		static readonly HapticCapabilities emptyHapticCapabilities = new HapticCapabilities();
		private static HapticCapabilities[] s_HapticCaps = new HapticCapabilities[4] { emptyHapticCapabilities, emptyHapticCapabilities, emptyHapticCapabilities, emptyHapticCapabilities };
		private static int[] hapticCapFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateHapticCapabilities(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (hapticCapFrame[index] != Time.frameCount)
			{
				hapticCapFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool TryGetHapticCapabilities(InputDeviceCharacteristics device, out HapticCapabilities hapticCaps)
		{
			hapticCaps = emptyHapticCapabilities;
			if (!IsConnected(device)) { return false; }
			if (!UpdateHapticCapabilities(device, out uint index)) { hapticCaps = s_HapticCaps[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetHapticCapabilities(out s_HapticCaps[index]))
						{
							hapticCaps = s_HapticCaps[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool TryGetHapticCapabilities(ControlDevice device, out HapticCapabilities hapticCaps) { return TryGetHapticCapabilities(device.characteristic(), out hapticCaps); }
		public static bool SendHapticImpulse(InputDeviceCharacteristics device, float amplitude, float duration)
		{
			if (!IsConnected(device)) { return false; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetHapticCapabilities(out HapticCapabilities value))
						{
							if (value.supportsImpulse)
							{
								amplitude = Mathf.Clamp(amplitude, 0, 1);
								return m_InputDevices[i].SendHapticImpulse(0, amplitude, duration);
							}
						}
					}
				}
			}

			return false;
		}
		public static bool SendHapticImpulse(ControlDevice device, float amplitude, float duration) { return SendHapticImpulse(device.characteristic(), amplitude, duration); }

		/// Pose
		private static Vector3[] s_Positions = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		private static int[] positionFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdatePosition(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (positionFrame[index] != Time.frameCount)
			{
				positionFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool GetPosition(InputDeviceCharacteristics device, out Vector3 position)
		{
			position = Vector3.zero;
			if (!IsTracked(device)) { return false; }
			if (!UpdatePosition(device, out uint index)) { position = s_Positions[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.devicePosition, out s_Positions[index]))
						{
							position = s_Positions[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool GetPosition(ControlDevice device, out Vector3 position) { return GetPosition(device.characteristic(), out position); }

		private static Quaternion[] s_Rotations = new Quaternion[4] { Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity };
		private static int[] rotationFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateRotation(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (rotationFrame[index] != Time.frameCount)
			{
				rotationFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool GetRotation(InputDeviceCharacteristics device, out Quaternion rotation)
		{
			rotation = Quaternion.identity;
			if (!IsTracked(device)) { return false; }
			if (!UpdateRotation(device, out uint index)) { rotation = s_Rotations[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceRotation, out s_Rotations[index]))
						{
							rotation = s_Rotations[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool GetRotation(ControlDevice device, out Quaternion rotation) { return GetRotation(device.characteristic(), out rotation); }

		private static Vector3[] s_Velocity = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		private static int[] velocityFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateVelocity(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (velocityFrame[index] != Time.frameCount)
			{
				velocityFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool GetVelocity(InputDeviceCharacteristics device, out Vector3 velocity)
		{
			velocity = Vector3.zero;
			if (!IsTracked(device)) { return false; }
			if (!UpdateVelocity(device, out uint index)) { velocity = s_Velocity[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceVelocity, out s_Velocity[index]))
						{
							velocity = s_Velocity[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool GetVelocity(ControlDevice device, out Vector3 velocity) { return GetVelocity(device.characteristic(), out velocity); }

		private static Vector3[] s_AngularVelocity = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		private static int[] angularVelocityFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateAngularVelocity(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (angularVelocityFrame[index] != Time.frameCount)
			{
				angularVelocityFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool GetAngularVelocity(InputDeviceCharacteristics device, out Vector3 angularVelocity)
		{
			angularVelocity = Vector3.zero;
			if (!IsTracked(device)) { return false; }
			if (!UpdateAngularVelocity(device, out uint index)) { angularVelocity = s_AngularVelocity[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out s_AngularVelocity[index]))
						{
							angularVelocity = s_AngularVelocity[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool GetAngularVelocity(ControlDevice device, out Vector3 angularVelocity) { return GetAngularVelocity(device.characteristic(), out angularVelocity); }

		private static Vector3[] s_Acceleration = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		private static int[] accelerationFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateAcceleration(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (accelerationFrame[index] != Time.frameCount)
			{
				accelerationFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool GetAcceleration(InputDeviceCharacteristics device, out Vector3 acceleration)
		{
			acceleration = Vector3.zero;
			if (!IsTracked(device)) { return false; }
			if (!UpdateAcceleration(device, out uint index)) { acceleration = s_Acceleration[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceAcceleration, out s_Acceleration[index]))
						{
							acceleration = s_Acceleration[index];
							return true;
						}
					}
				}
			}

			return false;
		}
		public static bool GetAcceleration(ControlDevice device, out Vector3 acceleration) { return GetAcceleration(device.characteristic(), out acceleration); }

		/// Battery
		private static float[] s_BatteryLevels = new float[4] { 0, 0, 0, 0 };
		private static int[] batteryFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateBatteryLevel(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (batteryFrame[index] != Time.frameCount)
			{
				batteryFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static float GetBatteryLevel(InputDeviceCharacteristics device)
		{
			if (!UpdateBatteryLevel(device, out uint index)) { return s_BatteryLevels[index]; }
			if (!IsConnected(device)) { return 0; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.batteryLevel, out s_BatteryLevels[index]))
							return s_BatteryLevels[index];
					}
				}
			}

			return 0;
		}
		public static float GetBatteryLevel(ControlDevice device) { return GetBatteryLevel(device.characteristic()); }

		private static bool m_UserPresence = false;
		private static int userPresenceFrame = -1;
		private static bool UpdateUserPresence()
		{
			if (userPresenceFrame != Time.frameCount)
			{
				userPresenceFrame = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool IsUserPresence()
		{
			if (!UpdateUserPresence()) { return m_UserPresence; }
			if (!IsConnected(kHMDCharacteristics)) { return false; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(kHMDCharacteristics))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.userPresence, out m_UserPresence))
							return m_UserPresence;
					}
				}
			}

			return false;
		}

		private static string[] s_Names = new string[4] { "", "", "", "" };
		private static int[] nameFrame = new int[4] { -1, -1, -1, -1 };
		private static bool UpdateName(InputDeviceCharacteristics device, out uint index)
		{
			index = GetIndex((uint)device);
			if (index == 0) { return false; }

			if (nameFrame[index] != Time.frameCount)
			{
				nameFrame[index] = Time.frameCount;
				return true;
			}
			return false;
		}
		public static bool Name(ControlDevice device, out string name)
		{
			name = "";
			if (!IsConnected(kHMDCharacteristics)) { return false; }
			if (!UpdateName(device.characteristic(), out uint index)) { name = s_Names[index]; return true; }

			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					if (m_InputDevices[i].characteristics.Equals(device.characteristic()))
					{
						s_Names[index] = m_InputDevices[i].name;
						name = s_Names[index];
						return true;
					}
				}
			}

			return false;
		}
	}
}