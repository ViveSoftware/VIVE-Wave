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
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class ChangeEyeSpace : MonoBehaviour
	{
		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (EyeManager.Instance == null || m_Text == null) { return; }

			EyeManager.EyeSpace space = EyeManager.Instance.LocationSpace;
			m_Text.text = space + " Space";
		}

		public void ChangeSpace()
		{
			if (EyeManager.Instance == null) { return; }

			if (EyeManager.Instance.LocationSpace == EyeManager.EyeSpace.World)
				EyeManager.Instance.LocationSpace = EyeManager.EyeSpace.Local;
			else
				EyeManager.Instance.LocationSpace = EyeManager.EyeSpace.World;
		}
	}
}
