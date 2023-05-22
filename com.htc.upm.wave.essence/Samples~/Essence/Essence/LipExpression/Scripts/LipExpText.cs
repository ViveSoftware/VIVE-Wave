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
	[RequireComponent(typeof(Text))]
	public class LipExpText : MonoBehaviour
	{
		[SerializeField]
		private LipExp m_LipExp = LipExp.Max;
		public LipExp LipExp { get { return m_LipExp; } set { m_LipExp = value; } }

		Text m_Text = null;
		void Start()
		{
			m_Text = GetComponent<Text>();
		}

		void Update()
		{
			if (m_Text == null || LipExpManager.Instance == null) { return; }

			float value = LipExpManager.Instance.GetLipExpression(m_LipExp);
			m_Text.text = m_LipExp.ToString() + ": " + value.ToString("N5");
		}
	}
}
