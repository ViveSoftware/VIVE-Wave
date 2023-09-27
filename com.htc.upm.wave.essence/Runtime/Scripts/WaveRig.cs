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
using UnityEngine.XR;
using Wave.Native;

namespace Wave.Essence
{
	[DisallowMultipleComponent]
	public sealed class WaveRig : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.WaveRig";
		void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		[SerializeField]
		private GameObject m_CameraOffset = null;
		public GameObject CameraOffset { get { return m_CameraOffset; } set { m_CameraOffset = value; } }

#pragma warning disable
		private Camera m_Camera = null;
#pragma warning enable
		[SerializeField]
		private GameObject m_CameraObject = null;
		public GameObject CameraObject { get { return m_CameraObject; } set { m_CameraObject = value; } }

#pragma warning disable
		private TrackingOriginModeFlags m_TrackingOriginEx = TrackingOriginModeFlags.Device;
#pragma warning enable
		[SerializeField]
		private TrackingOriginModeFlags m_TrackingOrigin = TrackingOriginModeFlags.Device;
		public TrackingOriginModeFlags TrackingOrigin { get { return m_TrackingOrigin; } set { m_TrackingOrigin = value; } }

		private Vector3 cameraPosOffset = Vector3.zero;
		[SerializeField]
		private float m_CameraYOffset = 1;
		public float CameraYOffset { get { return m_CameraYOffset; } set { m_CameraYOffset = value; } }

#if !UNITY_EDITOR && UNITY_ANDROID
		XRInputSubsystem m_InputSystem = null;
		private void Awake()
		{
			if (m_CameraObject != null) { m_Camera = m_CameraObject.GetComponent<Camera>(); }

			List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
			SubsystemManager.GetInstances(subsystems);
			foreach (var subsystem in subsystems)
			{
				if (subsystem.SubsystemDescriptor.id.Equals("WVR Input Provider"))
				{
					m_InputSystem = subsystem;
					break;
				}
			}
			if (m_InputSystem != null)
			{
				m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

				TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
				Log.i(LOG_TAG, "Awake() Tracking mode is set to " + mode, true);
			}
			m_TrackingOriginEx = m_TrackingOrigin;
		}
#endif
		private void Update()
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			if (m_InputSystem != null)
			{
				TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
				if (mode != m_TrackingOrigin || m_TrackingOriginEx != m_TrackingOrigin)
				{
					m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

					mode = m_InputSystem.GetTrackingOriginMode();
					Log.i(LOG_TAG, "Update() Tracking mode is set to " + mode, true);
					m_TrackingOriginEx = m_TrackingOrigin;
				}
			}
#endif

			if (m_CameraOffset != null)
			{
				cameraPosOffset.x = m_CameraOffset.transform.localPosition.x;
				cameraPosOffset.y = m_CameraYOffset;
				cameraPosOffset.z = m_CameraOffset.transform.localPosition.z;

				m_CameraOffset.transform.localPosition = cameraPosOffset;
			}
		}
	}
}