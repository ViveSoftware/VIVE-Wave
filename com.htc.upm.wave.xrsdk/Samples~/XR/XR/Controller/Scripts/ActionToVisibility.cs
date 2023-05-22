// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using UnityEngine;
using Wave.OpenXR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToVisibility : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = true;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		[SerializeField] private InputActionReference _actionReference = null;
#endif

		[SerializeField]
		private Utils.DeviceTypes m_ObjectType = Utils.DeviceTypes.HMD;
		public Utils.DeviceTypes ObjectType { get { return m_ObjectType; } set { m_ObjectType = value; } }

		[SerializeField] private GameObject _target = null;

        private void OnEnable()
        {
            if (null == _target)
                _target = gameObject;

            _target.SetActive(false);

#if ENABLE_INPUT_SYSTEM
			if (m_UseInputAction)
			{
				if (_actionReference != null && _actionReference.action != null)
					StartCoroutine(UpdateVisibility());
			}
			else
#endif
			{
				StartCoroutine(UpdateVisibility());
			}
		}

		private IEnumerator UpdateVisibility ()
        {
            while (isActiveAndEnabled)
            {
#if ENABLE_INPUT_SYSTEM
				if (m_UseInputAction)
				{
					if (_actionReference.action != null &&
						_actionReference.action.controls.Count > 0 &&
						_actionReference.action.controls[0].device != null)
					{
						_target.SetActive(true);
						break;
					}
				}
				else
#endif
				{
					bool isConnected = false;
					switch (m_ObjectType)
					{
						case Utils.DeviceTypes.ControllerLeft:
							isConnected = InputDeviceControl.IsConnected(InputDeviceControl.ControlDevice.Left);
							break;
						case Utils.DeviceTypes.ControllerRight:
							isConnected = InputDeviceControl.IsConnected(InputDeviceControl.ControlDevice.Right);
							break;
						#region Tracker
						case Utils.DeviceTypes.Tracker0:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker0);
							break;
						case Utils.DeviceTypes.Tracker1:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker1);
							break;
						case Utils.DeviceTypes.Tracker2:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker2);
							break;
						case Utils.DeviceTypes.Tracker3:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker3);
							break;
						case Utils.DeviceTypes.Tracker4:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker4);
							break;
						case Utils.DeviceTypes.Tracker5:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker5);
							break;
						case Utils.DeviceTypes.Tracker6:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker6);
							break;
						case Utils.DeviceTypes.Tracker7:
							isConnected = InputDeviceTracker.IsAvailable(InputDeviceTracker.TrackerId.Tracker7);
							break;
						#endregion
						case Utils.DeviceTypes.Eye:
							isConnected = InputDeviceEye.IsEyeTrackingAvailable();
							break;
						default:
							isConnected = InputDeviceControl.IsConnected(InputDeviceControl.ControlDevice.Head);
							break;
					}
					if (isConnected)
					{
						_target.SetActive(true);
						break;
					}
				}

				yield return new WaitForSeconds(1.0f);
            }
        }
	}
}
