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
	public static class InputDeviceHand
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceHand";
		static void DEBUG(string msg) { UnityEngine.Debug.Log(LOG_TAG + " " + msg); }

		public enum TrackingStatus : UInt32
		{
			NOT_START = 0,
			START_FAILURE = 1,
			STARTING = 2,
			STOPPING = 3,
			AVAILABLE = 4,
			UNSUPPORT = 5,
		}
		private static UInt32 Id(this TrackingStatus status) { return (UInt32)status; }

		#region Wave XR Interface
		private const string kNaturalHandStatus = "NaturalHandStatus";

		/// <summary>
		/// Activates the Wave Natural Hand Tracking feature.
		/// </summary>
		/// <param name="active">True for activation.</param>
		public static void ActivateNaturalHand(bool active)
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null && settings.EnableNaturalHand != active)
			{
				settings.EnableNaturalHand = active;
				string caller = "TBD";
				var frame = new StackFrame(1, true);
				if (frame != null)
				{
					var method = frame.GetMethod();
					if (method != null)
						caller = method.Name;
				}
				DEBUG("ActivateNaturalHand() " + (settings.EnableNaturalHand ? "Activate." : "Deactivate.") + " from " + caller);
				SettingsHelper.SetBool(WaveXRSettings.EnableNaturalHandText, settings.EnableNaturalHand);
			}
		}

		/// <summary>
		/// Retrieves current Natural Hand Tracking status.
		/// </summary>
		/// <returns>ID refers to <see cref="TrackingStatus">TrackingStatus</see>.</returns>
		public static TrackingStatus GetNaturalHandStatus()
		{
			UInt32 id = 5; // UNSUPPORT
			SettingsHelper.GetInt(kNaturalHandStatus, ref id);

			TrackingStatus status = TrackingStatus.UNSUPPORT;
			if (id == 0) { status = TrackingStatus.NOT_START; }
			if (id == 1) { status = TrackingStatus.START_FAILURE; }
			if (id == 2) { status = TrackingStatus.STARTING; }
			if (id == 3) { status = TrackingStatus.STOPPING; }
			if (id == 4) { status = TrackingStatus.AVAILABLE; }

			//Debug.Log(LOG_TAG + " GetNaturalHandStatus() " + status);
			return status;
		}

		[Obsolete("Electronic hand is NOT supported currently.")]
		public static void ActivateElectronicHand(bool active)
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null && settings.EnableElectronicHand != active)
			{
				settings.EnableElectronicHand = active;
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
				DEBUG("ActivateElectronicHand() " + (settings.EnableElectronicHand ? "Activate." : "Deactivate.") + " from " + caller);
				SettingsHelper.SetBool(WaveXRSettings.EnableElectronicHandText, settings.EnableElectronicHand);
			}
		}
		#endregion

		#region Wave XR Constants
		public const string kLeftHandName = "Wave Left Hand";
		public const string kRightHandName = "Wave Right Hand";
		public const string kLeftHandSN = "HTC-211116-LeftHand";
		public const string kRightHandSN = "HTC-211116-RightHand";

		public const string kHandConfidenceLeft = "HandConfidenceLeft";
		public const string kHandConfidenceRight = "HandConfidenceRight";
		public const string kHandScaleLeftX = "HandScaleLeftX", kHandScaleLeftY = "HandScaleLeftY", kHandScaleLeftZ = "HandScaleLeftZ";
		public const string kHandScaleRightX = "HandScaleRightX", kHandScaleRightY = "HandScaleRightY", kHandScaleRightZ = "HandScaleRightZ";

		/// <summary> Right Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kRightHandCharacteristics = (
			InputDeviceCharacteristics.HandTracking |
			InputDeviceCharacteristics.Right
		);
		/// <summary> Left Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kLeftHandCharacteristics = (
			InputDeviceCharacteristics.HandTracking |
			InputDeviceCharacteristics.Left
		);

		public const int kUnityXRFingerCount = 5;

		/**
		 * UnityEngine.XR Enumeration describing the AR rendering mode used with XR.Hand.
		 * public enum HandFinger
		 * {
		 *   Thumb = 0,
		 *   Index = 1,
		 *   Middle = 2,
		 *   Ring = 3,
		 *   Pinky = 4
		 * }
		 **/
		public const int kUnityXRMaxFingerBoneCount = 5;
		#endregion

		/// <summary>
		/// Checks if the Hand Tracking service is available.
		///
		/// Note the hand pose may not be tracked even if the Hand Tracking service is available. You can use the <see cref="IsTracked(bool)">IsTracked</see> function to check.
		/// </summary>
		/// <returns>True for available.</returns>
		public static bool IsAvailable() { return (GetNaturalHandStatus() == TrackingStatus.AVAILABLE); }

		[Obsolete("This API is deprecated. Please use IsAvailable() instead.")]
		public static bool IsAvailable(bool isLeft)
		{
			return IsAvailable();
		}

		public static string GetName(bool isLeft)
		{
			return (isLeft ? kLeftHandName : kRightHandName);
		}
		public static string GetSerialNumber(bool isLeft)
		{
			return (isLeft ? kLeftHandSN : kRightHandSN);
		}
		public static InputDeviceCharacteristics GetCharacteristic(bool isLeft)
		{
			return (isLeft ? kLeftHandCharacteristics : kRightHandCharacteristics);
		}
		/// <summary>
		/// Checks if an InputDevice is a Wave hand device.
		/// </summary>
		/// <param name="input">An <see href="https://docs.unity3d.com/ScriptReference/XR.InputDevice.html">XR InputDevice</see>.</param>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>True for Wave hand device.</returns>
		public static bool IsHandDevice(InputDevice input, bool isLeft)
		{
			if (input.name.Equals(GetName(isLeft)) &&
				input.serialNumber.Equals(GetSerialNumber(isLeft)) &&
				input.characteristics.Equals(GetCharacteristic(isLeft))
				)
			{
				return true;
			}

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

		private static Dictionary<bool, bool> s_IsConnected = new Dictionary<bool, bool>()
		{
			{ false, false }, // right
			{ true, false }, // left
		};
		private static Dictionary<bool, int> isConnectedFrame = new Dictionary<bool, int>()
		{
			{ false, -1 }, // right
			{ true, -1 }, // left
		};
		private static bool UpdateConnectedDevice(bool isLeft)
		{
			if (isConnectedFrame[isLeft] != Time.frameCount)
			{
				isConnectedFrame[isLeft] = Time.frameCount;
				return true;
			}
			return false;
		}
		private static bool IsHandDeviceConnected(InputDevice input, bool isLeft)
		{
			if (IsHandDevice(input, isLeft))
				return input.isValid;

			return false;
		}
		/// <summary>
		/// Checks if a Wave hand device is connected.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>True for conencted.</returns>
		public static bool IsHandDeviceConnected(bool isLeft)
		{
			if (!UpdateConnectedDevice(isLeft)) { return s_IsConnected[isLeft]; }

			UpdateInputDevices();
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (IsHandDeviceConnected(s_InputDevices[i], isLeft))
				{
					s_IsConnected[isLeft] = true;
					return true;
				}
			}

			s_IsConnected[isLeft] = false;
			return false;
		}

		private static Dictionary<bool, bool> s_IsTracked = new Dictionary<bool, bool>()
		{
			{ false, false }, // right
			{ true, false }, // left
		};
		private static Dictionary<bool, int> isTrackedFrame = new Dictionary<bool, int>()
		{
			{ false, -1 }, // right
			{ true, -1 }, // left
		};
		private static bool UpdateTrackedDevice(bool isLeft)
		{
			if (isTrackedFrame[isLeft] != Time.frameCount)
			{
				isTrackedFrame[isLeft] = Time.frameCount;
				return true;
			}
			return false;
		}
		private static bool IsTracked(InputDevice input, bool isLeft)
		{
			if (IsHandDeviceConnected(input, isLeft))
			{
				if (input.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
					return isTracked;
			}
			return false;
		}
		/// <summary>
		/// Checks is the left/right hand pose is <see href="https://docs.unity3d.com/ScriptReference/XR.CommonUsages-isTracked.html">tracked</see>.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>Tracked for valid pose.</returns>
		public static bool IsTracked(bool isLeft)
		{
			if (!UpdateTrackedDevice(isLeft)) { return s_IsTracked[isLeft]; }

			UpdateInputDevices();
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (IsTracked(s_InputDevices[i], isLeft))
				{
					s_IsTracked[isLeft] = true;
					return true;
				}
			}

			s_IsTracked[isLeft] = false;
			return false;
		}

		internal static Dictionary<bool, Bone> m_Palm = new Dictionary<bool, Bone>()
		{
			{ false, new Bone() },
			{ true, new Bone() },
		};
		private static Dictionary<bool, int> palmFrame = new Dictionary<bool, int>()
		{
			{ false, -1 }, // right
			{ true, -1 }, // left
		};
		private static bool UpdatePalm(bool isLeft)
		{
			if (palmFrame[isLeft] != Time.frameCount)
			{
				palmFrame[isLeft] = Time.frameCount;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Retrieves the <see href="https://docs.unity3d.com/ScriptReference/XR.Bone.html">bone data</see> of Palm.
		/// </summary>
		/// <param name="isLeft">True for left hand</param>
		/// <returns>The <see href="https://docs.unity3d.com/ScriptReference/XR.Bone.html">bone data</see> of Palm.</returns>
		public static Bone GetPalm(bool isLeft)
		{
			if (!UpdatePalm(isLeft)) { return m_Palm[isLeft]; }

			UpdateInputDevices();
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!IsTracked(s_InputDevices[i], isLeft)) { continue; }

				if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.handData, out Hand value))
				{
					if (value.TryGetRootBone(out Bone bone))
					{
						m_Palm[isLeft] = bone;
					}
				}
			}
			return m_Palm[isLeft];
		}

		internal static Dictionary<bool, Bone> m_Wrist = new Dictionary<bool, Bone>()
		{
			{ false, new Bone() },
			{ true, new Bone() },
		};
		private static Dictionary<bool, int> wristFrame = new Dictionary<bool, int>()
		{
			{ false, -1 }, // right
			{ true, -1 }, // left
		};
		private static bool UpdateWrist(bool isLeft)
		{
			if (wristFrame[isLeft] != Time.frameCount)
			{
				wristFrame[isLeft] = Time.frameCount;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Retrieves the <see href="https://docs.unity3d.com/ScriptReference/XR.Bone.html">bone data</see> of Wrist.
		/// </summary>
		/// <param name="isLeft">True for left hand</param>
		/// <returns>The <see href="https://docs.unity3d.com/ScriptReference/XR.Bone.html">bone data</see> of Wrist.</returns>
		public static Bone GetWrist(bool isLeft)
		{
			if (!UpdateWrist(isLeft)) { return m_Wrist[isLeft]; }

			if (GetPalm(isLeft).TryGetParentBone(out Bone value))
				m_Wrist[isLeft] = value;

			return m_Wrist[isLeft];
		}

		internal static Dictionary<bool, Dictionary<HandFinger, List<Bone>>> s_FingerBones = new Dictionary<bool, Dictionary<HandFinger, List<Bone>>>()
		{
			{ false, new Dictionary<HandFinger, List<Bone>>() {
					{ HandFinger.Thumb, new List<Bone>() },
					{ HandFinger.Index, new List<Bone>() },
					{ HandFinger.Middle, new List<Bone>() },
					{ HandFinger.Ring, new List<Bone>() },
					{ HandFinger.Pinky, new List<Bone>() }
				}
			},
			{ true, new Dictionary<HandFinger, List<Bone>>() {
					{ HandFinger.Thumb, new List<Bone>() },
					{ HandFinger.Index, new List<Bone>() },
					{ HandFinger.Middle, new List<Bone>() },
					{ HandFinger.Ring, new List<Bone>() },
					{ HandFinger.Pinky, new List<Bone>() }
				}
			}
		};
		private static Dictionary<bool, Dictionary<HandFinger, int>> fingerBonesFrame = new Dictionary<bool, Dictionary<HandFinger, int>>()
		{
			{ false, new Dictionary<HandFinger, int>() {
					{ HandFinger.Thumb, -1 },
					{ HandFinger.Index, -1 },
					{ HandFinger.Middle, -1 },
					{ HandFinger.Ring, -1 },
					{ HandFinger.Pinky, -1 }
				}
			},
			{ true, new Dictionary<HandFinger, int>() {
					{ HandFinger.Thumb, -1 },
					{ HandFinger.Index, -1 },
					{ HandFinger.Middle, -1 },
					{ HandFinger.Ring, -1 },
					{ HandFinger.Pinky, -1 }
				}
			}
		};
		private static bool UpdateFingerBones(bool isLeft, HandFinger finger)
		{
			if (fingerBonesFrame[isLeft][finger] != Time.frameCount)
			{
				fingerBonesFrame[isLeft][finger] = Time.frameCount;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Retrieves the bone list of a finger.
		/// The list length will be zero if cannot find a finger's bone list.
		/// </summary>
		/// <param name="isLeft">True for left hand</param>
		/// <param name="finger">The finger of <see href="https://docs.unity3d.com/ScriptReference/XR.HandFinger.html">XR HandFinger</see>.</param>
		/// <returns>The <see href="https://docs.unity3d.com/ScriptReference/XR.Bone.html">bone</see> list of a finger.</returns>
		public static List<Bone> GetFingerBones(bool isLeft, HandFinger finger)
		{
			if (!UpdateFingerBones(isLeft, finger)) { return s_FingerBones[isLeft][finger]; }

			UpdateInputDevices();
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!IsTracked(s_InputDevices[i], isLeft)) { continue; }

				if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.handData, out Hand handData))
				{
					if (!handData.TryGetFingerBones(finger, s_FingerBones[isLeft][finger]))
					{
						s_FingerBones[isLeft][finger].Clear();
					}
				}
			}
			return s_FingerBones[isLeft][finger];
		}

		/// <summary>
		/// Retrieves the scale of wrist.
		/// </summary>
		/// <param name="isLeft">Left or right hand.</param>
		/// <param name="scale">Wrist scale in Vector3.</param>
		/// <returns></returns>
		public static bool GetHandScale(bool isLeft, out Vector3 scale)
		{
			float scale_x = 0, scale_y = 0, scale_z = 0;
			if (isLeft)
			{
				SettingsHelper.GetFloat(kHandScaleLeftX, ref scale_x);
				SettingsHelper.GetFloat(kHandScaleLeftY, ref scale_y);
				SettingsHelper.GetFloat(kHandScaleLeftZ, ref scale_z);
			}
			else
			{
				SettingsHelper.GetFloat(kHandScaleRightX, ref scale_x);
				SettingsHelper.GetFloat(kHandScaleRightY, ref scale_y);
				SettingsHelper.GetFloat(kHandScaleRightZ, ref scale_z);
			}
			scale.x = scale_x;
			scale.y = scale_y;
			scale.z = scale_z;
			return true;
		}

		/// <summary>
		/// Retrieves the wrist confidence which is a 0~1 float value where 1 means the most accurate.
		/// </summary>
		/// <param name="isLeft">Left or right hand.</param>
		/// <returns></returns>
		public static float GetHandConfidence(bool isLeft)
		{
			float confidence = 0;
			if (isLeft)
				SettingsHelper.GetFloat(kHandConfidenceLeft, ref confidence);
			else
				SettingsHelper.GetFloat(kHandConfidenceRight, ref confidence);
			return confidence;
		}
	}
}