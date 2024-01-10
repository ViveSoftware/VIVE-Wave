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
	public class EyeDataText : MonoBehaviour
	{
		public bool IsLeft = false;

		private Text m_Text = null;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		void Update()
		{
			if (m_Text == null || EyeManager.Instance == null) { return; }

			m_Text.text = (IsLeft ? "Left eye: " : "Right eye: ");

			if (IsLeft)
			{
				Vector3 origin = Vector3.zero;
				{
					if (EyeManager.Instance.GetLeftEyeOrigin(out Vector3 value))
						origin = value;
					m_Text.text += "origin: (" + origin.x.ToString() + ", " + origin.y.ToString() + ", " + origin.z.ToString() + ")\n";
				}
				float pupilDiameter = 0;
				{
					if (EyeManager.Instance.GetLeftEyePupilDiameter(out float value))
						pupilDiameter = value;
					m_Text.text += "pupil diameter: " + pupilDiameter + "\n";
				}
				Vector2 pupilSensorPosition = Vector2.zero;
				{
					if (EyeManager.Instance.GetLeftEyePupilPositionInSensorArea(out Vector2 value))
						pupilSensorPosition = value;
					m_Text.text += "sensor position (" + pupilSensorPosition.x.ToString() + ", " + pupilSensorPosition.y.ToString() + ")\n";
				}
				float openness = 0;
				{
					if (EyeManager.Instance.GetLeftEyeOpenness(out float value))
						openness = value;
					m_Text.text += "openness: " + openness;
				}
			}
			else
			{
				Vector3 origin = Vector3.zero;
				{
					if (EyeManager.Instance.GetRightEyeOrigin(out Vector3 value))
						origin = value;
					m_Text.text += "origin: (" + origin.x.ToString() + ", " + origin.y.ToString() + ", " + origin.z.ToString() + ")\n";
				}
				float pupilDiameter = 0;
				{
					if (EyeManager.Instance.GetRightEyePupilDiameter(out float value))
						pupilDiameter = value;
					m_Text.text += "pupil diameter: " + pupilDiameter + "\n";
				}
				Vector2 pupilSensorPosition = Vector2.zero;
				{
					if (EyeManager.Instance.GetRightEyePupilPositionInSensorArea(out Vector2 value))
						pupilSensorPosition = value;
					m_Text.text += "sensor position (" + pupilSensorPosition.x.ToString() + ", " + pupilSensorPosition.y.ToString() + ")\n";
				}
				float openness = 0;
				{
					if (EyeManager.Instance.GetRightEyeOpenness(out float value))
						openness = value;
					m_Text.text += "openness: " + openness;
				}
			}
		}
	}
}
