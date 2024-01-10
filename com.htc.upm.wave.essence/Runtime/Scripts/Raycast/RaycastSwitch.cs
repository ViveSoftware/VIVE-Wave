// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using UnityEngine;
using Wave.Native;

namespace Wave.Essence.Raycast
{
    [DisallowMultipleComponent]
    public sealed class RaycastSwitch : MonoBehaviour
    {
		const string LOG_TAG = "Wave.Essence.Raycast.RaycastSwitch";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		[Serializable]
		public class GazeSettings
		{
			public bool Enabled = false;
		}
		[SerializeField]
		private GazeSettings m_GazeRaycast = new GazeSettings();
		public GazeSettings GazeRaycast { get { return m_GazeRaycast; } set { m_GazeRaycast = value; } }
		public static GazeSettings Gaze { get { return Instance.GazeRaycast; } }

		[Serializable]
		public class ControllerSettings
		{
			public bool Enabled = true;
		}
		[SerializeField]
		private ControllerSettings m_ControllerRaycast = new ControllerSettings();
		public ControllerSettings ControllerRaycast { get { return m_ControllerRaycast; } set { m_ControllerRaycast = value; } }
		public static ControllerSettings Controller { get { return Instance.ControllerRaycast; } }

		[Serializable]
		public class HandSettings
		{
			public bool Enabled = true;
		}
		[SerializeField]
		private HandSettings m_HandRaycast = new HandSettings();
		public HandSettings HandRaycast { get { return m_HandRaycast; } set { m_HandRaycast = value; } }
		public static HandSettings Hand { get { return Instance.HandRaycast; } }

		private static RaycastSwitch m_Instance = null;
		public static RaycastSwitch Instance
		{
			get
			{
				if (m_Instance == null)
				{
					var rs = new GameObject("RaycastSwitch");
					m_Instance = rs.AddComponent<RaycastSwitch>();
					// This object should survive all scene transitions.
					DontDestroyOnLoad(rs);
				}
				return m_Instance;
			}
		}

		private void Awake()
		{
			m_Instance = this;
		}
		private bool m_Enabled = false;
		private void OnEnable()
		{
			if (!m_Enabled)
			{
				INFO("OnEnable()");
				m_Enabled = true;
			}
		}
		private void OnDisable()
		{
			if (m_Enabled)
			{
				INFO("OnDisable()");
				m_Enabled = false;
			}
		}
		private void Update()
		{
			CheckSettings();

			if (Log.gpl.Print)
			{
				DEBUG("Update() Gaze.Enabled: " + GazeRaycast.Enabled
					+ ", Controller.Enabled: " + ControllerRaycast.Enabled
					+ ", Hand.Enabled: " + HandRaycast.Enabled);
			}
		}
		/// <summary> Updates Gaze, Controller and Hand settings in runtime. </summary>
		private void CheckSettings()
		{
		}
	}
}