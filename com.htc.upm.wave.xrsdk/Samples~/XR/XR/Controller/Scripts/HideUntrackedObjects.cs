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
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wave.XR.Sample.Controller
{
    public class HideUntrackedObjects : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        const string LOG_TAG = "Wave.XR.Sample.Controller.HideUntrackedObjects ";
        StringBuilder m_sb = null;
        StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        void DEBUG(StringBuilder msg) { Debug.Log(msg); }

        [Serializable]
        public class ObjectInfo
        {
            public uint DeviceIndex = 0;
            public InputActionReference IsActive;
            public InputActionReference TrackingState;
            public GameObject ObjectToHide;
        }

        [SerializeField]
        public List<ObjectInfo> m_ObjectInfos = new List<ObjectInfo>();
        public List<ObjectInfo> ObjectInfos { get { return m_ObjectInfos; } set { m_ObjectInfos = value; } }

        int printFrame = 0;
        protected bool printIntervalLog = false;

        private void Update()
        {
            printFrame++;
            printFrame %= 300;
            printIntervalLog = (printFrame == 0);

            if (m_ObjectInfos == null) { return; }

            string errMsg = "";

            for (int i = 0; i < m_ObjectInfos.Count; i++)
            {
                bool isActive = false;
				InputTrackingState trackingState = InputTrackingState.None;
                bool positionTracked = false, rotationTracked = false;

				// isActive
				if (Utils.GetButton(m_ObjectInfos[i].IsActive, out bool value, out errMsg))
				{
					isActive = value;
				}
				else
				{
					if (printIntervalLog)
					{
						sb.Clear().Append(LOG_TAG).Append(m_ObjectInfos[i].DeviceIndex)
							.Append(" Update() ").Append(m_ObjectInfos[i].IsActive.action.name).Append(", ").Append(errMsg);
						DEBUG(sb);
					}
				}

                // trackingState
                if (Utils.GetInteger(m_ObjectInfos[i].TrackingState, out InputTrackingState state, out errMsg))
                {
                    trackingState = state;
                }
                else
                {
                    if (printIntervalLog)
                    {
                        sb.Clear().Append(LOG_TAG).Append(m_ObjectInfos[i].DeviceIndex)
                            .Append(" Update() ").Append(m_ObjectInfos[i].TrackingState.action.name).Append(", ").Append(errMsg);
                        DEBUG(sb);
                    }
                }

                if (printIntervalLog)
                {
                    sb.Clear().Append(LOG_TAG).Append(m_ObjectInfos[i].DeviceIndex)
                        .Append("Update() isActive: ").Append(isActive).Append(", trackingState: ").Append(trackingState);
                    DEBUG(sb);
                }

                positionTracked = trackingState.HasFlag(InputTrackingState.Position);
                rotationTracked = trackingState.HasFlag(InputTrackingState.Rotation);

                bool tracked = isActive /*&& positionTracked */&& rotationTracked; // Show the object with 3DoF.
                m_ObjectInfos[i].ObjectToHide.SetActive(tracked);
            }
        }
#endif
	}
}
