// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SpatialTracking;
using Wave.XR.Sample;

public class ChooseDoF : MonoBehaviour {
	public enum TrackingSpace {
		TS_NO_SPECIFY = -1,
		TS_3DOF,
		TS_6DOF_Ground,
		TS_6DOF_Head
	};

	public static TrackingSpace whichHead = TrackingSpace.TS_NO_SPECIFY;
	public TrackingSpace WhichHead = TrackingSpace.TS_3DOF;

	void OnEnable() {
		TrackingOriginModeFlags mode = TrackingOriginModeFlags.Device;
		TrackedPoseDriver.TrackingType trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
		if (whichHead == TrackingSpace.TS_NO_SPECIFY)
			whichHead = WhichHead;
		switch (whichHead)
		{
			case TrackingSpace.TS_3DOF:
				trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
				mode = TrackingOriginModeFlags.Device;
				break;
			case TrackingSpace.TS_6DOF_Ground:
				mode = TrackingOriginModeFlags.Floor;
				break;
			case TrackingSpace.TS_6DOF_Head:
				mode = TrackingOriginModeFlags.Device;
				break;
			case TrackingSpace.TS_NO_SPECIFY:
				Application.Quit();
				break;
		}

		Utils.InputSubsystem.TrySetTrackingOriginMode(mode);
		Camera.main.GetComponent<TrackedPoseDriver>().trackingType = trackingType;
	}
}
