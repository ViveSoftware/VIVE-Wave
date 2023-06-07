// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wave.Essence.Hand.StaticGesture
{
	/// <summary> A gesture is calculated by using 21 joints defined in the enumeration. </summary>
	public enum GesturePoint
	{
		Wrist = HandManager.HandJoint.Wrist,
		Palm = HandManager.HandJoint.Palm,

		Thumb_Joint0 = HandManager.HandJoint.Thumb_Joint0,
		Thumb_Joint1 = HandManager.HandJoint.Thumb_Joint1,
		Thumb_Joint2 = HandManager.HandJoint.Thumb_Joint2,
		Thumb_Tip = HandManager.HandJoint.Thumb_Tip,        // 5

		Index_Joint1 = HandManager.HandJoint.Index_Joint1,
		Index_Joint2 = HandManager.HandJoint.Index_Joint2,
		Index_Joint3 = HandManager.HandJoint.Index_Joint3,
		Index_Tip = HandManager.HandJoint.Index_Tip,        // 9

		Middle_Joint1 = HandManager.HandJoint.Middle_Joint1,
		Middle_Joint2 = HandManager.HandJoint.Middle_Joint2,
		Middle_Joint3 = HandManager.HandJoint.Middle_Joint3,
		Middle_Tip = HandManager.HandJoint.Middle_Tip,      // 13

		Ring_Joint1 = HandManager.HandJoint.Ring_Joint1,
		Ring_Joint2 = HandManager.HandJoint.Ring_Joint2,
		Ring_Joint3 = HandManager.HandJoint.Ring_Joint3,
		Ring_Tip = HandManager.HandJoint.Ring_Tip,          // 17

		Pinky_Joint1 = HandManager.HandJoint.Pinky_Joint1,
		Pinky_Joint2 = HandManager.HandJoint.Pinky_Joint2,
		Pinky_Joint3 = HandManager.HandJoint.Pinky_Joint3,
		Pinky_Tip = HandManager.HandJoint.Pinky_Tip,        // 21
	}
	/// <summary> SDK defined gesture types. </summary>
	public enum GestureType
	{
		Unknown = 0,  // All other gestures not in predefined set
		Point = 1,
		Fist = 2,
		OK = 3,
		Like = 4,
		Five = 5,
		Victory = 6,
	}

	public struct GestureHandData
	{
		public bool valid;
		public bool isTracked;
		public bool isLeft;
		public Vector3[] points;
		public Quaternion[] rotations;
		public GestureType gesture;
	}

	public static class WXRGestureHand
	{
		public readonly static GesturePoint[] s_GesturePoints = new GesturePoint[]
		{
			GesturePoint.Wrist,
			GesturePoint.Palm,

			GesturePoint.Thumb_Joint0,
			GesturePoint.Thumb_Joint1,
			GesturePoint.Thumb_Joint2,
			GesturePoint.Thumb_Tip,		// 5

			GesturePoint.Index_Joint1,
			GesturePoint.Index_Joint2,
			GesturePoint.Index_Joint3,
			GesturePoint.Index_Tip,		// 9

			GesturePoint.Middle_Joint1,
			GesturePoint.Middle_Joint2,
			GesturePoint.Middle_Joint3,
			GesturePoint.Middle_Tip,	// 13

			GesturePoint.Ring_Joint1,
			GesturePoint.Ring_Joint2,
			GesturePoint.Ring_Joint3,
			GesturePoint.Ring_Tip,		// 17

			GesturePoint.Pinky_Joint1,
			GesturePoint.Pinky_Joint2,
			GesturePoint.Pinky_Joint3,
			GesturePoint.Pinky_Tip,		// 21
		};
		public static int Index(this GesturePoint point)
		{
			for (int i = 0; i < s_GesturePoints.Length; i++)
			{
				if (s_GesturePoints[i] == point) { return i; }
			}
			return -1;
		}
		public static void Init(this ref GestureHandData hand, bool isLeft)
		{
			hand.valid = false;
			hand.isTracked = false;
			hand.isLeft = isLeft;
			hand.points = new Vector3[s_GesturePoints.Length];
			hand.rotations = new Quaternion[s_GesturePoints.Length];
			hand.gesture = GestureType.Unknown;
		}
		public static GestureType Type(this HandManager.GestureType type)
		{
			switch (type)
			{
				case HandManager.GestureType.Fist:
					return GestureType.Fist;
				case HandManager.GestureType.Five:
					return GestureType.Five;
				case HandManager.GestureType.IndexUp:
					return GestureType.Point;
				case HandManager.GestureType.OK:
					return GestureType.OK;
				case HandManager.GestureType.ThumbUp:
					return GestureType.Like;
				case HandManager.GestureType.Yeah:
					return GestureType.Victory;
				default:
					break;
			}
			return GestureType.Unknown;
		}

		static Dictionary<bool, int> pointFrame = new Dictionary<bool, int>()
		{
			{ false, -1 }, // right
			{ true, -1 }, // left
		};
		private static bool AllowUpdatePoints(bool isLeft)
		{
			if (pointFrame[isLeft] != Time.frameCount)
			{
				pointFrame[isLeft] = Time.frameCount;
				return true;
			}
			return false;
		}
		private static void ValidatePoints(ref GestureHandData hand)
		{
			hand.valid = (HandManager.Instance != null);
			if (!hand.valid) { hand.isTracked = false; }
			if (hand.points == null || hand.points.Length != s_GesturePoints.Length)
			{
				hand.points = new Vector3[s_GesturePoints.Length];
			}
			if (hand.rotations == null || hand.rotations.Length != s_GesturePoints.Length)
			{
				hand.rotations = new Quaternion[s_GesturePoints.Length];
			}
		}
		public static void UpdatePoints(ref GestureHandData hand)
		{
			if (!AllowUpdatePoints(hand.isLeft)) { return; }

			ValidatePoints(ref hand);

			if (hand.valid)
			{
				hand.isTracked = HandManager.Instance.IsHandPoseValid(hand.isLeft);

				for (int i = 0; i < s_GesturePoints.Length; i++)
				{
					GetJointPosition(s_GesturePoints[i], ref hand.points[i], hand.isLeft);
					GetJointRotation(s_GesturePoints[i], ref hand.rotations[i], hand.isLeft);
				}
			}
		}
		private static void GetJointPosition(GesturePoint joint, ref Vector3 position, bool isLeft)
		{
			if (HandManager.Instance != null)
			{
				if (!HandManager.Instance.GetJointPosition((HandManager.HandJoint)joint, ref position, isLeft))
				{
					position = Vector3.zero;
				}
			}
		}
		private static void GetJointRotation(GesturePoint joint, ref Quaternion rotation, bool isLeft)
		{
			if (HandManager.Instance != null)
			{
				if (!HandManager.Instance.GetJointRotation((HandManager.HandJoint)joint, ref rotation, isLeft))
				{
					rotation = Quaternion.identity;
				}
			}
		}

		/// <summary> Retrieves SDK-defined left or right hand gesture types. </summary>
		public static GestureType GetGesture(bool isLeftHand)
		{
			GestureType gesture = GestureType.Unknown;

			if (HandManager.Instance != null &&
				HandManager.Instance.GetHandGestureStatus() == HandManager.GestureStatus.Available)
			{
				// Checks the engine pre-defined gesture.
				gesture = HandManager.Instance.GetHandGesture(isLeftHand).Type();
			}
			else
			{
				// Checks the SDK pre-defined gesture.
				if (CustomGestureProvider.Current != null)
				{
					gesture = CustomGestureProvider.Current.GetHandGesture(isLeftHand);
				}
			}

			return gesture;
		}
		/// <summary> Retrieves SDK or custom defined left or right hand gesture types. </summary>
		public static string GetSingleHandGesture(bool isLeftHand)
		{
			string gesture = "Unknown";

			// 1. Checks the engine and SDK pre-defined gesture.
			if (HandManager.Instance != null &&
				HandManager.Instance.GetHandGestureStatus() == HandManager.GestureStatus.Available)
			{
				// Checks the engine pre-defined gesture.
				gesture = HandManager.Instance.GetHandGesture(isLeftHand).ToString();
			}

			// 2. No pre-defined gesture, checks the SDK custom gesture.
			if ((CustomGestureProvider.Current != null) &&
				(gesture.Equals(HandManager.GestureType.Unknown.ToString()) || gesture.Equals(HandManager.GestureType.Invalid.ToString()))
				)
			{
				gesture = CustomGestureProvider.Current.GetCustomGesture(isLeftHand);
			}

			return gesture;
		}
		/// <summary> Retrieves custom defined dual hand gesture types. </summary>
		public static string GetDualHandGesture()
		{
			if (CustomGestureProvider.Current == null) { return GestureType.Unknown.ToString(); }

			return CustomGestureProvider.Current.GetDualHandGesture();
		}
		/// <summary> Retrieves the finger states of left or right hand. Notify the null state. </summary>
		public static HandState GetState(bool isLeftHand)
		{
			if (CustomGestureProvider.Current == null) { return null; }

			return (isLeftHand ? CustomGestureProvider.Current.LeftHandState : CustomGestureProvider.Current.RightHandState);
		}
	}

	#region Gesture Finger State
	public enum NodeDistanceType { Near = 0, Far = 1 }
	public enum ThumbState { Close, Open }
	[Flags]
	public enum ThumbStateFlag { Close = 1, Open = 2 }
	public enum FingerState { Close, Relax, Open }
	[Flags]
	public enum FingerStateFlag { Close = 1, Relax = 2, Open = 4 }
	public static class CustomGestureHelper
	{
		public static bool IsMatch(this FingerStateFlag condition, FingerState state)
		{
			var stateCond = (FingerStateFlag)(1 << (int)state);
			return (stateCond & condition) == stateCond;
		}
		public static bool IsMatch(this ThumbStateFlag condition, ThumbState state)
		{
			var stateCond = (ThumbStateFlag)(1 << (int)state);
			return (stateCond & condition) == stateCond;
		}
		public static bool IsMatch(this NodeDistanceType type, float distance, bool SingleHand = true)
		{
			if (type == NodeDistanceType.Near) return distance < (SingleHand ? 0.025f : 0.1f);
			// NodeDistanceType.Far
			return distance > (SingleHand ? 0.05f : 0.2f);
		}
		public static bool IsMatch(this RotationCondition rc, Quaternion palmRot)
		{
			if (HandManager.Instance == null) { return false; }

			float minX = rc.PitchOffset > 0 ? 0				 : 360 + rc.PitchOffset;
			float maxX = rc.PitchOffset > 0 ? rc.PitchOffset : 360;

			float minY = rc.YawOffset > 0 ? 0				: 360 + rc.YawOffset;
			float maxY = rc.YawOffset > 0 ? rc.YawOffset	: 360;

			float minZ = rc.RollOffset > 0 ? 0				: 360 + rc.RollOffset;
			float maxZ = rc.RollOffset > 0 ? rc.RollOffset	: 360;

			if (rc.PitchOffset != 0)
			{
				if (palmRot.eulerAngles.x < minX || palmRot.eulerAngles.x > maxX) { return false; }
			}
			if (rc.YawOffset != 0)
			{
				if (palmRot.eulerAngles.y < minY || palmRot.eulerAngles.y > maxY) { return false; }
			}
			if (rc.RollOffset != 0)
			{
				if (palmRot.eulerAngles.z < minZ || palmRot.eulerAngles.z > maxZ) { return false; }
			}

			return true;
		}
	}

	public class HandState
	{
		public ThumbState thumb;
		public FingerState index;
		public FingerState middle;
		public FingerState ring;
		public FingerState pinky;
	}
	#endregion

	class EnumFlagsAttribute : PropertyAttribute
	{
		public EnumFlagsAttribute() { }
	}
	[Serializable]
	public class NodeDistanceCondition
	{
		public int node1;
		public int node2;
		public NodeDistanceType distance;
	}
	[Serializable]
	public class RotationCondition
	{
		[SerializeField]
		[Range(-360, 360)]
		public float PitchOffset = 0;
		[SerializeField]
		[Range(-360, 360)]
		public float YawOffset = 0;
		[SerializeField]
		[Range(-360, 360)]
		public float RollOffset = 0;

		public RotationCondition()
		{
			PitchOffset = 0;
			YawOffset = 0;
			RollOffset = 0;
		}
	}
	[Serializable]
	public class CustomGestureCondition
	{
		[EnumFlags]
		public ThumbStateFlag Thumb = (ThumbStateFlag)(-1);
		[EnumFlags]
		public FingerStateFlag Index = (FingerStateFlag)(-1);
		[EnumFlags]
		public FingerStateFlag Middle = (FingerStateFlag)(-1);
		[EnumFlags]
		public FingerStateFlag Ring = (FingerStateFlag)(-1);
		[EnumFlags]
		public FingerStateFlag Pinky = (FingerStateFlag)(-1);

		public List<NodeDistanceCondition> FingerTipDistance = new List<NodeDistanceCondition>();

		public RotationCondition PalmRotation = new RotationCondition();

		public bool CheckHandMatch(GestureHandData hand, HandState state)
		{
			if (!hand.isTracked || state == null) return false;

			// 1. Check hand states.
			if (!Thumb.IsMatch(state.thumb)) return false;
			if (!Index.IsMatch(state.index)) return false;
			if (!Middle.IsMatch(state.middle)) return false;
			if (!Ring.IsMatch(state.ring)) return false;
			if (!Pinky.IsMatch(state.pinky)) return false;

			// 2. Check finger distance.
			if (FingerTipDistance != null)
			{
				if (FingerTipDistance != null && FingerTipDistance.Count > 0)
				{
					for (int i = 0; i < FingerTipDistance.Count; i++)
					{
						if (FingerTipDistance[i].node1 < 0 || FingerTipDistance[i].node1 >= WXRGestureHand.s_GesturePoints.Length ||
							FingerTipDistance[i].node2 < 0 || FingerTipDistance[i].node2 >= WXRGestureHand.s_GesturePoints.Length)
						{
							return false;
						}

						var distance = Vector3.Distance(hand.points[FingerTipDistance[i].node1], hand.points[FingerTipDistance[i].node2]);
						if (!FingerTipDistance[i].distance.IsMatch(distance)) return false;
					}
				}
			}

			// 3. Check hand rotation.
			if (PalmRotation != null)
			{
				if (!PalmRotation.IsMatch(hand.rotations[GesturePoint.Palm.Index()]))
					return false;
			}

			return true;
		}
	}

	public interface ICustomGestureProducer
	{
		void CheckGesture();
	}
	public abstract class BaseSingleHandGestureProducer : ScriptableObject, ICustomGestureProducer
	{
		public string Name;
		public bool IsLeftMatch { get; protected set; }
		public bool IsRightMatch { get; protected set; }
		public abstract void CheckGesture();
	}
	public abstract class BaseDualHandGestureProducer : ScriptableObject, ICustomGestureProducer
	{
		public string Name;
		public bool IsMatch { get; protected set; }
		public abstract void CheckGesture();
	}
}
