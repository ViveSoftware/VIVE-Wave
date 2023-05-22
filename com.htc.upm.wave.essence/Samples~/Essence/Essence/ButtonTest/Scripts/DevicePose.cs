// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using Wave.Native;
#if UNITY_EDITOR
using Wave.Essence.Editor;
#endif

namespace Wave.Essence.Samples.ButtonTest
{
	public class DevicePose : MonoBehaviour
	{
		public XR_Device Pose = XR_Device.Head;
		private Vector3 m_Position = Vector3.zero;
		private Quaternion m_Rotation = Quaternion.identity;

		// Update is called once per frame
		void Update()
		{
			if (WXRDevice.GetPosition(Pose, ref m_Position))
				transform.localPosition = m_Position;
			if (WXRDevice.GetRotation(Pose, ref m_Rotation))
				transform.localRotation = m_Rotation;
		}
	}
}
