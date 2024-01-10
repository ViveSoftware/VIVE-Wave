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
		public enum HandMotion : UInt32
		{
			None = 0,//WVR_HandPoseType.WVR_HandPoseType_Invalid,
			Pinch = 1,//WVR_HandPoseType.WVR_HandPoseType_Pinch,
			Hold = 2,//WVR_HandPoseType.WVR_HandPoseType_Hold,
		}
		public enum HandHoldRole : UInt32
		{
			None = 0,//WVR_HandHoldRoleType.WVR_HandHoldRoleType_None,
			Main = 1,//WVR_HandHoldRoleType.WVR_HandHoldRoleType_MainHold,
			Side = 2,//WVR_HandHoldRoleType.WVR_HandHoldRoleType_SideHold,
		}
		public enum HandHoldType : UInt32
		{
			None = 0,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_None,
			Gun = 1,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_Gun,
			OCSpray = 2,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_OCSpray,
			LongGun = 3,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_LongGun,
			Baton = 4,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_Baton,
			FlashLight = 5,//WVR_HandHoldObjectType.WVR_HandHoldObjectType_FlashLight,
		}

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
		public const string kWristLinearVelocityLeftX = "WristLinearVelocityLeftX", kWristLinearVelocityLeftY = "WristLinearVelocityLeftY", kWristLinearVelocityLeftZ = "WristLinearVelocityLeftZ";
		public const string kWristLinearVelocityRightX = "WristLinearVelocityRightX", kWristLinearVelocityRightY = "WristLinearVelocityRightY", kWristLinearVelocityRightZ = "WristLinearVelocityRightZ";
		public const string kWristAngularVelocityLeftX = "WristAngularVelocityLeftX", kWristAngularVelocityLeftY = "WristAngularVelocityLeftY", kWristAngularVelocityLeftZ = "WristAngularVelocityLeftZ";
		public const string kWristAngularVelocityRightX = "WristAngularVelocityRightX", kWristAngularVelocityRightY = "WristAngularVelocityRightY", kWristAngularVelocityRightZ = "WristAngularVelocityRightZ";
		public const string kHandMotionLeft = "HandMotionLeft";
		public const string kHandMotionRight = "HandMotionRight";
		public const string kHandRoleLeft = "HandRoleLeft";
		public const string kHandRoleRight = "HandRoleRight";
		public const string kHandObjectLeft = "HandObjectLeft";
		public const string kHandObjectRight = "HandObjectRight";
		public const string kHandOriginLeftX = "HandOriginLeftX", kHandOriginLeftY = "HandOriginLeftY", kHandOriginLeftZ = "HandOriginLeftZ";
		public const string kHandOriginRightX = "HandOriginRightX", kHandOriginRightY = "HandOriginRightY", kHandOriginRightZ = "HandOriginRightZ";
		public const string kHandDirectionLeftX = "HandDirectionLeftX", kHandDirectionLeftY = "HandDirectionLeftY", kHandDirectionLeftZ = "HandDirectionLeftZ";
		public const string kHandDirectionRightX = "HandDirectionRightX", kHandDirectionRightY = "HandDirectionRightY", kHandDirectionRightZ = "HandDirectionRightZ";
		public const string kHandStrengthLeft = "HandStrengthLeft";
		public const string kHandStrengthRight = "HandStrengthRight";
		public const string kHandPinchThreshold = "HandPinchThreshold";
		public const string kHandPinchOffThreshold = "HandPinchOffThreshold";
		public const string kHandIsPinchingLeft = "HandIsPinchingLeft";
		public const string kHandIsPinchingRight = "HandIsPinchingRight";
		public const string kHandGraspStrengthLeft = "HandGraspStrengthLeft";
		public const string kHandGraspStrengthRight = "HandGraspStrengthRight";
		public const string kHandIsGraspingLeft = "HandIsGraspingLeft";
		public const string kHandIsGraspingRight = "HandIsGraspingRight";

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

		internal static object inputDeviceLock = new object();
		internal static List<InputDevice> s_UpdatedDevices = new List<InputDevice>();
		internal static List<InputDevice> s_InputDevices 
		{
			get
			{
				lock (inputDeviceLock)
				{
					return s_UpdatedDevices;
				}
			}
		}
		internal static int inputDeviceFrame = -1;
		private static void UpdateInputDevices()
		{
			if (inputDeviceFrame != Time.frameCount)
			{
				inputDeviceFrame = Time.frameCount;
				lock (inputDeviceLock)
				{
					InputDevices.GetDevices(s_UpdatedDevices);
				}
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
			scale = Vector3.one;
			if (!IsAvailable()) { return false; }

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

		[Obsolete("This function is deprecated. Please use bool GetHandConfidence() instead.")]
		public static float GetHandConfidence(bool isLeft)
		{
			float confidence = 0;
			if (isLeft)
				SettingsHelper.GetFloat(kHandConfidenceLeft, ref confidence);
			else
				SettingsHelper.GetFloat(kHandConfidenceRight, ref confidence);
			return confidence;
		}
		/// <summary>
		/// Retrieves the wrist confidence which is a 0~1 float value where 1 means the most accurate.
		/// </summary>
		/// <param name="isLeft">Left or right hand.</param>
		/// <param name="confidence">0~1 float value where 1 means the most accurate.</param>
		/// <returns>True for the confidence is available.</returns>
		public static bool GetHandConfidence(bool isLeft, out float confidence)
		{
			confidence = 0;
			if (!IsAvailable()) { return false; }
#pragma warning disable
			confidence = GetHandConfidence(isLeft);
#pragma warning enable
			return true;
		}

		/// <summary>
		/// Retrieves the left/right wrist velocity.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="velocity">Velocity in Vector3.</param>
		/// <returns>True for valid velocity.</returns>
		public static bool GetWristLinearVelocity(bool isLeft, out Vector3 velocity)
		{
			velocity = Vector3.zero;
			if (!IsAvailable()) { return false; }

			float velocity_x = 0, velocity_y = 0, velocity_z = 0;
			if (isLeft)
			{
				SettingsHelper.GetFloat(kWristLinearVelocityLeftX, ref velocity_x);
				SettingsHelper.GetFloat(kWristLinearVelocityLeftY, ref velocity_y);
				SettingsHelper.GetFloat(kWristLinearVelocityLeftZ, ref velocity_z);
			}
			else
			{
				SettingsHelper.GetFloat(kWristLinearVelocityRightX, ref velocity_x);
				SettingsHelper.GetFloat(kWristLinearVelocityRightY, ref velocity_y);
				SettingsHelper.GetFloat(kWristLinearVelocityRightZ, ref velocity_z);
			}
			velocity.x = velocity_x;
			velocity.y = velocity_y;
			velocity.z = velocity_z;
			return true;
		}
		/// <summary>
		/// Retrieves the left/right wrist angular velocity.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="velocity">Angular velocity in Vector3.</param>
		/// <returns>True for valid angular velocity.</returns>
		public static bool GetWristAngularVelocity(bool isLeft, out Vector3 velocity)
		{
			velocity = Vector3.zero;
			if (!IsAvailable()) { return false; }

			float velocity_x = 0, velocity_y = 0, velocity_z = 0;
			if (isLeft)
			{
				SettingsHelper.GetFloat(kWristAngularVelocityLeftX, ref velocity_x);
				SettingsHelper.GetFloat(kWristAngularVelocityLeftY, ref velocity_y);
				SettingsHelper.GetFloat(kWristAngularVelocityLeftZ, ref velocity_z);
			}
			else
			{
				SettingsHelper.GetFloat(kWristAngularVelocityRightX, ref velocity_x);
				SettingsHelper.GetFloat(kWristAngularVelocityRightY, ref velocity_y);
				SettingsHelper.GetFloat(kWristAngularVelocityRightZ, ref velocity_z);
			}
			velocity.x = velocity_x;
			velocity.y = velocity_y;
			velocity.z = velocity_z;
			return true;
		}
		/// <summary>
		/// Retrieves current left/right hand motion.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="motion">None, Hold or Pinch.</param>
		/// <returns>True for valid motion.</returns>
		public static bool GetHandMotion(bool isLeft, out HandMotion motion)
		{
			motion = HandMotion.None;
			if (!IsAvailable()) { return false; }

			uint motionId = (uint)motion;

			if (isLeft)
				SettingsHelper.GetInt(kHandMotionLeft, ref motionId);
			else
				SettingsHelper.GetInt(kHandMotionRight, ref motionId);

			if (motionId == 1) { motion = HandMotion.Pinch; }
			if (motionId == 2) { motion = HandMotion.Hold; }
			return true;
		}
		/// <summary>
		/// Retrieves the role of left/right hand while holding.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="role">Main hold or Side hold.</param>
		/// <returns>True for valid role.</returns>
		public static bool GetHandHoldRole(bool isLeft, out HandHoldRole role)
		{
			role = HandHoldRole.None;
			if (!IsAvailable()) { return false; }

			uint roleId = (uint)role;

			if (isLeft)
				SettingsHelper.GetInt(kHandRoleLeft, ref roleId);
			else
				SettingsHelper.GetInt(kHandRoleRight, ref roleId);

			if (roleId == 1) { role = HandHoldRole.Main; }
			if (roleId == 2) { role = HandHoldRole.Side; }
			return true;
		}
		/// <summary>
		/// Retrieves the type of left/right handheld object.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="type">See <see cref="HandHoldType">HandHoldType</see>.</param>
		/// <returns>True for valid type.</returns>
		public static bool GetHandHoldType(bool isLeft, out HandHoldType type)
		{
			type = HandHoldType.None;
			if (!IsAvailable()) { return false; }

			uint typeId = (uint)type;

			if (isLeft)
				SettingsHelper.GetInt(kHandObjectLeft, ref typeId);
			else
				SettingsHelper.GetInt(kHandObjectRight, ref typeId);

			if (typeId == 1) { type = HandHoldType.Gun; }
			if (typeId == 2) { type = HandHoldType.OCSpray; }
			if (typeId == 3) { type = HandHoldType.LongGun; }
			if (typeId == 4) { type = HandHoldType.Baton; }
			if (typeId == 5) { type = HandHoldType.FlashLight; }
			return true;
		}

		/// <summary>
		/// Retrieves the origin of left/right hand pinch.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="origin">World space origin in Vector3.</param>
		/// <returns>True for valid origin.</returns>
		public static bool GetPinchOrigin(bool isLeft, out Vector3 origin)
		{
			origin = Vector3.zero;
			if (!IsAvailable()) { return false; }

			float origin_x = 0, origin_y = 0, origin_z = 0;
			if (isLeft)
			{
				SettingsHelper.GetFloat(kHandOriginLeftX, ref origin_x);
				SettingsHelper.GetFloat(kHandOriginLeftY, ref origin_y);
				SettingsHelper.GetFloat(kHandOriginLeftZ, ref origin_z);
			}
			else
			{
				SettingsHelper.GetFloat(kHandOriginRightX, ref origin_x);
				SettingsHelper.GetFloat(kHandOriginRightY, ref origin_y);
				SettingsHelper.GetFloat(kHandOriginRightZ, ref origin_z);
			}
			origin.x = origin_x;
			origin.y = origin_y;
			origin.z = origin_z;
			return true;
		}
		/// <summary>
		/// Retrieves the direction of left/right hand pinch.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="direction">World space direction in Vector3.</param>
		/// <returns>True for valid direction.</returns>
		public static bool GetPinchDirection(bool isLeft, out Vector3 direction)
		{
			direction = Vector3.forward;
			if (!IsAvailable()) { return false; }

			float direction_x = 0, direction_y = 0, direction_z = 0;
			if (isLeft)
			{
				SettingsHelper.GetFloat(kHandDirectionLeftX, ref direction_x);
				SettingsHelper.GetFloat(kHandDirectionLeftY, ref direction_y);
				SettingsHelper.GetFloat(kHandDirectionLeftZ, ref direction_z);
			}
			else
			{
				SettingsHelper.GetFloat(kHandDirectionRightX, ref direction_x);
				SettingsHelper.GetFloat(kHandDirectionRightY, ref direction_y);
				SettingsHelper.GetFloat(kHandDirectionRightZ, ref direction_z);
			}
			direction.x = direction_x;
			direction.y = direction_y;
			direction.z = direction_z;
			return true;
		}
		/// <summary>
		/// Retrieves the strength of left/right hand pinch.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="strength">The strength (0~1) where 1 means the thumb tip and index tip is touching.</param>
		/// <returns></returns>
		public static bool GetPinchStrength(bool isLeft, out float strength)
		{
			strength = 0;
			if (!IsAvailable()) { return false; }

			if (isLeft)
				SettingsHelper.GetFloat(kHandStrengthLeft, ref strength);
			else
				SettingsHelper.GetFloat(kHandStrengthRight, ref strength);

			return true;
		}
		/// <summary>
		/// Retrieves the system default threshold always used to judge if a "selection" happens when the pinch strength value is greater than the threshold.
		/// </summary>
		/// <param name="threshold">Threshold in float 0~1.</param>
		/// <returns>True for valid threshold.</returns>
		public static bool GetPinchThreshold(out float threshold)
		{
			threshold = 1; // The pinch strength will never > 1
			if (!IsAvailable()) { return false; }

			SettingsHelper.GetFloat(kHandPinchThreshold, ref threshold);
			return true;
		}
		/// <summary>
		/// Retrieves the system default threshold always used to judge if a "release" happens when a hand is pinching and the pinch strength value is less than the threshold.
		/// Note that the Pinch Off Threshold should NOT be greater than the Pinch Threshold.
		/// </summary>
		/// <param name="threshold">Threshold in float 0~1.</param>
		/// <returns>True for valid threshold.</returns>
		public static bool GetPinchOffThreshold(out float threshold)
		{
			threshold = 1; // The pinch strength will never > 1
			if (!IsAvailable()) { return false; }

			SettingsHelper.GetFloat(kHandPinchOffThreshold, ref threshold);
			return true;
		}
		/// <summary>
		/// Checks if a hand is pinching currently.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>True for pinching.</returns>
		public static bool IsHandPinching(bool isLeft)
		{
			bool isPinching = false;

			if (!GetHandMotion(isLeft, out HandMotion motion)) { return false; }
			if (motion != HandMotion.Pinch) { return false; }

			if (isLeft)
				SettingsHelper.GetBool(kHandIsPinchingLeft, ref isPinching);
			else
				SettingsHelper.GetBool(kHandIsPinchingRight, ref isPinching);

			return isPinching;
		}
		/// <summary>
		/// Retrieves the strength of left/right hand grasp.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <param name="strength">The strength (0~1) where 1 means the hand is grasping.</param>
		/// <returns></returns>
		public static bool GetGraspStrength(bool isLeft, out float strength)
		{
			strength = 0;
			if (!IsAvailable()) { return false; }

			if (isLeft)
				SettingsHelper.GetFloat(kHandGraspStrengthLeft, ref strength);
			else
				SettingsHelper.GetFloat(kHandGraspStrengthRight, ref strength);

			return true;
		}
		/// <summary>
		/// Checks if a hand is grasping currently.
		/// </summary>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>True for grasping.</returns>
		public static bool IsHandGrasping(bool isLeft)
		{
			bool isGrasping = false;

			if (isLeft)
				SettingsHelper.GetBool(kHandIsGraspingLeft, ref isGrasping);
			else
				SettingsHelper.GetBool(kHandIsGraspingRight, ref isGrasping);

			return isGrasping;
		}

	}
}