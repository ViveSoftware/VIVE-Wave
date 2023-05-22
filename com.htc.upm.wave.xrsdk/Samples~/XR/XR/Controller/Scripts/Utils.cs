// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.XR;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
	public static class Utils
	{
		public enum DeviceTypes : UInt32
		{
			HMD = 0,
			ControllerLeft = 1,
			ControllerRight = 2,

			Tracker0 = 10,
			Tracker1 = 11,
			Tracker2 = 12,
			Tracker3 = 13,
			Tracker4 = 14,
			Tracker5 = 15,
			Tracker6 = 16,
			Tracker7 = 17,

			Eye = 3,
		}

		public enum BinaryButtons : UInt32
		{
			menuButton,
			gripButton,
			primaryButton,
			primaryTouch,
			secondaryButton,
			secondaryTouch,
			primary2DAxisClick,
			primary2DAxisTouch,
			triggerButton,
			secondary2DAxisClick,
			secondary2DAxisTouch,
		}
		public static InputFeatureUsage<bool> InputFeature(this BinaryButtons button)
		{
			if (button == BinaryButtons.menuButton) { return UnityEngine.XR.CommonUsages.menuButton; }
			if (button == BinaryButtons.gripButton) { return UnityEngine.XR.CommonUsages.gripButton; }
			if (button == BinaryButtons.primaryButton) { return UnityEngine.XR.CommonUsages.primaryButton; }
			if (button == BinaryButtons.primaryTouch) { return UnityEngine.XR.CommonUsages.primaryTouch; }
			if (button == BinaryButtons.secondaryButton) { return UnityEngine.XR.CommonUsages.secondaryButton; }
			if (button == BinaryButtons.secondaryTouch) { return UnityEngine.XR.CommonUsages.secondaryTouch; }
			if (button == BinaryButtons.primary2DAxisClick) { return UnityEngine.XR.CommonUsages.primary2DAxisClick; }
			if (button == BinaryButtons.secondary2DAxisClick) { return UnityEngine.XR.CommonUsages.secondary2DAxisClick; }
			if (button == BinaryButtons.triggerButton) { return UnityEngine.XR.CommonUsages.triggerButton; }
			if (button == BinaryButtons.primary2DAxisTouch) { return UnityEngine.XR.CommonUsages.primary2DAxisTouch; }

			return UnityEngine.XR.CommonUsages.secondary2DAxisTouch;
		}

		public enum Vector2Buttons : UInt32
		{
			primary2DAxis,
			secondary2DAxis,
		}
		public static InputFeatureUsage<Vector2> InputFeature(this Vector2Buttons button)
		{
			if (button == Vector2Buttons.secondary2DAxis) { return UnityEngine.XR.CommonUsages.secondary2DAxis; }
			return UnityEngine.XR.CommonUsages.primary2DAxis;
		}

		public enum FloatButtons : UInt32
		{
			trigger,
			grip
		}
		public static InputFeatureUsage<float> InputFeature(this FloatButtons button)
		{
			if (button == FloatButtons.grip) { return UnityEngine.XR.CommonUsages.grip; }
			return UnityEngine.XR.CommonUsages.trigger;
		}

#if ENABLE_INPUT_SYSTEM
		private static bool VALIDATE(InputActionReference actionReference, out string msg)
		{
			msg = "Normal";

			if (actionReference == null)
			{
				msg = "Null reference.";
				return false;
			}
			else if (actionReference.action == null)
			{
				msg = "Null reference action.";
				return false;
			}
			else if (!actionReference.action.enabled)
			{
				msg = "Reference action disabled.";
				return false;
			}
			else if (actionReference.action.activeControl == null)
			{
				msg = "No active control of the reference action, phase: " + actionReference.action.phase;
				return false;
			}
			else if (actionReference.action.controls.Count <= 0)
			{
				msg = "Action control count is " + actionReference.action.controls.Count;
				return false;
			}

			return true;
		}
		public static bool GetButton(InputActionReference actionReference, out bool value, out string msg)
		{
			value = false;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(float))
					value = actionReference.action.ReadValue<float>() > 0;
				if (actionReference.action.activeControl.valueType == typeof(bool))
					value = actionReference.action.ReadValue<bool>();

				return true;
			}

			return false;
		}
		public static bool GetInteger(InputActionReference actionReference, out InputTrackingState value, out string msg)
		{
			value = 0;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(int))
				{
					int diff = 0;
					int i = actionReference.action.ReadValue<int>();

					diff = i & ((int)InputTrackingState.Position);
					if (diff != 0) { value |= InputTrackingState.Position; }

					diff = i & ((int)InputTrackingState.Rotation);
					if (diff != 0) { value |= InputTrackingState.Rotation; }

					diff = i & ((int)InputTrackingState.Velocity);
					if (diff != 0) { value |= InputTrackingState.Velocity; }

					diff = i & ((int)InputTrackingState.AngularVelocity);
					if (diff != 0) { value |= InputTrackingState.AngularVelocity; }

					diff = i & ((int)InputTrackingState.Acceleration);
					if (diff != 0) { value |= InputTrackingState.Acceleration; }

					diff = i & ((int)InputTrackingState.AngularAcceleration);
					if (diff != 0) { value |= InputTrackingState.AngularAcceleration; }
				}

				return true;
			}

			return false;
		}
		public static bool GetVector3(InputActionReference actionReference, out Vector3 value, out string msg)
		{
			value = Vector3.zero;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(Vector3))
					value = actionReference.action.ReadValue<Vector3>();

				return true;
			}

			return false;
		}
		public static bool GetQuaternion(InputActionReference actionReference, out Quaternion value, out string msg)
		{
			value = Quaternion.identity;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(Quaternion))
					value = actionReference.action.ReadValue<Quaternion>();

				Vector3 direction = value * Vector3.forward;
				return true;
			}

			return false;
		}
#endif
	}
}
