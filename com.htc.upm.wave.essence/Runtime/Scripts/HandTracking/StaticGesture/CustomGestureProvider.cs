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
using UnityEngine;
using UnityEngine.Events;
using Wave.Native;

namespace Wave.Essence.Hand.StaticGesture
{
	public class CustomGestureProvider : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Hand.StaticGesture.CustomGestureProvider";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		#region Inspector
		public List<BaseSingleHandGestureProducer> SingleHandCustomGestures = new List<BaseSingleHandGestureProducer>();
		public List<BaseDualHandGestureProducer> DualHandCustomGestures = new List<BaseDualHandGestureProducer>();

		[System.Serializable]
		public class GestureEvent : UnityEvent<GestureType> { }

		[Tooltip("Callback when the left gesture changes.")]
		public GestureEvent OnLeftGesture = new GestureEvent();

		[Tooltip("Callback when the right gesture changes.")]
		public GestureEvent OnRightGesture = new GestureEvent();
		#endregion

		// Returns the current singleton (or null if no instance exists).
		private static CustomGestureProvider m_Instance = null;
		public static CustomGestureProvider Current { get { return m_Instance; } }

		private HandState m_LeftHandState = new HandState();
		public HandState LeftHandState { get { return m_LeftHandState; } }

		private HandState m_RightHandState = new HandState();
		public HandState RightHandState { get { return m_RightHandState; } }

		/// Static gestures will be defined in StaticGestureCalculator ctor.
		private StaticGestureCalculator m_Calculator = new StaticGestureCalculator();

		private GestureHandData m_LeftHand;
		public GestureHandData LeftHand { get { return m_LeftHand; } }

		private GestureHandData m_RightHand;
		public GestureHandData RightHand { get { return m_RightHand; } }

		private GestureType m_LeftGesture = GestureType.Unknown, m_RightGesture = GestureType.Unknown;

		void Awake()
		{
			if (Current != null)
			{
				Debug.LogWarning("Only one CustomGestureProvider is allowed in the scene.");
				GameObject.Destroy(this);
				return;
			}
			m_Instance = this;

			m_LeftHand.Init(true);
			m_RightHand.Init(false);
			DEBUG("Awake() m_LeftHand: " + m_LeftHand.points.Length + ", m_RightHand: " + m_RightHand.points.Length);

			OnLeftGesture.AddListener(LeftGestureHandler);
			OnRightGesture.AddListener(RightGestureHandler);

			if (SingleHandCustomGestures == null) { SingleHandCustomGestures = new List<BaseSingleHandGestureProducer>(); }
			if (DualHandCustomGestures == null) { DualHandCustomGestures = new List<BaseDualHandGestureProducer>(); }
		}
		void Update()
		{
			if (m_Calculator == null) { m_Calculator = new StaticGestureCalculator(); }

			UpdateGesture(ref m_LeftHand, ref m_LeftHandState);
			if (m_LeftGesture != m_LeftHand.gesture)
			{
				m_LeftGesture = m_LeftHand.gesture;
				OnLeftGesture.Invoke(m_LeftGesture);
			}

			UpdateGesture(ref m_RightHand, ref m_RightHandState);
			if (m_RightGesture != m_RightHand.gesture)
			{
				m_RightGesture = m_RightHand.gesture;
				OnRightGesture.Invoke(m_RightGesture);
			}

			if (SingleHandCustomGestures != null && SingleHandCustomGestures.Count > 0)
			{
				for (int i = 0; i < SingleHandCustomGestures.Count; i++)
					SingleHandCustomGestures[i].CheckGesture();
			}
			if (DualHandCustomGestures != null && DualHandCustomGestures.Count > 0)
			{
				for (int i = 0; i < DualHandCustomGestures.Count; i++)
					DualHandCustomGestures[i].CheckGesture();
			}
		}

		private void UpdateGesture(ref GestureHandData hand, ref HandState state)
		{
			WXRGestureHand.UpdatePoints(ref hand);
			SetHandState(hand, ref state);
			m_Calculator.CalculateHandGesture(ref hand, state);
		}
		private static void SetHandState(GestureHandData hand, ref HandState state)
		{
			if (!hand.isTracked) { return; }

			state.thumb = GetThumbState(
				hand.points[GesturePoint.Thumb_Joint1.Index()]	, hand.points[GesturePoint.Thumb_Joint2.Index()]	, hand.points[GesturePoint.Thumb_Tip.Index()]);
			state.index = GetFingerState(
				hand.points[GesturePoint.Index_Joint1.Index()]	, hand.points[GesturePoint.Index_Joint2.Index()]	, hand.points[GesturePoint.Index_Tip.Index()]);
			state.middle = GetFingerState(
				hand.points[GesturePoint.Middle_Joint1.Index()]	, hand.points[GesturePoint.Middle_Joint2.Index()]	, hand.points[GesturePoint.Middle_Tip.Index()]);
			state.ring = GetFingerState(
				hand.points[GesturePoint.Ring_Joint1.Index()]	, hand.points[GesturePoint.Ring_Joint2.Index()]		, hand.points[GesturePoint.Ring_Tip.Index()]);
			state.pinky = GetFingerState(
				hand.points[GesturePoint.Pinky_Joint1.Index()]	, hand.points[GesturePoint.Pinky_Joint2.Index()]	, hand.points[GesturePoint.Pinky_Tip.Index()]);
		}
		private static ThumbState GetThumbState(Vector3 root, Vector3 node1, Vector3 top)
		{
			var angle = Vector3.Angle(node1 - root, top - node1);
			return angle < 15 ? ThumbState.Open : ThumbState.Close;
		}
		private static FingerState GetFingerState(Vector3 root, Vector3 node1, Vector3 top)
		{
			var angle = Vector3.Angle(node1 - root, top - node1);
			if (angle < 25) return FingerState.Open;
			if (angle > 75) return FingerState.Close;
			return FingerState.Relax;
		}

		#region Public Interface
		/// <summary> Retrieves SDK pre-defined left or right hand gesture types. </summary>
		public GestureType GetHandGesture(bool isLeftHand)
		{
			return (isLeftHand ? m_LeftHand.gesture : m_RightHand.gesture);
		}
		/// <summary> Retrieves SDK or custom defined left or right hand gesture types. </summary>
		public string GetCustomGesture(bool isLeftHand)
		{
			GestureType gesture = GetHandGesture(isLeftHand);
			if (gesture != GestureType.Unknown) { return gesture.ToString(); }

			if (SingleHandCustomGestures != null && SingleHandCustomGestures.Count > 0)
			{
				for (int i = 0; i < SingleHandCustomGestures.Count; i++)
				{
					if ((isLeftHand && SingleHandCustomGestures[i].IsLeftMatch) || (!isLeftHand && SingleHandCustomGestures[i].IsRightMatch))
					{
						return SingleHandCustomGestures[i].name;
					}
				}
			}

			return GestureType.Unknown.ToString();
		}
		/// <summary> Retrieves custom defined dual hand gesture types. </summary>
		public string GetDualHandGesture()
		{
			if (DualHandCustomGestures != null && DualHandCustomGestures.Count > 0)
			{
				for (int i = 0; i < DualHandCustomGestures.Count; i++)
				{
					if (DualHandCustomGestures[i].IsMatch) { return DualHandCustomGestures[i].name; }
				}
			}

			return GestureType.Unknown.ToString();
		}
		#endregion

		private void LeftGestureHandler(GestureType gesture)
		{
			DEBUG("Left gesture: " + gesture);
		}
		private void RightGestureHandler(GestureType gesture)
		{
			DEBUG("Right gesture: " + gesture);
		}
	}
}
