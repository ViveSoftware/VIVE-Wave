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
using Wave.Essence.Hand;
using Wave.Essence.Raycast;

namespace Wave.Essence.Samples.Raycast
{
	[RequireComponent(typeof(Text))]
	public class PinchText : MonoBehaviour
	{
		public GazeRaycastRing gaze = null;
		public HandRaycastPointer handL = null;
		public HandRaycastPointer handR = null;

		Text m_Text = null;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		void Update()
		{
			if (m_Text == null || HandManager.Instance == null) { return; }

			m_Text.text = "Threshold --\n";
			m_Text.text += "On: " + HandManager.Instance.GetPinchThreshold();
			m_Text.text += ", Off: " + HandManager.Instance.GetPinchOffThreshold() + "\n";
			m_Text.text += "Strength --\n";
			m_Text.text += "Left: " + HandManager.Instance.GetPinchStrength(true) + "\n";
			m_Text.text += "Right: " + HandManager.Instance.GetPinchStrength(false) + "\n";

			m_Text.text += "Gaze --\n";
			if (gaze != null)
			{
				m_Text.text += "IsPinching(L): " + gaze.pinchedL + "\n";
				m_Text.text += "IsPinching(R): " + gaze.pinchedR + "\n";
			}
			m_Text.text += "Left Hand --\n";
			if (handL != null)
			{
				m_Text.text += "On: " + handL.PinchStrength;
				m_Text.text += ", Off: " + (handL.PinchStrength - handL.PinchRelease) + "\n";
			}
			m_Text.text += "Right Hand --\n";
			if (handR != null)
			{
				m_Text.text += "On: " + handR.PinchStrength;
				m_Text.text += ", Off: " + (handR.PinchStrength - handR.PinchRelease) + "\n";
			}
		}
	}
}
