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
using UnityEngine.InputSystem.XR;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToHaptics : MonoBehaviour
    {
		const string LOG_TAG = "Wave.XR.Sample.Controller.ActionToHaptics";
		void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = true;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		public InputActionReference action;
        public InputActionReference hapticAction;
#endif
		public bool isLeft = false;
		public Utils.BinaryButtons hapticButton = Utils.BinaryButtons.triggerButton;

		public float _amplitude = 1.0f;
		public float _duration = 0.1f;
		public float _frequency = 0.0f;

#if ENABLE_INPUT_SYSTEM
		protected void OnActionPerformed(InputAction.CallbackContext ctx)
		{
			Debug.Log("ActionToHaptics() " + ctx.control.device.description.product);
			if (ctx.control.device.description.product.Equals("WVR_CR_Left_001"))
			{
				XRControllerWithRumble controllerLeft = InputSystem.GetDevice<XRControllerWithRumble>("LeftHand");
				if (controllerLeft != null)
				{
					Debug.Log("ActionToHaptics() has left controllerLeft.");
					controllerLeft.SendImpulse(0.9f, 0.5f);
				}
			}
			if (ctx.control.device.description.product.Equals("WVR_CR_Right_001"))
			{
				XRControllerWithRumble controllerRight = InputSystem.GetDevice<XRControllerWithRumble>("RightHand");
				if (controllerRight != null)
				{
					Debug.Log("ActionToHaptics() has right controllerLeft.");
					controllerRight.SendImpulse(_amplitude, _duration);
				}
			}
		}
#endif

		private void Start()
		{
#if ENABLE_INPUT_SYSTEM
			if (m_UseInputAction)
			{
				if (action != null && action.action != null) { action.action.performed += OnActionPerformed; }
				if (action == null || hapticAction == null)
					return;

				action.action.Enable();
				hapticAction.action.Enable();
				action.action.performed += (ctx) =>
				{
					var control = action.action.activeControl;
					if (null == control)
						return;
				};
			}
#endif
		}

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM
			if (!m_UseInputAction)
#endif
			{
				if (InputDeviceControl.KeyDown(
					(isLeft ? InputDeviceControl.ControlDevice.Left : InputDeviceControl.ControlDevice.Right),
					hapticButton.InputFeature()))
				{
					DEBUG("Update() " + (isLeft ? "Left " : "Right ") + hapticButton + " is pressed.");
					bool haptic = InputDeviceControl.SendHapticImpulse(
						(isLeft ? InputDeviceControl.ControlDevice.Left : InputDeviceControl.ControlDevice.Right),
						_amplitude,
						_duration);
					DEBUG("Update() " + (isLeft ? "Left " : "Right ") + hapticButton + " vibrates " + (haptic ? "successfully." : "failed."));
				}
			}
		}
	}
}
