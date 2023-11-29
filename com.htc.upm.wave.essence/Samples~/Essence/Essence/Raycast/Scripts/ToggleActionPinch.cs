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
using Wave.Essence.Raycast;

namespace Wave.Essence.Samples.Raycast
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleActionPinch : MonoBehaviour
	{
		public GazeRaycastRing gaze = null;
		public HandRaycastPointer handL = null;
		public HandRaycastPointer handR = null;

		Toggle m_Toggle = null;

		private void Awake()
		{
			m_Toggle = GetComponent<Toggle>();
		}

		public void UseDefault()
		{
			if (m_Toggle == null) { return; }

			if (gaze != null) { gaze.ControlHand.DefaultPinch = m_Toggle.isOn; }
			if (handL != null) { handL.UseDefaultPinch = m_Toggle.isOn; }
			if (handR != null) { handR.UseDefaultPinch = m_Toggle.isOn; }
		}
	}
}
