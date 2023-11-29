// "Wave SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;

namespace Wave.OpenXR
{
	public class TrackerPose : MonoBehaviour
	{
		private InputDeviceTracker.TrackerId m_Tracker = default;
		private bool isInit = false;

		// Update is called once per frame
		void Update()
		{
			if (isInit && InputDeviceTracker.IsTracked(m_Tracker))
			{
				InputDeviceTracker.GetPosition(m_Tracker, out Vector3 position);
				InputDeviceTracker.GetRotation(m_Tracker, out Quaternion rotation);
				transform.position = position;
				transform.rotation = rotation;
			}
		}

		public void SetTrackerId(InputDeviceTracker.TrackerId tracker)
		{
			m_Tracker = tracker;
			isInit = true;
		}
	}
}
