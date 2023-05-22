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
using System.Diagnostics;
using UnityEngine.Profiling;

using Wave.OpenXR;

namespace Wave.Essence.Hand
{
	[DisallowMultipleComponent]
	public sealed class HandManager : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Hand.HandManager";
		private void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}
		bool printIntervalLog = false;
		int logFrame = 0;
		private void INTERVAL(string msg) { if (printIntervalLog) { DEBUG(msg); } }
		private void INFO(string msg) { Log.i(LOG_TAG, msg, true); }
		private void WARNING(string msg) { Log.w(LOG_TAG, msg, true); }

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
			[HideInInspector]
			[Obsolete("This variable is deprecated. Use StartHandTracker() or StopHandTracker() to enable/disable the Hand Tracker component.")]
			public bool Enable = true;
			public bool InitialStart = false;
		}
		[Serializable]
		public class ElectronicTrackerOption
		{
			[HideInInspector]
			[Obsolete("This variable is deprecated. Use StartHandTracker() or StopHandTracker() to enable/disable the Hand Tracker component.")]
			public bool Enable = true;
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

		#region Wave XR Constants
		const string kHandConfidenceLeft = "HandConfidenceLeft";
		const string kHandConfidenceRight = "HandConfidenceRight";
		const string kHandScaleLeftX = "HandScaleLeftX", kHandScaleLeftY = "HandScaleLeftY", kHandScaleLeftZ = "HandScaleLeftZ";
		const string kHandScaleRightX = "HandScaleRightX", kHandScaleRightY = "HandScaleRightY", kHandScaleRightZ = "HandScaleRightZ";
		const string kWristLinearVelocityLeftX = "WristLinearVelocityLeftX", kWristLinearVelocityLeftY = "WristLinearVelocityLeftY", kWristLinearVelocityLeftZ = "WristLinearVelocityLeftZ";
		const string kWristLinearVelocityRightX = "WristLinearVelocityRightX", kWristLinearVelocityRightY = "WristLinearVelocityRightY", kWristLinearVelocityRightZ = "WristLinearVelocityRightZ";
		const string kWristAngularVelocityLeftX = "WristAngularVelocityLeftX", kWristAngularVelocityLeftY = "WristAngularVelocityLeftY", kWristAngularVelocityLeftZ = "WristAngularVelocityLeftZ";
		const string kWristAngularVelocityRightX = "WristAngularVelocityRightX", kWristAngularVelocityRightY = "WristAngularVelocityRightY", kWristAngularVelocityRightZ = "WristAngularVelocityRightZ";
		const string kHandMotionLeft = "HandMotionLeft";
		const string kHandMotionRight = "HandMotionRight";
		const string kHandRoleLeft = "HandRoleLeft";
		const string kHandRoleRight = "HandRoleRight";
		const string kHandObjectLeft = "HandObjectLeft";
		const string kHandObjectRight = "HandObjectRight";
		const string kHandOriginLeftX = "HandOriginLeftX", kHandOriginLeftY = "HandOriginLeftY", kHandOriginLeftZ = "HandOriginLeftZ";
		const string kHandOriginRightX = "HandOriginRightX", kHandOriginRightY = "HandOriginRightY", kHandOriginRightZ = "HandOriginRightZ";
		const string kHandDirectionLeftX = "HandDirectionLeftX", kHandDirectionLeftY = "HandDirectionLeftY", kHandDirectionLeftZ = "HandDirectionLeftZ";
		const string kHandDirectionRightX = "HandDirectionRightX", kHandDirectionRightY = "HandDirectionRightY", kHandDirectionRightZ = "HandDirectionRightZ";
		const string kHandStrengthLeft = "HandStrengthLeft";
		const string kHandStrengthRight = "HandStrengthRight";
		const string kHandPinchThreshold = "HandPinchThreshold";
		#endregion

		void Awake()
		{
			DEBUG("Awake()");
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
				DEBUG("Start() Starts hand gesture.");
				StartHandGesture();
			}

			if (m_TrackerOptions.Natural.InitialStart)
			{
				DEBUG("Start() Starts the natural hand tracker.");
				StartHandTracker(TrackerType.Natural);
			}
			if (m_TrackerOptions.Electronic.InitialStart)
			{
				DEBUG("Start() Starts the electronic hand tracker.");
				StartHandTracker(TrackerType.Electronic);
			}

			DEBUG("Start() CheckWristPositionFusion");
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

			INTERVAL("Update() Interaction Mode: " + interactionMode
				+ ", use xr device: " + m_UseXRDevice
				+ ", use xr data (natural): " + UseXRData(TrackerType.Natural)
				+ ", use xr data (electronic): " + UseXRData(TrackerType.Electronic)
				+ ", gesture value: " + m_GestureOptions.Gesture.optionValue
				+ ", gesture ref count: " + refCountGesture
				+ ", tracker: " + m_TrackerOptions.Tracker
				+ ", natural ref count: " + refCountNatural
				+ ", natural joint: " + m_NaturalHandJointCount
				+ ", electronic ref count: " + refCountElectronic
				+ ", electronic model: " + m_TrackerOptions.Electronic.Model
				+ ", electronic joint: " + m_ElectronicHandJointCount);
		}
		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				DEBUG("OnApplicationPause() Resume, CheckWristPositionFusion");
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
				return;

			SetHandGestureStatus(GestureStatus.Starting);
			DEBUG("StartHandGestureLock() " + m_GestureOptionValue);
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
			DEBUG("StartHandGestureLock() " + result + ", status: " + status);
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
				DEBUG("StartHandGestureThread()");
				StartHandGestureLock();
			}
		}
		public void StartHandGesture()
		{
			string caller = "TBD";
			var frame = new StackFrame(1, true);
			if (frame != null)
			{
				var method = frame.GetMethod();
				if (method != null)
					caller = method.Name;
				else
					caller = "No method.";
			}
			refCountGesture++;
			INFO("StartHandGesture() (" + refCountNatural + ") from " + caller);

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
				DEBUG("StopHandGestureLock()");
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
				DEBUG("StopHandGestureThread()");
				StopHandGestureLock();
			}
		}
		public void StopHandGesture()
		{
			string caller = "TBD";
			var frame = new StackFrame(1, true);
			if (frame != null)
			{
				var method = frame.GetMethod();
				if (method != null)
					caller = method.Name;
				else
					caller = "No method.";
			}
			refCountGesture = refCountGesture > 0 ? refCountGesture - 1 : 0;
			INFO("StopHandGesture() (" + refCountGesture + ") from " + caller);
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
				DEBUG("RestartHandGestureThread()");
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
				DEBUG("UpdateHandGestureType() Receives " + m_HandGestureLeft);
				GeneralEvent.Send(HAND_STATIC_GESTURE, HandType.Left, m_HandGestureLeft);
#pragma warning disable 0618
				GeneralEvent.Send(HAND_STATIC_GESTURE_LEFT, (WVR_HandGestureType)m_HandGestureLeft);
#pragma warning restore 0618
			}

			m_HandGestureRightEx = m_HandGestureRight;
			m_HandGestureRight = handGestureData.right.GetGestureType();

			if (m_HandGestureRight != m_HandGestureRightEx)
			{
				DEBUG("UpdateHandGestureType() Receives " + m_HandGestureRight);
				GeneralEvent.Send(HAND_STATIC_GESTURE, HandType.Right, m_HandGestureRight);
#pragma warning disable 0618
				GeneralEvent.Send(HAND_STATIC_GESTURE_RIGHT, (WVR_HandGestureType)m_HandGestureRight);
#pragma warning restore 0618
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
					DEBUG("CanStartHandTracker() status: " + status);
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
			DEBUG("CanStopHandTracker() status:" + status);
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
				DEBUG("StartHandTrackerLock() XR " + tracker);
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
			DEBUG("StartHandTrackerLock() " + tracker + ", " + result + ", status: " + status);
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
				DEBUG("StartHandTrackerThread() " + (TrackerType)tracker);
				StartHandTrackerLock((TrackerType)tracker);
			}
		}
		public void StartHandTracker(TrackerType tracker)
		{
			if (!CanStartHandTracker(tracker))
				return;

			string caller = "TBD";
			var frame = new StackFrame(1, true);
			if (frame != null)
			{
				var method = frame.GetMethod();
				if (method != null)
					caller = method.Name;
				else
					caller = "No method.";
			}
			if (tracker == TrackerType.Natural)
			{
				refCountNatural++;
				INFO("StartHandTracker() " + tracker + "(" + refCountNatural + ") from " + caller);
			}
			if (tracker == TrackerType.Electronic)
			{
				refCountElectronic++;
				INFO("StartHandTracker() " + tracker + "(" + refCountElectronic + ") from " + caller);
			}

			INFO("StartHandTracker() " + tracker);
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
				DEBUG("StopHandTrackerLock() XR " + tracker);
				return;
			}

			DEBUG("StopHandTrackerLock() " + tracker);
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
				DEBUG("StopHandTrackerThread() " + (TrackerType)tracker);
				StopHandTrackerLock((TrackerType)tracker);
			}
		}
		public void StopHandTracker(TrackerType tracker)
		{
			if (!CanStopHandTracker(tracker))
				return;

			string caller = "TBD";
			var frame = new StackFrame(1, true);
			if (frame != null)
			{
				var method = frame.GetMethod();
				if (method != null)
					caller = method.Name;
				else
					caller = "No method.";
			}
			if (tracker == TrackerType.Natural)
			{
				refCountNatural = refCountNatural > 0 ? refCountNatural - 1 : 0;
				INFO("StopHandTracker() " + tracker + "(" + refCountNatural + ") from " + caller);
				if (refCountNatural > 0) return;
			}
			if (tracker == TrackerType.Electronic)
			{
				refCountElectronic = refCountElectronic > 0 ? refCountElectronic - 1 : 0;
				INFO("StopHandTracker() " + tracker + "(" + refCountElectronic + ") from " + caller);
				if (refCountElectronic > 0) return;
			}

			INFO("StopHandTracker() " + tracker);
			Thread hand_tracker_t = new Thread(StopHandTrackerThread);
			hand_tracker_t.Name = "StopHandTrackerThread";
			hand_tracker_t.Start(tracker);
		}

		private void RestartHandTrackerThread(object tracker)
		{
			lock (handTrackerThreadLocker)
			{
				DEBUG("RestartHandTrackerThread() " + (TrackerType)tracker);
				if (UseXRData((TrackerType)tracker))
				{
					StopHandTrackerLock((TrackerType)tracker);

					uint waitCount = 0;
					TrackerStatus status = GetHandTrackerStatus((TrackerType)tracker);
					while (status != TrackerStatus.NotStart && waitCount <= 5)
					{
						DEBUG("RestartHandTrackerThread() status: " + status + ", wait 1s.");
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
			INFO("RestartHandTracker() " + tracker);
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

		/// <summary>
		/// Retrieves left/right hand's confidence of a <see cref="TrackerType">tracker</see>.
		/// </summary>
		/// <param name="tracker">Natural or electronic tracker.</param>
		/// <param name="isLeft">True for left hand.</param>
		/// <returns>A float {0, 1} value where 1 means the most reliable.</returns>
		public float GetHandConfidence(TrackerType tracker, bool isLeft)
		{
			if (UseXRData(tracker))
			{
				float confidence = 0;
				if (isLeft)
					SettingsHelper.GetFloat(kHandConfidenceLeft, ref confidence);
				else
					SettingsHelper.GetFloat(kHandConfidenceRight, ref confidence);
				return confidence;
			}

			if (tracker == TrackerType.Natural)
			{
				if (hasNaturalHandTrackerData && hasNaturalTrackerInfo)
				{
					if (isLeft)
						return m_NaturalHandTrackerData.left.confidence;
					else
						return m_NaturalHandTrackerData.right.confidence;
				}
			}
			if (tracker == TrackerType.Electronic)
			{
				if (hasElectronicHandTrackerData && hasElectronicTrackerInfo)
				{
					if (isLeft)
						return m_ElectronicHandTrackerData.left.confidence;
					else
						return m_ElectronicHandTrackerData.right.confidence;
				}
			}

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

		/// <summary> @position will not be updated when no position. </summary>
		public bool GetJointPosition(TrackerType tracker, HandJoint joint, ref Vector3 position, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
			{
				INTERVAL("GetJointPosition() tracker " + tracker + (isLeft ? " Left" : " Right") + " joint " + joint + " has invalid pose.");
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

			INTERVAL("GetJointPosition() "
				+ "tracker: " + tracker
				+ ", " + (isLeft ? "Left" : "Right") + " joint " + joint
				+ ", position {" + position.x.ToString() + ", " + position.y.ToString() + ", " + position.z.ToString() + ")");

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

		/// <summary> @rotation will not be updated when no rotation. </summary>
		public bool GetJointRotation(TrackerType tracker, HandJoint joint, ref Quaternion rotation, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
			{
				INTERVAL("GetJointRotation() tracker " + tracker + (isLeft ? " Left" : " Right") + " joint " + joint + " has invalid pose.");
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

			INTERVAL("GetJointRotation()"
				+ " tracker: " + tracker
				+ ", " + (isLeft ? "Left" : "Right") + " joint " + joint
				+ ", rotation {" + rotation.x.ToString() + ", " + rotation.y.ToString() + ", " + rotation.z.ToString() + ", " + rotation.w.ToString() + ")");

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

		/// <summary> @scale will not be updated when no scale. </summary>
		public bool GetHandScale(TrackerType tracker, ref Vector3 scale, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			if (UseXRData(tracker))
			{
				float scale_x = 0, scale_y = 0, scale_z = 0;
				if (isLeft)
				{
					SettingsHelper.GetFloat(kHandScaleLeftX, ref scale_x);
					SettingsHelper.GetFloat(kHandScaleLeftY, ref scale_y);
					SettingsHelper.GetFloat(kHandScaleLeftZ, ref scale_z);
				}
				else
				{
					SettingsHelper.GetFloat(kHandScaleRightX, ref scale_x);
					SettingsHelper.GetFloat(kHandScaleRightY, ref scale_y);
					SettingsHelper.GetFloat(kHandScaleRightZ, ref scale_z);
				}
				scale.x = scale_x;
				scale.y = scale_y;
				scale.z = scale_z;
				return true;
			}

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

			INTERVAL("GetHandScale()"
				+ " tracker: " + tracker
				+ ", " + (isLeft ? "Left" : "Right")
				+ ", scale {" + scale.x.ToString() + ", " + scale.y.ToString() + ", " + scale.z.ToString() + ")");

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

		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristLinearVelocity(TrackerType tracker, ref Vector3 velocity, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			if (UseXRData(tracker))
			{
				float velocity_x = 0, velocity_y = 0, velocity_z = 0;
				if (isLeft)
				{
					SettingsHelper.GetFloat(kWristLinearVelocityLeftX, ref velocity_x);
					SettingsHelper.GetFloat(kWristLinearVelocityLeftY, ref velocity_y);
					SettingsHelper.GetFloat(kWristLinearVelocityLeftZ, ref velocity_z);
				}
				else
				{
					SettingsHelper.GetFloat(kWristLinearVelocityRightX, ref velocity_x);
					SettingsHelper.GetFloat(kWristLinearVelocityRightY, ref velocity_y);
					SettingsHelper.GetFloat(kWristLinearVelocityRightZ, ref velocity_z);
				}
				velocity.x = velocity_x;
				velocity.y = velocity_y;
				velocity.z = velocity_z;
				return true;
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

			INTERVAL("GetWristLinearVelocity()"
				+ " tracker: " + tracker
				+ ", " + (isLeft ? "Left" : "Right")
				+ ", velocity {" + velocity.x.ToString() + ", " + velocity.y.ToString() + ", " + velocity.z.ToString() + ")");

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

		/// <summary> @velocity will not be updated when no velocity. </summary>
		public bool GetWristAngularVelocity(TrackerType tracker, ref Vector3 velocity, bool isLeft)
		{
			if (!IsHandPoseValid(tracker, isLeft))
				return false;

			if (UseXRData(tracker))
			{
				float velocity_x = 0, velocity_y = 0, velocity_z = 0;
				if (isLeft)
				{
					SettingsHelper.GetFloat(kWristAngularVelocityLeftX, ref velocity_x);
					SettingsHelper.GetFloat(kWristAngularVelocityLeftY, ref velocity_y);
					SettingsHelper.GetFloat(kWristAngularVelocityLeftZ, ref velocity_z);
				}
				else
				{
					SettingsHelper.GetFloat(kWristAngularVelocityRightX, ref velocity_x);
					SettingsHelper.GetFloat(kWristAngularVelocityRightY, ref velocity_y);
					SettingsHelper.GetFloat(kWristAngularVelocityRightZ, ref velocity_z);
				}
				velocity.x = velocity_x;
				velocity.y = velocity_y;
				velocity.z = velocity_z;
				return true;
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

			INTERVAL("GetWristAngularVelocity()"
					+ " tracker: " + tracker
					+ ", " + (isLeft ? "Left" : "Right")
					+ ", velocity {" + velocity.x.ToString() + ", " + velocity.y.ToString() + ", " + velocity.z.ToString() + ")");

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

		/// <summary> Checks if the player in taking a motion, e.g. Pinch, Hold. </summary>
		public bool GetHandMotion(TrackerType tracker, out HandMotion motion, bool isLeft)
		{
			motion = HandMotion.None;

			if (UseXRData(tracker))
			{
				uint motionId = (uint)motion;

				if (isLeft)
					SettingsHelper.GetInt(kHandMotionLeft, ref motionId);
				else
					SettingsHelper.GetInt(kHandMotionRight, ref motionId);

				if (motionId == 1) { motion = HandMotion.Pinch; }
				if (motionId == 2) { motion = HandMotion.Hold; }
				return true;
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

		/// <summary> Checks if the player is holding or side holding an equipment. </summary>
		public bool GetHandHoldRole(TrackerType tracker, out HandHoldRole role, bool isLeft)
		{
			role = HandHoldRole.None;
			if (GetHandMotion(tracker, isLeft) != HandMotion.Hold)
				return false;

			if (UseXRData(tracker))
			{
				role = HandHoldRole.None;
				uint roleId = (uint)role;

				if (isLeft)
					SettingsHelper.GetInt(kHandRoleLeft, ref roleId);
				else
					SettingsHelper.GetInt(kHandRoleRight, ref roleId);

				if (roleId == 1) { role = HandHoldRole.Main; }
				if (roleId == 2) { role = HandHoldRole.Side; }
				return true;
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

		/// <summary> Retrieves the type of equipment hold by the player. </summary>
		public bool GetHandHoldType(TrackerType tracker, out HandHoldType type, bool isLeft)
		{
			type = HandHoldType.None;
			if (GetHandMotion(tracker, isLeft) != HandMotion.Hold)
				return false;

			if (UseXRData(tracker))
			{
				uint typeId = (uint)type;

				if (isLeft)
					SettingsHelper.GetInt(kHandObjectLeft, ref typeId);
				else
					SettingsHelper.GetInt(kHandObjectRight, ref typeId);

				if (typeId == 1) { type = HandHoldType.Gun; }
				if (typeId == 2) { type = HandHoldType.OCSpray; }
				if (typeId == 3) { type = HandHoldType.LongGun; }
				if (typeId == 4) { type = HandHoldType.Baton; }
				if (typeId == 5) { type = HandHoldType.FlashLight; }
				return true;
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

		/// <summary> Retrieves the origin location in world space of Hand Pinch motion. </summary>
		public bool GetPinchOrigin(TrackerType tracker, ref Vector3 origin, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return false;

			if (UseXRData(tracker))
			{
				float origin_x = 0, origin_y = 0, origin_z = 0;
				if (isLeft)
				{
					SettingsHelper.GetFloat(kHandOriginLeftX, ref origin_x);
					SettingsHelper.GetFloat(kHandOriginLeftY, ref origin_y);
					SettingsHelper.GetFloat(kHandOriginLeftZ, ref origin_z);
				}
				else
				{
					SettingsHelper.GetFloat(kHandOriginRightX, ref origin_x);
					SettingsHelper.GetFloat(kHandOriginRightY, ref origin_y);
					SettingsHelper.GetFloat(kHandOriginRightZ, ref origin_z);
				}
				origin.x = origin_x;
				origin.y = origin_y;
				origin.z = origin_z;
				return true;
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

		/// <summary> Retrieves the direction vector in world space of Hand Pinch motion. </summary>
		public bool GetPinchDirection(TrackerType tracker, ref Vector3 direction, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return false;

			if (UseXRData(tracker))
			{
				float direction_x = 0, direction_y = 0, direction_z = 0;
				if (isLeft)
				{
					SettingsHelper.GetFloat(kHandDirectionLeftX, ref direction_x);
					SettingsHelper.GetFloat(kHandDirectionLeftY, ref direction_y);
					SettingsHelper.GetFloat(kHandDirectionLeftZ, ref direction_z);
				}
				else
				{
					SettingsHelper.GetFloat(kHandDirectionRightX, ref direction_x);
					SettingsHelper.GetFloat(kHandDirectionRightY, ref direction_y);
					SettingsHelper.GetFloat(kHandDirectionRightZ, ref direction_z);
				}
				direction.x = direction_x;
				direction.y = direction_y;
				direction.z = direction_z;
				return true;
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

		/// <summary> Retrieves the strength of Hand Pinch motion. </summary>
		public float GetPinchStrength(TrackerType tracker, bool isLeft)
		{
			if (GetHandMotion(tracker, isLeft) != HandMotion.Pinch)
				return 0;

			if (UseXRData(tracker))
			{
				float strength = 0;
				if (isLeft)
					SettingsHelper.GetFloat(kHandStrengthLeft, ref strength);
				else
					SettingsHelper.GetFloat(kHandStrengthRight, ref strength);
				return strength;
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

		/// <summary> Retrieves the default threshold of Hand Pinch motion. </summary>
		public float GetPinchThreshold(TrackerType tracker)
		{
			if (UseXRData(tracker))
			{
				float threshold = 0;
				SettingsHelper.GetFloat(kHandPinchThreshold, ref threshold);
				return threshold;
			}

			if ((tracker == TrackerType.Natural) && hasNaturalTrackerInfo)
			{
				return m_NaturalTrackerInfo.pinchThreshold;
			}
			if ((tracker == TrackerType.Electronic) && hasElectronicTrackerInfo)
			{
				return m_ElectronicTrackerInfo.pinchThreshold;
			}
			return 0;
		}
		public float GetPinchThreshold()
		{
			TrackerType tracker = TrackerType.Natural;
			if (GetPreferTracker(ref tracker))
				return GetPinchThreshold(tracker);
			return 0;
		}

		private bool m_WristPositionFused = false;
		private void CheckWristPositionFusion()
		{
			m_WristPositionFused = Interop.WVR_IsEnhanceHandStable();
			DEBUG("CheckWristPositionFusion() " + m_WristPositionFused);
		}
		private void OnWristPositionFusionChange(WVR_Event_t systemEvent)
		{
			DEBUG("OnWristPositionFusionChange()");
			CheckWristPositionFusion();
		}
		public void FuseWristPositionWithTracker(bool fuse)
		{
			DEBUG("FuseWristPositionWithTracker() " + fuse);
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
					DEBUG("GetHandJointCount() " + tracker);
					m_NaturalHandJointCount = count;
					InitializeHandTrackerInfo(ref m_NaturalTrackerInfo, ref s_NaturalHandJoints, ref s_NaturalHandJointsFlag, m_NaturalHandJointCount);
					InitializeHandTrackerData(
						ref m_NaturalHandTrackerData,
						ref m_NaturalHandJointDataLeft,
						ref m_NaturalHandJointDataRight,
						ref s_NaturalHandJointsPoseLeft,
						ref s_NaturalHandJointsPoseRight,
						m_NaturalHandJointCount);
					DEBUG("GetHandJointCount() " + tracker
						+ ", joint count: " + m_NaturalTrackerInfo.jointCount
						+ ", right (" + m_NaturalHandTrackerData.right.isValidPose + ", " + m_NaturalHandTrackerData.right.confidence + ", " + m_NaturalHandTrackerData.right.jointCount + ")"
						+ ", left (" + m_NaturalHandTrackerData.left.isValidPose + ", " + m_NaturalHandTrackerData.left.confidence + ", " + m_NaturalHandTrackerData.left.jointCount + ")");
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
					DEBUG("GetHandJointCount() " + tracker);
					m_ElectronicHandJointCount = count;
					InitializeHandTrackerInfo(ref m_ElectronicTrackerInfo, ref s_ElectronicHandJoints, ref s_ElectronicHandJointsFlag, m_ElectronicHandJointCount);
					InitializeHandTrackerData(
						ref m_ElectronicHandTrackerData,
						ref m_ElectronicHandJointDataLeft,
						ref m_ElectronicHandJointDataRight,
						ref s_ElectronicHandJointsPoseLeft,
						ref s_ElectronicHandJointsPoseRight,
						m_ElectronicHandJointCount);
					DEBUG("GetHandJointCount() " + tracker
						+ ", joint count: " + m_ElectronicTrackerInfo.jointCount
						+ ", right (" + m_ElectronicHandTrackerData.right.isValidPose + ", " + m_ElectronicHandTrackerData.right.confidence + ", " + m_ElectronicHandTrackerData.right.jointCount
						+ ", left (" + m_ElectronicHandTrackerData.left.isValidPose + ", " + m_ElectronicHandTrackerData.left.confidence + ", " + m_ElectronicHandTrackerData.left.jointCount);
				}
			}
		}

		#region Hand Tracker Info
		int[] intJointMappingArray;
		byte[] jointValidFlagArrayBytes;
		private void InitializeHandTrackerInfo(ref WVR_HandTrackerInfo_t handTrackerInfo, ref WVR_HandJoint[] jointMappingArray, ref ulong[] jointValidFlagArray, uint count)
		{
			DEBUG("InitializeHandTrackerInfo() count: " + count);
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
				WARNING("ExtractHandTrackerInfo() WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount SHOULD NOT be 0!!");
				return false;
			}

			// WVR_HandTrackerInfo_t.jointMappingArray
			if (jointMappingArray.Length != handTrackerInfo.jointCount)
			{
				WARNING("ExtractHandTrackerInfo() The WVR_GetHandJointCount count (jointMappingArray) " + jointMappingArray.Length
					+ " differs from WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount " + handTrackerInfo.jointCount);
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
				WARNING("ExtractHandTrackerInfo() The WVR_GetHandJointCount count (jointValidFlagArray) " + jointValidFlagArray.Length
					+ " differs from WVR_GetHandTrackerInfo WVR_HandTrackerInfo_t jointCount " + handTrackerInfo.jointCount);
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
			INFO("GetHandTrackerInfo() " + tracker);
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
								DEBUG("GetHandTrackerInfo() " + tracker
									+ ", joint count: " + m_NaturalTrackerInfo.jointCount
									+ ", s_NaturalHandJoints[" + i + "] = " + s_NaturalHandJoints[i]
									+ ", s_NaturalHandJointsFlag[" + i + "] = " + s_NaturalHandJointsFlag[i]);
							}
						}
					}
				}
				else
				{
					INTERVAL("GetHandTrackerInfo() Natural, failed!!");
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
								DEBUG("GetHandTrackerInfo() " + tracker
									+ ", joint count: " + m_ElectronicTrackerInfo.jointCount
									+ ", s_ElectronicHandJoints[" + i + "] = " + s_ElectronicHandJoints[i]
									+ ", s_ElectronicHandJointsFlag[" + i + "] = " + s_ElectronicHandJointsFlag[i]);
							}
						}
					}
				}
				else
				{
					INTERVAL("GetHandTrackerInfo() Electronic, failed!!");
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
				WARNING("ExtractHandJointData() WVR_GetHandTrackingData WVR_HandJointData_t jointCount SHOULD NOT be 0!!");
				return false;
			}

			if (jointsPose.Length != handJointData.jointCount)
			{
				WARNING("ExtractHandJointData() The WVR_GetHandJointCount count " + jointsPose.Length
					+ " differs from WVR_GetHandTrackingData WVR_HandJointData_t jointCount " + handJointData.jointCount);
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
						INTERVAL("GetHandTrackingData() Natural"
							+ ", timestamp: " + m_NaturalHandTrackerData.timestamp
							+ ", left valid? " + m_NaturalHandTrackerData.left.isValidPose
							+ ", left confidence: " + m_NaturalHandTrackerData.left.confidence.ToString()
							+ ", left count: " + m_NaturalHandTrackerData.left.jointCount
							+ ", right valid? " + m_NaturalHandTrackerData.right.isValidPose
							+ ", right confidence: " + m_NaturalHandTrackerData.right.confidence.ToString()
							+ ", right count: " + m_NaturalHandTrackerData.right.jointCount);

						if (m_NaturalHandPoseData.left.state.type == WVR_HandPoseType.WVR_HandPoseType_Pinch)
						{
							INTERVAL("GetHandTrackingData() Natural, left pinch "
								+ "strength: " + m_NaturalHandPoseData.left.pinch.strength.ToString()
							);
						}
						if (m_NaturalHandPoseData.left.state.type == WVR_HandPoseType.WVR_HandPoseType_Hold)
						{
							INTERVAL("GetHandTrackingData() Natural, left hold "
								+ "role: " + m_NaturalHandPoseData.left.hold.role
								+ ", type: " + m_NaturalHandPoseData.left.hold.type
							);
						}
						if (m_NaturalHandPoseData.right.state.type == WVR_HandPoseType.WVR_HandPoseType_Pinch)
						{
							INTERVAL("GetHandTrackingData() Natural, right pinch "
								+ "strength: " + m_NaturalHandPoseData.right.pinch.strength.ToString()
							);
						}
						if (m_NaturalHandPoseData.right.state.type == WVR_HandPoseType.WVR_HandPoseType_Hold)
						{
							INTERVAL("GetHandTrackingData() Natural, right hold "
								+ "role: " + m_NaturalHandPoseData.right.hold.role
								+ ", type: " + m_NaturalHandPoseData.right.hold.type
							);
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
						INTERVAL("GetHandTrackingData() Natural " + result);

						m_NaturalHandTrackerData.left.isValidPose = false;
						m_NaturalHandTrackerData.left.confidence = 0;
						m_NaturalHandTrackerData.right.isValidPose = false;
						m_NaturalHandTrackerData.right.confidence = 0;
					}
				}
				else
				{
					INTERVAL("GetHandTrackingData() Natural, hasNaturalTrackerInfo: " + hasNaturalTrackerInfo
						+ ", jointCount: " + m_NaturalTrackerInfo.jointCount
						+ ", handModelTypeBitMask: " + m_NaturalTrackerInfo.handModelTypeBitMask);
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
						INTERVAL("GetHandTrackingData() Electronic"
							+ ", timestamp: " + m_ElectronicHandTrackerData.timestamp
							+ ", left valid? " + m_ElectronicHandTrackerData.left.isValidPose
							+ ", left confidence: " + m_ElectronicHandTrackerData.left.confidence
							+ ", left count: " + m_ElectronicHandTrackerData.left.jointCount
							+ ", right valid? " + m_ElectronicHandTrackerData.right.isValidPose
							+ ", right confidence: " + m_ElectronicHandTrackerData.right.confidence
							+ ", right count: " + m_ElectronicHandTrackerData.right.jointCount);

						hasElectronicHandTrackerData = ExtractHandTrackerData(m_ElectronicHandTrackerData, ref s_ElectronicHandJointsPoseLeft, ref s_ElectronicHandJointsPoseRight);
						if (hasElectronicHandTrackerData)
						{
							m_ElectronicHandJointDataLeft = m_ElectronicHandTrackerData.left;
							m_ElectronicHandJointDataRight = m_ElectronicHandTrackerData.right;
						}
					}
					else
					{
						INTERVAL("GetHandTrackingData() Electronic " + result);

						m_ElectronicHandTrackerData.left.isValidPose = false;
						m_ElectronicHandTrackerData.left.confidence = 0;
						m_ElectronicHandTrackerData.right.isValidPose = false;
						m_ElectronicHandTrackerData.right.confidence = 0;
					}
				}
				else
				{
					INTERVAL("GetHandTrackingData() Electronic, hasNaturalTrackerInfo: " + hasNaturalTrackerInfo
						+ ", jointCount: " + m_ElectronicTrackerInfo.jointCount
						+ ", handModelTypeBitMask: " + m_ElectronicTrackerInfo.handModelTypeBitMask);
					hasElectronicHandTrackerData = false;
				}
				Profiler.EndSample();
			}
		}
		#endregion

		#region Obsolete Hand v1 APIs
		[Obsolete("This parameter is deprecated.")]
		public static readonly string HAND_STATIC_GESTURE_LEFT = "HAND_STATIC_GESTURE_LEFT";
		[Obsolete("This parameter is deprecated.")]
		public static readonly string HAND_STATIC_GESTURE_RIGHT = "HAND_STATIC_GESTURE_RIGHT";
		[Obsolete("This parameter is deprecated.")]
		public bool EnableHandTracking { get { return true; } }

		[Obsolete("This enum is deprecated. Please use GestureStatus instead.")]
		public enum HandGestureStatus
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
		[Obsolete("This enum is deprecated.")]
		public enum HandTrackingStatus
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
		[Obsolete("This enum is deprecated.")]
		public enum StaticGestures
		{
			UNKNOWN = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_Unknown,
			FIST = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_Fist,
			FIVE = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_Five,
			OK = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_OK,
			THUMBUP = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_ThumbUp,
			INDEXUP = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_IndexUp,
			PALM_PINCH = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_Palm_Pinch,
			YEAH = 1 << (int)WVR_HandGestureType.WVR_HandGestureType_Yeah,
		}

		[Obsolete("This function is deprecated.")]
		public ulong GetHandGestureLeft()
		{
			ulong gesture_value = 0;
			switch (m_HandGestureLeft)
			{
				case GestureType.Fist:
					gesture_value = (ulong)StaticGestures.FIST;
					break;
				case GestureType.Five:
					gesture_value = (ulong)StaticGestures.FIVE;
					break;
				case GestureType.IndexUp:
					gesture_value = (ulong)StaticGestures.INDEXUP;
					break;
				case GestureType.ThumbUp:
					gesture_value = (ulong)StaticGestures.THUMBUP;
					break;
				case GestureType.OK:
					gesture_value = (ulong)StaticGestures.OK;
					break;
				case GestureType.Palm_Pinch:
					gesture_value = (ulong)StaticGestures.PALM_PINCH;
					break;
				case GestureType.Yeah:
					gesture_value = (ulong)StaticGestures.YEAH;
					break;
				default:
					break;
			}
			return gesture_value;
		}
		[Obsolete("This function is deprecated.")]
		public ulong GetHandGestureRight()
		{
			ulong gesture_value = 0;
			switch (m_HandGestureRight)
			{
				case GestureType.Fist:
					gesture_value = (ulong)StaticGestures.FIST;
					break;
				case GestureType.Five:
					gesture_value = (ulong)StaticGestures.FIVE;
					break;
				case GestureType.IndexUp:
					gesture_value = (ulong)StaticGestures.INDEXUP;
					break;
				case GestureType.ThumbUp:
					gesture_value = (ulong)StaticGestures.THUMBUP;
					break;
				case GestureType.OK:
					gesture_value = (ulong)StaticGestures.OK;
					break;
				case GestureType.Palm_Pinch:
					gesture_value = (ulong)StaticGestures.PALM_PINCH;
					break;
				case GestureType.Yeah:
					gesture_value = (ulong)StaticGestures.YEAH;
					break;
				default:
					break;
			}
			return gesture_value;
		}
		[Obsolete("This function is deprecated.")]
		public HandTrackingStatus GetHandTrackingStatus() { return HandTrackingStatus.NoSupport; }
		[Obsolete("This function is deprecated.")]
		public void RestartHandTracking() { }
		[Obsolete("This function is deprecated.")]
		public bool GetHandPoseData(ref WVR_HandPoseData_t data) {
			data = m_NaturalHandPoseData;
			return hasNaturalHandTrackerData;
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
	}
}
