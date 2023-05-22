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

namespace Wave.Essence
{
	public abstract class IActionEvent : MonoBehaviour
	{
		public enum ActionTaker
		{
			Device = 1,     // to XR_Device
			Hand = 2,       // to XR_Hand
			HandDevice = 3, // to XR_HandDevice
			//Eye,			//TBD
		}

		protected virtual void OnEnable()
		{
			ActionEventHub.AddAction(this);
		}

		protected virtual void OnDisable()
		{
			ActionEventHub.RemoveAction(this);
		}

		/// <summary> An action of processing like looking at, selecting, observing... </summary>
		public abstract bool processing { get; }
		/// <summary> An action of selected like decided, chosen, clicked... </summary>
		public abstract bool selected { get; }
		/// <summary> An action of griped like keeping, holding, picked... </summary>
		public abstract bool griped { get; }

		public abstract ActionTaker GetActionTaker(out uint typeId);
	}
}