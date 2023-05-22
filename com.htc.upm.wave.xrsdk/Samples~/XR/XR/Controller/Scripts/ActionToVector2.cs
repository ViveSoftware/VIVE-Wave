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
using Wave.OpenXR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToVector2 : ActionToControl
    {
		const string LOG_TAG = "Wave.XR.Sample.Controller.ActionToVector2";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

		[SerializeField] public RectTransform _handle = null;

		public bool isLeft = false;
		public Utils.Vector2Buttons axisButton = Utils.Vector2Buttons.primary2DAxis;

#if ENABLE_INPUT_SYSTEM
		protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateHandle(ctx);

        protected override void OnActionStarted(InputAction.CallbackContext ctx) => UpdateHandle(ctx);

        protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateHandle(ctx);

        private void UpdateHandle(InputAction.CallbackContext ctx)
        {
            _handle.anchorMin = _handle.anchorMax = (ctx.ReadValue<Vector2>() + Vector2.one) * 0.5f;
        }
#endif

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM
			if (!UseInputAction)
#endif
			{
				if (_text != null) { _text.text = axisButton.ToString(); }
				if (InputDeviceControl.KeyAxis2D(
					(isLeft ? InputDeviceControl.ControlDevice.Left : InputDeviceControl.ControlDevice.Right),
					axisButton.InputFeature(),
					out Vector2 axis2d
					)
				)
				{
					_handle.anchorMin = _handle.anchorMax = (axis2d + Vector2.one) * 0.5f;
				}
			}
		}
	}
}
