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
using Wave.Essence.Events;
using Wave.Native;

public class CtrlrSwipeUD : MonoBehaviour
{
	void OnEvent(WVR_Event_t wvrEvent)
	{
		var _event = wvrEvent.common.type;
		Log.d("CtrlrSwipeUD", "OnEvent() _event = " + _event);

		switch (_event)
		{
			case WVR_EventType.WVR_EventType_DownToUpSwipe:
				transform.Rotate(30, 0, 0);
				break;
			case WVR_EventType.WVR_EventType_UpToDownSwipe:
				transform.Rotate(-30, 0, 0);
				break;
		}
	}

	void OnEnable()
	{
		SystemEvent.Listen(WVR_EventType.WVR_EventType_DownToUpSwipe, OnEvent);
		SystemEvent.Listen(WVR_EventType.WVR_EventType_UpToDownSwipe, OnEvent);
	}

	void OnDisable()
	{
		SystemEvent.Remove(WVR_EventType.WVR_EventType_DownToUpSwipe, OnEvent);
		SystemEvent.Remove(WVR_EventType.WVR_EventType_DownToUpSwipe, OnEvent);
	}
}

