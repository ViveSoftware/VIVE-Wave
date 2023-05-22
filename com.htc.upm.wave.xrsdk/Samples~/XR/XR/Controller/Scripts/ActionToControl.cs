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
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class ActionToControl : MonoBehaviour
    {
        [Tooltip("Optional text element that will be set to the name of the action")]
        [SerializeField] public Text _text = null;

#if ENABLE_INPUT_SYSTEM
		[SerializeField]
		private bool m_UseInputAction = true;
		public bool UseInputAction { get { return m_UseInputAction; } set { m_UseInputAction = value; } }

		[Tooltip("Action Reference that represents the control")]
		[SerializeField] private InputActionReference _actionReference = null;

		protected virtual void OnEnable()
        {
            if (_actionReference == null || _actionReference.action == null)
                return;

            _actionReference.action.started += OnActionStarted;
            _actionReference.action.performed += OnActionPerformed;
            _actionReference.action.canceled += OnActionCanceled;

            StartCoroutine(UpdateBinding());
        }

        protected virtual void OnDisable()
        {
            if (_actionReference == null || _actionReference.action == null)
                return;

            _actionReference.action.started -= OnActionStarted;
            _actionReference.action.performed -= OnActionPerformed;
            _actionReference.action.canceled -= OnActionCanceled;
        }

        private IEnumerator UpdateBinding ()
        {
            if(null != _text)
                _text.text = _actionReference.action.name;

            while (isActiveAndEnabled)
            {
                if(_actionReference.action != null &&
                    _actionReference.action.controls.Count > 0 &&
                    _actionReference.action.controls[0].device != null)
                {
                    OnActionBound();
                    break;
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        protected virtual void OnActionStarted(InputAction.CallbackContext ctx) { }

        protected virtual void OnActionPerformed(InputAction.CallbackContext ctx) { }

        protected virtual void OnActionCanceled(InputAction.CallbackContext ctx) { }
#endif

		protected virtual void OnActionBound()
        {
        }
	}
}
