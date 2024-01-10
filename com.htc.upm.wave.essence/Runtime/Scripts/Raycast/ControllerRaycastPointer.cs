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
using Wave.Native;
using Wave.Essence.Events;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wave.Essence.Raycast
{
	public class ControllerRaycastPointer : RaycastPointer
	{
		const string LOG_TAG = "Wave.Essence.Raycast.ControllerRaycastPointer";
		private void DEBUG(StringBuilder msg) { Log.d(LOG_TAG, msg, true); }

		[Serializable]
		public class ButtonOption
		{
			[SerializeField]
			private bool m_Primary2DAxisClick = false;
			public bool Primary2DAxisClick
			{
				get { return m_Primary2DAxisClick; }
				set
				{
					if (m_Primary2DAxisClick != value) { Update(); }
					m_Primary2DAxisClick = value;
				}
			}
			[SerializeField]
			private bool m_TriggerButton = true;
			public bool TriggerButton
			{
				get { return m_TriggerButton; }
				set
				{
					if (m_TriggerButton != value) { Update(); }
					m_TriggerButton = value;
				}
			}

			private List<InputFeatureUsage<bool>> m_OptionList = new List<InputFeatureUsage<bool>>();
			public List<InputFeatureUsage<bool>> OptionList { get { return m_OptionList; } }

			[HideInInspector]
			public List<bool> State = new List<bool>(), StateEx = new List<bool>();
			public void Update()
			{
				m_OptionList.Clear();
				State.Clear();
				StateEx.Clear();
				if (m_Primary2DAxisClick)
				{
					m_OptionList.Add(XR_BinaryButton.primary2DAxisClick);
					State.Add(false);
					StateEx.Add(false);
				}
				if (m_TriggerButton)
				{
					m_OptionList.Add(XR_BinaryButton.triggerButton);
					State.Add(false);
					StateEx.Add(false);
				}
			}
		}

		#region Inspector
		[Tooltip("The type of controller.")]
		[SerializeField]
		private XR_Hand m_Controller = XR_Hand.Right;
		public XR_Hand Controller { get { return m_Controller; } set { m_Controller = value; } }

		[Tooltip("Buttons to controller the raycast.")]
		[SerializeField]
		private ButtonOption m_ControlKey = new ButtonOption();
		public ButtonOption ControlKey { get { return m_ControlKey; } set { m_ControlKey = value; } }

		[Tooltip("To hide the ray when the controller is idle.")]
		[SerializeField]
		private bool m_HideWhenIdle = true;
		public bool HideWhenIdle { get { return m_HideWhenIdle; } set { m_HideWhenIdle = value; } }

		[Tooltip("To show the ray anymore.")]
		[SerializeField]
		private bool m_AlwaysEnable = false;
		public bool AlwaysEnable { get { return m_AlwaysEnable; } set { m_AlwaysEnable = value; } }
		#endregion

		#region MonoBehaviour overrides
		protected override void Awake()
		{
			base.Awake();

			m_ControlKey.Update();
			for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
			{
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("Awake() m_ControlKey[").Append(i).Append("] = ").Append(m_ControlKey.OptionList[i].name);
				DEBUG(sb);
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();

			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceTableStaticLocked, OnTableStaticLocked);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceTableStaticUnlocked, OnTableStaticUnocked);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceConnected, OnDeviceConnected);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceDisconnected, OnDeviceDisconnected);
		}
		protected override void OnDisable()
		{
			base.OnDisable();

			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceTableStaticLocked, OnTableStaticLocked);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceTableStaticUnlocked, OnTableStaticUnocked);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceConnected, OnDeviceConnected);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceDisconnected, OnDeviceDisconnected);
		}
		protected override void Update()
		{
			base.Update();

			if (!IsInteractable()) { return; }

			UpdateButtonStates();

			if (printIntervalLog)
			{
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("Update() m_ControlKey.Primary2DAxisClick: ").Append(m_ControlKey.Primary2DAxisClick)
					.Append(", m_ControlKey.TriggerButton: ").Append(m_ControlKey.TriggerButton);
				DEBUG(sb);
			}
		}
		protected override void Start()
		{
			base.Start();

			m_TableStatic = Interop.WVR_IsDeviceTableStatic((WVR_DeviceType)m_Controller);
			sb.Clear().Append(m_Controller.Name()).Append(" ").Append("Start() m_TableStatic = ").Append(m_TableStatic); DEBUG(sb);
		}
		private void OnApplicationPause(bool pause)
		{
			sb.Clear().Append(m_Controller.Name()).Append(" ").Append("OnApplicationPause() ").Append(pause); DEBUG(sb);
			if (!pause)
			{
				m_TableStatic = Interop.WVR_IsDeviceTableStatic((WVR_DeviceType)m_Controller);
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("Resume, m_TableStatic = ").Append(m_TableStatic); DEBUG(sb);
			}
		}
		#endregion

		private bool m_TableStatic = false;
		private void OnTableStaticLocked(WVR_Event_t systemEvent)
		{
			var deviceType = systemEvent.device.type;
			if (deviceType == (WVR_DeviceType)m_Controller)
			{
				m_TableStatic = Interop.WVR_IsDeviceTableStatic(deviceType);
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("OnTableStaticLocked() m_TableStatic = ").Append(m_TableStatic); DEBUG(sb);
			}
		}
		private void OnTableStaticUnocked(WVR_Event_t systemEvent)
		{
			var deviceType = systemEvent.device.type;
			if (deviceType == (WVR_DeviceType)m_Controller)
			{
				m_TableStatic = Interop.WVR_IsDeviceTableStatic(deviceType);
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("OnTableStaticUnocked() m_TableStatic = ").Append(m_TableStatic); DEBUG(sb);
			}
		}
		private void OnDeviceConnected(WVR_Event_t systemEvent)
		{
			var deviceType = systemEvent.device.type;
			if (deviceType == (WVR_DeviceType)m_Controller)
			{
				m_TableStatic = false;
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("OnDeviceConnected() set m_TableStatic to false."); DEBUG(sb);
			}
		}
		private void OnDeviceDisconnected(WVR_Event_t systemEvent)
		{
			var deviceType = systemEvent.device.type;
			if (deviceType == (WVR_DeviceType)m_Controller)
			{
				m_TableStatic = false;
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("OnDeviceDisconnected() set m_TableStatic to false."); DEBUG(sb);
			}
		}

		private bool IsInteractable()
		{
			bool enabled = RaycastSwitch.Controller.Enabled;
			bool validPose = WXRDevice.IsTracked((XR_Device)m_Controller);
			bool hasFocus = ClientInterface.IsFocused;
			bool hideWhenIdle = m_HideWhenIdle && m_TableStatic;

			m_Interactable = (m_AlwaysEnable || enabled) && validPose && hasFocus && (!hideWhenIdle);

			if (printIntervalLog)
			{
				sb.Clear().Append(m_Controller.Name()).Append(" ").Append("IsInteractable() enabled: ").Append(enabled)
					.Append(", validPose: ").Append(validPose)
					.Append(", hasFocus: ").Append(hasFocus)
					.Append(", m_HideWhenIdle: ").Append(m_HideWhenIdle)
					.Append(", m_TableStatic: ").Append(m_TableStatic)
					.Append(", hideWhenIdle: ").Append(hideWhenIdle)
					.Append(", m_AlwaysEnable: ").Append(m_AlwaysEnable)
					.Append(", m_Interactable: ").Append(m_Interactable);
				DEBUG(sb);
			}

			return m_Interactable;
		}

		private void UpdateButtonStates()
		{
			down = false;
			hold = false;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
				{
					down |= WXRDevice.ButtonPress(
						(WVR_DeviceType)m_Controller,
						m_ControlKey.OptionList[i].ViveFocus3Button(m_Controller == XR_Hand.Left)
						);
					hold |= WXRDevice.ButtonHold(
						(WVR_DeviceType)m_Controller,
						m_ControlKey.OptionList[i].ViveFocus3Button(m_Controller == XR_Hand.Left)
						);
				}
			} else
#endif
			{
				for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
				{
					m_ControlKey.StateEx[i] = m_ControlKey.State[i];
					m_ControlKey.State[i] = WXRDevice.KeyDown((XR_Device)m_Controller, m_ControlKey.OptionList[i]);

					down |= (m_ControlKey.State[i] && !m_ControlKey.StateEx[i]);
					hold |= (m_ControlKey.State[i]);

					if (down)
					{
						sb.Clear().Append(m_Controller.Name()).Append(" ").Append("UpdateButtonStates() ").Append(", ").Append(m_ControlKey.OptionList[i].name).Append(", down: ").Append(down);
						DEBUG(sb);
					}
				}
			}
		}

		#region RaycastImpl Actions overrides
		internal bool down = false, hold = false;
		protected override bool OnDown()
		{
			return down;
		}
		protected override bool OnHold()
		{
			return hold;
		}
		#endregion
	}
}
