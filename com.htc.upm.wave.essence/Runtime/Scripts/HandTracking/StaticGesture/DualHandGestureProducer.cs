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
using System.Collections.Generic;

namespace Wave.Essence.Hand.StaticGesture
{
	// Note: attach to NodeDistanceCondition or List<NodeDistanceCondition> if it's used for dual hand
	public class DualHandAttribute : PropertyAttribute
	{
		public DualHandAttribute() { }
	}

	[CreateAssetMenu(fileName = "CustomGesture",
					 menuName = "Wave/Dual Hand Gesture", order = 251)]
	public class DualHandGestureProducer : BaseDualHandGestureProducer
	{
		public CustomGestureCondition leftCondition;
		public CustomGestureCondition rightCondition;

		[DualHand]
		public List<NodeDistanceCondition> CrossHandFingerTipDistance;

		public override void CheckGesture()
		{
			IsMatch = false;

			if (CustomGestureProvider.Current == null) { return; }

			if (!leftCondition.CheckHandMatch(CustomGestureProvider.Current.LeftHand, CustomGestureProvider.Current.LeftHandState))
				return;
			if (!rightCondition.CheckHandMatch(CustomGestureProvider.Current.RightHand, CustomGestureProvider.Current.RightHandState))
				return;

			if (CrossHandFingerTipDistance != null && CrossHandFingerTipDistance.Count > 0)
			{
				for (int i = 0; i < CrossHandFingerTipDistance.Count; i++)
				{
					if (CrossHandFingerTipDistance[i].node1 < 0 || CrossHandFingerTipDistance[i].node1 >= WXRGestureHand.s_GesturePoints.Length ||
						CrossHandFingerTipDistance[i].node2 < 0 || CrossHandFingerTipDistance[i].node2 >= WXRGestureHand.s_GesturePoints.Length)
					{
						return;
					}

					var distance = Vector3.Distance(
						CustomGestureProvider.Current.LeftHand.points[CrossHandFingerTipDistance[i].node1],
						CustomGestureProvider.Current.RightHand.points[CrossHandFingerTipDistance[i].node2]
					);
					if (!CrossHandFingerTipDistance[i].distance.IsMatch(distance, false)) return;
				}
			}

			IsMatch = true;
		}
	}

}
