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
using System;

namespace Wave.Essence.Hand
{
	public class BonePoseImpl
	{
		private const string LOG_TAG = "Wave.Essence.Hand.BonePoseImpl";
		private void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		public enum Bones
		{
			ROOT = 0,

			LEFT_WRIST,
			LEFT_THUMB_JOINT1,
			LEFT_THUMB_JOINT2,
			LEFT_THUMB_JOINT3,
			LEFT_THUMB_TIP,
			LEFT_INDEX_JOINT1,
			LEFT_INDEX_JOINT2,
			LEFT_INDEX_JOINT3,
			LEFT_INDEX_TIP,
			LEFT_MIDDLE_JOINT1,
			LEFT_MIDDLE_JOINT2,
			LEFT_MIDDLE_JOINT3,
			LEFT_MIDDLE_TIP,
			LEFT_RING_JOINT1,
			LEFT_RING_JOINT2,
			LEFT_RING_JOINT3,
			LEFT_RING_TIP,
			LEFT_PINKY_JOINT1,
			LEFT_PINKY_JOINT2,
			LEFT_PINKY_JOINT3,
			LEFT_PINKY_TIP,
			// Total 21 left bones.

			RIGHT_WRIST,
			RIGHT_THUMB_JOINT1,
			RIGHT_THUMB_JOINT2,
			RIGHT_THUMB_JOINT3,
			RIGHT_THUMB_TIP,
			RIGHT_INDEX_JOINT1,
			RIGHT_INDEX_JOINT2,
			RIGHT_INDEX_JOINT3,
			RIGHT_INDEX_TIP,
			RIGHT_MIDDLE_JOINT1,
			RIGHT_MIDDLE_JOINT2,
			RIGHT_MIDDLE_JOINT3,
			RIGHT_MIDDLE_TIP,
			RIGHT_RING_JOINT1,
			RIGHT_RING_JOINT2,
			RIGHT_RING_JOINT3,
			RIGHT_RING_TIP,
			RIGHT_PINKY_JOINT1,
			RIGHT_PINKY_JOINT2,
			RIGHT_PINKY_JOINT3,
			RIGHT_PINKY_TIP,
			// Total 21 right bones.
		};

		private readonly Bones[] a_LeftBones = new Bones[] {
			Bones.LEFT_WRIST,
			Bones.LEFT_THUMB_JOINT1,
			Bones.LEFT_THUMB_JOINT2,
			Bones.LEFT_THUMB_JOINT3,
			Bones.LEFT_THUMB_TIP,
			Bones.LEFT_INDEX_JOINT1,
			Bones.LEFT_INDEX_JOINT2,
			Bones.LEFT_INDEX_JOINT3,
			Bones.LEFT_INDEX_TIP,
			Bones.LEFT_MIDDLE_JOINT1,
			Bones.LEFT_MIDDLE_JOINT2,
			Bones.LEFT_MIDDLE_JOINT3,
			Bones.LEFT_MIDDLE_TIP,
			Bones.LEFT_RING_JOINT1,
			Bones.LEFT_RING_JOINT2,
			Bones.LEFT_RING_JOINT3,
			Bones.LEFT_RING_TIP,
			Bones.LEFT_PINKY_JOINT1,
			Bones.LEFT_PINKY_JOINT2,
			Bones.LEFT_PINKY_JOINT3,
			Bones.LEFT_PINKY_TIP
		};

		private readonly Bones[] a_RightBones = new Bones[] {
			Bones.RIGHT_WRIST,
			Bones.RIGHT_THUMB_JOINT1,
			Bones.RIGHT_THUMB_JOINT2,
			Bones.RIGHT_THUMB_JOINT3,
			Bones.RIGHT_THUMB_TIP,
			Bones.RIGHT_INDEX_JOINT1,
			Bones.RIGHT_INDEX_JOINT2,
			Bones.RIGHT_INDEX_JOINT3,
			Bones.RIGHT_INDEX_TIP,
			Bones.RIGHT_MIDDLE_JOINT1,
			Bones.RIGHT_MIDDLE_JOINT2,
			Bones.RIGHT_MIDDLE_JOINT3,
			Bones.RIGHT_MIDDLE_TIP,
			Bones.RIGHT_RING_JOINT1,
			Bones.RIGHT_RING_JOINT2,
			Bones.RIGHT_RING_JOINT3,
			Bones.RIGHT_RING_TIP,
			Bones.RIGHT_PINKY_JOINT1,
			Bones.RIGHT_PINKY_JOINT2,
			Bones.RIGHT_PINKY_JOINT3,
			Bones.RIGHT_PINKY_TIP
		};

		private class BoneData
		{
			private RigidTransform rigidTransform = RigidTransform.identity;

			public BoneData()
			{
				rigidTransform = RigidTransform.identity;
			}

			public RigidTransform GetTransform() { return rigidTransform; }
			public Vector3 GetPosition() { return rigidTransform.pos; }
			public void SetPosition(Vector3 in_pos) { rigidTransform.pos = in_pos; }
			public Quaternion GetRotation() { return rigidTransform.rot; }
			public void SetRotation(Quaternion in_rot) { rigidTransform.rot = in_rot; }
		};

		static BoneData[] s_BoneData;

		public BonePoseImpl()
		{
			DEBUG("BonePoseImpl()");
			s_BoneData = new BoneData[Enum.GetNames(typeof(Bones)).Length];
			for (int i = 0; i < Enum.GetNames(typeof(Bones)).Length; i++)
			{
				s_BoneData[i] = new BoneData();
			}
		}

		int trackFrameCount = -1;
		private bool AllowGetTrackingData()
		{
			if (Time.frameCount != trackFrameCount)
			{
				trackFrameCount = Time.frameCount;
				return true;
			}

			return false;
		}

		[Obsolete("This function is deprecated.")]
		public RigidTransform GetBoneTransform(Bones bone_type)
		{
			RigidTransform rt = RigidTransform.identity;

			if (HandManager.Instance == null)
				return RigidTransform.identity;

			switch (bone_type)
			{
				case Bones.LEFT_WRIST:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Wrist, ref rt.pos, true);
					HandManager.Instance.GetJointRotation(HandManager.HandJoint.Wrist, ref rt.rot, true);
					break;
				case Bones.LEFT_THUMB_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint0, ref rt.pos, true);
					break;
				case Bones.LEFT_THUMB_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint1, ref rt.pos, true);
					break;
				case Bones.LEFT_THUMB_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint2, ref rt.pos, true);
					break;
				case Bones.LEFT_THUMB_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Tip, ref rt.pos, true);
					break;
				case Bones.LEFT_INDEX_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint1, ref rt.pos, true);
					break;
				case Bones.LEFT_INDEX_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint2, ref rt.pos, true);
					break;
				case Bones.LEFT_INDEX_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint3, ref rt.pos, true);
					break;
				case Bones.LEFT_INDEX_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Tip, ref rt.pos, true);
					break;
				case Bones.LEFT_MIDDLE_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint1, ref rt.pos, true);
					break;
				case Bones.LEFT_MIDDLE_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint2, ref rt.pos, true);
					break;
				case Bones.LEFT_MIDDLE_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint3, ref rt.pos, true);
					break;
				case Bones.LEFT_MIDDLE_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Tip, ref rt.pos, true);
					break;
				case Bones.LEFT_RING_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint1, ref rt.pos, true);
					break;
				case Bones.LEFT_RING_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint2, ref rt.pos, true);
					break;
				case Bones.LEFT_RING_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint3, ref rt.pos, true);
					break;
				case Bones.LEFT_RING_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Tip, ref rt.pos, true);
					break;
				case Bones.LEFT_PINKY_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint1, ref rt.pos, true);
					break;
				case Bones.LEFT_PINKY_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint2, ref rt.pos, true);
					break;
				case Bones.LEFT_PINKY_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint3, ref rt.pos, true);
					break;
				case Bones.LEFT_PINKY_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Tip, ref rt.pos, true);
					break;
				case Bones.RIGHT_WRIST:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Wrist, ref rt.pos, false);
					HandManager.Instance.GetJointRotation(HandManager.HandJoint.Wrist, ref rt.rot, false);
					break;
				case Bones.RIGHT_THUMB_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint0, ref rt.pos, false);
					break;
				case Bones.RIGHT_THUMB_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint1, ref rt.pos, false);
					break;
				case Bones.RIGHT_THUMB_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Joint2, ref rt.pos, false);
					break;
				case Bones.RIGHT_THUMB_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Thumb_Tip, ref rt.pos, false);
					break;
				case Bones.RIGHT_INDEX_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint1, ref rt.pos, false);
					break;
				case Bones.RIGHT_INDEX_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint2, ref rt.pos, false);
					break;
				case Bones.RIGHT_INDEX_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Joint3, ref rt.pos, false);
					break;
				case Bones.RIGHT_INDEX_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Index_Tip, ref rt.pos, false);
					break;
				case Bones.RIGHT_MIDDLE_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint1, ref rt.pos, false);
					break;
				case Bones.RIGHT_MIDDLE_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint2, ref rt.pos, false);
					break;
				case Bones.RIGHT_MIDDLE_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Joint3, ref rt.pos, false);
					break;
				case Bones.RIGHT_MIDDLE_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Middle_Tip, ref rt.pos, false);
					break;
				case Bones.RIGHT_RING_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint1, ref rt.pos, false);
					break;
				case Bones.RIGHT_RING_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint2, ref rt.pos, false);
					break;
				case Bones.RIGHT_RING_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Joint3, ref rt.pos, false);
					break;
				case Bones.RIGHT_RING_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Ring_Tip, ref rt.pos, false);
					break;
				case Bones.RIGHT_PINKY_JOINT1:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint1, ref rt.pos, false);
					break;
				case Bones.RIGHT_PINKY_JOINT2:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint2, ref rt.pos, false);
					break;
				case Bones.RIGHT_PINKY_JOINT3:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Joint3, ref rt.pos, false);
					break;
				case Bones.RIGHT_PINKY_TIP:
					HandManager.Instance.GetJointPosition(HandManager.HandJoint.Pinky_Tip, ref rt.pos, false);
					break;
				default:
					break;
			}

			return rt;
		}
		[Obsolete("This function is deprecated.")]
		public RigidTransform GetBoneTransform(int index, bool isLeft)
		{
			int bone_index = (int)Bones.ROOT;

			if (isLeft)
				bone_index = index + 1;
			else
				bone_index = index + 22;

			return GetBoneTransform((Bones)bone_index);
		}
		[Obsolete("This function is deprecated.")]
		public bool IsBonePoseValid(Bones bone_type)
		{
			for (int i = 0; i < a_LeftBones.Length; i++)
			{
				if (a_LeftBones[i] == bone_type)
					return IsHandPoseValid(HandManager.HandType.Left);
			}

			for (int i = 0; i < a_RightBones.Length; i++)
			{
				if (a_RightBones[i] == bone_type)
					return IsHandPoseValid(HandManager.HandType.Right);
			}

			return false;
		}
		[Obsolete("This function is deprecated.")]
		public bool IsHandPoseValid(HandManager.HandType hand)
		{
			if (HandManager.Instance == null)
				return false;

			return HandManager.Instance.IsHandPoseValid(hand);
		}
		[Obsolete("This function is deprecated.")]
		public float GetBoneConfidence(Bones bone_type)
		{
			for (int i = 0; i < a_LeftBones.Length; i++)
			{
				if (a_LeftBones[i] == bone_type)
					return GetHandConfidence(HandManager.HandType.Left);
			}

			for (int i = 0; i < a_RightBones.Length; i++)
			{
				if (a_RightBones[i] == bone_type)
					return GetHandConfidence(HandManager.HandType.Right);
			}

			return 0;
		}
		[Obsolete("This function is deprecated.")]
		public float GetHandConfidence(HandManager.HandType hand)
		{
			if (HandManager.Instance == null)
				return 0;

			return HandManager.Instance.GetHandConfidence(hand);
		}
	}
}
