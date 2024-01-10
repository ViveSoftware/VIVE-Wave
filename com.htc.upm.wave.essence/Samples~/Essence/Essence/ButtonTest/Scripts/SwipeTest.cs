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
using Wave.Essence.Events;

namespace Wave.Essence.Samples.ButtonTest
{
	public class SwipeTest : MonoBehaviour
	{
		private const string LOG_TAG = "SwipeTest";
		private void DEBUG(string msg) { Log.d(LOG_TAG, m_DeviceType + " " + msg, true); }

		[SerializeField]
		private XR_Device m_DeviceType = XR_Device.Dominant;
		public XR_Device DeviceType { get { return m_DeviceType; } set { m_DeviceType = value; } }

		void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DownToUpSwipe, OnSwipe);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_LeftToRightSwipe, OnSwipe);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_RightToLeftSwipe, OnSwipe);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_UpToDownSwipe, OnSwipe);
		}

		void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DownToUpSwipe, OnSwipe);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_LeftToRightSwipe, OnSwipe);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_RightToLeftSwipe, OnSwipe);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_UpToDownSwipe, OnSwipe);
		}

		private void OnSwipe(WVR_Event_t systemEvent)
		{
			DEBUG("OnSwipe() Event: " + systemEvent.common.type + ", device: " + systemEvent.device.type);

			if (systemEvent.device.type != m_DeviceType.GetRoleType())
				return;

			switch (systemEvent.common.type)
			{
				case WVR_EventType.WVR_EventType_LeftToRightSwipe:
					transform.Rotate(0, -180 * (10 * Time.deltaTime), 0);
					break;
				case WVR_EventType.WVR_EventType_RightToLeftSwipe:
					transform.Rotate(0, 180 * (10 * Time.deltaTime), 0);
					break;
				case WVR_EventType.WVR_EventType_DownToUpSwipe:
					transform.Rotate(180 * (10 * Time.deltaTime), 0, 0);
					break;
				case WVR_EventType.WVR_EventType_UpToDownSwipe:
					transform.Rotate(-180 * (10 * Time.deltaTime), 0, 0);
					break;
				default:
					break;
			}
		}
	}
}
