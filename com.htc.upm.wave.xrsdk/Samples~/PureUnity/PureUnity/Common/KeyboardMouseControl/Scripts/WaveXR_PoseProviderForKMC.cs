using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

namespace Wave.XR.Sample.KMC
{
	public class WaveXR_PoseProviderForKMC : BasePoseProvider
	{
		public TrackedPoseDriver.TrackedPose poseSource = TrackedPoseDriver.TrackedPose.Center;
		public WaveXR_KeyboardMouseControl kmc;
		public Pose pose = new Pose();

		public override PoseDataFlags GetPoseFromProvider(out Pose output)
		{
			switch (poseSource)
			{
				case TrackedPoseDriver.TrackedPose.Center:
					if (kmc.CurrentTarget == 0)
					{
						pose.position = kmc.PoseHMD.m_AccPos;
						pose.rotation = kmc.PoseHMD.m_AccRot;
					}
					else
					{
						pose.position = Vector3.zero;
						pose.rotation = Quaternion.identity;
					}
					output = pose;
					return PoseDataFlags.Position | PoseDataFlags.Rotation;
				case TrackedPoseDriver.TrackedPose.LeftPose:
					pose.position = kmc.PoseCtrlL.m_AccPos;
					pose.rotation = kmc.PoseCtrlL.m_AccRot;
					output = pose;
					return PoseDataFlags.Position | PoseDataFlags.Rotation;
				case TrackedPoseDriver.TrackedPose.RightPose:
					pose.position = kmc.PoseCtrlR.m_AccPos;
					pose.rotation = kmc.PoseCtrlR.m_AccRot;
					output = pose;
					return PoseDataFlags.Position | PoseDataFlags.Rotation;
				default:
					output = pose;
					return PoseDataFlags.NoData;
			}
		}

	}
}
