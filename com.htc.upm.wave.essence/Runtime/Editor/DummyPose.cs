// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using UnityEngine;
using Wave.Native;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	public static class DummyPose
	{
		public static bool ToUpdatePose = false;
		public static WVR_PoseOriginModel Origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
		private static uint WVR_DEVICE_COUNT_LEVEL_1 = 3;
		private static WVR_DevicePosePair_t[] wvrPoses = new WVR_DevicePosePair_t[WaveEssence.kDeviceCount];
		private static RigidTransform[] rtPoses = new RigidTransform[WaveEssence.kDeviceCount];

		private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		public static IEnumerator UpdatePoses()
		{
			while (ToUpdatePose)
			{
				yield return waitForEndOfFrame;

				Interop.WVR_GetSyncPose(Origin, wvrPoses, WVR_DEVICE_COUNT_LEVEL_1);
				for (uint wvr_index = 0; wvr_index < WVR_DEVICE_COUNT_LEVEL_1; wvr_index++)
				{
					WVR_DevicePosePair_t wvr_pose = wvrPoses[wvr_index];
					for (uint rt_index = 0; rt_index < WaveEssence.kDeviceCount; rt_index++)
					{
						if ((WVR_DeviceType)rt_index == wvr_pose.type)
						{
							rtPoses[rt_index].update(wvr_pose.pose.PoseMatrix);
							break;
						}
					}
				}
			}
		}

		public static Vector3 GetPosition(WVR_DeviceType device)
		{
			return rtPoses[(uint)device].pos;
		}

		public static Quaternion GetRotation(WVR_DeviceType device)
		{
			return rtPoses[(uint)device].rot;
		}
	}
}
#endif