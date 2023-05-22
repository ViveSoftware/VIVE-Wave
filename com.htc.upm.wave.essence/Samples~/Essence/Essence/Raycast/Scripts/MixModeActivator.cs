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

namespace Wave.Essence.Samples.Raycast
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class MixModeActivator : MonoBehaviour
	{
		private Text m_Text = null;
		private bool m_Activated = false;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (m_Text == null) { return; }

			m_Text.text = (m_Activated ? "Mixed Mode" : "Unmix Mode");
		}
		public void SetMixMode()
		{
			m_Activated = !m_Activated;
			Log.d("Wave.Essence.Samples.Raycast.MixModeActivator", "WVR_SetMixMode(" + m_Activated + ")", true);
#pragma warning disable 612, 618
			Interop.WVR_SetMixMode(m_Activated);
#pragma warning restore 612, 618
		}
	}
}
