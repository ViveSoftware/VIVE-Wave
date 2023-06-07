// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.Essence.Events;
using Wave.Native;
using Wave.OpenXR;
using Wave.XR.Settings;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;
using UnityEngine.XR;
using System.Runtime.InteropServices;
using System.Text;

namespace Wave.Essence.Tracker
{
	public class TrackerManager : MonoBehaviour
	{
		private const string LOG_TAG = "Wave.Essence.Tracker.TrackerManager";
		private void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}
		private void DEBUG(StringBuilder sb)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, sb, true);
		}
		bool printIntervalLog = false;
		int logFrame = 0;
		private void INTERVAL(string msg) { if (printIntervalLog) { DEBUG(msg); } }

		private StringBuilder m_TrackerManagerStringBuilder = null;
		internal StringBuilder TrackerManagerStringBuilder {
			get {
				if (m_TrackerManagerStringBuilder == null) { m_TrackerManagerStringBuilder = new StringBuilder(); }
				return m_TrackerManagerStringBuilder;
			}
		}

		private bool m_UseXRDevice = true;
		public bool UseXRDevice { get { return m_UseXRDevice; } set { m_UseXRDevice = value; } }

		WaveXRSettings m_WaveXRSettings = null;
		bool UseXRData()
		{
			// Tracker is already enabled in WaveXRSettings.
			bool XRAlreadyEnabled = (m_WaveXRSettings != null ? m_WaveXRSettings.EnableTracker : false);

			return (
				(XRAlreadyEnabled || m_UseXRDevice)
				&& (!Application.isEditor)
				);
		}

		public enum TrackerStatus
		{
			// Initial, can call Start API in this state.
			NotStart = 0,
			StartFailure,

			// Processing, should NOT call API in this state.
			Starting,
			Stopping,

			// Running, can call Stop API in this state.
			Available,

			// Do nothing.
			NoSupport
		}

		private static TrackerManager instance = null;
		public static TrackerManager Instance { get { return instance; } }

		[SerializeField]
		private bool m_InitialStartTracker = false;
		public bool InitialStartTracker { get { return m_InitialStartTracker; } set { m_InitialStartTracker = value; } }

		#region Wave XR Constants
		const string kTrackerComponentStatus = "TrackerComponentStatus";
		#endregion

		#region Monobehaviour overrides
		private void Awake()
		{
			instance = this;

			m_WaveXRSettings = WaveXRSettings.GetInstance();

			/// Checks the feature support.
			var supportedFeature = Interop.WVR_GetSupportedFeatures();

			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_Tracker) == 0)
				m_TrackerStatus = TrackerStatus.NoSupport;
			else
				m_TrackerStatus = TrackerStatus.NotStart;

			Log.i(LOG_TAG, "Awake() tracker status: " + m_TrackerStatus);

			/// Initializes the tracker attributes.
			s_TrackerCaps = new WVR_TrackerCapabilities[TrackerUtils.s_TrackerIds.Length];
			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[i];
				s_TrackerConnection.Add(tracker, false);

				s_TrackerRole.Add(tracker, TrackerRole.Undefined);

				ResetTrackerCapability(tracker);

				s_TrackerPoses.Add(tracker, new TrackerPose());

				s_TrackerButtonBits.Add(tracker, 0);
				s_TrackerTouchBits.Add(tracker, 0);
				s_TrackerAnalogBits.Add(tracker, 0);

				s_TrackerButtonStates.Add(tracker, new TrackerButtonStates());

				s_ButtonAxisType.Add(tracker, new AxisType[kButtonCount]);

				ss_TrackerPress.Add(tracker, new bool[kButtonCount]);
				ss_TrackerPressEx.Add(tracker, new bool[kButtonCount]);
				ss_TrackerTouch.Add(tracker, new bool[kButtonCount]);
				ss_TrackerTouchEx.Add(tracker, new bool[kButtonCount]);

				for (int id = 0; id < kButtonCount; id++)
				{
					s_ButtonAxisType[tracker][id] = AxisType.None;

					ss_TrackerPress[tracker][id] = false;
					ss_TrackerPressEx[tracker][id] = false;
					ss_TrackerTouch[tracker][id] = false;
					ss_TrackerTouchEx[tracker][id] = false;
				}

				s_TrackerTimestamp.Add(tracker, 0);
				s_TrackerBattery.Add(tracker, 0);
				s_TrackerExtData.Add(tracker, new Int32[12]);

				s_HasTrackerDeviceName.Add(tracker, false);
				s_TrackerDeviceName.Add(tracker, "");
			}
		}

		private void OnEnable()
		{
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerConnected, OnTrackerConnected);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerDisconnected, OnTrackerDisconnected);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerBatteryLevelUpdate, OnTrackerBatteryLevelUpdate);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerRoleChanged, OnTrackerRoleChanged);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerButtonPressed, OnTrackerButtonPressed);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerButtonUnpressed, OnTrackerButtonUnpressed);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerTouchTapped, OnTrackerTouchTapped);
			SystemEvent.Listen(WVR_EventType.WVR_EventType_TrackerTouchUntapped, OnTrackerTouchUntapped);

			if (m_InitialStartTracker) { StartTracker(); }
		}
		private void OnDisable()
		{
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerConnected, OnTrackerConnected);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerDisconnected, OnTrackerDisconnected);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerBatteryLevelUpdate, OnTrackerBatteryLevelUpdate);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerRoleChanged, OnTrackerRoleChanged);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerButtonPressed, OnTrackerButtonPressed);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerButtonUnpressed, OnTrackerButtonUnpressed);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerTouchTapped, OnTrackerTouchTapped);
			SystemEvent.Remove(WVR_EventType.WVR_EventType_TrackerTouchUntapped, OnTrackerTouchUntapped);
		}

		static List<InputDevice> s_InputDevices = new List<InputDevice>();
		private void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			UpdateTrackerExtData();

			if (printIntervalLog)
			{
				var sb = TrackerManagerStringBuilder;
				sb.Clear();
				sb.Append("Update() ").Append("use xr device: ").Append(m_UseXRDevice).Append(", use xr data: ").Append(UseXRData());

				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
				{
					TrackerId tracker = TrackerUtils.s_TrackerIds[i];

					sb.Clear();
					sb.Append("Update() ").Append(tracker)
					.Append(", name: ").Append(s_TrackerDeviceName[tracker])
					.Append(", connection: ").Append(s_TrackerConnection[tracker])
					.Append(", valid: ").Append(s_TrackerPoses[tracker].valid)
					.Append(", role: ").Append(s_TrackerRole[tracker])
					.Append(", battery: ").Append(s_TrackerBattery[tracker]);
				}
				DEBUG(sb);
			}

			if (UseXRData()) { return; }

			CheckAllTrackerPoseStates();
		}

		private void OnApplicationPause(bool pause)
		{
			DEBUG("OnApplicationPause() " + pause);
			if (GetTrackerStatus() != TrackerStatus.Available) { return; }
			if (!pause)
			{
				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
				{
					TrackerId tracker = TrackerUtils.s_TrackerIds[i];
					DEBUG("Resume() 1.check " + tracker + " connection.");
					CheckTrackerConnection(tracker);

					DEBUG("Resume() 2.check " + tracker + " role.");
					CheckTrackerRole(tracker);

					DEBUG("Resume() 3.check " + tracker + " capability.");
					CheckTrackerCapbility(tracker);

					DEBUG("Resume() 4. check " + tracker + " input capability.");
					CheckTrackerInputs(tracker);

					DEBUG("Resume() 5. check " + tracker + " button analog type.");
					CheckTrackerButtonAnalog(tracker);

					DEBUG("Resume() 6. check " + tracker + " buttons.");
					CheckAllTrackerButtons(tracker);

					DEBUG("Resume() 7.check " + tracker + " battery.");
					CheckTrackerBattery(tracker);

					DEBUG("Resume() 8.check " + tracker + " name.");
					UpdateTrackerDeviceName(tracker);
				}
			}
		}

		private void Start()
		{
			if (GetTrackerStatus() != TrackerStatus.Available) { return; }
			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[i];
				DEBUG("Start() 1.check " + tracker + " connection.");
				CheckTrackerConnection(tracker);

				DEBUG("Start() 2.check " + tracker + " role.");
				CheckTrackerRole(tracker);

				DEBUG("Start() 3.check " + tracker + " capability.");
				CheckTrackerCapbility(tracker);

				// For WVR_TrackerCapabilities.supportsInputDevice
				DEBUG("Start() 4. check " + tracker + " input capability.");
				CheckTrackerInputs(tracker);

				// Depends on IsTrackerInputAvailable
				DEBUG("Start() 5. check " + tracker + " button analog type.");
				CheckTrackerButtonAnalog(tracker);

				// Depends on IsTrackerInputAvailable
				DEBUG("Start() 6. check " + tracker + " buttons.");
				CheckAllTrackerButtons(tracker);

				// For WVR_TrackerCapabilities.supportsBatteryLevel
				DEBUG("Start() 7.check " + tracker + " battery.");
				CheckTrackerBattery(tracker);

				DEBUG("Start() 8.check " + tracker + " name.");
				UpdateTrackerDeviceName(tracker);
			}
		}
		#endregion

		#region Life Cycle
		private TrackerStatus m_TrackerStatus = TrackerStatus.NotStart;
		private static ReaderWriterLockSlim m_TrackerStatusRWLock = new ReaderWriterLockSlim();
		private void SetTrackerStatus(TrackerStatus status)
		{
			try
			{
				m_TrackerStatusRWLock.TryEnterWriteLock(2000);
				m_TrackerStatus = status;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "SetTrackerStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_TrackerStatusRWLock.ExitWriteLock();
			}
		}

		private bool CanStartTracker()
		{
			TrackerStatus status = GetTrackerStatus();
			if (status == TrackerStatus.Available ||
				status == TrackerStatus.NoSupport ||
				status == TrackerStatus.Starting ||
				status == TrackerStatus.Stopping)
			{
				return false;
			}

			return true;
		}
		private bool CanStopTracker()
		{
			TrackerStatus status = GetTrackerStatus();
			if (status == TrackerStatus.Available) { return true; }
			return false;
		}

		private uint m_TrackerRefCount = 0;
		public delegate void TrackerResultDelegate(object sender, bool result);
		private event TrackerResultDelegate trackerResultCB = null;
		private void StartTrackerLock()
		{
			if (UseXRData())
			{
				InputDeviceTracker.ActivateTracker(true);
				return;
			}

			if (!CanStartTracker()) { return; }

			SetTrackerStatus(TrackerStatus.Starting);
			WVR_Result result = Interop.WVR_StartTracker();
			switch (result)
			{
				case WVR_Result.WVR_Success:
					SetTrackerStatus(TrackerStatus.Available);
					break;
				case WVR_Result.WVR_Error_FeatureNotSupport:
					SetTrackerStatus(TrackerStatus.NoSupport);
					break;
				default:
					SetTrackerStatus(TrackerStatus.StartFailure);
					break;
			}
			DEBUG("StartTrackerLock() result: " + result);
			if (result == WVR_Result.WVR_Success)
			{
				// Check all states anyway when starting the tracker successfully
				// because the tracker may be connected before starting the tracker.
				for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
					CheckStatusWhenConnectionChanges(TrackerUtils.s_TrackerIds[i]);
			}

			if (trackerResultCB != null)
			{
				trackerResultCB(this, result == WVR_Result.WVR_Success ? true : false);
				trackerResultCB = null;
			}
		}

		private object trackerThreadLocker = new object();
		private void StartTrackerThread()
		{
			lock (trackerThreadLocker)
			{
				DEBUG("StartTrackerThread()");
				StartTrackerLock();
			}
		}
		public void StartTracker(TrackerResultDelegate callback)
		{
			if (trackerResultCB == null)
			{
				trackerResultCB = callback;
			}
			else
			{
				trackerResultCB += callback;
			}

			StartTracker();
		}
		public void StartTracker()
		{
			m_TrackerRefCount++;
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

			if (!CanStartTracker())
			{
				DEBUG("StartTracker() can NOT start tracker.");
				if (trackerResultCB != null) { trackerResultCB = null; }
				return;
			}

			Thread tracker_t = new Thread(StartTrackerThread);
			tracker_t.Name = "StartTrackerThread";
			tracker_t.Start();
		}

		private void StopTrackerLock()
		{
			if (UseXRData())
			{
				InputDeviceTracker.ActivateTracker(false);
				return;
			}

			if (!CanStopTracker()) { return; }

			DEBUG("StopTrackerLock()");
			SetTrackerStatus(TrackerStatus.Stopping);
			Interop.WVR_StopTracker();
			SetTrackerStatus(TrackerStatus.NotStart);

			// Reset all tracker status.
			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[i];
				s_TrackerConnection[tracker] = false;

				CheckTrackerRole(tracker);
				CheckTrackerCapbility(tracker);
				CheckTrackerInputs(tracker);
				CheckTrackerButtonAnalog(tracker);
				UpdateTrackerDeviceName(tracker);
			}
		}
		private void StopTrackerThread()
		{
			lock (trackerThreadLocker)
			{
				DEBUG("StopTrackerThread()");
				StopTrackerLock();
			}
		}
		public void StopTracker()
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
			m_TrackerRefCount--;
			Log.i(LOG_TAG, "StopTracker(" + m_TrackerRefCount + ") from " + caller, true);
			if (m_TrackerRefCount > 0) { return; }

			if (!CanStopTracker())
			{
				DEBUG("CanStopTracker() can NOT stop tracker.");
				return;
			}

			Thread tracker_t = new Thread(StopTrackerThread);
			tracker_t.Name = "StopTrackerThread";
			tracker_t.Start();
		}
		#endregion

		#region Unity XR Tracker definitions
		/// <summary> Standalone Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kAloneTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice
		);
		/// <summary> Right Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kRightTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Right
		);
		/// <summary> Left Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kLeftTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Left
		);
		#endregion

		#region Connection
		Dictionary<TrackerId, bool> s_TrackerConnection = new Dictionary<TrackerId, bool>();
		private void CheckTrackerConnection(TrackerId trackerId)
		{
			bool connected = Interop.WVR_IsTrackerConnected(trackerId.Id());
			if (s_TrackerConnection[trackerId] != connected)
			{
				s_TrackerConnection[trackerId] = connected;
				DEBUG("CheckTrackerConnection() " + trackerId + ": " + s_TrackerConnection[trackerId]);
				CheckStatusWhenConnectionChanges(trackerId);
			}
		}
		void CheckStatusWhenConnectionChanges(TrackerId trackerId)
		{
			DEBUG("CheckStatusWhenConnectionChanges() " + trackerId);
			CheckTrackerRole(trackerId);
			CheckTrackerCapbility(trackerId);
			CheckTrackerInputs(trackerId);
			CheckTrackerButtonAnalog(trackerId);
			CheckAllTrackerButtons(trackerId);
			CheckTrackerBattery(trackerId);
			UpdateTrackerDeviceName(trackerId);
		}
		private void OnTrackerConnected(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.tracker.trackerId.Id();
			DEBUG("OnTrackerConnected() " + trackerId);

			if (s_TrackerConnection[trackerId] != true)
			{
				s_TrackerConnection[trackerId] = true;
				CheckStatusWhenConnectionChanges(trackerId);
			}
		}
		private void OnTrackerDisconnected(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.tracker.trackerId.Id();
			DEBUG("OnTrackerDisconnected() " + trackerId);

			if (s_TrackerConnection[trackerId] != false)
			{
				s_TrackerConnection[trackerId] = false;
				CheckStatusWhenConnectionChanges(trackerId);
			}
		}
		#endregion

		#region Role
		Dictionary<TrackerId, TrackerRole> s_TrackerRole = new Dictionary<TrackerId, TrackerRole>();
		private void CheckTrackerRole(TrackerId trackerId)
		{
			if (IsTrackerConnected(trackerId))
			{
				s_TrackerRole[trackerId] = Interop.WVR_GetTrackerRole(trackerId.Id()).Id();
				DEBUG("CheckTrackerRole() " + trackerId + " role: " + s_TrackerRole[trackerId]);
			}
			else
			{
				s_TrackerRole[trackerId] = TrackerRole.Undefined;
			}
		}
		private void OnTrackerRoleChanged(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.tracker.trackerId.Id();
			DEBUG("OnTrackerRoleChanged() " + trackerId);
			CheckTrackerRole(trackerId);
		}
		#endregion

		#region Capability
		WVR_TrackerCapabilities[] s_TrackerCaps = null;
		private void ResetTrackerCapability(TrackerId trackerId)
		{
			s_TrackerCaps[trackerId.Num()].supportsOrientationTracking = false;
			s_TrackerCaps[trackerId.Num()].supportsPositionTracking = false;
			s_TrackerCaps[trackerId.Num()].supportsInputDevice = false;
			s_TrackerCaps[trackerId.Num()].supportsHapticVibration = false;
			s_TrackerCaps[trackerId.Num()].supportsBatteryLevel = false;

		}
		private void CheckTrackerCapbility(TrackerId trackerId)
		{
			if (IsTrackerConnected(trackerId))
			{
				WVR_Result result = Interop.WVR_GetTrackerCapabilities(trackerId.Id(), ref s_TrackerCaps[trackerId.Num()]);
				if (result != WVR_Result.WVR_Success) { ResetTrackerCapability(trackerId); }

				DEBUG("CheckTrackerCapbility() " + trackerId + ", result: " + result
					+ "\n\tsupportsOrientationTracking: " + s_TrackerCaps[trackerId.Num()].supportsOrientationTracking
					+ "\n\tsupportsPositionTracking: " + s_TrackerCaps[trackerId.Num()].supportsPositionTracking
					+ "\n\tsupportsInputDevice: " + s_TrackerCaps[trackerId.Num()].supportsInputDevice
					+ "\n\tsupportsHapticVibration: " + s_TrackerCaps[trackerId.Num()].supportsHapticVibration
					+ "\n\tsupportsBatteryLevel: " + s_TrackerCaps[trackerId.Num()].supportsBatteryLevel);
			}
			else
			{
				ResetTrackerCapability(trackerId);
			}
		}
		#endregion

		#region Pose State
		class TrackerPose
		{
			public bool valid = false;
			public RigidTransform rigid = RigidTransform.identity;
			public Vector3 velocity = Vector3.zero;
			public Vector3 angularVelocity = Vector3.zero;

			public TrackerPose()
			{
				valid = false;
				rigid = RigidTransform.identity;
				velocity = Vector3.zero;
				angularVelocity = Vector3.zero;
			}
		}
		WVR_PoseOriginModel originModel = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
		Dictionary<TrackerId, TrackerPose> s_TrackerPoses = new Dictionary<TrackerId, TrackerPose>();
		private void CheckTrackerPoseState(TrackerId trackerId)
		{
			s_TrackerPoses[trackerId].valid = false;

			ClientInterface.GetOrigin(ref originModel);

			if (IsTrackerConnected(trackerId) && s_TrackerCaps[trackerId.Num()].supportsOrientationTracking)
			{
				WVR_PoseOriginModel origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead_3DoF;
				if (s_TrackerCaps[trackerId.Num()].supportsPositionTracking)
				{
					origin = originModel;
				}

				WVR_PoseState_t pose = new WVR_PoseState_t();
				WVR_Result result = Interop.WVR_GetTrackerPoseState(trackerId.Id(), origin, 0, ref pose);
				if (result == WVR_Result.WVR_Success)
				{
					s_TrackerPoses[trackerId].valid = pose.IsValidPose;
					s_TrackerPoses[trackerId].rigid.update(pose.PoseMatrix);
					Coordinate.GetVectorFromGL(pose.Velocity, out s_TrackerPoses[trackerId].velocity);
					s_TrackerPoses[trackerId].angularVelocity.x = -pose.AngularVelocity.v0;
					s_TrackerPoses[trackerId].angularVelocity.y = -pose.AngularVelocity.v1;
					s_TrackerPoses[trackerId].angularVelocity.z = pose.AngularVelocity.v2;
				}
			}
		}
		private void CheckAllTrackerPoseStates()
		{
			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				CheckTrackerPoseState(TrackerUtils.s_TrackerIds[i]);
			}
		}
		#endregion

		#region Input Capability
		Dictionary<TrackerId, Int32> s_TrackerButtonBits = new Dictionary<TrackerId, Int32>();
		Dictionary<TrackerId, Int32> s_TrackerTouchBits = new Dictionary<TrackerId, Int32>();
		Dictionary<TrackerId, Int32> s_TrackerAnalogBits = new Dictionary<TrackerId, Int32>();
		private void CheckTrackerInputs(TrackerId trackerId)
		{
			s_TrackerButtonBits[trackerId] = IsTrackerConnected(trackerId) ?
				(s_TrackerCaps[trackerId.Num()].supportsInputDevice ?
					Interop.WVR_GetTrackerInputDeviceCapability(trackerId.Id(), WVR_InputType.WVR_InputType_Button) : 0
				) : 0;

			s_TrackerTouchBits[trackerId] = IsTrackerConnected(trackerId) ?
				(s_TrackerCaps[trackerId.Num()].supportsInputDevice ?
					Interop.WVR_GetTrackerInputDeviceCapability(trackerId.Id(), WVR_InputType.WVR_InputType_Touch) : 0
				) : 0;

			s_TrackerAnalogBits[trackerId] = IsTrackerConnected(trackerId) ?
				(s_TrackerCaps[trackerId.Num()].supportsInputDevice ?
					Interop.WVR_GetTrackerInputDeviceCapability(trackerId.Id(), WVR_InputType.WVR_InputType_Analog) : 0
				) : 0;

			DEBUG("CheckTrackerInputs() " + trackerId
				+ ", button: " + s_TrackerButtonBits[trackerId]
				+ ", touch: " + s_TrackerTouchBits[trackerId]
				+ ", analog: " + s_TrackerAnalogBits[trackerId]);
		}
		private bool IsTrackerInputAvailable(TrackerId trackerId, WVR_InputType inputType, uint id)
		{
			bool ret = false;

			Int32 input = 1 << (Int32)id;
			switch (inputType)
			{
				case WVR_InputType.WVR_InputType_Button:
					ret = ((s_TrackerButtonBits[trackerId] & input) == input);
					break;
				case WVR_InputType.WVR_InputType_Touch:
					ret = ((s_TrackerTouchBits[trackerId] & input) == input);
					break;
				case WVR_InputType.WVR_InputType_Analog:
					ret = ((s_TrackerAnalogBits[trackerId] & input) == input);
					break;
				default:
					break;
			}

			return ret;
		}
		#endregion

		#region Button Analog
		Dictionary<TrackerId, AxisType[]> s_ButtonAxisType = new Dictionary<TrackerId, AxisType[]>();
		private void CheckTrackerButtonAnalog(TrackerId trackerId)
		{
			for (uint id = 0; id < kButtonCount; id++)
			{
				if (!id.ValidWVRInputId()) { continue; }

				s_ButtonAxisType[trackerId][id] = IsTrackerConnected(trackerId) ?
					(IsTrackerInputAvailable(trackerId, WVR_InputType.WVR_InputType_Analog, id) ?
						(Interop.WVR_GetTrackerInputDeviceAnalogType(trackerId.Id(), (WVR_InputId)id)).Id() : AxisType.None
					) : AxisType.None;
			}
			DEBUG("CheckTrackerButtonAnalog() " + trackerId
				+ ", system: " + s_ButtonAxisType[trackerId][WVR_InputId.WVR_InputId_Alias1_System.ArrayIndex()]
				+ ", menu: " + s_ButtonAxisType[trackerId][WVR_InputId.WVR_InputId_Alias1_Menu.ArrayIndex()]
				+ ", A: " + s_ButtonAxisType[trackerId][WVR_InputId.WVR_InputId_Alias1_A.ArrayIndex()]
				+ ", B: " + s_ButtonAxisType[trackerId][WVR_InputId.WVR_InputId_Alias1_B.ArrayIndex()]);
		}
		#endregion

		#region Button State
		const uint kButtonCount = (uint)WVR_InputId.WVR_InputId_Max;
		class TrackerButtonStates
		{
			public bool[] s_ButtonPress = new bool[kButtonCount];
			public int[] s_ButtonPressFrame = new int[kButtonCount];
			public bool[] s_ButtonTouch = new bool[kButtonCount];
			public int[] s_ButtonTouchFrame = new int[kButtonCount];
			public Vector2[] s_ButtonAxis = new Vector2[kButtonCount];
			public int[] s_ButtonAxisFrame = new int[kButtonCount];

			public TrackerButtonStates()
			{
				for (int i = 0; i < s_ButtonPress.Length; i++)
				{
					s_ButtonPress[i] = false;
					s_ButtonPressFrame[i] = 0;
					s_ButtonTouch[i] = false;
					s_ButtonTouchFrame[i] = 0;
					s_ButtonAxis[i].x = 0;
					s_ButtonAxis[i].y = 0;
					s_ButtonAxisFrame[i] = 0;
				}
			}
		};
		Dictionary<TrackerId, TrackerButtonStates> s_TrackerButtonStates = new Dictionary<TrackerId, TrackerButtonStates>();
		/// <summary> Checks all buttons' states of a TrackerId. Do NOT call this function every frame. </summary>
		private void CheckAllTrackerButtons(TrackerId trackerId)
		{
			CheckAllTrackerButton(trackerId, WVR_InputType.WVR_InputType_Button);
			CheckAllTrackerButton(trackerId, WVR_InputType.WVR_InputType_Touch);
			CheckAllTrackerButton(trackerId, WVR_InputType.WVR_InputType_Analog);
		}
		/// <summary> Checks all buttons' states of a TrackerId and WVR_InputType. Do NOT call this function every frame. </summary>
		private void CheckAllTrackerButton(TrackerId trackerId, WVR_InputType cap)
		{
			for (uint id = 0; id < kButtonCount; id++)
			{
				if (!id.ValidWVRInputId()) { continue; }

				switch (cap)
				{
					case WVR_InputType.WVR_InputType_Button:
						s_TrackerButtonStates[trackerId].s_ButtonPress[id] = IsTrackerConnected(trackerId) ?
							(IsTrackerInputAvailable(trackerId, cap, id) ?
								Interop.WVR_GetTrackerInputButtonState(trackerId.Id(), (WVR_InputId)id) : false
							) : false;
						break;
					case WVR_InputType.WVR_InputType_Touch:
						s_TrackerButtonStates[trackerId].s_ButtonTouch[id] = IsTrackerConnected(trackerId) ?
							(IsTrackerInputAvailable(trackerId, cap, id) ?
								Interop.WVR_GetTrackerInputTouchState(trackerId.Id(), (WVR_InputId)id) : false
							) : false;
						break;
					case WVR_InputType.WVR_InputType_Analog:
						if (IsTrackerConnected(trackerId) && IsTrackerInputAvailable(trackerId, cap, id))
						{
							WVR_Axis_t axis = Interop.WVR_GetTrackerInputAnalogAxis(trackerId.Id(), (WVR_InputId)id);
							s_TrackerButtonStates[trackerId].s_ButtonAxis[id].x = axis.x;
							s_TrackerButtonStates[trackerId].s_ButtonAxis[id].y = axis.y;
						}
						else
						{
							s_TrackerButtonStates[trackerId].s_ButtonAxis[id] = Vector2.zero;
						}
						break;
					default:
						break;
				}
			}
		}

		private void OnTrackerButtonPressed(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.trackerInput.tracker.trackerId.Id();
			WVR_InputId id = systemEvent.trackerInput.inputId;
			DEBUG("OnTrackerButtonPressed() " + trackerId + ", " + id);

			s_TrackerButtonStates[trackerId].s_ButtonPress[id.ArrayIndex()] = true;
		}
		private void OnTrackerButtonUnpressed(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.trackerInput.tracker.trackerId.Id();
			WVR_InputId id = systemEvent.trackerInput.inputId;
			DEBUG("OnTrackerButtonUnpressed() " + trackerId + ", " + id);

			s_TrackerButtonStates[trackerId].s_ButtonPress[id.ArrayIndex()] = false;
		}
		private void OnTrackerTouchTapped(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.trackerInput.tracker.trackerId.Id();
			WVR_InputId id = systemEvent.trackerInput.inputId;
			DEBUG("OnTrackerTouchTapped() " + trackerId + ", " + id);

			s_TrackerButtonStates[trackerId].s_ButtonTouch[id.ArrayIndex()] = true;
		}
		private void OnTrackerTouchUntapped(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.trackerInput.tracker.trackerId.Id();
			WVR_InputId id = systemEvent.trackerInput.inputId;
			DEBUG("OnTrackerTouchUntapped() " + trackerId + ", " + id);

			s_TrackerButtonStates[trackerId].s_ButtonTouch[id.ArrayIndex()] = false;
		}

		bool AllowUpdateTrackerButton(TrackerId trackerId, WVR_InputId id)
		{
			if (s_TrackerButtonStates[trackerId].s_ButtonPressFrame[id.ArrayIndex()] != Time.frameCount)
			{
				s_TrackerButtonStates[trackerId].s_ButtonPressFrame[id.ArrayIndex()] = Time.frameCount;
				return true;
			}
			return false;
		}
		Dictionary<TrackerId, bool[]> ss_TrackerPress = new Dictionary<TrackerId, bool[]>();
		Dictionary<TrackerId, bool[]> ss_TrackerPressEx = new Dictionary<TrackerId, bool[]>();
		private void UpdateTrackerPress(TrackerId trackerId, WVR_InputId id)
		{
			if (AllowUpdateTrackerButton(trackerId, id))
			{
				ss_TrackerPressEx[trackerId][id.ArrayIndex()] = ss_TrackerPress[trackerId][id.ArrayIndex()];
				ss_TrackerPress[trackerId][id.ArrayIndex()] = s_TrackerButtonStates[trackerId].s_ButtonPress[id.ArrayIndex()];
			}
		}

		bool AllowUpdateTrackerTouch(TrackerId trackerid, WVR_InputId id)
		{
			if (s_TrackerButtonStates[trackerid].s_ButtonTouchFrame[id.ArrayIndex()] != Time.frameCount)
			{
				s_TrackerButtonStates[trackerid].s_ButtonTouchFrame[id.ArrayIndex()] = Time.frameCount;
				return true;
			}
			return false;
		}
		Dictionary<TrackerId, bool[]> ss_TrackerTouch = new Dictionary<TrackerId, bool[]>();
		Dictionary<TrackerId, bool[]> ss_TrackerTouchEx = new Dictionary<TrackerId, bool[]>();
		private void UpdateTrackerTouch(TrackerId trackerId, WVR_InputId id)
		{
			if (AllowUpdateTrackerTouch(trackerId, id))
			{
				ss_TrackerTouchEx[trackerId][id.ArrayIndex()] = ss_TrackerTouch[trackerId][id.ArrayIndex()];
				ss_TrackerTouch[trackerId][id.ArrayIndex()] = s_TrackerButtonStates[trackerId].s_ButtonTouch[id.ArrayIndex()];
			}
		}

		bool AllowUpdateTrackerAxis(TrackerId trackerId, WVR_InputId id)
		{
			if (s_TrackerButtonStates[trackerId].s_ButtonAxisFrame[id.ArrayIndex()] != Time.frameCount)
			{
				s_TrackerButtonStates[trackerId].s_ButtonAxisFrame[id.ArrayIndex()] = Time.frameCount;
				return true;
			}

			return false;
		}
		private void UpdateTrackerAxis(TrackerId trackerId, WVR_InputId id)
		{
			if (IsTrackerInputAvailable(trackerId, WVR_InputType.WVR_InputType_Analog, (uint)id))
			{
				if (AllowUpdateTrackerAxis(trackerId, id))
				{
					WVR_Axis_t axis = Interop.WVR_GetTrackerInputAnalogAxis(trackerId.Id(), id);
					s_TrackerButtonStates[trackerId].s_ButtonAxis[id.ArrayIndex()].x = axis.x;
					s_TrackerButtonStates[trackerId].s_ButtonAxis[id.ArrayIndex()].y = axis.y;
				}
			}
			else
			{
				s_TrackerButtonStates[trackerId].s_ButtonAxis[id.ArrayIndex()] = Vector2.zero;
			}
		}
		#endregion

		#region Battery Life
		Dictionary<TrackerId, float> s_TrackerBattery = new Dictionary<TrackerId, float>();
		private void CheckTrackerBattery(TrackerId trackerId)
		{
			s_TrackerBattery[trackerId] = IsTrackerConnected(trackerId) ?
				(s_TrackerCaps[trackerId.Num()].supportsBatteryLevel
					? Interop.WVR_GetTrackerBatteryLevel(trackerId.Id()) : 0
				) : 0;
			DEBUG("CheckTrackerBattery() " + trackerId + ": " + s_TrackerBattery[trackerId]);
		}
		private void OnTrackerBatteryLevelUpdate(WVR_Event_t systemEvent)
		{
			TrackerId trackerId = systemEvent.tracker.trackerId.Id();
			DEBUG("OnTrackerBatteryLevelUpdate() " + trackerId);
			CheckTrackerBattery(trackerId);
		}
		#endregion

		#region Vibration
		public bool TriggerTrackerVibration(TrackerId trackerId, UInt32 durationMicroSec = 500000, UInt32 frequency = 0, float amplitude = 0.5f)
		{
			amplitude = Mathf.Clamp(amplitude, 0, 1);
			float durationSec = durationMicroSec / 1000000;

			if (UseXRData())
			{
				return InputDeviceTracker.HapticPulse(trackerId.InputDevice(), durationMicroSec, frequency, amplitude);
			}

			if (IsTrackerConnected(trackerId) && s_TrackerCaps[trackerId.Num()].supportsHapticVibration)
			{
				WVR_Result result = Interop.WVR_TriggerTrackerVibration(trackerId.Id(), durationMicroSec, frequency, amplitude);
				DEBUG("TriggerTrackerVibration() " + trackerId);

				return (result == WVR_Result.WVR_Success);
			}

			return false;
		}
		#endregion

		#region Extended Data
		const int kTrackerExtDataTypeSize = 4;
		private Dictionary<TrackerId, Int32[]> s_TrackerExtData = new Dictionary<TrackerId, Int32[]>();
		private Dictionary<TrackerId, UInt64> s_TrackerTimestamp = new Dictionary<TrackerId, UInt64>();
		private void UpdateTrackerExtData()
		{
			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				TrackerId tracker = TrackerUtils.s_TrackerIds[i];
				if (!IsTrackerConnected(tracker)) { continue; }

				Int32 exDataSize = 0;
				UInt64 timestamp = 0;
				IntPtr exData = Interop.WVR_GetTrackerExtendedData(tracker.Id(), ref exDataSize, ref timestamp);

				s_TrackerTimestamp[tracker] = timestamp;
				if (exDataSize > 0)
				{
					s_TrackerExtData[tracker] = new Int32[exDataSize];
					for (int d = 0; d < exDataSize; d++)
					{
						s_TrackerExtData[tracker][d] = Marshal.ReadInt32(exData, d * kTrackerExtDataTypeSize);
					}
				}
			}
		}
		#endregion

		#region Device Name
		private Dictionary<TrackerId, bool> s_HasTrackerDeviceName = new Dictionary<TrackerId, bool>();
		private Dictionary<TrackerId, string> s_TrackerDeviceName = new Dictionary<TrackerId, string>();
		private void UpdateTrackerDeviceName(TrackerId trackerId)
		{
			if (IsTrackerConnected(trackerId))
			{
				UInt32 nameSize = 0;
				IntPtr deviceNamePtr = IntPtr.Zero;
				WVR_Result result = Interop.WVR_GetTrackerDeviceName(trackerId.Id(), ref nameSize, ref deviceNamePtr);
				if (result == WVR_Result.WVR_Success && nameSize > 0)
				{
					deviceNamePtr = Marshal.AllocHGlobal((int)nameSize);
					result = Interop.WVR_GetTrackerDeviceName(trackerId.Id(), ref nameSize, ref deviceNamePtr);
					s_HasTrackerDeviceName[trackerId] = (result == WVR_Result.WVR_Success);
					if (s_HasTrackerDeviceName[trackerId])
					{
						s_TrackerDeviceName[trackerId] = Marshal.PtrToStringAnsi(deviceNamePtr);
					}
				}
				else
				{
					s_HasTrackerDeviceName[trackerId] = false;
				}
				Marshal.FreeHGlobal(deviceNamePtr);
				DEBUG("UpdateTrackerDeviceName() " + trackerId + " nameSize: " + nameSize + ", " + s_TrackerDeviceName[trackerId]);
			}
			else
			{
				s_HasTrackerDeviceName[trackerId] = false;
			}
		}
		#endregion

		#region Public Interface
		public TrackerStatus GetTrackerStatus()
		{
			if (UseXRData())
			{
				TrackerStatus status = TrackerStatus.NotStart;
				uint statusId = (uint)status;

				SettingsHelper.GetInt(kTrackerComponentStatus, ref statusId);

				if (statusId == 0) { status = TrackerStatus.NotStart; }
				if (statusId == 1) { status = TrackerStatus.StartFailure; }
				if (statusId == 2) { status = TrackerStatus.Starting; }
				if (statusId == 3) { status = TrackerStatus.Stopping; }
				if (statusId == 4) { status = TrackerStatus.Available; }
				if (statusId == 5) { status = TrackerStatus.NoSupport; }

				return status;
			}

			try
			{
				m_TrackerStatusRWLock.TryEnterReadLock(2000);
				return m_TrackerStatus;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "GetTrackerStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_TrackerStatusRWLock.ExitReadLock();
			}
		}

		public bool IsTrackerConnected(TrackerId trackerId)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.IsAvailable(trackerId.InputDevice());
			}

			return s_TrackerConnection[trackerId];
		}
		public bool IsTrackerPoseValid(TrackerId trackerId)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.IsTracked(trackerId.InputDevice());
			}

			return s_TrackerPoses[trackerId].valid;
		}

		public TrackerRole GetTrackerRole(TrackerId trackerId)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetRole(trackerId.InputDevice()).Role();
			}

			return s_TrackerRole[trackerId];
		}

		public bool GetTrackerPosition(TrackerId trackerId, out Vector3 position)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetPosition(trackerId.InputDevice(), out position);
			}

			position = s_TrackerPoses[trackerId].rigid.pos;
			return s_TrackerPoses[trackerId].valid;
		}
		public Vector3 GetTrackerPosition(TrackerId trackerId)
		{
			if (UseXRData())
			{
				if (GetTrackerPosition(trackerId, out Vector3 value))
				{
					return value;
				}
				else
				{
					return Vector3.zero;
				}
			}

			return s_TrackerPoses[trackerId].rigid.pos;
		}
		public bool GetTrackerRotation(TrackerId trackerId, out Quaternion rotation)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetRotation(trackerId.InputDevice(), out rotation);
			}

			rotation = s_TrackerPoses[trackerId].rigid.rot;
			return s_TrackerPoses[trackerId].valid;
		}
		public Quaternion GetTrackerRotation(TrackerId trackerId)
		{
			if (UseXRData())
			{
				if (GetTrackerRotation(trackerId, out Quaternion value))
				{
					return value;
				}
				else
				{
					return Quaternion.identity;
				}
			}

			return s_TrackerPoses[trackerId].rigid.rot;
		}
		public bool GetTrackerVelocity(TrackerId trackerId, out Vector3 velocity)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetVelocity(trackerId.InputDevice(), out velocity);
			}
			velocity = s_TrackerPoses[trackerId].velocity;
			return s_TrackerPoses[trackerId].valid;
		}
		public Vector3 GetTrackerVelocity(TrackerId trackerId)
		{
			if (UseXRData())
			{
				if (GetTrackerVelocity(trackerId, out Vector3 velocity))
					return velocity;
				else
					return Vector3.zero;
			}
			return s_TrackerPoses[trackerId].velocity;
		}
		public bool GetTrackerAngularVelocity(TrackerId trackerId, out Vector3 angularVelocity)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetAngularVelocity(trackerId.InputDevice(), out angularVelocity);
			}
			angularVelocity = s_TrackerPoses[trackerId].angularVelocity;
			return s_TrackerPoses[trackerId].valid;
		}
		public Vector3 GetTrackerAngularVelocity(TrackerId trackerId)
		{
			if (UseXRData())
			{
				if (GetTrackerAngularVelocity(trackerId, out Vector3 angularVelocity))
					return angularVelocity;
				else
					return Vector3.zero;
			}
			return s_TrackerPoses[trackerId].angularVelocity;
		}

		public AxisType GetTrackerButtonAxisType(TrackerId trackerId, TrackerButton id)
		{
			return s_ButtonAxisType[trackerId][id.Num()];
		}

		public bool TrackerButtonPress(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerPress(trackerId, id.Id());
			return (!ss_TrackerPressEx[trackerId][id.Num()] && ss_TrackerPress[trackerId][id.Num()]);
		}
		public bool TrackerButtonHold(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerPress(trackerId, id.Id());
			return (ss_TrackerPressEx[trackerId][id.Num()] && ss_TrackerPress[trackerId][id.Num()]);
		}
		public bool TrackerButtonRelease(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerPress(trackerId, id.Id());
			return (ss_TrackerPressEx[trackerId][id.Num()] && !ss_TrackerPress[trackerId][id.Num()]);
		}
		public bool TrackerButtonTouch(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerTouch(trackerId, id.Id());
			return (!ss_TrackerTouchEx[trackerId][id.Num()] && ss_TrackerTouch[trackerId][id.Num()]);
		}
		public bool TrackerButtonTouching(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerTouch(trackerId, id.Id());
			return (ss_TrackerTouchEx[trackerId][id.Num()] && ss_TrackerTouch[trackerId][id.Num()]);
		}
		public bool TrackerButtonUntouch(TrackerId trackerId, TrackerButton id)
		{
			UpdateTrackerTouch(trackerId, id.Id());
			return (ss_TrackerTouchEx[trackerId][id.Num()] && !ss_TrackerTouch[trackerId][id.Num()]);
		}
		public Vector2 TrackerButtonAxis(TrackerId trackerId, TrackerButton id)
		{
			if (UseXRData())
			{
				if (id == TrackerButton.Trigger)
				{
					Vector2 axis = Vector2.zero;
					if (InputDeviceTracker.ButtonAxis(trackerId.InputDevice(), CommonUsages.trigger, out float value))
					{
						axis.x = value;
						return axis;
					}
				}

				return Vector2.zero;
			}

			UpdateTrackerAxis(trackerId, id.Id());
			return s_TrackerButtonStates[trackerId].s_ButtonAxis[id.Num()];
		}

		public float GetTrackerBatteryLife(TrackerId trackerId)
		{
			if (UseXRData())
			{
				if (InputDeviceTracker.BatteryLevel(trackerId.InputDevice(), out float value))
					return value;

				return 0;
			}
			return s_TrackerBattery[trackerId];
		}
		public Int32[] GetTrackerExtData(TrackerId trackerId)
		{
			return s_TrackerExtData[trackerId];
		}
		public Int32[] GetTrackerExtData(TrackerId trackerId, out UInt64 timestamp)
		{
			timestamp = s_TrackerTimestamp[trackerId];
			return GetTrackerExtData(trackerId);
		}
		public bool GetTrackerDeviceName(TrackerId trackerId, out string trackerName)
		{
			if (UseXRData())
			{
				return InputDeviceTracker.GetTrackerDeviceName(trackerId.InputDevice(), out trackerName);
			}
			trackerName = s_TrackerDeviceName[trackerId];
			return s_HasTrackerDeviceName[trackerId];
		}
		#endregion
	}
}
