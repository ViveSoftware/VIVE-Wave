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
using Wave.Native;
using Wave.OpenXR;

namespace Wave.Essence.Tracker
{
	public enum TrackerId
	{
		Tracker0 = WVR_TrackerId.WVR_TrackerId_0,
		Tracker1 = WVR_TrackerId.WVR_TrackerId_1,
		Tracker2 = WVR_TrackerId.WVR_TrackerId_2,
		Tracker3 = WVR_TrackerId.WVR_TrackerId_3,
		Tracker4 = WVR_TrackerId.WVR_TrackerId_4,
		Tracker5 = WVR_TrackerId.WVR_TrackerId_5,
		Tracker6 = WVR_TrackerId.WVR_TrackerId_6,
		Tracker7 = WVR_TrackerId.WVR_TrackerId_7,
		Tracker8 = WVR_TrackerId.WVR_TrackerId_8,
		Tracker9 = WVR_TrackerId.WVR_TrackerId_9,
		Tracker10 = WVR_TrackerId.WVR_TrackerId_10,
		Tracker11 = WVR_TrackerId.WVR_TrackerId_11,
		Tracker12 = WVR_TrackerId.WVR_TrackerId_12,
		Tracker13 = WVR_TrackerId.WVR_TrackerId_13,
		Tracker14 = WVR_TrackerId.WVR_TrackerId_14,
		Tracker15 = WVR_TrackerId.WVR_TrackerId_15,
	}

	public enum TrackerRole
	{
		Undefined   = WVR_TrackerRole.WVR_TrackerRole_Undefined,
		Standalone  = WVR_TrackerRole.WVR_TrackerRole_Standalone,
		Pair1_Right = WVR_TrackerRole.WVR_TrackerRole_Pair1_Right,
		Pair1_Left  = WVR_TrackerRole.WVR_TrackerRole_Pair1_Left,

		Shoulder_Right = WVR_TrackerRole.WVR_TrackerRole_Shoulder_Right, // 32
		Upper_Arm_Right = WVR_TrackerRole.WVR_TrackerRole_Upper_Arm_Right, // 33
		Elbow_Right = WVR_TrackerRole.WVR_TrackerRole_Elbow_Right, // 34
		Forearm_Right   = WVR_TrackerRole.WVR_TrackerRole_Forearm_Right, // 35
		Wrist_Right     = WVR_TrackerRole.WVR_TrackerRole_Wrist_Right, // 36
		Hand_Right = WVR_TrackerRole.WVR_TrackerRole_Hand_Right, // 37
		Thigh_Right     = WVR_TrackerRole.WVR_TrackerRole_Thigh_Right, // 38
		Knee_Right = WVR_TrackerRole.WVR_TrackerRole_Knee_Right, // 39
		Calf_Right      = WVR_TrackerRole.WVR_TrackerRole_Calf_Right, // 40
		Ankle_Right     = WVR_TrackerRole.WVR_TrackerRole_Ankle_Right, // 41
		Foot_Right = WVR_TrackerRole.WVR_TrackerRole_Foot_Right, // 42

		Shoulder_Left = WVR_TrackerRole.WVR_TrackerRole_Shoulder_Left, // 47
		Upper_Arm_Left  = WVR_TrackerRole.WVR_TrackerRole_Upper_Arm_Left, // 48
		Elbow_Left = WVR_TrackerRole.WVR_TrackerRole_Elbow_Left, // 49
		Forearm_Left    = WVR_TrackerRole.WVR_TrackerRole_Forearm_Left, // 50
		Wrist_Left      = WVR_TrackerRole.WVR_TrackerRole_Wrist_Left, // 51
		Hand_Left = WVR_TrackerRole.WVR_TrackerRole_Hand_Left, // 52
		Thigh_Left      = WVR_TrackerRole.WVR_TrackerRole_Thigh_Left, // 53
		Knee_Left = WVR_TrackerRole.WVR_TrackerRole_Knee_Left, // 54
		Calf_Left       = WVR_TrackerRole.WVR_TrackerRole_Calf_Left, // 55
		Ankle_Left      = WVR_TrackerRole.WVR_TrackerRole_Ankle_Left, // 56
		Foot_Left = WVR_TrackerRole.WVR_TrackerRole_Foot_Left, // 57

		Chest = WVR_TrackerRole.WVR_TrackerRole_Chest, // 62
		Waist = WVR_TrackerRole.WVR_TrackerRole_Waist, // 63

		Camera = WVR_TrackerRole.WVR_TrackerRole_Camera, // 71
		Keyboard = WVR_TrackerRole.WVR_TrackerRole_Keyboard, // 72
	}

	public enum TrackerButton
	{
		System = WVR_InputId.WVR_InputId_0,
		Menu = WVR_InputId.WVR_InputId_Alias1_Menu,
		A = WVR_InputId.WVR_InputId_Alias1_A,
		B = WVR_InputId.WVR_InputId_Alias1_B,
		X = WVR_InputId.WVR_InputId_Alias1_X,
		Y = WVR_InputId.WVR_InputId_Alias1_Y,
		Touchpad = WVR_InputId.WVR_InputId_Alias1_Touchpad,
		Trigger = WVR_InputId.WVR_InputId_Alias1_Trigger,
	}

	public enum AxisType
	{
		None = WVR_AnalogType.WVR_AnalogType_None,
		XY = WVR_AnalogType.WVR_AnalogType_2D,
		XOnly = WVR_AnalogType.WVR_AnalogType_1D,
	}

	public static class TrackerUtils
	{
		public static TrackerId[] s_TrackerIds = new TrackerId[]
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

		public static TrackerId Id(this WVR_TrackerId trackerId)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_0) { return TrackerId.Tracker0; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1) { return TrackerId.Tracker1; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_2) { return TrackerId.Tracker2; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_3) { return TrackerId.Tracker3; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_4) { return TrackerId.Tracker4; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_5) { return TrackerId.Tracker5; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_6) { return TrackerId.Tracker6; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_7) { return TrackerId.Tracker7; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_8) { return TrackerId.Tracker8; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_9) { return TrackerId.Tracker9; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_10) { return TrackerId.Tracker10; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_11) { return TrackerId.Tracker11; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_12) { return TrackerId.Tracker12; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_13) { return TrackerId.Tracker13; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_14) { return TrackerId.Tracker14; }
			if (trackerId == WVR_TrackerId.WVR_TrackerId_15) { return TrackerId.Tracker15; }

			return TrackerId.Tracker0;
		}
		public static WVR_TrackerId Id(this TrackerId trackerId)
		{
			return (WVR_TrackerId)trackerId;
		}

		public static string Name(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return "Tracker0"; }
			if (trackerId == TrackerId.Tracker1) { return "Tracker1"; }
			if (trackerId == TrackerId.Tracker2) { return "Tracker2"; }
			if (trackerId == TrackerId.Tracker3) { return "Tracker3"; }
			if (trackerId == TrackerId.Tracker4) { return "Tracker4"; }
			if (trackerId == TrackerId.Tracker5) { return "Tracker5"; }
			if (trackerId == TrackerId.Tracker6) { return "Tracker6"; }
			if (trackerId == TrackerId.Tracker7) { return "Tracker7"; }
			if (trackerId == TrackerId.Tracker8) { return "Tracker8"; }
			if (trackerId == TrackerId.Tracker9) { return "Tracker9"; }
			if (trackerId == TrackerId.Tracker10) { return "Tracker10"; }
			if (trackerId == TrackerId.Tracker11) { return "Tracker11"; }
			if (trackerId == TrackerId.Tracker12) { return "Tracker12"; }
			if (trackerId == TrackerId.Tracker13) { return "Tracker13"; }
			if (trackerId == TrackerId.Tracker14) { return "Tracker14"; }
			if (trackerId == TrackerId.Tracker15) { return "Tracker15"; }

			return "";
		}
		public static int Num(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return 0; }
			if (trackerId == TrackerId.Tracker1) { return 1; }
			if (trackerId == TrackerId.Tracker2) { return 2; }
			if (trackerId == TrackerId.Tracker3) { return 3; }
			if (trackerId == TrackerId.Tracker4) { return 4; }
			if (trackerId == TrackerId.Tracker5) { return 5; }
			if (trackerId == TrackerId.Tracker6) { return 6; }
			if (trackerId == TrackerId.Tracker7) { return 7; }
			if (trackerId == TrackerId.Tracker8) { return 8; }
			if (trackerId == TrackerId.Tracker9) { return 9; }
			if (trackerId == TrackerId.Tracker10) { return 10; }
			if (trackerId == TrackerId.Tracker11) { return 11; }
			if (trackerId == TrackerId.Tracker12) { return 12; }
			if (trackerId == TrackerId.Tracker13) { return 13; }
			if (trackerId == TrackerId.Tracker14) { return 14; }
			if (trackerId == TrackerId.Tracker15) { return 15; }

			return 0;
		}
		public static InputDeviceTracker.TrackerId InputDevice(this TrackerId id)
		{
			if (id == TrackerId.Tracker0) { return InputDeviceTracker.TrackerId.Tracker0; }
			if (id == TrackerId.Tracker1) { return InputDeviceTracker.TrackerId.Tracker1; }
			if (id == TrackerId.Tracker2) { return InputDeviceTracker.TrackerId.Tracker2; }
			if (id == TrackerId.Tracker3) { return InputDeviceTracker.TrackerId.Tracker3; }
			if (id == TrackerId.Tracker4) { return InputDeviceTracker.TrackerId.Tracker4; }
			if (id == TrackerId.Tracker5) { return InputDeviceTracker.TrackerId.Tracker5; }
			if (id == TrackerId.Tracker6) { return InputDeviceTracker.TrackerId.Tracker6; }
			if (id == TrackerId.Tracker7) { return InputDeviceTracker.TrackerId.Tracker7; }
			if (id == TrackerId.Tracker8) { return InputDeviceTracker.TrackerId.Tracker8; }
			if (id == TrackerId.Tracker9) { return InputDeviceTracker.TrackerId.Tracker9; }
			if (id == TrackerId.Tracker10) { return InputDeviceTracker.TrackerId.Tracker10; }
			if (id == TrackerId.Tracker11) { return InputDeviceTracker.TrackerId.Tracker11; }
			if (id == TrackerId.Tracker12) { return InputDeviceTracker.TrackerId.Tracker12; }
			if (id == TrackerId.Tracker13) { return InputDeviceTracker.TrackerId.Tracker13; }
			if (id == TrackerId.Tracker14) { return InputDeviceTracker.TrackerId.Tracker14; }
			if (id == TrackerId.Tracker15) { return InputDeviceTracker.TrackerId.Tracker15; }

			return InputDeviceTracker.TrackerId.Tracker0;
		}

		public static string Name(this TrackerRole role)
		{
			if (role == TrackerRole.Ankle_Left) { return "Ankle_Left"; }
			if (role == TrackerRole.Ankle_Right) { return "Ankle_Right"; }
			if (role == TrackerRole.Calf_Left) { return "Calf_Left"; }
			if (role == TrackerRole.Calf_Right) { return "Calf_Right"; }
			if (role == TrackerRole.Camera) { return "Camera"; }
			if (role == TrackerRole.Chest) { return "Chest"; }
			if (role == TrackerRole.Elbow_Left) { return "Elbow_Left"; }
			if (role == TrackerRole.Elbow_Right) { return "Elbow_Right"; }
			if (role == TrackerRole.Foot_Left) { return "Foot_Left"; }
			if (role == TrackerRole.Foot_Right) { return "Foot_Right"; }
			if (role == TrackerRole.Forearm_Left) { return "Forearm_Left"; }
			if (role == TrackerRole.Forearm_Right) { return "Forearm_Right"; }
			if (role == TrackerRole.Hand_Left) { return "Hand_Left"; }
			if (role == TrackerRole.Hand_Right) { return "Hand_Right"; }
			if (role == TrackerRole.Keyboard) { return "Keyboard"; }
			if (role == TrackerRole.Knee_Left) { return "Knee_Left"; }
			if (role == TrackerRole.Knee_Right) { return "Knee_Right"; }
			if (role == TrackerRole.Pair1_Left) { return "Pair1_Left"; }
			if (role == TrackerRole.Pair1_Right) { return "Pair1_Right"; }
			if (role == TrackerRole.Shoulder_Left) { return "Shoulder_Left"; }
			if (role == TrackerRole.Shoulder_Right) { return "Shoulder_Right"; }
			if (role == TrackerRole.Standalone) { return "Standalone"; }
			if (role == TrackerRole.Thigh_Left) { return "Thigh_Left"; }
			if (role == TrackerRole.Thigh_Right) { return "Thigh_Right"; }
			if (role == TrackerRole.Undefined) { return "Undefined"; }
			if (role == TrackerRole.Upper_Arm_Left) { return "Upper_Arm_Left"; }
			if (role == TrackerRole.Upper_Arm_Right) { return "Upper_Arm_Right"; }
			if (role == TrackerRole.Waist) { return "Waist"; }
			if (role == TrackerRole.Wrist_Left) { return "Wrist_Left"; }
			if (role == TrackerRole.Wrist_Right) { return "Wrist_Right"; }

			return "";
		}
		public static InputDeviceTracker.TrackerRole InputDevice(this TrackerRole role)
		{
			if (role == TrackerRole.Standalone) { return InputDeviceTracker.TrackerRole.Standalone; }
			if (role == TrackerRole.Pair1_Left) { return InputDeviceTracker.TrackerRole.Pair1_Left; }
			if (role == TrackerRole.Pair1_Right) { return InputDeviceTracker.TrackerRole.Pair1_Right; }

			if (role == TrackerRole.Shoulder_Right) { return InputDeviceTracker.TrackerRole.Shoulder_Right; } // 32
			if (role == TrackerRole.Upper_Arm_Right) { return InputDeviceTracker.TrackerRole.Upper_Arm_Right; } // 33
			if (role == TrackerRole.Elbow_Right) { return InputDeviceTracker.TrackerRole.Elbow_Right; } // 34
			if (role == TrackerRole.Forearm_Right) { return InputDeviceTracker.TrackerRole.Forearm_Right; } // 35
			if (role == TrackerRole.Wrist_Right) { return InputDeviceTracker.TrackerRole.Wrist_Right; } // 36
			if (role == TrackerRole.Hand_Right) { return InputDeviceTracker.TrackerRole.Hand_Right; } // 37
			if (role == TrackerRole.Thigh_Right) { return InputDeviceTracker.TrackerRole.Thigh_Right; } // 38
			if (role == TrackerRole.Knee_Right) { return InputDeviceTracker.TrackerRole.Knee_Right; } // 39
			if (role == TrackerRole.Calf_Right) { return InputDeviceTracker.TrackerRole.Calf_Right; } // 40
			if (role == TrackerRole.Ankle_Right) { return InputDeviceTracker.TrackerRole.Ankle_Right; } // 41
			if (role == TrackerRole.Foot_Right) { return InputDeviceTracker.TrackerRole.Foot_Right; } // 42

			if (role == TrackerRole.Shoulder_Left) { return InputDeviceTracker.TrackerRole.Shoulder_Left; } // 47
			if (role == TrackerRole.Upper_Arm_Left) { return InputDeviceTracker.TrackerRole.Upper_Arm_Left; } // 48
			if (role == TrackerRole.Elbow_Left) { return InputDeviceTracker.TrackerRole.Elbow_Left; } // 49
			if (role == TrackerRole.Forearm_Left) { return InputDeviceTracker.TrackerRole.Forearm_Left; } // 50
			if (role == TrackerRole.Wrist_Left) { return InputDeviceTracker.TrackerRole.Wrist_Left; } // 51
			if (role == TrackerRole.Hand_Left) { return InputDeviceTracker.TrackerRole.Hand_Left; } // 52
			if (role == TrackerRole.Thigh_Left) { return InputDeviceTracker.TrackerRole.Thigh_Left; } // 53
			if (role == TrackerRole.Knee_Left) { return InputDeviceTracker.TrackerRole.Knee_Left; } // 54
			if (role == TrackerRole.Calf_Left) { return InputDeviceTracker.TrackerRole.Calf_Left; } // 55
			if (role == TrackerRole.Ankle_Left) { return InputDeviceTracker.TrackerRole.Ankle_Left; } // 56
			if (role == TrackerRole.Foot_Left) { return InputDeviceTracker.TrackerRole.Foot_Left; } // 57

			if (role == TrackerRole.Chest) { return InputDeviceTracker.TrackerRole.Chest; } // 62
			if (role == TrackerRole.Waist) { return InputDeviceTracker.TrackerRole.Waist; } // 63

			if (role == TrackerRole.Camera) { return InputDeviceTracker.TrackerRole.Camera; } // 71
			if (role == TrackerRole.Keyboard) { return InputDeviceTracker.TrackerRole.Keyboard; } // 72

			return InputDeviceTracker.TrackerRole.Undefined;
		}
		public static TrackerRole Role(this InputDeviceTracker.TrackerRole role)
		{
			if (role == InputDeviceTracker.TrackerRole.Standalone) { return TrackerRole.Standalone; }
			if (role == InputDeviceTracker.TrackerRole.Pair1_Left) { return TrackerRole.Pair1_Left; }
			if (role == InputDeviceTracker.TrackerRole.Pair1_Right) { return TrackerRole.Pair1_Right; }

			if (role == InputDeviceTracker.TrackerRole.Shoulder_Right) { return TrackerRole.Shoulder_Right; } // 32
			if (role == InputDeviceTracker.TrackerRole.Upper_Arm_Right) { return TrackerRole.Upper_Arm_Right; } // 33
			if (role == InputDeviceTracker.TrackerRole.Elbow_Right) { return TrackerRole.Elbow_Right; } // 34
			if (role == InputDeviceTracker.TrackerRole.Forearm_Right) { return TrackerRole.Forearm_Right; } // 35
			if (role == InputDeviceTracker.TrackerRole.Wrist_Right) { return TrackerRole.Wrist_Right; } // 36
			if (role == InputDeviceTracker.TrackerRole.Hand_Right) { return TrackerRole.Hand_Right; } // 37
			if (role == InputDeviceTracker.TrackerRole.Thigh_Right) { return TrackerRole.Thigh_Right; } // 38
			if (role == InputDeviceTracker.TrackerRole.Knee_Right) { return TrackerRole.Knee_Right; } // 39
			if (role == InputDeviceTracker.TrackerRole.Calf_Right) { return TrackerRole.Calf_Right; } // 40
			if (role == InputDeviceTracker.TrackerRole.Ankle_Right) { return TrackerRole.Ankle_Right; } // 41
			if (role == InputDeviceTracker.TrackerRole.Foot_Right) { return TrackerRole.Foot_Right; } // 42

			if (role == InputDeviceTracker.TrackerRole.Shoulder_Left) { return TrackerRole.Shoulder_Left; } // 47
			if (role == InputDeviceTracker.TrackerRole.Upper_Arm_Left) { return TrackerRole.Upper_Arm_Left; } // 48
			if (role == InputDeviceTracker.TrackerRole.Elbow_Left) { return TrackerRole.Elbow_Left; } // 49
			if (role == InputDeviceTracker.TrackerRole.Forearm_Left) { return TrackerRole.Forearm_Left; } // 50
			if (role == InputDeviceTracker.TrackerRole.Wrist_Left) { return TrackerRole.Wrist_Left; } // 51
			if (role == InputDeviceTracker.TrackerRole.Hand_Left) { return TrackerRole.Hand_Left; } // 52
			if (role == InputDeviceTracker.TrackerRole.Thigh_Left) { return TrackerRole.Thigh_Left; } // 53
			if (role == InputDeviceTracker.TrackerRole.Knee_Left) { return TrackerRole.Knee_Left; } // 54
			if (role == InputDeviceTracker.TrackerRole.Calf_Left) { return TrackerRole.Calf_Left; } // 55
			if (role == InputDeviceTracker.TrackerRole.Ankle_Left) { return TrackerRole.Ankle_Left; } // 56
			if (role == InputDeviceTracker.TrackerRole.Foot_Left) { return TrackerRole.Foot_Left; } // 57

			if (role == InputDeviceTracker.TrackerRole.Chest) { return TrackerRole.Chest; } // 62
			if (role == InputDeviceTracker.TrackerRole.Waist) { return TrackerRole.Waist; } // 63

			if (role == InputDeviceTracker.TrackerRole.Camera) { return TrackerRole.Camera; } // 71
			if (role == InputDeviceTracker.TrackerRole.Keyboard) { return TrackerRole.Keyboard; } // 72

			return TrackerRole.Undefined;
		}

		public static int Num(this TrackerButton button)
		{
			if (button == TrackerButton.System) { return 0; }
			if (button == TrackerButton.Menu) { return 1; }
			if (button == TrackerButton.A) { return 10; }
			if (button == TrackerButton.B) { return 11; }
			if (button == TrackerButton.X) { return 12; }
			if (button == TrackerButton.Y) { return 13; }
			if (button == TrackerButton.Touchpad) { return 16; }
			if (button == TrackerButton.Trigger) { return 17; }

			return 31;
		}
		public static WVR_InputId Id(this TrackerButton button)
		{
			if (button == TrackerButton.System) { return WVR_InputId.WVR_InputId_Alias1_System; }
			if (button == TrackerButton.Menu) { return WVR_InputId.WVR_InputId_Alias1_Menu; }
			if (button == TrackerButton.A) { return WVR_InputId.WVR_InputId_Alias1_A; }
			if (button == TrackerButton.B) { return WVR_InputId.WVR_InputId_Alias1_B; }
			if (button == TrackerButton.X) { return WVR_InputId.WVR_InputId_Alias1_X; }
			if (button == TrackerButton.Y) { return WVR_InputId.WVR_InputId_Alias1_Y; }
			if (button == TrackerButton.Touchpad) { return WVR_InputId.WVR_InputId_Alias1_Touchpad; }
			if (button == TrackerButton.Trigger) { return WVR_InputId.WVR_InputId_Alias1_Trigger; }

			return WVR_InputId.WVR_InputId_Alias1_System;
		}
		public static InputFeatureUsage<bool> Usage(this TrackerButton button, WVR_InputType inputType)
		{
			if (inputType == WVR_InputType.WVR_InputType_Button)
			{
				if (button == TrackerButton.Menu || button == TrackerButton.System) { return XR_Feature.menuButton; }
				if (button == TrackerButton.A || button == TrackerButton.X) { return XR_Feature.primaryButton; }
				if (button == TrackerButton.B || button == TrackerButton.Y) { return XR_Feature.secondaryButton; }
				if (button == TrackerButton.Touchpad) { return XR_Feature.primary2DAxisClick; }
				if (button == TrackerButton.Trigger) { return XR_Feature.triggerButton; }
			}
			if (inputType == WVR_InputType.WVR_InputType_Touch)
			{
				if (button == TrackerButton.A || button == TrackerButton.X) { return XR_Feature.primaryTouch; }
				if (button == TrackerButton.B || button == TrackerButton.Y) { return XR_Feature.secondaryTouch; }
				if (button == TrackerButton.Touchpad) { return XR_Feature.primary2DAxisTouch; }
				if (button == TrackerButton.Trigger) { return XR_Feature.triggerTouch; }
			}

			return new InputFeatureUsage<bool>("");
		}

		#region Native
		public static bool ValidWVRInputId(this uint id)
		{
			if (id >= (uint)WVR_InputId.WVR_InputId_0 && id <= (uint)WVR_InputId.WVR_InputId_19)
				return true;

			return false;
		}
		public static int ArrayIndex(this WVR_InputId id)
		{
			if (id == WVR_InputId.WVR_InputId_Max) { return 0; } // prevent overflow
			return (int)id;
		}

		public static AxisType Id(this WVR_AnalogType analog)
		{
			if (analog == WVR_AnalogType.WVR_AnalogType_None) { return AxisType.None; }
			if (analog == WVR_AnalogType.WVR_AnalogType_2D) { return AxisType.XY; }
			if (analog == WVR_AnalogType.WVR_AnalogType_1D) { return AxisType.XOnly; }

			return AxisType.None;
		}

		public static TrackerRole Id(this WVR_TrackerRole role)
		{
			if (role == WVR_TrackerRole.WVR_TrackerRole_Standalone) { return TrackerRole.Standalone; }
			if (role == WVR_TrackerRole.WVR_TrackerRole_Pair1_Right) { return TrackerRole.Pair1_Right; }
			if (role == WVR_TrackerRole.WVR_TrackerRole_Pair1_Left) { return TrackerRole.Pair1_Left; }

			if (role == WVR_TrackerRole.WVR_TrackerRole_Shoulder_Right) { return TrackerRole.Shoulder_Right; } // 32
			if (role == WVR_TrackerRole.WVR_TrackerRole_Upper_Arm_Right) { return TrackerRole.Upper_Arm_Right; } // 33
			if (role == WVR_TrackerRole.WVR_TrackerRole_Elbow_Right) { return TrackerRole.Elbow_Right; } // 34
			if (role == WVR_TrackerRole.WVR_TrackerRole_Forearm_Right) { return TrackerRole.Forearm_Right; } // 35
			if (role == WVR_TrackerRole.WVR_TrackerRole_Wrist_Right) { return TrackerRole.Wrist_Right; } // 36
			if (role == WVR_TrackerRole.WVR_TrackerRole_Hand_Right) { return TrackerRole.Hand_Right; } // 37
			if (role == WVR_TrackerRole.WVR_TrackerRole_Thigh_Right) { return TrackerRole.Thigh_Right; } // 38
			if (role == WVR_TrackerRole.WVR_TrackerRole_Knee_Right) { return TrackerRole.Knee_Right; } // 39
			if (role == WVR_TrackerRole.WVR_TrackerRole_Calf_Right) { return TrackerRole.Calf_Right; } // 40
			if (role == WVR_TrackerRole.WVR_TrackerRole_Ankle_Right) { return TrackerRole.Ankle_Right; } // 41
			if (role == WVR_TrackerRole.WVR_TrackerRole_Foot_Right) { return TrackerRole.Foot_Right; } // 42

			if (role == WVR_TrackerRole.WVR_TrackerRole_Shoulder_Left) { return TrackerRole.Shoulder_Left; } // 47
			if (role == WVR_TrackerRole.WVR_TrackerRole_Upper_Arm_Left) { return TrackerRole.Upper_Arm_Left; } // 48
			if (role == WVR_TrackerRole.WVR_TrackerRole_Elbow_Left) { return TrackerRole.Elbow_Left; } // 49
			if (role == WVR_TrackerRole.WVR_TrackerRole_Forearm_Left) { return TrackerRole.Forearm_Left; } // 50
			if (role == WVR_TrackerRole.WVR_TrackerRole_Wrist_Left) { return TrackerRole.Wrist_Left; } // 51
			if (role == WVR_TrackerRole.WVR_TrackerRole_Hand_Left) { return TrackerRole.Hand_Left; } // 52
			if (role == WVR_TrackerRole.WVR_TrackerRole_Thigh_Left) { return TrackerRole.Thigh_Left; } // 53
			if (role == WVR_TrackerRole.WVR_TrackerRole_Knee_Left) { return TrackerRole.Knee_Left; } // 54
			if (role == WVR_TrackerRole.WVR_TrackerRole_Calf_Left) { return TrackerRole.Calf_Left; } // 55
			if (role == WVR_TrackerRole.WVR_TrackerRole_Ankle_Left) { return TrackerRole.Ankle_Left; } // 56
			if (role == WVR_TrackerRole.WVR_TrackerRole_Foot_Left) { return TrackerRole.Foot_Left; } // 57

			if (role == WVR_TrackerRole.WVR_TrackerRole_Chest) { return TrackerRole.Chest; } // 62
			if (role == WVR_TrackerRole.WVR_TrackerRole_Waist) { return TrackerRole.Waist; } // 63

			if (role == WVR_TrackerRole.WVR_TrackerRole_Camera) { return TrackerRole.Camera; } // 71
			if (role == WVR_TrackerRole.WVR_TrackerRole_Keyboard) { return TrackerRole.Keyboard; } // 72

			return TrackerRole.Undefined;
		}
		#endregion
	}
}
