// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using Wave.OpenXR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToButton : ActionToControl
    {
		const string LOG_TAG = "Wave.XR.Sample.Controller.ActionToButton";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

		[SerializeField] private Image _image = null;

        [SerializeField] private Color _normalColor = Color.red;

        [SerializeField] private Color _pressedColor = Color.green;

		public bool isLeft = false;
		public Utils.BinaryButtons binaryButton = Utils.BinaryButtons.triggerButton;

		private void Awake()
        {
            if (_image != null)
            {
                _image.enabled = false;
                _image.color = _normalColor;
            }

#if ENABLE_INPUT_SYSTEM
			if (!UseInputAction)
#endif
			{
				OnActionBound();
			}
		}

#if ENABLE_INPUT_SYSTEM
		protected override void OnActionStarted(InputAction.CallbackContext ctx)
        {
            if (_image != null)
                _image.color = _pressedColor;
        }

        protected override void OnActionCanceled(InputAction.CallbackContext ctx)
        {
            if (_image != null)
                _image.color = _normalColor;
        }
#endif

		protected override void OnActionBound()
        {
            if(_image != null)
                _image.enabled = true;
        }

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM
			if (!UseInputAction)
#endif
			{
				if (_text != null) { _text.text = binaryButton.Name(); }
				if (InputDeviceControl.KeyDown(
					(isLeft ? InputDeviceControl.ControlDevice.Left : InputDeviceControl.ControlDevice.Right),
					binaryButton.InputFeature())
				)
				{
					if (_image != null)
						_image.color = _pressedColor;
				}
				else
				{
					if (_image != null)
						_image.color = _normalColor;
				}
			}
		}
	}
}
