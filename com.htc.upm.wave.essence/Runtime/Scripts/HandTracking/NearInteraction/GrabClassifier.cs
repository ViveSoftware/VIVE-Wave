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
	public class GrabClassifier
	{
		private struct GrabFingerData
		{
			public Vector3 direction;
			public bool isInside;
			public float hysteresisCurl;
			public float prevCurl;
		};

		const string LOG_TAG = "Wave.Essence.Hand.NearInteraction.GrabClassifier";
		void DEBUG(string msg) { Log.d(LOG_TAG, (m_Behavior.IsLeftHand ? "Left" : "Right") + ", " + msg, true); }
		const bool grabLog = false;
		void VERBOSE(string msg)
		{
			if (grabLog) DEBUG(msg);
		}

		#region Properties from GrabBehavior
		private GrabBehavior m_Behavior = null;
		private float m_SphereCollidingRadius = 0.1f;
		private int m_CollidingLayerMask = ~0;
		private QueryTriggerInteraction m_QueryTrigger = QueryTriggerInteraction.Collide;
		#endregion

		/// Colliding candidates of each finger.
		private int[] m_CollidingCandidateCounts = new int[kFingerCount];
		private Collider[][] m_CollidingCandidates = new Collider[kFingerCount][];

		/// Grabbing data.
		private GrabFingerData[] m_GrabFingers = new GrabFingerData[kFingerCount];
		private float grabCoolDown = 0;

		public GrabClassifier(
			GrabBehavior behavior,
			float sphereCollidingRadius,
			int collidingLayerMask,
			QueryTriggerInteraction queryTrigger)
		{
			m_Behavior				= behavior;
			m_SphereCollidingRadius = sphereCollidingRadius;
			m_CollidingLayerMask	= collidingLayerMask;
			m_QueryTrigger			= queryTrigger;

			/// 1. Initialize the colliding candidates.
			for (int i = 0; i < m_CollidingCandidates.Length; i++)
			{
				m_CollidingCandidates[i] = new Collider[kFingerCount];
			}

			/// 2. Initialize the grab fingers data.
			for (int i = 0; i < m_GrabFingers.Length; i++)
			{
				m_GrabFingers[i].isInside = false;
				m_GrabFingers[i].hysteresisCurl = 1f;
				m_GrabFingers[i].prevCurl = 1f;
			}

			grabCoolDown = 0;
		}

		const int kFingerCount = 5;

		#region Colliding Candidates
		/// Physics.OverlapCapsuleNonAlloc() related
		const float kThumbRadius = 0.015f;
		const float kFingerRadius = 0.01f;

		/// <summary> Updates colliding candidates of each finger. </summary>
		private void GetFingersCapsuleColliderResults(
			ref int[] candidateCount,
			ref Collider[][] candidates)
		{
			var hand = m_Behavior.Hand;
			for (int i = 0; i < kFingerCount; i++)
			{
				Vector3 top = hand.fingers[i].tip.position;
				Vector3 bottom = hand.fingers[i].joint1.position;
				candidateCount[i] = GetCapsuleColliderResults(
					point0: top,
					point1: bottom,
					radius: i == 0 ? kThumbRadius : kFingerRadius,
					results: ref candidates[i]
				);
			}
		}

		private int GetCapsuleColliderResults(
			Vector3 point0,
			Vector3 point1,
			float radius,
			ref Collider[] results)
		{
			int overlapCount = 0;

			overlapCount = Physics.OverlapCapsuleNonAlloc(
				point0: point0,
				point1: point1,
				radius: radius,
				results: results
			);

			return overlapCount;
		}

		public static int GetSphereColliderResults(
			Vector3 position,
			ref Collider[] resultsBuffer,
			float sphereCollidingRadius,
			int collidingLayerMask,
			QueryTriggerInteraction queryTrigger)
		{
			int overlapCount = 0;
			while (true)
			{
				overlapCount = Physics.OverlapSphereNonAlloc(position,
															 sphereCollidingRadius,
															 resultsBuffer,
															 collidingLayerMask,
															 queryTrigger);
				if (overlapCount < resultsBuffer.Length)
				{
					break;
				}
				else
				{
					resultsBuffer = new Collider[resultsBuffer.Length * 2];
				}
			}
			return overlapCount;
		}
		#endregion

		#region Grab Classification
		/// Finger curl related
		const float kVelocityMax_GrabCurl = 0.001f, kVelocityMax_Curl = 0f;
		const float kCurlMax = 0f, kCurlMin = -0.02f;
		const float kThumbStickiness = 0.01f;
		const float kFingerStickiness = 0f;

		const float kGrabCooldown = 0.2f;

		private bool m_GrabClassifiedEx = false;
		private bool m_GrabClassified = false;

		/// <summary> Classify grab situations and update grabbing data. </summary>
		private void HandleGrabClassification(
			ref GrabFingerData[] grabFingers,
			HandInteractable actable,
			int[] collidingCandidateCount,
			Collider[][] collidingCandidates)
		{
			var hand = m_Behavior.Hand;

			// 0: Thumb
			grabFingers[0].direction = hand.thumb.direction;
			// 1: Index
			grabFingers[1].direction = hand.index.direction;
			// 2: Middle
			grabFingers[2].direction = hand.middle.direction;
			// 3: Ring
			grabFingers[3].direction = hand.ring.direction;
			// 4: Pinky
			grabFingers[4].direction = hand.pinky.direction;

			/// Determine the finger currCurl and whether the finger is inside.
			for (int finger = 0; finger < grabFingers.Length; finger++)
			{
				/** 
				 * Calculate how extended the finger is.
				 * For the thumb, currCurl = (thumb direction) dot (hand x-direction). The right hand x-direction should be negative.
				 * For other fingers, currCurl = (finger direction) dot (hand direction)
				 * 
				 * Then calculate the currCurl velocity.
				 **/
				float currCurl = Vector3.Dot(
					grabFingers[finger].direction,
					finger != 0 ?
						hand.direction :
						(hand.isLeft ? 1f : -1f) * (hand.rotation * Vector3.right)
				);
				float curlVelocity = currCurl - grabFingers[finger].prevCurl;
				grabFingers[finger].prevCurl = currCurl;

				/**
				 * The collidingCandidates are calculated by Physics.OverlapCapsuleNonAlloc()
				 * To check if current interactable is touched,
				 * we should confirm the collided object and interactable rigidbody.
				 **/
				bool collidingWithObject = false;
				for (int i = 0; i < collidingCandidateCount[finger]; i++)
				{
					if (collidingCandidates[finger][i].attachedRigidbody != null &&
						collidingCandidates[finger][i].attachedRigidbody == actable.rigid)
					{
						collidingWithObject = true;
						break;
					}
				}

				/**
				 * If current interactable is touched and
				 * 1. currCurl less than kCurlMax
				 * 2. currCurl more than kCurlMin
				 * 3. currCurl velocity less than max currCurl velocity
				 * , the interactable will be regarded as "grabbed" by this finger.
				 **/
				float conditionalMaxCurlVelocity = actable.IsGrabbed ? kVelocityMax_GrabCurl : kVelocityMax_Curl;

				VERBOSE("HandleGrabClassification() finger " + finger
					+ ", collidingWithObject: " + collidingWithObject);

				collidingWithObject = collidingWithObject
					&& (currCurl < kCurlMax)
					&& (currCurl > kCurlMin)
					&& (curlVelocity < conditionalMaxCurlVelocity);

				VERBOSE("HandleGrabClassification() finger " + finger
					+ ", collidingWithObject: " + collidingWithObject
					+ ", conditionalMaxCurlVelocity: " + conditionalMaxCurlVelocity
					+ ", kCurlMax: " + kCurlMax
					+ ", kCurlMin: " + kCurlMin
					+ ", currCurl: " + currCurl
					+ ", hysteresisCurl: " + grabFingers[finger].hysteresisCurl
					+ ", curlVelocity: " + curlVelocity
					+ ", is inside: " + grabFingers[finger].isInside);

				/// Colliding with an object means the finger goes inside.
				if (!grabFingers[finger].isInside)
				{
					grabFingers[finger].isInside = collidingWithObject;
					grabFingers[finger].hysteresisCurl = currCurl;// + (finger == 0 ? kThumbStickiness : kFingerStickiness);
				}
				else
				{
					if (currCurl > grabFingers[finger].hysteresisCurl)
					{
						grabFingers[finger].isInside = collidingWithObject;
					}
				}

				VERBOSE("HandleGrabClassification() finger " + finger
					+ ", collidingWithObject: " + collidingWithObject
					+ ", is inside: " + grabFingers[finger].isInside);
			} // for (int finger = 0; finger < grabFingers.Length; finger++)

			/**
			 * If thumb and one other finger is "inside" the object, it's a grab!
			 * This is the trick!
			 **/
			m_GrabClassified =
				(grabFingers[0].isInside &&
				(grabFingers[1].isInside || grabFingers[2].isInside || grabFingers[3].isInside || grabFingers[4].isInside));

			VERBOSE("HandleGrabClassification()"
				+ " m_GrabClassified: " + m_GrabClassified
				+ ", grabCoolDown: " + grabCoolDown);
			/**
			 * If grabbing within 10 frames of releasing, discard grab.
			 * Suppresses spurious regrabs and makes throws work better.
			 **/
			if (grabCoolDown <= kGrabCooldown)
			{
				m_GrabClassified = false;
				grabCoolDown += Time.fixedDeltaTime;
			}

			/// Determine if the object is near the hand or if it's too far away.
			if (m_GrabClassified && !m_GrabClassifiedEx)
			{
				bool nearObject = false;

				Vector3 handCenterPosition = hand.position + (hand.direction * 0.05f) + (hand.normal * 0.01f);
				Collider[] nearHandCandidates = new Collider[kFingerCount];
				int candidateCount =
					GetSphereColliderResults(
						position: handCenterPosition,
						resultsBuffer: ref nearHandCandidates,
						sphereCollidingRadius: m_SphereCollidingRadius,
						collidingLayerMask: m_CollidingLayerMask,
						queryTrigger: m_QueryTrigger
					);

				VERBOSE("HandleGrabClassification() candidateCount: " + candidateCount);

				for (int i = 0; i < candidateCount; i++)
				{
					VERBOSE("HandleGrabClassification()"
						+ " nearHandCandidates[" + i + "]: " + nearHandCandidates[i].gameObject.name
						+ ", actable: " + actable.actable.name);
					if (nearHandCandidates[i].attachedRigidbody != null &&
						nearHandCandidates[i].attachedRigidbody == actable.rigid)
					{
						VERBOSE("HandleGrabClassification() MATCH!! The candidate is near.");
						nearObject = true;
						break;
					}
				}

				if (!nearObject)
				{
					m_GrabClassified = false;
					grabFingers[0].isInside = false;
				}
			}

		} // void HandleGrabClassification()
		#endregion

		public void FixedUpdateGrabState()
		{
			/// Checks hand grab candidates.
			GetFingersCapsuleColliderResults(
				candidateCount: ref m_CollidingCandidateCounts,
				candidates: ref m_CollidingCandidates);
		}

		private enum GrabMode
		{
			BeginGrab,
			ReleaseGrab,
		}
		private bool FixedUpdateGrabStateChange(HandInteractable actable, GrabMode mode)
		{
			HandleGrabClassification(
				grabFingers: ref m_GrabFingers,
				actable: actable,
				collidingCandidateCount: m_CollidingCandidateCounts,
				collidingCandidates: m_CollidingCandidates);

			bool stateChanged = false;

			/**
			 * We only update m_GrabClassifiedEx here to
			 * prevent updating m_GrabClassifiedEx every frame.
			 * 
			 * BTW, we can change m_GrabClassifiedEx in other function for a special purpose,
			 * e.g. force release.
			 **/
			if (!m_GrabClassifiedEx && m_GrabClassified
				&& mode == GrabMode.BeginGrab)
			{
				stateChanged = true;
				// Updates grab status only when state changes.
				m_GrabClassifiedEx = m_GrabClassified;
			}
			else
			if (m_GrabClassifiedEx && !m_GrabClassified
				&& mode == GrabMode.ReleaseGrab && m_Behavior.GrabbedObject == actable)
			{
				stateChanged = true;
				// Updates grab status only when state changes.
				m_GrabClassifiedEx = m_GrabClassified;

				grabCoolDown = 0f;
			}

			return stateChanged;
		}

		/// <summary> Called in FixedUpdate() to check if grasp is beginning. </summary>
		public bool FixedUpdateGrabBegin(out HandInteractable grabbedObject)
		{
			grabbedObject = null;

			// Already grabbing or lost tracking.
			if (m_Behavior.isGrabbing || !m_Behavior.isTracked) { return false; }

			foreach (var actable in m_Behavior.GrabCandidates)
			{
				if (FixedUpdateGrabStateChange(actable, GrabMode.BeginGrab))
				{
					grabbedObject = actable;
					DEBUG("FixedUpdateGrabBegin() grabbedObject: " + grabbedObject.actable.name);
					return true;
				}
			}

			return false;
		}

		/// <summary> Called in FixedUpdate() to check if grasp is releasing. </summary>
		public bool FixedUpdateGrabEnd(out HandInteractable releasedObject)
		{
			releasedObject = null;

			// Nothing to release.
			if (!m_Behavior.isGrabbing) { return false; }

			if (FixedUpdateGrabStateChange(m_Behavior.GrabbedObject, GrabMode.ReleaseGrab))
			{
				releasedObject = m_Behavior.GrabbedObject;
				DEBUG("FixedUpdateGrabEnd() releasedObject: " + releasedObject.actable.name);
				return true;
			}

			return false;
		}
	}
}
