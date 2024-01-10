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
using UnityEngine.XR;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using Wave.Native;
using Wave.Essence.Events;
using Wave.XR.Function;
using Wave.XR.Settings;
using UnityEngine.Profiling;

using Wave.OpenXR;
using System.Text;

namespace Wave.Essence.Hand
{
	[DisallowMultipleComponent]
	public sealed class HandManager : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Hand.HandManager";
		private StringBuilder m_sb = null;
		internal StringBuilder sb {
			get {
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		private void DEBUG(StringBuilder msg) { Log.d(LOG_TAG, msg, true); }
		bool printIntervalLog = false;
		int logFrame = 0;
		private void INFO(StringBuilder msg) { Log.i(LOG_TAG, msg, true); }
		private void WARNING(StringBuilder msg) { Log.w(LOG_TAG, msg, true); }

		#region Global Declaration
		public static readonly string HAND_STATIC_GESTURE = "HAND_STATIC_GESTURE";
		public static readonly string HAND_DYNAMIC_GESTURE_LEFT = "HAND_DYNAMIC_GESTURE_LEFT";
		public static readonly string HAND_DYNAMIC_GESTURE_RIGHT = "HAND_DYNAMIC_GESTURE_RIGHT";
		public static readonly string HAND_GESTURE_STATUS = "HAND_GESTURE_STATUS";
		public static readonly string HAND_TRACKER_STATUS = "HAND_TRACKER_STATUS";

		public enum HandType
		{
			Right = 0,
			Left = 1
		};
		public enum HandJoint
		{
			Palm = WVR_HandJoint.WVR_HandJoint_Palm,
			Wrist = WVR_HandJoint.WVR_HandJoint_Wrist,
			Thumb_Joint0 = WVR_HandJoint.WVR_HandJoint_Thumb_Joint0,
			Thumb_Joint1 = WVR_HandJoint.WVR_HandJoint_Thumb_Joint1,
			Thumb_Joint2 = WVR_HandJoint.WVR_HandJoint_Thumb_Joint2,
			Thumb_Tip = WVR_HandJoint.WVR_HandJoint_Thumb_Tip,
			Index_Joint0 = WVR_HandJoint.WVR_HandJoint_Index_Joint0,
			Index_Joint1 = WVR_HandJoint.WVR_HandJoint_Index_Joint1,
			Index_Joint2 = WVR_HandJoint.WVR_HandJoint_Index_Joint2,
			Index_Joint3 = WVR_HandJoint.WVR_HandJoint_Index_Joint3,
			Index_Tip = WVR_HandJoint.WVR_HandJoint_Index_Tip,
			Middle_Joint0 = WVR_HandJoint.WVR_HandJoint_Middle_Joint0,
			Middle_Joint1 = WVR_HandJoint.WVR_HandJoint_Middle_Joint1,
			Middle_Joint2 = WVR_HandJoint.WVR_HandJoint_Middle_Joint2,
			Middle_Joint3 = WVR_HandJoint.WVR_HandJoint_Middle_Joint3,
			Middle_Tip = WVR_HandJoint.WVR_HandJoint_Middle_Tip,
			Ring_Joint0 = WVR_HandJoint.WVR_HandJoint_Ring_Joint0,
			Ring_Joint1 = WVR_HandJoint.WVR_HandJoint_Ring_Joint1,
			Ring_Joint2 = WVR_HandJoint.WVR_HandJoint_Ring_Joint2,
			Ring_Joint3 = WVR_HandJoint.WVR_HandJoint_Ring_Joint3,
			Ring_Tip = WVR_HandJoint.WVR_HandJoint_Ring_Tip,
			Pinky_Joint0 = WVR_HandJoint.WVR_HandJoint_Pinky_Joint0,
			Pinky_Joint1 = WVR_HandJoint.WVR_HandJoint_Pinky_Joint1,
			Pinky_Joint2 = WVR_HandJoint.WVR_HandJoint_Pinky_Joint2,
			Pinky_Joint3 = WVR_HandJoint.WVR_HandJoint_Pinky_Joint3,
			Pinky_Tip = WVR_HandJoint.WVR_HandJoint_Pinky_Tip,
		};
		public enum HandModel
		{
			WithoutController = WVR_HandModelType.WVR_HandModelType_WithoutController,
			WithController = WVR_HandModelType.WVR_HandModelType_WithController,
		}
		public enum HandMotion
		{
			None = WVR_HandPoseType.WVR_HandPoseType_Invalid,
			Pinch = WVR_HandPoseType.WVR_HandPoseType_Pinch,
			Hold = WVR_HandPoseType.WVR_HandPoseType_Hold,
		}
		public enum HandHoldRole
		{
			None = WVR_HandHoldRoleType.WVR_HandHoldRoleType_None,
			Main = WVR_HandHoldRoleType.WVR_HandHoldRoleType_MainHold,
			Side = WVR_HandHoldRoleType.WVR_HandHoldRoleType_SideHold,
		}
		public enum HandHoldType
		{
			None = WVR_HandHoldObjectType.WVR_HandHoldObjectType_None,
			Gun = WVR_HandHoldObjectType.WVR_HandHoldObjectType_Gun,
			OCSpray = WVR_HandHoldObjectType.WVR_HandHoldObjectType_OCSpray,
			LongGun = WVR_HandHoldObjectType.WVR_HandHoldObjectType_LongGun,
			Baton = WVR_HandHoldObjectType.WVR_HandHoldObjectType_Baton,
			FlashLight = WVR_HandHoldObjectType.WVR_HandHoldObjectType_FlashLight,
		}

		public delegate void HandGestureResultDelegate(object sender, bool result);
		public enum GestureStatus
		{
			// Initial, can call Start API in this state.
			NotStart,
			StartFailure,

			// Processing, should NOT call API in this state.
			Starting,
			Stopping,

			// Running, can call Stop API in this state.
			Available,

			// Do nothing.
			NoSupport
		}
		public enum GestureType
		{
			Invalid = WVR_HandGestureType.WVR_HandGestureType_Invalid,
			Unknown = WVR_HandGestureType.WVR_HandGestureType_Unknown,
			Fist = WVR_HandGestureType.WVR_HandGestureType_Fist,
			Five = WVR_HandGestureType.WVR_HandGestureType_Five,
			OK = WVR_HandGestureType.WVR_HandGestureType_OK,
			ThumbUp = WVR_HandGestureType.WVR_HandGestureType_ThumbUp,
			IndexUp = WVR_HandGestureType.WVR_HandGestureType_IndexUp,
			Palm_Pinch = WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch,
			Yeah = WVR_HandGestureType.WVR_HandGestureType_Yeah,
		}
		[Serializable]
		public class GestureSetter
		{
			private bool Unknown = true;
			public bool Fist = false;
			public bool Five = false;
			public bool OK = false;
			public bool ThumbUp = false;
			public bool IndexUp = false;
			public bool Palm_Pinch = false;
			public bool Yeah = false;

			public ulong optionValue { get; private set; }
			public void UpdateOptionValue()
			{
				optionValue = (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Invalid;
				if (Unknown)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Unknown;
				if (Fist)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Fist;
				if (Five)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Five;
				if (OK)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_OK;
				if (ThumbUp)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_ThumbUp;
				if (IndexUp)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_IndexUp;
				if (Palm_Pinch)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch;
				if (Yeah)
					optionValue |= (ulong)1 << (int)WVR_HandGestureType.WVR_HandGestureType_Yeah;
			}
		}
		[Serializable]
		public class GestureOption
		{
			[HideInInspector]
			[Obsolete("This variable is deprecated. Use StartHandGesture() or StopHandGesture() to enable/disable the Hand Gesture component.")]
			public bool Enable = false;
			public bool InitialStart = false;
			public GestureSetter Gesture = new GestureSetter();
		}

		public delegate void HandTrackerResultDelegate(object sender, bool result);
		public enum TrackerStatus
		{
			// Initial, can call Start API in this state.
			NotStart,
			StartFailure,

			// Processing, should NOT call API in this state.
			Starting,
			Stopping,

			// Running, can call Stop API in this state.
			Available,

			// Do nothing.
			NoSupport
		}
		public enum TrackerType
		{
			Natural = WVR_HandTrackerType.WVR_HandTrackerType_Natural,
			Electronic = WVR_HandTrackerType.WVR_HandTrackerType_Electronic,
		}
		public enum TrackerSelector
		{
			[Tooltip("Uses electronic hand first.")]
			ElectronicPrior,
			[Tooltip("Uses natural hand first.")]
			NaturalPrior,
		}
		[Serializable]
		public class NaturalTrackerOption
		{
			public bool InitialStart = false;
		}
		[Serializable]
		public class ElectronicTrackerOption
		{
			public bool InitialStart = false;
			[Tooltip("Selects the hand model: with or without a controller stick.")]
			public HandModel Model = HandModel.WithoutController;
		}
		[Serializable]
		public class TrackerOption
		{
			[Tooltip("Selects the prefer tracker.")]
			public TrackerSelector Tracker = TrackerSelector.NaturalPrior;
			public NaturalTrackerOption Natural = new NaturalTrackerOption();
			public ElectronicTrackerOption Electronic = new ElectronicTrackerOption();
		}
		#endregion

		#region Inspector
		[Tooltip("Selects supported gesture types.")]
		[SerializeField]
		private GestureOption m_GestureOptions = new GestureOption();
		public GestureOption GestureOptions { get { return m_GestureOptions; } set { m_GestureOptions = value; } }

		[SerializeField]
		private TrackerOption m_TrackerOptions = new TrackerOption();
		public TrackerOption TrackerOptions { get { return m_TrackerOptions; } set { m_TrackerOptions = value; } }
		#endregion

		private static HandManager m_Instance = null;
		public static HandManager Instance { get { return m_Instance; } }
		[Obsolete("Do NOT use! Use HandInputSwitch.Instance.PrimaryInput instead.")]
		public static HandType PrimaryInput = HandType.Right;

		private XR_InteractionMode interactionMode = XR_InteractionMode.Default;

		#region MonoBehaviour Overrides
		private ulong supportedFeature = 0;
		private ulong m_GestureOptionValue = 0;

		private delegate void ConvertHandTrackingDataToUnityDelegate(ref WVR_HandJointData_t src, ref HandJointData26 dest);

		private ConvertHandTrackingDataToUnityDelegate ConvertHandTrackingDataToUnity = null;

		void Awake()
		{
			sb.Clear().Append("Awake()"); DEBUG(sb);
			if (m_Instance == null)
			{
				m_Instance = this;
				// TODO Cant use DontDestroyOnLoad to an object which is not put in transform root.
				// TODO how to kill self?
				// TODO how to spawn when access?
				DontDestroyOnLoad(m_Instance);
			}

			m_WaveXRSettings = WaveXRSettings.GetInstance();

			supportedFeature = Interop.WVR_GetSupportedFeatures();
			// 1. Checks if Hand Gesture is supported.
			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_HandGesture) == 0)
			{
				Log.w(LOG_TAG, "WVR_SupportedFeature_HandGesture is not enabled.", true);
				SetHandGestureStatus(GestureStatus.NoSupport);
			}
			// 2. Checks if Natural Hand is supported.
			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_HandTracking) == 0)
			{
				Log.w(LOG_TAG, "Awake() WVR_SupportedFeature_HandTracking is not enabled.", true);
				SetHandTrackerStatus(TrackerType.Natural, TrackerStatus.NoSupport);
			}
			// 3. Checks if WVR_SupportedFeature electronic hand is supported.
			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_ElectronicHand) == 0)
			{
				Log.w(LOG_TAG, "Awake() WVR_SupportedFeature_ElectronicHand is not enabled.", true);
				SetHandTrackerStatus(TrackerType.Electronic, TrackerStatus.NoSupport);
			}

			interactionMode = ClientInterface.InteractionMode;

			m_GestureOptions.Gesture.UpdateOptionValue();
			m_GestureOptionValue = m_GestureOptions.Gesture.optionValue;

			if (HandInputSwitch.Instance != null)
				Log.i(LOG_TAG, "Awake() Loaded HandInputSwitch.", true);
		}
		void Start()
		{
			if (m_GestureOptions.InitialStart)
			{
				sb.Clear().Append("Start() Starts hand gesture."); DEBUG(sb);
				StartHandGesture();
			}

			if (m_TrackerOptions.Natural.InitialStart)
			{
				sb.Clear().Append("Start() Starts the natural hand tracker."); DEBUG(sb);
				StartHandTracker(TrackerType.Natural);
			}
			if (m_TrackerOptions.Electronic.InitialStart)
			{
				sb.Clear().Append("Start() Starts the electronic hand tracker."); DEBUG(sb);
				StartHandTracker(TrackerType.Electronic);
			}

			sb.Clear().Append("Start() CheckWristPositionFusion"); DEBUG(sb);
			CheckWristPositionFusion();

			var ptr = FunctionsHelper.GetFuncPtr("ConvertHandTrackingDataToUnity");
			if (ptr != IntPtr.Zero)
				ConvertHandTrackingDataToUnity = Marshal.GetDelegateForFunctionPointer<ConvertHandTrackingDataToUnityDelegate>(ptr);
			else
				ConvertHandTrackingDataToUnity = null;
		}
		void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			interactionMode = ClientInterface.InteractionMode;

			/// ------------- Gesture -------------
            Profiler.BeginSample("Gesture");
			// Restart the Hand Gesture component when gesture options change.
			m_GestureOptions.Gesture.UpdateOptionValue();
			if (m_GestureOptionValue != m_GestureOptions.Gesture.optionValue)
			{
				m_GestureOptionValue = m_GestureOptions.Gesture.optionValue;
				RestartHandGesture();
			}
			else
			{
				GetHandGestureData();
				UpdateHandGestureType();
			}
			Profiler.EndSample();


			/// ------------- Tracking -------------
			GetHandTrackingData(TrackerType.Natural);
			GetHandTrackingData(TrackerType.Electronic);

			if (printIntervalLog)
			{
				sb.Clear().Append("Update() Interaction Mode: ").Append(interactionMode.Name())
				.Append(", use xr device: ").Append(m_UseXRDevice)
				.Append(", use xr data (natural): ").Append(UseXRData(TrackerType.Natural))
				.Append(", use xr data (electronic): ").Append(UseXRData(TrackerType.Electronic))
				.Append(", gesture value: ").Append(m_GestureOptions.Gesture.optionValue)
				.Append(", gesture ref count: ").Append(refCountGesture)
				.Append(", tracker: ").Append(m_TrackerOptions.Tracker.Name())
				.Append(", natural ref count: ").Append(refCountNatural)
				.Append(", natural joint: ").Append(m_NaturalHandJointCount)
				.Append(", electronic ref count: ").Append(refCountElectronic)
				.Append(", electronic model: ").Append(m_TrackerOptions.Electronic.Model.Name())
				.Append(", electronic joint: ").Append(m_ElectronicHandJointCount);
				DEBUG(sb);
			}
		}
		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				sb.Clear().Append("OnApplicationPause() Resume, CheckWristPositionFusion"); DEBUG(sb);
				CheckWristPositionFusion();
			}
		}
		private void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_Hand_EnhanceStable, OnWristPositionFusionChange);
		}
		private void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_Hand_EnhanceStable, OnWristPositionFusionChange);
		}
		#endregion

		public bool GetPreferTracker(ref TrackerType tracker)
		{
			TrackerStatus natural_status = GetHandTrackerStatus(TrackerType.Natural);
			TrackerStatus electronic_status = GetHandTrackerStatus(TrackerType.Electronic);
			if (TrackerOptions.Tracker == TrackerSelector.NaturalPrior)
			{
				if (natural_status == TrackerStatus.Available)
				{
					tracker = TrackerType.Natural;
					return true;
				}
				else if (electronic_status == TrackerStatus.Available)
				{
					tracker = TrackerType.Electronic;
					return true;
				}
			}
			else // (m_TrackerOptions.Tracker == TrackerSelector.ElectronicPrior)
			{
				if (electronic_status == TrackerStatus.Available)
				{
					tracker = TrackerType.Electronic;
					return true;
				}
				else if (natural_status == TrackerStatus.Available)
				{
					tracker = TrackerType.Natural;
					return true;
				}
			}
			return false;
		}

		#region Hand Gesture Lifecycle
		private GestureStatus m_HandGestureStatus = GestureStatus.NotStart;
		private static ReaderWriterLockSlim m_HandGestureStatusRWLock = new ReaderWriterLockSlim();
		private void SetHandGestureStatus(GestureStatus status)
		{
			try
			{
				m_HandGestureStatusRWLock.TryEnterWriteLock(2000);
				m_HandGestureStatus = status;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "SetHandGestureStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_HandGestureStatusRWLock.ExitWriteLock();
			}
		}

		private bool CanStartHandGesture()
		{
			GestureStatus status = GetHandGestureStatus();
			if (status == GestureStatus.NotStart ||
				status == GestureStatus.StartFailure)
			{
				return true;
			}

			/*if (!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.GestureLeft) &&
				!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.GestureRight))
			{
				return false;
			}*/

			return false;
		}
		private bool CanStopHandGesture()
		{
			GestureStatus status = GetHandGestureStatus();
			if (status == GestureStatus.Available)
			{
				return true;
			}

			return false;
		}

		private uint refCountGesture = 0;
		private object handGestureThreadLock = new object();
		private event HandGestureResultDelegate handGestureResultCB = null;
		private void StartHandGestureLock()
		{
			if (!CanStartHandGesture())
			{
				sb.Clear().Append("StartHandGestureLock() cannot start hand gesture."); WARNING(sb);
				return;
			}

			SetHandGestureStatus(GestureStatus.Starting);
			sb.Clear().Append("StartHandGestureLock() ").Append(m_GestureOptionValue); DEBUG(sb);
			WVR_Result result = Interop.WVR_StartHandGesture(m_GestureOptionValue);
			switch(result)
			{
				case WVR_Result.WVR_Success:
					SetHandGestureStatus(GestureStatus.Available);
					break;
				case WVR_Result.WVR_Error_FeatureNotSupport:
					SetHandGestureStatus(GestureStatus.NoSupport);
					break;
				default:
					SetHandGestureStatus(GestureStatus.StartFailure);
					break;
			}

			GestureStatus status = GetHandGestureStatus();
			sb.Clear().Append("StartHandGestureLock() ").Append(result).Append(", status: ").Append(status.Name()); DEBUG(sb);
			GeneralEvent.Send(HAND_GESTURE_STATUS, status);

			if (handGestureResultCB != null)
			{
				handGestureResultCB(this, (result == WVR_Result.WVR_Success));
				handGestureResultCB = null;
			}
		}
		private void StartHandGestureThread()
		{
			lock (handGestureThreadLock)
			{
				sb.Clear().Append("StartHandGestureThread()"); DEBUG(sb);
				StartHandGestureLock();
			}
		}
		public void StartHandGesture()
		{
			string caller = Misc.GetCaller();
			refCountGesture++;
			sb.Clear().Append("StartHandGesture() (").Append(refCountNatural).Append(") from ").Append(caller); INFO(sb);

			if (!CanStartHandGesture())
				return;

			Thread hand_gesture_t = new Thread(StartHandGestureThread);
			hand_gesture_t.Name = "StartHandGestureThread";
			hand_gesture_t.Start();
		}

		private void StopHandGestureLock()
		{
			if (!CanStopHandGesture()) { return; }

			GestureStatus status = GetHandGestureStatus();
			if (status == GestureStatus.Available)
			{
				sb.Clear().Append("StopHandGestureLock()"); DEBUG(sb);
				SetHandGestureStatus(GestureStatus.Stopping);
				Interop.WVR_StopHandGesture();
				SetHandGestureStatus(GestureStatus.NotStart);
				hasHandGestureData = false;
			}

			status = GetHandGestureStatus();
			GeneralEvent.Send(HAND_GESTURE_STATUS, status);
		}
		private void StopHandGestureThread()
		{
			lock (handGestureThreadLock)
			{
				sb.Clear().Append("StopHandGestureThread()"); DEBUG(sb);
				StopHandGestureLock();
			}
		}
		public void StopHandGesture()
		{
			string caller = Misc.GetCaller();
			refCountGesture = refCountGesture > 0 ? refCountGesture - 1 : 0;
			sb.Clear().Append("StopHandGesture() (").Append(refCountGesture).Append(") from ").Append(caller); INFO(sb);
			if (refCountGesture > 0) return;

			if (!CanStopHandGesture())
				return;

			Thread hand_gesture_t = new Thread(StopHandGestureThread);
			hand_gesture_t.Name = "StopHandGestureThread";
			hand_gesture_t.Start();
		}

		private void RestartHandGestureThread()
		{
			lock (handGestureThreadLock)
			{
				sb.Clear().Append("RestartHandGestureThread()"); DEBUG(sb);
				StopHandGestureLock();
				StartHandGestureLock();
			}
		}
		#endregion

		#region Hand Gesture Interface
		public GestureStatus GetHandGestureStatus()
		{
			try
			{
				m_HandGestureStatusRWLock.TryEnterReadLock(2000);
				return m_HandGestureStatus;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "GetHandGestureStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_HandGestureStatusRWLock.ExitReadLock();
			}
		}
		public void RestartHandGesture()
		{
			GestureStatus status = GetHandGestureStatus();
			if (status == GestureStatus.Starting || status == GestureStatus.Stopping)
				return;
			Thread hand_gesture_t = new Thread(RestartHandGestureThread);
			hand_gesture_t.Name = "RestartHandGestureThread";
			hand_gesture_t.Start();
		}
		public void RestartHandGesture(HandGestureResultDelegate callback)
		{
			if (handGestureResultCB == null)
				handGestureResultCB = callback;
			else
				handGestureResultCB += callback;

			RestartHandGesture();
		}

		private bool hasHandGestureData = false;
		private WVR_HandGestureData_t handGestureData = new WVR_HandGestureData_t();
		private void GetHandGestureData()
		{
			GestureStatus status = GetHandGestureStatus();
			if (status == GestureStatus.Available)
				hasHandGestureData = Interop.WVR_GetHandGestureData(ref handGestureData) == WVR_Result.WVR_Success ? true : false;
		}

		private GestureType m_HandGestureLeftEx = GestureType.Invalid;
		private GestureType m_HandGestureLeft = GestureType.Invalid;
		private GestureType m_HandGestureRightEx = GestureType.Invalid;
		private GestureType m_HandGestureRight = GestureType.Invalid;
		private void UpdateHandGestureType()
		{
			if (!hasHandGestureData) { return; }

			m_HandGestureLeftEx = m_HandGestureLeft;
			m_HandGestureLeft = handGestureData.left.GetGestureType();

			if (m_HandGestureLeft != m_HandGestureLeftEx)
			{
				sb.Clear().Append("UpdateHandGestureType() Receives ").Append(m_HandGestureLeft); DEBUG(sb);
				GeneralEvent.Send(HAND_STATIC_GESTURE, HandType.Left, m_HandGestureLeft);
			}

			m_HandGestureRightEx = m_HandGestureRight;
			m_HandGestureRight = handGestureData.right.GetGestureType();

			if (m_HandGestureRight != m_HandGestureRightEx)
			{
				sb.Clear().Append("UpdateHandGestureType() Receives ").Append(m_HandGestureRight); DEBUG(sb);
				GeneralEvent.Send(HAND_STATIC_GESTURE, HandType.Right, m_HandGestureRight);
			}
		}

		public GestureType GetHandGesture(bool isLeft)
		{
			return isLeft ? m_HandGestureLeft : m_HandGestureRight;
		}
		public GestureType GetHandGesture(HandType hand)
		{
			return GetHandGesture(hand == HandType.Left ? true : false);
		}
		#endregion

		static WaveXRSettings m_WaveXRSettings = null;
		static bool m_UseXRDevice = true;
		public bool UseXRDevice { get { return m_UseXRDevice; } set { m_UseXRDevice = value; } }
		static bool UseXRData(TrackerType tracker)
		{
			// Hand is already enabled in WaveXRSettings.
			bool XRAlreadyEnabled = false;
			if (m_WaveXRSettings != null)
			{
				XRAlreadyEnabled = (
				  ((tracker == TrackerType.Natural) && m_WaveXRSettings.EnableNaturalHand)
				  //|| ((tracker == TrackerType.Electronic) && m_WaveXRSettings.EnableElectronicHand) // not support electronic hand now.
				  );
			}

			return (
				(XRAlreadyEnabled || m_UseXRDevice)
				&& (tracker == TrackerType.Natural) // XR supports natural hand only.
				&& (!Application.isEditor)
				);
		}

		#region Hand Tracking Lifecycle
		private TrackerStatus m_NaturalTrackerStatus = TrackerStatus.NotStart, m_ElectronicTrackerStatus = TrackerStatus.NotStart;
		private static ReaderWriterLockSlim m_NaturalTrackerStatusRWLock = new ReaderWriterLockSlim(), m_ElectronicTrackerStatusRWLock = new ReaderWriterLockSlim();
		private void SetHandTrackerStatus(TrackerType tracker, TrackerStatus status)
		{
			try
			{
				if (tracker == TrackerType.Electronic)
				{
					m_ElectronicTrackerStatusRWLock.TryEnterWriteLock(2000);
					m_ElectronicTrackerStatus = status;
				}
				if (tracker == TrackerType.Natural)
				{
					m_NaturalTrackerStatusRWLock.TryEnterWriteLock(2000);
					m_NaturalTrackerStatus = status;
				}
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "SetHandTrackerStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				if (tracker == TrackerType.Natural)
					m_NaturalTrackerStatusRWLock.ExitWriteLock();
				if (tracker == TrackerType.Electronic)
					m_ElectronicTrackerStatusRWLock.ExitWriteLock();
			}
		}

		private bool CanStartHandTracker(TrackerSelector selector)
		{
			if (selector == TrackerSelector.ElectronicPrior)
			{
				if (!CanStartHandTracker(TrackerType.Electronic))
				{
					TrackerStatus electronic_status = GetHandTrackerStatus(TrackerType.Electronic);
					switch (electronic_status)
					{
						case TrackerStatus.NoSupport:		// Electronic tracker is not supported.
						case TrackerStatus.NotStart:		// Electronic tracker is supported but no electronic hand connected.
						case TrackerStatus.StartFailure:	// Electronic tracker is supported but has been started failed.
							if (!CanStartHandTracker(TrackerType.Natural))
								return false;
							// else return true; // The natural tracker is able to start.
							break;
						default:
							break;
					}
				}
				// else return true; // The electronic tracker is able to start.
			}
			if (selector == TrackerSelector.NaturalPrior)
			{
				if (!CanStartHandTracker(TrackerType.Natural))
				{
					TrackerStatus natural_status = GetHandTrackerStatus(TrackerType.Natural);
					switch (natural_status)
					{
						case TrackerStatus.NoSupport:	   // Natural tracker is not supported.
						case TrackerStatus.NotStart:		// Natural tracker is supported but no electronic hand connected.
						case TrackerStatus.StartFailure:	// Natural tracker is supported but has been started failed.
							if (!CanStartHandTracker(TrackerType.Electronic))
								return false;
							// else return true; // The natural tracker is able to start.
							break;
						default:
							break;
					}
				}
				// else return true; // The natural tracker is able to start.
			}

			return true;
		}
		private bool CanStartHandTracker(TrackerType tracker)
		{
			if (tracker == TrackerType.Natural)
			{
				TrackerStatus status = GetHandTrackerStatus(TrackerType.Natural);
				if (status == TrackerStatus.Available ||
					status == TrackerStatus.Starting ||
					status == TrackerStatus.Stopping ||
					status == TrackerStatus.NoSupport)
				{
					sb.Clear().Append("CanStartHandTracker() status: ").Append(status); DEBUG(sb);
					return false;
				}

				/*if (!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.NaturalLeft) &&
					!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.NaturalRight))
				{
					return false;
				}*/
			}
			if (tracker == TrackerType.Electronic)
			{
				TrackerStatus status = GetHandTrackerStatus(TrackerType.Electronic);
				if (status == TrackerStatus.Available ||
					status == TrackerStatus.Starting ||
					status == TrackerStatus.Stopping ||
					status == TrackerStatus.NoSupport)
				{
					return false;
				}

				/*if (!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.ElectronicLeft) &&
					!WaveEssence.Instance.IsHandDeviceConnected(XR_HandDevice.ElectronicRight))
				{
					return false;
				}*/
			}

			return true;
		}
		private bool CanStopHandTracker(TrackerType tracker)
		{
			var status = GetHandTrackerStatus(tracker);
			if (status == TrackerStatus.Available) { return true; }
			sb.Clear().Append("CanStopHandTracker() status:").Append(status.Name()); DEBUG(sb);
			return false;
		}

		private uint refCountNatural = 0, refCountElectronic = 0;
		private object handTrackerThreadLocker = new object();
		private event HandTrackerResultDelegate handTrackerResultCB = null;
		private void StartHandTrackerLock(TrackerType tracker)
		{
			if (!CanStartHandTracker(tracker))
				return;

			if (UseXRData(tracker))
			{
				if (tracker == TrackerType.Natural) { InputDeviceHand.ActivateNaturalHand(true); }
#pragma warning disable
				if (tracker == TrackerType.Electronic) { InputDeviceHand.ActivateElectronicHand(true); }
#pragma warning enable
				sb.Clear().Append("StartHandTrackerLock() XR ").Append(tracker.Name()); DEBUG(sb);
				return;
			}

			SetHandTrackerStatus(tracker, TrackerStatus.Starting);
			WVR_Result result = Interop.WVR_StartHandTracking((WVR_HandTrackerType)tracker);
			switch (result)
			{
				case WVR_Result.WVR_Success:
					SetHandTrackerStatus(tracker, TrackerStatus.Available);
					GetHandJointCount(tracker);
					GetHandTrackerInfo(tracker);
					break;
				case WVR_Result.WVR_Error_FeatureNotSupport:
					SetHandTrackerStatus(tracker, TrackerStatus.NoSupport);
					break;
				default:
					SetHandTrackerStatus(tracker, TrackerStatus.StartFailure);
					break;
			}

			TrackerStatus status = GetHandTrackerStatus(tracker);
			sb.Clear().Append("StartHandTrackerLock() ").Append	(tracker.Name()).Append(", ").Append(result).Append(", status: ").Append(status.Name()); DEBUG(sb);
			GeneralEvent.Send(HAND_TRACKER_STATUS, tracker, status);

			if (handTrackerResultCB != null)
			{
				handTrackerResultCB(this, result == WVR_Result.WVR_Success ? true : false);
				handTrackerResultCB = null;
			}
		}
		private void StartHandTrackerThread(object tracker)
		{
			lock (handTrackerThreadLocker)
			{
				sb.Clear().Append("StartHandTrackerThread() ").Append(((TrackerType)tracker).Name()); DEBUG(sb);
				StartHandTrackerLock((TrackerType)tracker);
			}
		}
		public void StartHandTracker(TrackerType tracker)
		{
			if (!CanStartHandTracker(tracker))
				return;

			string caller = Misc.GetCaller();
			if (tracker == TrackerType.Natural)
			{
				refCountNatural++;
				sb.Clear().Append("StartHandTracker() ").Append(tracker.Name()).Append("(").Append(refCountNatural).Append(") from ").Append(caller); INFO(sb);
			}
			if (tracker == TrackerType.Electronic)
			{
				refCountElectronic++;
				sb.Clear().Append("StartHandTracker() ").Append(tracker.Name()).Append("(").Append(refCountElectronic).Append(") from ").Append(caller); INFO(sb);
			}

			Thread hand_tracker_t = new Thread(StartHandTrackerThread);
			hand_tracker_t.Name = "StartHandTrackerThread";
			hand_tracker_t.Start(tracker);
		}

		private void StopHandTrackerLock(TrackerType tracker)
		{
			if (!CanStopHandTracker(tracker))
				return;

			if (UseXRData(tracker))
			{
				if (tracker == TrackerType.Natural) { InputDeviceHand.ActivateNaturalHand(false); }
#pragma warning disable
				if (tracker == TrackerType.Electronic) { InputDeviceHand.ActivateElectronicHand(false); }
#pragma warning enable
				sb.Clear().Append("StopHandTrackerLock() XR ").Append(tracker.Name()); DEBUG(sb);
				return;
			}

			sb.Clear().Append("StopHandTrackerLock() ").Append(tracker.Name()); DEBUG(sb);
			SetHandTrackerStatus(tracker, TrackerStatus.Stopping);
			Interop.WVR_StopHandTracking((WVR_HandTrackerType)tracker);
			SetHandTrackerStatus(tracker, TrackerStatus.NotStart);
			if (tracker == TrackerType.Natural)
				hasNaturalHandTrackerData = false;
			if (tracker == TrackerType.Electronic)
				hasElectronicHandTrackerData = false;

			TrackerStatus status = GetHandTrackerStatus(tracker);
			GeneralEvent.Send(HAND_TRACKER_STATUS, tracker, status);
		}
		private void StopHandTrackerThread(object tracker)
		{
			lock (handTrackerThreadLocker)
			{
				sb.Clear().Append("StopHandTrackerThread() ").Append(((TrackerType)tracker).Name()); DEBUG(sb);
				StopHandTrackerLock((TrackerType)tracker);
			}
		}
		public void StopHandTracker(TrackerType tracker)
		{
			if (!CanStopHandTracker(tracker))
				return;

			string caller = Misc.GetCaller();
			if (tracker == TrackerType.Natural)
			{
				refCountNatural = refCountNatural > 0 ? refCountNatural - 1 : 0;
				sb.Clear().Append("StopHandTracker() ").Append(tracker.Name()).Append("(").Append(refCountNatural).Append(") from ").Append(caller); INFO(sb);
				if (refCountNatural > 0) return;
			}
			if (tracker == TrackerType.Electronic)
			{
				refCountElectronic = refCountElectronic > 0 ? refCountElectronic - 1 : 0;
				sb.Clear().Append("StopHandTracker() ").Append(tracker.Name()).Append("(").Append(refCountElectronic).Append(") from ").Append(caller); INFO(sb);
				if (refCountElectronic > 0) return;
			}

			Thread hand_tracker_t = new Thread(StopHandTrackerThread);
			hand_tracker_t.Name = "StopHandTrackerThread";
			hand_tracker_t.Start(tracker);
		}

		private void RestartHandTrackerThread(object tracker)
		{
			lock (handTrackerThreadLocker)
			{
				sb.Clear().Append("RestartHandTrackerThread() ").Append(((TrackerType)tracker).Name()); DEBUG(sb);
				if (UseXRData((TrackerType)tracker))
				{
					StopHandTrackerLock((TrackerType)tracker);

					uint waitCount = 0;
					TrackerStatus status = GetHandTrackerStatus((TrackerType)tracker);
					while (status != TrackerStatus.NotStart && waitCount <= 5)
					{
						sb.Clear().Append("RestartHandTrackerThread() status: ").Append(status.Name()).Append(", wait 1s."); DEBUG(sb);
						Thread.Sleep(1000); // wait 1s.
						waitCount++;
						status = GetHandTrackerStatus((TrackerType)tracker);
					}

					StartHandTrackerLock((TrackerType)tracker);
				}
				else
				{
					StopHandTrackerLock((TrackerType)tracker);
					StartHandTrackerLock((TrackerType)tracker);
				}
			}
		}
		#endregion

		#region Hand Tracking Interface
		internal static UnityEngine.XR.Bone boneDef = new Bone();
		public static bool GetBone(HandJoint joint, bool isLeft, out UnityEngine.XR.Bone outBone)
		{
			outBone = boneDef;
			if (!UseXRData(TrackerType.Natural)) { return false; } // Supports natural hand only.

			if (joint == HandJoint.Wrist)
				outBone = InputDeviceHand.GetWrist(isLeft);
			else if (joint == HandJoint.Palm)
				outBone = InputDeviceHand.GetPalm(isLeft);
			else
			{
				if (joint.Index() < 0) { return false; }
				var finger_bones = InputDeviceHand.GetFingerBones(isLeft, joint.Finger());
				if (finger_bones.Count <= joint.Index()) { return false; }
				outBone = finger_bones[joint.Index()];
			}

			return true;
		}

		public TrackerStatus GetHandTrackerStatus(TrackerType tracker)
		{
			if (UseXRData(tracker))
			{
				if (tracker == TrackerType.Electronic) { return TrackerStatus.NoSupport; }

				var status = InputDeviceHand.GetNaturalHandStatus();
				switch(status)
				{
					case InputDeviceHand.TrackingStatus.NOT_START:
						return TrackerStatus.NotStart;
					case InputDeviceHand.TrackingStatus.START_FAILURE:
						return TrackerStatus.StartFailure;
					case InputDeviceHand.TrackingStatus.STARTING:
						return TrackerStatus.Starting;
					case InputDeviceHand.TrackingStatus.STOPPING:
						return TrackerStatus.Stopping;
					case InputDeviceHand.TrackingStatus.AVAILABLE:
						return TrackerStatus.Available;
					default:
						return TrackerStatus.NoSupport;
				}
			}

			try
			{
				if (tracker == TrackerType.Electronic)
				{
					m_ElectronicTrackerStatusRWLock.TryEnterReadLock(2000);
					return m_ElectronicTrackerStatus;
				}
				else // TrackerType.Natural
				{
					m_NaturalTrackerStatusRWLock.TryEnterReadLock(2000);
					return m_NaturalTrackerStatus;
				}
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "GetHandTrackerStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				if (tracker == TrackerType.Electronic)
					m_ElectronicTrackerStatusRWLock.ExitReadLock();
				else // TrackerType.Natural
					m_NaturalTrackerStatusRWLock.ExitReadLock();
			}
		}
		public TrackerStatus GetHandTrackerStatus()
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetHandTrackerStatus(tracker);
			else
				return TrackerStatus.NotStart;
		}

		public void RestartHandTracker(TrackerType tracker)
		{
			TrackerStatus status = GetHandTrackerStatus();
			if (status == TrackerStatus.Starting || status == TrackerStatus.Stopping)
				return;

			sb.Clear().Append("RestartHandTracker() ").Append(tracker.Name()); INFO(sb);
			Thread hand_tracker_t = new Thread(RestartHandTrackerThread);
			hand_tracker_t.Name = "RestartHandTrackerThread";
			hand_tracker_t.Start(tracker);
		}
		public void RestartHandTracker(TrackerType tracker, HandTrackerResultDelegate callback)
		{
			if (handTrackerResultCB == null)
				handTrackerResultCB = callback;
			else
				handTrackerResultCB += callback;

			RestartHandTracker(tracker);
		}
		public void RestartHandTracker()
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				RestartHandTracker(tracker);
		}

		/// <summary>
		/// Retrieves the timestamp of hand tracking data of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="timestamp">The timestamp of hand tracking data.</param>
		/// <returns>True for valid data.</returns>
		public bool GetHandTrackingTimestamp(TrackerType tracker, out long timestamp)
		{
			timestamp = 0;

			if (tracker == TrackerType.Natural && hasNaturalHandTrackerData && hasNaturalTrackerInfo)
			{
				timestamp = m_NaturalHandTrackerData.timestamp;
				return true;
			}
			if (tracker == TrackerType.Electronic && hasElectronicHandTrackerData && hasElectronicTrackerInfo)
			{
				timestamp = m_ElectronicHandTrackerData.timestamp;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Retrieves the timestamp of hand tracking data of the current available <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="timestamp">The timestamp of hand tracking data.</param>
		/// <returns>True for valid data.</returns>
		public bool GetHandTrackingTimestamp(out long timestamp)
		{
			timestamp = 0;

			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
			{
				return GetHandTrackingTimestamp(tracker, out timestamp);
			}

			return false;
		}

		#region Pose Valid
		/// <summary>
		/// Checks if left/hand pose of a <see cref="TrackerType">tracker</see> is valid.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>True for valid data.</returns>
		public bool IsHandPoseValid(TrackerType tracker, bool isLeft)
		{
			if (UseXRData(tracker))
			{
				return InputDeviceHand.IsTracked(isLeft);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						return m_NaturalHandTrackerData.left.isValidPose;
					else
						return m_NaturalHandTrackerData.right.isValidPose;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						return m_ElectronicHandTrackerData.left.isValidPose;
					else
						return m_ElectronicHandTrackerData.right.isValidPose;
				}
			}

			return false;
		}
		public bool IsHandPoseValid(TrackerType tracker, HandType hand)
		{
			return IsHandPoseValid(tracker, hand == HandType.Left ? true : false);
		}
		public bool IsHandPoseValid(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return IsHandPoseValid(tracker, isLeft);
			return false;
		}
		public bool IsHandPoseValid(HandType hand)
		{
			return IsHandPoseValid(hand == HandType.Left ? true : false);
		}
		public bool IsHandPoseValid(TrackerType tracker, bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(tracker, out timestamp);
			return IsHandPoseValid(tracker, isLeft);
		}
		public bool IsHandPoseValid(bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(out timestamp);
			return IsHandPoseValid(isLeft);
		}
		#endregion
		#region Confidence
		public bool GetHandConfidence(TrackerType tracker, bool isLeft, out float confidence)
		{
			confidence = 0;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetHandConfidence(isLeft, out confidence);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					confidence = isLeft ? m_NaturalHandTrackerData.left.confidence : m_NaturalHandTrackerData.right.confidence;
					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					confidence = isLeft ? m_ElectronicHandTrackerData.left.confidence : m_ElectronicHandTrackerData.right.confidence;
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// Retrieves left/right hand's confidence of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>A float {0, 1} value where 1 means the most reliable.</returns>
		public float GetHandConfidence(TrackerType tracker, bool isLeft)
		{
			if (GetHandConfidence(tracker, isLeft, out float confidence)) { return confidence; }
			return 0;
		}
		public float GetHandConfidence(TrackerType tracker, HandType hand)
		{
			return GetHandConfidence(tracker, hand == HandType.Left ? true : false);
		}
		public float GetHandConfidence(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetHandConfidence(tracker, isLeft);
			return 0;
		}
		public float GetHandConfidence(HandType hand)
		{
			return GetHandConfidence(hand == HandType.Left ? true : false);
		}
		public float GetHandConfidence(TrackerType tracker, bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetHandConfidence(tracker, isLeft);
		}
		public float GetHandConfidence(bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(out timestamp);
			return GetHandConfidence(isLeft);
		}
		#endregion
		#region Grasp Strength
		public bool GetGraspStrength(TrackerType tracker, bool isLeft, out float strength)
		{
			strength = 0;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetGraspStrength(isLeft, out strength);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					strength = isLeft ? m_NaturalHandTrackerData.left.grasp.strength : m_NaturalHandTrackerData.right.grasp.strength;
					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					strength = isLeft ? m_ElectronicHandTrackerData.left.grasp.strength : m_ElectronicHandTrackerData.right.grasp.strength;
					return true;
				}
			}

			return false;
		}
		/// <summary>
		/// Retrieves left/right hand's grasp strength of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="isLeft">True for left hand.</param>
		public float GetGraspStrength(TrackerType tracker, bool isLeft)
		{
			if (GetGraspStrength(tracker, isLeft, out float strength)) { return strength; }
			return 0;
		}
		public float GetGraspStrength(TrackerType tracker, HandType hand)
		{
			return GetGraspStrength(tracker, hand == HandType.Left ? true : false);
		}
		public float GetGraspStrength(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetGraspStrength(tracker, isLeft);
			return 0;
		}
		public float GetGraspStrength(HandType hand)
		{
			return GetGraspStrength(hand == HandType.Left ? true : false);
		}
		public float GetGraspStrength(TrackerType tracker, bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetGraspStrength(tracker, isLeft);
		}
		public float GetGraspStrength(bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(out timestamp);
			return GetGraspStrength(isLeft);
		}
		#endregion
		#region Hand Grasp
		/// <summary>
		/// Retrieves left/right hand's grasp status of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="isLeft">True for left hand.</param>
		public bool IsHandGrasping(TrackerType tracker, bool isLeft)
		{
			if (UseXRData(tracker))
			{
				return InputDeviceHand.IsHandGrasping(isLeft);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					return isLeft ? m_NaturalHandTrackerData.left.grasp.isGrasp : m_NaturalHandTrackerData.right.grasp.isGrasp;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					return isLeft ? m_ElectronicHandTrackerData.left.grasp.isGrasp : m_ElectronicHandTrackerData.right.grasp.isGrasp; ;
				}
			}

			return false;
		}
		public bool IsHandGrasping(TrackerType tracker, HandType hand)
		{
			return IsHandGrasping(tracker, hand == HandType.Left ? true : false);
		}
		public bool IsHandGrasping(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return IsHandGrasping(tracker, isLeft);
			return false;
		}
		public bool IsHandGrasping(HandType hand)
		{
			return IsHandGrasping(hand == HandType.Left ? true : false);
		}
		public bool IsHandGrasping(TrackerType tracker, bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(tracker, out timestamp);
			return IsHandGrasping(tracker, isLeft);
		}
		public bool IsHandGrasping(bool isLeft, out long timestamp)
		{
			GetHandTrackingTimestamp(out timestamp);
			return IsHandGrasping(isLeft);
		}
		#endregion
		#region Joint Position
		/// <summary> @position will not be updated when no position. </summary>
		public bool GetJointPosition(TrackerType tracker, HandJoint joint, ref Vector3 position, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
			{
				if (printIntervalLog)
				{
					sb.Clear().Append("GetJointPosition() tracker ")
						.Append(tracker.Name())
						.Append((isLeft ? " Left" : " Right"))
						.Append(" joint ").Append(joint.Name())
						.Append(" has invalid pose.");
					DEBUG(sb);
				}
				return false;
			}

			bool ret = false;

			if (UseXRData(tracker))
			{
				if (GetBone(joint, isLeft, out Bone bone))
				{
					if (bone.TryGetPosition(out Vector3 pos))
					{
						position = pos;
						ret = true;
					}
				}
			}
			else
			{
				int joint_index = -1;

				if (tracker == TrackerType.Natural)
				{
					// Checks the WVR_HandTrackerInfo_t to get the valid joint index.
					for (int i = 0; i < m_NaturalTrackerInfo.jointCount; i++)
					{
						if (s_NaturalHandJoints[i] == (WVR_HandJoint)joint &&
							(s_NaturalHandJointsFlag[i] & (ulong)WVR_HandJointValidFlag.WVR_HandJointValidFlag_PositionValid) != 0)
						{
							joint_index = i;
							break;
						}
					}

					// Gets the WVR_HandJointData_t with the valid index.
					if (joint_index >= 0)
					{
						if (isLeft && joint_index < s_NaturalHandJointsPoseLeft.Length)
						{
							Coordinate.GetVectorFromGL(s_NaturalHandJointsPoseLeft[joint_index].position, out position);
							ret = true;
						}
						if (!isLeft && joint_index < s_NaturalHandJointsPoseRight.Length)
						{
							Coordinate.GetVectorFromGL(s_NaturalHandJointsPoseRight[joint_index].position, out position);
							ret = true;
						}
					}
				}
				if (tracker == TrackerType.Electronic)
				{
					// Checks the WVR_HandTrackerInfo_t to get the valid joint index.
					for (int i = 0; i < m_ElectronicTrackerInfo.jointCount; i++)
					{
						if (s_ElectronicHandJoints[i] == (WVR_HandJoint)joint &&
							(s_ElectronicHandJointsFlag[i] & (ulong)WVR_HandJointValidFlag.WVR_HandJointValidFlag_PositionValid) != 0)
						{
							joint_index = i;
							break;
						}
					}

					// Gets the WVR_HandJointData_t with the valid index.
					if (joint_index >= 0)
					{
						if (isLeft && joint_index < s_ElectronicHandJointsPoseLeft.Length)
						{
							Coordinate.GetVectorFromGL(s_ElectronicHandJointsPoseLeft[joint_index].position, out position);
							ret = true;
						}
						if (!isLeft && joint_index < s_ElectronicHandJointsPoseRight.Length)
						{
							Coordinate.GetVectorFromGL(s_ElectronicHandJointsPoseRight[joint_index].position, out position);
							ret = true;
						}
					}
				}
			}

			if (printIntervalLog)
			{
				sb.Clear().Append("GetJointPosition() tracker: ").Append(tracker.Name())
					.Append(", ").Append((isLeft ? "Left" : "Right"))
					.Append(" joint ").Append(joint.Name())
					.Append(", ").Append("position (").Append(position.x).Append(", ").Append(position.y).Append(", ").Append(position.z).Append(")");
				DEBUG(sb);
			}

			return ret;
		}
		/// <summary> @position will not be updated when no position. </summary>
		public bool GetJointPosition(TrackerType tracker, HandJoint joint, ref Vector3 position, HandType hand)
		{
			return GetJointPosition(tracker, joint, ref position, hand == HandType.Left ? true : false);
		}
		public bool GetJointPosition(HandJoint joint, ref Vector3 position, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetJointPosition(tracker, joint, ref position, isLeft);
			return false;
		}
		public bool GetJointPosition(HandJoint joint, ref Vector3 position, HandType hand)
		{
			return GetJointPosition(joint, ref position, hand == HandType.Left ? true : false);
		}
		public bool GetJointPosition(TrackerType tracker, HandJoint joint, out Vector3 position, bool isLeft, out long timestamp)
		{
			position = Vector3.zero;
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetJointPosition(tracker, joint, ref position, isLeft);
		}
		public bool GetJointPosition(HandJoint joint, out Vector3 position, bool isLeft, out long timestamp)
		{
			position = Vector3.zero;
			GetHandTrackingTimestamp(out timestamp);
			return GetJointPosition(joint, ref position, isLeft);
		}
		#endregion
		#region Joint Rotation
		/// <summary> @rotation will not be updated when no rotation. </summary>
		public bool GetJointRotation(TrackerType tracker, HandJoint joint, ref Quaternion rotation, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
			{
				if (printIntervalLog)
				{
					sb.Clear().Append("GetJointRotation() tracker ").Append(tracker.Name())
						.Append((isLeft ? " Left" : " Right")).Append(" joint ").Append(joint.Name()).Append(" has invalid pose.");
					DEBUG(sb);
				}
				return false;
			}

			bool ret = false;

			if (UseXRData(tracker))
			{
				if (GetBone(joint, isLeft, out Bone bone))
				{
					if (bone.TryGetRotation(out Quaternion rot))
					{
						rotation = rot;
						ret = true;
					}
				}
			}
			else
			{
				int joint_index = -1;

				if (tracker == TrackerType.Natural)
				{
					// Checks the WVR_HandTrackerInfo_t to get the valid joint index.
					for (int i = 0; i < m_NaturalTrackerInfo.jointCount; i++)
					{
						if (s_NaturalHandJoints[i] == (WVR_HandJoint)joint &&
							(s_NaturalHandJointsFlag[i] & (ulong)WVR_HandJointValidFlag.WVR_HandJointValidFlag_RotationValid) != 0)
						{
							joint_index = i;
							break;
						}
					}

					// Gets the WVR_HandJointData_t with the valid index.
					if (joint_index >= 0)
					{
						if (isLeft && joint_index < s_NaturalHandJointsPoseLeft.Length)
						{
							Coordinate.GetQuaternionFromGL(s_NaturalHandJointsPoseLeft[joint_index].rotation, out rotation);
							ret = true;
						}
						if (!isLeft && joint_index < s_NaturalHandJointsPoseRight.Length)
						{
							Coordinate.GetQuaternionFromGL(s_NaturalHandJointsPoseRight[joint_index].rotation, out rotation);
							ret = true;
						}
					}
				}
				if (tracker == TrackerType.Electronic)
				{
					// Checks the WVR_HandTrackerInfo_t to get the valid joint index.
					for (int i = 0; i < m_ElectronicTrackerInfo.jointCount; i++)
					{
						if (s_ElectronicHandJoints[i] == (WVR_HandJoint)joint &&
							(s_ElectronicHandJointsFlag[i] & (ulong)WVR_HandJointValidFlag.WVR_HandJointValidFlag_RotationValid) != 0)
						{
							joint_index = i;
							break;
						}
					}

					// Gets the WVR_HandJointData_t with the valid index.
					if (joint_index >= 0)
					{
						if (isLeft && joint_index < s_ElectronicHandJointsPoseLeft.Length)
						{
							Coordinate.GetQuaternionFromGL(s_ElectronicHandJointsPoseLeft[joint_index].rotation, out rotation);
							ret = true;
						}
						if (!isLeft && joint_index < s_ElectronicHandJointsPoseRight.Length)
						{
							Coordinate.GetQuaternionFromGL(s_ElectronicHandJointsPoseRight[joint_index].rotation, out rotation);
							ret = true;
						}
					}
				}
			}

			if (printIntervalLog)
			{
				sb.Clear().Append("GetJointRotation()").Append(" tracker: ").Append(tracker.Name())
					.Append(", ").Append((isLeft ? "Left" : "Right")).Append(" joint ").Append(joint.Name())
					.Append(", rotation (").Append(rotation.x).Append(", ").Append(rotation.y).Append(", ").Append(rotation.z).Append(", ").Append(rotation.w).Append(")");
				DEBUG(sb);
			}

			return ret;
		}
		/// <summary> @rotation will not be updated when no rotation. </summary>
		public bool GetJointRotation(TrackerType tracker, HandJoint joint, ref Quaternion rotation, HandType hand)
		{
			return GetJointRotation(tracker, joint, ref rotation, hand == HandType.Left ? true : false);
		}
		public bool GetJointRotation(HandJoint joint, ref Quaternion rotation, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetJointRotation(tracker, joint, ref rotation, isLeft);
			return false;
		}
		public bool GetJointRotation(HandJoint joint, ref Quaternion rotation, HandType hand)
		{
			return GetJointRotation(joint, ref rotation, hand == HandType.Left ? true : false);
		}
		public bool GetJointRotation(TrackerType tracker, HandJoint joint, out Quaternion rotation, bool isLeft, out long timestamp)
		{
			rotation = Quaternion.identity;
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetJointRotation(tracker, joint, ref rotation, isLeft);
		}
		public bool GetJointRotation(HandJoint joint, out Quaternion rotation, bool isLeft, out long timestamp)
		{
			rotation = Quaternion.identity;
			GetHandTrackingTimestamp(out timestamp);
			return GetJointRotation(joint, ref rotation, isLeft);
		}
		#endregion
		#region Scale
		/// <summary> @scale will not be updated when no scale. </summary>
		public bool GetHandScale(TrackerType tracker, ref Vector3 scale, bool isLeft)
		{
			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetHandScale(isLeft, out scale);
			}

			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			bool ret = false;

			if (tracker == TrackerType.Natural)
			{
				var scaleGL = isLeft ? m_NaturalHandJointDataLeft.scale : m_NaturalHandJointDataRight.scale;
				scale.x = scaleGL.v0;
				scale.y = scaleGL.v1;
				scale.z = scaleGL.v2;

				ret = true;
			}
			if (tracker == TrackerType.Electronic)
			{
				var scaleGL = isLeft ? m_ElectronicHandJointDataLeft.scale : m_ElectronicHandJointDataRight.scale;
				scale.x = scaleGL.v0;
				scale.y = scaleGL.v1;
				scale.z = scaleGL.v2;

				ret = true;
			}

			if (printIntervalLog)
			{
				sb.Clear().Append("GetHandScale()").Append(" tracker: ").Append(tracker.Name())
					.Append(", ").Append((isLeft ? "Left" : "Right"))
					.Append(" scale (").Append(scale.x).Append(", ").Append(scale.y).Append(", ").Append(scale.z).Append(")");
				DEBUG(sb);
			}

			return ret;
		}

		/// <summary> @scale will not be updated when no scale. </summary>
		public bool GetHandScale(TrackerType tracker, ref Vector3 scale, HandType hand)
		{
			return GetHandScale(tracker, ref scale, hand == HandType.Left ? true : false);
		}
		public bool GetHandScale(ref Vector3 scale, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetHandScale(tracker, ref scale, isLeft);
			return false;
		}
		public bool GetHandScale(ref Vector3 scale, HandType hand)
		{
			return GetHandScale(ref scale, hand == HandType.Left ? true : false);
		}
		public bool GetHandScale(TrackerType tracker, out Vector3 scale, bool isLeft, out long timestamp)
		{
			scale = Vector3.zero;
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetHandScale(tracker, ref scale, isLeft);
		}
		public bool GetHandScale(out Vector3 scale, bool isLeft, out long timestamp)
		{
			scale = Vector3.zero;
			GetHandTrackingTimestamp(out timestamp);
			return GetHandScale(ref scale, isLeft);
		}
		#endregion
		#region Linear Velocity
		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristLinearVelocity(TrackerType tracker, ref Vector3 velocity, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetWristLinearVelocity(isLeft, out velocity);
			}

			bool ret = false;

			if (tracker == TrackerType.Natural)
			{
				if (isLeft)
					Coordinate.GetVectorFromGL(m_NaturalHandJointDataLeft.wristLinearVelocity, out velocity);
				else
					Coordinate.GetVectorFromGL(m_NaturalHandJointDataRight.wristLinearVelocity, out velocity);

				ret = true;
			}
			if (tracker == TrackerType.Electronic)
			{
				if (isLeft)
					Coordinate.GetVectorFromGL(m_ElectronicHandJointDataLeft.wristLinearVelocity, out velocity);
				else
					Coordinate.GetVectorFromGL(m_ElectronicHandJointDataRight.wristLinearVelocity, out velocity);

				ret = true;
			}

			if (printIntervalLog)
			{
				sb.Clear().Append("GetWristLinearVelocity()").Append(" tracker: ").Append(tracker.Name())
					.Append(", ").Append((isLeft ? "Left" : "Right"))
					.Append(" velocity (").Append(velocity.x).Append(", ").Append(velocity.y).Append(", ").Append(velocity.z).Append(")");
				DEBUG(sb);
			}

			return ret;
		}
		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristLinearVelocity(TrackerType tracker, ref Vector3 velocity, HandType hand)
		{
			return GetWristLinearVelocity(tracker, ref velocity, hand == HandType.Left ? true : false);
		}
		public bool GetWristLinearVelocity(ref Vector3 velocity, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetWristLinearVelocity(tracker, ref velocity, isLeft);
			return false;
		}
		public bool GetWristLinearVelocity(ref Vector3 velocity, HandType hand)
		{
			return GetWristLinearVelocity(ref velocity, hand == HandType.Left ? true : false);
		}
		public bool GetWristLinearVelocity(TrackerType tracker, out Vector3 velocity, bool isLeft, out long timestamp)
		{
			velocity = Vector3.zero;
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetWristLinearVelocity(tracker, ref velocity, isLeft);
		}
		public bool GetWristLinearVelocity(out Vector3 velocity, bool isLeft, out long timestamp)
		{
			velocity = Vector3.zero;
			GetHandTrackingTimestamp(out timestamp);
			return GetWristLinearVelocity(ref velocity, isLeft);
		}
		#endregion
		#region Angular Velocity
		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristAngularVelocity(TrackerType tracker, ref Vector3 velocity, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetWristAngularVelocity(isLeft, out velocity);
			}

			bool ret = false;

			if (tracker == TrackerType.Natural)
			{
				if (isLeft)
					Coordinate.GetVectorFromGL(m_NaturalHandJointDataLeft.wristAngularVelocity, out velocity);
				else
					Coordinate.GetVectorFromGL(m_NaturalHandJointDataRight.wristAngularVelocity, out velocity);

				ret = true;
			}
			if (tracker == TrackerType.Electronic)
			{
				if (isLeft)
					Coordinate.GetVectorFromGL(m_ElectronicHandJointDataLeft.wristAngularVelocity, out velocity);
				else
					Coordinate.GetVectorFromGL(m_ElectronicHandJointDataRight.wristAngularVelocity, out velocity);

				ret = true;
			}

			if (printIntervalLog)
			{
				sb.Clear().Append("GetWristAngularVelocity()").Append(" tracker: ").Append(tracker.Name())
					.Append(", ").Append((isLeft ? "Left" : "Right"))
					.Append(" velocity (").Append(velocity.x).Append(", ").Append(velocity.y).Append(", ").Append(velocity.z).Append(")");
				DEBUG(sb);
			}

			return ret;
		}
		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristAngularVelocity(TrackerType tracker, ref Vector3 velocity, HandType hand)
		{
			return GetWristAngularVelocity(tracker, ref velocity, hand == HandType.Left ? true : false);
		}
		public bool GetWristAngularVelocity(ref Vector3 velocity, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetWristAngularVelocity(tracker, ref velocity, isLeft);
			return false;
		}
		public bool GetWristAngularVelocity(ref Vector3 velocity, HandType hand)
		{
			return GetWristAngularVelocity(ref velocity, hand == HandType.Left ? true : false);
		}
		public bool GetWristAngularVelocity(TrackerType tracker, out Vector3 velocity, bool isLeft, out long timestamp)
		{
			velocity = Vector3.zero;
			GetHandTrackingTimestamp(tracker, out timestamp);
			return GetWristAngularVelocity(tracker, ref velocity, isLeft);
		}
		public bool GetWristAngularVelocity(out Vector3 velocity, bool isLeft, out long timestamp)
		{
			velocity = Vector3.zero;
			GetHandTrackingTimestamp(out timestamp);
			return GetWristAngularVelocity(ref velocity, isLeft);
		}
		#endregion

		/// <summary>
		/// Retrieves the timestamp of hand pose data of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="timestamp">Timestamp of hand pose data</param>
		/// <returns></returns>
		public bool GetHandPoseTimestamp(TrackerType tracker, out long timestamp)
		{
			timestamp = 0;

			if (tracker == TrackerType.Natural && hasNaturalHandTrackerData && hasNaturalTrackerInfo)
			{
				timestamp = m_NaturalHandPoseData.timestamp;
				return true;
			}
			if (tracker == TrackerType.Electronic && hasElectronicHandTrackerData && hasElectronicTrackerInfo)
			{
				timestamp = m_ElectronicHandPoseData.timestamp;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Retrieves the timestamp of hand pose data of the current available <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="timestamp">Timestamp of hand pose data</param>
		/// <returns></returns>
		public bool GetHandPoseTimestamp(out long timestamp)
		{
			timestamp = 0;

			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
			{
				return GetHandTrackingTimestamp(tracker, out timestamp);
			}

			return false;
		}

		#region Motion
		/// <summary> Checks if the player in taking a motion, e.g. Pinch, Hold. </summary>
		public bool GetHandMotion(TrackerType tracker, out HandMotion motion, bool isLeft)
		{
			motion = HandMotion.None;

			if (UseXRData(tracker))
			{
				if (InputDeviceHand.GetHandMotion(isLeft, out InputDeviceHand.HandMotion xrMotion))
				{
					motion = xrMotion.WaveType();
					return true;
				}
				return false;
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					motion = isLeft ?
						(HandMotion)m_NaturalHandPoseData.left.state.type :
						(HandMotion)m_NaturalHandPoseData.right.state.type;

					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					motion = isLeft ?
						(HandMotion)m_ElectronicHandPoseData.left.state.type :
						(HandMotion)m_ElectronicHandPoseData.right.state.type;

					return true;
				}
			}

			return false;
		}
		public HandMotion GetHandMotion(TrackerType tracker, bool isLeft)
		{
			HandMotion motion = HandMotion.None;

			if (GetHandMotion(tracker, out HandMotion value, isLeft))
				motion = value;

			return motion;
		}
		public HandMotion GetHandMotion(TrackerType tracker, HandType hand)
		{
			return GetHandMotion(tracker, hand == HandType.Left ? true : false);
		}
		public bool GetHandMotion(out HandMotion motion, bool isLeft)
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
			{
				return GetHandMotion(tracker, out motion, isLeft);
			}

			motion = HandMotion.None;
			return false;
		}
		public HandMotion GetHandMotion(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetHandMotion(tracker, isLeft);
			return HandMotion.None;
		}
		public HandMotion GetHandMotion(HandType hand)
		{
			return GetHandMotion(hand == HandType.Left ? true : false);
		}
		public bool GetHandMotion(TrackerType tracker, out HandMotion motion, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetHandMotion(tracker, out motion, isLeft);
		}
		public bool GetHandMotion(out HandMotion motion, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(out timestamp);
			return GetHandMotion(out motion, isLeft);
		}
		#endregion
		#region Hold Role
		/// <summary> Checks if the player is holding or side holding an equipment. </summary>
		public bool GetHandHoldRole(TrackerType tracker, out HandHoldRole role, bool isLeft)
		{
			role = HandHoldRole.None;
			if (GetHandMotion(tracker, isLeft) != HandMotion.Hold)
				return false;

			if (UseXRData(tracker))
			{
				if (InputDeviceHand.GetHandHoldRole(isLeft, out InputDeviceHand.HandHoldRole xrRole))
				{
					role = xrRole.WaveType();
					return true;
				}
				return false;
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					role = isLeft ?
						(HandHoldRole)m_NaturalHandPoseData.left.hold.role :
						(HandHoldRole)m_NaturalHandPoseData.right.hold.role;

					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					role = isLeft ?
						(HandHoldRole)m_ElectronicHandPoseData.left.hold.role :
						(HandHoldRole)m_ElectronicHandPoseData.right.hold.role;

					return true;
				}
			}

			return false;
		}
		public HandHoldRole GetHandHoldRole(TrackerType tracker, bool isLeft)
		{
			HandHoldRole role = HandHoldRole.None;

			if (GetHandHoldRole(tracker, out HandHoldRole value, isLeft))
				role = value;

			return role;
		}
		public HandHoldRole GetHandHoldRole(TrackerType tracker, HandType hand)
		{
			return GetHandHoldRole(tracker, hand == HandType.Left ? true : false);

		}
		public bool GetHandHoldRole(out HandHoldRole role, bool isLeft)
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
			{
				return GetHandHoldRole(tracker, out role, isLeft);
			}

			role = HandHoldRole.None;
			return false;
		}
		public HandHoldRole GetHandHoldRole(bool isLeft)
		{
			HandHoldRole role = HandHoldRole.None;

			if (GetHandHoldRole(out HandHoldRole value, isLeft))
				role = value;

			return role;
		}
		public bool GetHandHoldRole(TrackerType tracker, out HandHoldRole role, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetHandHoldRole(tracker, out role, isLeft);
		}
		public bool GetHandHoldRole(out HandHoldRole role, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(out timestamp);
			return GetHandHoldRole(out role, isLeft);
		}
		#endregion
		#region Hold Type
		/// <summary> Retrieves the type of equipment hold by the player. </summary>
		public bool GetHandHoldType(TrackerType tracker, out HandHoldType type, bool isLeft)
		{
			type = HandHoldType.None;
			if (GetHandMotion(tracker, isLeft) != HandMotion.Hold)
				return false;

			if (UseXRData(tracker))
			{
				if (InputDeviceHand.GetHandHoldType(isLeft, out InputDeviceHand.HandHoldType xrType))
				{
					type = xrType.WaveType();
					return true;
				}
				return false;
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					type = isLeft ?
						(HandHoldType)m_NaturalHandPoseData.left.hold.type :
						(HandHoldType)m_NaturalHandPoseData.right.hold.type;

					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					type = isLeft ?
						(HandHoldType)m_ElectronicHandPoseData.left.hold.type :
						(HandHoldType)m_ElectronicHandPoseData.right.hold.type;

					return true;
				}
			}

			return false;
		}
		public HandHoldType GetHandHoldType(TrackerType tracker, bool isLeft)
		{
			HandHoldType type = HandHoldType.None;

			if (GetHandHoldType(tracker, out HandHoldType value, isLeft))
				type = value;

			return type;
		}
		public HandHoldType GetHandHoldType(TrackerType tracker, HandType hand)
		{
			return GetHandHoldType(tracker, hand == HandType.Left ? true : false);
		}
		public bool GetHandHoldType(out HandHoldType type, bool isLeft)
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
			{
				return GetHandHoldType(tracker, out type, isLeft);
			}

			type = HandHoldType.None;
			return false;
		}
		public HandHoldType GetHandHoldType(bool isLeft)
		{
			HandHoldType type = HandHoldType.None;

			if (GetHandHoldType(out HandHoldType value, isLeft))
				type = value;

			return type;
		}
		public bool GetHandHoldType(TrackerType tracker, out HandHoldType type, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetHandHoldType(tracker, out type, isLeft);
		}
		public bool GetHandHoldType(out HandHoldType type, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(out timestamp);
			return GetHandHoldType(out type, isLeft);
		}
		#endregion
		#region Pinch Origin
		/// <summary> Retrieves the origin location in world space of Hand Pinch motion. </summary>
		public bool GetPinchOrigin(TrackerType tracker, ref Vector3 origin, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return false;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetPinchOrigin(isLeft, out origin);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						Coordinate.GetVectorFromGL(m_NaturalHandPoseData.left.pinch.origin, out origin);
					else
						Coordinate.GetVectorFromGL(m_NaturalHandPoseData.right.pinch.origin, out origin);

					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						Coordinate.GetVectorFromGL(m_ElectronicHandPoseData.left.pinch.origin, out origin);
					else
						Coordinate.GetVectorFromGL(m_ElectronicHandPoseData.right.pinch.origin, out origin);

					return true;
				}
			}

			return false;
		}
		public bool GetPinchOrigin(TrackerType tracker, ref Vector3 origin, HandType hand)
		{
			return GetPinchOrigin(tracker, ref origin, hand == HandType.Left ? true : false);
		}
		public bool GetPinchOrigin(ref Vector3 origin, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetPinchOrigin(tracker, ref origin, isLeft);
			return false;
		}
		public bool GetPinchOrigin(ref Vector3 origin, HandType hand)
		{
			return GetPinchOrigin(ref origin, hand == HandType.Left ? true : false);
		}
		public bool GetPinchOrigin(TrackerType tracker, out Vector3 origin, bool isLeft, out long timestamp)
		{
			origin = Vector3.zero;
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetPinchOrigin(tracker, ref origin, isLeft);
		}
		public bool GetPinchOrigin(out Vector3 origin, bool isLeft, out long timestamp)
		{
			origin = Vector3.zero;
			GetHandPoseTimestamp(out timestamp);
			return GetPinchOrigin(ref origin, isLeft);
		}
		#endregion
		#region Pinch Direction
		/// <summary> Retrieves the direction vector in world space of Hand Pinch motion. </summary>
		public bool GetPinchDirection(TrackerType tracker, ref Vector3 direction, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return false;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetPinchDirection(isLeft, out direction);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						Coordinate.GetVectorFromGL(m_NaturalHandPoseData.left.pinch.direction, out direction);
					else
						Coordinate.GetVectorFromGL(m_NaturalHandPoseData.right.pinch.direction, out direction);

					return true;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						Coordinate.GetVectorFromGL(m_ElectronicHandPoseData.left.pinch.direction, out direction);
					else
						Coordinate.GetVectorFromGL(m_ElectronicHandPoseData.right.pinch.direction, out direction);

					return true;
				}
			}

			return false;
		}
		public bool GetPinchDirection(TrackerType tracker, ref Vector3 direction, HandType hand)
		{
			return GetPinchDirection(tracker, ref direction, hand == HandType.Left ? true : false);
		}
		public bool GetPinchDirection(ref Vector3 direction, bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetPinchDirection(tracker, ref direction, isLeft);
			return false;
		}
		public bool GetPinchDirection(ref Vector3 direction, HandType hand)
		{
			return GetPinchDirection(ref direction, hand == HandType.Left ? true : false);
		}
		public bool GetPinchDirection(TrackerType tracker, out Vector3 direction, bool isLeft, out long timestamp)
		{
			direction = Vector3.zero;
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetPinchDirection(tracker, ref direction, isLeft);
		}
		public bool GetPinchDirection(out Vector3 direction, bool isLeft, out long timestamp)
		{
			direction = Vector3.zero;
			GetHandPoseTimestamp(out timestamp);
			return GetPinchDirection(ref direction, isLeft);
		}
		#endregion
		#region Pinch Strength
		/// <summary> Retrieves the strength of Hand Pinch motion. </summary>
		public float GetPinchStrength(TrackerType tracker, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return 0;

			if (UseXRData(tracker))
			{
				if (InputDeviceHand.GetPinchStrength(isLeft, out float strength))
					return strength;
				return 0;
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						return m_NaturalHandPoseData.left.pinch.strength;
					else
						return m_NaturalHandPoseData.right.pinch.strength;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						return m_ElectronicHandPoseData.left.pinch.strength;
					else
						return m_ElectronicHandPoseData.right.pinch.strength;
				}
			}

			return 0;
		}
		public float GetPinchStrength(TrackerType tracker, HandType hand)
		{
			return GetPinchStrength(tracker, hand == HandType.Left ? true : false);
		}
		public float GetPinchStrength(bool isLeft)
		{
			TrackerType tracker = TrackerType.Electronic;
			if (GetPreferTracker(ref tracker))
				return GetPinchStrength(tracker, isLeft);
			return 0;
		}
		public float GetPinchStrength(HandType hand)
		{
			return GetPinchStrength(hand == HandType.Left ? true : false);
		}
		public float GetPinchStrength(TrackerType tracker, bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(tracker, out timestamp);
			return GetPinchStrength(tracker, isLeft);
		}
		public float GetPinchStrength(bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(out timestamp);
			return GetPinchStrength(isLeft);
		}
		#endregion

		/// <summary> Retrieves the default threshold of Hand Pinch motion. </summary>
		public bool GetPinchThreshold(TrackerType tracker, out float threshold)
		{
			threshold = 1; // The pinch strength will never > 1

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetPinchThreshold(out threshold);
			}

			if ((tracker == TrackerType.Natural) && hasNaturalTrackerInfo)
			{
				threshold = m_NaturalTrackerInfo.pinchThreshold;
				return true;
			}
			if ((tracker == TrackerType.Electronic) && hasElectronicTrackerInfo)
			{
				threshold = m_ElectronicTrackerInfo.pinchThreshold;
				return true;
			}

			return false;
		}
		public float GetPinchThreshold(TrackerType tracker)
		{
			if (GetPinchThreshold(tracker, out float threshold))
				return threshold;
			return 1; // The pinch strength will never > 1
		}
		public float GetPinchThreshold()
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
				return GetPinchThreshold(tracker);
			return 1; // The pinch strength will never > 1
		}
		/// <summary> Retrieves the default threshold of Hand Pinch motion. </summary>
		public bool GetPinchOffThreshold(TrackerType tracker, out float threshold)
		{
			threshold = 1; // The pinch strength will never > 1

			if (UseXRData(tracker))
			{
				return InputDeviceHand.GetPinchOffThreshold(out threshold);
			}

			if ((tracker == TrackerType.Natural) && hasNaturalTrackerInfo)
			{
				threshold = m_NaturalTrackerInfo.pinchOff;
				return true;
			}
			if ((tracker == TrackerType.Electronic) && hasElectronicTrackerInfo)
			{
				threshold = m_ElectronicTrackerInfo.pinchOff;
				return true;
			}

			return false;
		}
		public float GetPinchOffThreshold(TrackerType tracker)
		{
			if (GetPinchOffThreshold(tracker, out float threshold))
				return threshold;
			return 1; // The pinch strength will never > 1
		}
		public float GetPinchOffThreshold()
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
				return GetPinchOffThreshold(tracker);
			return 1; // The pinch strength will never > 1
		}
		public bool IsHandPinching(TrackerType tracker, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return false;

			if (UseXRData(tracker))
			{
				return InputDeviceHand.IsHandPinching(isLeft);
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						return m_NaturalHandPoseData.left.pinch.isPinching;
					else
						return m_NaturalHandPoseData.right.pinch.isPinching;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						return m_ElectronicHandPoseData.left.pinch.isPinching;
					else
						return m_ElectronicHandPoseData.right.pinch.isPinching;
				}
			}

			return false;
		}
		public bool IsHandPinching(bool isLeft)
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
				return IsHandPinching(tracker, isLeft);
			return false;
		}
		public bool IsHandPinching(HandType hand)
		{
			return IsHandPinching(hand == HandType.Left);
		}
		public bool IsHandPinching(bool isLeft, out long timestamp)
		{
			GetHandPoseTimestamp(out timestamp);
			return IsHandPinching(isLeft);
		}
		public bool IsHandPinching(HandType hand, out long timestamp)
		{
			return IsHandPinching(hand == HandType.Left, out timestamp);
		}

		private bool m_WristPositionFused = false;
		private void CheckWristPositionFusion()
		{
			m_WristPositionFused = Interop.WVR_IsEnhanceHandStable();
			sb.Clear().Append("CheckWristPositionFusion() ").Append(m_WristPositionFused); DEBUG(sb);
		}
		private void OnWristPositionFusionChange(WVR_Event_t systemEvent)
		{
			sb.Clear().Append("OnWristPositionFusionChange()"); DEBUG(sb);
			CheckWristPositionFusion();
		}
		public void FuseWristPositionWithTracker(bool fuse)
		{
			sb.Clear().Append("FuseWristPositionWithTracker() ").Append(fuse); DEBUG(sb);
			Interop.WVR_EnhanceHandStable(fuse);
			CheckWristPositionFusion();
		}
		public bool IsWristPositionFused() { return m_WristPositionFused; }
		#endregion

		private uint m_NaturalHandJointCount = 0;
		private WVR_HandTrackingData_t m_NaturalHandTrackerData = new WVR_HandTrackingData_t();
		private WVR_HandJointData_t m_NaturalHandJointDataLeft = new WVR_HandJointData_t();
		private WVR_Pose_t[] s_NaturalHandJointsPoseLeft;
		private WVR_HandJointData_t m_NaturalHandJointDataRight = new WVR_HandJointData_t();
		private WVR_Pose_t[] s_NaturalHandJointsPoseRight;

		private uint m_ElectronicHandJointCount;
		private WVR_HandTrackingData_t m_ElectronicHandTrackerData = new WVR_HandTrackingData_t();
		private WVR_HandJointData_t m_ElectronicHandJointDataLeft = new WVR_HandJointData_t();
		private WVR_Pose_t[] s_ElectronicHandJointsPoseLeft;
		private WVR_HandJointData_t m_ElectronicHandJointDataRight = new WVR_HandJointData_t();
		private WVR_Pose_t[] s_ElectronicHandJointsPoseRight;
		private HandJointData26 jointData26 = new HandJointData26();

		private void GetHandJointCount(TrackerType tracker)
		{
			if (GetHandTrackerStatus(tracker) != TrackerStatus.Available)
			{
				if (tracker == TrackerType.Natural)
					m_NaturalHandJointCount = 0;
				if (tracker == TrackerType.Electronic)
					m_ElectronicHandJointCount = 0;
				return;
			}

			uint count = 0;
			WVR_Result result = Interop.WVR_GetHandJointCount((WVR_HandTrackerType)tracker, ref count);
			if (tracker == TrackerType.Natural)
			{
				if (result != WVR_Result.WVR_Success)
				{
					m_NaturalHandJointCount = 0;
					return;
				}
				if (m_NaturalHandJointCount != count)
				{
					m_NaturalHandJointCount = count;
					sb.Clear().Append("GetHandJointCount() ").Append(tracker).Append(", count: ").Append(m_NaturalHandJointCount); DEBUG(sb);

					InitializeHandTrackerInfo(ref m_NaturalTrackerInfo, ref s_NaturalHandJoints, ref s_NaturalHandJointsFlag, m_NaturalHandJointCount);
					InitializeHandTrackerData(
						ref m_NaturalHandTrackerData,
						ref m_NaturalHandJointDataLeft,
						ref m_NaturalHandJointDataRight,
						ref s_NaturalHandJointsPoseLeft,
						ref s_NaturalHandJointsPoseRight,
						m_NaturalHandJointCount);

					sb.Clear().Append("GetHandJointCount() ").Append(tracker)
						.Append(", joint count: ").Append(m_NaturalTrackerInfo.jointCount)
						.Append(", right (").Append(m_NaturalHandTrackerData.right.isValidPose).Append(", ").Append(m_NaturalHandTrackerData.right.confidence).Append(", ").Append(m_NaturalHandTrackerData.right.jointCount).Append(", ").Append(m_NaturalHandTrackerData.right.grasp.strength).Append(", ").Append(m_NaturalHandTrackerData.right.grasp.isGrasp).Append(")")
						.Append(", left (").Append(m_NaturalHandTrackerData.left.isValidPose).Append(", ").Append(m_NaturalHandTrackerData.left.confidence).Append(", ").Append(m_NaturalHandTrackerData.left.jointCount).Append(", ").Append(m_NaturalHandTrackerData.left.grasp.strength).Append(", ").Append(m_NaturalHandTrackerData.left.grasp.isGrasp).Append(")");
					DEBUG(sb);
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (result != WVR_Result.WVR_Success)
				{
					m_ElectronicHandJointCount = 0;
					return;
				}
				if (m_ElectronicHandJointCount != count)
				{
					m_ElectronicHandJointCount = count;
					sb.Clear().Append("GetHandJointCount() ").Append(tracker.Name()).Append(", count: ").Append(m_ElectronicHandJointCount); DEBUG(sb);

					InitializeHandTrackerInfo(ref m_ElectronicTrackerInfo, ref s_ElectronicHandJoints, ref s_ElectronicHandJointsFlag, m_ElectronicHandJointCount);
					InitializeHandTrackerData(
						ref m_ElectronicHandTrackerData,
						ref m_ElectronicHandJointDataLeft,
						ref m_ElectronicHandJointDataRight,
						ref s_ElectronicHandJointsPoseLeft,
						ref s_ElectronicHandJointsPoseRight,
						m_ElectronicHandJointCount);

					sb.Clear().Append("GetHandJointCount() ").Append(tracker.Name())
						.Append(", joint count: ").Append(m_ElectronicTrackerInfo.jointCount)
						.Append(", right (").Append(m_ElectronicHandTrackerData.right.isValidPose).Append(", ").Append(m_ElectronicHandTrackerData.right.confidence).Append(", ").Append(m_ElectronicHandTrackerData.right.jointCount).Append(", ").Append(m_ElectronicHandTrackerData.right.grasp.strength).Append(", ").Append(m_ElectronicHandTrackerData.right.grasp.isGrasp).Append(")")
						.Append(", left (").Append(m_ElectronicHandTrackerData.left.isValidPose).Append(", ").Append(m_ElectronicHandTrackerData.left.confidence).Append(", ").Append(m_ElectronicHandTrackerData.left.jointCount).Append(", ").Append(m_ElectronicHandTrackerData.left.grasp.strength).Append(", ").Append(m_ElectronicHandTrackerData.left.grasp.isGrasp).Append(")");
					DEBUG(sb);
				}
			}
		}

		#region Hand Tracker Info
		int[] intJointMappingArray;
		byte[] jointValidFlagArrayBytes;
		private void InitializeHandTrackerInfo(ref WVR_HandTrackerInfo_t handTrackerInfo, ref WVR_HandJoint[] jointMappingArray, ref ulong[] jointValidFlagArray, uint count)
		{
			sb.Clear().Append("InitializeHandTrackerInfo() count: ").Append(count);
			handTrackerInfo.jointCount = count;
			handTrackerInfo.handModelTypeBitMask = 0;

			/// WVR_HandTrackerInfo_t.jointMappingArray
			jointMappingArray = new WVR_HandJoint[count];
			intJointMappingArray = new int[jointMappingArray.Length];
			intJointMappingArray = Array.ConvertAll(jointMappingArray, delegate (WVR_HandJoint value) { return (int)value; });
			handTrackerInfo.jointMappingArray = Marshal.AllocHGlobal(sizeof(int) * intJointMappingArray.Length);
			Marshal.Copy(intJointMappingArray, 0, handTrackerInfo.jointMappingArray, intJointMappingArray.Length);
			/*unsafe
			{
				fixed (WVR_HandJoint* pJointMappingArray = jointMappingArray)
				{
					handTrackerInfo.jointMappingArray = pJointMappingArray;
				}
			}*/

			/// WVR_HandTrackerInfo_t.jointValidFlagArray
			jointValidFlagArray = new ulong[count];
			int jointValidFlagArrayByteLength = Buffer.ByteLength(jointValidFlagArray);
			jointValidFlagArrayBytes = new byte[jointValidFlagArrayByteLength];
			Buffer.BlockCopy(jointValidFlagArray, 0, jointValidFlagArrayBytes, 0, jointValidFlagArrayBytes.Length);

			handTrackerInfo.jointValidFlagArray = Marshal.AllocHGlobal(sizeof(byte) * jointValidFlagArrayBytes.Length);
			Marshal.Copy(jointValidFlagArrayBytes, 0, handTrackerInfo.jointValidFlagArray, jointValidFlagArrayBytes.Length);
			/*unsafe
			{
				fixed (ulong* pHandJointsFlag = jointValidFlagArray)
				{
					handTrackerInfo.jointValidFlagArray = pHandJointsFlag;
				}
			}*/

			handTrackerInfo.pinchThreshold = 0;
		}
		private bool ExtractHandTrackerInfo(WVR_HandTrackerInfo_t handTrackerInfo, ref WVR_HandJoint[] jointMappingArray, ref ulong[] jointValidFlagArray)
		{
			if (handTrackerInfo.jointCount == 0)
			{
				sb.Clear().Append("ExtractHandTrackerInfo() WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount SHOULD NOT be 0!!");
				WARNING(sb);
				return false;
			}

			// WVR_HandTrackerInfo_t.jointMappingArray
			if (jointMappingArray.Length != handTrackerInfo.jointCount)
			{
				sb.Clear().Append("ExtractHandTrackerInfo() The WVR_GetHandJointCount count (jointMappingArray) ").Append(jointMappingArray.Length)
					.Append(" differs from WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount ").Append(handTrackerInfo.jointCount);
				WARNING(sb);

				jointMappingArray = new WVR_HandJoint[handTrackerInfo.jointCount];
				intJointMappingArray = new int[jointMappingArray.Length];
			}

			Marshal.Copy(handTrackerInfo.jointMappingArray, intJointMappingArray, 0, intJointMappingArray.Length);
			jointMappingArray = Array.ConvertAll(intJointMappingArray, delegate (int value) { return (WVR_HandJoint)value; });
			/*unsafe
			{
				for (int i = 0; i < jointMappingArray.Length; i++)
				{
					jointMappingArray[i] = *(handTrackerInfo.jointMappingArray + i);
				}
			}*/

			// WVR_HandTrackerInfo_t.jointValidFlagArray
			if (jointValidFlagArray.Length != handTrackerInfo.jointCount)
			{
				sb.Clear().Append("ExtractHandTrackerInfo() The WVR_GetHandJointCount count (jointValidFlagArray) ").Append(jointValidFlagArray.Length)
					.Append(" differs from WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount ").Append(handTrackerInfo.jointCount);
				WARNING(sb);

				jointValidFlagArray = new ulong[handTrackerInfo.jointCount];
				int jointValidFlagArrayByteLength = Buffer.ByteLength(jointValidFlagArray);
				jointValidFlagArrayBytes = new byte[jointValidFlagArrayByteLength];
			}

			Marshal.Copy(handTrackerInfo.jointValidFlagArray, jointValidFlagArrayBytes, 0, jointValidFlagArrayBytes.Length);
			for (int byteIndex = 0; byteIndex < jointValidFlagArrayBytes.Length; byteIndex = byteIndex+8)
			{
				int i = (byteIndex / 8);
				jointValidFlagArray[i] = BitConverter.ToUInt64(jointValidFlagArrayBytes, byteIndex);
			}
			/*unsafe
			{
				for (int i = 0; i < jointValidFlagArray.Length; i++)
				{
					jointValidFlagArray[i] = *(handTrackerInfo.jointValidFlagArray + i);
				}
			}*/

			return true;
		}

		private bool hasNaturalTrackerInfo = false;
		private WVR_HandTrackerInfo_t m_NaturalTrackerInfo = new WVR_HandTrackerInfo_t();
		private WVR_HandJoint[] s_NaturalHandJoints;
		private ulong[] s_NaturalHandJointsFlag;

		private bool hasElectronicTrackerInfo;
		private WVR_HandTrackerInfo_t m_ElectronicTrackerInfo = new WVR_HandTrackerInfo_t();
		private WVR_HandJoint[] s_ElectronicHandJoints;
		private ulong[] s_ElectronicHandJointsFlag;
		private void GetHandTrackerInfo(TrackerType tracker)
		{
			sb.Clear().Append("GetHandTrackerInfo() ").Append(tracker.Name()); INFO(sb);
			if (tracker == TrackerType.Natural)
			{
				if (GetHandTrackerStatus(TrackerType.Natural) == TrackerStatus.Available && m_NaturalHandJointCount != 0)
				{
					hasNaturalTrackerInfo = Interop.WVR_GetHandTrackerInfo((WVR_HandTrackerType)tracker, ref m_NaturalTrackerInfo) == WVR_Result.WVR_Success ? true : false;
					if (hasNaturalTrackerInfo)
					{
						hasNaturalTrackerInfo = ExtractHandTrackerInfo(m_NaturalTrackerInfo, ref s_NaturalHandJoints, ref s_NaturalHandJointsFlag);
						if (hasNaturalTrackerInfo)
						{
							for (int i = 0; i < m_NaturalTrackerInfo.jointCount; i++)
							{
								sb.Clear().Append("GetHandTrackerInfo() ").Append(tracker.Name())
									.Append(", joint count: ").Append(m_NaturalTrackerInfo.jointCount)
									.Append(", s_NaturalHandJoints[").Append(i).Append("] = ").Append(s_NaturalHandJoints[i])
									.Append(", s_NaturalHandJointsFlag[").Append(i).Append("] = ").Append(s_NaturalHandJointsFlag[i]);
								DEBUG(sb);
							}
						}
					}
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append("GetHandTrackerInfo() Natural, failed!!");
						DEBUG(sb);
					}
					hasNaturalTrackerInfo = false;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (GetHandTrackerStatus(TrackerType.Electronic) == TrackerStatus.Available && m_ElectronicHandJointCount != 0)
				{
					hasElectronicTrackerInfo = Interop.WVR_GetHandTrackerInfo((WVR_HandTrackerType)tracker, ref m_ElectronicTrackerInfo) == WVR_Result.WVR_Success ? true : false;
					if (hasElectronicTrackerInfo)
					{
						hasElectronicTrackerInfo = ExtractHandTrackerInfo(m_ElectronicTrackerInfo, ref s_ElectronicHandJoints, ref s_ElectronicHandJointsFlag);
						if (hasElectronicTrackerInfo)
						{
							for (int i = 0; i < m_ElectronicTrackerInfo.jointCount; i++)
							{
								sb.Clear().Append("GetHandTrackerInfo() ").Append(tracker.Name())
									.Append(", joint count: ").Append(m_ElectronicTrackerInfo.jointCount)
									.Append(", s_ElectronicHandJoints[").Append(i).Append("] = ").Append(s_ElectronicHandJoints[i])
									.Append(", s_ElectronicHandJointsFlag[").Append(i).Append("] = ").Append(s_ElectronicHandJointsFlag[i]);
								DEBUG(sb);
							}
						}
					}
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append("GetHandTrackerInfo() Electronic, failed!!");
						DEBUG(sb);
					}
					hasElectronicTrackerInfo = false;
				}
			}
		}
		#endregion

		#region Hand Tracker Data
		private void InitializeHandJointData(ref WVR_HandJointData_t handJointData, ref WVR_Pose_t[] jointsPose, uint count)
		{
			handJointData.isValidPose = false;
			handJointData.confidence = 0;
			handJointData.grasp.isGrasp = false;
			handJointData.grasp.strength = 0;
			handJointData.jointCount = count;

			WVR_Pose_t wvr_pose_type = default(WVR_Pose_t);
			handJointData.joints = Marshal.AllocHGlobal(Marshal.SizeOf(wvr_pose_type) * (int)count);
			handJointData.scale = new WVR_Vector3f_t() {
				v0 = 1,
				v1 = 1,
				v2 = 1
			};
			handJointData.wristLinearVelocity = new WVR_Vector3f_t()
			{
				v0 = 0,
				v1 = 0,
				v2 = 0
			};
			handJointData.wristAngularVelocity = new WVR_Vector3f_t()
			{
				v0 = 0,
				v1 = 0,
				v2 = 0
			};

			jointsPose = new WVR_Pose_t[count];

			long offset = 0;
			if (IntPtr.Size == 4)
				offset = handJointData.joints.ToInt32();
			else
				offset = handJointData.joints.ToInt64();

			for (int i = 0; i < jointsPose.Length; i++)
			{
				IntPtr wvr_pose_ptr = new IntPtr(offset);
				Marshal.StructureToPtr(jointsPose[i], wvr_pose_ptr, false);
				offset += Marshal.SizeOf(wvr_pose_type);
			}
		}
		private void InitializeHandTrackerData(
			ref WVR_HandTrackingData_t handTrackerData,
			ref WVR_HandJointData_t handJointDataLeft,
			ref WVR_HandJointData_t handJointDataRight,
			ref WVR_Pose_t[] handJointsPoseLeft,
			ref WVR_Pose_t[] handJointsPoseRight,
			uint count
		)
		{
			handTrackerData.timestamp = 0;

			InitializeHandJointData(ref handJointDataLeft, ref handJointsPoseLeft, count);
			handTrackerData.left = handJointDataLeft;

			InitializeHandJointData(ref handJointDataRight, ref handJointsPoseRight, count);
			handTrackerData.right = handJointDataRight;
		}

		private bool ExtractHandJointData(WVR_HandJointData_t handJointData, ref WVR_Pose_t[] jointsPose)
		{
			if (handJointData.jointCount == 0)
			{
				sb.Clear().Append("ExtractHandJointData() WVR_GetHandTrackingData WVR_HandJointData_t jointCount SHOULD NOT be 0!!");
				WARNING(sb);
				return false;
			}

			if (jointsPose.Length != handJointData.jointCount)
			{
				sb.Clear().Append("ExtractHandJointData() The WVR_GetHandJointCount count ").Append(jointsPose.Length)
					.Append(" differs from WVR_GetHandTrackingData WVR_HandJointData_t jointCount ").Append(handJointData.jointCount);
				WARNING(sb);

				jointsPose = new WVR_Pose_t[handJointData.jointCount];
			}

			WVR_Pose_t wvr_pose_type = default(WVR_Pose_t);

			Profiler.BeginSample("Get JointsPose");
			int offset = 0;
			for (int i = 0; i < jointsPose.Length; i++)
			{
				if (IntPtr.Size == 4)
					jointsPose[i] = (WVR_Pose_t)Marshal.PtrToStructure(new IntPtr(handJointData.joints.ToInt32() + offset), typeof(WVR_Pose_t));
				else
					jointsPose[i] = (WVR_Pose_t)Marshal.PtrToStructure(new IntPtr(handJointData.joints.ToInt64() + offset), typeof(WVR_Pose_t));

				offset += Marshal.SizeOf(wvr_pose_type);
			}
			Profiler.EndSample();

			return true;
		}

		private void ExtractHandJointData2(ref WVR_HandJointData_t jd, ref WVR_Pose_t[] poses)
		{
			Profiler.BeginSample("Get JointsPose");
			ConvertHandTrackingDataToUnity(ref jd, ref jointData26);

			poses[0] = jointData26.j00;
			poses[1] = jointData26.j01;
			poses[2] = jointData26.j02;
			poses[3] = jointData26.j03;
			poses[4] = jointData26.j04;
			poses[5] = jointData26.j05;
			poses[6] = jointData26.j06;
			poses[7] = jointData26.j07;
			poses[8] = jointData26.j08;
			poses[9] = jointData26.j09;
			poses[10] = jointData26.j10;
			poses[11] = jointData26.j11;
			poses[12] = jointData26.j12;
			poses[13] = jointData26.j13;
			poses[14] = jointData26.j14;
			poses[15] = jointData26.j15;
			poses[16] = jointData26.j16;
			poses[17] = jointData26.j17;
			poses[18] = jointData26.j18;
			poses[19] = jointData26.j19;
			poses[20] = jointData26.j20;
			poses[21] = jointData26.j21;
			poses[22] = jointData26.j22;
			poses[23] = jointData26.j23;
			poses[24] = jointData26.j24;
			poses[25] = jointData26.j25;
			Profiler.EndSample();
		}

		private bool ExtractHandTrackerData(WVR_HandTrackingData_t handTrackerData, ref WVR_Pose_t[] handJointsPoseLeft, ref WVR_Pose_t[] handJointsPoseRight)
		{
			if (handTrackerData.left.jointCount == 26 && 
				handTrackerData.right.jointCount == 26 &&
				handJointsPoseLeft.Length == 26 &&
				handJointsPoseRight.Length == 26 &&
				ConvertHandTrackingDataToUnity != null)
			{
				// A very fast way to convert data
				ExtractHandJointData2(ref handTrackerData.left, ref handJointsPoseLeft);
				ExtractHandJointData2(ref handTrackerData.right, ref handJointsPoseRight);
				return true;
			}

			if (!ExtractHandJointData(handTrackerData.left, ref handJointsPoseLeft))
				return false;
			if (!ExtractHandJointData(handTrackerData.right, ref handJointsPoseRight))
				return false;

			return true;
		}

		private WVR_PoseOriginModel originModel = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
		private WVR_HandPoseData_t m_NaturalHandPoseData = new WVR_HandPoseData_t(), m_ElectronicHandPoseData = new WVR_HandPoseData_t();
		private bool hasNaturalHandTrackerData = false, hasElectronicHandTrackerData = false;
		private void GetHandTrackingData(TrackerType tracker)
		{
			if (UseXRData(tracker)) { return; }

			if (GetHandTrackerStatus(tracker) != TrackerStatus.Available)
				return;

			ClientInterface.GetOrigin(ref originModel);

			if (tracker == TrackerType.Natural)
			{
				Profiler.BeginSample("Natural Hand");
				WVR_Result result = WVR_Result.WVR_Error_InvalidArgument;
				if (hasNaturalTrackerInfo &&
					m_NaturalTrackerInfo.jointCount != 0 &&
					(m_NaturalTrackerInfo.handModelTypeBitMask & (ulong)WVR_HandModelType.WVR_HandModelType_WithoutController) != 0)
				{
					Profiler.BeginSample("GetHandTrackingData");
					result = Interop.WVR_GetHandTrackingData(
						WVR_HandTrackerType.WVR_HandTrackerType_Natural,
						WVR_HandModelType.WVR_HandModelType_WithoutController,
						originModel,
						ref m_NaturalHandTrackerData,
						ref m_NaturalHandPoseData
					);
					Profiler.EndSample();

					hasNaturalHandTrackerData = result == WVR_Result.WVR_Success ? true : false;
					if (hasNaturalHandTrackerData)
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("GetHandTrackingData() Natural")
								.Append(", timestamp: ").Append(m_NaturalHandTrackerData.timestamp)
								.Append(", left valid? ").Append(m_NaturalHandTrackerData.left.isValidPose)
								.Append(", left confidence: ").Append(m_NaturalHandTrackerData.left.confidence)
								.Append(", left count: ").Append(m_NaturalHandTrackerData.left.jointCount)
								.Append(", left grasp strength: ").Append(m_NaturalHandTrackerData.left.grasp.strength)
								.Append(", left grasp isGrasp: ").Append(m_NaturalHandTrackerData.left.grasp.isGrasp)
								.Append(", right valid? ").Append(m_NaturalHandTrackerData.right.isValidPose)
								.Append(", right confidence: ").Append(m_NaturalHandTrackerData.right.confidence)
								.Append(", right count: ").Append(m_NaturalHandTrackerData.right.jointCount)
								.Append(", right grasp strength: ").Append(m_NaturalHandTrackerData.right.grasp.strength)
								.Append(", right grasp isGrasp: ").Append(m_NaturalHandTrackerData.right.grasp.isGrasp);

							if (m_NaturalHandPoseData.left.state.type == WVR_HandPoseType.WVR_HandPoseType_Pinch)
							{
								sb.Append("GetHandTrackingData() Natural, left pinch ")
									.Append("strength: ").Append(m_NaturalHandPoseData.left.pinch.strength);
							}
							if (m_NaturalHandPoseData.left.state.type == WVR_HandPoseType.WVR_HandPoseType_Hold)
							{
								sb.Append("GetHandTrackingData() Natural, left hold ")
									.Append("role: ").Append(m_NaturalHandPoseData.left.hold.role)
									.Append(", type: ").Append(m_NaturalHandPoseData.left.hold.type);
							}
							if (m_NaturalHandPoseData.right.state.type == WVR_HandPoseType.WVR_HandPoseType_Pinch)
							{
								sb.Append("GetHandTrackingData() Natural, right pinch ")
									.Append("strength: ").Append(m_NaturalHandPoseData.right.pinch.strength);
							}
							if (m_NaturalHandPoseData.right.state.type == WVR_HandPoseType.WVR_HandPoseType_Hold)
							{
								sb.Append("GetHandTrackingData() Natural, right hold ")
									.Append("role: ").Append(m_NaturalHandPoseData.right.hold.role)
									.Append(", type: ").Append(m_NaturalHandPoseData.right.hold.type);
							}

							DEBUG(sb);
						}

						hasNaturalHandTrackerData = ExtractHandTrackerData(m_NaturalHandTrackerData, ref s_NaturalHandJointsPoseLeft, ref s_NaturalHandJointsPoseRight);
						if (hasNaturalHandTrackerData)
						{
							m_NaturalHandJointDataLeft = m_NaturalHandTrackerData.left;
							m_NaturalHandJointDataRight = m_NaturalHandTrackerData.right;
						}
					}
					else
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("GetHandTrackingData() Natural " + result);
							DEBUG(sb);
						}

						m_NaturalHandTrackerData.left.isValidPose = false;
						m_NaturalHandTrackerData.left.confidence = 0;
						m_NaturalHandTrackerData.left.grasp.isGrasp = false;
						m_NaturalHandTrackerData.left.grasp.strength = 0;
						m_NaturalHandTrackerData.right.isValidPose = false;
						m_NaturalHandTrackerData.right.confidence = 0;
						m_NaturalHandTrackerData.right.grasp.isGrasp = false;
						m_NaturalHandTrackerData.right.grasp.strength = 0;
					}
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append("GetHandTrackingData() Natural, hasNaturalTrackerInfo: ").Append(hasNaturalTrackerInfo)
							.Append(", jointCount: ").Append(m_NaturalTrackerInfo.jointCount)
							.Append(", handModelTypeBitMask: ").Append(m_NaturalTrackerInfo.handModelTypeBitMask);
						DEBUG(sb);
					}
					hasNaturalHandTrackerData = false;
				}
				Profiler.EndSample();
			}
			if (tracker == TrackerType.Electronic)
			{
				Profiler.BeginSample("Natural Hand");
				switch (m_TrackerOptions.Electronic.Model)
				{
					case HandModel.WithController:
						if ((m_ElectronicTrackerInfo.handModelTypeBitMask & (ulong)m_TrackerOptions.Electronic.Model) == 0)
							m_TrackerOptions.Electronic.Model = HandModel.WithoutController;
						break;
					case HandModel.WithoutController:
						if ((m_ElectronicTrackerInfo.handModelTypeBitMask & (ulong)m_TrackerOptions.Electronic.Model) == 0)
							m_TrackerOptions.Electronic.Model = HandModel.WithController;
						break;
					default:
						break;
				}

				WVR_Result result = WVR_Result.WVR_Error_InvalidArgument;
				if (hasElectronicTrackerInfo &&
					m_ElectronicTrackerInfo.jointCount != 0 &&
					(m_ElectronicTrackerInfo.handModelTypeBitMask & (ulong)m_TrackerOptions.Electronic.Model) != 0)
				{
					result = Interop.WVR_GetHandTrackingData(
						WVR_HandTrackerType.WVR_HandTrackerType_Electronic,
						(WVR_HandModelType)m_TrackerOptions.Electronic.Model,
						originModel,
						ref m_ElectronicHandTrackerData,
						ref m_ElectronicHandPoseData
					);

					hasElectronicHandTrackerData = result == WVR_Result.WVR_Success ? true : false;
					if (hasElectronicHandTrackerData)
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("GetHandTrackingData() Electronic")
								.Append(", timestamp: ").Append(m_ElectronicHandTrackerData.timestamp)
								.Append(", left valid? ").Append(m_ElectronicHandTrackerData.left.isValidPose)
								.Append(", left confidence: ").Append(m_ElectronicHandTrackerData.left.confidence)
								.Append(", left count: ").Append(m_ElectronicHandTrackerData.left.jointCount)
								.Append(", left grasp strength: ").Append(m_ElectronicHandTrackerData.left.grasp.strength)
								.Append(", left grasp isGrasp: ").Append(m_ElectronicHandTrackerData.left.grasp.isGrasp)
								.Append(", right valid? ").Append(m_ElectronicHandTrackerData.right.isValidPose)
								.Append(", right confidence: ").Append(m_ElectronicHandTrackerData.right.confidence)
								.Append(", right count: ").Append(m_ElectronicHandTrackerData.right.jointCount)
								.Append(", right grasp strength: ").Append(m_ElectronicHandTrackerData.right.grasp.strength)
								.Append(", right grasp isGrasp: ").Append(m_ElectronicHandTrackerData.right.grasp.isGrasp);
							DEBUG(sb);
						}

						hasElectronicHandTrackerData = ExtractHandTrackerData(m_ElectronicHandTrackerData, ref s_ElectronicHandJointsPoseLeft, ref s_ElectronicHandJointsPoseRight);
						if (hasElectronicHandTrackerData)
						{
							m_ElectronicHandJointDataLeft = m_ElectronicHandTrackerData.left;
							m_ElectronicHandJointDataRight = m_ElectronicHandTrackerData.right;
						}
					}
					else
					{
						if (printIntervalLog)
						{
							sb.Clear().Append("GetHandTrackingData() Electronic " + result);
							DEBUG(sb);
						}

						m_ElectronicHandTrackerData.left.isValidPose = false;
						m_ElectronicHandTrackerData.left.confidence = 0;
						m_ElectronicHandTrackerData.left.grasp.isGrasp = false;
						m_ElectronicHandTrackerData.left.grasp.strength = 0;
						m_ElectronicHandTrackerData.right.isValidPose = false;
						m_ElectronicHandTrackerData.right.confidence = 0;
						m_ElectronicHandTrackerData.right.grasp.isGrasp = false;
						m_ElectronicHandTrackerData.right.grasp.strength = 0;
					}
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append("GetHandTrackingData() Electronic, hasNaturalTrackerInfo: ").Append(hasNaturalTrackerInfo)
							.Append(", jointCount: ").Append(m_ElectronicTrackerInfo.jointCount)
							.Append(", handModelTypeBitMask: ").Append(m_ElectronicTrackerInfo.handModelTypeBitMask);
						DEBUG(sb);
					}
					hasElectronicHandTrackerData = false;
				}
				Profiler.EndSample();
			}
		}
		#endregion
	}

	public static class HandManagerHelper
	{
		public static HandManager.GestureType GetGestureType(this WVR_HandGestureType type)
		{
			switch (type)
			{
				case WVR_HandGestureType.WVR_HandGestureType_Unknown:
					return HandManager.GestureType.Unknown;
				case WVR_HandGestureType.WVR_HandGestureType_Fist:
					return HandManager.GestureType.Fist;
				case WVR_HandGestureType.WVR_HandGestureType_Five:
					return HandManager.GestureType.Five;
				case WVR_HandGestureType.WVR_HandGestureType_OK:
					return HandManager.GestureType.OK;
				case WVR_HandGestureType.WVR_HandGestureType_ThumbUp:
					return HandManager.GestureType.ThumbUp;
				case WVR_HandGestureType.WVR_HandGestureType_IndexUp:
					return HandManager.GestureType.IndexUp;
				case WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch:
					return HandManager.GestureType.Palm_Pinch;
				case WVR_HandGestureType.WVR_HandGestureType_Yeah:
					return HandManager.GestureType.Yeah;
				default:
					break;
			}
			return HandManager.GestureType.Invalid;
		}
		public static string Name(this HandManager.GestureType type)
		{
			switch(type)
			{
				case HandManager.GestureType.Unknown:
					return "Unknown";
				case HandManager.GestureType.Fist:
					return "Fist";
				case HandManager.GestureType.Five:
					return "Five";
				case HandManager.GestureType.OK:
					return "OK";
				case HandManager.GestureType.ThumbUp:
					return "ThumbUp";
				case HandManager.GestureType.IndexUp:
					return "IndexUp";
				case HandManager.GestureType.Palm_Pinch:
					return "Palm_Pinch";
				case HandManager.GestureType.Yeah:
					return "Yeah";
				default:
					break;
			}

			return "Invalid";
		}
		public static HandFinger Finger(this HandManager.HandJoint joint)
		{
			switch(joint)
			{
				case HandManager.HandJoint.Thumb_Joint0:
				case HandManager.HandJoint.Thumb_Joint1:
				case HandManager.HandJoint.Thumb_Joint2:
				case HandManager.HandJoint.Thumb_Tip:
					return HandFinger.Thumb;
				case HandManager.HandJoint.Index_Joint0:
				case HandManager.HandJoint.Index_Joint1:
				case HandManager.HandJoint.Index_Joint2:
				case HandManager.HandJoint.Index_Joint3:
				case HandManager.HandJoint.Index_Tip:
					return HandFinger.Index;
				case HandManager.HandJoint.Middle_Joint0:
				case HandManager.HandJoint.Middle_Joint1:
				case HandManager.HandJoint.Middle_Joint2:
				case HandManager.HandJoint.Middle_Joint3:
				case HandManager.HandJoint.Middle_Tip:
					return HandFinger.Middle;
				case HandManager.HandJoint.Ring_Joint0:
				case HandManager.HandJoint.Ring_Joint1:
				case HandManager.HandJoint.Ring_Joint2:
				case HandManager.HandJoint.Ring_Joint3:
				case HandManager.HandJoint.Ring_Tip:
					return HandFinger.Ring;
				case HandManager.HandJoint.Pinky_Joint0:
				case HandManager.HandJoint.Pinky_Joint1:
				case HandManager.HandJoint.Pinky_Joint2:
				case HandManager.HandJoint.Pinky_Joint3:
				case HandManager.HandJoint.Pinky_Tip:
					return HandFinger.Pinky;
				default:
					break;
			}
			return HandFinger.Thumb;
		}
		/// <summary> Retrieves the joint index of a finger. -1 means the joint does NOT belong to a finger.  </summary>
		public static int Index(this HandManager.HandJoint joint)
		{
			switch (joint)
			{
				case HandManager.HandJoint.Thumb_Joint0:
				case HandManager.HandJoint.Middle_Joint0:
				case HandManager.HandJoint.Index_Joint0:
				case HandManager.HandJoint.Ring_Joint0:
				case HandManager.HandJoint.Pinky_Joint0:
					return 0;
				case HandManager.HandJoint.Thumb_Joint1:
				case HandManager.HandJoint.Index_Joint1:
				case HandManager.HandJoint.Middle_Joint1:
				case HandManager.HandJoint.Ring_Joint1:
				case HandManager.HandJoint.Pinky_Joint1:
					return 1;
				case HandManager.HandJoint.Thumb_Joint2:
				case HandManager.HandJoint.Index_Joint2:
				case HandManager.HandJoint.Middle_Joint2:
				case HandManager.HandJoint.Ring_Joint2:
				case HandManager.HandJoint.Pinky_Joint2:
					return 2;
				case HandManager.HandJoint.Thumb_Tip:
				case HandManager.HandJoint.Index_Joint3:
				case HandManager.HandJoint.Middle_Joint3:
				case HandManager.HandJoint.Ring_Joint3:
				case HandManager.HandJoint.Pinky_Joint3:
					return 3;
				case HandManager.HandJoint.Index_Tip:
				case HandManager.HandJoint.Middle_Tip:
				case HandManager.HandJoint.Ring_Tip:
				case HandManager.HandJoint.Pinky_Tip:
					return 4;
				default:
					break;
			}
			return -1;
		}
		public static string Name(this HandManager.HandJoint joint)
		{
			switch (joint)
			{
				case HandManager.HandJoint.Thumb_Joint0:
					return "Thumb_Joint0";
				case HandManager.HandJoint.Middle_Joint0:
					return "Middle_Joint0";
				case HandManager.HandJoint.Index_Joint0:
					return "Index_Joint0";
				case HandManager.HandJoint.Ring_Joint0:
					return "Ring_Joint0";
				case HandManager.HandJoint.Pinky_Joint0:
					return "Pinky_Joint0";
				case HandManager.HandJoint.Thumb_Joint1:
					return "Thumb_Joint1";
				case HandManager.HandJoint.Index_Joint1:
					return "Index_Joint1";
				case HandManager.HandJoint.Middle_Joint1:
					return "Middle_Joint1";
				case HandManager.HandJoint.Ring_Joint1:
					return "Ring_Joint1";
				case HandManager.HandJoint.Pinky_Joint1:
					return "Pinky_Joint1";
				case HandManager.HandJoint.Thumb_Joint2:
					return "Thumb_Joint2";
				case HandManager.HandJoint.Index_Joint2:
					return "Index_Joint2";
				case HandManager.HandJoint.Middle_Joint2:
					return "Middle_Joint2";
				case HandManager.HandJoint.Ring_Joint2:
					return "Ring_Joint2";
				case HandManager.HandJoint.Pinky_Joint2:
					return "Pinky_Joint2";
				case HandManager.HandJoint.Thumb_Tip:
					return "Thumb_Tip";
				case HandManager.HandJoint.Index_Joint3:
					return "Index_Joint3";
				case HandManager.HandJoint.Middle_Joint3:
					return "Middle_Joint3";
				case HandManager.HandJoint.Ring_Joint3:
					return "Pinky_Joint3";
				case HandManager.HandJoint.Pinky_Joint3:
					return "Pinky_Joint3";
				case HandManager.HandJoint.Index_Tip:
					return "Index_Tip";
				case HandManager.HandJoint.Middle_Tip:
					return "Middle_Tip";
				case HandManager.HandJoint.Ring_Tip:
					return "Ring_Tip";
				case HandManager.HandJoint.Pinky_Tip:
					return "Pinky_Tip";
				default:
					break;
			}
			return "";
		}
		public static string Name(this HandManager.TrackerType type)
		{
			if (type == HandManager.TrackerType.Electronic) { return "Electronic"; }
			return "Natural";
		}
		public static string Name(this HandManager.TrackerSelector tracker)
		{
			if (tracker == HandManager.TrackerSelector.ElectronicPrior) { return "ElectronicPrior"; }
			return "NaturalPrior";
		}
		public static string Name(this HandManager.HandModel model)
		{
			if (model == HandManager.HandModel.WithController) { return "WithController"; }
			return "WithoutController";
		}
		public static string Name(this HandManager.GestureStatus status)
		{
			if (status == HandManager.GestureStatus.Available) { return "AVAILABLE"; }
			if (status == HandManager.GestureStatus.NotStart) { return "Not Started"; }
			if (status == HandManager.GestureStatus.StartFailure) { return "Start Failed"; }
			if (status == HandManager.GestureStatus.Starting) { return "Starting"; }
			if (status == HandManager.GestureStatus.Stopping) { return "Stopping"; }

			return "No Support";
		}
		public static string Name(this HandManager.TrackerStatus status)
		{
			if (status == HandManager.TrackerStatus.Available) { return "Available"; }
			if (status == HandManager.TrackerStatus.NotStart) { return "Not Started"; }
			if (status == HandManager.TrackerStatus.StartFailure) { return "Start Failed"; }
			if (status == HandManager.TrackerStatus.Starting) { return "Starting"; }
			if (status == HandManager.TrackerStatus.Stopping) { return "Stopping"; }
			return "No Support";
		}
		public static string Name(this HandManager.HandType type)
		{
			if (type == HandManager.HandType.Left) { return "Left"; }
			if (type == HandManager.HandType.Right) { return "Right"; }
			return "";
		}

		public static HandManager.HandMotion WaveType(this InputDeviceHand.HandMotion type)
		{
			if (type == InputDeviceHand.HandMotion.Hold) { return HandManager.HandMotion.Hold; }
			if (type == InputDeviceHand.HandMotion.Pinch) { return HandManager.HandMotion.Pinch; }

			return HandManager.HandMotion.None;
		}
		public static HandManager.HandHoldRole WaveType(this InputDeviceHand.HandHoldRole type)
		{
			if (type == InputDeviceHand.HandHoldRole.Main) { return HandManager.HandHoldRole.Main; }
			if (type == InputDeviceHand.HandHoldRole.Side) { return HandManager.HandHoldRole.Side; }

			return HandManager.HandHoldRole.None;
		}
		public static HandManager.HandHoldType WaveType(this InputDeviceHand.HandHoldType type)
		{
			if (type == InputDeviceHand.HandHoldType.Baton) { return HandManager.HandHoldType.Baton; }
			if (type == InputDeviceHand.HandHoldType.FlashLight) { return HandManager.HandHoldType.FlashLight; }
			if (type == InputDeviceHand.HandHoldType.Gun) { return HandManager.HandHoldType.Gun; }
			if (type == InputDeviceHand.HandHoldType.LongGun) { return HandManager.HandHoldType.LongGun; }
			if (type == InputDeviceHand.HandHoldType.OCSpray) { return HandManager.HandHoldType.OCSpray; }

			return HandManager.HandHoldType.None;
		}
	}
}
