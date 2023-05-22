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
using UnityEngine.UI;
using UnityEngine.XR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

namespace Wave.XR.Sample.Controller
{
	[RequireComponent(typeof(Text))]
	public class WristTrackerData : MonoBehaviour
	{
		const string LOG_TAG = "Wave.XR.Sample.Controller.WristTrackerData";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
		bool printIntervalLog = false;
		int logFrame = 0;
		void INTERVAL(string msg) { if (printIntervalLog && !Application.isEditor) { DEBUG(msg); } }

		#region Inspector
		[SerializeField]
		private bool m_IsLeft = false;
		public bool IsLeft { get { return m_IsLeft; } set { m_IsLeft = value; } }

		[SerializeField]
		private InputActionReference m_IsTracked = null;
		public InputActionReference IsTracked { get { return m_IsTracked; } set { m_IsTracked = value; } }

		[SerializeField]
		private InputActionReference m_TrackingState = null;
		public InputActionReference TrackingState { get { return m_TrackingState; } set { m_TrackingState = value; } }

		[SerializeField]
		private InputActionReference m_Position = null;
		public InputActionReference Position { get { return m_Position; } set { m_Position = value; } }

		[SerializeField]
		private InputActionReference m_Rotation = null;
		public InputActionReference Rotation { get { return m_Rotation; } set { m_Rotation = value; } }

		[SerializeField]
		private InputActionReference m_MenuButton = null;
		public InputActionReference MenuButton { get { return m_MenuButton; } set { m_MenuButton = value; } }

		[SerializeField]
		private InputActionReference m_PrimaryButton = null;
		public InputActionReference PrimaryButton { get { return m_PrimaryButton; } set { m_PrimaryButton = value; } }
		#endregion

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

		#region MonoBehaviour
		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (m_Text == null) { return; }

			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			m_Text.text = m_IsLeft ? "Left Tracker: " : "Right Tracker: ";

			// isTracked
			if (Utils.GetButton(m_IsTracked, out bool tracked, out string trackedText))
			{
				m_Text.text += "Tracked: " + tracked;
			}
			else
			{
				m_Text.text += "Tracked: " + trackedText;
			}
			m_Text.text += "\n";

			// trackingState
			if (Utils.GetInteger(m_TrackingState, out InputTrackingState state, out string stateText))
			{
				m_Text.text += "State: " + state;
			}
			else
			{
				m_Text.text += "State: " + stateText;
			}
			m_Text.text += "\n";

			// position
			if (Utils.GetVector3(m_Position, out Vector3 pos, out string posText))
			{
				m_Text.text += "Position: " + "(" + pos.x.ToString() + ", " + pos.y.ToString() + ", " + pos.z.ToString() + ")";
			}
			else
			{
				m_Text.text += "Position: " + posText;
			}
			m_Text.text += "\n";

			// rotation
			if (Utils.GetQuaternion(m_Rotation, out Quaternion rot, out string rotText))
			{
				var direction = rot * Vector3.forward;
				m_Text.text += "Direction: " + "(" + direction.x.ToString() + ", " + direction.y.ToString() + ", " + direction.z.ToString() + ")";
			}
			else
			{
				m_Text.text += "Direction: " + rotText;
			}
			m_Text.text += "\n";

			// menu button
			if (Utils.GetButton(m_MenuButton, out bool menu, out string menuText))
			{
				m_Text.text += "Menu: " + menu;
			}
			else
			{
				m_Text.text += "Menu: " + false;
			}
			m_Text.text += "\n";

			// primary button
			if (Utils.GetButton(m_PrimaryButton, out bool primary, out string primaryText))
			{
				m_Text.text += "Primary: " + primary;
			}
			else
			{
				m_Text.text += "Primary: " + false;
			}
		}
		#endregion
	}
}
#endif
