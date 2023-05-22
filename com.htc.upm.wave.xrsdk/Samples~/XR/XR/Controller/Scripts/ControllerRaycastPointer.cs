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
using System;
using System.Collections.Generic;
using Wave.OpenXR;

namespace Wave.XR.Sample.Controller
{
	public class ControllerRaycastPointer : RaycastPointer
	{
		const string LOG_TAG = "Wave.XR.Sample.Controller.ControllerRaycastPointer";
		private void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + m_Controller + " " + msg); }
		private void INTERVAL(string msg) { if (printIntervalLog) { DEBUG(msg); } }

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
					m_OptionList.Add(CommonUsages.primary2DAxisClick);
					State.Add(false);
					StateEx.Add(false);
				}
				if (m_TriggerButton)
				{
					m_OptionList.Add(CommonUsages.triggerButton);
					State.Add(false);
					StateEx.Add(false);
				}
			}
		}

		#region Inspector
		[Tooltip("The type of controller.")]
		[SerializeField]
		private InputDeviceControl.ControlDevice m_Controller = InputDeviceControl.ControlDevice.Right;
		public InputDeviceControl.ControlDevice Controller { get { return m_Controller; } set { m_Controller = value; } }

		[Tooltip("Buttons to controller the raycast.")]
		[SerializeField]
		private ButtonOption m_ControlKey = new ButtonOption();
		public ButtonOption ControlKey { get { return m_ControlKey; } set { m_ControlKey = value; } }
		#endregion

		#region MonoBehaviour overrides
		protected override void Awake()
		{
			base.Awake();

			m_ControlKey.Update();
			for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
			{
				DEBUG("Awake() m_ControlKey[" + i + "] = " + m_ControlKey.OptionList[i].name);
			}
		}
		protected override void Update()
		{
			base.Update();

			UpdateButtonStates();

			INTERVAL("Update() m_ControlKey.Primary2DAxisClick: " + m_ControlKey.Primary2DAxisClick
				+ ", m_ControlKey.TriggerButton: " + m_ControlKey.TriggerButton
				);
		}
		#endregion

		private void UpdateButtonStates()
		{
			down = false;
			hold = false;

			for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
			{
				m_ControlKey.StateEx[i] = m_ControlKey.State[i];
				m_ControlKey.State[i] = InputDeviceControl.KeyDown(m_Controller, m_ControlKey.OptionList[i]);

				down |= (m_ControlKey.State[i] && !m_ControlKey.StateEx[i]);
				hold |= (m_ControlKey.State[i]);

				if (down)
				{
					DEBUG("UpdateButtonStates() " +
						", " + m_ControlKey.OptionList[i].name +
						", down: " + down);
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
