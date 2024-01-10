// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[DisallowMultipleComponent]
public class CameraRig : MonoBehaviour
{
	// Singleton
	private static CameraRig _instance;
	public static CameraRig Instance => _instance;
		
	const string TAG = "Wave.Essence.WaveRig";

	[SerializeField]
	private GameObject m_CameraOffset = null;
	public GameObject CameraOffset { get { return m_CameraOffset; } set { m_CameraOffset = value; } }

	[SerializeField]
	private GameObject m_CameraObject = null;
	public GameObject CameraObject { get { return m_CameraObject; } set { m_CameraObject = value; } }

	[SerializeField]
	private TrackingOriginModeFlags m_TrackingOrigin = TrackingOriginModeFlags.Device;
	public TrackingOriginModeFlags TrackingOrigin { get { return m_TrackingOrigin; } set { m_TrackingOrigin = value; UpdateTackingOrigin(); } }

	private Vector3 cameraPosOffset = Vector3.zero;
	[SerializeField]
	private float m_CameraYOffset = 1;
	public float CameraYOffset { get { return m_CameraYOffset; } set { m_CameraYOffset = value; } }

	XRInputSubsystem m_InputSystem = null;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Debug.LogWarning("The CameraRig should be a singleton object, and there is already one in the scene.");
			RemoveSelf();
		}
		else
		{
			_instance = this;
		}
	}

	public void RemoveSelf()
	{
		var component = gameObject.GetComponent<CameraRig>();

		if (component != null)
		{
			Destroy(component);
		}
	}

	private IEnumerator Start()
	{
		float timeAcc = 0;

		List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();

		// Try to get input subsystem in 3 seconds.
		while (timeAcc < 3 && m_InputSystem == null)
		{
			yield return null;
			timeAcc += Time.unscaledDeltaTime;
			SubsystemManager.GetInstances(subsystems);
			if (subsystems.Count <= 0)
			{
				continue;
			}

			foreach (XRInputSubsystem subsystem in subsystems)
			{
				if (subsystem.running)
				{
					m_InputSystem = subsystem;
					break;
				}
			}
		}

		if (m_InputSystem == null)
		{
			yield break;
		}

		{
			TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
			if (mode != m_TrackingOrigin)
			{
				m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

				mode = m_InputSystem.GetTrackingOriginMode();
				Debug.Log("Tracking mode is set to " + mode);
			}
			else
			{
				Debug.Log("Tracking mode is already set to " + mode);
			}
		}

		if (m_CameraOffset != null)
		{
			cameraPosOffset.x = m_CameraOffset.transform.localPosition.x;
			cameraPosOffset.z = m_CameraOffset.transform.localPosition.z;

			if (m_TrackingOrigin != TrackingOriginModeFlags.Floor)
				cameraPosOffset.y = m_CameraYOffset;
			else
				cameraPosOffset.y = 0;

			m_CameraOffset.transform.localPosition = cameraPosOffset;
		}
	}

	void UpdateTackingOrigin()
	{
		if (m_InputSystem == null)
			return;

		TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
		if (mode != m_TrackingOrigin)
		{
			m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

			mode = m_InputSystem.GetTrackingOriginMode();
			Debug.Log("Update() Tracking mode is set to " + mode);
		}
	}
}
