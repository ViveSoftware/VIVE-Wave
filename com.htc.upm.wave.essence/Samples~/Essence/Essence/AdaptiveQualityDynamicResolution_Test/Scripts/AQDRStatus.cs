// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wave.Native;
using Wave.Essence;
using Wave.Essence.Render;

namespace AQDR
{
	[RequireComponent(typeof(Text))]
	public class AQDRStatus : MonoBehaviour
	{
		Text text;

		public AdaptiveQuality AQ = null;
		public DynamicResolution DR = null;

		bool AQEnabled = false;
		DynamicResolution.AQEvent AQEvent = DynamicResolution.AQEvent.None;
		float DRScale = 0;

		// Use this for initialization
		void Start()
		{
			if (text == null)
				text = GetComponent<Text>();

			StartCoroutine(Check());
		}

		IEnumerator Check()
		{
			bool needUpdate = true;
			while (enabled)
			{
				bool aqEnabled = Interop.WVR_IsAdaptiveQualityEnabled();
				if (aqEnabled != AQEnabled)
					needUpdate = true;
				if (DR.CurrentScale != DRScale)
					needUpdate = true;

				if (needUpdate)
				{
					DRScale = DR.CurrentScale;
					AQEnabled = aqEnabled;
					AQEvent = DR.CurrentAQEvent;

					var sb = Log.CSB;
					sb.Append("Quality:   ").Append(QualitySettings.names[QualitySettings.GetQualityLevel()]).AppendLine();
					sb.Append("AQ Enabled:").Append(AQEnabled).AppendLine();
					sb.Append("AQ Event:  ").Append(AQEvent.ToString()).AppendLine();
					sb.Append("Res Scale: ").Append(DRScale).AppendLine();
					text.text = sb.ToString();
				}
				yield return new WaitForSeconds(1);
			}
		}
	}
}
