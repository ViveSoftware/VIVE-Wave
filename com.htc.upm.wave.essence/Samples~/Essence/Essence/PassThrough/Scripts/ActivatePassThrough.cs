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
using Wave.Native;
using Wave.Essence.Hand;
using Wave.Essence.Events;
using UnityEngine.UI;

namespace Wave.Essence.Samples.PassThrough
{
	public class ActivatePassThrough : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Samples.PassThrough.ActivatePassThrough";
		public Text statusIQ;
		public Text statusIF;
		public Text statusIR;

		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		bool showUnderlay = false;

		public void ActivateUnderlay()
		{
			showUnderlay = !showUnderlay;
			var result = Interop.WVR_ShowPassthroughUnderlay(showUnderlay);
			DEBUG("ActivateUnderlay() " + showUnderlay + ", result: " + result);
		}
		bool showOverlay = false;
		public void ActivateOverlay()
		{
			showOverlay = !showOverlay;
			var result = Interop.WVR_ShowPassthroughOverlay(showOverlay);
			DEBUG("ActivateUnderlay() " + showOverlay + ", result: " + result);
		}

		public void OnButtonClickHigh()
		{
			bool ret = Interop.WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality.QualityMode);
			statusIQ.text = "Set Q. ret=" + ret;
		}
		public void OnButtonClickMedium()
		{
			bool ret = Interop.WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality.DefaultMode);
			statusIQ.text = "Set D. ret=" + ret;
		}
		public void OnButtonClickLow()
		{
			bool ret = Interop.WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality.PerformanceMode);
			statusIQ.text = "Set P. ret=" + ret;
		}

		public void OnButtonClickEnvironmentView()
		{
			bool ret = Interop.WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus.View);
			statusIF.text = "Set V. ret=" + ret;
		}
		public void OnButtonClickHandInteraction()
		{
			bool ret = Interop.WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus.Scale);
			statusIF.text = "Set S. ret=" + ret;
		}

		public void OnButtonClickRateBoost()
		{
			WVR_Result ret = Interop.WVR_SetPassthroughImageRate(WVR_PassthroughImageRate.Boost);
			if (ret == WVR_Result.WVR_Success)
				statusIR.text = "Set B. ret=0";
			else
				statusIR.text = "Set B. ret=" + ret;
		}
		public void OnButtonClickRateNormal()
		{
			WVR_Result ret = Interop.WVR_SetPassthroughImageRate(WVR_PassthroughImageRate.Normal);
			if (ret == WVR_Result.WVR_Success)
				statusIR.text = "Set N. ret=0";
			else
				statusIR.text = "Set N. ret=" + ret;
		}

		private void Update()
		{
			if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Menu))
				ActivateOverlay();
		}

		private bool mEnabled = false;
		void OnEnable()
		{
			if (!mEnabled)
			{
				GeneralEvent.Listen(HandManager.HAND_STATIC_GESTURE, OnStaticGesture);
				mEnabled = true;
			}
		}
		void OnDisable()
		{
			if (mEnabled)
			{
				GeneralEvent.Remove(HandManager.HAND_STATIC_GESTURE, OnStaticGesture);
				mEnabled = false;
			}
		}

		private void OnStaticGesture(params object[] args)
		{
			var hand = (HandManager.HandType)args[0];
			var gesture = (HandManager.GestureType)args[1];
			DEBUG("OnStaticGesture() " + hand + ", " + gesture);

			if (hand == HandManager.HandType.Left && gesture == HandManager.GestureType.Palm_Pinch)
				ActivateOverlay();
		}
	}
}
