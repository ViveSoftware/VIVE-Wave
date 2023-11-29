// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.XR;
using Wave.Native;
using Wave.OpenXR;

namespace Wave.Essence
{
	/**
	 * Follows the order in CommonUsage
	 * public static InputFeatureUsage<bool> primaryButton;
	 * public static InputFeatureUsage<bool> primaryTouch;
	 * public static InputFeatureUsage<bool> secondaryButton;
	 * public static InputFeatureUsage<bool> secondaryTouch;
	 * public static InputFeatureUsage<bool> gripButton;
	 * public static InputFeatureUsage<bool> triggerButton;
	 * public static InputFeatureUsage<bool> menuButton;
	 * public static InputFeatureUsage<bool> primary2DAxisClick;
	 * public static InputFeatureUsage<bool> primary2DAxisTouch;
	 * public static InputFeatureUsage<bool> secondary2DAxisClick;
	 * public static InputFeatureUsage<bool> secondary2DAxisTouch;
	 * public static InputFeatureUsage<float> trigger;
	 * public static InputFeatureUsage<float> grip;
	 * public static InputFeatureUsage<Vector2> primary2DAxis;
	 * public static InputFeatureUsage<Vector2> secondary2DAxis;
	 **/
	public class XR_Feature
	{
		// A binary value representing the Button A(X) press state.
		public static readonly InputFeatureUsage<bool> primaryButton = CommonUsages.primaryButton;
		// A binary value representing the Button A(X) touch state.
		public static readonly InputFeatureUsage<bool> primaryTouch = CommonUsages.primaryTouch;
		// A binary value representing the Button B(Y) press state.
		public static readonly InputFeatureUsage<bool> secondaryButton = CommonUsages.secondaryButton;
		// A binary value representing the Button B(Y) touch state.
		public static readonly InputFeatureUsage<bool> secondaryTouch = CommonUsages.secondaryTouch;
		// A binary value representing the Grip press state.
		public static readonly InputFeatureUsage<bool> gripButton = CommonUsages.gripButton;
		// A binary value representing the Trigger press state.
		public static readonly InputFeatureUsage<bool> triggerButton = CommonUsages.triggerButton;
		// A binary value representing the Menu press state.
		public static readonly InputFeatureUsage<bool> menuButton = CommonUsages.menuButton;
		// A binary value representing the Touchpad or Thumbstick press state.
		public static readonly InputFeatureUsage<bool> primary2DAxisClick = CommonUsages.primary2DAxisClick;
		// A binary value representing the Touchpad or Thumbstick touch state.
		public static readonly InputFeatureUsage<bool> primary2DAxisTouch = CommonUsages.primary2DAxisTouch;
		// A binary value representing the Thumbstick press state.
		public static readonly InputFeatureUsage<bool> secondary2DAxisClick = CommonUsages.secondary2DAxisClick;
		// A binary value representing the Thumbstick touch state.
		public static readonly InputFeatureUsage<bool> secondary2DAxisTouch = CommonUsages.secondary2DAxisTouch;

		public static readonly InputFeatureUsage<bool> triggerTouch = new InputFeatureUsage<bool>("TriggerTouch");
		// A float value representing the Trigger axis.
		public static readonly InputFeatureUsage<float> trigger = CommonUsages.trigger;
		// A float value representing the Grip axis.
		public static readonly InputFeatureUsage<float> grip = CommonUsages.grip;

		// A Vector2 value representing the Touchpad or Thumbstick axis.
		public static readonly InputFeatureUsage<Vector2> primary2DAxis = CommonUsages.primary2DAxis;
		// A Vector2 value representing the Thumbstick axis.
		public static readonly InputFeatureUsage<Vector2> secondary2DAxis = CommonUsages.secondary2DAxis;

		// A binary feature that represents if the device is currently tracking properly.  True means fully tracked, false means either partially or not tracked.
		public static readonly InputFeatureUsage<bool> ValidPose = CommonUsages.isTracked;

		// A binary feature that represents a user is currently wearing a HMD.
		public static readonly InputFeatureUsage<bool> userPresence = CommonUsages.userPresence;

		public static readonly InputFeatureUsage<Vector3> devicePosition = CommonUsages.devicePosition;
		public static readonly InputFeatureUsage<Quaternion> deviceRotation = CommonUsages.deviceRotation;

		// A float value represents the current battery life of this device.
		public static readonly InputFeatureUsage<float> batteryLevel = CommonUsages.batteryLevel;
	}

	public class XR_BinaryButton
	{
		public static readonly InputFeatureUsage<bool> primaryButton = XR_Feature.primaryButton;
		public const string primaryButtonName = "PrimaryButton";
		public static readonly InputFeatureUsage<bool> primaryTouch = XR_Feature.primaryTouch;
		public const string primaryTouchName = "PrimaryTouch";
		public static readonly InputFeatureUsage<bool> secondaryButton = XR_Feature.secondaryButton;
		public const string secondaryButtonName = "SecondaryButton";
		public static readonly InputFeatureUsage<bool> secondaryTouch = XR_Feature.secondaryTouch;
		public const string secondaryTouchName = "SecondaryTouch";
		public static readonly InputFeatureUsage<bool> gripButton = XR_Feature.gripButton;
		public const string gripButtonName = "GripButton";
		public static readonly InputFeatureUsage<bool> triggerButton = XR_Feature.triggerButton;
		public const string triggerButtonName = "TriggerButton";
		public static readonly InputFeatureUsage<bool> triggerTouch = XR_Feature.triggerTouch;
		public const string triggerTouchName = "TriggerTouch";
		public static readonly InputFeatureUsage<bool> menuButton = XR_Feature.menuButton;
		public const string menuButtonName = "MenuButton";
		public static readonly InputFeatureUsage<bool> primary2DAxisClick = XR_Feature.primary2DAxisClick;
		public const string primary2DAxisClickName = "Primary2DAxisClick";
		public static readonly InputFeatureUsage<bool> primary2DAxisTouch = XR_Feature.primary2DAxisTouch;
		public const string primary2DAxisTouchName = "Primary2DAxisTouch";
		public static readonly InputFeatureUsage<bool> secondary2DAxisClick = XR_Feature.secondary2DAxisClick;
		public const string secondary2DAxisClickName = "Secondary2DAxisClick";
		public static readonly InputFeatureUsage<bool> secondary2DAxisTouch = XR_Feature.secondary2DAxisTouch;
		public const string secondary2DAxisTouchName = "Secondary2DAxisTouch";
	};

	public class XR_Axis1DButton
	{
		public static readonly InputFeatureUsage<float> trigger = XR_Feature.trigger;
		public const string triggerName = "Trigger";
		public static readonly InputFeatureUsage<float> grip = XR_Feature.grip;
		public const string gripName = "Grip";
	};

	public class XR_Axis2DButton
	{
		public static readonly InputFeatureUsage<Vector2> primary2DAxis = XR_Feature.primary2DAxis;
		public const string primary2DAxisName = "Primary2DAxis";
		public static readonly InputFeatureUsage<Vector2> secondary2DAxis = XR_Feature.secondary2DAxis;
		public const string secondary2DAxisName = "Secondary2DAxis";
	};

	public enum XR_Device
	{
		Head = WVR_DeviceType.WVR_DeviceType_HMD,
		Right = WVR_DeviceType.WVR_DeviceType_Controller_Right,
		Left = WVR_DeviceType.WVR_DeviceType_Controller_Left,

		Dominant = Right,
		NonDominant = Left
	};

	public enum XR_Hand
	{
		Right = XR_Device.Right,
		Left = XR_Device.Left,

		Dominant = Right,
		NonDominant = Left,
	}

	public enum XR_HandDevice
	{
		NaturalRight = WVR_DeviceType.WVR_DeviceType_NaturalHand_Right,
		NaturalLeft = WVR_DeviceType.WVR_DeviceType_NaturalHand_Left,
		ElectronicRight = WVR_DeviceType.WVR_DeviceType_ElectronicHand_Right,
		ElectronicLeft = WVR_DeviceType.WVR_DeviceType_ElectronicHand_Left,
		GestureRight = WVR_DeviceType.WVR_DeviceType_HandGesture_Right,
		GestureLeft = WVR_DeviceType.WVR_DeviceType_HandGesture_Left,
	};

	public enum XR_InteractionMode
	{
		Default = WVR_InteractionMode.WVR_InteractionMode_SystemDefault,
		Gaze = WVR_InteractionMode.WVR_InteractionMode_Gaze,
		Controller = WVR_InteractionMode.WVR_InteractionMode_Controller,
		Hand = WVR_InteractionMode.WVR_InteractionMode_Hand,
	}

	public enum XR_ControllerPoseMode
	{
		Raw = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw,
		Trigger = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger,
		Panel = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel,
		Handle = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle,
	}

	public static class DefinitionExtension
	{
		public static string Name(this XR_Device device)
		{
			if (device == XR_Device.Head) { return "Head"; }
			if (device == XR_Device.Dominant || device == XR_Device.Right) { return "Right"; }
			if (device == XR_Device.NonDominant || device == XR_Device.Left) { return "Left"; }

			return "Unknown";
		}
		public static string Name(this XR_InteractionMode mode)
		{
			if (mode == XR_InteractionMode.Gaze) { return "Gaze"; }
			if (mode == XR_InteractionMode.Controller) { return "Controller"; }
			if (mode == XR_InteractionMode.Hand) { return "Hand"; }
			return "Default";
		}
		public static string Name(this XR_Hand hand)
		{
			if (hand == XR_Hand.Dominant) { return "Dominant Hand"; }
			if (hand == XR_Hand.Right) { return "Right Hand"; }
			if (hand == XR_Hand.NonDominant) { return "NonDominant Hand"; }
			if (hand == XR_Hand.Left) { return "Left Hand"; }
			return "";
		}

		public static WVR_InputId ViveFocus3Button(this InputFeatureUsage<bool> input, bool isLeft)
		{
			switch(input.name)
			{
				case XR_BinaryButton.menuButtonName:
					return WVR_InputId.WVR_InputId_Alias1_Menu;
				case XR_BinaryButton.gripButtonName:
					return WVR_InputId.WVR_InputId_Alias1_Grip;
				case XR_BinaryButton.primaryButtonName:
					return (isLeft ? WVR_InputId.WVR_InputId_Alias1_X : WVR_InputId.WVR_InputId_Alias1_A);
				case XR_BinaryButton.secondaryButtonName:
					return (isLeft ? WVR_InputId.WVR_InputId_Alias1_Y : WVR_InputId.WVR_InputId_Alias1_B);
				case XR_BinaryButton.triggerButtonName:
					return WVR_InputId.WVR_InputId_Alias1_Trigger;
				case XR_BinaryButton.primary2DAxisClickName:
				case XR_BinaryButton.primary2DAxisTouchName:
					return WVR_InputId.WVR_InputId_Alias1_Thumbstick;
				default:
					break;
			}

			return WVR_InputId.WVR_InputId_Max;
		}

		public static InputDeviceCharacteristics InputDevice(this XR_Device device)
		{
			if (device == XR_Device.Right) { return InputDeviceControl.kControllerRightCharacteristics; }
			if (device == XR_Device.Left) { return InputDeviceControl.kControllerLeftCharacteristics; }
			return InputDeviceControl.kHMDCharacteristics;
		}
	}
}
