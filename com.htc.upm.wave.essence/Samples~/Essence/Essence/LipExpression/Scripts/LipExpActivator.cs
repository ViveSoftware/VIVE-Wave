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
using Wave.Native;
using Wave.Essence.LipExpression;

namespace Wave.Essence.Samples.LipExpression
{
	[RequireComponent(typeof(Button))]
	public class LipExpActivator : MonoBehaviour
	{
		private const string LOG_TAG = "Wave.Essence.Samples.LipExpression.LipExpActivator";
		private static void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		[SerializeField]
		private Text m_Text = null;

		private Button m_Button = null;
		void Start()
		{
			m_Button = GetComponent<Button>();
		}
		private void Update()
		{
			if (m_Button == null || m_Text == null || LipExpManager.Instance == null) { return; }

			var status = LipExpManager.Instance.GetLipExpStatus();

			if (status == LipExpManager.LipExpStatus.Available)
			{
				m_Text.text = "Stop Lip Expression";
				m_Button.interactable = true;
			} else if (
				status == LipExpManager.LipExpStatus.NotStart ||
				status == LipExpManager.LipExpStatus.StartFailure)
			{
				m_Text.text = "Start Lip Expression";
				m_Button.interactable = true;
			}
			else
			{
				m_Button.interactable = false;
			}
		}

		public void ActivateLipExp()
		{
			if (LipExpManager.Instance == null) { return; }

			bool available = LipExpManager.Instance.IsLipExpEnabled();
			if (available)
			{
				DEBUG("Stop lip expression.");
				LipExpManager.Instance.StopLipExp();
			}
			else
			{
				DEBUG("Start lip expression.");
				LipExpManager.Instance.StartLipExp();
			}
		}
	}
}
