// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Text;
using UnityEngine;
using Wave.Essence.Hand;
using Wave.Native;

namespace Wave.Essence.Raycast
{
	public class HandRaycastPointer : RaycastPointer
	{
		const string LOG_TAG = "Wave.Essence.Raycast.HandRaycastPointer";
		private void DEBUG(StringBuilder msg) { Log.d(LOG_TAG, msg, true); }

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

		private float m_PinchStrengthCustom = .5f;
		[Tooltip("Pinch strength to trigger events.")]
		[SerializeField]
		[Range(0, 1)]
		private float m_PinchStrength = .5f;
		public float PinchStrength {
			get { return m_PinchStrength; }
			set {
				m_PinchStrength = value;
				m_PinchStrengthCustom = value;
			}
		}

		private float m_PinchReleaseCustom = .5f;
		[Tooltip("Pinch will release when strength < (Pinch Strength - Pinch Release)")]
		[SerializeField]
		[Range(0, .9f)]
		private float m_PinchRelease = .5f;
		public float PinchRelease {
			get { return m_PinchRelease; }
			set {
				m_PinchRelease = value;
				m_PinchReleaseCustom = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			m_PinchStrengthCustom = m_PinchStrength;
			m_PinchReleaseCustom = m_PinchRelease;
		}

		const float kPinchThreshold = .5f;
		private void Validate()
		{
			// Retrieves pinch on/off threshold.
			if (m_UseDefaultPinch && HandManager.Instance != null)
			{
				m_PinchStrength = Mathf.Clamp01(HandManager.Instance.GetPinchThreshold());
				float pinchOffThreshold = Mathf.Clamp01(HandManager.Instance.GetPinchOffThreshold());
				if (m_PinchStrength > pinchOffThreshold) { m_PinchRelease = m_PinchStrength - pinchOffThreshold; }
			}
			else
			{
				m_PinchStrength = m_PinchStrengthCustom;
				m_PinchRelease = m_PinchReleaseCustom;
			}
			// Checks pinch off threshold.
			if (m_PinchStrength <= kPinchThreshold) { m_PinchRelease = 0; }
			else // Pinch On Threshold > 0.5f
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

		[SerializeField]
		private bool m_EnablePinchArea = true;
		public bool EnablePinchArea { get { return m_EnablePinchArea; } set { m_EnablePinchArea = value; } }
		[SerializeField]
		[Range(0, .5f)]
		private float m_LeftInteractive = 0.3f;
		public float LeftInteractive { get { return m_LeftInteractive; } set { m_LeftInteractive = value; } }
		[SerializeField]
		[Range(.5f, 1)]
		private float m_RightInteractive = 0.7f;
		public float RightInteractive { get { return m_RightInteractive; } set { m_RightInteractive = value; } }
		[SerializeField]
		[Range(.5f, 1)]
		private float m_TopInteractive = 0.7f;
		public float TopInteractive { get { return m_TopInteractive; } set { m_TopInteractive = value; } }
		[SerializeField]
		[Range(0, .5f)]
		private float m_BottomInteractive = 0.3f;
		public float BottomInteractive { get { return m_BottomInteractive; } set { m_BottomInteractive = value; } }
		#endregion

		private Vector3 origin = Vector3.zero, direction = Vector3.zero;
		protected override void Update()
		{
			Validate();

			base.Update();

			if (printIntervalLog)
			{
				sb.Clear().Append(m_Hand.Name()).Append(" ").Append("Update() m_UsePose: ").Append(m_UsePose)
					.Append(", m_PinchStrength: ").Append(m_PinchStrength)
					.Append(", m_PinchRelease: ").Append(m_PinchRelease);
				DEBUG(sb);
			}

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
			bool validMotion = (HandManager.Instance.GetHandMotion(m_Hand) != HandManager.HandMotion.None);

			m_Interactable = (m_AlwaysEnable || enabled) && validPose && hasFocus && validMotion;

			if (printIntervalLog)
			{
				sb.Clear().Append(m_Hand.Name()).Append(" ").Append("IsInteractable() enabled: ").Append(enabled)
					.Append(", validPose: ").Append(validPose)
					.Append(", hasFocus: ").Append(hasFocus)
					.Append(", m_AlwaysEnable: ").Append(m_AlwaysEnable);
				DEBUG(sb);
			}

			return m_Interactable;
		}

		private bool IsInteractiveArea()
		{
			if (m_EnablePinchArea)
			{
				Vector3 pixelPosition = Camera.main.WorldToViewportPoint(Pointer.transform.position);
				return pixelPosition.x > m_LeftInteractive && pixelPosition.x < m_RightInteractive &&
					pixelPosition.y > m_BottomInteractive && pixelPosition.y < m_TopInteractive;
			}
			return true;
		}

		bool eligibleForClick = false;
		protected override bool OnDown()
		{
			if (HandManager.Instance == null) { return false; }

			if (!eligibleForClick)
			{
				bool down = m_UseDefaultPinch ? HandManager.Instance.IsHandPinching(m_Hand) : HandManager.Instance.GetPinchStrength(m_Hand) > m_PinchStrength;
				if (down && IsInteractiveArea())
				{
					eligibleForClick = true;
					return true;
				}
			}

			return false;
		}
		protected override bool OnHold()
		{
			bool hold = false;
			if (m_UseDefaultPinch)
			{
				if (eligibleForClick) { hold = HandManager.Instance.IsHandPinching(m_Hand); }
			}
			else
			{
				float pinchStrength = HandManager.Instance.GetPinchStrength(m_Hand);
				hold = pinchStrength > m_PinchStrength;
				if (eligibleForClick) { hold = pinchStrength > (m_PinchStrength - m_PinchRelease); }
			}
			if (!hold)
				eligibleForClick = false;
			return hold;
		}
	}
}
