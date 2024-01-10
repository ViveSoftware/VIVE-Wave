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

namespace Wave.Essence.Hand.NearInteraction
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public abstract class BaseInteractable : MonoBehaviour, IInteractable
	{
		const string LOG_TAG = "Wave.Essence.Hand.NearInteraction.BaseInteractable";
		void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		#region IInteractable
		private GameObject m_actable = null;
		public GameObject actable { get { return m_actable; } }

		private Transform m_Transform = null;
		public Transform trans { get { return m_Transform; } }

		private Rigidbody m_rigid = null;
		public Rigidbody rigid { get { return m_rigid; } }
		#endregion

		/// <summary>
		/// Called when this interactable begins colliding with any interactors.
		/// This interactable was not colliding with any interactors last frame.
		/// </summary>
		public UnityEvent OnTouchBegin;
		/// <summary>
		/// Called when this interactable ceases colliding with any interactors.
		/// This interactable was colliding with interactors last frame.
		/// </summary>
		public UnityEvent OnTouchEnd;
		/// <summary>
		/// Called every frame during which one or more interactors is colliding with this interactable.
		/// </summary>
		public UnityEvent OnTouching;

		#region Properties
		/// The interactors those are touching this interactable.
		private Dictionary<BaseInteractor, float> s_TouchCandidates = new Dictionary<BaseInteractor, float>();
		public List<BaseInteractor> TouchCandidates {
			get
			{
				List<BaseInteractor> actors = new List<BaseInteractor>();
				foreach (var actor in s_TouchCandidates)
					actors.Add(actor.Key);
				return actors;
			}
		}
		/// <summary> The interactable is under touching. </summary>
		public bool IsTouching { get { return TouchCandidates.Count > 0; } }
		#endregion

		protected virtual void OnEnable()
		{
			m_actable = gameObject;
			m_Transform = transform;
			m_rigid = GetComponent<Rigidbody>();
			if (m_rigid != null)
			{
				INFO("OnEnable() add interactable " + gameObject.name);
				InteractionHub.AddInteractable(m_rigid, this);
			}

		}
		protected virtual void OnDisable()
		{
			if (m_rigid != null)
			{
				INFO("OnDisable() remove interactable " + gameObject.name);
				InteractionHub.RemoveInteractable(m_rigid);
			}
		}
		protected virtual void Update()
		{
		}

		void AddTouchCandidates(Rigidbody rigid)
		{
			if (rigid == null) { return; }

			if (InteractionHub.GetInteractor(rigid, out BaseInteractor value))
			{
				if (!s_TouchCandidates.ContainsKey(value))
					s_TouchCandidates.Add(value, Time.frameCount);

				value.NotifyCollisionEnter(this);
			}

			OnTouchBegin.Invoke();
		}
		void RemoveTouchCandidates(Rigidbody rigid)
		{
			if (rigid == null) { return; }

			if (InteractionHub.GetInteractor(rigid, out BaseInteractor value))
			{
				if (s_TouchCandidates.ContainsKey(value))
					s_TouchCandidates.Remove(value);

				value.NotifyCollisionExit(this);
			}

			OnTouchEnd.Invoke();
		}

		void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody == null) { return; }

			INFO("OnCollisionEnter() " + collision.gameObject.name);

			AddTouchCandidates(collision.rigidbody);
		}
		void OnCollisionExit(Collision collision)
		{
			if (collision.rigidbody == null) { return; }

			INFO("OnCollisionExit() " + collision.gameObject.name);

			RemoveTouchCandidates(collision.rigidbody);
		}

		void OnTriggerEnter(Collider collider)
		{
			if (collider.attachedRigidbody == null) { return; }

			INFO("OnTriggerEnter() " + collider.gameObject.name);

			AddTouchCandidates(collider.attachedRigidbody);
		}
		void OnTriggerExit(Collider collider)
		{
			if (collider.attachedRigidbody == null) { return; }

			INFO("OnTriggerExit() " + collider.gameObject.name);

			RemoveTouchCandidates(collider.attachedRigidbody);
		}
	}
}