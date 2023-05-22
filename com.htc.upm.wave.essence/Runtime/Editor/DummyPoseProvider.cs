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
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;
using Wave.Native;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	public class DummyPoseProvider : BasePoseProvider
	{
		public enum OriginType
		{
			Head = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead,
			Ground = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround,
			Head_Rotation_Only = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF
		}
		public TrackedPoseDriver.TrackedPose poseSource = TrackedPoseDriver.TrackedPose.Center;
		public OriginType m_Origin = OriginType.Head;

		private void OnEnable()
		{
			if (!DummyPose.ToUpdatePose)
			{
				Log.i("DummyPoseProvider", "OnEnable() " + this.poseSource, true);
				DummyPose.ToUpdatePose = true;
				DummyPose.Origin = (WVR_PoseOriginModel)this.m_Origin;
				StartCoroutine(DummyPose.UpdatePoses());
			}
		}

		private void OnDisable()
		{
			if (DummyPose.ToUpdatePose)
			{
				Log.i("DummyPoseProvider", "OnDisable() " + this.poseSource, true);
				StopCoroutine(DummyPose.UpdatePoses());
				DummyPose.ToUpdatePose = false;
			}
		}

		Vector3 m_Position = Vector3.zero;
		Quaternion m_Rotation = Quaternion.identity;
		private Pose m_Pose = new Pose();
		public override PoseDataFlags GetPoseFromProvider(out Pose output)
		{
			switch (poseSource)
			{
				case TrackedPoseDriver.TrackedPose.Center:
					m_Pose.position = DummyPose.GetPosition(WVR_DeviceType.WVR_DeviceType_HMD);
					m_Pose.rotation = DummyPose.GetRotation(WVR_DeviceType.WVR_DeviceType_HMD);
					break;
				case TrackedPoseDriver.TrackedPose.LeftPose:
					m_Pose.position = DummyPose.GetPosition(WVR_DeviceType.WVR_DeviceType_Controller_Left);
					m_Pose.rotation = DummyPose.GetRotation(WVR_DeviceType.WVR_DeviceType_Controller_Left);
					break;
				case TrackedPoseDriver.TrackedPose.RightPose:
					m_Pose.position = DummyPose.GetPosition(WVR_DeviceType.WVR_DeviceType_Controller_Right);
					m_Pose.rotation = DummyPose.GetRotation(WVR_DeviceType.WVR_DeviceType_Controller_Right);
					break;
				default:
					m_Pose.position = Vector3.zero;
					m_Pose.rotation = Quaternion.identity;
					output = m_Pose;
					return PoseDataFlags.NoData;
			}

			output = m_Pose;
			return PoseDataFlags.Position | PoseDataFlags.Rotation;
		}
	}
}
#endif