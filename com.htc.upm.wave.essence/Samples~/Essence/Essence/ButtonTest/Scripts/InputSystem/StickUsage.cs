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
using UnityEngine.UI;
using Wave.Native;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.Essence.Samples.ButtonTest
{
	public class StickUsage : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Samples.ButtonTest.StickUsage";
		void DEBUG(string msg) { Log.d(LOG_TAG, gameObject.name + " " + msg, true); }
		bool printIntervalLog = false;
#pragma warning disable
		int logFrame = 0;
#pragma warning enable
		void INTERVAL(string msg) { if (printIntervalLog && !Application.isEditor) { DEBUG(msg); } }

		public string usageName;

#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private InputActionReference m_StickButton = null;
		public InputActionReference StickButton { get { return m_StickButton; } set { m_StickButton = value; } }
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
		private void GetButtonValue(InputActionReference actionReference, out Vector2 value, out string msg)
		{
			value = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
			if (VALIDATE(actionReference, out msg))
			{
				if (actionReference.action.activeControl.valueType == typeof(Vector2))
					value = actionReference.action.ReadValue<Vector2>();

				INTERVAL("GetButtonValue(" + value.x + ", " + value.y + ")");
			}
			else
			{
				INTERVAL("GetButtonValue() invalid input: " + msg);
			}
#endif
		}
#endif

		public Text textComponent;
		public Slider horizontalSliderComponent;
		public Slider verticalSliderComponent;
		public Text valueTextComponent;

		private void Start()
		{
			if (textComponent != null)
			{
				textComponent.text = usageName;
			}
		}

		void Update()
		{
#if ENABLE_INPUT_SYSTEM
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			GetButtonValue(m_StickButton, out Vector2 value, out string msg);

			if (horizontalSliderComponent != null)
				horizontalSliderComponent.value = value.x;

			if (verticalSliderComponent != null)
				verticalSliderComponent.value = value.y;

			if (valueTextComponent != null)
				valueTextComponent.text = string.Format("[{0},{1}]", value.x.ToString("F"), value.y.ToString("F"));
#endif
		}
	}
}
