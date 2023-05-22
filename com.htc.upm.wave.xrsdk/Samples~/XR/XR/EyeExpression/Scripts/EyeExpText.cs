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
using Wave.OpenXR;

namespace Wave.XR.Sample.EyeExp
{
	[RequireComponent(typeof(Text))]
	public class EyeExpText : MonoBehaviour
	{
		[SerializeField]
		private InputDeviceEye.Expressions m_EyeExp = InputDeviceEye.Expressions.LEFT_BLINK;
		public InputDeviceEye.Expressions EyeExp { get { return m_EyeExp; } set { m_EyeExp = value; } }

		Text m_Text = null;
		void Start()
		{
			m_Text = GetComponent<Text>();
		}

		void Update()
		{
			if (m_Text == null) { return; }

			float value = InputDeviceEye.GetEyeExpressionValue(m_EyeExp);
			m_Text.text = m_EyeExp.ToString() + ": " + value.ToString("N5");
		}
	}
}
