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

namespace Wave.Essence.Hand.NearInteraction
{
	public enum NearJoint
	{
		Palm = HandManager.HandJoint.Palm,
		Wrist = HandManager.HandJoint.Wrist,
		Thumb_Joint0 = HandManager.HandJoint.Thumb_Joint0,
		Thumb_Joint1 = HandManager.HandJoint.Thumb_Joint1,
		Thumb_Joint2 = HandManager.HandJoint.Thumb_Joint2,
		Thumb_Tip = HandManager.HandJoint.Thumb_Tip,
		Index_Joint0 = HandManager.HandJoint.Index_Joint0,
		Index_Joint1 = HandManager.HandJoint.Index_Joint1,
		Index_Joint2 = HandManager.HandJoint.Index_Joint2,
		Index_Joint3 = HandManager.HandJoint.Index_Joint3,
		Index_Tip = HandManager.HandJoint.Index_Tip,
		Middle_Joint0 = HandManager.HandJoint.Middle_Joint0,
		Middle_Joint1 = HandManager.HandJoint.Middle_Joint1,
		Middle_Joint2 = HandManager.HandJoint.Middle_Joint2,
		Middle_Joint3 = HandManager.HandJoint.Middle_Joint3,
		Middle_Tip = HandManager.HandJoint.Middle_Tip,
		Ring_Joint0 = HandManager.HandJoint.Ring_Joint0,
		Ring_Joint1 = HandManager.HandJoint.Ring_Joint1,
		Ring_Joint2 = HandManager.HandJoint.Ring_Joint2,
		Ring_Joint3 = HandManager.HandJoint.Ring_Joint3,
		Ring_Tip = HandManager.HandJoint.Ring_Tip,
		Pinky_Joint0 = HandManager.HandJoint.Pinky_Joint0,
		Pinky_Joint1 = HandManager.HandJoint.Pinky_Joint1,
		Pinky_Joint2 = HandManager.HandJoint.Pinky_Joint2,
		Pinky_Joint3 = HandManager.HandJoint.Index_Joint3,
		Pinky_Tip = HandManager.HandJoint.Pinky_Tip,
	}
	public struct NearJointData
	{
		public Vector3 position;
		public Quaternion rotation;
	}
	public struct NearFingerData
	{
		public Vector3 direction;
		public NearJointData joint0;
		public NearJointData joint1;
		public NearJointData joint2;
		public NearJointData joint3;
		public NearJointData tip;
	}
	public struct NearHandData
	{
		public bool valid;
		public bool isTracked;
		public bool isLeft;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 normal;
		public Vector3 direction;
		public NearJointData wrist;
		public NearFingerData thumb;
		public NearFingerData index;
		public NearFingerData middle;
		public NearFingerData ring;
		public NearFingerData pinky;
		public NearFingerData[] fingers;
	}

	public static class WXRHand
	{
		private static NearHandData left, right;
		private static void validate(bool isLeft)
		{
			if (isLeft)
			{
				left.valid = (HandManager.Instance != null);
				if (!left.valid) { left.isTracked = false; }
			}
			else
			{
				right.valid = (HandManager.Instance != null);
				if (!right.valid) { right.isTracked = false; }
			}
		}

		static int updateFrameCountLeft = 0, updateFrameCountRight = 0;
		private static bool AllowUpdateData(bool isLeft)
		{
			if (isLeft && (updateFrameCountLeft != Time.frameCount))
			{
				updateFrameCountLeft = Time.frameCount;
				return true;
			}
			if (!isLeft && (updateFrameCountRight != Time.frameCount))
			{
				updateFrameCountRight = Time.frameCount;
				return true;
			}
			return false;
		}
		private static void UpdateData(bool isLeft)
		{
			if (!AllowUpdateData(isLeft)) { return; }

			if (left.fingers == null || left.fingers.Length != 5) { left.fingers = new NearFingerData[5]; }
			if (right.fingers == null || right.fingers.Length != 5) { right.fingers = new NearFingerData[5]; }

			validate(isLeft);
			if (isLeft && left.valid)
			{
				var inst = HandManager.Instance;
				left.isLeft = true;
				left.isTracked = inst.IsHandPoseValid(true);

				/// Use middle joint 0 as the palm due to current palm and wrist pose are equivalent.
				GetJointPosition(NearJoint.Middle_Joint0/*Palm*/, ref left.position, true);
				GetJointRotation(NearJoint.Middle_Joint0/*Palm*/, ref left.rotation, true);
				left.direction = left.rotation * Vector3.up;
				left.normal = left.rotation * Vector3.forward;

				GetJointPosition(NearJoint.Wrist, ref left.wrist.position, true);
				GetJointRotation(NearJoint.Wrist, ref left.wrist.rotation, true);

				GetJointPosition(NearJoint.Thumb_Joint0, ref left.thumb.joint0.position, true);
				GetJointPosition(NearJoint.Thumb_Joint1, ref left.thumb.joint1.position, true);
				GetJointPosition(NearJoint.Thumb_Joint2, ref left.thumb.joint2.position, true);
				GetJointPosition(NearJoint.Thumb_Tip, ref left.thumb.tip.position, true);
				GetJointRotation(NearJoint.Thumb_Joint0, ref left.thumb.joint0.rotation, true);
				GetJointRotation(NearJoint.Thumb_Joint1, ref left.thumb.joint1.rotation, true);
				GetJointRotation(NearJoint.Thumb_Joint2, ref left.thumb.joint2.rotation, true);
				GetJointRotation(NearJoint.Thumb_Tip, ref left.thumb.tip.rotation, true);
				left.thumb.direction = left.thumb.tip.position - left.thumb.joint2.position;

				GetJointPosition(NearJoint.Index_Joint0, ref left.index.joint0.position, true);
				GetJointPosition(NearJoint.Index_Joint1, ref left.index.joint1.position, true);
				GetJointPosition(NearJoint.Index_Joint2, ref left.index.joint2.position, true);
				GetJointPosition(NearJoint.Index_Joint3, ref left.index.joint3.position, true);
				GetJointPosition(NearJoint.Index_Tip, ref left.index.tip.position, true);
				GetJointRotation(NearJoint.Index_Joint0, ref left.index.joint0.rotation, true);
				GetJointRotation(NearJoint.Index_Joint1, ref left.index.joint1.rotation, true);
				GetJointRotation(NearJoint.Index_Joint2, ref left.index.joint2.rotation, true);
				GetJointRotation(NearJoint.Index_Joint3, ref left.index.joint3.rotation, true);
				GetJointRotation(NearJoint.Index_Tip, ref left.index.tip.rotation, true);
				left.index.direction = left.index.tip.position - left.index.joint3.position;

				GetJointPosition(NearJoint.Middle_Joint0, ref left.middle.joint0.position, true);
				GetJointPosition(NearJoint.Middle_Joint1, ref left.middle.joint1.position, true);
				GetJointPosition(NearJoint.Middle_Joint2, ref left.middle.joint2.position, true);
				GetJointPosition(NearJoint.Middle_Joint3, ref left.middle.joint3.position, true);
				GetJointPosition(NearJoint.Middle_Tip, ref left.middle.tip.position, true);
				GetJointRotation(NearJoint.Middle_Joint0, ref left.middle.joint0.rotation, true);
				GetJointRotation(NearJoint.Middle_Joint1, ref left.middle.joint1.rotation, true);
				GetJointRotation(NearJoint.Middle_Joint2, ref left.middle.joint2.rotation, true);
				GetJointRotation(NearJoint.Middle_Joint3, ref left.middle.joint3.rotation, true);
				GetJointRotation(NearJoint.Middle_Tip, ref left.middle.tip.rotation, true);
				left.middle.direction = left.middle.tip.position - left.middle.joint3.position;

				GetJointPosition(NearJoint.Ring_Joint0, ref left.ring.joint0.position, true);
				GetJointPosition(NearJoint.Ring_Joint1, ref left.ring.joint1.position, true);
				GetJointPosition(NearJoint.Ring_Joint2, ref left.ring.joint2.position, true);
				GetJointPosition(NearJoint.Ring_Joint3, ref left.ring.joint3.position, true);
				GetJointPosition(NearJoint.Ring_Tip, ref left.ring.tip.position, true);
				GetJointRotation(NearJoint.Ring_Joint0, ref left.ring.joint0.rotation, true);
				GetJointRotation(NearJoint.Ring_Joint1, ref left.ring.joint1.rotation, true);
				GetJointRotation(NearJoint.Ring_Joint2, ref left.ring.joint2.rotation, true);
				GetJointRotation(NearJoint.Ring_Joint3, ref left.ring.joint3.rotation, true);
				GetJointRotation(NearJoint.Ring_Tip, ref left.ring.tip.rotation, true);
				left.ring.direction = left.ring.tip.position - left.ring.joint3.position;

				GetJointPosition(NearJoint.Pinky_Joint0, ref left.pinky.joint0.position, true);
				GetJointPosition(NearJoint.Pinky_Joint1, ref left.pinky.joint1.position, true);
				GetJointPosition(NearJoint.Pinky_Joint2, ref left.pinky.joint2.position, true);
				GetJointPosition(NearJoint.Pinky_Joint3, ref left.pinky.joint3.position, true);
				GetJointPosition(NearJoint.Pinky_Tip, ref left.pinky.tip.position, true);
				GetJointRotation(NearJoint.Pinky_Joint0, ref left.pinky.joint0.rotation, true);
				GetJointRotation(NearJoint.Pinky_Joint1, ref left.pinky.joint1.rotation, true);
				GetJointRotation(NearJoint.Pinky_Joint2, ref left.pinky.joint2.rotation, true);
				GetJointRotation(NearJoint.Pinky_Joint3, ref left.pinky.joint3.rotation, true);
				GetJointRotation(NearJoint.Pinky_Tip, ref left.pinky.tip.rotation, true);
				left.pinky.direction = left.pinky.tip.position - left.pinky.joint3.position;

				left.fingers[0] = left.thumb;
				left.fingers[1] = left.index;
				left.fingers[2] = left.middle;
				left.fingers[3] = left.ring;
				left.fingers[4] = left.pinky;
			}
			if (!isLeft && right.valid)
			{
				var inst = HandManager.Instance;
				right.isLeft = false;
				right.isTracked = inst.IsHandPoseValid(false);

				/// Use middle joint 0 as the palm due to current palm and wrist pose are equivalent.
				GetJointPosition(NearJoint.Middle_Joint0/*Palm*/, ref right.position, false);
				GetJointRotation(NearJoint.Middle_Joint0/*Palm*/, ref right.rotation, false);
				right.direction = right.rotation * Vector3.up;
				right.normal = right.rotation * Vector3.forward;

				GetJointPosition(NearJoint.Wrist, ref right.wrist.position, false);
				GetJointRotation(NearJoint.Wrist, ref right.wrist.rotation, false);

				GetJointPosition(NearJoint.Thumb_Joint0, ref right.thumb.joint0.position, false);
				GetJointPosition(NearJoint.Thumb_Joint1, ref right.thumb.joint1.position, false);
				GetJointPosition(NearJoint.Thumb_Joint2, ref right.thumb.joint2.position, false);
				GetJointPosition(NearJoint.Thumb_Tip, ref right.thumb.tip.position, false);
				GetJointRotation(NearJoint.Thumb_Joint0, ref right.thumb.joint0.rotation, false);
				GetJointRotation(NearJoint.Thumb_Joint1, ref right.thumb.joint1.rotation, false);
				GetJointRotation(NearJoint.Thumb_Joint2, ref right.thumb.joint2.rotation, false);
				GetJointRotation(NearJoint.Thumb_Tip, ref right.thumb.tip.rotation, false);
				right.thumb.direction = right.thumb.tip.position - right.thumb.joint2.position;

				GetJointPosition(NearJoint.Index_Joint0, ref right.index.joint0.position, false);
				GetJointPosition(NearJoint.Index_Joint1, ref right.index.joint1.position, false);
				GetJointPosition(NearJoint.Index_Joint2, ref right.index.joint2.position, false);
				GetJointPosition(NearJoint.Index_Joint3, ref right.index.joint3.position, false);
				GetJointPosition(NearJoint.Index_Tip, ref right.index.tip.position, false);
				GetJointRotation(NearJoint.Index_Joint0, ref right.index.joint0.rotation, false);
				GetJointRotation(NearJoint.Index_Joint1, ref right.index.joint1.rotation, false);
				GetJointRotation(NearJoint.Index_Joint2, ref right.index.joint2.rotation, false);
				GetJointRotation(NearJoint.Index_Joint3, ref right.index.joint3.rotation, false);
				GetJointRotation(NearJoint.Index_Tip, ref right.index.tip.rotation, false);
				right.index.direction = right.index.tip.position - right.index.joint3.position;

				GetJointPosition(NearJoint.Middle_Joint0, ref right.middle.joint0.position, false);
				GetJointPosition(NearJoint.Middle_Joint1, ref right.middle.joint1.position, false);
				GetJointPosition(NearJoint.Middle_Joint2, ref right.middle.joint2.position, false);
				GetJointPosition(NearJoint.Middle_Joint3, ref right.middle.joint3.position, false);
				GetJointPosition(NearJoint.Middle_Tip, ref right.middle.tip.position, false);
				GetJointRotation(NearJoint.Middle_Joint0, ref right.middle.joint0.rotation, false);
				GetJointRotation(NearJoint.Middle_Joint1, ref right.middle.joint1.rotation, false);
				GetJointRotation(NearJoint.Middle_Joint2, ref right.middle.joint2.rotation, false);
				GetJointRotation(NearJoint.Middle_Joint3, ref right.middle.joint3.rotation, false);
				GetJointRotation(NearJoint.Middle_Tip, ref right.middle.tip.rotation, false);
				right.middle.direction = right.middle.tip.position - right.middle.joint3.position;

				GetJointPosition(NearJoint.Ring_Joint0, ref right.ring.joint0.position, false);
				GetJointPosition(NearJoint.Ring_Joint1, ref right.ring.joint1.position, false);
				GetJointPosition(NearJoint.Ring_Joint2, ref right.ring.joint2.position, false);
				GetJointPosition(NearJoint.Ring_Joint3, ref right.ring.joint3.position, false);
				GetJointPosition(NearJoint.Ring_Tip, ref right.ring.tip.position, false);
				GetJointRotation(NearJoint.Ring_Joint0, ref right.ring.joint0.rotation, false);
				GetJointRotation(NearJoint.Ring_Joint1, ref right.ring.joint1.rotation, false);
				GetJointRotation(NearJoint.Ring_Joint2, ref right.ring.joint2.rotation, false);
				GetJointRotation(NearJoint.Ring_Joint3, ref right.ring.joint3.rotation, false);
				GetJointRotation(NearJoint.Ring_Tip, ref right.ring.tip.rotation, false);
				right.ring.direction = right.ring.tip.position - right.ring.joint3.position;

				GetJointPosition(NearJoint.Pinky_Joint0, ref right.pinky.joint0.position, false);
				GetJointPosition(NearJoint.Pinky_Joint1, ref right.pinky.joint1.position, false);
				GetJointPosition(NearJoint.Pinky_Joint2, ref right.pinky.joint2.position, false);
				GetJointPosition(NearJoint.Pinky_Joint3, ref right.pinky.joint3.position, false);
				GetJointPosition(NearJoint.Pinky_Tip, ref right.pinky.tip.position, false);
				GetJointRotation(NearJoint.Pinky_Joint0, ref right.pinky.joint0.rotation, false);
				GetJointRotation(NearJoint.Pinky_Joint1, ref right.pinky.joint1.rotation, false);
				GetJointRotation(NearJoint.Pinky_Joint2, ref right.pinky.joint2.rotation, false);
				GetJointRotation(NearJoint.Pinky_Joint3, ref right.pinky.joint3.rotation, false);
				GetJointRotation(NearJoint.Pinky_Tip, ref right.pinky.tip.rotation, false);
				right.pinky.direction = right.pinky.tip.position - right.pinky.joint3.position;

				right.fingers[0] = right.thumb;
				right.fingers[1] = right.index;
				right.fingers[2] = right.middle;
				right.fingers[3] = right.ring;
				right.fingers[4] = right.pinky;
			}
		}
		public static NearHandData Get(bool isLeft)
		{
			UpdateData(isLeft);
			return isLeft ? left : right;
		}
		public static Vector3 GetHand3DPosition(bool isLeft)
		{
			if (HandManager.Instance == null) { return Vector3.zero; }

			return isLeft ? left.wrist.position : right.wrist.position;
		}
		public static void GetJointPosition(NearJoint joint, ref Vector3 position, bool isLeft)
		{
			Vector3 pos = Vector3.zero;

			if (HandManager.Instance != null)
			{
				if (HandManager.Instance.GetJointPosition((HandManager.HandJoint)joint, ref pos, isLeft))
				{
					position = pos;
				}
			}
		}
		public static void GetJointRotation(NearJoint joint, ref Quaternion rotation, bool isLeft)
		{
			Quaternion rot = Quaternion.identity;

			if (HandManager.Instance != null)
			{
				if (HandManager.Instance.GetJointRotation((HandManager.HandJoint)joint, ref rot, isLeft))
				{
					rotation = rot;
				}
			}
		}
	}
}
