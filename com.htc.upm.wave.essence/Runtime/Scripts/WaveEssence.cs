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
using Wave.Native;
using Wave.Essence.Events;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
using Wave.Essence.Editor;
#endif

namespace Wave.Essence
{
	public class WaveEssence : MonoBehaviour
	{
		private const string LOG_TAG = "Wave.Essence.WaveEssence";
		private void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		private static WaveEssence instance = null;
		public static WaveEssence Instance
		{
			get
			{
				if (instance == null)
				{
					var gameObject = new GameObject("WaveEssence", typeof(WaveVR_SystemEvent));
					instance = gameObject.AddComponent<WaveEssence>();
					// This object should survive all scene transitions.
					DontDestroyOnLoad(instance);
				}
				return instance;
			}
		}

		#region Global Interface
		private bool m_IsLeftHanded = false;
		public bool IsLeftHanded { get { return m_IsLeftHanded; } }

		private Dictionary<WVR_DeviceType, bool> m_Connected = new Dictionary<WVR_DeviceType, bool>();
		public bool IsHandDeviceConnected(XR_HandDevice hand)
		{
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				return true;
			}
#endif
			return m_Connected[(WVR_DeviceType)hand];
		}

		/// <summary>
		/// WVR_DeviceType_Invalid = 0,
		/// WVR_DeviceType_HMD = 1,
		/// WVR_DeviceType_Controller_Right = 2,
		/// WVR_DeviceType_Controller_Left = 3,
		/// </summary>
		public static readonly int kDeviceCount = 4;
		#endregion


		#region MonoBehaviour overrides
		void Awake()
		{
			instance = this;

			for (int i = 0; i < Enum.GetNames(typeof(WVR_DeviceType)).Length; i++)
				m_Connected.Add((WVR_DeviceType)i, false);
		}
		void Start()
		{
			Log.i(LOG_TAG, "Start() 1.Check the device default role.");
			UpdateControllerRole();
			Log.i(LOG_TAG, "Start() 2.Check the device connections.");
			UpdateDeviceConnections();
			Log.i(LOG_TAG, "Start() 3.Update input mapping tables.");
			UpdateInputMappingTables();
			/*
			Log.i(LOG_TAG, "Start() 4.Update all button events");
			UpdateAllEventStates();
			*/
			Log.i(LOG_TAG, "Start() 5.Update current pose mode.");
			UpdateCurrentPoseMode(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			UpdateCurrentPoseMode(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			Log.i(LOG_TAG, "Start() 6.Get all controller pose offsets.");
			UpdateAllControllerPoseOffset();
			Log.i(LOG_TAG, "Start() 7.Update the interaction mode.");
			UpdateInteractionMode();
		}
		void OnApplicationPause(bool pauseStatus)
		{
			Log.i(LOG_TAG, "OnApplicationPause() pauseStatus: " + pauseStatus, true);
			if (!pauseStatus)
			{
				Log.i(LOG_TAG, "Resume - 1.Check the device default role in resume.");
				UpdateControllerRole();
				Log.i(LOG_TAG, "Resume - 2.Check the device connections.");
				UpdateDeviceConnections();
				Log.i(LOG_TAG, "Resume - 3.Update input mapping tables.");
				UpdateInputMappingTables();
				/*
				Log.i(LOG_TAG, "Resume - 4.Update all button events");
				UpdateAllEventStates();
				*/
				Log.i(LOG_TAG, "Resume - 5.Update current pose mode.");
				UpdateCurrentPoseMode(WVR_DeviceType.WVR_DeviceType_Controller_Right);
				UpdateCurrentPoseMode(WVR_DeviceType.WVR_DeviceType_Controller_Left);
				Log.i(LOG_TAG, "Resume - 6.Get all controller pose offsets.");
				UpdateAllControllerPoseOffset();
				Log.i(LOG_TAG, "Resume() 7.Update the interaction mode.");
				UpdateInteractionMode();
			}
		}
		private void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_RenderingToBePaused, OnRenderingToBePaused);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_RenderingToBeResumed, OnRenderingToBeResumed);

			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceRoleChanged, OnControllerRoleChange);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceConnected, OnDeviceConnected);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceDisconnected, OnDeviceDisconnected);
			/*
			SystemEvent.Listen(WVR_EventType.WVR_EventType_ButtonPressed, OnEventButtonPressed);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_ButtonUnpressed, OnEventButtonUnpressed);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TouchTapped, OnEventTouchTapped);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TouchUntapped, OnEventTouchUntapped);
			*/
			SystemEvent.Listen(WVR_EventType.WVR_EventType_ControllerPoseModeChanged, OnControllerPoseModeChanged);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_ControllerPoseModeOffsetReady, OnControllerPoseModeOffsetReady);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_SystemInteractionModeChanged, OnInteractionModeChanged);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_InputDevMappingChanged, OnInputDevMappingChanged);

#if UNITY_EDITOR
			if (!DummyPose.ToUpdatePose)
			{
				DummyPose.ToUpdatePose = true;
				DummyPose.Origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
				StartCoroutine(DummyPose.UpdatePoses());
			}
#endif
		}
		private void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_RenderingToBePaused, OnRenderingToBePaused);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_RenderingToBeResumed, OnRenderingToBeResumed);

			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceRoleChanged, OnControllerRoleChange);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceConnected, OnDeviceConnected);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceDisconnected, OnDeviceDisconnected);
			
			/*
			SystemEvent.Remove(WVR_EventType.WVR_EventType_ButtonPressed, OnEventButtonPressed);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_ButtonUnpressed, OnEventButtonUnpressed);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TouchTapped, OnEventTouchTapped);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TouchUntapped, OnEventTouchUntapped);
			*/
			SystemEvent.Remove(WVR_EventType.WVR_EventType_ControllerPoseModeChanged, OnControllerPoseModeChanged);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_ControllerPoseModeOffsetReady, OnControllerPoseModeOffsetReady);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_SystemInteractionModeChanged, OnInteractionModeChanged);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_InputDevMappingChanged, OnInputDevMappingChanged);

#if UNITY_EDITOR
			if (DummyPose.ToUpdatePose)
			{
				StopCoroutine(DummyPose.UpdatePoses());
				DummyPose.ToUpdatePose = false;
			}
#endif
		}
		void Update()
		{
			Log.gpl.check();

			UpdateEventButtons();
		}
		#endregion

		#region Major Standalone Functions
		private void OnRenderingToBePaused(WVR_Event_t systemEvent)
		{
			DEBUG("WVR_EventType_RenderingToBePaused");
		}
		private void OnRenderingToBeResumed(WVR_Event_t systemEvent)
		{
			DEBUG("WVR_EventType_RenderingToBeResumed");
		}

		private void UpdateControllerRole()
		{
			WVR_DeviceType default_role = Interop.WVR_GetDefaultControllerRole();
			m_IsLeftHanded = (default_role == WVR_DeviceType.WVR_DeviceType_Controller_Left ? true : false);
			DEBUG("UpdateControllerRole() m_IsLeftHanded = " + m_IsLeftHanded);
		}
		private void OnControllerRoleChange(WVR_Event_t systemEvent)
		{
			WVR_DeviceType default_role = Interop.WVR_GetDefaultControllerRole();
			bool left_handed = (default_role == WVR_DeviceType.WVR_DeviceType_Controller_Left ? true : false);
			if (m_IsLeftHanded != left_handed)
			{
				m_IsLeftHanded = left_handed;
				DEBUG("OnControllerRoleChange() Set left handed mode to " + m_IsLeftHanded);
				UpdateDeviceConnections();
				UpdateInputMappingTables();
				ResetAllButtonStates();
			}
			else
			{
				DEBUG("OnControllerRoleChange() No change for left handed mode: " + m_IsLeftHanded);
			}
		}

		public bool IsConnected(WVR_DeviceType deviceType)
		{
			return m_Connected[deviceType];
		}
		private void UpdateDeviceConnections()
		{
			for (int i = 0; i < Enum.GetNames(typeof(WVR_DeviceType)).Length; i++)
			{
				bool connected = Interop.WVR_IsDeviceConnected((WVR_DeviceType)i);
				DeviceConnectionHandler((WVR_DeviceType)i, connected);
				DEBUG("UpdateDeviceConnections() " + (WVR_DeviceType)i + " connected? " + m_Connected[(WVR_DeviceType)i]);
			}
		}
		private void OnDeviceConnected(WVR_Event_t systemEvent)
		{
			WVR_DeviceType device = systemEvent.device.type;
			DEBUG("OnDeviceConnected() " + device);
			DeviceConnectionHandler(device, true);
		}
		private void OnDeviceDisconnected(WVR_Event_t systemEvent)
		{
			WVR_DeviceType device = systemEvent.device.type;
			DEBUG("OnDeviceDisconnected() " + device);
			DeviceConnectionHandler(device, false);
		}
		private void DeviceConnectionHandler(WVR_DeviceType device, bool connected)
		{
			if (m_Connected[device] == connected)
				return;

			m_Connected[device] = connected;
			if (m_Connected[device])
			{
				if (device == WVR_DeviceType.WVR_DeviceType_HMD)
					UpdateInputMappingTableHead();
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				{
					UpdateInputMappingTableRight();
					UpdateControllerPoseOffset(device);
				}
				if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				{
					UpdateInputMappingTableLeft();
					UpdateControllerPoseOffset(device);
				}

				//UpdateEventStates(device);
			}
			else
			{
				ResetButtonStates(device);
			}
		}

		/*private void OnEventButtonPressed(WVR_Event_t systemEvent)
		{
			DEBUG("OnEventButtonPressed() " + systemEvent.device.type + ", " + systemEvent.input.inputId);
			SetEventPress(systemEvent.device.type, systemEvent.input.inputId, true);
		}
		private void OnEventButtonUnpressed(WVR_Event_t systemEvent)
		{
			DEBUG("OnEventButtonUnpressed() " + systemEvent.device.type + ", " + systemEvent.input.inputId);
			SetEventPress(systemEvent.device.type, systemEvent.input.inputId, false);
		}
		private void OnEventTouchTapped(WVR_Event_t systemEvent)
		{
			DEBUG("OnEventTouchTapped() " + systemEvent.device.type + ", " + systemEvent.input.inputId);
			SetEventTouch(systemEvent.device.type, systemEvent.input.inputId, true);
		}
		private void OnEventTouchUntapped(WVR_Event_t systemEvent)
		{
			DEBUG("OnEventTouchUntapped() " + systemEvent.device.type + ", " + systemEvent.input.inputId);
			SetEventTouch(systemEvent.device.type, systemEvent.input.inputId, false);
		}*/

		private void OnControllerPoseModeChanged(WVR_Event_t systemEvent)
		{
			DEBUG("OnControllerPoseModeChanged() " + systemEvent.device.type);
			if (UpdateCurrentPoseMode(systemEvent.device.type))
			{
				if (systemEvent.device.type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
					UpdateControllerPoseOffset(systemEvent.device.type, m_RightPoseMode.mode);
				if (systemEvent.device.type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
					UpdateControllerPoseOffset(systemEvent.device.type, m_LeftPoseMode.mode);
			}
		}
		private void OnControllerPoseModeOffsetReady(WVR_Event_t systemEvent)
		{
			DEBUG("OnControllerPoseModeOffsetReady()");
			UpdateAllControllerPoseOffset();
		}
		#endregion

		#region Wave Native Button
		/**
		 * When receiving a button press/release/touch/untouch event, update the event press/touch states.
		 * If someone requests the button press/touch states, update the button press/touch states with the event press/touch states.
		 * List the button state as below:
		 * - no press -> press
		 * - hold
		 * - press -> release
		 * - no touch -> touch
		 * - keep touching
		 * - touch -> untouch
		 **/
		private bool[,] s_EventPress = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool[,] s_ButtonPress = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool[,] s_ButtonPressEx = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];

		private bool[,] s_EventTouch = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool[,] s_ButtonTouch = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool[,] s_ButtonTouchEx = new bool[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];

		private Vector2[,] s_ButtonAxis = new Vector2[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		/// <summary> Resets the button states of a device. Called when the device is disconnected. </summary>
		private void ResetButtonStates(WVR_DeviceType device)
		{
			DEBUG("ResetButtonStates() " + device);
			for (int button_index = 0; button_index < (int)WVR_InputId.WVR_InputId_Max; button_index++)
			{
				s_EventPress[(int)device, button_index] = false;
				s_ButtonPress[(int)device, button_index] = false;
				s_ButtonPressEx[(int)device, button_index] = false;

				s_EventTouch[(int)device, button_index] = false;
				s_ButtonTouch[(int)device, button_index] = false;
				s_ButtonTouchEx[(int)device, button_index] = false;

				s_ButtonAxis[(int)device, button_index].x = 0;
				s_ButtonAxis[(int)device, button_index].y = 0;
			}
		}
		/// <summary> Resets the button states of all devices. Called when the controller role changes. </summary>
		private void ResetAllButtonStates()
		{
			DEBUG("ResetAllButtonStates()");
			ResetButtonStates(WVR_DeviceType.WVR_DeviceType_HMD);
			ResetButtonStates(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			ResetButtonStates(WVR_DeviceType.WVR_DeviceType_Controller_Left);
		}

		private void UpdateEventButtonsHmd()
		{
			WVR_DeviceType dev = WVR_DeviceType.WVR_DeviceType_HMD;
			if (!m_Connected[dev]) return;

			uint inputType = (uint)WVR_InputType.WVR_InputType_Button;
			uint buttons = 0, touches = 0;

			if (Interop.WVR_GetInputDeviceState(dev, inputType, ref buttons, ref touches, null, 0))
			{
				for (uint id = 0; id < (uint)WVR_InputId.WVR_InputId_Max; id++)
				{
					int input = 1 << (int)id;
					bool pressed = ((buttons & input) == input);
					/// Press
					if (s_EventPress[(uint)dev, id] != pressed)
					{
						s_EventPress[(uint)dev, id] = pressed;
						DEBUG("UpdateEventButtonsHmd() HMD button " + id + " is " + (pressed ? "pressed." : "released."));
					}
					/// No Touch & Axis
				}
			}
		}
		uint inputTypeLeft = (uint)(WVR_InputType.WVR_InputType_Button | WVR_InputType.WVR_InputType_Touch | WVR_InputType.WVR_InputType_Analog);
		uint inputTypeRight = (uint)(WVR_InputType.WVR_InputType_Button | WVR_InputType.WVR_InputType_Touch | WVR_InputType.WVR_InputType_Analog);
		Dictionary<WVR_DeviceType, WVR_AnalogState_t[]> analogState = new Dictionary<WVR_DeviceType, WVR_AnalogState_t[]>()
		{
			{ WVR_DeviceType.WVR_DeviceType_Invalid, null },			// 0
			{ WVR_DeviceType.WVR_DeviceType_HMD, null },
			{ WVR_DeviceType.WVR_DeviceType_Controller_Right, null },
			{ WVR_DeviceType.WVR_DeviceType_Controller_Left, null },
			{ WVR_DeviceType.WVR_DeviceType_Camera, null },
			{ WVR_DeviceType.WVR_DeviceType_EyeTracking, null },		// 5
			{ WVR_DeviceType.WVR_DeviceType_HandGesture_Right, null },
			{ WVR_DeviceType.WVR_DeviceType_HandGesture_Left, null },
			{ WVR_DeviceType.WVR_DeviceType_NaturalHand_Right, null },
			{ WVR_DeviceType.WVR_DeviceType_NaturalHand_Left, null },
			{ WVR_DeviceType.WVR_DeviceType_ElectronicHand_Right, null },   // 10
			{ WVR_DeviceType.WVR_DeviceType_ElectronicHand_Left, null },
		};
		private void UpdateEventButtonsController(WVR_DeviceType dev)
		{
			if (!m_Connected[dev]) return;
			uint inputType = (uint)(WVR_InputType.WVR_InputType_Button | WVR_InputType.WVR_InputType_Touch | WVR_InputType.WVR_InputType_Analog);
			switch (dev)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					inputType = inputTypeLeft;
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					inputType = inputTypeRight;
					break;
				default:
					return;
			}

			uint buttons = 0, touches = 0;
			int analogCount = Interop.WVR_GetInputTypeCount(dev, WVR_InputType.WVR_InputType_Analog);

			if (analogCount > 0)
			{
				if (analogState[dev] == null || analogState[dev].Length < analogCount)
				{
					analogState[dev] = new WVR_AnalogState_t[analogCount];
				}

				if (Interop.WVR_GetInputDeviceState(dev, inputType, ref buttons, ref touches, analogState[dev], (uint)analogCount))
				{
					for (uint id = 0; id < (uint)WVR_InputId.WVR_InputId_Max; id++)
					{
						int input = 1 << (int)id;
						/// Press
						bool pressed = ((buttons & input) == input);
						if (s_EventPress[(uint)dev, id] != pressed)
						{
							s_EventPress[(uint)dev, id] = pressed;
							DEBUG("UpdateEventButtonsController() " + dev + ", analogCount: " + analogCount + ", button " + id + " is " + (pressed ? "pressed." : "released."));
						}
						/// Touch
						bool touched = ((touches & input) == input);
						if (s_EventTouch[(uint)dev, id] != touched)
						{
							s_EventTouch[(uint)dev, id] = touched;
							DEBUG("UpdateEventButtonsController() " + dev + ", analogCount: " + analogCount + ", button " + id + " is " + (touched ? "touched." : "untouched."));
						}
						/// Axis
						if (s_EventTouch[(uint)dev, id])
						{
							for (uint index = 0; index < analogCount; index++)
							{
								if (id == (uint)analogState[dev][index].id)
								{
									s_ButtonAxis[(uint)dev, id].x = analogState[dev][index].axis.x;
									s_ButtonAxis[(uint)dev, id].y = analogState[dev][index].axis.y;
								}
							}
						}
						else
						{
							s_ButtonAxis[(uint)dev, id].x = 0;
							s_ButtonAxis[(uint)dev, id].y = 0;
						}
					}
				}
			}
			else
			{
				if (Interop.WVR_GetInputDeviceState(dev, inputType, ref buttons, ref touches, null, 0))
				{
					for (uint id = 0; id < (uint)WVR_InputId.WVR_InputId_Max; id++)
					{
						int input = 1 << (int)id;
						/// Press
						bool pressed = ((buttons & input) == input);
						if (s_EventPress[(uint)dev, id] != pressed)
						{
							s_EventPress[(uint)dev, id] = pressed;
							DEBUG("UpdateEventButtonsController() " + dev + " button " + id + " is " + (pressed ? "pressed." : "released."));
						}
						/// Touch
						bool touched = ((touches & input) == input);
						if (s_EventTouch[(uint)dev, id] != touched)
						{
							s_EventTouch[(uint)dev, id] = touched;
							DEBUG("UpdateEventButtonsController() " + dev + " button " + id + " is " + (touched ? "touched." : "untouched."));
						}
						/// Axis
						s_ButtonAxis[(uint)dev, id].x = 0;
						s_ButtonAxis[(uint)dev, id].y = 0;
					}
				}
			}
		}
		private void UpdateEventButtons()
		{
			Profiler.BeginSample("UpdateEventButtonsHmd");
			UpdateEventButtonsHmd();
			Profiler.EndSample();
			Profiler.BeginSample("UpdateEventButtonsController");
			UpdateEventButtonsController(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			Profiler.EndSample();
			Profiler.BeginSample("UpdateEventButtonsController");
			UpdateEventButtonsController(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			Profiler.EndSample();
		}

		// Event states.
		/*
		private void SetEventPress(WVR_DeviceType device, WVR_InputId id, bool state)
		{
			DEBUG("SetEventPress() " + device + ", " + id + " is " + (state ? "pressed." : "released."));
			s_EventPress[(int)device, (int)id] = state;
		}
		private void SetEventTouch(WVR_DeviceType device, WVR_InputId id, bool state)
		{
			DEBUG("SetEventTouch() " + device + ", " + id + " is " + (state ? "touched." : "untouched."));
			s_EventTouch[(int)device, (int)id] = state;
		}
		/// <summary> Updates the button event states of a device. Called when the device is connected. </summary>
		private void UpdateEventStates(WVR_DeviceType device)
		{
			bool valid = true;

			if (device != WVR_DeviceType.WVR_DeviceType_HMD &&
				device != WVR_DeviceType.WVR_DeviceType_Controller_Right &&
				device != WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				valid = false;
			}

			if (!m_Connected[device])
				valid = false;

			for (int i = 0; i < (int)WVR_InputId.WVR_InputId_Max; i++)
			{
				s_EventPress[(int)device, i] = valid ? Interop.WVR_GetInputButtonState(device, (WVR_InputId)i) : false;
				s_EventTouch[(int)device, i] = valid ? Interop.WVR_GetInputTouchState(device, (WVR_InputId)i) : false;
				DEBUG("UpdateEventStates() " + device + ", button " + i + " pressed? " + s_EventPress[(int)device, i] + ", touched? " + s_EventTouch[(int)device, i]);
			}
		}
		/// <summary> Updates the button event states of all devices. Called when AP starts and resumes. </summary>
		private void UpdateAllEventStates()
		{
			DEBUG("UpdateAllEventStates()");
			UpdateEventStates(WVR_DeviceType.WVR_DeviceType_HMD);
			UpdateEventStates(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			UpdateEventStates(WVR_DeviceType.WVR_DeviceType_Controller_Left);
		}
		*/

		// Button press states.
		private int[,] buttonPressFrame = new int[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool AllowUpdateButtonPress(WVR_DeviceType device, WVR_InputId id)
		{
			if (Time.frameCount != buttonPressFrame[(int)device, (int)id])
			{
				buttonPressFrame[(int)device, (int)id] = Time.frameCount;
				return true;
			}
			return false;
		}
		private void UpdateButtonPress(WVR_DeviceType device, WVR_InputId id)
		{
			if (AllowUpdateButtonPress(device, id))
			{
				s_ButtonPressEx[(int)device, (int)id] = s_ButtonPress[(int)device, (int)id];
				s_ButtonPress[(int)device, (int)id] = s_EventPress[(int)device, (int)id];
			}
		}
		public bool ButtonPress(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonPress(device, id);
			return (!s_ButtonPressEx[(int)device, (int)id] && s_ButtonPress[(int)device, (int)id]);
		}
		public bool ButtonHold(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonPress(device, id);
			return (s_ButtonPressEx[(int)device, (int)id] && s_ButtonPress[(int)device, (int)id]);
		}
		public bool ButtonRelease(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonPress(device, id);
			return (s_ButtonPressEx[(int)device, (int)id] && !s_ButtonPress[(int)device, (int)id]);
		}

		// Button touch states.
		private int[,] buttonTouchFrame = new int[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool AllowUpdateButtonTouch(WVR_DeviceType device, WVR_InputId id)
		{
			if (Time.frameCount != buttonTouchFrame[(int)device, (int)id])
			{
				buttonTouchFrame[(int)device, (int)id] = Time.frameCount;
				return true;
			}
			return false;
		}
		private void UpdateButtonTouch(WVR_DeviceType device, WVR_InputId id)
		{
			if (AllowUpdateButtonTouch(device, id))
			{
				s_ButtonTouchEx[(int)device, (int)id] = s_ButtonTouch[(int)device, (int)id];
				s_ButtonTouch[(int)device, (int)id] = s_EventTouch[(int)device, (int)id];
			}
		}
		public bool ButtonTouch(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonTouch(device, id);
			return (!s_ButtonTouchEx[(int)device, (int)id] && s_ButtonTouch[(int)device, (int)id]);
		}
		public bool ButtonTouching(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonTouch(device, id);
			return (s_ButtonTouchEx[(int)device, (int)id] && s_ButtonTouch[(int)device, (int)id]);
		}
		public bool ButtonUntouch(WVR_DeviceType device, WVR_InputId id)
		{
			UpdateButtonTouch(device, id);
			return (s_ButtonTouchEx[(int)device, (int)id] && !s_ButtonTouch[(int)device, (int)id]);
		}

		// Button axis.
		private int[,] buttonAxisFrame = new int[Enum.GetNames(typeof(WVR_DeviceType)).Length, (int)WVR_InputId.WVR_InputId_Max];
		private bool AllowUpdateButtonAxis(WVR_DeviceType device, WVR_InputId id)
		{
			if (Time.frameCount != buttonAxisFrame[(int)device, (int)id])
			{
				buttonAxisFrame[(int)device, (int)id] = Time.frameCount;
				return true;
			}
			return false;
		}
		public Vector2 ButtonAxis(WVR_DeviceType device, WVR_InputId id)
		{
			/*if (AllowUpdateButtonAxis(device, id))
			{
				s_ButtonAxis[(int)device, (int)id] = Vector2.zero;
				if (device == WVR_DeviceType.WVR_DeviceType_HMD ||
					device == WVR_DeviceType.WVR_DeviceType_Controller_Right ||
					device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				{
					if (m_Connected[device])
					{
						if (s_EventTouch[(int)device, (int)id] || s_EventPress[(int)device, (int)id])
						{
							WVR_Axis_t axis = Interop.WVR_GetInputAnalogAxis(device, id);
							s_ButtonAxis[(int)device, (int)id].x = axis.x;
							s_ButtonAxis[(int)device, (int)id].y = axis.y;
						}
					}
				}
			}*/

			return s_ButtonAxis[(int)device, (int)id];
		}
		#endregion

		#region Wave Key Mapping
		private const uint inputTableSize = (uint)WVR_InputId.WVR_InputId_Max;

		private uint inputTableHMDSize = 0;
		private WVR_InputMappingPair_t[] inputTableHMD = new WVR_InputMappingPair_t[inputTableSize];
		private void UpdateInputMappingTableHead()
		{
			if (!m_Connected[WVR_DeviceType.WVR_DeviceType_HMD])
				return;

			inputTableHMDSize = Interop.WVR_GetInputMappingTable(WVR_DeviceType.WVR_DeviceType_HMD, inputTableHMD, inputTableSize);
			if (inputTableHMDSize > 0)
			{
				for (int _i = 0; _i < (int)inputTableHMDSize; _i++)
				{
					if (inputTableHMD[_i].source.capability != 0)
					{
						DEBUG("UpdateInputMappingTableHead()"
							+ " button: " + inputTableHMD[_i].source.id + "(capability: " + inputTableHMD[_i].source.capability + ")"
							+ " is mapping to HMD input ID: " + inputTableHMD[_i].destination.id);
					}
					else
					{
						DEBUG("UpdateInputMappingTableHead() source button " + inputTableHMD[_i].source.id + " has invalid capability.");
					}
				}
			}
		}

		private uint inputTableRightSize = 0;
		private WVR_InputMappingPair_t[] inputTableRight = new WVR_InputMappingPair_t[inputTableSize];
		private void UpdateInputMappingTableRight()
		{
			if (!m_Connected[WVR_DeviceType.WVR_DeviceType_Controller_Right])
				return;

			inputTableRightSize = Interop.WVR_GetInputMappingTable(WVR_DeviceType.WVR_DeviceType_Controller_Right, inputTableRight, inputTableSize);
			if (inputTableRightSize > 0)
			{
				for (int _i = 0; _i < (int)inputTableRightSize; _i++)
				{
					if (inputTableRight[_i].source.capability != 0)
					{
						DEBUG("UpdateInputMappingTableRight()"
							+ " button: " + inputTableRight[_i].source.id + "(capability: " + inputTableRight[_i].source.capability + ")"
							+ " is mapping to Dominant input ID: " + inputTableRight[_i].destination.id);
					}
					else
					{
						DEBUG("UpdateInputMappingTableRight() source button " + inputTableRight[_i].source.id + " has invalid capability.");
					}
				}
			}
		}

		private uint inputTableLeftSize = 0;
		private WVR_InputMappingPair_t[] inputTableLeft = new WVR_InputMappingPair_t[inputTableSize];
		private void UpdateInputMappingTableLeft()
		{
			if (!m_Connected[WVR_DeviceType.WVR_DeviceType_Controller_Left])
				return;

			inputTableLeftSize = Interop.WVR_GetInputMappingTable(WVR_DeviceType.WVR_DeviceType_Controller_Left, inputTableLeft, inputTableSize);
			if (inputTableLeftSize > 0)
			{
				for (int _i = 0; _i < (int)inputTableLeftSize; _i++)
				{
					if (inputTableLeft[_i].source.capability != 0)
					{
						DEBUG("UpdateInputMappingTableLeft()"
							+ " button: " + inputTableLeft[_i].source.id + "(capability: " + inputTableLeft[_i].source.capability + ")"
							+ " is mapping to NonDominant input ID: " + inputTableLeft[_i].destination.id);
					}
					else
					{
						DEBUG("UpdateInputMappingTableLeft() source button " + inputTableLeft[_i].source.id + " has invalid capability.");
					}
				}
			}
		}

		private void OnInputDevMappingChanged(WVR_Event_t systemEvent)
		{
			WVR_DeviceType device = systemEvent.device.type;
			DEBUG("OnInputDevMappingChanged() " + device);
			if (device == WVR_DeviceType.WVR_DeviceType_HMD)
				UpdateInputMappingTableHead();
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				UpdateInputMappingTableRight();
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				UpdateInputMappingTableLeft();
		}
		private void UpdateInputMappingTables()
		{
			UpdateInputMappingTableHead();
			UpdateInputMappingTableRight();
			UpdateInputMappingTableLeft();
		}
		public bool GetInputMappingPair(WVR_DeviceType device, ref WVR_InputId destination)
		{
			if (!m_Connected[device])
				return false;

			if (device == WVR_DeviceType.WVR_DeviceType_HMD)
			{
				for (int index = 0; index < (int)inputTableHMDSize; index++)
				{
					if (inputTableHMD[index].destination.id == destination)
					{
						destination = inputTableHMD[index].source.id;
						return true;
					}
				}
			}
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Right)
			{
				for (int index = 0; index < (int)inputTableRightSize; index++)
				{
					if (inputTableRight[index].destination.id == destination)
					{
						destination = inputTableRight[index].source.id;
						return true;
					}
				}
			}
			if (device == WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				for (int index = 0; index < (int)inputTableLeftSize; index++)
				{
					if (inputTableLeft[index].destination.id == destination)
					{
						destination = inputTableLeft[index].source.id;
						return true;
					}
				}
			}

			return false;
		}
		#endregion

		#region Controller Pose Mode
		internal class PoseModeSetting
		{
			public bool isValid = false;
			public WVR_ControllerPoseMode mode = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw;
			public PoseModeSetting(bool in_isValid, WVR_ControllerPoseMode in_mode)
			{
				isValid = in_isValid;
				mode = in_mode;
			}
		};
		PoseModeSetting m_RightPoseMode = new PoseModeSetting(false, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw);
		PoseModeSetting m_LeftPoseMode = new PoseModeSetting(false, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw);
		private bool UpdateCurrentPoseMode(WVR_DeviceType type)
		{
			WVR_ControllerPoseMode mode = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw;
			bool isValid = Interop.WVR_GetControllerPoseMode(type, ref mode);
			DEBUG("UpdateCurrentPoseMode() isValid: " + isValid + ", device: " + type + ", mode: " + mode);
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
			{
				m_RightPoseMode.isValid = isValid;
				if (m_RightPoseMode.isValid) { m_RightPoseMode.mode = mode; }
			}
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				m_LeftPoseMode.isValid = isValid;
				if (m_LeftPoseMode.isValid) { m_LeftPoseMode.mode = mode; }
			}
			return isValid;
		}
		private Vector3 triggerModePositionOffsetLeft = Vector3.zero, panelModePositionOffsetLeft = Vector3.zero, handleModePositionOffsetLeft = Vector3.zero;
		private Vector3 triggerModePositionOffsetRight = Vector3.zero, panelModePositionOffsetRight = Vector3.zero, handleModePositionOffsetRight = Vector3.zero;
		private Quaternion triggerModeRotationOffsetLeft = Quaternion.identity, panelModeRotationOffsetLeft = Quaternion.identity, handleModeRotationOffsetLeft = Quaternion.identity;
		private Quaternion triggerModeRotationOffsetRight = Quaternion.identity, panelModeRotationOffsetRight = Quaternion.identity, handleModeRotationOffsetRight = Quaternion.identity;
		private void UpdateAllControllerPoseOffset()
		{
			Log.i(LOG_TAG, "UpdateAllControllerPoseOffset()");
			UpdateControllerPoseOffset(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			UpdateControllerPoseOffset(WVR_DeviceType.WVR_DeviceType_Controller_Right);
		}
		private void UpdateControllerPoseOffset(WVR_DeviceType type)
		{
			Log.i(LOG_TAG, "UpdateControllerPoseOffset() " + type);
			UpdateControllerPoseOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger);
			UpdateControllerPoseOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel);
			UpdateControllerPoseOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle);
		}
		private void UpdateControllerPoseOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			Log.i(LOG_TAG, "UpdateControllerPoseOffset() " + type + ", " + mode);
			WVR_Vector3f_t pos_offset = new WVR_Vector3f_t();
			WVR_Quatf_t rot_offset = new WVR_Quatf_t();
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				switch (mode)
				{
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out triggerModePositionOffsetLeft);
							Coordinate.GetQuaternionFromGL(rot_offset, out triggerModeRotationOffsetLeft);
						}
						break;
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out panelModePositionOffsetLeft);
							Coordinate.GetQuaternionFromGL(rot_offset, out panelModeRotationOffsetLeft);
						}
						break;
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out handleModePositionOffsetLeft);
							Coordinate.GetQuaternionFromGL(rot_offset, out handleModeRotationOffsetLeft);
						}
						break;
					default:
						break;
				}
			}
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
			{
				switch (mode)
				{
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out triggerModePositionOffsetRight);
							Coordinate.GetQuaternionFromGL(rot_offset, out triggerModeRotationOffsetRight);
						}
						break;
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out panelModePositionOffsetRight);
							Coordinate.GetQuaternionFromGL(rot_offset, out panelModeRotationOffsetRight);
						}
						break;
					case WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle:
						if (Interop.WVR_GetControllerPoseModeOffset(type, WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle, ref pos_offset, ref rot_offset))
						{
							Coordinate.GetVectorFromGL(pos_offset, out handleModePositionOffsetRight);
							Coordinate.GetQuaternionFromGL(rot_offset, out handleModeRotationOffsetRight);
						}
						break;
					default:
						break;
				}
			}
		}
		public Vector3 GetCurrentControllerPositionOffset(WVR_DeviceType type)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				return GetControllerPositionOffset(type, m_RightPoseMode.mode);
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				return GetControllerPositionOffset(type, m_LeftPoseMode.mode);
			return Vector3.zero;
		}
		public Vector3 GetControllerPositionOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					{
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger)
							return triggerModePositionOffsetLeft;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel)
							return panelModePositionOffsetLeft;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle)
							return handleModePositionOffsetLeft;
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					{
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger)
							return triggerModePositionOffsetRight;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel)
							return panelModePositionOffsetRight;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle)
							return handleModePositionOffsetRight;
					}
					break;
				default:
					break;
			}
			return Vector3.zero;
		}
		public Quaternion GetCurrentControllerRotationOffset(WVR_DeviceType type)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				return GetControllerRotationOffset(type, m_RightPoseMode.mode);
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				return GetControllerRotationOffset(type, m_LeftPoseMode.mode);
			return Quaternion.identity;
		}
		public Quaternion GetControllerRotationOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			switch (type)
			{
				case WVR_DeviceType.WVR_DeviceType_Controller_Left:
					{
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger)
							return triggerModeRotationOffsetLeft;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel)
							return panelModeRotationOffsetLeft;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle)
							return handleModeRotationOffsetLeft;
					}
					break;
				case WVR_DeviceType.WVR_DeviceType_Controller_Right:
					{
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Trigger)
							return triggerModeRotationOffsetRight;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Panel)
							return panelModeRotationOffsetRight;
						if (mode == WVR_ControllerPoseMode.WVR_ControllerPoseMode_Handle)
							return handleModeRotationOffsetRight;
					}
					break;
				default:
					break;
			}
			return Quaternion.identity;
		}
		public bool GetControllerPoseMode(WVR_DeviceType type, out WVR_ControllerPoseMode mode)
		{
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Right)
			{
				mode = m_RightPoseMode.mode;
				return m_RightPoseMode.isValid;
			}
			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
			{
				mode = m_LeftPoseMode.mode;
				return m_LeftPoseMode.isValid;
			}

			mode = WVR_ControllerPoseMode.WVR_ControllerPoseMode_Raw;
			return false;
		}
		public bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			DEBUG("SetControllerPoseMode() " + type + ", " + mode);
			return Interop.WVR_SetControllerPoseMode(type, mode);
		}
		#endregion

		#region Interaction Mode
		XR_InteractionMode m_InteractionMode = XR_InteractionMode.Default;
		void UpdateInteractionMode()
		{
			m_InteractionMode = (XR_InteractionMode)Interop.WVR_GetInteractionMode();
			DEBUG("UpdateInteractionMode() m_InteractionMode = " + m_InteractionMode);
		}

		[System.Obsolete("This is an obsolete function.", true)]
		public void SetInteractionMode(XR_InteractionMode mode)
		{
			if (Interop.WVR_SetInteractionMode((WVR_InteractionMode)mode))
			{
				m_InteractionMode = mode;
				DEBUG("SetInteractionMode() " + m_InteractionMode);
			}
		}

		public XR_InteractionMode GetInteractionMode()
		{
#if UNITY_EDITOR
			bool EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			if(EnableDirectPreview) { return (XR_InteractionMode)Interop.WVR_GetInteractionMode(); }
#endif
			return m_InteractionMode;
		}
		private void OnInteractionModeChanged(WVR_Event_t systemEvent)
		{
			DEBUG("OnInteractionModeChanged()");
			UpdateInteractionMode();
		}
		#endregion

		#region Pose in Editor Playing Mode
#if UNITY_EDITOR
		public Vector3 GetPosition(WVR_DeviceType device) { return DummyPose.GetPosition(device); }
		public Quaternion GetRotation(WVR_DeviceType device) { return DummyPose.GetRotation(device); }
#endif
		#endregion
	}
}
