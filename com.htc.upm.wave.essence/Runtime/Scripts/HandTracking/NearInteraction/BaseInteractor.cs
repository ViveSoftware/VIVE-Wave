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
using Wave.Native;

namespace Wave.Essence.Hand.NearInteraction
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public abstract class BaseInteractor : MonoBehaviour, IInteractor
	{
		const string LOG_TAG = "Wave.Essence.Hand.NearInteraction.BaseInteractor";
		void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		#region IInteractor
		private GameObject m_actor = null;
		public GameObject actor { get { return m_actor; } }

		private Transform m_Transform = null;
		public Transform trans { get { return m_Transform; } }

		private Rigidbody m_rigid = null;
		public Rigidbody rigid { get { return m_rigid; } }
		#endregion

		#region Properties
		private Dictionary<BaseInteractable, float> s_TouchCandidates = new Dictionary<BaseInteractable, float>();
		/// <summary> The interactables those are touched by this interactor. </summary>
		public List<BaseInteractable> TouchCandidates {
			get
			{
				List<BaseInteractable> actables = new List<BaseInteractable>();
				foreach (var actable in s_TouchCandidates)
					actables.Add(actable.Key);
				return actables;
			}
		}
		/// <summary> The interactor is touching interactable(s). </summary>
		public bool IsTouching { get { return TouchCandidates.Count > 0; } }
		#endregion

		protected virtual void OnEnable()
		{
			m_actor = gameObject;
			m_Transform = transform;
			m_rigid = GetComponent<Rigidbody>();
			if (m_rigid != null)
			{
				INFO("OnEnable() add interactor " + gameObject.name);
				InteractionHub.AddInteractor(m_rigid, this);

				m_rigid.useGravity = false;
			}
		}
		protected virtual void OnDisable()
		{
			if (m_rigid != null)
			{
				INFO("OnDisable() remove interactor " + gameObject.name);
				InteractionHub.RemoveInteractor(m_rigid);
			}
		}

		public void NotifyCollisionEnter(BaseInteractable actable)
		{
			if (actable == null) { return; }
			if (!s_TouchCandidates.ContainsKey(actable))
			{
				INFO("NotifyCollisionEnter() " + actable.actable.name);
				s_TouchCandidates.Add(actable, Time.frameCount);
			}
		}
		public void NotifyCollisionExit(BaseInteractable actable)
		{
			if (actable == null) { return; }
			if (s_TouchCandidates.ContainsKey(actable))
			{
				INFO("NotifyCollisionExit() " + actable.actable.name);
				s_TouchCandidates.Remove(actable);
			}
		}
	}
}