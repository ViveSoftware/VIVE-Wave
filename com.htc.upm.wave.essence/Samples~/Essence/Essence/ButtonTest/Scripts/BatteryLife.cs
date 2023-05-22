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

namespace Wave.Essence.Samples.ButtonTest
{
	[RequireComponent(typeof(Text))]
	public class BatteryLife : MonoBehaviour
	{
		[SerializeField]
		private XR_Device m_TrackedDevice = XR_Device.Dominant;
		public XR_Device TrackedDevice { get { return m_TrackedDevice; } set { m_TrackedDevice = value; } }

		Text m_Text = null;
		void OnEnable()
		{
			m_Text = GetComponent<Text>();
		}

		void Update()
		{
			if (m_Text != null)
			{
				float battery_percent = WXRDevice.GetBatteryLevel(m_TrackedDevice) * 100;
				if (battery_percent >= 0)
					m_Text.text = "Battery: " + battery_percent.ToString() + "%";
				else
					m_Text.text = "Battery: Invalid.";
			}
		}
	}
}
