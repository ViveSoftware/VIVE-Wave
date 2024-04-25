// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Threading;
using UnityEngine;
using Wave.Native;
using Wave.OpenXR;
using Wave.XR.Settings;

namespace Wave.Essence.LipExpression
{
	public class LipExpManager : MonoBehaviour
	{
		private const string LOG_TAG = "Wave.Essence.LipExp.LipExpManager";
		private static void DEBUG(string msg) { Log.d(LOG_TAG, msg, true); }
		private static void INFO(string msg) { Log.i(LOG_TAG, msg, true); }

		public delegate void LipExpResultDelegate(object sender, bool result);
		public enum LipExpStatus
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

		#region Inspector
		[SerializeField]
		private bool m_InitialStart = false;
		public bool InitialStart { get { return m_InitialStart; } set { m_InitialStart = value; } }

		[Tooltip("Retrieves the Lip Expression data from UnityEngine.XR.InputDevice.")]
		private bool m_UseXRDevice = true;
		public bool UseXRDevice { get { return m_UseXRDevice; } set { m_UseXRDevice = value; } }

		WaveXRSettings m_WaveXRSettings = null;
		private bool UseXRData()
		{
			// Lip Expression is already enabled in WaveXRSettings.
			bool XRAlreadyEnabled = (m_WaveXRSettings != null ? m_WaveXRSettings.EnableLipExpression : false);

			return (
				(XRAlreadyEnabled || m_UseXRDevice)
				&& (!Application.isEditor)
				);
		}
		#endregion

		private static LipExpManager m_Instance = null;
		public static LipExpManager Instance { get { return m_Instance; } }

		#region MonoBehaviour overrides
		private void Awake()
		{
			m_Instance = this;

			m_WaveXRSettings = WaveXRSettings.GetInstance();

			var supportedFeature = Interop.WVR_GetSupportedFeatures();
			INFO("Awake() supportedFeature: " + supportedFeature);

			if ((supportedFeature & (ulong)WVR_SupportedFeature.WVR_SupportedFeature_LipExp) == 0)
			{
				Log.w(LOG_TAG, "WVR_SupportedFeature_LipExpression is not enabled.", true);
				SetLipExpStatus(LipExpStatus.NoSupport);
			}
		}
		void Start()
		{
			INFO("Start()");
			if (m_InitialStart) { StartLipExp(); }
		}
		private void Update()
		{
			UpdateData();
		}
		#endregion

		#region Life Cycle
		private LipExpStatus m_LipExpStatus = LipExpStatus.NotStart;
		private static ReaderWriterLockSlim m_LipExpStatusRWLock = new ReaderWriterLockSlim();
		private void SetLipExpStatus(LipExpStatus status)
		{
			try
			{
				m_LipExpStatusRWLock.TryEnterWriteLock(2000);
				m_LipExpStatus = status;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "SetLipExpStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_LipExpStatusRWLock.ExitWriteLock();
			}
		}

		private bool CanStartLipExp()
		{
			LipExpStatus status = GetLipExpStatus();
			if (status == LipExpStatus.NotStart ||
				status == LipExpStatus.StartFailure)
			{
				return true;
			}
			return false;
		}
		private bool CanStopLipExp()
		{
			LipExpStatus status = GetLipExpStatus();
			if (status == LipExpStatus.Available)
			{
				return true;
			}
			return false;
		}

		private uint m_LipExpRefCount = 0;
		private object lipExpThreadLocker = new object();
		private event LipExpResultDelegate lipExpResultCB = null;
		private void StartLipExpLock()
		{
			if (!CanStartLipExp()) { return; }

			if (UseXRData())
			{
				DEBUG("StartLipExpLock() XR ");

				#region Input Device
				InputDeviceLip.ActivateLipExp(true);
				#endregion

				if (lipExpResultCB != null) { lipExpResultCB = null; } // Don't support callback when using XR data.
				return;
			}

			SetLipExpStatus(LipExpStatus.Starting);
			WVR_Result result = Interop.WVR_StartLipExp();
			switch (result)
			{
				case WVR_Result.WVR_Success:
					SetLipExpStatus(LipExpStatus.Available);
					break;
				case WVR_Result.WVR_Error_FeatureNotSupport:
					SetLipExpStatus(LipExpStatus.NoSupport);
					break;
				default:
					SetLipExpStatus(LipExpStatus.StartFailure);
					break;
			}

			LipExpStatus status = GetLipExpStatus();
			DEBUG("StartLipExpLock() result: " + result + ", status: " + status);

			if (lipExpResultCB != null)
			{
				lipExpResultCB(this, result == WVR_Result.WVR_Success ? true : false);
				lipExpResultCB = null;
			}
		}
		private void StartLipExpThread()
		{
			lock (lipExpThreadLocker)
			{
				DEBUG("StartLipExpThread()");
				StartLipExpLock();
			}
		}
		public void StartLipExp()
		{
			if (!CanStartLipExp())
			{
				INFO("StartLipExp() can NOT start lip expression.");
				if (lipExpResultCB != null) { lipExpResultCB = null; }
				return;
			}

			string caller = Misc.GetCaller();
			m_LipExpRefCount++;
			INFO("StartLipExp(" + m_LipExpRefCount + ") from " + caller);

			Thread lipExp_t = new Thread(StartLipExpThread);
			lipExp_t.Name = "StartLipExpThread";
			lipExp_t.Start();
		}
		public void StartLipExp(LipExpResultDelegate callback)
		{
			if (lipExpResultCB == null)
			{
				lipExpResultCB = callback;
			}
			else
			{
				lipExpResultCB += callback;
			}

			StartLipExp();
		}

		private void StopLipExpLock()
		{
			if (!CanStopLipExp()) { return; }

			if (UseXRData())
			{
				DEBUG("StopLipExpLock() XR");

				#region Input Device
				InputDeviceLip.ActivateLipExp(false);
				#endregion

				hasLipExpData = false;
				return;
			}

			INFO("StopLipExpLock()");
			SetLipExpStatus(LipExpStatus.Stopping);
			Interop.WVR_StopLipExp();
			SetLipExpStatus(LipExpStatus.NotStart);

			hasLipExpData = false;
		}
		private void StopLipExpThread()
		{
			lock (lipExpThreadLocker)
			{
				DEBUG("StopLipExpThread()");
				StopLipExpLock();
			}
		}
		public void StopLipExp()
		{
			if (!CanStopLipExp())
			{
				INFO("CanStopLipExp() can NOT stop lip expression.");
				return;
			}

			string caller = Misc.GetCaller();
			if (m_LipExpRefCount > 0) { m_LipExpRefCount--; }
			INFO("StopLipExp(" + m_LipExpRefCount + ") from " + caller);
			if (m_LipExpRefCount != 0) { return; }

			Thread tracker_t = new Thread(StopLipExpThread);
			tracker_t.Name = "StopLipExpThread";
			tracker_t.Start();
		}
		#endregion

		#region Lip Expression Data
		private float[] m_LipExpValues = new float[(uint)LipExp.Max];
		private bool hasLipExpData = false;
		void UpdateData()
		{
			var status = GetLipExpStatus();
			if (status == LipExpStatus.Available)
			{
				if (UseXRData())
				{
					hasLipExpData = InputDeviceLip.HasLipExpValue();
					for (int i = 0; i < InputDeviceLip.s_LipExps.Length; i++)
					{
						m_LipExpValues[i] = hasLipExpData ? InputDeviceLip.GetLipExpValue(InputDeviceLip.s_LipExps[i]) : 0;
					}
				}
				else
				{
					var result = Interop.WVR_GetLipExpData(m_LipExpValues);
					hasLipExpData = (result == WVR_Result.WVR_Success);
				}
			}
			else
			{
				hasLipExpData = false;
			}
		}
		#endregion

		#region Public Functions
		public LipExpStatus GetLipExpStatus()
		{
			if (UseXRData())
			{
				uint statusId = InputDeviceLip.GetLipExpStatus();
				LipExpStatus status = LipExpStatus.NoSupport;

				if (statusId == 0) { status = LipExpStatus.NotStart; }
				if (statusId == 1) { status = LipExpStatus.StartFailure; }
				if (statusId == 2) { status = LipExpStatus.Starting; }
				if (statusId == 3) { status = LipExpStatus.Stopping; }
				if (statusId == 4) { status = LipExpStatus.Available; }
				if (statusId == 5) { status = LipExpStatus.NoSupport; }

				return status;
			}

			try
			{
				m_LipExpStatusRWLock.TryEnterReadLock(2000);
				return m_LipExpStatus;
			}
			catch (Exception e)
			{
				Log.e(LOG_TAG, "GetLipExpStatus() " + e.Message, true);
				throw;
			}
			finally
			{
				m_LipExpStatusRWLock.ExitReadLock();
			}
		}

		public bool IsLipExpEnabled()
		{
			var status = GetLipExpStatus();
			return (status == LipExpStatus.Available);
		}

		public bool HasLipExpData()
		{
			return hasLipExpData;
		}
		public float GetLipExpression(LipExp lipExp)
		{
			if (hasLipExpData && (int)lipExp >= 0 && (int)lipExp < (int)LipExp.Max)
			{
				return m_LipExpValues[(uint)lipExp];
			}
			return 0;
		}
		public bool GetLipExpressions(out float[] lipExps)
		{
			lipExps = m_LipExpValues;
			return hasLipExpData;
		}
		#endregion
	}
}