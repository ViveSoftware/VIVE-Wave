// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using System.Threading;
using System;
using Wave.Native;
using Wave.Essence.Events;
using Wave.OpenXR;
using Wave.XR.Settings;
using System.Text;
#if UNITY_EDITOR
using Wave.Essence.Editor;
#endif

namespace Wave.Essence.Eye
{
	[DisallowMultipleComponent]
	public sealed class EyeManager : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.Eye.EyeManager";
		void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		void DEBUG(StringBuilder sb) { Log.d(LOG_TAG, sb, true); }
		bool printIntervalLog = false;
		int logFrame = 0;
		void INTERVAL(string msg) { if (printIntervalLog) { DEBUG(msg); } }
		void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		private StringBuilder m_EyeManagerStringBuilder = null;
		internal StringBuilder EyeManagerStringBuilder {
			get {
				if (m_EyeManagerStringBuilder == null) { m_EyeManagerStringBuilder = new StringBuilder(); }
				return m_EyeManagerStringBuilder;
			}
		}

		private static EyeManager m_Instance = null;
		public static EyeManager Instance { get { return m_Instance; } }

		#region Public declaration
		public const string EYE_TRACKING_STATUS = "EYE_TRACKING_STATUS";
		public enum EyeType
		{
			Combined = 0,
			Right,
			Left
		}
		public enum EyeTrackingStatus
		{
			// Initial, can call Start API in this state.
			NOT_START,
			START_FAILURE,

			// Processing, should NOT call API in this state.
			STARTING,
			STOPING,

			// Running, can call Stop API in this state.
			AVAILABLE,

			// Do nothing.
			UNSUPPORT
		}
		public enum EyeSpace : UInt32
		{
			Local = WVR_CoordinateSystem.WVR_CoordinateSystem_Local,
			World = WVR_CoordinateSystem.WVR_CoordinateSystem_Global
		}
		#endregion

		#region Inspector
		private bool m_EnableEyeTrackingEx = true;
		[Tooltip("Enables or disables the eye tracking.")]
		[SerializeField]
		private bool m_EnableEyeTracking = true;
		public bool EnableEyeTracking { get { return m_EnableEyeTracking; } set { m_EnableEyeTracking = value; } }

		[Tooltip("Eye Tracking in local space or world space.")]
		[SerializeField]
		private EyeSpace m_LocationSpace = EyeSpace.World;
		public EyeSpace LocationSpace { get { return m_LocationSpace; } set { m_LocationSpace = value; } }

		private bool m_NormalizeZ = true;
		public bool NormalizeZ { get { return m_NormalizeZ; } set { m_NormalizeZ = value; } }
		#endregion

		static bool m_UseXRDevice = true;
		public bool UseXRDevice { get { return m_UseXRDevice; } set { m_UseXRDevice = value; } }
		private WaveXRSettings m_WaveXRSettings = null;
		private InputDeviceEye.TrackingStatus m_XRTrackingStatus = InputDeviceEye.TrackingStatus.UNSUPPORT;
		bool UseXRData()
		{
			// Tracker is already enabled in WaveXRSettings.
			bool XRAlreadyEnabled = (m_WaveXRSettings != null ? m_WaveXRSettings.EnableEyeTracking : false);

			return (
				(XRAlreadyEnabled || m_UseXRDevice)
				&& (!Application.isEditor)
				);
		}

		#region MonoBehaviour overrides
		private void Awake()
		{
			m_Instance = this;

			var supportedFeature = Interop.WVR_GetSupportedFeatures();
			INFO("Awake() supportedFeature: " + supportedFeature);

			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_EyeTracking) == 0)
			{
				Log.w(LOG_TAG, "WVR_SupportedFeature_EyeTracking is not enabled.", true);
				SetEyeTrackingStatus(EyeTrackingStatus.UNSUPPORT);
			}
		}
		private bool mEnabled = false;
		private void OnEnable()
		{
			if (!mEnabled)
			{
				m_EnableEyeTrackingEx = m_EnableEyeTracking;
				if (m_EnableEyeTracking)
					StartEyeTracking();

				mEnabled = true;
			}
		}
		private void OnDisable()
		{
			if (mEnabled)
			{
				mEnabled = false;
			}
		}
		void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			if (m_EnableEyeTrackingEx != m_EnableEyeTracking)
			{
				m_EnableEyeTrackingEx = m_EnableEyeTracking;
				if (m_EnableEyeTracking)
				{
					DEBUG("Update() Start eye tracking.");
					StartEyeTracking();
				}
				if (!m_EnableEyeTracking)
				{
					DEBUG("Update() Stop eye tracking.");
					StopEyeTracking();
				}
			}

			if (m_EnableEyeTracking)
			{
				GetEyeTrackingData();

				if (printIntervalLog)
				{
					var sb = EyeManagerStringBuilder;
					sb.Clear();
					sb.Append("Update() m_LocationSpace: ").Append(m_LocationSpace).Append(", hasEyeTrackingData: ").Append(hasEyeTrackingData)
					.Append("Update() m_CombinedEyeOriginValid: ").Append(m_CombinedEyeOriginValid)
					.Append(", m_CombinedEyeOrigin (").Append(m_CombinedEyeOrigin.x.ToString()).Append(", ").Append(m_CombinedEyeOrigin.y.ToString()).Append(", ").Append(m_CombinedEyeOrigin.z.ToString()).Append(")")
					.Append(", m_CombinedEyeDirectionValid: ").Append(m_CombinedEyeDirectionValid)
					.Append(", m_CombinedEyeDirection (").Append(m_CombinedEyeDirection.x.ToString()).Append(", ").Append(m_CombinedEyeDirection.y.ToString()).Append(", ").Append(m_CombinedEyeDirection.z.ToString()).Append(")")
					.Append("Update() m_LeftEyeOriginValid: ").Append(m_LeftEyeOriginValid)
					.Append(", m_LeftEyeOrigin (").Append(m_LeftEyeOrigin.x.ToString()).Append(", ").Append(m_LeftEyeOrigin.y.ToString()).Append(", ").Append(m_LeftEyeOrigin.z.ToString()).Append(")")
					.Append(", m_LeftEyeDirectionValid: ").Append(m_LeftEyeDirectionValid)
					.Append(", m_LeftEyeDirection (").Append(m_LeftEyeDirection.x.ToString()).Append(", ").Append(m_LeftEyeDirection.y.ToString()).Append(", ").Append(m_LeftEyeDirection.z.ToString()).Append(")")
					.Append("Update() m_RightEyeOriginValid: ").Append(m_RightEyeOriginValid)
					.Append(", m_RightEyeOrigin (").Append(m_RightEyeOrigin.x.ToString()).Append(", ").Append(m_RightEyeOrigin.y.ToString()).Append(", ").Append(m_RightEyeOrigin.z.ToString()).Append(")")
					.Append(", m_RightEyeDirectionValid: ").Append(m_RightEyeDirectionValid)
					.Append(", m_RightEyeDirection (").Append(m_RightEyeDirection.x.ToString()).Append(", ").Append(m_RightEyeDirection.y.ToString()).Append(", ").Append(m_RightEyeDirection.z.ToString()).Append(")");
					DEBUG(sb);
				}
			}
		}
		#endregion

		#region Eye Tracking Lifecycle
		private EyeTrackingStatus m_EyeTrackingStatus = EyeTrackingStatus.NOT_START;
		private static ReaderWriterLockSlim m_EyeTrackingStatusRWLock = new ReaderWriterLockSlim();
		private void SetEyeTrackingStatus(EyeTrackingStatus status)
		{
			try
			{
				m_EyeTrackingStatusRWLock.TryEnterWriteLock(2000);
				m_EyeTrackingStatus = status;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "SetEyeTrackingStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_EyeTrackingStatusRWLock.ExitWriteLock();
			}
		}

		private bool CanStartEyeTracking()
		{
			EyeTrackingStatus status = GetEyeTrackingStatus();
			if (!m_EnableEyeTracking ||
				status == EyeTrackingStatus.AVAILABLE ||
				status == EyeTrackingStatus.STARTING ||
				status == EyeTrackingStatus.STOPING ||
				status == EyeTrackingStatus.UNSUPPORT)
			{
				return false;
			}

			return true;
		}
		private bool CanStopEyeTracking()
		{
			EyeTrackingStatus status = GetEyeTrackingStatus();
			if (!m_EnableEyeTracking && status == EyeTrackingStatus.AVAILABLE)
				return true;
			return false;
		}

		public delegate void EyeTrackingResultDelegate(object sender, bool result);
		private object m_EyeTrackingThreadLock = new object();
		private event EyeTrackingResultDelegate m_EyeTrackingResultCB = null;
		private void StartEyeTrackingLock()
		{
			if (UseXRData())
			{
				DEBUG("StartEyeTrackingLock() XR");
				InputDeviceEye.ActivateEyeTracking(true);
				return;
			}
			if (!CanStartEyeTracking())
				return;

			SetEyeTrackingStatus(EyeTrackingStatus.STARTING);
			WVR_Result result = Interop.WVR_StartEyeTracking();
			switch (result)
			{
				case WVR_Result.WVR_Success:
					SetEyeTrackingStatus(EyeTrackingStatus.AVAILABLE);
					break;
				case WVR_Result.WVR_Error_FeatureNotSupport:
					SetEyeTrackingStatus(EyeTrackingStatus.UNSUPPORT);
					break;
				default:
					SetEyeTrackingStatus(EyeTrackingStatus.START_FAILURE);
					break;
			}

			EyeTrackingStatus status = GetEyeTrackingStatus();
			INFO("StartEyeTrackingLock() result: " + result + ", status: " + status);
			GeneralEvent.Send(EYE_TRACKING_STATUS, status);

			if (m_EyeTrackingResultCB != null)
			{
				m_EyeTrackingResultCB(this, result == WVR_Result.WVR_Success ? true : false);
				m_EyeTrackingResultCB = null;
			}
		}
		private void StartEyeTrackingThread()
		{
			lock (m_EyeTrackingThreadLock)
			{
				DEBUG("StartEyeTrackingThread()");
				StartEyeTrackingLock();
			}
		}
		private void StartEyeTracking()
		{
			if (!CanStartEyeTracking())
				return;

			INFO("StartEyeTracking()");
			Thread eye_tracking_t = new Thread(StartEyeTrackingThread);
			eye_tracking_t.Name = "StartEyeTrackingThread";
			eye_tracking_t.Start();
		}

		private void StopEyeTrackingLock()
		{
			if (UseXRData())
			{
				DEBUG("StopEyeTrackingLock() XR");
				InputDeviceEye.ActivateEyeTracking(false);
				hasEyeTrackingData = false;
				return;
			}

			if (!CanStopEyeTracking())
				return;

			INFO("StopEyeTrackingLock()");
			SetEyeTrackingStatus(EyeTrackingStatus.STOPING);
			Interop.WVR_StopEyeTracking();
			SetEyeTrackingStatus(EyeTrackingStatus.NOT_START);
			hasEyeTrackingData = false;

			EyeTrackingStatus status = GetEyeTrackingStatus();
			GeneralEvent.Send(EYE_TRACKING_STATUS, status);
		}
		private void StopEyeTrackingThread()
		{
			lock (m_EyeTrackingThreadLock)
			{
				DEBUG("StopEyeTrackingThread()");
				StopEyeTrackingLock();
			}
		}
		private void StopEyeTracking()
		{
			if (!CanStopEyeTracking())
				return;

			INFO("StopEyeTracking()");
			Thread eye_tracking_t = new Thread(StopEyeTrackingThread);
			eye_tracking_t.Name = "StopEyeTrackingThread";
			eye_tracking_t.Start();
		}

		private void RestartEyeTrackingThread()
		{
			lock (m_EyeTrackingThreadLock)
			{
				INFO("RestartEyeTrackingThread()");
				StopEyeTrackingLock();
				StartEyeTrackingLock();
			}
		}
		/// <summary> Restarts the eye tracking service. </summary>
		public void RestartEyeTracking()
		{
			EyeTrackingStatus status = GetEyeTrackingStatus();
			if (status == EyeTrackingStatus.STARTING || status == EyeTrackingStatus.STOPING)
				return;
			Thread eye_tracking_t = new Thread(RestartEyeTrackingThread);
			eye_tracking_t.Name = "RestartEyeTrackingThread";
			eye_tracking_t.Start();
		}
		public void RestartEyeTracking(EyeTrackingResultDelegate callback)
		{
			if (m_EyeTrackingResultCB == null)
				m_EyeTrackingResultCB = callback;
			else
				m_EyeTrackingResultCB += callback;

			RestartEyeTracking();
		}
		#endregion

		#region Eye Tracking Data
		private WVR_EyeTracking_t m_EyeData = new WVR_EyeTracking_t();
		private bool m_CombinedEyeOriginValid = false, m_CombinedEyeDirectionValid = false;
		private Vector3 m_CombinedEyeOrigin = Vector3.zero, m_CombinedEyeDirection = Vector3.zero;

		private bool m_LeftEyeOriginValid = false, m_LeftEyeDirectionValid = false, m_LeftEyeOpennessValid = false, m_LeftEyePupilDiameterValid = false, m_LeftEyePupilPositionInSensorAreaValid = false;
		private Vector3 m_LeftEyeOrigin = Vector3.zero, m_LeftEyeDirection = Vector3.zero;
		private float m_LeftEyeOpenness = 0, m_LeftEyePupilDiameter = 0;
		private Vector2 m_LeftEyePupilPositionInSensorArea = Vector2.zero;

		private bool m_RightEyeOriginValid = false, m_RightEyeDirectionValid = false, m_RightEyeOpennessValid = false, m_RightEyePupilDiameterValid = false, m_RightEyePupilPositionInSensorAreaValid = false;
		private Vector3 m_RightEyeOrigin = Vector3.zero, m_RightEyeDirection = Vector3.zero;
		private float m_RightEyeOpenness = 0, m_RightEyePupilDiameter = 0;
		private Vector2 m_RightEyePupilPositionInSensorArea = Vector2.zero;

		private bool hasEyeTrackingData = false;
		private void GetEyeTrackingData()
		{
			if (UseXRData())
			{
				InputDeviceEye.SetEyeTrackingSpace(m_LocationSpace.TrackingSpace());
				hasEyeTrackingData = InputDeviceEye.IsEyeTrackingTracked();
				if (hasEyeTrackingData)
				{
					m_CombinedEyeOriginValid = InputDeviceEye.GetCombinedEyeOrigin(out m_CombinedEyeOrigin);
					m_CombinedEyeDirectionValid = InputDeviceEye.GetCombinedEyeDirection(out m_CombinedEyeDirection);

					m_LeftEyeOriginValid = InputDeviceEye.GetLeftEyeOrigin(out m_LeftEyeOrigin);
					m_LeftEyeDirectionValid = InputDeviceEye.GetLeftEyeDirection(out m_LeftEyeDirection);
					m_LeftEyeOpennessValid = InputDeviceEye.GetLeftEyeOpenness(out m_LeftEyeOpenness);
					m_LeftEyePupilDiameterValid = InputDeviceEye.GetLeftEyePupilDiameter(out m_LeftEyePupilDiameter);
					m_LeftEyePupilPositionInSensorAreaValid = InputDeviceEye.GetLeftEyePupilPositionInSensorArea(out m_LeftEyePupilPositionInSensorArea);

					m_RightEyeOriginValid = InputDeviceEye.GetRightEyeOrigin(out m_RightEyeOrigin);
					m_RightEyeDirectionValid = InputDeviceEye.GetRightEyeDirection(out m_RightEyeDirection);
					m_RightEyeOpennessValid = InputDeviceEye.GetRightEyeOpenness(out m_RightEyeOpenness);
					m_RightEyePupilDiameterValid = InputDeviceEye.GetRightEyePupilDiameter(out m_RightEyePupilDiameter);
					m_RightEyePupilPositionInSensorAreaValid = InputDeviceEye.GetRightEyePupilPositionInSensorArea(out m_RightEyePupilPositionInSensorArea);
				}
				return;
			}

			EyeTrackingStatus status = GetEyeTrackingStatus();
			if (status == EyeTrackingStatus.AVAILABLE)
			{
				hasEyeTrackingData = Interop.WVR_GetEyeTracking(ref m_EyeData, (WVR_CoordinateSystem)m_LocationSpace) == WVR_Result.WVR_Success ? true : false;
				if (hasEyeTrackingData)
				{
					/// Combined eye data.
					m_CombinedEyeOriginValid =
						((m_EyeData.combined.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeOriginValid) != 0);
					if (m_CombinedEyeOriginValid)
						Coordinate.GetVectorFromGL(m_EyeData.combined.gazeOrigin, out m_CombinedEyeOrigin);

					m_CombinedEyeDirectionValid = 
						((m_EyeData.combined.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid) != 0);
					if (m_CombinedEyeDirectionValid)
					{
						Coordinate.GetVectorFromGL(m_EyeData.combined.gazeDirectionNormalized, out m_CombinedEyeDirection);
						if (m_NormalizeZ) { Coordinate.Vector3NormalizeZ(ref m_CombinedEyeDirection); }
					}

					/// Left eye data.
					m_LeftEyeOriginValid =
						((m_EyeData.left.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeOriginValid) != 0);
					if (m_LeftEyeOriginValid)
						Coordinate.GetVectorFromGL(m_EyeData.left.gazeOrigin, out m_LeftEyeOrigin);

					m_LeftEyeDirectionValid =
						((m_EyeData.left.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid) != 0);
					if (m_LeftEyeDirectionValid)
					{
						Coordinate.GetVectorFromGL(m_EyeData.left.gazeDirectionNormalized, out m_LeftEyeDirection);
						if (m_NormalizeZ) { Coordinate.Vector3NormalizeZ(ref m_LeftEyeDirection); }
					}

					m_LeftEyeOpennessValid =
						((m_EyeData.left.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_EyeOpennessValid) != 0);
					if (m_LeftEyeOpennessValid)
						m_LeftEyeOpenness = m_EyeData.left.eyeOpenness;

					m_LeftEyePupilDiameterValid =
						((m_EyeData.left.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_PupilDiameterValid) != 0);
					if (m_LeftEyePupilDiameterValid)
						m_LeftEyePupilDiameter = m_EyeData.left.pupilDiameter;

					m_LeftEyePupilPositionInSensorAreaValid =
						((m_EyeData.left.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_PupilPositionInSensorAreaValid) != 0);
					if (m_LeftEyePupilPositionInSensorAreaValid)
					{
						m_LeftEyePupilPositionInSensorArea.x = m_EyeData.left.pupilPositionInSensorArea.v0;
						m_LeftEyePupilPositionInSensorArea.y = m_EyeData.left.pupilPositionInSensorArea.v1;
					}

					/// Right eye data.
					m_RightEyeOriginValid =
						((m_EyeData.right.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeOriginValid) != 0);
					if (m_RightEyeOriginValid)
						Coordinate.GetVectorFromGL(m_EyeData.right.gazeOrigin, out m_RightEyeOrigin);

					m_RightEyeDirectionValid =
						((m_EyeData.right.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_GazeDirectionNormalizedValid) != 0);
					if (m_RightEyeDirectionValid)
					{
						Coordinate.GetVectorFromGL(m_EyeData.right.gazeDirectionNormalized, out m_RightEyeDirection);
						if (m_NormalizeZ) { Coordinate.Vector3NormalizeZ(ref m_RightEyeDirection); }
					}

					m_RightEyeOpennessValid =
						((m_EyeData.right.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_EyeOpennessValid) != 0);
					if (m_RightEyeOpennessValid)
						m_RightEyeOpenness = m_EyeData.right.eyeOpenness;

					m_RightEyePupilDiameterValid =
						((m_EyeData.right.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_PupilDiameterValid) != 0);
					if (m_RightEyePupilDiameterValid)
						m_RightEyePupilDiameter = m_EyeData.right.pupilDiameter;

					m_RightEyePupilPositionInSensorAreaValid =
						((m_EyeData.right.eyeTrackingValidBitMask & (ulong)WVR_EyeTrackingStatus.WVR_PupilPositionInSensorAreaValid) != 0);
					if (m_RightEyePupilPositionInSensorAreaValid)
					{
						m_RightEyePupilPositionInSensorArea.x = m_EyeData.right.pupilPositionInSensorArea.v0;
						m_RightEyePupilPositionInSensorArea.y = m_EyeData.right.pupilPositionInSensorArea.v1;
					}
				}
			}
			else
			{
				hasEyeTrackingData = false;
			}
		}
		#endregion

		#region Public Functions
		/// <summary> Retrieves current eye tracking service status. </summary>
		public EyeTrackingStatus GetEyeTrackingStatus()
		{
			if (UseXRData())
			{
				m_XRTrackingStatus = InputDeviceEye.GetEyeTrackingStatus();

				// Sends an event when status changes.
				bool sendEvent = false;
				if (m_XRTrackingStatus != m_EyeTrackingStatus.TrackingStatus())
				{
					sendEvent = true;

					switch (m_XRTrackingStatus)
					{
						case InputDeviceEye.TrackingStatus.NOT_START:
							m_EyeTrackingStatus = EyeTrackingStatus.NOT_START;
							break;
						case InputDeviceEye.TrackingStatus.START_FAILURE:
							m_EyeTrackingStatus = EyeTrackingStatus.START_FAILURE;
							break;
						case InputDeviceEye.TrackingStatus.STARTING:
							m_EyeTrackingStatus = EyeTrackingStatus.STARTING;
							break;
						case InputDeviceEye.TrackingStatus.STOPPING:
							m_EyeTrackingStatus = EyeTrackingStatus.STOPING;
							break;
						case InputDeviceEye.TrackingStatus.AVAILABLE:
							m_EyeTrackingStatus = EyeTrackingStatus.AVAILABLE;
							break;
						default:
							m_EyeTrackingStatus = EyeTrackingStatus.UNSUPPORT;
							break;
					}
				}

				if (sendEvent) { GeneralEvent.Send(EYE_TRACKING_STATUS, m_EyeTrackingStatus); }
			}

			try
			{
				m_EyeTrackingStatusRWLock.TryEnterReadLock(2000);
				return m_EyeTrackingStatus;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "GetEyeTrackingStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_EyeTrackingStatusRWLock.ExitReadLock();
			}
		}
		/// <summary> Checks whether the eye tracking service is available. </summary>
		public bool IsEyeTrackingAvailable()
		{
			EyeTrackingStatus status = GetEyeTrackingStatus();
			if (status == EyeTrackingStatus.AVAILABLE)
				return true;

			return false;
		}
		/// <summary> Checks if the eye tracking data is provided. </summary>
		public bool HasEyeTrackingData() { return hasEyeTrackingData; }

		/// <summary> Retrieves the origin location of specified eye. </summary>
		public bool GetEyeOrigin(EyeType eye, out Vector3 origin)
		{
			origin = Vector3.zero;

			if (eye == EyeType.Combined) { return GetCombinedEyeOrigin(out origin); }
			if (eye == EyeType.Left) { return GetLeftEyeOrigin(out origin); }
			if (eye == EyeType.Right) { return GetRightEyeOrigin(out origin); }

			return false;
		}
		public bool GetEyeDirectionNormalized(EyeType eye, out Vector3 direction)
		{
			direction = Vector3.zero;

			if (eye == EyeType.Combined) { return GetCombindedEyeDirectionNormalized(out direction); }
			if (eye == EyeType.Left) { return GetLeftEyeDirectionNormalized(out direction); }
			if (eye == EyeType.Right) { return GetRightEyeDirectionNormalized(out direction); }

			return false;
		}

		// ------------------------- Combined Eye -------------------------
		/// <summary> Retrieves the origin location of combined eye. </summary>
		public bool GetCombinedEyeOrigin(out Vector3 origin)
		{
			if (!hasEyeTrackingData)
			{
				origin = Vector3.zero;
				return false;
			}
			origin = m_CombinedEyeOrigin;
			return m_CombinedEyeOriginValid;
		}
		/// <summary> Retrieves the looking direction (z-normalized) of combined eye. </summary>
		public bool GetCombindedEyeDirectionNormalized(out Vector3 direction)
		{
			if (!hasEyeTrackingData)
			{
				direction = Vector3.zero;
				return false;
			}
			direction = m_CombinedEyeDirection;
			return m_CombinedEyeDirectionValid;
		}

		// ------------------------- Left Eye -------------------------
		/// <summary> Retrieves the origin location of left eye. </summary>
		public bool GetLeftEyeOrigin(out Vector3 origin)
		{
			if (!hasEyeTrackingData)
			{
				origin = Vector3.zero;
				return false;
			}
			origin = m_LeftEyeOrigin;
			return m_LeftEyeOriginValid;
		}
		/// <summary> Retrieves the looking direction (z-normalized) of left eye. </summary>
		public bool GetLeftEyeDirectionNormalized(out Vector3 direction)
		{
			if (!hasEyeTrackingData)
			{
				direction = Vector3.zero;
				return false;
			}
			direction = m_LeftEyeDirection;
			return m_LeftEyeDirectionValid;
		}
		/// <summary> Retrieves the value representing how open the left eye is. </summary>
		public bool GetLeftEyeOpenness(out float openness)
		{
			if (!hasEyeTrackingData)
			{
				openness = 0;
				return false;
			}
			openness = m_LeftEyeOpenness;
			return m_LeftEyeOpennessValid;
		}
		/// <summary> Retrieves the diameter of left eye pupil in millimeters. </summary>
		public bool GetLeftEyePupilDiameter(out float diameter)
		{
			if (!hasEyeTrackingData)
			{
				diameter = 0;
				return false;
			}
			diameter = m_LeftEyePupilDiameter;
			return m_LeftEyePupilDiameterValid;
		}
		/// <summary> Retrieves the normalized position of left eye pupil in [0,1]. </summary>
		public bool GetLeftEyePupilPositionInSensorArea(out Vector2 area)
		{
			if (!hasEyeTrackingData)
			{
				area = Vector2.zero;
				return false;
			}
			area = m_LeftEyePupilPositionInSensorArea;
			return m_LeftEyePupilPositionInSensorAreaValid;
		}

		// ------------------------- Right Eye -------------------------
		/// <summary> Retrieves the origin location of right eye. </summary>
		public bool GetRightEyeOrigin(out Vector3 origin)
		{
			if (!hasEyeTrackingData)
			{
				origin = Vector3.zero;
				return false;
			}
			origin = m_RightEyeOrigin;
			return m_RightEyeOriginValid;
		}
		/// <summary> Retrieves the looking direction (z-normalized) of right eye. </summary>
		public bool GetRightEyeDirectionNormalized(out Vector3 direction)
		{
			if (!hasEyeTrackingData)
			{
				direction = Vector3.zero;
				return false;
			}
			direction = m_RightEyeDirection;
			return m_RightEyeDirectionValid;
		}
		/// <summary> Retrieves the value representing how open the right eye is. </summary>
		public bool GetRightEyeOpenness(out float openness)
		{
			if (!hasEyeTrackingData)
			{
				openness = 0;
				return false;
			}
			openness = m_RightEyeOpenness;
			return m_RightEyeOpennessValid;
		}
		/// <summary> Retrieves the diameter of right eye pupil in millimeters. </summary>
		public bool GetRightEyePupilDiameter(out float diameter)
		{
			if (!hasEyeTrackingData)
			{
				diameter = 0;
				return false;
			}
			diameter = m_RightEyePupilDiameter;
			return m_RightEyePupilDiameterValid;
		}
		/// <summary> Retrieves the normalized position of right eye pupil in [0,1]. </summary>
		public bool GetRightEyePupilPositionInSensorArea(out Vector2 area)
		{
			if (!hasEyeTrackingData)
			{
				area = Vector2.zero;
				return false;
			}
			area = m_RightEyePupilPositionInSensorArea;
			return m_RightEyePupilPositionInSensorAreaValid;
		}
		#endregion
	}

	public static class EyeManagerHelper
	{
		public static InputDeviceEye.TrackingSpace TrackingSpace(this EyeManager.EyeSpace space)
		{
			if (space == EyeManager.EyeSpace.Local) { return InputDeviceEye.TrackingSpace.Local; }
			return InputDeviceEye.TrackingSpace.World;
		}

		public static InputDeviceEye.TrackingStatus TrackingStatus(this EyeManager.EyeTrackingStatus status)
		{
			switch(status)
			{
				case EyeManager.EyeTrackingStatus.NOT_START:
					return InputDeviceEye.TrackingStatus.NOT_START;
				case EyeManager.EyeTrackingStatus.START_FAILURE:
					return InputDeviceEye.TrackingStatus.START_FAILURE;
				case EyeManager.EyeTrackingStatus.STARTING:
					return InputDeviceEye.TrackingStatus.STARTING;
				case EyeManager.EyeTrackingStatus.STOPING:
					return InputDeviceEye.TrackingStatus.STOPPING;
				case EyeManager.EyeTrackingStatus.AVAILABLE:
					return InputDeviceEye.TrackingStatus.AVAILABLE;
				default:
					return InputDeviceEye.TrackingStatus.UNSUPPORT;
			}
		}
	}
}
