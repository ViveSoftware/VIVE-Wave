// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace Wave.Native
{
#if UNITY_EDITOR
	public class WVR_EditorImpl : MonoBehaviour
	{
		private const string LOG_TAG = "WVR_EditorImpl";
		private void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		private void ERROR(string msg) { Log.e(LOG_TAG, msg, true); }

		public static WVR_EditorImpl Instance
		{
			get
			{
				if (instance == null)
				{
					if (instance == null)
					{
						var gameObject = new GameObject("WVR_EditorImpl");
						instance = gameObject.AddComponent<WVR_EditorImpl>();
						// This object should survive all scene transitions.
						GameObject.DontDestroyOnLoad(instance);
					}
				}
				return instance;
			}
		}
		private static WVR_EditorImpl instance = null;

		#region Variables
		private WVR_Event_t mEvent = new WVR_Event_t();
		private bool hasEvent = false;

		private const string MOUSE_X = "Mouse X";
		private const string MOUSE_Y = "Mouse Y";
		private const string MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";

		// =================== Button Events ===============================
		uint state_press_hmd = 0;
		uint state_press_right = 0;
		uint state_press_left = 0;

		uint state_touch_right = 0;
		private WVR_Axis_t[] state_axis_right = new WVR_Axis_t[(int)WVR_InputId.WVR_InputId_Max];
		uint state_touch_left = 0;
		private WVR_Axis_t[] state_axis_left = new WVR_Axis_t[(int)WVR_InputId.WVR_InputId_Max];
		private const float kAxisX = .6f, kAxisY = -.3f;

		private bool mFocusIsCapturedBySystem = false;
		private float mFPS = 60.0f;
		#endregion

		#region com.unity.inputsystem
#if ENABLE_INPUT_SYSTEM
		WVR_Input wvrInput = null;
		private void CreateWVRInput()
		{
			if (wvrInput == null)
			{
				wvrInput = new WVR_Input();
				DEBUG("CreateWVRInput() WVR_Input is created.");
			}
		}
#endif
		#endregion

		#region Monobehaviour overrides
		private void Awake()
		{
#if ENABLE_INPUT_SYSTEM
			CreateWVRInput();
#endif
		}

		void OnEnable()
		{
			DEBUG("OnEnable()");

			for (uint i = 0; i < state_axis_right.Length; i++)
			{
				state_axis_right[i].x = 0;
				state_axis_right[i].y = 0;
			}
			for (uint i = 0; i < state_axis_left.Length; i++)
			{
				state_axis_left[i].x = 0;
				state_axis_left[i].y = 0;
			}

			InitializeBonesAndHandTrackingData();
			InitHandGesture();
			InitTracker();

#if ENABLE_INPUT_SYSTEM
			wvrInput.Enable();
#endif
		}
		private void OnDisable()
		{
#if ENABLE_INPUT_SYSTEM
			wvrInput.Disable();
#endif
		}
		void Start()
		{
			DEBUG("Start()");
			Cursor.visible = false;
		}

		private float xAxis = 0, yAxis = 0, zAxis = 0;
		private float xOffset = 0, yOffset = 0, zOffset = 0;
#if ENABLE_INPUT_SYSTEM
		private Vector2 mouseAxis = Vector2.zero, mouseAxisEx = Vector2.zero;
		private float axisProportion = .1f;
#endif
		void Update()
		{
			mFPS = 1.0f / Time.deltaTime;

			ButtonPressed(WVR_DeviceType.WVR_DeviceType_HMD);
			ButtonUnpressed(WVR_DeviceType.WVR_DeviceType_HMD);
			ButtonPressed(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			ButtonUnpressed(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			ButtonPressed(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			ButtonUnpressed(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			TouchTapped(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			TouchUntapped(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			TouchTapped(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			TouchUntapped(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			SideToSideSwipe();

#if ENABLE_LEGACY_INPUT_MANAGER
			xAxis = Input.GetAxis(MOUSE_X);
			yAxis = Input.GetAxis(MOUSE_Y);
			zAxis = Input.GetAxis(MOUSE_SCROLLWHEEL);
#elif ENABLE_INPUT_SYSTEM
			mouseAxis = wvrInput.mouse.axis.ReadValue<Vector2>() * axisProportion;
			xAxis = -(mouseAxisEx.x - mouseAxis.x);
			yAxis = -(mouseAxisEx.y - mouseAxis.y);
			mouseAxisEx = mouseAxis;
#endif
			float axis_x = xAxis + xOffset;
			float axis_y = yAxis + yOffset;
			float axis_z = zAxis + zOffset;
			UpdateHeadPose(axis_x, axis_y, axis_z);
			UpdateRightPose(axis_x, axis_y, axis_z);
			UpdateLefHandPose(axis_x, axis_y, axis_z);
			SetDevicePosePairHead();
			SetDevicePosePairRight();
			SetDevicePosePairLeft();

			// Eye Tracking
			UpdateGazePoint();

			// Hand
			UpdateBonesAndHandTrackingData();
			UpdateHandGesture();

			// Tracker
			UpdateTracker();
			BraceletPressed(WVR_TrackerId.WVR_TrackerId_0);
			BraceletUnpressed(WVR_TrackerId.WVR_TrackerId_0);
			BraceletPressed(WVR_TrackerId.WVR_TrackerId_1);
			BraceletUnpressed(WVR_TrackerId.WVR_TrackerId_1);
		}
		#endregion

		#region [Event] Polling
		private void clearEventQueue()
		{
			hasEvent = false;
			mEvent.device.type = WVR_DeviceType.WVR_DeviceType_Invalid;
			mEvent.input.inputId = WVR_InputId.WVR_InputId_Max;
		}

		public bool PollEventQueue(ref WVR_Event_t e)
		{
			// Get current state
			bool ret = hasEvent;
			e = mEvent;
			// Clear current state after poll queue.
			clearEventQueue();

			return ret;
		}
		#endregion

		#region [Event] Buttons
		private bool IsButtonAvailable(WVR_DeviceType device, WVR_InputId button)
		{
			if (device == WVR_DeviceType.WVR_DeviceType_HMD && inputTable_Hmd != null)
			{
				for (int i = 0; i < inputTable_Hmd.Length; i++)
				{
					if (inputTable_Hmd[i].destination.id == button)
						return true;
				}
			}
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right && inputTable_Right != null)
			{
				for (int i = 0; i < inputTable_Right.Length; i++)
				{
					if (inputTable_Right[i].destination.id == button)
						return true;
				}
			}
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left && inputTable_Left != null)
			{
				for (int i = 0; i < inputTable_Left.Length; i++)
				{
					if (inputTable_Left[i].destination.id == button)
						return true;
				}
			}
			return false;
		}
		public int GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
		{
			int count = 0;
			if (inputType == WVR_InputType.WVR_InputType_Button)
			{
				for (int id = 0; id < (int)WVR_InputId.WVR_InputId_Max; id++)
				{
					int input = 1 << id;
					if (type == WVR_DeviceType.WVR_DeviceType_HMD)
					{
						if ((state_press_hmd & input) == input)
							count++;
					}
					if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
					{
						if ((state_press_left & input) == input)
							count++;
					}
					if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
					{
						if ((state_press_right & input) == input)
							count++;
					}
				}
			}
			if (inputType == WVR_InputType.WVR_InputType_Touch || inputType == WVR_InputType.WVR_InputType_Analog)
			{
				for (int id = 0; id < (int)WVR_InputId.WVR_InputId_Max; id++)
				{
					int input = 1 << id;
					if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
					{
						if ((state_touch_left & input) == input)
							count++;
					}
					if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
					{
						if ((state_touch_right & input) == input)
							count++;
					}
				}
			}
			return count;
		}

		private void ButtonPressed(WVR_DeviceType type)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Enter))
					{
						if (WXRInput.GetKeyDown(KeyCode.E))
						{
							DEBUG("ButtonPressed() " + type + ", Enter.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Enter;
							hasEvent = true;

							state_press_hmd |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Enter;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Touchpad))
					{
						if (WXRInput.GetMouseButtonDown(1))   // right mouse key
						{
							DEBUG("ButtonPressed() " + type + ", Touchpad");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
							hasEvent = true;

							state_axis_right[(uint)mEvent.input.inputId].x = kAxisX;
							state_axis_right[(uint)mEvent.input.inputId].y = kAxisY;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
							state_touch_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Trigger))
					{
						if (WXRInput.GetKeyDown(KeyCode.T))
						{
							DEBUG("ButtonPressed() " + type + ", Trigger");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Trigger;
							hasEvent = true;

							state_axis_right[(uint)mEvent.input.inputId].x = kAxisX;
							state_axis_right[(uint)mEvent.input.inputId].y = kAxisY;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
							state_touch_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_A))
					{
						if (WXRInput.GetKeyDown(KeyCode.A))
						{
							DEBUG("ButtonPressed() " + type + ", A.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_A;
							hasEvent = true;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_A;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_B))
					{
						if (WXRInput.GetKeyDown(KeyCode.B))
						{
							DEBUG("ButtonPressed() " + type + ", B.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_B;
							hasEvent = true;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_B;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_X))
					{
						if (WXRInput.GetKeyDown(KeyCode.X))
						{
							DEBUG("ButtonPressed() " + type + ", X.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_X;
							hasEvent = true;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_X;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Y))
					{
						if (WXRInput.GetKeyDown(KeyCode.Y))
						{
							DEBUG("ButtonPressed() " + type + ", Y.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Y;
							hasEvent = true;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Y;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Menu))
					{
						if (WXRInput.GetKeyDown(KeyCode.M))
						{
							DEBUG("ButtonPressed() " + type + ", M.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Menu;
							hasEvent = true;

							state_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Menu;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Trigger))
					{
						if (WXRInput.GetKeyDown(KeyCode.R))
						{
							DEBUG("ButtonPressed() " + type + ", trigger.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonPressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Trigger;
							hasEvent = true;

							state_press_left |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
						}
					}
					break;
				default:
					break;
			}
		}
		private void ButtonUnpressed(WVR_DeviceType type)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Enter))
					{
						if (WXRInput.GetKeyUp(KeyCode.E))
						{
							DEBUG("ButtonUnpressed() " + type + ", Enter.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Enter;
							hasEvent = true;

							state_press_hmd ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Enter;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Touchpad))
					{
						if (WXRInput.GetMouseButtonUp(1))   // right mouse key
						{
							DEBUG("ButtonUnpressed() " + type + ", Touchpad.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
							state_touch_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Trigger))
					{
						if (WXRInput.GetKeyUp(KeyCode.T))
						{
							DEBUG("ButtonUnpressed() " + type + ", Trigger.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Trigger;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
							state_touch_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_A))
					{
						if (WXRInput.GetKeyUp(KeyCode.A))
						{
							DEBUG("ButtonUnpressed() " + type + ", A.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_A;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_A;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_B))
					{
						if (WXRInput.GetKeyUp(KeyCode.B))
						{
							DEBUG("ButtonUnpressed() " + type + ", B.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_B;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_B;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_X))
					{
						if (WXRInput.GetKeyUp(KeyCode.X))
						{
							DEBUG("ButtonUnpressed() " + type + ", X.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_X;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_X;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Y))
					{
						if (WXRInput.GetKeyUp(KeyCode.Y))
						{
							DEBUG("ButtonUnpressed() " + type + ", Y.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Y;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Y;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Menu))
					{
						if (WXRInput.GetKeyUp(KeyCode.M))
						{
							DEBUG("ButtonUnpressed() " + type + ", M.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Menu;
							hasEvent = true;

							state_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Menu;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Trigger))
					{
						if (WXRInput.GetKeyUp(KeyCode.R))
						{
							DEBUG("ButtonUnpressed() " + type + ", trigger.");
							mEvent.common.type = WVR_EventType.WVR_EventType_ButtonUnpressed;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Trigger;
							hasEvent = true;

							state_press_left ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger;
						}
					}
					break;
				default:
					break;
			}
		}
		public bool GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
		{
			bool pressed = false;
			int input = 1 << (int)id;
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					pressed = ((state_press_hmd & input) == input);
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					pressed = ((state_press_right & input) == input);
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					pressed = ((state_press_left & input) == input);
					break;
				default:
					break;
			}
			return pressed;
		}

		private void TouchTapped(WVR_DeviceType type)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Parking))
					{
						if (WXRInput.GetKeyDown(KeyCode.P))
						{
							DEBUG("TouchTapped() " + type + ", Parking.");
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchTapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Parking;
							hasEvent = true;

							state_touch_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Parking;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Grip))
					{
						if (WXRInput.GetKeyDown(KeyCode.G))
						{
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchTapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Grip;
							hasEvent = true;

							state_axis_right[(uint)mEvent.input.inputId].x = kAxisX;
							state_axis_right[(uint)mEvent.input.inputId].y = kAxisY;

							state_touch_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Grip;
							DEBUG("TouchTapped() " + type + ", grip" + ", axis (" + state_axis_right[(uint)mEvent.input.inputId].x + ", " + state_axis_right[(uint)mEvent.input.inputId].y + ")");
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Touchpad))
					{
						if (WXRInput.GetMouseButtonDown(0))   // left mouse key
						{
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchTapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
							hasEvent = true;

							state_axis_left[(uint)mEvent.input.inputId].x = kAxisX;
							state_axis_left[(uint)mEvent.input.inputId].y = kAxisY;

							state_touch_left |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
							DEBUG("TouchTapped() " + type + ", Touchpad" + ", axis (" + state_axis_left[(uint)mEvent.input.inputId].x + ", " + state_axis_left[(uint)mEvent.input.inputId].y + ")");
						}
					}

					/*{
						if (WXRInput.GetKeyDown(KeyCode.W))
						{
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchTapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Bumper;
							hasEvent = true;

							state_axis_left[(uint)mEvent.input.inputId].x = kAxisX;
							state_axis_left[(uint)mEvent.input.inputId].y = kAxisY;

							state_touch_left |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Bumper;
							DEBUG("TouchTapped() " + type + ", Bumper" + ", axis (" + state_axis_left[(uint)mEvent.input.inputId].x + ", " + state_axis_left[(uint)mEvent.input.inputId].y + ")");
						}
					}*/
					break;
				default:
					break;
			}
		}
		private void TouchUntapped(WVR_DeviceType type)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Parking))
					{
						if (WXRInput.GetKeyUp(KeyCode.P))
						{
							DEBUG("TouchUntapped() " + type + ", Parking.");
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchUntapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Parking;
							hasEvent = true;

							state_touch_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Parking;
						}
					}
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Grip))
					{
						if (WXRInput.GetKeyUp(KeyCode.G))
						{
							DEBUG("TouchUntapped() " + type + ", Grip.");
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchUntapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Grip;
							hasEvent = true;

							state_axis_right[(uint)mEvent.input.inputId].x = 0;
							state_axis_right[(uint)mEvent.input.inputId].y = 0;

							state_touch_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Grip;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if (IsButtonAvailable(type, WVR_InputId.WVR_InputId_Alias1_Touchpad))
					{
						if (WXRInput.GetMouseButtonUp(0))   // left  mouse key
						{
							DEBUG("TouchUntapped() " + type + ", Touchpad.");
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchUntapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
							hasEvent = true;

							state_axis_left[(uint)mEvent.input.inputId].x = 0;
							state_axis_left[(uint)mEvent.input.inputId].y = 0;

							state_touch_left ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Touchpad;
						}
					}

					/*{
						if (WXRInput.GetKeyUp(KeyCode.W))   // left  mouse key
						{
							DEBUG("TouchUntapped() " + type + ", Bumper.");
							mEvent.common.type = WVR_EventType.WVR_EventType_TouchUntapped;
							mEvent.device.type = type;
							mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Bumper;
							hasEvent = true;

							state_axis_left[(uint)mEvent.input.inputId].x = 0;
							state_axis_left[(uint)mEvent.input.inputId].y = 0;

							state_touch_left ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_Bumper;
						}
					}*/
					break;
				default:
					break;
			}
		}
		public bool GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
		{
			bool touched = false;
			int input = 1 << (int)id;
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					touched = ((state_touch_right & input) == input);
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					touched = ((state_touch_left & input) == input);
					break;
				default:
					break;
			}
			return touched;
		}
		public WVR_Axis_t GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
		{
			WVR_Axis_t axis2d;
			axis2d.x = 0;
			axis2d.y = 0;

			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					axis2d = state_axis_left[(uint)id];
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					axis2d = state_axis_right[(uint)id];
					break;
				default:
					break;
			}

			return axis2d;
		}

		public bool GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
				[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					if ((inputMask & (uint)WVR_InputType.WVR_InputType_Button) != 0)
						buttons = state_press_hmd;
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if ((inputMask & (uint)WVR_InputType.WVR_InputType_Button) != 0)
					{
						buttons = state_press_right;
					}
					if ((inputMask & (uint)WVR_InputType.WVR_InputType_Touch) != 0)
					{
						touches = state_touch_right;
					}
					if (((inputMask & (uint)WVR_InputType.WVR_InputType_Analog) != 0) && analogArrayCount > 0)
					{
						if (analogArray == null || analogArray.Length != analogArrayCount)
							analogArray = new WVR_AnalogState_t[analogArrayCount];

						int analogArrayIndex = 0;
						for (uint id = 0; id < (uint)WVR_InputId.WVR_InputId_Max; id++)
						{
							int input = 1 << (int)id;
							if ((state_touch_right & input) == input)
							{
								analogArray[analogArrayIndex].id = (WVR_InputId)id;
								analogArray[analogArrayIndex].type = WVR_AnalogType.WVR_AnalogType_2D;
								analogArray[analogArrayIndex].axis = state_axis_right[id];
								analogArrayIndex++;
							}
							if (analogArrayIndex >= analogArray.Length)
								break;
						}
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if ((inputMask & (uint)WVR_InputType.WVR_InputType_Button) != 0)
					{
						buttons = state_press_left;
					}
					if ((inputMask & (uint)WVR_InputType.WVR_InputType_Touch) != 0)
					{
						touches = state_touch_left;
					}
					if (((inputMask & (uint)WVR_InputType.WVR_InputType_Analog) != 0) && analogArrayCount > 0)
					{
						if (analogArray.Length != analogArrayCount)
							analogArray = new WVR_AnalogState_t[analogArrayCount];

						int analogArrayIndex = 0;
						for (uint id = 0; id < (uint)WVR_InputId.WVR_InputId_Max; id++)
						{
							int input = 1 << (int)id;
							if ((state_touch_left & input) == input)
							{
								analogArray[analogArrayIndex].id = (WVR_InputId)id;
								analogArray[analogArrayIndex].type = WVR_AnalogType.WVR_AnalogType_2D;
								analogArray[analogArrayIndex].axis = state_axis_left[id];
								analogArrayIndex++;
							}
							if (analogArrayIndex >= analogArray.Length)
								break;
						}
					}
					break;
				default:
					return false;
			}
			return true;
		}
		#endregion

		#region [Event] Swipe
		private void SideToSideSwipe()
		{
			if (WXRInput.GetKeyUp(KeyCode.H))   // Down To Up Swipe
			{
				DEBUG("SideToSideSwipe() WVR_EventType_DownToUpSwipe.");
				mEvent.common.type = WVR_EventType.WVR_EventType_DownToUpSwipe;
				mEvent.device.type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
				mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
				hasEvent = true;
			}
			if (WXRInput.GetKeyUp(KeyCode.J))   // Right To Left Swipe
			{
				DEBUG("SideToSideSwipe() WVR_EventType_RightToLeftSwipe.");
				mEvent.common.type = WVR_EventType.WVR_EventType_RightToLeftSwipe;
				mEvent.device.type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
				mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
				hasEvent = true;
			}
			if (WXRInput.GetKeyUp(KeyCode.K))   // Up To Down Swipe
			{
				DEBUG("SideToSideSwipe() WVR_EventType_UpToDownSwipe.");
				mEvent.common.type = WVR_EventType.WVR_EventType_UpToDownSwipe;
				mEvent.device.type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
				mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
				hasEvent = true;
			}
			if (WXRInput.GetKeyUp(KeyCode.L))   // Left To Right Swipe
			{
				DEBUG("SideToSideSwipe() WVR_EventType_LeftToRightSwipe.");
				mEvent.common.type = WVR_EventType.WVR_EventType_LeftToRightSwipe;
				mEvent.device.type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
				mEvent.input.inputId = WVR_InputId.WVR_InputId_Alias1_Touchpad;
				hasEvent = true;
			}
		}
		#endregion

		#region Device Pose
		// =================== Pose ===============================
		// Head position variables.
		private WVR_DevicePosePair_t posePairHead = new WVR_DevicePosePair_t();
		private WVR_DevicePosePair_t posePairRight = new WVR_DevicePosePair_t();
		private WVR_DevicePosePair_t posePairLeft = new WVR_DevicePosePair_t();

		private bool is6DoFPose = false;

		private Vector3 defaultHeadPosition = Vector3.zero;
		private Vector3 headPosition = Vector3.zero;
		private float headPosX = 0, headPosY = 0, headPosZ = 0;
		private readonly Vector3 CENTER_EYE_POSITION = new Vector3(0, 0.15f, 0.12f);
		private Vector3 NECK_OFFSET = new Vector3(0, 0.15f, -0.08f);
		// Head rotation variables.
		private Quaternion headRotation = Quaternion.identity;
		//private float headAngleX = 0, headAngleY = 0, headAngleZ = 0;
		// Head RigidTransform and Pose Matirx
		private RigidTransform headRigidTransform = RigidTransform.identity;
		private WVR_Matrix4f_t headPoseMatrix;

		// Right position variables.
		private const float shiftSpeed_Right = 10.0f;
		private Vector3 rightPosition = Vector3.zero;
		private float rightPosX = 0, rightPosY = 0, rightPosZ = 0;
		// Right rotation variables.
		private Quaternion rightRotation = Quaternion.identity;
		private float rightAngleX = 0, rightAngleY = 0, rightAngleZ = 0;
		// Right RigidTransform and Pose Matirx
		private RigidTransform rightRigidTransform = RigidTransform.identity;
		private WVR_Matrix4f_t rightPoseMatrix;

		// Left position variables.
		private Vector3 leftPosition = Vector3.zero;
		private float leftPosX = 0, leftPosY = 0, leftPosZ = 0;
		// Left rotation variables.
		private Quaternion leftRotation = Quaternion.identity;
		private float leftAngleX = 0, leftAngleY = 0, leftAngleZ = 0;
		// Right RigidTransform and Pose Matirx
		private RigidTransform leftRigidTransform = RigidTransform.identity;
		private WVR_Matrix4f_t leftPoseMatrix;

		// Position simulation variables.
		private Quaternion bodyRotation = Quaternion.identity;
		private Vector3 bodyDirection = Vector3.zero;
		private const float BodyAngleBound = 0.01f;
		private const float BodyAngleLimitation = 0.3f; // bound of controller angle in SPEC provided to provider.
		private uint framesOfFreeze = 0;                // if framesOfFreeze >= mFPS, means controller freezed.
		private Vector3 v3ChangeArmXAxis = new Vector3(0, 1, 1);
		private readonly Vector3 HEADTOELBOW_OFFSET = new Vector3(0.2f, -0.7f, 0f);
		private readonly Vector3 ELBOW_PITCH_OFFSET = new Vector3(-0.2f, 0.55f, 0.08f);
		private readonly Vector3 ELBOW_RAISE_OFFSET = new Vector3(0, 0, 0);
		private const float ELBOW_PITCH_ANGLE_MIN = 0;
		private const float ELBOW_PITCH_ANGLE_MAX = 60;
		private const float ELBOW_TO_XYPLANE_LERP_MIN = 0.45f;
		private const float ELBOW_TO_XYPLANE_LERP_MAX = 0.65f;
		private readonly Vector3 ELBOWTOWRIST_OFFSET = new Vector3(0.0f, 0.0f, 0.15f);
		private readonly Vector3 WRISTTOCONTROLLER_OFFSET = new Vector3(0.0f, 0.0f, 0.05f);
		/// controller lerp speed for smooth movement between with head position case and without head position case
		private float smoothMoveSpeed = 0.3f;
		private Vector3 controllerSimulatedPosition = Vector3.zero;
		//private Quaternion controllerSimulatedRotation = Quaternion.identity;

		private WVR_PoseOriginModel hmdOriginModel = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
		public void GetSyncPose(WVR_PoseOriginModel origin, WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
		{
			hmdOriginModel = origin;
			uint index = 0;
			if (index < pairArrayCount)
				poseArray[index++] = posePairHead;
			if (index < pairArrayCount)
				poseArray[index++] = posePairRight;
			if (index < pairArrayCount)
				poseArray[index++] = posePairLeft;
		}

		private float MinimumX = -90F;
		private float MaximumX = 90F;
		Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
			angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
			q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

			return q;
		}

		private Quaternion headRotHorizontal = Quaternion.identity, headRotVertical = Quaternion.identity;
		private void UpdateHeadPose(float axis_x, float axis_y, float axis_z)
		{
			/*if (WXRInput.GetKey(KeyCode.LeftAlt))
			{
				headAngleX -= axis_y * 2.4f;
				headAngleX = Mathf.Clamp(headAngleX, -89, 89);
				headAngleY += axis_x * 5;
				if (headAngleY <= -180)
				{
					headAngleY += 360;
				}
				else if (headAngleY > 180)
				{
					headAngleY -= 360;
				}
			}
			if (WXRInput.GetKey(KeyCode.LeftControl))
			{
				headAngleZ += axis_x * 5;
				headAngleZ = Mathf.Clamp(headAngleZ, -89, 89);
			}
			headRotation = Quaternion.Euler(headAngleX, headAngleY, headAngleZ);*/

			if (!WXRInput.GetKey(KeyCode.RightAlt) && !WXRInput.GetKey(KeyCode.LeftAlt) && WXRInput.GetMouseButton(1))    // right mouse key
			{
				float yRot = axis_x * 2;
				float xRot = axis_y * 2;

				headRotHorizontal *= Quaternion.Euler(0f, yRot, 0f);
				headRotVertical *= Quaternion.Euler(-xRot, 0f, 0f);
				headRotVertical = ClampRotationAroundXAxis(headRotVertical);

				headRotation = headRotHorizontal * headRotVertical;
			}

			if (!WXRInput.GetKey(KeyCode.RightShift) && !WXRInput.GetKey(KeyCode.LeftShift) && WXRInput.GetMouseButton(0))   // left mouse key
			{
				headPosX += axis_x / 5;
				headPosY += axis_y / 5;
				headPosZ += axis_z;

				headPosition.x = headPosX;
				headPosition.y = headPosY;
				headPosition.z = headPosZ;
				if (hmdOriginModel == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF && enableNeckModel)
				{
					headPosition = ApplyNeckToHead(headPosition);
				}
				headPosition.y = hmdOriginModel == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround ? headPosition.y + 1.75f : headPosition.y;
			}

			headRigidTransform.update(headPosition, headRotation);
			headPoseMatrix = GetOpenGLMatrix44(headPosition, headRotation);
		}

		private void UpdateRightPose(float axis_x, float axis_y, float axis_z)
		{
			// Right-Alt + mouse for x & y angle.
			if (WXRInput.GetKey(KeyCode.RightAlt))
			{
				rightAngleY += axis_x / 2;
				rightAngleX -= axis_y * 1.5f;
			}
			// Right-Ctrl + mouse for z angle.
			if (WXRInput.GetKey(KeyCode.RightControl))
			{
				rightAngleZ += axis_z * 5;
			}
			rightRotation = Quaternion.Euler(rightAngleX, rightAngleY, rightAngleZ);

			// Right-Shift + mouse for position.
			if (WXRInput.GetKey(KeyCode.RightShift))
			{
				rightPosX += axis_x / 5;
				rightPosY += axis_y / 5;
				rightPosZ += axis_z;
			}

			//-------- keyboard control ---------
			if (WXRInput.GetKey(KeyCode.RightArrow)) { rightPosX += shiftSpeed_Right * Time.deltaTime; }
			if (WXRInput.GetKey(KeyCode.LeftArrow)) { rightPosX -= shiftSpeed_Right * Time.deltaTime; }
			if (WXRInput.GetKey(KeyCode.UpArrow)) { rightPosY += shiftSpeed_Right * Time.deltaTime; }
			if (WXRInput.GetKey(KeyCode.DownArrow)) { rightPosY -= shiftSpeed_Right * Time.deltaTime; }

			if (simulationType == WVR_SimulationType.WVR_SimulationType_ForceOff
				|| (simulationType == WVR_SimulationType.WVR_SimulationType_Auto && is6DoFPose == true)
			)
			{
				rightPosition.x = rightPosX;
				rightPosition.y = rightPosY;
				rightPosition.z = rightPosZ;
			}
			if (simulationType == WVR_SimulationType.WVR_SimulationType_ForceOn
				|| (simulationType == WVR_SimulationType.WVR_SimulationType_Auto && is6DoFPose == false)
			)
			{
				updateControllerPose(WVR_DeviceType.WVR_DeviceType_Controller_Right, rightRigidTransform);
				rightPosition = Vector3.Lerp(rightPosition, controllerSimulatedPosition, smoothMoveSpeed);
			}

			rightRigidTransform.update(rightPosition, rightRotation);

			// ControllerPoseMode
			switch (m_RightControllerPoseMode)
			{
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger:
					rightRotation = rightRotation * Quaternion.Euler(triggerModeRotationOffset);
					break;
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle:
					rightRotation = rightRotation * Quaternion.Euler(handleModeRotationOffset);
					break;
				default:
					break;
			}

			rightPoseMatrix = GetOpenGLMatrix44(rightPosition, rightRotation);
		}

		private void UpdateLefHandPose(float axis_x, float axis_y, float axis_z)
		{
			//-------- mouse control ---------
			if (WXRInput.GetKey(KeyCode.LeftAlt))
			{
				leftAngleY += axis_x / 2;
				leftAngleX -= (float)(axis_y * 1.5f);
			}
			if (WXRInput.GetKey(KeyCode.LeftControl))
			{
				leftAngleZ += axis_z * 5;
			}
			leftRotation = Quaternion.Euler(leftAngleX, leftAngleY, leftAngleZ);

			if (WXRInput.GetKey(KeyCode.LeftShift))
			{
				leftPosX += axis_x / 5;
				leftPosY += axis_y / 5;
				leftPosZ += axis_z;
			}
			if (simulationType == WVR_SimulationType.WVR_SimulationType_ForceOff
				|| (simulationType == WVR_SimulationType.WVR_SimulationType_Auto && is6DoFPose == true)
			)
			{
				leftPosition.x = leftPosX;
				leftPosition.y = leftPosY;
				leftPosition.z = leftPosZ;
			}
			if (simulationType == WVR_SimulationType.WVR_SimulationType_ForceOn
				|| (simulationType == WVR_SimulationType.WVR_SimulationType_Auto && is6DoFPose == false)
			)
			{
				updateControllerPose(WVR_DeviceType.WVR_DeviceType_Controller_Left, leftRigidTransform);
				leftPosition = Vector3.Lerp(leftPosition, controllerSimulatedPosition, smoothMoveSpeed);
			}

			leftRigidTransform.update(leftPosition, leftRotation);

			// ControllerPoseMode
			switch (m_LeftControllerPoseMode)
			{
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger:
					leftRotation = leftRotation * Quaternion.Euler(triggerModeRotationOffset);
					break;
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle:
					leftRotation = leftRotation * Quaternion.Euler(handleModeRotationOffset);
					break;
				default:
					break;
			}

			leftPoseMatrix = GetOpenGLMatrix44(leftPosition, leftRotation);
		}

		private void SetDevicePosePairHead()
		{
			posePairHead.type = WVR_DeviceType.WVR_DeviceType_HMD;
			posePairHead.pose.IsValidPose = true;
			posePairHead.pose.PoseMatrix = headPoseMatrix;
			posePairHead.pose.Velocity.v0 = 0.1f;
			posePairHead.pose.Velocity.v1 = 0.0f;
			posePairHead.pose.Velocity.v2 = 0.0f;
			posePairHead.pose.AngularVelocity.v0 = 0.1f;
			posePairHead.pose.AngularVelocity.v1 = 0.1f;
			posePairHead.pose.AngularVelocity.v2 = 0.1f;
			posePairHead.pose.Is6DoFPose = is6DoFPose;
			posePairHead.pose.OriginModel = hmdOriginModel;
		}

		private void SetDevicePosePairRight()
		{
			posePairRight.type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
			posePairRight.pose.IsValidPose = true;
			posePairRight.pose.PoseMatrix = rightPoseMatrix;
			posePairRight.pose.Velocity.v0 = 0.1f;
			posePairRight.pose.Velocity.v1 = 0.0f;
			posePairRight.pose.Velocity.v2 = 0.0f;
			posePairRight.pose.AngularVelocity.v0 = 0.1f;
			posePairRight.pose.AngularVelocity.v1 = 0.1f;
			posePairRight.pose.AngularVelocity.v2 = 0.1f;
			posePairRight.pose.Is6DoFPose = is6DoFPose;
			posePairRight.pose.OriginModel = hmdOriginModel;
		}

		private void SetDevicePosePairLeft()
		{
			posePairLeft.type = WVR_DeviceType.WVR_DeviceType_Controller_Left;
			posePairLeft.pose.IsValidPose = true;
			posePairLeft.pose.PoseMatrix = leftPoseMatrix;
			posePairLeft.pose.Velocity.v0 = 0.1f;
			posePairLeft.pose.Velocity.v1 = 0.0f;
			posePairLeft.pose.Velocity.v2 = 0.0f;
			posePairLeft.pose.AngularVelocity.v0 = 0.1f;
			posePairLeft.pose.AngularVelocity.v1 = 0.1f;
			posePairLeft.pose.AngularVelocity.v2 = 0.1f;
			posePairLeft.pose.Is6DoFPose = is6DoFPose;
			posePairLeft.pose.OriginModel = hmdOriginModel;
		}

		private Vector3 GetHeadPosition()
		{
			return followHead ? headPosition : defaultHeadPosition;
		}

		private Vector3 GetHeadForward()
		{
			return headRotation * Vector3.forward;
		}

		private void updateControllerPose(WVR_DeviceType device, RigidTransform rtPose)
		{
			bodyRotation = Quaternion.identity;
			UpdateHeadAndBodyPose(device, rtPose);

			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				v3ChangeArmXAxis.x = 1.0f;
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				v3ChangeArmXAxis.x = -1.0f;

			ComputeControllerPose(rtPose);
			controllerSimulatedPosition += CENTER_EYE_POSITION;
		}

		private void UpdateHeadAndBodyPose(WVR_DeviceType device, RigidTransform rtPose)
		{
			// Determine the gaze direction horizontally.
			Vector3 gazeDirection = GetHeadForward();
			gazeDirection.y = 0;
			gazeDirection.Normalize();

			float _bodyLerpFilter = BodyRotationFilter(device, rtPose);
			if (_bodyLerpFilter > 0)
			{
				if (!followHead)
				{
					defaultHeadPosition = headPosition;
				}
			}

			bodyDirection = Vector3.Slerp(bodyDirection, gazeDirection, _bodyLerpFilter);
			bodyRotation = Quaternion.FromToRotation(Vector3.forward, bodyDirection);
		}

		private bool quaternionEqual(Quaternion qua1, Quaternion qua2)
		{
			if (qua1.x == qua2.x &&
				qua1.y == qua2.y &&
				qua1.z == qua2.z &&
				qua1.w == qua2.w)
				return true;
			return false;
		}

		private float BodyRotationFilter(WVR_DeviceType device, RigidTransform rtPose)
		{
			float _bodyLerpFilter = 0;

			try
			{
				Quaternion _rot_old = Quaternion.identity;
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
					_rot_old = rightRotation;
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
					_rot_old = leftRotation;
				Quaternion _rot_new = rtPose.rot;
				float _rot_XY_angle_old = 0, _rot_XY_angle_new = 0;

				Vector3 _rot_forward = Vector3.zero;
				Quaternion _rot_XY_rotation = Quaternion.identity;

				_rot_forward = _rot_old * Vector3.forward;
				_rot_XY_rotation = Quaternion.FromToRotation(Vector3.forward, _rot_forward);
				_rot_XY_angle_old = Quaternion.Angle(_rot_XY_rotation, Quaternion.identity);

				_rot_forward = _rot_new * Vector3.forward;
				_rot_XY_rotation = Quaternion.FromToRotation(Vector3.forward, _rot_forward);
				_rot_XY_angle_new = Quaternion.Angle(_rot_XY_rotation, Quaternion.identity);

				float _diff_angle = _rot_XY_angle_new - _rot_XY_angle_old;
				_diff_angle = _diff_angle > 0 ? _diff_angle : -_diff_angle;

				_bodyLerpFilter = Mathf.Clamp((_diff_angle - BodyAngleBound) / BodyAngleLimitation, 0, 1.0f);

				framesOfFreeze = _bodyLerpFilter < 1.0f ? framesOfFreeze + 1 : 0;

				if (framesOfFreeze > mFPS)
					_bodyLerpFilter = 0;
			}
			catch (NullReferenceException e)
			{
				ERROR("BodyRotationFilter() NullReferenceException " + e.Message);
			}
			catch (MissingReferenceException e)
			{
				ERROR("BodyRotationFilter() MissingReferenceException " + e.Message);
			}
			catch (MissingComponentException e)
			{
				ERROR("BodyRotationFilter() MissingComponentException " + e.Message);
			}
			catch (IndexOutOfRangeException e)
			{
				ERROR("BodyRotationFilter() IndexOutOfRangeException " + e.Message);
			}
			return _bodyLerpFilter;
		}

		private Vector3 ApplyNeckToHead(Vector3 head_position)
		{
			Vector3 _neckOffset = headRotation * NECK_OFFSET;
			_neckOffset.y -= NECK_OFFSET.y;  // add neck length
			head_position += _neckOffset;

			return head_position;
		}

		/// <summary>
		/// Get the position of controller in Arm Model
		/// 
		/// Consider the parts construct controller position:
		/// Parts contain elbow, wrist and controller and each part has default offset from head.
		/// <br>
		/// 1. simulated elbow offset = default elbow offset apply body rotation = body rotation (Quaternion) * elbow offset (Vector3)
		/// <br>
		/// 2. simulated wrist offset = default wrist offset apply elbow rotation = elbow rotation (Quaternion) * wrist offset (Vector3)
		/// <br>
		/// 3. simulated controller offset = default controller offset apply wrist rotation = wrist rotation (Quat) * controller offset (V3)
		/// <br>
		/// head + 1 + 2 + 3 = controller position.
		/// </summary>
		/// <param name="rtPose">RigidTransform</param>
		private void ComputeControllerPose(RigidTransform rtPose)
		{
			// if bodyRotation angle is θ, _inverseBodyRation is -θ
			// the operator * of Quaternion in Unity means concatenation, not multipler.
			// If quaternion qA has angle θ, quaternion qB has angle ε,
			// qA * qB will plus θ and ε which means rotating angle θ then rotating angle ε.
			// (_inverseBodyRotation * rotation of controller in world space) means angle ε subtracts angle θ.
			Quaternion _controllerRotation = Quaternion.Inverse(bodyRotation) * rtPose.rot;
			Vector3 _headPosition = GetHeadPosition();

			/// 1. simulated elbow offset = default elbow offset apply body rotation = body rotation (Quaternion) * elbow offset (Vector3)
			// Default left / right elbow offset.
			Vector3 _elbowOffset = Vector3.Scale(HEADTOELBOW_OFFSET, v3ChangeArmXAxis);
			// Default left / right elbow pitch offset.
			Vector3 _elbowPitchOffset = Vector3.Scale(ELBOW_PITCH_OFFSET, v3ChangeArmXAxis) + ELBOW_RAISE_OFFSET;

			// Use controller pitch to simulate elbow pitch.
			// Range from ELBOW_PITCH_ANGLE_MIN ~ ELBOW_PITCH_ANGLE_MAX.
			// The percent of pitch angle will be used to calculate the position offset.
			Vector3 _controllerForward = _controllerRotation * Vector3.forward;
			float _controllerPitch = 90.0f - Vector3.Angle(_controllerForward, Vector3.up); // 0~90
			float _controllerPitchRadio = (_controllerPitch - ELBOW_PITCH_ANGLE_MIN) / (ELBOW_PITCH_ANGLE_MAX - ELBOW_PITCH_ANGLE_MIN);
			_controllerPitchRadio = Mathf.Clamp(_controllerPitchRadio, 0.0f, 1.0f);

			// According to pitch angle percent, plus offset to elbow position.
			_elbowOffset += _elbowPitchOffset * _controllerPitchRadio;
			// Apply body rotation and head position to calculate final elbow position.
			_elbowOffset = _headPosition + bodyRotation * _elbowOffset;

			// Rotation from Z-axis to XY-plane used to simulated elbow & wrist rotation.
			Quaternion _controllerXYRotation = Quaternion.FromToRotation(Vector3.forward, _controllerForward);
			float _controllerXYRotationRadio = (Quaternion.Angle(_controllerXYRotation, Quaternion.identity)) / 180;
			// Simulate the elbow raising curve.
			float _elbowCurveLerpValue = ELBOW_TO_XYPLANE_LERP_MIN + (_controllerXYRotationRadio * (ELBOW_TO_XYPLANE_LERP_MAX - ELBOW_TO_XYPLANE_LERP_MIN));
			Quaternion _controllerXYLerpRotation = Quaternion.Lerp(Quaternion.identity, _controllerXYRotation, _elbowCurveLerpValue);


			/// 2. simulated wrist offset = default wrist offset apply elbow rotation = elbow rotation (Quaternion) * wrist offset (Vector3)
			// Default left / right wrist offset
			Vector3 _wristOffset = Vector3.Scale(ELBOWTOWRIST_OFFSET, v3ChangeArmXAxis);
			// elbow rotation + curve = wrist rotation
			// wrist rotation = controller XY rotation
			// => elbow rotation + curve = controller XY rotation
			// => elbow rotation = controller XY rotation - curve
			Quaternion _elbowRotation = bodyRotation * Quaternion.Inverse(_controllerXYLerpRotation) * _controllerXYRotation;
			// Apply elbow offset and elbow rotation to calculate final wrist position.
			_wristOffset = _elbowOffset + _elbowRotation * _wristOffset;


			/// 3. simulated controller offset = default controller offset apply wrist rotation = wrist rotation (Quat) * controller offset (V3)
			// Default left / right controller offset.
			Vector3 _controllerOffset = Vector3.Scale(WRISTTOCONTROLLER_OFFSET, v3ChangeArmXAxis);
			Quaternion _wristRotation = _controllerXYRotation;
			// Apply wrist offset and wrist rotation to calculate final controller position.
			_controllerOffset = _wristOffset + _wristRotation * _controllerOffset;

			controllerSimulatedPosition = /*bodyRotation */ _controllerOffset;
			//controllerSimulatedRotation = bodyRotation * _controllerRotation;
		}

		private WVR_Matrix4f_t GetOpenGLMatrix44(Vector3 pos, Quaternion rot)
		{
			WVR_Matrix4f_t matrix44;
			// m0 = 1 - 2 * y^2 - 2 * z^2
			matrix44.m0 = 1 - (2 * (rot.y * rot.y)) - (2 * (rot.z * rot.z));
			// m1 = 2xy - 2zw
			matrix44.m1 = (2 * rot.x * rot.y) - (2 * rot.z * rot.w);
			// m2 = -(2xz + 2yw)
			matrix44.m2 = -((2 * rot.x * rot.z) + (2 * rot.y * rot.w));
			// m3 = X
			matrix44.m3 = pos.x;
			// m4 = 2xy + 2zw
			matrix44.m4 = (2 * rot.x * rot.y) + (2 * rot.z * rot.w);
			// m5 = 1 - 2 * x^2 - 2 * z^2
			matrix44.m5 = 1 - (2 * (rot.x * rot.x)) - (2 * (rot.z * rot.z));
			// m6 = 2xw - 2yz
			matrix44.m6 = (2 * rot.x * rot.w) - (2 * rot.y * rot.z);
			// m7 = Y
			matrix44.m7 = pos.y;
			// m8 = 2yw - 2xz
			matrix44.m8 = (2 * rot.y * rot.w) - (2 * rot.x * rot.z);
			// m9 = -(2yz + 2xw)
			matrix44.m9 = -((2 * rot.y * rot.z) + (2 * rot.x * rot.w));
			// m10 = 1 - 2 * x^2 - 2 * y^2
			matrix44.m10 = 1 - (2 * rot.x * rot.x) - (2 * rot.y * rot.y);
			// m11 = -Z
			matrix44.m11 = -pos.z;
			// m12 = 0
			matrix44.m12 = 0;
			// m13 = 0
			matrix44.m13 = 0;
			// m14 = 0
			matrix44.m14 = 0;
			// m15 = 1
			matrix44.m15 = 1;

			return matrix44;
		}

		private WVR_Quatf_t GetOpenGLQuaternion(Quaternion rot)
		{
			WVR_Quatf_t qua;
			qua.x = rot.x;
			qua.y = rot.y;
			qua.z = -rot.z;
			qua.w = -rot.w;
			return qua;
		}

		private WVR_Vector3f_t GetOpenGLVector(Vector3 pos)
		{
			WVR_Vector3f_t vec;
			vec.v0 = pos.x;
			vec.v1 = pos.y;
			vec.v2 = -pos.z;
			return vec;
		}
		#endregion

		#region Simulation Pose
		private WVR_SimulationType simulationType = WVR_SimulationType.WVR_SimulationType_ForceOn;
		public void SetArmModel(WVR_SimulationType type)
		{
			DEBUG("SetArmModel() " + type);
			simulationType = type;
		}

		private bool followHead = false;
		public void SetArmSticky(bool stickyArm)
		{
			DEBUG("Follow head = " + stickyArm);
			followHead = stickyArm;
		}

		private bool enableNeckModel = true;
		public void SetNeckModelEnabled(bool enabled)
		{
			DEBUG("SetNeckModelEnabled() " + enabled);
			enableNeckModel = enabled;
		}
		#endregion

		#region Key Mapping
		private WVR_InputMappingPair_t[] inputTable_Hmd = null, inputTable_Right = null, inputTable_Left = null;

		public bool SetInputRequest(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_HMD)
			{
				inputTable_Hmd = new WVR_InputMappingPair_t[size];
				WVR_InputId[] inputId_Hmd = new WVR_InputId[size];
				for (int i = 0; i < (int)size; i++)
				{
					inputId_Hmd[i] = request[i].id;
					inputTable_Hmd[i].destination = request[i];
				}
				UpdateInputTable(inputId_Hmd, inputTable_Hmd);
				for (int i = 0; i < inputTable_Hmd.Length; i++)
					DEBUG("SetInputRequest() " + type + ", src: " + inputTable_Hmd[i].source.id + ", dst: " + inputTable_Hmd[i].destination.id);
			}

			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
			{
				inputTable_Right = new WVR_InputMappingPair_t[size];
				WVR_InputId[] inputId_Right = new WVR_InputId[size];
				for (int i = 0; i < (int)size; i++)
				{
					inputId_Right[i] = request[i].id;
					inputTable_Right[i].destination = request[i];
				}
				UpdateInputTable(inputId_Right, inputTable_Right);
				for (int i = 0; i < inputTable_Right.Length; i++)
					DEBUG("SetInputRequest() " + type + ", src: " + inputTable_Right[i].source.id + ", dst: " + inputTable_Right[i].destination.id);
			}
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				inputTable_Left = new WVR_InputMappingPair_t[size];
				WVR_InputId[] inputId_Left = new WVR_InputId[size];
				for (int i = 0; i < (int)size; i++)
				{
					inputId_Left[i] = request[i].id;
					inputTable_Left[i].destination = request[i];
				}
				UpdateInputTable(inputId_Left, inputTable_Left);
				for (int i = 0; i < inputTable_Left.Length; i++)
					DEBUG("SetInputRequest() " + type + ", src: " + inputTable_Left[i].source.id + ", dst: " + inputTable_Left[i].destination.id);
			}
			return true;
		}

		void UpdateInputTable(WVR_InputId[] buttons, WVR_InputMappingPair_t[] inputTable)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				switch (buttons[i])
				{
					case WVR_InputId.WVR_InputId_Alias1_System:
					case WVR_InputId.WVR_InputId_Alias1_Menu:
					case WVR_InputId.WVR_InputId_Alias1_Grip:
					case WVR_InputId.WVR_InputId_Alias1_DPad_Left:
					case WVR_InputId.WVR_InputId_Alias1_DPad_Up:
					case WVR_InputId.WVR_InputId_Alias1_DPad_Right:
					case WVR_InputId.WVR_InputId_Alias1_DPad_Down:
					case WVR_InputId.WVR_InputId_Alias1_Volume_Up:
					case WVR_InputId.WVR_InputId_Alias1_Volume_Down:
					case WVR_InputId.WVR_InputId_Alias1_Bumper:
					case WVR_InputId.WVR_InputId_Alias1_A:
					case WVR_InputId.WVR_InputId_Alias1_B:
					case WVR_InputId.WVR_InputId_Alias1_X:
					case WVR_InputId.WVR_InputId_Alias1_Y:
					case WVR_InputId.WVR_InputId_Alias1_Enter:
					case WVR_InputId.WVR_InputId_Alias1_Back:
						inputTable[i].source.id = buttons[i];
						inputTable[i].source.capability = (uint)WVR_InputType.WVR_InputType_Button;
						inputTable[i].source.axis_type = WVR_AnalogType.WVR_AnalogType_None;
						break;
					case WVR_InputId.WVR_InputId_Alias1_Touchpad:
					case WVR_InputId.WVR_InputId_Alias1_Thumbstick:
						inputTable[i].source.id = buttons[i];
						inputTable[i].source.capability = (uint)(WVR_InputType.WVR_InputType_Button | WVR_InputType.WVR_InputType_Touch | WVR_InputType.WVR_InputType_Analog);
						inputTable[i].source.axis_type = WVR_AnalogType.WVR_AnalogType_2D;
						break;
					case WVR_InputId.WVR_InputId_Alias1_Trigger:
						inputTable[i].source.id = buttons[i];
						inputTable[i].source.capability = (uint)(WVR_InputType.WVR_InputType_Button | WVR_InputType.WVR_InputType_Touch | WVR_InputType.WVR_InputType_Analog);
						inputTable[i].source.axis_type = WVR_AnalogType.WVR_AnalogType_1D;
						break;
					case WVR_InputId.WVR_InputId_Alias1_Parking:
						inputTable[i].source.id = buttons[i];
						inputTable[i].source.capability = (uint)WVR_InputType.WVR_InputType_Touch;
						inputTable[i].source.axis_type = WVR_AnalogType.WVR_AnalogType_None;
						break;
					default:
						break;
				}
			}
		}

		public uint GetInputMappingTable(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size)
		{
			uint count = 0;
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					if (inputTable_Hmd != null)
					{
						for (int i = 0; i < inputTable_Hmd.Length; i++)
							table[i] = inputTable_Hmd[i];
						count = (uint)inputTable_Hmd.Length;
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					if (inputTable_Right != null)
					{
						for (int i = 0; i < inputTable_Right.Length; i++)
							table[i] = inputTable_Right[i];
						count = (uint)inputTable_Right.Length;
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					if (inputTable_Left != null)
					{
						for (int i = 0; i < inputTable_Left.Length; i++)
							table[i] = inputTable_Left[i];
						count = (uint)inputTable_Left.Length;
					}
					break;
				default:
					break;
			}

			return count;
		}

		public bool GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair)
		{
			WVR_InputMappingPair_t[] inputTable = new WVR_InputMappingPair_t[10];
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_HMD:
					inputTable = inputTable_Hmd;
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					inputTable = inputTable_Right;
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					inputTable = inputTable_Left;
					break;
				default:
					break;
			}

			for (int i = 0; i < inputTable.Length; i++)
			{
				if (destination == inputTable[i].destination.id)
				{
					pair.source = inputTable[i].source;
					pair.destination = inputTable[i].destination;
					return true;
				}
			}

			return false;
		}
		#endregion

		#region Interaction Mode
		private WVR_InteractionMode interactionMode = WVR_InteractionMode.WVR_InteractionMode_Controller;
		[System.Obsolete("This is an obsolete function.", true)]
		public bool SetInteractionMode(WVR_InteractionMode mode)
		{
			interactionMode = mode;
			return true;
		}
		public WVR_InteractionMode GetInteractionMode()
		{
			return interactionMode;
		}

		private WVR_GazeTriggerType gazeType = WVR_GazeTriggerType.WVR_GazeTriggerType_TimeoutButton;
		public bool SetGazeTriggerType(WVR_GazeTriggerType type)
		{
			gazeType = type;
			return true;
		}
		public WVR_GazeTriggerType GetGazeTriggerType()
		{
			return gazeType;
		}
		#endregion

		#region Arena
		private WVR_Arena_t mArena;
		private WVR_ArenaVisible mArenaVisible;
		[Obsolete("This API is deprecated and is no longer supported.", true)]
		public bool SetArena(ref WVR_Arena_t arena)
		{
			mArena = arena;
			return true;
		}

		public WVR_Arena_t GetArena()
		{
			return mArena;
		}

		public bool IsOverArenaRange()
		{
			return false;
		}

		public void SetArenaVisible(WVR_ArenaVisible config)
		{
			mArenaVisible = config;
		}

		public WVR_ArenaVisible GetArenaVisible()
		{
			return mArenaVisible;
		}
		#endregion

		#region Focused Controller
		private WVR_DeviceType focusedType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
		public WVR_DeviceType GetFocusedController()
		{
			return focusedType;
		}

		public void SetFocusedController(WVR_DeviceType focusController)
		{
			focusedType = focusController;
		}
		#endregion

		#region Eye Tracking
		public WVR_Result StartEyeTracking()
		{
			return WVR_Result.WVR_Success;
		}

		public void StopEyeTracking()
		{
		}

		private Vector3 eyeGazeOrigin = Vector3.zero;
		private Vector3 eyeGazeDirection = new Vector3(-0.5f, 0, 1);
		private readonly Vector3[] gazeDirectionArray = new Vector3[]
		{
		new Vector3(-0.5f, 0, 1),
		new Vector3(0, 0, 1),
		new Vector3(0.5f, 0, 1)
		};

		private const float eyeGazeDistance = 5.0f;
		private Vector3 eyeGazePointRight = new Vector3(0, 0, eyeGazeDistance);
		private readonly Vector3[] gazePointsArray = new Vector3[] {
		new Vector3(-2,     2, eyeGazeDistance),
		new Vector3(-1.5f,  2, eyeGazeDistance),
		new Vector3(-1,     2, eyeGazeDistance),
		new Vector3(-0.5f,  2, eyeGazeDistance),
		new Vector3(0,      2, eyeGazeDistance),
		new Vector3(0.5f,   2, eyeGazeDistance),
		new Vector3(1,      2, eyeGazeDistance),
		new Vector3(1.5f,   2, eyeGazeDistance),
		new Vector3(2,      2, eyeGazeDistance),

		new Vector3(-2,     1.5f, eyeGazeDistance),
		new Vector3(-1.5f,  1.5f, eyeGazeDistance),
		new Vector3(-1,     1.5f, eyeGazeDistance),
		new Vector3(-0.5f,  1.5f, eyeGazeDistance),
		new Vector3(0,      1.5f, eyeGazeDistance),
		new Vector3(0.5f,   1.5f, eyeGazeDistance),
		new Vector3(1,      1.5f, eyeGazeDistance),
		new Vector3(1.5f,   1.5f, eyeGazeDistance),
		new Vector3(2,      1.5f, eyeGazeDistance),

		new Vector3(-2,     1, eyeGazeDistance),
		new Vector3(-1.5f,  1, eyeGazeDistance),
		new Vector3(-1,     1, eyeGazeDistance),
		new Vector3(-0.5f,  1, eyeGazeDistance),
		new Vector3(0,      1, eyeGazeDistance),
		new Vector3(0.5f,   1, eyeGazeDistance),
		new Vector3(1,      1, eyeGazeDistance),
		new Vector3(1.5f,   1, eyeGazeDistance),
		new Vector3(2,      1, eyeGazeDistance),

		new Vector3(-2,     0.5f, eyeGazeDistance),
		new Vector3(-1.5f,  0.5f, eyeGazeDistance),
		new Vector3(-1,     0.5f, eyeGazeDistance),
		new Vector3(-0.5f,  0.5f, eyeGazeDistance),
		new Vector3(0,      0.5f, eyeGazeDistance),
		new Vector3(0.5f,   0.5f, eyeGazeDistance),
		new Vector3(1,      0.5f, eyeGazeDistance),
		new Vector3(1.5f,   0.5f, eyeGazeDistance),
		new Vector3(2,      0.5f, eyeGazeDistance),

		new Vector3(-2,     0, eyeGazeDistance),
		new Vector3(-1.5f,  0, eyeGazeDistance),
		new Vector3(-1,     0, eyeGazeDistance),
		new Vector3(-0.5f,  0, eyeGazeDistance),
		new Vector3(0,      0, eyeGazeDistance),
		new Vector3(0.5f,   0, eyeGazeDistance),
		new Vector3(1,      0, eyeGazeDistance),
		new Vector3(1.5f,   0, eyeGazeDistance),
		new Vector3(2,      0, eyeGazeDistance),

		new Vector3(-2,     -0.5f, eyeGazeDistance),
		new Vector3(-1.5f,  -0.5f, eyeGazeDistance),
		new Vector3(-1,     -0.5f, eyeGazeDistance),
		new Vector3(-0.5f,  -0.5f, eyeGazeDistance),
		new Vector3(0,      -0.5f, eyeGazeDistance),
		new Vector3(0.5f,   -0.5f, eyeGazeDistance),
		new Vector3(1,      -0.5f, eyeGazeDistance),
		new Vector3(1.5f,   -0.5f, eyeGazeDistance),
		new Vector3(2,      -0.5f, eyeGazeDistance),

		new Vector3(-2,     -1, eyeGazeDistance),
		new Vector3(-1.5f,  -1, eyeGazeDistance),
		new Vector3(-1,     -1, eyeGazeDistance),
		new Vector3(-0.5f,  -1, eyeGazeDistance),
		new Vector3(0,      -1, eyeGazeDistance),
		new Vector3(0.5f,   -1, eyeGazeDistance),
		new Vector3(1,      -1, eyeGazeDistance),
		new Vector3(1.5f,   -1, eyeGazeDistance),
		new Vector3(2,      -1, eyeGazeDistance),

		new Vector3(-2,     -1.5f, eyeGazeDistance),
		new Vector3(-1.5f,  -1.5f, eyeGazeDistance),
		new Vector3(-1,     -1.5f, eyeGazeDistance),
		new Vector3(-0.5f,  -1.5f, eyeGazeDistance),
		new Vector3(0,      -1.5f, eyeGazeDistance),
		new Vector3(0.5f,   -1.5f, eyeGazeDistance),
		new Vector3(1,      -1.5f, eyeGazeDistance),
		new Vector3(1.5f,   -1.5f, eyeGazeDistance),
		new Vector3(2,      -1.5f, eyeGazeDistance),

		new Vector3(-2,     -2, eyeGazeDistance),
		new Vector3(-1.5f,  -2, eyeGazeDistance),
		new Vector3(-1,     -2, eyeGazeDistance),
		new Vector3(-0.5f,  -2, eyeGazeDistance),
		new Vector3(0,      -2, eyeGazeDistance),
		new Vector3(0.5f,   -2, eyeGazeDistance),
		new Vector3(1,      -2, eyeGazeDistance),
		new Vector3(1.5f,   -2, eyeGazeDistance),
		new Vector3(2,      -2, eyeGazeDistance),
	};

		private Vector3 GetOpenGLPos(Vector3 pos)
		{
			pos.z *= -1;
			return pos;
		}

		private int gazeDirectionArrayIndex = 0;
		private int gazeFrames = 0, gazePointsArrayIndex = 0;
		private const int gazeFramesInterval = 300;
		private void UpdateGazePoint()
		{
			eyeGazeOrigin = GetOpenGLPos(headPosition);
			if (gazeFrames == 0)
			{
				gazeDirectionArrayIndex++;
				gazeDirectionArrayIndex %= gazeDirectionArray.Length;
				eyeGazeDirection = GetOpenGLPos(gazeDirectionArray[gazeDirectionArrayIndex]);

				// --------------------- old data ---------------------
				gazePointsArrayIndex++;
				gazePointsArrayIndex %= gazePointsArray.Length;
				eyeGazePointRight = GetOpenGLPos(gazePointsArray[gazePointsArrayIndex]);
			}
			gazeFrames++;
			gazeFrames %= gazeFramesInterval;
		}

		public WVR_Result GetEyeTracking(ref WVR_EyeTracking_t data)
		{
			data.combined.eyeTrackingValidBitMask = (ulong)(
				WVR_EyeTrackingStatus.WVR_GazeOriginValid |
				WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid
				);
			data.combined.gazeOrigin.v0 = eyeGazeOrigin.x;
			data.combined.gazeOrigin.v1 = eyeGazeOrigin.y;
			data.combined.gazeOrigin.v2 = eyeGazeOrigin.z;
			data.combined.gazeDirectionNormalized.v0 = eyeGazeDirection.x;
			data.combined.gazeDirectionNormalized.v1 = eyeGazeDirection.y;
			data.combined.gazeDirectionNormalized.v2 = eyeGazeDirection.z;

			data.right.eyeTrackingValidBitMask = (ulong)(
				WVR_EyeTrackingStatus.WVR_GazeOriginValid |
				WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid |
				WVR_EyeTrackingStatus.WVR_PupilDiameterValid |
				WVR_EyeTrackingStatus.WVR_EyeOpennessValid |
				WVR_EyeTrackingStatus.WVR_PupilPositionInSensorAreaValid
				);
			data.right.gazeOrigin.v0 = eyeGazeOrigin.x + 0.08f;
			data.right.gazeOrigin.v1 = eyeGazeOrigin.y;
			data.right.gazeOrigin.v2 = eyeGazeOrigin.z;
			data.right.gazeDirectionNormalized.v0 = eyeGazeDirection.x;
			data.right.gazeDirectionNormalized.v1 = eyeGazeDirection.y;
			data.right.gazeDirectionNormalized.v2 = eyeGazeDirection.z;

			data.left.eyeTrackingValidBitMask = (ulong)(
				WVR_EyeTrackingStatus.WVR_GazeOriginValid |
				WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid |
				WVR_EyeTrackingStatus.WVR_PupilDiameterValid |
				WVR_EyeTrackingStatus.WVR_EyeOpennessValid |
				WVR_EyeTrackingStatus.WVR_PupilPositionInSensorAreaValid
				);
			data.left.gazeOrigin.v0 = eyeGazeOrigin.x - 0.08f;
			data.left.gazeOrigin.v1 = eyeGazeOrigin.y;
			data.left.gazeOrigin.v2 = eyeGazeOrigin.z;
			data.left.gazeDirectionNormalized.v0 = eyeGazeDirection.x;
			data.left.gazeDirectionNormalized.v1 = eyeGazeDirection.y;
			data.left.gazeDirectionNormalized.v2 = eyeGazeDirection.z;
			return WVR_Result.WVR_Success;
		}
		#endregion

		#region Hand
		#region Gesture Core
		private WVR_HandGestureType[] s_GestureTypes = new WVR_HandGestureType[] {
			WVR_HandGestureType.WVR_HandGestureType_Invalid,
			WVR_HandGestureType.WVR_HandGestureType_Unknown,
			WVR_HandGestureType.WVR_HandGestureType_Fist,
			WVR_HandGestureType.WVR_HandGestureType_Five,
			WVR_HandGestureType.WVR_HandGestureType_OK,
			WVR_HandGestureType.WVR_HandGestureType_ThumbUp,
			WVR_HandGestureType.WVR_HandGestureType_IndexUp,
			WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch,
			WVR_HandGestureType.WVR_HandGestureType_Yeah,
		};
		private ulong[] s_GestureValues =
		{
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Invalid,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Unknown,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Fist,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Five,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_OK,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_ThumbUp,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_IndexUp,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch,
			(ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Yeah,
		};

		private int gestureTypeIndex = 0;
		private float gestureChangeTime = 0;
		private float gestureChangeDuration = .5f;

		private void InitHandGesture()
		{
			gestureTypeIndex = 0;
			gestureChangeTime = Time.unscaledTime;
		}
		private void UpdateHandGesture()
		{
			if (Time.unscaledTime - gestureChangeTime > gestureChangeDuration)
			{
				gestureChangeTime = Time.unscaledTime;
				int count = 0;
				while (count < s_GestureTypes.Length)
				{
					count++;

					gestureTypeIndex++;
					gestureTypeIndex %= s_GestureTypes.Length;

					ulong gesture_value = s_GestureValues[gestureTypeIndex];
					if ((gestureDemands & gesture_value) != 0)
					{
						DEBUG("Right gesture is changed to " + s_GestureTypes[gestureTypeIndex]);
						return;
					}
				}

				gestureTypeIndex = 0;
			}
		}
		#endregion

		private WVR_PoseOriginModel handOrgin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;

		#region Location and Orientation Core
		private enum HandJointType
		{
			// Left Arm, 19 finger bones + wrist / fore arm / upper arm.
			BONE_UPPERARM_L = 0,
			BONE_FOREARM_L,
			Wrist_L,
			Palm_L,
			Thumb_Joint1_L,
			Thumb_Joint2_L,
			Thumb_Joint3_L,
			Thumb_Tip_L,
			Index_Joint1_L,
			Index_Joint2_L,
			Index_Joint3_L,
			Index_Tip_L,
			Middle_Joint1_L,
			Middle_Joint2_L,
			Middle_Joint3_L,
			Middle_Tip_L,
			Ring_Joint1_L,
			Ring_Joint2_L,
			Ring_Joint3_L,
			Ring_Tip_L,
			Pinky_Joint1_L,
			Pinky_Joint2_L,
			Pinky_Joint3_L,
			Pinky_Tip_L,

			// Right Arm, 19 finger bones + wrist / fore arm / upper arm.
			BONE_UPPERARM_R,
			BONE_FOREARM_R,
			Wrist_R,
			Palm_R,
			Thumb_Joint1_R,
			Thumb_Joint2_R,
			Thumb_Joint3_R,
			Thumb_Tip_R,
			Index_Joint1_R,
			Index_Joint2_R,
			Index_Joint3_R,
			Index_Tip_R,
			Middle_Joint1_R,
			Middle_Joint2_R,
			Middle_Joint3_R,
			Middle_Tip_R,
			Ring_Joint1_R,
			Ring_Joint2_R,
			Ring_Joint3_R,
			Ring_Tip_R,
			Pinky_Joint1_R,
			Pinky_Joint2_R,
			Pinky_Joint3_R,
			Pinky_Tip_R,

			BONES_COUNT
		};

		// Left wrist.
		private readonly Vector3 WRIST_L_POS = new Vector3(-0.09f, 0, 0.2f);
		private readonly Vector3 BONE_HAND_WRIST_L_ROT = new Vector3(7, 0, -15);
		// Left palm.
		private readonly Vector3 PALM_L_POS = new Vector3(-0.08f, 0.02f, 0.2f);
		private readonly Vector3 HAND_PALM_L_ROT = new Vector3(0, 0, 1);
		// Left thumb.
		private readonly Vector3 THUMB_JOIN2_L_POS = new Vector3(-0.05f, 0.02f, 0.2f);
		private readonly Vector3 THUMB_JOIN2_L_ROT = new Vector3(0, 0, -42.54f);
		private readonly Vector3 THUMB_JOIN3_L_POS = new Vector3(-0.04f, 0.03f, 0.2f);
		private readonly Vector3 THUMB_JOIN3_L_ROT = new Vector3(0, 0, -42.54f);
		private readonly Vector3 THUMB_TIP_L_POS = new Vector3(-0.03f, 0.04f, 0.2f);
		private readonly Vector3 THUMB_TIP_L_ROT = new Vector3(0, 0, -42.54f);
		// Left index.
		private readonly Vector3 INDEX_JOINT1_L_POS = new Vector3(-0.06f, 0.04f, 0.2f);
		private readonly Vector3 INDEX_JOINT1_L_ROT = new Vector3(0, 0, -16);
		private readonly Vector3 INDEX_JOINT2_L_POS = new Vector3(-0.056f, 0.05f, 0.2f);
		private readonly Vector3 INDEX_JOINT2_L_ROT = new Vector3(0, 0, -16);
		private readonly Vector3 INDEX_JOINT3_L_POS = new Vector3(-0.052f, 0.06f, 0.2f);
		private readonly Vector3 INDEX_JOINT3_L_ROT = new Vector3(0, 0, -16);
		private readonly Vector3 INDEX_TIP_L_POS = new Vector3(-0.048f, 0.07f, 0.2f);
		private readonly Vector3 INDEX_TIP_L_ROT = new Vector3(0, 0, -16);
		// Left middle.
		private readonly Vector3 MIDDLE_JOINT1_L_POS = new Vector3(-0.075f, 0.045f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT1_L_ROT = new Vector3(0, 0, -0.87f);
		private readonly Vector3 MIDDLE_JOINT2_L_POS = new Vector3(-0.074f, 0.055f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT2_L_ROT = new Vector3(0, 0, -0.87f);
		private readonly Vector3 MIDDLE_JOINT3_L_POS = new Vector3(-0.073f, 0.065f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT3_L_ROT = new Vector3(0, 0, -0.87f);
		private readonly Vector3 MIDDLE_TIP_L_POS = new Vector3(-0.072f, 0.075f, 0.2f);
		private readonly Vector3 MIDDLE_TIP_L_ROT = new Vector3(0, 0, -0.87f);
		// Left ring.
		private readonly Vector3 RING_JOINT1_L_POS = new Vector3(-0.087f, 0.04f, 0.2f);
		private readonly Vector3 RING_JOINT1_L_ROT = new Vector3(0, 0, 12.48f);
		private readonly Vector3 RING_JOINT2_L_POS = new Vector3(-0.089f, 0.05f, 0.2f);
		private readonly Vector3 RING_JOINT2_L_ROT = new Vector3(0, 0, 12.48f);
		private readonly Vector3 RING_JOINT3_L_POS = new Vector3(-0.091f, 0.06f, 0.2f);
		private readonly Vector3 RING_JOINT3_L_ROT = new Vector3(0, 0, 12.48f);
		private readonly Vector3 RING_TIP_L_POS = new Vector3(-0.093f, 0.07f, 0.2f);
		private readonly Vector3 RING_TIP_L_ROT = new Vector3(0, 0, 12.48f);
		// Left pinky.
		private readonly Vector3 PINKY_JOINT1_L_POS = new Vector3(-0.099f, 0.03f, 0.2f);
		private readonly Vector3 PINKY_JOINT1_L_ROT = new Vector3(0, 0, 28);
		private readonly Vector3 PINKY_JOINT2_L_POS = new Vector3(-0.103f, 0.04f, 0.2f);
		private readonly Vector3 PINKY_JOINT2_L_ROT = new Vector3(0, 0, 28);
		private readonly Vector3 PINKY_JOINT3_L_POS = new Vector3(-0.106f, 0.05f, 0.2f);
		private readonly Vector3 PINKY_JOINT3_L_ROT = new Vector3(0, 0, 28);
		private readonly Vector3 PINKY_TIP_L_POS = new Vector3(-0.109f, 0.06f, 0.2f);
		private readonly Vector3 PINKY_TIP_L_ROT = new Vector3(0, 0, 28);

		// Right wrist.
		private readonly Vector3 HAND_WRIST_R_POS = new Vector3(0.09f, 0, 0.2f);
		private readonly Vector3 HAND_WRIST_R_ROT = new Vector3(7, 0, 15);
		// Right palm.
		private readonly Vector3 HAND_PALM_R_POS = new Vector3(0.08f, 0.02f, 0.2f);
		private readonly Vector3 HAND_PALM_R_ROT = new Vector3(0, 0, 1);
		// Right thumb.
		private readonly Vector3 THUMB_JOINT2_R_POS = new Vector3(0.05f, 0.02f, 0.2f);
		private readonly Vector3 THUMB_JOINT2_R_ROT = new Vector3(0, 0, 42.54f);
		private readonly Vector3 THUMB_JOINT3_R_POS = new Vector3(0.04f, 0.03f, 0.2f);
		private readonly Vector3 THUMB_JOINT3_R_ROT = new Vector3(0, 0, 42.54f);
		private readonly Vector3 THUMB_TIP_R_POS = new Vector3(0.03f, 0.04f, 0.2f);
		private readonly Vector3 THUMB_TIP_R_ROT = new Vector3(0, 0, 42.54f);
		// Right index.
		private readonly Vector3 INDEX_JOINT1_R_POS = new Vector3(0.06f, 0.04f, 0.2f);
		private readonly Vector3 INDEX_JOINT1_R_ROT = new Vector3(0, 0, 16);
		private readonly Vector3 INDEX_JOINT2_R_POS = new Vector3(0.056f, 0.05f, 0.2f);
		private readonly Vector3 INDEX_JOINT2_R_ROT = new Vector3(0, 0, 16);
		private readonly Vector3 INDEX_JOINT3_R_POS = new Vector3(0.052f, 0.06f, 0.2f);
		private readonly Vector3 INDEX_JOINT3_R_ROT = new Vector3(0, 0, 16);
		private readonly Vector3 INDEX_TIP_R_POS = new Vector3(0.048f, 0.07f, 0.2f);
		private readonly Vector3 INDEX_TIP_R_ROT = new Vector3(0, 0, 16);
		// Right middle.
		private readonly Vector3 MIDDLE_JOINT1_R_POS = new Vector3(0.075f, 0.045f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT1_R_ROT = new Vector3(0, 0, 0.87f);
		private readonly Vector3 MIDDLE_JOINT2_R_POS = new Vector3(0.074f, 0.055f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT2_R_ROT = new Vector3(0, 0, 0.87f);
		private readonly Vector3 MIDDLE_JOINT3_R_POS = new Vector3(0.073f, 0.065f, 0.2f);
		private readonly Vector3 MIDDLE_JOINT3_R_ROT = new Vector3(0, 0, 0.87f);
		private readonly Vector3 MIDDLE_TIP_R_POS = new Vector3(0.072f, 0.075f, 0.2f);
		private readonly Vector3 MIDDLE_TIP_R_ROT = new Vector3(0, 0, 0.87f);
		// Right ring.
		private readonly Vector3 RING_JOINT1_R_POS = new Vector3(0.087f, 0.04f, 0.2f);
		private readonly Vector3 RING_JOINT1_R_ROT = new Vector3(0, 0, -12.48f);
		private readonly Vector3 RING_JOINT2_R_POS = new Vector3(0.089f, 0.05f, 0.2f);
		private readonly Vector3 RING_JOINT2_R_ROT = new Vector3(0, 0, -12.48f);
		private readonly Vector3 RING_JOINT3_R_POS = new Vector3(0.091f, 0.06f, 0.2f);
		private readonly Vector3 RING_JOINT3_R_ROT = new Vector3(0, 0, -12.48f);
		private readonly Vector3 RING_TIP_R_POS = new Vector3(0.093f, 0.07f, 0.2f);
		private readonly Vector3 RING_TIP_R_ROT = new Vector3(0, 0, -12.48f);
		// Right pinky.
		private readonly Vector3 PINKY_JOINT1_R_POS = new Vector3(0.099f, 0.03f, 0.2f);
		private readonly Vector3 PINKY_JOINT1_R_ROT = new Vector3(0, 0, -28);
		private readonly Vector3 PINKY_JOINT2_R_POS = new Vector3(0.103f, 0.04f, 0.2f);
		private readonly Vector3 PINKY_JOINT2_R_ROT = new Vector3(0, 0, -28);
		private readonly Vector3 PINKY_JOINT3_R_POS = new Vector3(0.106f, 0.05f, 0.2f);
		private readonly Vector3 PINKY_JOINT3_R_ROT = new Vector3(0, 0, -28);
		private readonly Vector3 PINKY_TIP_R_POS = new Vector3(0.109f, 0.06f, 0.2f);
		private readonly Vector3 PINKY_TIP_R_ROT = new Vector3(0, 0, -28);

		// Left bones.
		private List<WVR_Vector3f_t> leftBonesPosition = new List<WVR_Vector3f_t>();
		private List<WVR_Quatf_t> leftBonesOrientation = new List<WVR_Quatf_t>();
		private WVR_Matrix4f_t leftWristMatrix;
		// Right bones.
		private List<WVR_Vector3f_t> rightBonesPosition = new List<WVR_Vector3f_t>();
		private List<WVR_Quatf_t> rightBonesOrientation = new List<WVR_Quatf_t>();
		private WVR_Matrix4f_t rightWristMatrix;
		private Vector3 leftPinchDirection = new Vector3(0, 0.5f, 1);
		private Vector3 rightPinchDirection = new Vector3(0, 0.5f, 1);

		private void InitializeBonesAndHandTrackingData()
		{
			for (int i = 0; i < (int)HandJointType.BONES_COUNT; i++)
			{
				WVR_Vector3f_t pos;
				pos.v0 = 0;
				pos.v1 = 0;
				pos.v2 = 0;
				leftBonesPosition.Add(pos);
				rightBonesPosition.Add(pos);

				WVR_Quatf_t rot;
				rot.w = 0;
				rot.x = 0;
				rot.y = 0;
				rot.z = 0;
				leftBonesOrientation.Add(rot);
				rightBonesOrientation.Add(rot);
			}
		}

		private readonly Vector3 HAND_L_POS_OFFSET = new Vector3(0, 0, 0.1f);
		private readonly Vector3 HAND_R_POS_OFFSET = new Vector3(0, 0, 0.1f);
		private Vector3 leftYawRotation = new Vector3(0, 0.1f, 0), rightYawRotation = new Vector3(0, -0.1f, 0);
		private Quaternion leftYawOrientation = Quaternion.identity, rightYawOrientation = Quaternion.identity;
		private int boneCount = 0, boneCountAdder = 1;
		private float pinchStrengthLeft = 0, pinchStrengthRight = 0.5f;

		private readonly Vector3 HAND_L_POS__FUSION_OFFSET = new Vector3(0, 0.1f, 0.2f);
		private readonly Vector3 HAND_R_POS__FUSION_OFFSET = new Vector3(0, 0.1f, 0.2f);
		private bool fusionBracelet = false;
		private void UpdateBonesAndHandTrackingData()
		{
			// Move the bone position continuously.
			if (boneCount == 100 || boneCount == -100)
			{
				boneCountAdder *= -1;

				pinchStrengthLeft += .1f;
				pinchStrengthLeft %= 1;

				pinchStrengthRight += .1f;
				pinchStrengthRight %= 1;
			}
			boneCount += boneCountAdder;
			leftYawRotation.y += 0.1f * boneCountAdder;
			leftYawOrientation = Quaternion.Euler(leftYawRotation);
			rightYawRotation.y += -0.1f * boneCountAdder;
			rightYawOrientation = Quaternion.Euler(rightYawRotation);

			Vector3 vec_raw = Vector3.zero; // Raw data position.
			Vector3 vec = Vector3.zero;
			Quaternion qua = Quaternion.identity;

			// Calculate the left bone offset according to the origin.
			Vector3 BONE_HAND_L_POS_OFFSET = fusionBracelet ? HAND_L_POS__FUSION_OFFSET : HAND_L_POS_OFFSET;
			BONE_HAND_L_POS_OFFSET.y = handOrgin == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround ? BONE_HAND_L_POS_OFFSET.y + 1.75f : BONE_HAND_L_POS_OFFSET.y;

			// Left wrist.
			vec_raw = leftYawOrientation * (WRIST_L_POS + BONE_HAND_L_POS_OFFSET);    // Assume raw data with the offset of the origin.
			vec = leftYawOrientation * (WRIST_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(BONE_HAND_WRIST_L_ROT);
			leftBonesPosition[(int)HandJointType.Wrist_L] = GetOpenGLVector(vec_raw);
			leftBonesOrientation[(int)HandJointType.Wrist_L] = GetOpenGLQuaternion(qua);
			leftWristMatrix = GetOpenGLMatrix44(vec, qua);

			// Left palm.
			vec = leftYawOrientation * (PALM_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(HAND_PALM_L_ROT);
			leftBonesPosition[(int)HandJointType.Palm_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Palm_L] = GetOpenGLQuaternion(qua);

			// Left thumb.
			vec = leftYawOrientation * (THUMB_JOIN2_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_JOIN2_L_ROT);
			leftBonesPosition[(int)HandJointType.Thumb_Joint2_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Thumb_Joint2_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (THUMB_JOIN3_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_JOIN3_L_ROT);
			leftBonesPosition[(int)HandJointType.Thumb_Joint3_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Thumb_Joint3_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (THUMB_TIP_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_TIP_L_ROT);
			leftBonesPosition[(int)HandJointType.Thumb_Tip_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Thumb_Tip_L] = GetOpenGLQuaternion(qua);

			// Left index.
			vec = leftYawOrientation * (INDEX_JOINT1_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT1_L_ROT);
			leftBonesPosition[(int)HandJointType.Index_Joint1_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Index_Joint1_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (INDEX_JOINT2_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT2_L_ROT);
			leftBonesPosition[(int)HandJointType.Index_Joint2_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Index_Joint2_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (INDEX_JOINT3_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT3_L_ROT);
			leftBonesPosition[(int)HandJointType.Index_Joint3_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Index_Joint3_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (INDEX_TIP_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_TIP_L_ROT);
			leftBonesPosition[(int)HandJointType.Index_Tip_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Index_Tip_L] = GetOpenGLQuaternion(qua);

			// Left middle.
			vec = leftYawOrientation * (MIDDLE_JOINT1_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT1_L_ROT);
			leftBonesPosition[(int)HandJointType.Middle_Joint1_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Middle_Joint1_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (MIDDLE_JOINT2_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT2_L_ROT);
			leftBonesPosition[(int)HandJointType.Middle_Joint2_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Middle_Joint2_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (MIDDLE_JOINT3_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT3_L_ROT);
			leftBonesPosition[(int)HandJointType.Middle_Joint3_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Middle_Joint3_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (MIDDLE_TIP_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_TIP_L_ROT);
			leftBonesPosition[(int)HandJointType.Middle_Tip_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Middle_Tip_L] = GetOpenGLQuaternion(qua);

			// Left ring.
			vec = leftYawOrientation * (RING_JOINT1_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT1_L_ROT);
			leftBonesPosition[(int)HandJointType.Ring_Joint1_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Ring_Joint1_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (RING_JOINT2_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT2_L_ROT);
			leftBonesPosition[(int)HandJointType.Ring_Joint2_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Ring_Joint2_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (RING_JOINT3_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT3_L_ROT);
			leftBonesPosition[(int)HandJointType.Ring_Joint3_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Ring_Joint3_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (RING_TIP_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(RING_TIP_L_ROT);
			leftBonesPosition[(int)HandJointType.Ring_Tip_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Ring_Tip_L] = GetOpenGLQuaternion(qua);

			// Left pinky.
			vec = leftYawOrientation * (PINKY_JOINT1_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT1_L_ROT);
			leftBonesPosition[(int)HandJointType.Pinky_Joint1_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Pinky_Joint1_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (PINKY_JOINT2_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT2_L_ROT);
			leftBonesPosition[(int)HandJointType.Pinky_Joint2_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Pinky_Joint2_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (PINKY_JOINT3_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT3_L_ROT);
			leftBonesPosition[(int)HandJointType.Pinky_Joint3_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Pinky_Joint3_L] = GetOpenGLQuaternion(qua);

			vec = leftYawOrientation * (PINKY_TIP_L_POS + BONE_HAND_L_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_TIP_L_ROT);
			leftBonesPosition[(int)HandJointType.Pinky_Tip_L] = GetOpenGLVector(vec);
			leftBonesOrientation[(int)HandJointType.Pinky_Tip_L] = GetOpenGLQuaternion(qua);

			// ----------------------------------
			Vector3 BONE_HAND_R_POS_OFFSET = fusionBracelet ? HAND_R_POS__FUSION_OFFSET : HAND_R_POS_OFFSET;
			BONE_HAND_R_POS_OFFSET.y = handOrgin == WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround ? BONE_HAND_R_POS_OFFSET.y + 1.75f : BONE_HAND_R_POS_OFFSET.y;

			// Right wrist.
			vec_raw = rightYawOrientation * (HAND_WRIST_R_POS + BONE_HAND_R_POS_OFFSET);   // Assume raw data with the offset of the origin.
			vec = rightYawOrientation * (HAND_WRIST_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(HAND_WRIST_R_ROT);
			rightBonesPosition[(int)HandJointType.Wrist_R] = GetOpenGLVector(vec_raw);
			rightBonesOrientation[(int)HandJointType.Wrist_R] = GetOpenGLQuaternion(qua);
			rightWristMatrix = GetOpenGLMatrix44(vec, qua);

			// Right palm.
			vec = rightYawOrientation * (HAND_PALM_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(HAND_PALM_R_ROT);
			rightBonesPosition[(int)HandJointType.Palm_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Palm_R] = GetOpenGLQuaternion(qua);

			// Right thumb.
			vec = rightYawOrientation * (THUMB_JOINT2_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_JOINT2_R_ROT);
			rightBonesPosition[(int)HandJointType.Thumb_Joint2_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Thumb_Joint2_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (THUMB_JOINT3_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_JOINT3_R_ROT);
			rightBonesPosition[(int)HandJointType.Thumb_Joint3_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Thumb_Joint3_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (THUMB_TIP_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(THUMB_TIP_R_ROT);
			rightBonesPosition[(int)HandJointType.Thumb_Tip_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Thumb_Tip_R] = GetOpenGLQuaternion(qua);

			// Right index.
			vec = rightYawOrientation * (INDEX_JOINT1_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT1_R_ROT);
			rightBonesPosition[(int)HandJointType.Index_Joint1_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Index_Joint1_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (INDEX_JOINT2_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT2_R_ROT);
			rightBonesPosition[(int)HandJointType.Index_Joint2_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Index_Joint2_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (INDEX_JOINT3_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_JOINT3_R_ROT);
			rightBonesPosition[(int)HandJointType.Index_Joint3_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Index_Joint3_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (INDEX_TIP_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(INDEX_TIP_R_ROT);
			rightBonesPosition[(int)HandJointType.Index_Tip_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Index_Tip_R] = GetOpenGLQuaternion(qua);

			// Right middle.
			vec = rightYawOrientation * (MIDDLE_JOINT1_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT1_R_ROT);
			rightBonesPosition[(int)HandJointType.Middle_Joint1_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Middle_Joint1_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (MIDDLE_JOINT2_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT2_R_ROT);
			rightBonesPosition[(int)HandJointType.Middle_Joint2_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Middle_Joint2_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (MIDDLE_JOINT3_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_JOINT3_R_ROT);
			rightBonesPosition[(int)HandJointType.Middle_Joint3_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Middle_Joint3_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (MIDDLE_TIP_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(MIDDLE_TIP_R_ROT);
			rightBonesPosition[(int)HandJointType.Middle_Tip_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Middle_Tip_R] = GetOpenGLQuaternion(qua);

			// Right ring.
			vec = rightYawOrientation * (RING_JOINT1_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT1_R_ROT);
			rightBonesPosition[(int)HandJointType.Ring_Joint1_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Ring_Joint1_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (RING_JOINT2_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT2_R_ROT);
			rightBonesPosition[(int)HandJointType.Ring_Joint2_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Ring_Joint2_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (RING_JOINT3_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(RING_JOINT3_R_ROT);
			rightBonesPosition[(int)HandJointType.Ring_Joint3_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Ring_Joint3_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (RING_TIP_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(RING_TIP_R_ROT);
			rightBonesPosition[(int)HandJointType.Ring_Tip_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Ring_Tip_R] = GetOpenGLQuaternion(qua);

			// Right pinky.
			vec = rightYawOrientation * (PINKY_JOINT1_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT1_R_ROT);
			rightBonesPosition[(int)HandJointType.Pinky_Joint1_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Pinky_Joint1_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (PINKY_JOINT2_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT2_R_ROT);
			rightBonesPosition[(int)HandJointType.Pinky_Joint2_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Pinky_Joint2_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (PINKY_JOINT3_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_JOINT3_R_ROT);
			rightBonesPosition[(int)HandJointType.Pinky_Joint3_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Pinky_Joint3_R] = GetOpenGLQuaternion(qua);

			vec = rightYawOrientation * (PINKY_TIP_R_POS + BONE_HAND_R_POS_OFFSET);
			qua = Quaternion.Euler(PINKY_TIP_R_ROT);
			rightBonesPosition[(int)HandJointType.Pinky_Tip_R] = GetOpenGLVector(vec);
			rightBonesOrientation[(int)HandJointType.Pinky_Tip_R] = GetOpenGLQuaternion(qua);

			// ----------------------------------
		}
		#endregion

		#region Hand Gesture
		private ulong m_GestureSupportedMask = (ulong)(
			  (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Invalid)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Unknown)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Fist)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Five)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_OK)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_ThumbUp)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_IndexUp)
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch
			| (1 << (int)WVR_HandGestureType.WVR_HandGestureType_Yeah))
			);
		public WVR_Result GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
		{
			info.supportedMask = m_GestureSupportedMask;
			return WVR_Result.WVR_Success;
		}

		private bool isHandGestureEnabled = false;
		private ulong gestureDemands = 0;
		public WVR_Result StartHandGesture(ulong demands)
		{
			DEBUG("StartHandGesture() " + demands);
			isHandGestureEnabled = true;
			gestureDemands = demands;
			return WVR_Result.WVR_Success;
		}
		public void StopHandGesture()
		{
			isHandGestureEnabled = false;
		}
		public WVR_Result GetHandGestureData(ref WVR_HandGestureData_t data)
		{
			if (isHandGestureEnabled)
			{
				data.timestamp = Time.frameCount;
				data.right = s_GestureTypes[gestureTypeIndex];
				data.left = WVR_HandGestureType.WVR_HandGestureType_Invalid;
			}
			else
			{
				data.timestamp = 0;
				data.right = WVR_HandGestureType.WVR_HandGestureType_Invalid;
				data.left = WVR_HandGestureType.WVR_HandGestureType_Invalid;
			}
			return WVR_Result.WVR_Success;
		}
		#endregion

		#region Hand Tracker (v2 API)
		private bool isNaturalHandEnabled = false, isElectronicHandEnabled = false;
		public WVR_Result StartHandTracking(WVR_HandTrackerType tracker)
		{
			//if (interactionMode != WVR_InteractionMode.WVR_InteractionMode_Hand) { return WVR_Result.WVR_Error_FeatureNotSupport; }

			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Natural) { isNaturalHandEnabled = true; }
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Electronic) { isElectronicHandEnabled = true; }

			return WVR_Result.WVR_Success;
		}
		public void StopHandTracking(WVR_HandTrackerType tracker)
		{
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Natural)
				isNaturalHandEnabled = false;
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Electronic)
				isElectronicHandEnabled = false;
		}

		const uint kJointCountNatural = 22, kJointCountElectronic = 22;
		private uint GetJointCount(WVR_HandTrackerType tracker)
		{
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Natural && isNaturalHandEnabled)
				return kJointCountNatural;
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Electronic && isElectronicHandEnabled)
				return kJointCountElectronic;

			return 0;
		}
		public WVR_Result GetHandJointCount(WVR_HandTrackerType tracker, ref uint jointCount)
		{
			jointCount = GetJointCount(tracker);
			return WVR_Result.WVR_Success;
		}

		private WVR_HandTrackerInfo_t handTrackerInfo = new WVR_HandTrackerInfo_t();
		const ulong kModelTypeBitMask = (ulong)(WVR_HandModelType.WVR_HandModelType_WithoutController/* | WVR_HandModelType.WVR_HandModelType_WithController*/);
		private WVR_HandJoint[] s_HandJoints;
		private int[] s_intHandJoints;
		const ulong kJointValidFlag = (ulong)(WVR_HandJointValidFlag.WVR_HandJointValidFlag_PositionValid | WVR_HandJointValidFlag.WVR_HandJointValidFlag_RotationValid);
		private ulong[] s_HandJointsFlag;
		private byte[] s_byteHandJointsFlag;
		private void FillHandTrackerInfo(WVR_HandTrackerType tracker)
		{
			handTrackerInfo.jointCount = GetJointCount(tracker);
			handTrackerInfo.handModelTypeBitMask = kModelTypeBitMask;

			/// WVR_HandTrackerInfo_t.jointMappingArray
			s_HandJoints = new WVR_HandJoint[]
			{
			WVR_HandJoint.WVR_HandJoint_Wrist,

			WVR_HandJoint.WVR_HandJoint_Thumb_Joint0,
			WVR_HandJoint.WVR_HandJoint_Thumb_Joint1,
			WVR_HandJoint.WVR_HandJoint_Thumb_Joint2,
			WVR_HandJoint.WVR_HandJoint_Thumb_Tip,

			WVR_HandJoint.WVR_HandJoint_Index_Joint1,
			WVR_HandJoint.WVR_HandJoint_Index_Joint2,
			WVR_HandJoint.WVR_HandJoint_Index_Joint3,
			WVR_HandJoint.WVR_HandJoint_Index_Tip,

			WVR_HandJoint.WVR_HandJoint_Middle_Joint1,
			WVR_HandJoint.WVR_HandJoint_Middle_Joint2,
			WVR_HandJoint.WVR_HandJoint_Middle_Joint3,
			WVR_HandJoint.WVR_HandJoint_Middle_Tip,

			WVR_HandJoint.WVR_HandJoint_Ring_Joint1,
			WVR_HandJoint.WVR_HandJoint_Ring_Joint2,
			WVR_HandJoint.WVR_HandJoint_Ring_Joint3,
			WVR_HandJoint.WVR_HandJoint_Ring_Tip,

			WVR_HandJoint.WVR_HandJoint_Pinky_Joint1,
			WVR_HandJoint.WVR_HandJoint_Pinky_Joint2,
			WVR_HandJoint.WVR_HandJoint_Pinky_Joint3,
			WVR_HandJoint.WVR_HandJoint_Pinky_Tip,

			WVR_HandJoint.WVR_HandJoint_Palm,
			};

			s_intHandJoints = new int[s_HandJoints.Length];
			s_intHandJoints = Array.ConvertAll(s_HandJoints, delegate (WVR_HandJoint value) { return (int)value; });
			handTrackerInfo.jointMappingArray = Marshal.AllocHGlobal(sizeof(int) * s_intHandJoints.Length);
			Marshal.Copy(s_intHandJoints, 0, handTrackerInfo.jointMappingArray, s_intHandJoints.Length);
			/*unsafe
			{
				fixed(WVR_HandJoint* pHandJoints = s_HandJoints)
				{
					handTrackerInfo.jointMappingArray = pHandJoints;
				}
			}*/

			/// WVR_HandTrackerInfo_t.jointValidFlagArray
			s_HandJointsFlag = new ulong[]
			{
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Wrist,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Thumb_Joint0,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Thumb_Joint1,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Thumb_Joint2,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Thumb_Tip,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Index_Joint1,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Index_Joint2,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Index_Joint3,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Index_Tip,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Middle_Joint1,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Middle_Joint2,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Middle_Joint3,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Middle_Tip,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Ring_Joint1,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Ring_Joint2,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Ring_Joint3,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Ring_Tip,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Pinky_Joint1,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Pinky_Joint2,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Pinky_Joint3,
			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Pinky_Tip,

			kJointValidFlag, //WVR_HandJoint.WVR_HandJoint_Palm,
			};

			int byteBufferLength = Buffer.ByteLength(s_HandJointsFlag);
			s_byteHandJointsFlag = new byte[byteBufferLength];
			Buffer.BlockCopy(s_HandJointsFlag, 0, s_byteHandJointsFlag, 0, byteBufferLength);

			handTrackerInfo.jointValidFlagArray = Marshal.AllocHGlobal(sizeof(byte) * byteBufferLength);
			Marshal.Copy(s_byteHandJointsFlag, 0, handTrackerInfo.jointValidFlagArray, byteBufferLength);
			/*unsafe
			{
				fixed (ulong* pHandJointsFlag = s_HandJointsFlag)
				{
					handTrackerInfo.jointValidFlagArray = pHandJointsFlag;
				}
			}*/

			/*ulong ulong_type = default(ulong);
			handTrackerInfo.jointValidFlagArray = Marshal.AllocHGlobal(Marshal.SizeOf(ulong_type) * (int)handTrackerInfo.jointCount);

			long offset = 0;
			if (IntPtr.Size == 4)
				offset = handTrackerInfo.jointValidFlagArray.ToInt32();
			else
				offset = handTrackerInfo.jointValidFlagArray.ToInt64();

			for (int i = 0; i < s_HandJointsFlag.Length; i++)
			{
				IntPtr ulong_ptr = new IntPtr(offset);
				Marshal.StructureToPtr(s_HandJointsFlag[i], ulong_ptr, false);
				offset += Marshal.SizeOf(ulong_type);
			}*/

			handTrackerInfo.pinchThreshold = 0.51f;
			handTrackerInfo.pinchOff = 0.2f;
		}
		public WVR_Result GetHandTrackerInfo(WVR_HandTrackerType tracker, ref WVR_HandTrackerInfo_t info)
		{
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Electronic && !isElectronicHandEnabled)
				return WVR_Result.WVR_Error_FeatureNotSupport;
			if (tracker == WVR_HandTrackerType.WVR_HandTrackerType_Natural && !isNaturalHandEnabled)
				return WVR_Result.WVR_Error_FeatureNotSupport;

			FillHandTrackerInfo(tracker);
			info = handTrackerInfo;
			return WVR_Result.WVR_Success;
		}

		private WVR_Pose_t[] m_JointsPose = new WVR_Pose_t[kJointCountNatural];
		private void FillHandJointData(WVR_HandTrackerType tracker, ref WVR_HandJointData_t handJointData, bool isLeft)
		{
			handJointData.isValidPose = true;
			handJointData.confidence = 0.8f;
			handJointData.jointCount = GetJointCount(tracker);
			handJointData.scale.v0 = .6f;
			handJointData.scale.v1 = .6f;
			handJointData.scale.v2 = -.6f;
			handJointData.wristLinearVelocity.v0 = .05f;
			handJointData.wristLinearVelocity.v1 = .05f;
			handJointData.wristLinearVelocity.v2 = -.05f;
			handJointData.wristAngularVelocity.v0 = .01f;
			handJointData.wristAngularVelocity.v1 = .01f;
			handJointData.wristAngularVelocity.v2 = -.01f;

			WVR_Pose_t wvr_pose_type = default(WVR_Pose_t);
			handJointData.joints = Marshal.AllocHGlobal(Marshal.SizeOf(wvr_pose_type) * (int)handJointData.jointCount);

			if (m_JointsPose.Length != handJointData.jointCount)
			{
				m_JointsPose = new WVR_Pose_t[handJointData.jointCount];
			}
			if (isLeft)
			{
				m_JointsPose[0].position = leftBonesPosition[(int)HandJointType.Wrist_L];
				m_JointsPose[0].rotation = leftBonesOrientation[(int)HandJointType.Wrist_L];

				m_JointsPose[1].position = leftBonesPosition[(int)HandJointType.Thumb_Joint1_L];
				m_JointsPose[1].rotation = leftBonesOrientation[(int)HandJointType.Thumb_Joint1_L];

				m_JointsPose[2].position = leftBonesPosition[(int)HandJointType.Thumb_Joint2_L];
				m_JointsPose[2].rotation = leftBonesOrientation[(int)HandJointType.Thumb_Joint2_L];

				m_JointsPose[3].position = leftBonesPosition[(int)HandJointType.Thumb_Joint3_L];
				m_JointsPose[3].rotation = leftBonesOrientation[(int)HandJointType.Thumb_Joint3_L];

				m_JointsPose[4].position = leftBonesPosition[(int)HandJointType.Thumb_Tip_L];
				m_JointsPose[4].rotation = leftBonesOrientation[(int)HandJointType.Thumb_Tip_L];

				m_JointsPose[5].position = leftBonesPosition[(int)HandJointType.Index_Joint1_L];
				m_JointsPose[5].rotation = leftBonesOrientation[(int)HandJointType.Index_Joint1_L];

				m_JointsPose[6].position = leftBonesPosition[(int)HandJointType.Index_Joint2_L];
				m_JointsPose[6].rotation = leftBonesOrientation[(int)HandJointType.Index_Joint2_L];

				m_JointsPose[7].position = leftBonesPosition[(int)HandJointType.Index_Joint3_L];
				m_JointsPose[7].rotation = leftBonesOrientation[(int)HandJointType.Index_Joint3_L];

				m_JointsPose[8].position = leftBonesPosition[(int)HandJointType.Index_Tip_L];
				m_JointsPose[8].rotation = leftBonesOrientation[(int)HandJointType.Index_Tip_L];

				m_JointsPose[9].position = leftBonesPosition[(int)HandJointType.Middle_Joint1_L];
				m_JointsPose[9].rotation = leftBonesOrientation[(int)HandJointType.Middle_Joint1_L];

				m_JointsPose[10].position = leftBonesPosition[(int)HandJointType.Middle_Joint2_L];
				m_JointsPose[10].rotation = leftBonesOrientation[(int)HandJointType.Middle_Joint2_L];

				m_JointsPose[11].position = leftBonesPosition[(int)HandJointType.Middle_Joint3_L];
				m_JointsPose[11].rotation = leftBonesOrientation[(int)HandJointType.Middle_Joint3_L];

				m_JointsPose[12].position = leftBonesPosition[(int)HandJointType.Middle_Tip_L];
				m_JointsPose[12].rotation = leftBonesOrientation[(int)HandJointType.Middle_Tip_L];

				m_JointsPose[13].position = leftBonesPosition[(int)HandJointType.Ring_Joint1_L];
				m_JointsPose[13].rotation = leftBonesOrientation[(int)HandJointType.Ring_Joint1_L];

				m_JointsPose[14].position = leftBonesPosition[(int)HandJointType.Ring_Joint2_L];
				m_JointsPose[14].rotation = leftBonesOrientation[(int)HandJointType.Ring_Joint2_L];

				m_JointsPose[15].position = leftBonesPosition[(int)HandJointType.Ring_Joint3_L];
				m_JointsPose[15].rotation = leftBonesOrientation[(int)HandJointType.Ring_Joint3_L];

				m_JointsPose[16].position = leftBonesPosition[(int)HandJointType.Ring_Tip_L];
				m_JointsPose[16].rotation = leftBonesOrientation[(int)HandJointType.Ring_Tip_L];

				m_JointsPose[17].position = leftBonesPosition[(int)HandJointType.Pinky_Joint1_L];
				m_JointsPose[17].rotation = leftBonesOrientation[(int)HandJointType.Pinky_Joint1_L];

				m_JointsPose[18].position = leftBonesPosition[(int)HandJointType.Pinky_Joint2_L];
				m_JointsPose[18].rotation = leftBonesOrientation[(int)HandJointType.Pinky_Joint2_L];

				m_JointsPose[19].position = leftBonesPosition[(int)HandJointType.Pinky_Joint3_L];
				m_JointsPose[19].rotation = leftBonesOrientation[(int)HandJointType.Pinky_Joint3_L];

				m_JointsPose[20].position = leftBonesPosition[(int)HandJointType.Pinky_Tip_L];
				m_JointsPose[20].rotation = leftBonesOrientation[(int)HandJointType.Pinky_Tip_L];

				m_JointsPose[21].position = leftBonesPosition[(int)HandJointType.Palm_L];
				m_JointsPose[21].rotation = leftBonesOrientation[(int)HandJointType.Palm_L];
			}
			else
			{
				m_JointsPose[0].position = rightBonesPosition[(int)HandJointType.Wrist_R];
				m_JointsPose[0].rotation = rightBonesOrientation[(int)HandJointType.Wrist_R];

				m_JointsPose[1].position = rightBonesPosition[(int)HandJointType.Thumb_Joint1_R];
				m_JointsPose[1].rotation = rightBonesOrientation[(int)HandJointType.Thumb_Joint1_R];

				m_JointsPose[2].position = rightBonesPosition[(int)HandJointType.Thumb_Joint2_R];
				m_JointsPose[2].rotation = rightBonesOrientation[(int)HandJointType.Thumb_Joint2_R];

				m_JointsPose[3].position = rightBonesPosition[(int)HandJointType.Thumb_Joint3_R];
				m_JointsPose[3].rotation = rightBonesOrientation[(int)HandJointType.Thumb_Joint3_R];

				m_JointsPose[4].position = rightBonesPosition[(int)HandJointType.Thumb_Tip_R];
				m_JointsPose[4].rotation = rightBonesOrientation[(int)HandJointType.Thumb_Tip_R];

				m_JointsPose[5].position = rightBonesPosition[(int)HandJointType.Index_Joint1_R];
				m_JointsPose[5].rotation = rightBonesOrientation[(int)HandJointType.Index_Joint1_R];

				m_JointsPose[6].position = rightBonesPosition[(int)HandJointType.Index_Joint2_R];
				m_JointsPose[6].rotation = rightBonesOrientation[(int)HandJointType.Index_Joint2_R];

				m_JointsPose[7].position = rightBonesPosition[(int)HandJointType.Index_Joint3_R];
				m_JointsPose[7].rotation = rightBonesOrientation[(int)HandJointType.Index_Joint3_R];

				m_JointsPose[8].position = rightBonesPosition[(int)HandJointType.Index_Tip_R];
				m_JointsPose[8].rotation = rightBonesOrientation[(int)HandJointType.Index_Tip_R];

				m_JointsPose[9].position = rightBonesPosition[(int)HandJointType.Middle_Joint1_R];
				m_JointsPose[9].rotation = rightBonesOrientation[(int)HandJointType.Middle_Joint1_R];

				m_JointsPose[10].position = rightBonesPosition[(int)HandJointType.Middle_Joint2_R];
				m_JointsPose[10].rotation = rightBonesOrientation[(int)HandJointType.Middle_Joint2_R];

				m_JointsPose[11].position = rightBonesPosition[(int)HandJointType.Middle_Joint3_R];
				m_JointsPose[11].rotation = rightBonesOrientation[(int)HandJointType.Middle_Joint3_R];

				m_JointsPose[12].position = rightBonesPosition[(int)HandJointType.Middle_Tip_R];
				m_JointsPose[12].rotation = rightBonesOrientation[(int)HandJointType.Middle_Tip_R];

				m_JointsPose[13].position = rightBonesPosition[(int)HandJointType.Ring_Joint1_R];
				m_JointsPose[13].rotation = rightBonesOrientation[(int)HandJointType.Ring_Joint1_R];

				m_JointsPose[14].position = rightBonesPosition[(int)HandJointType.Ring_Joint2_R];
				m_JointsPose[14].rotation = rightBonesOrientation[(int)HandJointType.Ring_Joint2_R];

				m_JointsPose[15].position = rightBonesPosition[(int)HandJointType.Ring_Joint3_R];
				m_JointsPose[15].rotation = rightBonesOrientation[(int)HandJointType.Ring_Joint3_R];

				m_JointsPose[16].position = rightBonesPosition[(int)HandJointType.Ring_Tip_R];
				m_JointsPose[16].rotation = rightBonesOrientation[(int)HandJointType.Ring_Tip_R];

				m_JointsPose[17].position = rightBonesPosition[(int)HandJointType.Pinky_Joint1_R];
				m_JointsPose[17].rotation = rightBonesOrientation[(int)HandJointType.Pinky_Joint1_R];

				m_JointsPose[18].position = rightBonesPosition[(int)HandJointType.Pinky_Joint2_R];
				m_JointsPose[18].rotation = rightBonesOrientation[(int)HandJointType.Pinky_Joint2_R];

				m_JointsPose[19].position = rightBonesPosition[(int)HandJointType.Pinky_Joint3_R];
				m_JointsPose[19].rotation = rightBonesOrientation[(int)HandJointType.Pinky_Joint3_R];

				m_JointsPose[20].position = rightBonesPosition[(int)HandJointType.Pinky_Tip_R];
				m_JointsPose[20].rotation = rightBonesOrientation[(int)HandJointType.Pinky_Tip_R];

				m_JointsPose[21].position = rightBonesPosition[(int)HandJointType.Palm_R];
				m_JointsPose[21].rotation = rightBonesOrientation[(int)HandJointType.Palm_R];
			}

			long offset = 0;
			if (IntPtr.Size == 4)
				offset = handJointData.joints.ToInt32();
			else
				offset = handJointData.joints.ToInt64();

			for (int i = 0; i < m_JointsPose.Length; i++)
			{
				IntPtr wvr_pose_ptr = new IntPtr(offset);
				Marshal.StructureToPtr(m_JointsPose[i], wvr_pose_ptr, false);
				offset += Marshal.SizeOf(wvr_pose_type);
			}
		}
		private WVR_HandTrackingData_t m_HandTrackerData = new WVR_HandTrackingData_t();
		private WVR_HandJointData_t handJointDataLeft = new WVR_HandJointData_t();
		private WVR_HandJointData_t handJointDataRight = new WVR_HandJointData_t();
		private void FillHandTrackerData(WVR_HandTrackerType tracker)
		{
			m_HandTrackerData.timestamp = Time.frameCount;

			FillHandJointData(tracker, ref handJointDataLeft, true);
			m_HandTrackerData.left = handJointDataLeft;

			FillHandJointData(tracker, ref handJointDataRight, false);
			m_HandTrackerData.right = handJointDataRight;
		}

		private WVR_HandPosePinchState_t handPinchStateLeft = new WVR_HandPosePinchState_t();
		private WVR_HandPosePinchState_t handPinchStateRight = new WVR_HandPosePinchState_t();
		private WVR_HandPoseHoldState_t handHoldStateLeft = new WVR_HandPoseHoldState_t();
		private WVR_HandPoseHoldState_t handHoldStateRight = new WVR_HandPoseHoldState_t();
		private void UpdateHandPoseData()
		{
			handPinchStateLeft.state.type = WVR_HandPoseType.WVR_HandPoseType_Pinch;
			handPinchStateLeft.strength = pinchStrengthLeft;
			handPinchStateLeft.origin = leftBonesPosition[(int)HandJointType.Wrist_L];
			handPinchStateLeft.direction = GetOpenGLVector(leftPinchDirection);

			handPinchStateRight.state.type = WVR_HandPoseType.WVR_HandPoseType_Pinch;
			handPinchStateRight.finger = WVR_FingerType.WVR_FingerType_Index;
			handPinchStateRight.strength = pinchStrengthRight;
			handPinchStateRight.origin = rightBonesPosition[(int)HandJointType.Wrist_R];
			handPinchStateRight.direction = GetOpenGLVector(rightPinchDirection);

			handHoldStateLeft.state.type = WVR_HandPoseType.WVR_HandPoseType_Hold;
			handHoldStateLeft.role = WVR_HandHoldRoleType.WVR_HandHoldRoleType_SideHold;
			handHoldStateLeft.type = WVR_HandHoldObjectType.WVR_HandHoldObjectType_Gun;

			handHoldStateRight.state.type = WVR_HandPoseType.WVR_HandPoseType_Hold;
			handHoldStateRight.role = WVR_HandHoldRoleType.WVR_HandHoldRoleType_MainHold;
			handHoldStateRight.type = WVR_HandHoldObjectType.WVR_HandHoldObjectType_Gun;
		}
		private WVR_HandPoseData_t m_HandPoseData = new WVR_HandPoseData_t();
		private void FillHandPoseData(WVR_HandPoseType poseType)
		{
			UpdateHandPoseData();

			m_HandPoseData.timestamp = Time.frameCount;
			m_HandPoseData.left.state.type = poseType;
			m_HandPoseData.right.state.type = poseType;

			// WVR_HandPoseState_t is an union, should only use the data of a pose type.
			switch (poseType)
			{
				case WVR_HandPoseType.WVR_HandPoseType_Pinch:
					m_HandPoseData.left.pinch = handPinchStateLeft;
					m_HandPoseData.right.pinch = handPinchStateRight;
					break;
				case WVR_HandPoseType.WVR_HandPoseType_Hold:
					m_HandPoseData.left.hold = handHoldStateLeft;
					m_HandPoseData.right.hold = handHoldStateRight;
					break;
				default:
					break;
			}
		}

		// Used for different joints poses.
		//private WVR_HandModelType m_HandModelType = WVR_HandModelType.WVR_HandModelType_WithController;
		public WVR_Result GetHandTrackingData(
					WVR_HandTrackerType trackerType,
					WVR_HandModelType modelType,
					WVR_PoseOriginModel originModel,
					ref WVR_HandTrackingData_t handTrackerData,
					ref WVR_HandPoseData_t handPoseData)
		{
			if (trackerType == WVR_HandTrackerType.WVR_HandTrackerType_Electronic && !isElectronicHandEnabled)
				return WVR_Result.WVR_Error_FeatureNotSupport;
			if (trackerType == WVR_HandTrackerType.WVR_HandTrackerType_Natural && !isNaturalHandEnabled)
				return WVR_Result.WVR_Error_FeatureNotSupport;

			//m_HandModelType = modelType;
			handOrgin = originModel;

			FillHandTrackerData(trackerType);
			handTrackerData = m_HandTrackerData;

			/// Fills WVR_HandPoseData_t
			FillHandPoseData(WVR_HandPoseType.WVR_HandPoseType_Pinch);
			handPoseData = m_HandPoseData;

			return WVR_Result.WVR_Success;
		}

		public bool ControllerSupportElectronicHand() { return true; }

		public void EnhanceHandStable(bool wear) { fusionBracelet = wear; }
		public bool IsEnhanceHandStable() { return fusionBracelet; }

		public void SetMixMode(bool enable)
		{
			DEBUG("SetMixMode() " + enable);
		}
		#endregion
		#endregion

		#region Lip Expression
		private readonly WVR_LipExpression[] s_LipExpressions = new WVR_LipExpression[]
		{
			WVR_LipExpression.WVR_LipExpression_Jaw_Right,
			WVR_LipExpression.WVR_LipExpression_Jaw_Left,
			WVR_LipExpression.WVR_LipExpression_Jaw_Forward,
			WVR_LipExpression.WVR_LipExpression_Jaw_Open,
			WVR_LipExpression.WVR_LipExpression_Mouth_Ape_Shape,
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Right,		// 5
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Left,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Right,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Left,
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Overturn,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Overturn,	// 10
			WVR_LipExpression.WVR_LipExpression_Mouth_Pout,
			WVR_LipExpression.WVR_LipExpression_Mouth_Smile_Right,
			WVR_LipExpression.WVR_LipExpression_Mouth_Smile_Left,
			WVR_LipExpression.WVR_LipExpression_Mouth_Sad_Right,
			WVR_LipExpression.WVR_LipExpression_Mouth_Sad_Left,			// 15
			WVR_LipExpression.WVR_LipExpression_Cheek_Puff_Right,
			WVR_LipExpression.WVR_LipExpression_Cheek_Puff_Left,
			WVR_LipExpression.WVR_LipExpression_Cheek_Suck,
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Upright,
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Upleft,		// 20
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Downright,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Downleft,
			WVR_LipExpression.WVR_LipExpression_Mouth_Upper_Inside,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Inside,
			WVR_LipExpression.WVR_LipExpression_Mouth_Lower_Overlay,	// 25
			WVR_LipExpression.WVR_LipExpression_Tongue_Longstep1,
			WVR_LipExpression.WVR_LipExpression_Tongue_Left,
			WVR_LipExpression.WVR_LipExpression_Tongue_Right,
			WVR_LipExpression.WVR_LipExpression_Tongue_Up,
			WVR_LipExpression.WVR_LipExpression_Tongue_Down,			// 30
			WVR_LipExpression.WVR_LipExpression_Tongue_Roll,
			WVR_LipExpression.WVR_LipExpression_Tongue_Longstep2,
			WVR_LipExpression.WVR_LipExpression_Tongue_Upright_Morph,
			WVR_LipExpression.WVR_LipExpression_Tongue_Upleft_Morph,
			WVR_LipExpression.WVR_LipExpression_Tongue_Downright_Morph,	// 35
			WVR_LipExpression.WVR_LipExpression_Tongue_Downleft_Morph,
		};
		private readonly float[] s_LipExpValues = new float[]
		{
			.1f, .2f, .3f, .4f, .5f, .6f, .7f, .8f, .9f
		};

		private bool m_LipExpEnabled = false;
		public WVR_Result StartLipExp()
		{
			if (!m_LipExpEnabled)
			{
				m_LipExpEnabled = true;
				DEBUG("StartLipExp()");
				return WVR_Result.WVR_Success;
			}
			return WVR_Result.WVR_Error_FeatureNotSupport;
		}
		public WVR_Result GetLipExpData([In, Out] float[] value)
		{
			if (!m_LipExpEnabled) { return WVR_Result.WVR_Error_FeatureNotSupport; }

			int size = Math.Min(value.Length, s_LipExpValues.Length);
			for (int i = 0; i < size; i++)
				value[i] = s_LipExpValues[i];

			return WVR_Result.WVR_Success;
		}
		public void StopLipExp()
		{
			m_LipExpEnabled = false;
		}
		#endregion

		#region Controller Pose Mode
		WVR_ControllerPoseMode m_RightControllerPoseMode = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw, m_LeftControllerPoseMode = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw;
		Vector3 triggerModeRotationOffset = new Vector3(45, 0, 0);
		Vector3 panelModeRotationOffset = new Vector3(0, 0, 0);
		Vector3 handleModeRotationOffset = new Vector3(-30, 0, 0);
		public bool GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
		{
			translation = GetOpenGLVector(Vector3.zero);

			switch (mode)
			{
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger:
					{
						Quaternion rotation_offset = Quaternion.Euler(triggerModeRotationOffset);
						quaternion = GetOpenGLQuaternion(rotation_offset);
					}
					break;
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel:
					{
						Quaternion rotation_offset = Quaternion.Euler(panelModeRotationOffset);
						quaternion = GetOpenGLQuaternion(rotation_offset);
					}
					break;
				case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle:
					{
						Quaternion rotation_offset = Quaternion.Euler(handleModeRotationOffset);
						quaternion = GetOpenGLQuaternion(rotation_offset);
					}
					break;
				default:
					break;
			}

			return true;
		}
		public bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				m_RightControllerPoseMode = mode;
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				m_LeftControllerPoseMode = mode;
			return true;
		}
		public bool GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				mode = m_RightControllerPoseMode;
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				mode = m_LeftControllerPoseMode;
			return true;
		}
		#endregion

		#region Bracelet
		private WVR_TrackerCapabilities m_BraceletCaps;
		private WVR_PoseState_t m_BraceletPoseLeft, m_BraceletPoseRight;
		private Int32 m_TrackerInputButton0 = (Int32)(
			WVR_InputId.WVR_InputId_Alias1_System |
			WVR_InputId.WVR_InputId_Alias1_A);
		private Int32 m_TrackerInputButton1 = (Int32)(
			WVR_InputId.WVR_InputId_Alias1_Menu |
			WVR_InputId.WVR_InputId_Alias1_X);
		private Dictionary<WVR_InputId, WVR_AnalogType> s_BraceletAnalogTypes = new Dictionary<WVR_InputId, WVR_AnalogType>()
		{
			{ WVR_InputId.WVR_InputId_Alias1_Grip, WVR_AnalogType.WVR_AnalogType_1D },
			{ WVR_InputId.WVR_InputId_Alias1_Touchpad, WVR_AnalogType.WVR_AnalogType_2D },
			{ WVR_InputId.WVR_InputId_Alias1_Trigger, WVR_AnalogType.WVR_AnalogType_1D },
			{ WVR_InputId.WVR_InputId_Alias1_Thumbstick, WVR_AnalogType.WVR_AnalogType_2D },
		};
		private void InitTracker()
		{
			/// Bracelet
			m_BraceletCaps.supportsBatteryLevel = true;
			m_BraceletCaps.supportsHapticVibration = true;
			m_BraceletCaps.supportsInputDevice = true;
			m_BraceletCaps.supportsOrientationTracking = true;
			m_BraceletCaps.supportsPositionTracking = true;
		}
		private void UpdateTracker()
		{
			m_BraceletPoseRight.IsValidPose = true;
			m_BraceletPoseRight.PoseMatrix = rightPoseMatrix;
			m_BraceletPoseRight.Velocity.v0 = 0.1f;
			m_BraceletPoseRight.Velocity.v1 = 0.0f;
			m_BraceletPoseRight.Velocity.v2 = 0.0f;
			m_BraceletPoseRight.AngularVelocity.v0 = 0.1f;
			m_BraceletPoseRight.AngularVelocity.v1 = 0.1f;
			m_BraceletPoseRight.AngularVelocity.v2 = 0.1f;
			m_BraceletPoseRight.Is6DoFPose = is6DoFPose;
			m_BraceletPoseRight.OriginModel = hmdOriginModel;

			m_BraceletPoseLeft.IsValidPose = true;
			m_BraceletPoseLeft.PoseMatrix = leftPoseMatrix;
			m_BraceletPoseLeft.Velocity.v0 = 0.1f;
			m_BraceletPoseLeft.Velocity.v1 = 0.0f;
			m_BraceletPoseLeft.Velocity.v2 = 0.0f;
			m_BraceletPoseLeft.AngularVelocity.v0 = 0.1f;
			m_BraceletPoseLeft.AngularVelocity.v1 = 0.1f;
			m_BraceletPoseLeft.AngularVelocity.v2 = 0.1f;
			m_BraceletPoseLeft.Is6DoFPose = is6DoFPose;
			m_BraceletPoseLeft.OriginModel = hmdOriginModel;
		}

		bool m_TrackerEnabled = false;
		public WVR_Result StartTracker()
		{
			m_TrackerEnabled = true;
			return WVR_Result.WVR_Success;
		}
		public void StopTracker()
		{
			m_TrackerEnabled = false;
		}
		public bool IsTrackerConnected(WVR_TrackerId trackerId)
		{
			//if (trackerId == WVR_TrackerId.WVR_TrackerId_1 || trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					return true;
				}
			}
			return false;
		}
		public WVR_TrackerRole GetTrackerRole(WVR_TrackerId trackerId)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_0)
				return WVR_TrackerRole.WVR_TrackerRole_Pair1_Right;
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1)
				return WVR_TrackerRole.WVR_TrackerRole_Pair1_Left;

			return WVR_TrackerRole.WVR_TrackerRole_Undefined;
		}
		public WVR_Result GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					capabilities = m_BraceletCaps;
					return WVR_Result.WVR_Success;
				}
			}

			return WVR_Result.WVR_Error_FeatureNotSupport;
		}
		public WVR_Result GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			if (m_TrackerEnabled)
			{
				if (trackerId == WVR_TrackerId.WVR_TrackerId_0)
				{
					poseState = m_BraceletPoseRight;
					return WVR_Result.WVR_Success;
				}
				if (trackerId == WVR_TrackerId.WVR_TrackerId_1)
				{
					poseState = m_BraceletPoseLeft;
					return WVR_Result.WVR_Success;
				}
			}

			return WVR_Result.WVR_Error_FeatureNotSupport;
		}
		public Int32 GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_0 || trackerId == WVR_TrackerId.WVR_TrackerId_1)
			{
				if (m_TrackerEnabled)
				{
					switch (inputType)
					{
						case WVR_InputType.WVR_InputType_Button:
							return trackerId == WVR_TrackerId.WVR_TrackerId_0 ? m_TrackerInputButton0 : m_TrackerInputButton1;
						case WVR_InputType.WVR_InputType_Touch:
						case WVR_InputType.WVR_InputType_Analog:
						default:
							break;
					}
				}
			}
			return -1;
		}
		public WVR_AnalogType GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled && s_BraceletAnalogTypes.ContainsKey(id))
				{
					return s_BraceletAnalogTypes[id];
				}
			}
			return WVR_AnalogType.WVR_AnalogType_None;
		}

		uint tracker_press_right = 0, tracker_press_left = 0;
		private void BraceletPressed(WVR_TrackerId trackerId)
		{
			switch (trackerId)
			{
				case WVR_TrackerId.WVR_TrackerId_0:
					if (WXRInput.GetKeyDown(KeyCode.P))
					{
						DEBUG("BraceletPressed() " + trackerId + ", A.");
						mEvent.trackerInput.tracker.common.type = WVR_EventType.WVR_EventType_TrackerButtonPressed;
						mEvent.trackerInput.tracker.trackerId = trackerId;
						mEvent.trackerInput.inputId = WVR_InputId.WVR_InputId_Alias1_A;
						hasEvent = true;

						tracker_press_right |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_A;
					}
					break;
				case WVR_TrackerId.WVR_TrackerId_1:
					if (WXRInput.GetKeyDown(KeyCode.Q))
					{
						DEBUG("BraceletPressed() " + trackerId + ", X.");
						mEvent.trackerInput.tracker.common.type = WVR_EventType.WVR_EventType_TrackerButtonPressed;
						mEvent.trackerInput.tracker.trackerId = trackerId;
						mEvent.trackerInput.inputId = WVR_InputId.WVR_InputId_Alias1_X;
						hasEvent = true;

						tracker_press_left |= 1 << (int)WVR_InputId.WVR_InputId_Alias1_X;
					}
					break;
				default:
					break;
			}
		}
		private void BraceletUnpressed(WVR_TrackerId trackerId)
		{
			switch (trackerId)
			{
				case WVR_TrackerId.WVR_TrackerId_0:
					if (WXRInput.GetKeyUp(KeyCode.P))
					{
						DEBUG("BraceletUnpressed() " + trackerId + ", A.");
						mEvent.trackerInput.tracker.common.type = WVR_EventType.WVR_EventType_TrackerButtonUnpressed;
						mEvent.trackerInput.tracker.trackerId = trackerId;
						mEvent.trackerInput.inputId = WVR_InputId.WVR_InputId_Alias1_A;
						hasEvent = true;

						tracker_press_right ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_A;
					}
					break;
				case WVR_TrackerId.WVR_TrackerId_1:
					if (WXRInput.GetKeyUp(KeyCode.Q))
					{
						DEBUG("BraceletPressed() " + trackerId + ", X.");
						mEvent.trackerInput.tracker.common.type = WVR_EventType.WVR_EventType_TrackerButtonUnpressed;
						mEvent.trackerInput.tracker.trackerId = trackerId;
						mEvent.trackerInput.inputId = WVR_InputId.WVR_InputId_Alias1_X;
						hasEvent = true;

						tracker_press_left ^= 1 << (int)WVR_InputId.WVR_InputId_Alias1_X;
					}
					break;
				default:
					break;
			}
		}

		public bool GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			bool pressed = false;

			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					int input = 1 << (int)id;
					switch (trackerId)
					{
						case WVR_TrackerId.WVR_TrackerId_0:
							pressed = ((tracker_press_right & input) == input);
							break;
						case WVR_TrackerId.WVR_TrackerId_1:
							pressed = ((tracker_press_left & input) == input);
							break;
						default:
							break;
					}
				}
			}

			return pressed;
		}
		public bool GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			bool touched = false;

			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					int input = 1 << (int)id;
					switch (trackerId)
					{
						case WVR_TrackerId.WVR_TrackerId_0:
							touched = ((state_touch_right & input) == input);
							break;
						case WVR_TrackerId.WVR_TrackerId_1:
							touched = ((state_touch_left & input) == input);
							break;
						default:
							break;
					}
				}
			}

			return touched;
		}
		public WVR_Axis_t GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id)
		{
			WVR_Axis_t axis2d;
			axis2d.x = 0;
			axis2d.y = 0;

			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					switch (trackerId)
					{
						case WVR_TrackerId.WVR_TrackerId_1:
							axis2d = state_axis_left[(uint)id];
							break;
						case WVR_TrackerId.WVR_TrackerId_0:
							axis2d = state_axis_right[(uint)id];
							break;
						default:
							break;
					}
				}
			}

			return axis2d;
		}
		public float GetTrackerBatteryLevel(WVR_TrackerId trackerId)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					return .7f;
				}
			}
			return 0;
		}
		public WVR_Result TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f)
		{
			if (trackerId == WVR_TrackerId.WVR_TrackerId_1 ||
				trackerId == WVR_TrackerId.WVR_TrackerId_0)
			{
				if (m_TrackerEnabled)
				{
					return WVR_Result.WVR_Success;
				}
			}

			return WVR_Result.WVR_Error_FeatureNotSupport;
		}

		const int kTrackerExtDataTypeSize = 4;
		Int32[] s_Tracker0ExtData = new Int32[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		Int32[] s_Tracker1ExtData = new Int32[] { 1, 1, 1, 1, 1, 1 };
		public IntPtr GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp)
		{
			timestamp = (UInt64)Time.frameCount;

			exDataSize = (trackerId == WVR_TrackerId.WVR_TrackerId_0 ?
				s_Tracker0ExtData.Length : s_Tracker1ExtData.Length
			);

			IntPtr exData = Marshal.AllocHGlobal(exDataSize * kTrackerExtDataTypeSize);
			for (int i = 0; i < exDataSize; i++)
			{
				Marshal.WriteInt32(
					exData,
					i * kTrackerExtDataTypeSize,
					(trackerId == WVR_TrackerId.WVR_TrackerId_0 ? s_Tracker0ExtData[i] : s_Tracker1ExtData[i])
				);
			}

			return exData;
		}
		static Dictionary<WVR_TrackerId, string> s_TrackerDeviceNames = new Dictionary<WVR_TrackerId, string>() {
			{ WVR_TrackerId.WVR_TrackerId_0, "Wrist_Right"	/*new string(new char[] {'W', 'r', 'i', 's', 't', '_', 'R', 'i', 'g', 'h', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_1, "Wrist_Left"	/*new string(new char[] {'W', 'r', 'i', 's', 't', '_', 'L', 'e', 'f', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_2, "Waist"		/*new string(new char[] {'W', 'a', 'i', 's', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_3, "Ankle_Right"	/*new string(new char[] {'A', 'n', 'k', 'l', 'e', '_', 'R', 'i', 'g', 'h', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_4, "Ankle_Left"	/*new string(new char[] {'A', 'n', 'k', 'l', 'e', '_', 'L', 'e', 'f', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_5, "Upper_Arm_Right"	/*new string(new char[] {'U', 'p', 'p', 'e', 'r', '_', 'A', 'r', 'm', '_', 'R', 'i', 'g', 'h', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_6, "Upper_Arm_Left"	/*new string(new char[] {'U', 'p', 'p', 'e', 'r', '_', 'A', 'r', 'm', '_', 'L', 'e', 'f', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_7, "Thigh_Right"	/*new string(new char[] {'T', 'h', 'i', 'g', 'h', '_', 'R', 'i', 'g', 'h', 't' })*/ },
			{ WVR_TrackerId.WVR_TrackerId_8, "Thigh_Left"	/*new string(new char[] {'T', 'h', 'i', 'g', 'h', '_', 'L', 'e', 'f', 't' })*/ },
		};
		public WVR_Result GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, ref IntPtr deviceName)
		{
			if (!s_TrackerDeviceNames.ContainsKey(trackerId)) { return WVR_Result.WVR_Error_FeatureNotSupport; }

			if (deviceName.Equals(IntPtr.Zero))
			{
				nameSize = (UInt32)s_TrackerDeviceNames[trackerId].Length;
			}
			else
			{
				deviceName = Marshal.StringToHGlobalAnsi(s_TrackerDeviceNames[trackerId]);

				//var name = Marshal.PtrToStringAnsi(deviceName);
				//DEBUG("GetTrackerDeviceName() " + trackerId + ", name: " + name);
			}

			return WVR_Result.WVR_Success;
		}

		private int m_focusedTracker = 2;
		public void SetFocusedTracker(int focusedTracker) { m_focusedTracker = focusedTracker; }
		public int GetFocusedTracker() { return m_focusedTracker; }
		#endregion

		#region wvr_notifydeviceinfo.h
		public WVR_Result StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize)
		{
			DEBUG("StartNotifyDeviceInfo() " + type + ", " + unBufferSize);
			return WVR_Result.WVR_Success;
		}
		public void StopNotifyDeviceInfo(WVR_DeviceType type)
		{
			DEBUG("StopNotifyDeviceInfo() " + type);
		}
		public void UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue)
		{
			var info = Marshal.PtrToStringAnsi(dataValue);
			DEBUG("UpdateNotifyDeviceInfo() " + info);
		}
		#endregion

		public bool IsDeviceConnected(WVR_DeviceType type)
		{
			return true;
		}

		public bool IsInputFocusCapturedBySystem()
		{
			mFocusIsCapturedBySystem = WXRInput.GetKey(KeyCode.Escape);
			return mFocusIsCapturedBySystem;
		}

		public void InAppRecenter(WVR_RecenterType recenterType)
		{
			xOffset = -xAxis;
			yOffset = -yAxis;
			zOffset = -zAxis;
			DEBUG(xOffset + ", " + yOffset + ", " + zOffset);
		}

		public ulong GetSupportedFeatures()
		{
			return (
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_PassthroughImage |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_PassthroughOverlay |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_HandGesture |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_HandTracking |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_ElectronicHand |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_EyeTracking |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_EyeExp |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_LipExp |
				(ulong)WVR_SupportedFeature.WVR_SupportedFeature_Tracker
			);
		}

		Dictionary<WVR_DeviceType, bool> s_TableStatic = new Dictionary<WVR_DeviceType, bool>()
		{
			{ WVR_DeviceType.WVR_DeviceType_Controller_Left, false },
			{ WVR_DeviceType.WVR_DeviceType_Controller_Right, false },
		};
		public bool IsDeviceTableStatic(WVR_DeviceType type)
		{
			if (!s_TableStatic.ContainsKey(type)) { return false; }

			return s_TableStatic[type];
		}
	}
#endif
}
