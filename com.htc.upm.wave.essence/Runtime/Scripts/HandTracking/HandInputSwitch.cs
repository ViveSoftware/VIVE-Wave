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

namespace Wave.Essence.Hand
{
	[DisallowMultipleComponent]
	public sealed class HandInputSwitch : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Hand.HandInputSwitch";
		private void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }

		#region Inspector
		[Tooltip("True for only one input from one hand. False for double inputs from double hands.")]
		[SerializeField]
		private bool m_SingleInput = true;
		public bool SingleInput { get { return m_SingleInput; } set { m_SingleInput = value; } }

		[Tooltip("If SingleInput is set, PrimaryInput will be used to decide the main input hand.")]
		[SerializeField]
		private HandManager.HandType m_PrimaryInput = HandManager.HandType.Right;
		public HandManager.HandType PrimaryInput { get { return m_PrimaryInput; } set { m_PrimaryInput = value; } }
		#endregion

		private static HandInputSwitch m_Instance = null;
		public static HandInputSwitch Instance
		{
			get
			{
				if (m_Instance == null)
				{
					var gameObject = new GameObject("HandInputSwitch");
					m_Instance = gameObject.AddComponent<HandInputSwitch>();
					// This object should survive all scene transitions.
					DontDestroyOnLoad(m_Instance);
				}
				return m_Instance;
			}
		}

		private float m_PinchStrength = .7f;
		public float PinchStrength { get { return m_PinchStrength; } set { m_PinchStrength = value; } }

		bool pinchedEx = false;
		private bool IsPinched(HandManager.HandType hand)
		{
			if (HandManager.Instance == null) { return false; }

			bool pinched =
				(HandManager.Instance.GetHandMotion(hand) == HandManager.HandMotion.Pinch) &&
				(HandManager.Instance.GetPinchStrength(hand) > m_PinchStrength);

			if (pinchedEx != pinched)
			{
				pinchedEx = pinched;
				if (pinched) { return true; }
			}

			return false;
		}

		private void SetFocusHand()
		{
			if ((m_PrimaryInput == HandManager.HandType.Right) && IsPinched(HandManager.HandType.Left))
			{
				m_PrimaryInput = HandManager.HandType.Left;
				DEBUG("The focus hand is set to " + m_PrimaryInput);
				Interop.WVR_SetFocusedController(WVR_DeviceType.WVR_DeviceType_Controller_Left);
			}
			if ((m_PrimaryInput == HandManager.HandType.Left) && IsPinched(HandManager.HandType.Right))
			{
				m_PrimaryInput = HandManager.HandType.Right;
				DEBUG("The focus hand is set to " + m_PrimaryInput);
				Interop.WVR_SetFocusedController(WVR_DeviceType.WVR_DeviceType_Controller_Right);
			}
		}

		private void GetFocusHand()
		{
			WVR_DeviceType focus_dev = Interop.WVR_GetFocusedController();
			if (focus_dev == WVR_DeviceType.WVR_DeviceType_Controller_Right)
				m_PrimaryInput = HandManager.HandType.Right;
			if (focus_dev == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				m_PrimaryInput = HandManager.HandType.Left;
			DEBUG("GetFocusHand() Focus hand: " + m_PrimaryInput);
		}

		#region Monobehaviour overrides
		private void Awake()
		{
			m_Instance = this;
		}
		void Start()
		{
			Log.i(LOG_TAG, "Start() Check the focus hand .");
			GetFocusHand();
		}
		void OnApplicationPause(bool pauseStatus)
		{
			Log.i(LOG_TAG, "OnApplicationPause() pauseStatus: " + pauseStatus, true);
			if (!pauseStatus)
			{
				Log.i(LOG_TAG, "OnApplicationPause() Check the focus hand in resume.");
				GetFocusHand();
			}
		}

		private bool hasSystemFocus = true;
		void Update()
		{
			if (hasSystemFocus != ClientInterface.IsFocused)
			{
				hasSystemFocus = ClientInterface.IsFocused;
				DEBUG("Update() " + (hasSystemFocus ? "Gets system focus." : "Focus is captured by system."));
				if (hasSystemFocus) { GetFocusHand(); }
			}
			SetFocusHand();
		}
		#endregion
	}
}