// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using UnityEngine;
using Wave.OpenXR;
using System.Text;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
	public class HideUntrackedObject : MonoBehaviour
	{
		const string LOG_TAG = "Wave.XR.Sample.Controller.HideUntrackedObject ";
		StringBuilder m_sb = null;
		StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg)
		{
			msg.Insert(0, LOG_TAG);
			Debug.Log(msg);
		}

#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = true;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		[SerializeField]
		private InputActionReference m_IsTracked;
		public InputActionReference IsTracked { get => m_IsTracked; set => m_IsTracked = value; }
#endif

		[SerializeField]
		private Utils.DeviceTypes m_ObjectType = Utils.DeviceTypes.HMD;
		public Utils.DeviceTypes ObjectType { get { return m_ObjectType; } set { m_ObjectType = value; } }

		[SerializeField]
		private GameObject m_ObjectToHide = null;
		public GameObject ObjectToHide { get => m_ObjectToHide; set => m_ObjectToHide = value; }

		int printFrame = 0;
		bool printIntervalLog = false;
		private void Update()
		{
			printFrame++;
			printFrame %= 300;
			printIntervalLog = (printFrame == 0);

			bool isTracked = false;
#if ENABLE_INPUT_SYSTEM
			if (m_UseInputAction)
			{
				if (Utils.GetButton(m_IsTracked, out bool value, out string msg))
				{
					isTracked = value;
					if (printIntervalLog)
					{
						sb.Clear().Append("Update() ").Append(m_ObjectType).Append(", name: ").Append(m_IsTracked.action.name).Append(", isTracked: ").Append(isTracked);
						DEBUG(sb);
					}
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append("Update() ").Append(msg); DEBUG(sb);
					}
				}
			}
			else
#endif
			{
				switch(m_ObjectType)
				{
					case Utils.DeviceTypes.ControllerLeft:
						isTracked = InputDeviceControl.IsTracked(InputDeviceControl.ControlDevice.Left);
						break;
					case Utils.DeviceTypes.ControllerRight:
						isTracked = InputDeviceControl.IsTracked(InputDeviceControl.ControlDevice.Right);
						break;
					#region Tracker
					case Utils.DeviceTypes.Tracker0:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker0);
						break;
					case Utils.DeviceTypes.Tracker1:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker1);
						break;
					case Utils.DeviceTypes.Tracker2:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker2);
						break;
					case Utils.DeviceTypes.Tracker3:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker3);
						break;
					case Utils.DeviceTypes.Tracker4:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker4);
						break;
					case Utils.DeviceTypes.Tracker5:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker5);
						break;
					case Utils.DeviceTypes.Tracker6:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker6);
						break;
					case Utils.DeviceTypes.Tracker7:
						isTracked = InputDeviceTracker.IsTracked(InputDeviceTracker.TrackerId.Tracker7);
						break;
					#endregion
					case Utils.DeviceTypes.Eye:
						isTracked = InputDeviceEye.IsEyeTrackingTracked();
						break;
					default:
						isTracked = InputDeviceControl.IsTracked(InputDeviceControl.ControlDevice.Head);
						break;
				}
			}
			m_ObjectToHide.SetActive(isTracked);
		}
	}
}
