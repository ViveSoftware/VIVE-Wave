// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;

namespace Wave.Essence.Hand.StaticGesture
{
	public class StaticGestureCalculator
	{
		private CustomGestureCondition Point = new CustomGestureCondition();
		private CustomGestureCondition Fist = new CustomGestureCondition();
		private CustomGestureCondition OK = new CustomGestureCondition();
		private CustomGestureCondition LikeLeft = new CustomGestureCondition();
		private CustomGestureCondition LikeRight = new CustomGestureCondition();
		private CustomGestureCondition Five = new CustomGestureCondition();
		private CustomGestureCondition Victory = new CustomGestureCondition();

		public StaticGestureCalculator()
		{
			/// Point
			Point.Thumb = ThumbStateFlag.Close;
			Point.Index = FingerStateFlag.Open;
			Point.Middle = FingerStateFlag.Close;
			Point.Ring = FingerStateFlag.Close;
			Point.Pinky = FingerStateFlag.Close | FingerStateFlag.Relax;
			// Point PalmRotation
			RotationCondition pointRot = new RotationCondition();
			pointRot.PitchOffset = -70;
			Point.PalmRotation = pointRot;

			/// Fist
			Fist.Thumb = ThumbStateFlag.Close;
			Fist.Index = FingerStateFlag.Close;
			Fist.Middle = FingerStateFlag.Close;
			Fist.Ring = FingerStateFlag.Close;
			Fist.Pinky = FingerStateFlag.Close;

			/// OK
			OK.Thumb = ThumbStateFlag.Close | ThumbStateFlag.Open;
			OK.Index = FingerStateFlag.Close | FingerStateFlag.Relax;
			OK.Middle = FingerStateFlag.Open;
			OK.Ring = FingerStateFlag.Open;
			OK.Pinky = FingerStateFlag.Open | FingerStateFlag.Relax;
			// OK NodeDistanceCondition
			NodeDistanceCondition okCond = new NodeDistanceCondition();
			okCond.node1 = GesturePoint.Thumb_Tip.Index(); // Thumb tip
			okCond.node2 = GesturePoint.Index_Tip.Index(); // Index tip
			okCond.distance = NodeDistanceType.Near;
			OK.FingerTipDistance = new List<NodeDistanceCondition> { okCond };
			// OK PalmRotation
			RotationCondition okRot = new RotationCondition();
			okRot.PitchOffset = 300;
			OK.PalmRotation = okRot;

			/// Left hand Like
			LikeLeft.Thumb = ThumbStateFlag.Open;
			LikeLeft.Index = FingerStateFlag.Close;
			LikeLeft.Middle = FingerStateFlag.Close;
			LikeLeft.Ring = FingerStateFlag.Close;
			LikeLeft.Pinky = FingerStateFlag.Close;
			// Like PalmRotation
			RotationCondition likeRotLeft = new RotationCondition();
			likeRotLeft.RollOffset = 120;
			LikeLeft.PalmRotation = likeRotLeft;

			/// Right hand Like
			LikeRight.Thumb = ThumbStateFlag.Open;
			LikeRight.Index = FingerStateFlag.Close;
			LikeRight.Middle = FingerStateFlag.Close;
			LikeRight.Ring = FingerStateFlag.Close;
			LikeRight.Pinky = FingerStateFlag.Close;
			// Like PalmRotation
			RotationCondition likeRotRight = new RotationCondition();
			likeRotRight.RollOffset = -120;
			LikeRight.PalmRotation = likeRotRight;

			// Five
			Five.Thumb = ThumbStateFlag.Open;
			Five.Index = FingerStateFlag.Open | FingerStateFlag.Relax;
			Five.Middle = FingerStateFlag.Open;
			Five.Ring = FingerStateFlag.Open;
			Five.Pinky = FingerStateFlag.Open;
			// Five NodeDistanceCondition
			NodeDistanceCondition fiveCond = new NodeDistanceCondition();
			fiveCond.node1 = GesturePoint.Thumb_Tip.Index(); // Thumb tip
			fiveCond.node2 = GesturePoint.Index_Tip.Index(); // Index tip
			fiveCond.distance = NodeDistanceType.Far;
			Five.FingerTipDistance = new List<NodeDistanceCondition> { fiveCond };

			// Victory
			Victory.Thumb = ThumbStateFlag.Close;
			Victory.Index = FingerStateFlag.Open | FingerStateFlag.Relax;
			Victory.Middle = FingerStateFlag.Open;
			Victory.Ring = FingerStateFlag.Close | FingerStateFlag.Relax;
			Victory.Pinky = FingerStateFlag.Close | FingerStateFlag.Relax;
		}

		public void CalculateHandGesture(ref GestureHandData hand, HandState state)
		{
			hand.gesture = GestureType.Unknown;

			if (!hand.isTracked || state == null) { return; }

			if (Point.CheckHandMatch(hand, state))
				hand.gesture = GestureType.Point;
			else if (OK.CheckHandMatch(hand, state))
				hand.gesture = GestureType.OK;
			else if (
				(hand.isLeft && LikeLeft.CheckHandMatch(hand, state)) ||
				(!hand.isLeft && LikeRight.CheckHandMatch(hand, state))
				)
			{
				hand.gesture = GestureType.Like;
			}
			else if (Victory.CheckHandMatch(hand, state))
				hand.gesture = GestureType.Victory;
			else if (Five.CheckHandMatch(hand, state))
				hand.gesture = GestureType.Five;
			else if (Fist.CheckHandMatch(hand, state))
			{
				/*if (Vector3.Dot(
					hand.points[GesturePoint.Middle_Joint2.Index()] - hand.points[GesturePoint.Index_Joint2.Index()],
					hand.points[GesturePoint.Thumb_Tip.Index()] - hand.points[GesturePoint.Index_Joint2.Index()]) > 0)*/
				{ hand.gesture = GestureType.Fist; }
			}
		}
	}
}