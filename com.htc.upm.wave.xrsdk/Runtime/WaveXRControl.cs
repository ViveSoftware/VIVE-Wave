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
using UnityEngine;
using UnityEngine.XR;

namespace Wave.OpenXR
{
	public static class InputDeviceControl
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceControl";
		static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

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
		/// <summary> Wave Left Controller Characteristics </summary>
		public const InputDeviceCharacteristics kControllerLeftCharacteristics = (
			InputDeviceCharacteristics.Left |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.HeldInHand
		);
		/// <summary> Wave Right Controller Characteristics </summary>
		public const InputDeviceCharacteristics kControllerRightCharacteristics = (
			InputDeviceCharacteristics.Right |
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Controller |
			InputDeviceCharacteristics.HeldInHand
		);

		public static InputDeviceCharacteristics characteristic(this ControlDevice cd)
		{
			return (
				cd == ControlDevice.Head ? kHMDCharacteristics : (
					cd == ControlDevice.Right ? kControllerRightCharacteristics : kControllerLeftCharacteristics
				)
			);
		}

		internal static List<InputDevice> m_InputDevices = new List<InputDevice>();

		/// Tracking state
		public static bool IsConnected(InputDeviceCharacteristics device)
		{
			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					if (m_InputDevices[i].characteristics.Equals(device))
						return true;
				}
			}

			return false;
		}
		public static bool IsConnected(ControlDevice device) { return IsConnected(device.characteristic()); }

		public static bool IsTracked(InputDeviceCharacteristics device)
		{
			bool isTracked = false;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool value))
							isTracked = value;
					}
				}
			}

			return isTracked;
		}
		public static bool IsTracked(ControlDevice device) { return IsTracked(device.characteristic()); }

		/// Button
		public static bool KeyDown(InputDeviceCharacteristics device, InputFeatureUsage<bool> button)
		{
			bool isDown = false;

			InputDevices.GetDevices(m_InputDevices);
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

			InputDevices.GetDevices(m_InputDevices);
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

			InputDevices.GetDevices(m_InputDevices);
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
		public static bool TryGetHapticCapabilities(InputDeviceCharacteristics device, out HapticCapabilities hapticCaps)
		{
			hapticCaps = emptyHapticCapabilities;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetHapticCapabilities(out HapticCapabilities value))
						{
							hapticCaps = value;
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
			InputDevices.GetDevices(m_InputDevices);
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
		public static bool GetPosition(InputDeviceCharacteristics device, out Vector3 position)
		{
			position = Vector3.zero;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool tracked))
						{
							if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 value))
							{
								position = value;
								return true;
							}
						}
					}
				}
			}

			return false;
		}
		public static bool GetPosition(ControlDevice device, out Vector3 position) { return GetPosition(device.characteristic(), out position); }
		public static bool GetRotation(InputDeviceCharacteristics device, out Quaternion rotation)
		{
			rotation = Quaternion.identity;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool tracked))
						{
							if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion value))
							{
								rotation = value;
								return true;
							}
						}
					}
				}
			}

			return false;
		}
		public static bool GetRotation(ControlDevice device, out Quaternion rotation) { return GetRotation(device.characteristic(), out rotation); }
		public static bool GetVelocity(InputDeviceCharacteristics device, out Vector3 velocity)
		{
			velocity = Vector3.zero;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool tracked))
						{
							if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 value))
							{
								velocity = value;
								return true;
							}
						}
					}
				}
			}

			return false;
		}
		public static bool GetVelocity(ControlDevice device, out Vector3 velocity) { return GetVelocity(device.characteristic(), out velocity); }
		public static bool GetAngularVelocity(InputDeviceCharacteristics device, out Vector3 angularVelocity)
		{
			angularVelocity = Vector3.zero;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool tracked))
						{
							if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 value))
							{
								angularVelocity = value;
								return true;
							}
						}
					}
				}
			}

			return false;
		}
		public static bool GetAngularVelocity(ControlDevice device, out Vector3 angularVelocity) { return GetAngularVelocity(device.characteristic(), out angularVelocity); }

		/// Battery
		public static float GetBatteryLevel(InputDeviceCharacteristics device)
		{
			float level = 0;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(device))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.batteryLevel, out float value))
							level = value;
					}
				}
			}

			return level;
		}
		public static float GetBatteryLevel(ControlDevice device) { return GetBatteryLevel(device.characteristic()); }

		public static bool IsUserPresence()
		{
			bool userPresence = false;

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					// The device is connected.
					if (m_InputDevices[i].characteristics.Equals(kHMDCharacteristics))
					{
						if (m_InputDevices[i].TryGetFeatureValue(CommonUsages.userPresence, out bool value))
							userPresence = value;
					}
				}
			}

			return userPresence;
		}

		public static bool Name(ControlDevice device, out string name)
		{
			name = "";

			InputDevices.GetDevices(m_InputDevices);
			if (m_InputDevices != null && m_InputDevices.Count > 0)
			{
				for (int i = 0; i < m_InputDevices.Count; i++)
				{
					if (m_InputDevices[i].characteristics.Equals(device.characteristic()))
					{
						name = m_InputDevices[i].name;
						return true;
					}
				}
			}

			return false;
		}
	}
}