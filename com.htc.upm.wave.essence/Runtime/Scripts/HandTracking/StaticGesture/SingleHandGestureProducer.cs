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

namespace Wave.Essence.Hand.StaticGesture
{
	[CreateAssetMenu(fileName = "CustomGesture",
					 menuName = "Wave/Single Hand Gesture", order = 250)]
	public class SingleHandGestureProducer : BaseSingleHandGestureProducer
	{
		public CustomGestureCondition condition;

		public override void CheckGesture()
		{
			if (CustomGestureProvider.Current == null) { return; }

			IsLeftMatch =
				condition.CheckHandMatch(CustomGestureProvider.Current.LeftHand, CustomGestureProvider.Current.LeftHandState);
			IsRightMatch =
				condition.CheckHandMatch(CustomGestureProvider.Current.RightHand, CustomGestureProvider.Current.RightHandState);

			//DEBUG("CheckGesture() " + name + ", IsLeftMatch: " + IsLeftMatch + ", IsRightMatch: " + IsRightMatch);
		}
	}
}