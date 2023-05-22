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

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

namespace Wave.XR.Sample.Controller
{
	[RequireComponent(typeof(Text))]
	public class EyeGazeData : MonoBehaviour
	{
		const string LOG_TAG = "Wave.XR.Sample.Controller.EyeGazeData";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
		bool printIntervalLog = false;
		int logFrame = 0;
		void INTERVAL(string msg) { if (printIntervalLog && !Application.isEditor) { DEBUG(msg); } }

		#region Inspector
		[SerializeField]
		private InputActionReference m_IsTracked = null;
		public InputActionReference IsTracked { get { return m_IsTracked; } set { m_IsTracked = value; } }
		private bool GetButtonValue(InputActionReference actionReference, out bool value, out string msg)
		{
			value = false;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(float))
					value = actionReference.action.ReadValue<float>() > 0;
				if (actionReference.action.activeControl.valueType == typeof(bool))
					value = actionReference.action.ReadValue<bool>();

				INTERVAL("GetButtonValue(" + value + ")");
				return true;
			}
			else
			{
				INTERVAL("GetButtonValue() invalid input: " + msg);
			}

			return false;
		}

		[SerializeField]
		private InputActionReference m_TrackingState = null;
		public InputActionReference TrackingState { get { return m_TrackingState; } set { m_TrackingState = value; } }
		private bool GetIntegerValue(InputActionReference actionReference, out int value, out string msg)
		{
			value = 0;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(int))
					value = actionReference.action.ReadValue<int>();

				INTERVAL("GetIntegerValue(" + value + ")");
				return true;
			}
			else
			{
				INTERVAL("GetIntegerValue() invalid input: " + msg);
			}

			return false;
		}

		[SerializeField]
		private InputActionReference m_Position = null;
		public InputActionReference Position { get { return m_Position; } set { m_Position = value; } }
		private bool GetVector3Value(InputActionReference actionReference, out Vector3 value, out string msg)
		{
			value = Vector3.zero;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(Vector3))
					value = actionReference.action.ReadValue<Vector3>();

				INTERVAL("GetVector3Value(" + value.x.ToString() + ", " + value.y.ToString() + ", " + value.z.ToString() + ")");
				return true;
			}
			else
			{
				INTERVAL("GetVector3Value() invalid input: " + msg);
			}

			return false;
		}

		[SerializeField]
		private InputActionReference m_Rotation = null;
		public InputActionReference Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
		private bool GetQuaternionValue(InputActionReference actionReference, out Quaternion value, out string msg)
		{
			value = Quaternion.identity;

			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(Quaternion))
					value = actionReference.action.ReadValue<Quaternion>();

				Vector3 direction = value * Vector3.forward;
				INTERVAL("GetQuaternionValue(" + direction.x.ToString() + ", " + direction.y.ToString() + ", " + direction.z.ToString() + ")");
				return true;
			}
			else
			{
				INTERVAL("GetQuaternionValue() invalid input: " + msg);
			}

			return false;
		}
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

			m_Text.text = "Eye: ";

			// isTracked
			if (GetButtonValue(m_IsTracked, out bool tracked, out string trackedText))
			{
				m_Text.text += "Tracked: " + tracked;
			}
			else
			{
				m_Text.text += "Tracked: " + trackedText;
			}
			m_Text.text += "\n";

			// trackingState
			if (GetIntegerValue(m_TrackingState, out int state, out string stateText))
			{
				m_Text.text += "State: " + state;
			}
			else
			{
				m_Text.text += "State: " + stateText;
			}
			m_Text.text += "\n";

			// position
			if (GetVector3Value(m_Position, out Vector3 pos, out string posText))
			{
				m_Text.text += "Position: " + "(" + pos.x.ToString() + ", " + pos.y.ToString() + ", " + pos.z.ToString() + ")";
			}
			else
			{
				m_Text.text += "Position: " + posText ;
			}
			m_Text.text += "\n";

			// rotation
			if (GetQuaternionValue(m_Rotation, out Quaternion rot, out string rotText))
			{
				var direction = rot * Vector3.forward;
				m_Text.text += "Direction: " + "(" + direction.x.ToString() + ", " + direction.y.ToString() + ", " + direction.z.ToString() + ")";
			}
			else
			{
				m_Text.text += "Direction: " + rotText;
			}
		}
		#endregion
	}
}
#endif
