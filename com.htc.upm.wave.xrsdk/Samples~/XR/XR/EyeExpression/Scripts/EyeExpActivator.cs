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
using Wave.XR.Settings;
using Wave.OpenXR;
using System.Collections;

namespace Wave.XR.Sample.EyeExp
{
	[RequireComponent(typeof(Button))]
	public class EyeExpActivator : MonoBehaviour
	{
		const string LOG_TAG = "Wave.XR.Sample.EyeExp.EyeExpActivator";

		Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponentInChildren<Text>();
		}

		private void Update()
		{
			if (m_Text == null) { return; }

			m_Text.text = InputDeviceEye.IsEyeExpressionAvailable() ? "Disable Eye Expression" : "Enable Eye Expression";
		}

		public void ActivateEyeExpression()
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null)
			{
				bool active = !settings.EnableEyeExpression;
				Debug.Log(LOG_TAG + " ActivateEyeExpression() " + active);
				InputDeviceEye.ActivateEyeExpression(active);
			}
		}
	}
}
