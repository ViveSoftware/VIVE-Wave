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
using Wave.Native;

namespace Wave.Essence
{
	/**
	 * @brief Configures the pose mode of different devices.
	 **/
	[DisallowMultipleComponent]
	sealed public class PoseModeConfigurator : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.PoseModeConfigurator";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		#region Customized Settings
		private XR_ControllerPoseMode m_RightControllerModeEx = XR_ControllerPoseMode.Raw;
		[Tooltip("Sets up the pose mode of the dominant controller.")]
		[SerializeField]
		private XR_ControllerPoseMode m_RightControllerMode = XR_ControllerPoseMode.Raw;
		public XR_ControllerPoseMode RightControllerMode { get { return m_RightControllerMode; } set { m_RightControllerMode = value; } }

		private XR_ControllerPoseMode m_LeftControllerModeEx = XR_ControllerPoseMode.Raw;
		[Tooltip("Sets up the pose mode of the non-dominant controller.")]
		[SerializeField]
		private XR_ControllerPoseMode m_LeftControllerMode = XR_ControllerPoseMode.Raw;
		public XR_ControllerPoseMode LeftControllerMode { get { return m_LeftControllerMode; } set { m_LeftControllerMode = value; } }
		#endregion

		#region MonoBehaviour overrides
		private void Start()
		{
			m_RightControllerModeEx = m_RightControllerMode;
			if (WXRDevice.SetControllerPoseMode(XR_Hand.Right, m_RightControllerMode))
				DEBUG("Start() Sets up the right controller pose mode to " + m_RightControllerMode + " successfully.");
			else
				DEBUG("Start() Failed to set up the right controller pose mode to " + m_RightControllerMode);

			m_LeftControllerModeEx = m_LeftControllerMode;
			if (WXRDevice.SetControllerPoseMode(XR_Hand.Left, m_LeftControllerMode))
				DEBUG("Start() Sets up the left controller pose mode to " + m_LeftControllerMode + " successfully.");
			else
				DEBUG("Start() Failed to set up the left controller pose mode to " + m_LeftControllerMode);
		}

		private void Update()
		{
			if (m_RightControllerModeEx != m_RightControllerMode)
			{
				m_RightControllerModeEx = m_RightControllerMode;
				if (WXRDevice.SetControllerPoseMode(XR_Hand.Right, m_RightControllerMode))
					DEBUG("Update() Sets up the right controller pose mode to " + m_RightControllerMode + " successfully.");
				else
					DEBUG("Update() Failed to set up the right controller pose mode to " + m_RightControllerMode);
			}

			if (m_LeftControllerModeEx != m_LeftControllerMode)
			{
				m_LeftControllerModeEx = m_LeftControllerMode;
				if (WXRDevice.SetControllerPoseMode(XR_Hand.Left, m_LeftControllerMode))
					DEBUG("Start() Sets up the left controller pose mode to " + m_LeftControllerMode + " successfully.");
				else
					DEBUG("Start() Failed to set up the left controller pose mode to " + m_LeftControllerMode);
			}
		}
		#endregion
	}
}
