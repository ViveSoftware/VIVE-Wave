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
using UnityEngine;
using UnityEngine.XR;
using Wave.Native;
using Wave.Essence.Eye;
using Wave.Essence.Hand;
using System.Text;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.Essence.Raycast
{
	public class GazeRaycastRing : RaycastRing
	{
		const string LOG_TAG = "Wave.Essence.Raycast.GazeRaycastRing";
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

		[Serializable]
		public class PinchOption
		{
			[HideInInspector]
			public bool IsPinching = false;

			[SerializeField]
			private bool m_LeftHand = false;
			public bool LeftHand { get { return m_LeftHand; } set { m_LeftHand = value; } }

			[SerializeField]
			private bool m_RightHand = false;
			public bool RightHand { get { return m_RightHand; } set { m_RightHand = value; } }

			[SerializeField]
			private bool m_DefaultPinch = false;
			public bool DefaultPinch { get { return m_DefaultPinch; } set { m_DefaultPinch = value; } }
		}

		#region Inspector
		[SerializeField]
		[Tooltip("Use Eye Tracking data for Gaze.")]
		private bool m_EyeTracking = false;
		public bool EyeTracking { get { return m_EyeTracking; } set { m_EyeTracking = value; } }

		[Tooltip("Which eye's data")]
		[SerializeField]
		private EyeManager.EyeType m_Eye = EyeManager.EyeType.Combined;
		public EyeManager.EyeType Eye { get { return m_Eye; } set { m_Eye = value; } }

		[Tooltip("Event triggered by gaze.")]
		[SerializeField]
		private GazeEvent m_InputEvent = GazeEvent.Down;
		public GazeEvent InputEvent { get { return m_InputEvent; } set { m_InputEvent = value; } }

		[SerializeField]
		private ButtonOption m_ControlKey = new ButtonOption();
		public ButtonOption ControlKey { get { return m_ControlKey; } set { m_ControlKey = value; } }

		[SerializeField]
		private PinchOption m_ControlHand = new PinchOption();
		public PinchOption ControlHand { get { return m_ControlHand; } set { m_ControlHand = value; } }

#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = false;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		[SerializeField]
		private InputActionProperty m_RotationInput;
		public InputActionProperty RotationInput
		{
			get => m_RotationInput;
			set => m_RotationInput = value;
		}
		public static bool VALIDATE(InputActionProperty actionReference, out string msg)
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

			return true;
		}
#endif

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
				sb.Clear().Append("Awake() m_ControlKey[").Append(i).Append("] = ").Append(m_ControlKey.OptionList[i].name);
				DEBUG(sb);
			}
		}

		private bool m_KeyDown = false, m_HandPinch = false;
		protected override void Update()
		{
			base.Update();

			if (!IsInteractable()) { return; }

			m_KeyDown = ButtonPressed();
			m_HandPinch = m_ControlHand.DefaultPinch ? CheckHandPinchedByDefault() : CheckHandPinchedByThreshold();

			if (printIntervalLog)
			{
				sb.Clear().Append("Update() m_InputEvent: ").Append(m_InputEvent.Name())
					.Append(", m_AlwaysEnable: ").Append(m_AlwaysEnable)
					.Append(", m_ControlKey.Primary2DAxisClick: ").Append(m_ControlKey.Primary2DAxisClick)
					.Append(", m_ControlKey.TriggerButton: ").Append(m_ControlKey.TriggerButton)
					.Append(", m_ControlHand.LeftHand: ").Append(m_ControlHand.LeftHand)
					.Append(", m_ControlHand.RightHand: ").Append(m_ControlHand.RightHand)
					.Append(", m_ControlHand.DefaultPinch: ").Append(m_ControlHand.DefaultPinch);
				DEBUG(sb);
			}
		}
		#endregion

		private bool IsInteractable()
		{
			bool enabled = RaycastSwitch.Gaze.Enabled;
			bool hasFocus = ClientInterface.IsFocused;

			m_Interactable = (m_AlwaysEnable || enabled) && hasFocus;

			if (printIntervalLog)
			{
				sb.Clear().Append("IsInteractable() enabled: ").Append(enabled).Append(", hasFocus: ").Append(hasFocus).Append(", m_AlwaysEnable: ").Append(m_AlwaysEnable);
				DEBUG(sb);
			}

			return m_Interactable;
		}

		private bool ButtonPressed()
		{
			bool down = false;

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
				{
					down |=
						WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left,m_ControlKey.OptionList[i].ViveFocus3Button(true)) ||
						WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, m_ControlKey.OptionList[i].ViveFocus3Button(false));
				}
			}
			else
#endif
			{
				for (int i = 0; i < m_ControlKey.OptionList.Count; i++)
				{
					m_ControlKey.StateEx[i] = m_ControlKey.State[i];
					m_ControlKey.State[i] =
						WXRDevice.KeyDown(XR_Device.Left, m_ControlKey.OptionList[i]) ||
						WXRDevice.KeyDown(XR_Device.Right, m_ControlKey.OptionList[i]);

					down |= (m_ControlKey.State[i] && !m_ControlKey.StateEx[i]);
				}
			}

			return down;
		}

		const float kPinchDefOn = .5f, kPinchDiff = .3f;
		public bool pinchedL = false, pinchedR = false;
		private bool CheckHandPinchedByThreshold()
		{
			float pinchThreshold = kPinchDefOn;
			float pinchDiff = kPinchDiff;
			if (HandManager.Instance.GetPinchThreshold(HandManager.TrackerType.Natural, out float thresholdOn)) { pinchThreshold = thresholdOn; }
			if (HandManager.Instance.GetPinchOffThreshold(HandManager.TrackerType.Natural, out float thresholdOff)) { pinchDiff = pinchThreshold - thresholdOff; }

			bool pinched = false;

			var pinchStrengthL = HandManager.Instance.GetPinchStrength(true);
			pinchedL = (pinchStrengthL > pinchThreshold);
			var pinchStrengthR = HandManager.Instance.GetPinchStrength(false);
			pinchedR = (pinchStrengthR > pinchThreshold);

			if (!m_ControlHand.IsPinching)
			{
				if (m_ControlHand.LeftHand) { pinched |= pinchedL; }
				if (m_ControlHand.RightHand) { pinched |= pinchedR; }
				m_ControlHand.IsPinching = pinched;
			}
			else
			{
				bool hold = false;
				if (m_ControlHand.LeftHand)
				{
					hold |= pinchThreshold > kPinchDefOn ?
						pinchStrengthL > (pinchThreshold - pinchDiff) : pinchStrengthL > kPinchDefOn;
				}
				if (m_ControlHand.RightHand)
				{
					hold |= pinchThreshold > kPinchDefOn ?
						pinchStrengthR > (pinchThreshold - pinchDiff) : pinchStrengthR > kPinchDefOn;
				}
				if (!hold)
				{
					m_ControlHand.IsPinching = false;
				}
			}

			return pinched;
		}
		private bool CheckHandPinchedByDefault()
		{
			bool pinched = false;

			pinchedL = HandManager.Instance.IsHandPinching(true);
			pinchedR = HandManager.Instance.IsHandPinching(false);

			if (!m_ControlHand.IsPinching)
			{
				if (m_ControlHand.LeftHand) { pinched |= pinchedL; }
				if (m_ControlHand.RightHand) { pinched |= pinchedR; }
				m_ControlHand.IsPinching = pinched;
			}
			else
			{
				bool hold = false;
				if (m_ControlHand.LeftHand) { hold |= pinchedL; }
				if (m_ControlHand.RightHand) { hold |= pinchedR; }
				if (!hold)
				{
					m_ControlHand.IsPinching = false;
				}
			}

			return pinched;
		}

		protected override bool UseEyeData(out Vector3 direction, out EyeManager.EyeSpace space)
		{
			if (printIntervalLog)
			{
				sb.Clear().Append("UseEyeData() m_EyeTracking: ").Append(m_EyeTracking).Append(", m_Eye: ").Append(m_Eye);
				DEBUG(sb);
			}

			if (m_EyeTracking)
			{
#if ENABLE_INPUT_SYSTEM
				if (printIntervalLog)
				{
					sb.Clear().Append("UseEyeData() m_UseInputAction: ").Append(m_UseInputAction);
					DEBUG(sb);
				}
				if (m_UseInputAction)
				{
					if (VALIDATE(m_RotationInput, out string msg))
					{
						if (!m_RotationInput.action.enabled)
						{
							sb.Clear().Append("UseEyeData() enable ").Append(m_RotationInput.action.name); DEBUG(sb);
							m_RotationInput.action.Enable();
						}
						direction = m_RotationInput.action.ReadValue<Quaternion>() * Vector3.forward;
						space = EyeManager.EyeSpace.World;
						return true;
					}
					else
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("UseEyeData() ").Append(msg);
							DEBUG(sb);
						}
					}
				}
				else
				{
					if ((EyeManager.Instance != null) && EyeManager.Instance.HasEyeTrackingData())
					{
						direction = Vector3.forward;
						if (EyeManager.Instance.GetEyeDirectionNormalized(m_Eye, out Vector3 value))
						{
							direction = value;
						}
						space = EyeManager.Instance.LocationSpace;
						if (printIntervalLog)
						{
							sb.Clear().Append("UseEyeData() space: ").Append(space)
								.Append(", direction (").Append(direction.x).Append(", ").Append(direction.y).Append(", ").Append(direction.z).Append(")");
							DEBUG(sb);
						}
						return true;
					}
					else
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("UseEyeData() no data.");
							DEBUG(sb);
						}
					}
				}
#else
				if ((EyeManager.Instance != null) && EyeManager.Instance.HasEyeTrackingData())
				{
					direction = Vector3.forward;
					if (EyeManager.Instance.GetEyeDirectionNormalized(m_Eye, out Vector3 value))
					{
						direction = value;
					}
					space = EyeManager.Instance.LocationSpace;
					return true;
				}
#endif
			}

			return base.UseEyeData(out direction, out space);
		}

		#region RaycastImpl Actions overrides
		protected override bool OnDown()
		{
			if (m_InputEvent != GazeEvent.Down) { return false; }

			bool down = false;
			if (m_RingPercent >= 100 || m_KeyDown || m_HandPinch)
			{
				m_RingPercent = 0;
				m_GazeOnTime = Time.unscaledTime;
				down = true;

				sb.Clear().Append("OnDown()"); DEBUG(sb);
			}

			return down;
		}

		protected override bool OnSubmit()
		{
			if (m_InputEvent != GazeEvent.Submit) { return false; }

			bool submit = false;
			if (m_RingPercent >= 100 || m_KeyDown | m_HandPinch)
			{
				m_RingPercent = 0;
				m_GazeOnTime = Time.unscaledTime;
				submit = true;

				sb.Clear().Append("OnSubmit()"); DEBUG(sb);
			}

			return submit;
		}
		#endregion
	}
}
