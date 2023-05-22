// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine.UI;
using UnityEngine;
using Wave.OpenXR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToAxis : ActionToControl
    {
		const string LOG_TAG = "Wave.XR.Sample.Controller.ActionToAxis";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

		[Tooltip("Slider controlled by the action value")]
        [SerializeField] private Slider _slider = null;

		public bool isLeft = false;
		public Utils.FloatButtons floatButton = Utils.FloatButtons.trigger;

#if ENABLE_INPUT_SYSTEM
		protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateValue(ctx);
        protected override void OnActionStarted(InputAction.CallbackContext ctx) => UpdateValue(ctx);
        protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateValue(ctx);

        private void UpdateValue(InputAction.CallbackContext ctx) => _slider.value = ctx.ReadValue<float>();
#endif

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM
			if (!UseInputAction)
#endif
			{
				if (_text != null) { _text.text = floatButton.ToString(); }
				if (InputDeviceControl.KeyAxis1D(
					(isLeft ? InputDeviceControl.ControlDevice.Left : InputDeviceControl.ControlDevice.Right),
					floatButton.InputFeature(),
					out float axis1d
					)
				)
				{
					_slider.value = axis1d;
				}
			}
		}
	}
}
