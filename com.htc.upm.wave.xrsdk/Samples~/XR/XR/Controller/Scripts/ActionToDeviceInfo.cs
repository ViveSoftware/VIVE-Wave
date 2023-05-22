// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Wave.OpenXR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToDeviceInfo : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = true;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		[SerializeField] private InputActionReference _actionReference = null;
#endif
		public InputDeviceControl.ControlDevice Controller = InputDeviceControl.ControlDevice.Left;

		[SerializeField] private Text _text = null;

        private void OnEnable()
        {
            UpdateText();
        }

        private void UpdateText()
        {
#if ENABLE_INPUT_SYSTEM
			if (m_UseInputAction)
			{
				if (null == _actionReference || null == _actionReference.action || _actionReference.action.controls.Count == 0 || _text == null)
					return;

				var device = _actionReference.action.controls[0].device;
				_text.text = $"{device.name}\n{device.deviceId}\n{string.Join(",", device.usages.Select(u => u.ToString()))}";
			}
			else
#endif
			{
				_text.text = Controller.ToString();
				if (InputDeviceControl.Name(Controller, out string name))
					_text.text += "\n"+name;
			}
		}
	}
}
