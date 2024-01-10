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

namespace Wave.Essence.Samples.PassThrough
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class PoseModeSetting : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Samples.PassThrough.PoseModeSetting";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (m_Text == null) { return; }

			m_Text.text = "Pose Mode";
			if (WXRDevice.GetControllerPoseMode(XR_Hand.Right, out XR_ControllerPoseMode mode))
				m_Text.text = mode + " Mode";
		}

		public void RawMode()
		{
			bool ret = WXRDevice.SetControllerPoseMode(XR_Hand.Right, XR_ControllerPoseMode.Raw);
			DEBUG("RawMode() Set Right controller to Raw mode " + (ret ? "successfully." : "failed"));
			ret = WXRDevice.SetControllerPoseMode(XR_Hand.Left, XR_ControllerPoseMode.Raw);
			DEBUG("RawMode() Set Left controller to Raw mode " + (ret ? "successfully." : "failed"));
		}
		public void TriggerMode()
		{
			bool ret = WXRDevice.SetControllerPoseMode(XR_Hand.Right, XR_ControllerPoseMode.Trigger);
			DEBUG("TriggerMode() Set Right controller to Trigger mode " + (ret ? "successfully." : "failed"));
			ret = WXRDevice.SetControllerPoseMode(XR_Hand.Left, XR_ControllerPoseMode.Trigger);
			DEBUG("TriggerMode() Set Left controller to Trigger mode " + (ret ? "successfully." : "failed"));
		}
		public void PanelMode()
		{
			bool ret = WXRDevice.SetControllerPoseMode(XR_Hand.Right, XR_ControllerPoseMode.Panel);
			DEBUG("PanelMode() Set Right controller to Panel mode " + (ret ? "successfully." : "failed"));
			ret = WXRDevice.SetControllerPoseMode(XR_Hand.Left, XR_ControllerPoseMode.Panel);
			DEBUG("PanelMode() Set Left controller to Panel mode " + (ret ? "successfully." : "failed"));
		}
		public void HandleMode()
		{
			bool ret = WXRDevice.SetControllerPoseMode(XR_Hand.Right, XR_ControllerPoseMode.Handle);
			DEBUG("HandleMode() Set Right controller to Handle mode " + (ret ? "successfully." : "failed"));
			ret = WXRDevice.SetControllerPoseMode(XR_Hand.Left, XR_ControllerPoseMode.Handle);
			DEBUG("HandleMode() Set Left controller to Handle mode " + (ret ? "successfully." : "failed"));
		}
	}
}
