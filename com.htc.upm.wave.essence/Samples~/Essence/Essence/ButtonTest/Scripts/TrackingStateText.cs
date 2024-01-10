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
using Wave.Essence.Events;

namespace Wave.Essence.Samples.ButtonTest
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class TrackingStateText : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Samples.ButtonTest.TrackingStateText";
		void DEBUG(string msg) { Log.d(LOG_TAG, m_TrackingDevice + " " + msg, true); }

		public enum DeviceTypes
		{
			HMD = XR_Device.Head,
			ControllerLeft = XR_Device.Left,
			ControllerRight = XR_Device.Right,
		}

		[SerializeField]
		private DeviceTypes m_TrackingDevice = DeviceTypes.HMD;
		public DeviceTypes TrackingDevice { get { return m_TrackingDevice; } set { m_TrackingDevice = value; } }

		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}

		private void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_DeviceErrorStatusUpdate, OnDeviceErrorStatusUpdate);
		}
		private void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_DeviceErrorStatusUpdate, OnDeviceErrorStatusUpdate);
		}

		private void Update()
		{
			if (m_Text == null) { return; }

			bool isTracked = WXRDevice.IsTracked((XR_Device)m_TrackingDevice);

			m_Text.text = "Tracking: ";
			if (isTracked)
			{
				if (!m_LostTracking) { m_Text.text += "Position "; }
				m_Text.text += "Rotation";
			}
			else
			{
				m_Text.text += "Lost";
			}
		}

		private bool m_LostTracking = false;
		private void OnDeviceErrorStatusUpdate(WVR_Event_t systemEvent)
		{
			WVR_DeviceType device = systemEvent.device.type;
			if (device != (WVR_DeviceType)m_TrackingDevice) { return; }

			DEBUG("OnDeviceErrorStatusUpdate() m_LostTracking before: " + m_LostTracking);
			if (!m_LostTracking)
			{
				m_LostTracking = Interop.WVR_GetDeviceErrorState((WVR_DeviceType)m_TrackingDevice, WVR_DeviceErrorStatus.WVR_DeviceErrorStatus_DeviceLostTracking);
			}
			else
			{
				m_LostTracking = !Interop.WVR_GetDeviceErrorState((WVR_DeviceType)m_TrackingDevice, WVR_DeviceErrorStatus.WVR_DeviceErrorStatus_DeviceLostTrackingRestore);
			}
			DEBUG("OnDeviceErrorStatusUpdate() m_LostTracking after: " + m_LostTracking);
		}
	}
}
