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
	public class HandInteractable : BaseInteractable
	{
		const string LOG_TAG = "Wave.Essence.Hand.NearInteraction.HandInteractable";
		void DEBUG(string msg) { Log.d(LOG_TAG, actable.name + ", " + msg, true); }

		#region Inspector
		[SerializeField]
		private bool m_Grabbable = true;
		public bool Grabbable { get { return m_Grabbable; } set { m_Grabbable = value; } }
		#endregion

		#region Properties
		private GrabBehavior m_GrabBehavior = null;
		public GrabBehavior GrabBehavior { get { return m_GrabBehavior; } set { m_GrabBehavior = value; } }
		public bool IsGrabbed { get { return (m_GrabBehavior != null); } }
		#endregion

		protected override void Update()
		{
			base.Update();
		}

		#region Atable movement
		private Vector3 beginPositionActable = Vector3.zero, beginPositionBehavior = Vector3.zero;
		private bool useGravityOrigin = false;
		private void BeginGrabActable(GrabBehavior behavior)
		{
			if (!m_Grabbable) { return; }

			DEBUG("BeginGrabActable() by " + behavior.gameObject.name);
			beginPositionActable = trans.position;
			beginPositionBehavior = behavior.GrabPosition;

			// Do NOT use gravity when grabbing.
			useGravityOrigin = rigid.useGravity;
			rigid.useGravity = false;
		}
		private void GrabbingActable(GrabBehavior behavior)
		{
			if (!m_Grabbable) { return; }
			if (m_GrabBehavior == behavior)
			{
				Vector3 posOffset = behavior.GrabPosition - beginPositionBehavior;
				trans.position = beginPositionActable + posOffset;
			}
		}
		private void EndGrabActable(GrabBehavior behavior)
		{
			if (!m_Grabbable) { return; }

			DEBUG("EndGrabActable() by " + behavior.gameObject.name);
			rigid.useGravity = useGravityOrigin;
		}
		#endregion

		/// <summary> Called by a GrabBehavior to inform that grab begins. </summary>
		public void GrabBeginHandle(GrabBehavior behavior)
		{
			DEBUG("GrabBeginHandle() by "
				+ (behavior.IsLeftHand ? "Left": "Right") + ": " + behavior.gameObject.name);

			m_GrabBehavior = behavior;
			BeginGrabActable(behavior);
		}
		/// <summary> Called by a GrabBehavior to inform grabbing. </summary>
		public void GrabbingHandle(GrabBehavior behavior)
		{
			if (m_GrabBehavior != behavior) { return; }

			/*DEBUG("GrabbingHandle() by "
				+ (behavior.IsLeftHand ? "Left" : "Right") + ": " + behavior.gameObject.name);*/

			GrabbingActable(behavior);
		}
		/// <summary> Called by a GrabBehavior to inform that grab ends. </summary>
		public void GrabEndHandle(GrabBehavior behavior)
		{
			if (m_GrabBehavior != behavior) { return; }

			DEBUG("GrabEndHandle() by "
				+ (behavior.IsLeftHand ? "Left" : "Right") + ": " + behavior.gameObject.name);

			m_GrabBehavior = null;
			EndGrabActable(behavior);
		}
	}
}