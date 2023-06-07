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
using Wave.Essence.Hand;
using Wave.Native;

namespace Wave.Essence.Raycast
{
	public class HandRaycastPointer : RaycastPointer
	{
		const string LOG_TAG = "Wave.Essence.Raycast.HandRaycastPointer";
		private void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, m_Hand + " " + msg, true);
		}
		private void INTERVAL(string msg) { if (printIntervalLog) { DEBUG(msg); } }

		#region Inspector
		[SerializeField]
		private HandManager.HandType m_Hand = HandManager.HandType.Right;
		public HandManager.HandType Hand { get { return m_Hand; } set { m_Hand = value; } }

		[Tooltip("To apply poses on the raycast pointer.")]
		[SerializeField]
		private bool m_UsePose = true;
		public bool UsePose { get { return m_UsePose; } set { m_UsePose = value; } }

		[Tooltip("Use system pinch threshold.")]
		[SerializeField]
		private bool m_UseDefaultPinch = false;
		public bool UseDefaultPinch { get { return m_UseDefaultPinch; } set { m_UseDefaultPinch = value; } }

		[Tooltip("Pinch strength to trigger events.")]
		[SerializeField]
		[Range(0, 1)]
		private float m_PinchStrength = .5f;
		public float PinchStrength { get { return m_PinchStrength; } set { m_PinchStrength = value; } }

		[Tooltip("Pinch will release when strength < (Pinch Strength - Pinch Release)")]
		[SerializeField]
		[Range(0, .9f)]
		private float m_PinchRelease = .5f;
		public float PinchRelease { get { return m_PinchRelease; } set { m_PinchRelease = value; } }

		const float kPinchThreshold = .5f;
		private void Validate()
		{
			// Checks PinchStrength.
			if (m_UseDefaultPinch && HandManager.Instance != null) { m_PinchStrength = HandManager.Instance.GetPinchThreshold(); }
			// Checks PinchRelease
			if (m_PinchStrength <= kPinchThreshold) { m_PinchRelease = 0; }
			else
			{
				// m_PinchStrength > kPinchThreshold but
				// m_PinchStrength < m_PinchRelease thus m_PinchStrength - m_PinchRelease < 0
				// So set m_PinchRelease to kPinchThreshold thus
				// m_PinchStrength (which > kPinchThreshold) - m_PinchRelease (= kPinchThreshold) > 0.
				// And m_PinchStrength - m_PinchRelease must > 0 or will always hold.
				if (m_PinchRelease > m_PinchStrength)
					m_PinchRelease = kPinchThreshold;
			}
		}

		[SerializeField]
		private bool m_AlwaysEnable = false;
		public bool AlwaysEnable { get { return m_AlwaysEnable; } set { m_AlwaysEnable = value; } }
		#endregion

		private Vector3 origin = Vector3.zero, direction = Vector3.zero;
		protected override void Update()
		{
			Validate();

			base.Update();

			INTERVAL("Update() m_UsePose: " + m_UsePose + ", m_PinchStrength: " + m_PinchStrength + ", m_PinchRelease: " + m_PinchRelease);

			if (!IsInteractable()) { return; }

			if (m_UsePose)
			{
				HandManager.Instance.GetPinchOrigin(ref origin, m_Hand);
				HandManager.Instance.GetPinchDirection(ref direction, m_Hand);

				transform.localPosition = origin;
				transform.localRotation = Quaternion.LookRotation(direction);
			}
		}

		private bool IsInteractable()
		{
			if (HandManager.Instance == null) { return false; }

			bool enabled = RaycastSwitch.Hand.Enabled;
			bool validPose = HandManager.Instance.IsHandPoseValid(m_Hand);
			bool hasFocus = ClientInterface.IsFocused;
			bool valid_motion = (HandManager.Instance.GetHandMotion(m_Hand) != HandManager.HandMotion.None);

			m_Interactable = (m_AlwaysEnable || enabled) && validPose && hasFocus && valid_motion;

			INTERVAL("IsInteractable() enabled: " + enabled + ", validPose: " + validPose + ", hasFocus: " + hasFocus + ", m_AlwaysEnable: " + m_AlwaysEnable);

			return m_Interactable;
		}

		bool eligibleForClick = false;
		protected override bool OnDown()
		{
			if (HandManager.Instance == null) { return false; }

			if (!eligibleForClick)
			{
				bool down = HandManager.Instance.GetPinchStrength(m_Hand) > m_PinchStrength;
				if (down)
				{
					eligibleForClick = true;
					return true;
				}
			}

			return false;
		}
		protected override bool OnHold()
		{
			float pinchStrength = HandManager.Instance.GetPinchStrength(m_Hand);
			bool hold = pinchStrength > m_PinchStrength;
			if (eligibleForClick) { hold = pinchStrength > (m_PinchStrength - m_PinchRelease); }
			if (!hold)
				eligibleForClick = false;
			return hold;
		}
	}
}
