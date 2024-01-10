// "Wave SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Wave.OpenXR
{
	[RequireComponent(typeof(Text))]
	public class TrackerText : MonoBehaviour
	{
		public enum TextSelect
		{
			DeviceName,
			Role,
			Connection,
			Position,
			Rotation,
			Velocity,
			AngularVelocity,
			Acceleration,
			AngularAcceleration,
			Press,
			Battery,
		}

		#region Inspector
		public TextSelect TextFor = TextSelect.Connection;
		#endregion

		private Text m_Text = null;
		private InputDeviceTracker.TrackerId m_Tracker = InputDeviceTracker.TrackerId.Tracker0;
		private bool isInit = false;

		readonly Dictionary<InputFeatureUsage<bool>, string> s_ButtonsBinding = new Dictionary<InputFeatureUsage<bool>, string>()
		{
			{CommonUsages.menuButton, "MenuButton"},
			{CommonUsages.primaryButton, "PrimaryButton"},
			{CommonUsages.secondaryButton, "SecondaryButton"},
			{CommonUsages.primary2DAxisClick, "Touchpad"},
			{CommonUsages.triggerButton, "TriggerButton"}
		};


		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		private void Update()
		{
			if (m_Text == null || !isInit) { return; }

			
			switch (TextFor)
			{
				case TextSelect.DeviceName:
					m_Text.text = m_Tracker.Name() + " ";
					TextDeviceName();
					break;
				case TextSelect.Role: TextRole(); break;
				case TextSelect.Connection:TextConnection(); break;
				case TextSelect.Position: TextPosition(); break;
				case TextSelect.Rotation: TextRotation(); break;
				case TextSelect.Velocity: TextVelocity(); break;
				case TextSelect.AngularVelocity: TextAngularVelocity(); break;
				case TextSelect.Acceleration: TextAcceleration(); break;
				case TextSelect.AngularAcceleration: TextAngularAcceleration(); break;
				case TextSelect.Press: TextPress(); break;
				case TextSelect.Battery: TextBattery(); break;
				default:
					break;
			}
		}

		public void SetTrackerId(InputDeviceTracker.TrackerId tracker)
		{
			m_Tracker = tracker;
			isInit = true;
		}

		private void TextConnection()
		{
			if (InputDeviceTracker.IsAvailable(m_Tracker))
			{
				m_Text.text = "Connected: True";
				if (InputDeviceTracker.GetTrackingState(m_Tracker, out InputTrackingState state))
				{
					if (state == InputTrackingState.All) { m_Text.text += " | State : all"; }
					else
					{
						m_Text.text += " | State :";
						if (state.HasFlag(InputTrackingState.Position)) { m_Text.text += " P"; }
						if (state.HasFlag(InputTrackingState.Rotation)) { m_Text.text += " R"; }
						if (state.HasFlag(InputTrackingState.Velocity)) { m_Text.text += " V"; }
						if (state.HasFlag(InputTrackingState.AngularVelocity)) { m_Text.text += " AV"; }
						if (state.HasFlag(InputTrackingState.Acceleration)) { m_Text.text += " A"; }
						if (state.HasFlag(InputTrackingState.AngularAcceleration)) { m_Text.text += " AA"; }
					}
				}
				else
				{
					m_Text.text += " | State : invalid";
				}
			}
			else
			{
				m_Text.text = "Connected: False";
			}
		}

		private void TextRole()
		{
			m_Text.text = "Role: " + InputDeviceTracker.GetRole(m_Tracker);
		}

		private void TextPosition()
		{
			InputDeviceTracker.GetPosition(m_Tracker, out Vector3 pos);
			m_Text.text = "Position x: " + pos.x.ToString("F3") + ", y: " + pos.y.ToString("F3") + ", z: " + pos.z.ToString("F3");
		}

		private void TextRotation()
		{
			InputDeviceTracker.GetRotation(m_Tracker, out Quaternion rot);
			Vector3 eulerAngles = rot.eulerAngles;
			m_Text.text = "Rotation x: " + rot.x.ToString("F3") + ", y: " + rot.y.ToString("F3") + ", z: " + rot.z.ToString("F3") + ", w: " + rot.w.ToString("F3");
		}

		private void TextVelocity()
		{
			InputDeviceTracker.GetVelocity(m_Tracker, out Vector3 vel);
			m_Text.text = "Velocity x: " + vel.x.ToString("F3") + ", y: " + vel.y.ToString("F3") + ", z: " + vel.z.ToString("F3");
		}

		private void TextAngularVelocity()
		{
			InputDeviceTracker.GetAngularVelocity(m_Tracker, out Vector3 vel);
			m_Text.text = "AngularVel x: " + vel.x.ToString("F3") + ", y: " + vel.y.ToString("F3") + ", z: " + vel.z.ToString("F3");
		}

		private void TextAcceleration()
		{
			InputDeviceTracker.GetAcceleration(m_Tracker, out Vector3 acc);
			m_Text.text = "Acceleration x: " + acc.x.ToString("F3") + ", y: " + acc.y.ToString("F3") + ", z: " + acc.z.ToString("F3");
		}

		private void TextAngularAcceleration()
		{
			InputDeviceTracker.GetAngularAcceleration(m_Tracker, out Vector3 acc);
			m_Text.text = "AngularAcc x: " + acc.x.ToString("F3") + ", y: " + acc.y.ToString("F3") + ", z: " + acc.z.ToString("F3");
		}

		private void TextPress()
		{
			m_Text.text = "Pressed:";
			for(int i=0; i<s_ButtonsBinding.Count; i++)
			{
				var button = s_ButtonsBinding.ElementAt(i);
				if (InputDeviceTracker.ButtonDown(m_Tracker, button.Key, out bool value))
				{
					if (value)
					{
						m_Text.text += $" {button.Value} ";
						InputDeviceTracker.HapticPulse(m_Tracker);
					}
				}
			}
		}

		private void TextBattery()
		{
			InputDeviceTracker.BatteryLevel(m_Tracker, out float level);
			m_Text.text = "Battery: " + level * 100 + "%";
		}

		private void TextDeviceName()
		{
			InputDeviceTracker.GetTrackerDeviceName(m_Tracker, out string trackerName);
			m_Text.text += trackerName;
		}
	}
}
