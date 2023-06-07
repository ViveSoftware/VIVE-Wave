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
using Wave.Essence;
using Wave.XR.Sample;
using System.Collections;
using UnityEngine.SpatialTracking;

namespace Wave.Essence.Samples.PassThrough
{
	public class ChangeOriginalUI : MonoBehaviour
	{
		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		void UpdateText(WVR_PoseOriginModel model)
		{
			if (m_Text == null) return;
			switch (model)
			{
				case WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead:
					m_Text.text = "Origin on Head";
					break;
				case WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround:
					m_Text.text = "Origin on Ground";
					break;
				case WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF:
					m_Text.text = "Origin on Head 3DoF";
					break;
				default:
					m_Text.text = "Origin";
					break;
			}
		}

		private IEnumerator Start()
		{
			while (Utils.InputSubsystem == null)
				yield return null;
			var mode = Utils.InputSubsystem.GetTrackingOriginMode();
			UpdateText(mode == UnityEngine.XR.TrackingOriginModeFlags.Device ? 
				WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead : 
				WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround);
		}

		public void OnHeadClicked()
		{
			if (Utils.InputSubsystem.TrySetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Device))
			{
				Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
				UpdateText(WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead);
			}
		}

		public void OnHead3DofClicked()
		{
			if (Utils.InputSubsystem.TrySetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Device))
			{
				Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
				UpdateText(WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF);
			}
		}

		public void OnGroundClicked()
		{
			if (Utils.InputSubsystem.TrySetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Floor))
			{
				Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
				UpdateText(WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround);
			}
		}
	}
}
