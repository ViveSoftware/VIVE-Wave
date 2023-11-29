// "Wave SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.OpenXR
{
	public class TrackerDeviceManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject poseMarkerTemplate = null;

		private Dictionary<InputDeviceTracker.TrackerId, GameObject> trackerObjectList = new Dictionary<InputDeviceTracker.TrackerId, GameObject>();
		private bool enableTracker = true;

		private void Awake()
		{
			if (poseMarkerTemplate != null)
				poseMarkerTemplate.SetActive(false);
			InputDeviceTracker.ActivateTracker(true);
		}

		private void Update()
		{
			foreach (InputDeviceTracker.TrackerId id in Enum.GetValues(typeof(InputDeviceTracker.TrackerId)))
			{
				bool isTracked = InputDeviceTracker.IsTracked(id);
				bool isInit = trackerObjectList.ContainsKey(id);
				if (isInit)
				{
					trackerObjectList[id].SetActive(isTracked && enableTracker);
				}
				else if (!isInit && isTracked && poseMarkerTemplate != null)
				{
					GameObject poseMarker = Instantiate(poseMarkerTemplate, parent: transform);
					poseMarker.SetActive(true);
					TrackerPose trackerPose = poseMarker.GetComponentInChildren<TrackerPose>();
					trackerPose.SetTrackerId(id);
					trackerObjectList.Add(id, poseMarker);
				}
			}
		}

		public void EnableTracker(Text textUI)
		{
			enableTracker = !enableTracker;
			InputDeviceTracker.ActivateTracker(enableTracker);
			textUI.text = enableTracker ? "Disable Tracker" : "Enable Tracker";
		}
	}
}
