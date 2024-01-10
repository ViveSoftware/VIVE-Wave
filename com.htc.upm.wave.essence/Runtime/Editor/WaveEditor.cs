// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEditor;
using UnityEngine;
using Wave.Essence.Events;
using Wave.Native;

#if UNITY_EDITOR
namespace Wave.Essence.Editor
{
	public class WaveEditor : MonoBehaviour
	{
		private static string LOG_TAG = "WaveEditor";
		private void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		private void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		private static WaveEditor m_Instance = null;
		public static WaveEditor Instance
		{
			get
			{
				AssertInstance();
				return m_Instance;
			}
		}
		public static void AssertInstance()
		{
			if (m_Instance == null)
			{
				var gameObject = new GameObject("WaveEditor");
				m_Instance = gameObject.AddComponent<WaveEditor>();
				// This object should survive all scene transitions.
				DontDestroyOnLoad(m_Instance);
			}
		}

		void Awake()
		{
			m_Instance = this;
		}

		private WVR_Event_t wvrEvent = new WVR_Event_t();
		void Update()
		{
			bool EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			if (!EnableDirectPreview)
			{
				if (Interop.WVR_PollEventQueue(ref wvrEvent))
					SystemEvent.Send(wvrEvent);
			}
		}
	}
}
#endif
