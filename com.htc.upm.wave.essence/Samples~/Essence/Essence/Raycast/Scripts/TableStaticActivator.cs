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
using Wave.Essence.Raycast;
using System.Runtime.InteropServices;
using System;

namespace Wave.Essence.Samples.Raycast
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public class TableStaticActivator : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Samples.Raycast.TableStaticActivator";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		public ControllerRaycastPointer CRPLeft = null;
		public ControllerRaycastPointer CRPRight = null;

		private Text m_Text = null;
		private bool m_Hide = false;

		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			if (m_Text == null) { return; }

			m_Text.text = (m_Hide ? "Hide Static" : "Show Static");
		}
		private void Start()
		{
			HideIdleController();

			IntPtr ptrParameterName = Marshal.StringToHGlobalAnsi("ResetEyeCamera");
			Interop.WVR_SetParameters(WVR_DeviceType.WVR_DeviceType_HMD, ptrParameterName);
			Marshal.FreeHGlobal(ptrParameterName);
			DEBUG("Start() ResetEyeCamera");
		}

		public void HideIdleController()
		{
			m_Hide = !m_Hide;
			Log.d("Wave.Essence.Samples.Raycast.TableStaticActivator", "HideIdleController() " + m_Hide, true);
			
			if (CRPLeft != null) { CRPLeft.HideWhenIdle = m_Hide; }
			if (CRPRight != null) { CRPRight.HideWhenIdle = m_Hide; }
		}
	}
}
