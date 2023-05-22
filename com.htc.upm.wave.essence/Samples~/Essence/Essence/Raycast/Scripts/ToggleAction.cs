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
using Wave.Essence.Eye;

namespace Wave.Essence.Samples.Raycast
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleAction : MonoBehaviour
	{
		Toggle m_Toggle = null;

		private void Awake()
		{
			m_Toggle = GetComponent<Toggle>();
		}

		public void ActivateEyeTracking()
		{
			if (m_Toggle == null || EyeManager.Instance == null) { return; }

			EyeManager.Instance.EnableEyeTracking = m_Toggle.isOn;
		}
	}
}
