// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using Wave.Essence.Raycast;
using Wave.Essence.Hand;

namespace Wave.Essence.Samples.Raycast
{
	[RequireComponent(typeof(Text))]
	public class GazeRingData : MonoBehaviour
	{
		public GazeRaycastRing gaze = null;

		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (gaze == null || m_Text == null || HandManager.Instance == null) { return; }

			string threshold = HandManager.Instance.GetPinchThreshold().ToString("N2");
			m_Text.text = "Threshold: " + threshold;

			bool isPinching = gaze.ControlHand.IsPinching;
			m_Text.text += "\nIsPinching: " + isPinching;

			string strengthL = HandManager.Instance.GetPinchStrength(true).ToString("N2");
			m_Text.text += "\nLeft Strength: " + strengthL;
			m_Text.text += "\nLeft Pinch: " + gaze.pinchedL;

			string strengthR = HandManager.Instance.GetPinchStrength(false).ToString("N2");
			m_Text.text += "\nRight Strength: " + strengthR;
			m_Text.text += "\nRight Pinch: " + gaze.pinchedR;
		}
	}
}
