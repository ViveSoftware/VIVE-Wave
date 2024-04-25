// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Linq;
using UnityEngine;
using Wave.OpenXR;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System.Text;

using PoseState = UnityEngine.InputSystem.XR.PoseState;

namespace Wave.XR
{
    public struct ViveWaveSelfTrackerState : IInputStateTypeInfo
    {
        // You must tag every state with a FourCC code for type
        // checking. The characters can be anything. Choose something that allows
        // you to easily recognize memory that belongs to your own Device.
        public FourCC format => new FourCC('W', 'A', 'V', 'E');

        [InputControl(name = "pose", layout = "Pose", format = "Pose", offset = 0, displayName = "Pose")]
        public PoseState pose;
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Vive Self Tracker (Wave)", commonUsages = new[] { "Tracker 0", "Tracker 1", "Tracker 2", "Tracker 3", "Tracker 4" }, stateType = typeof(ViveWaveSelfTrackerState), isGenericTypeOfDevice = true)]
    public class ViveWaveSelfTracker : InputDevice, IInputUpdateCallbackReceiver
    {
        const string LOG_TAG = "Wave.XR.ViveWaveSelfTracker ";
        private static StringBuilder m_sb = null;
        private static StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        static void DEBUG(StringBuilder msg) { Debug.Log(msg); }
        uint logFrame = 0;
        static bool printIntervalLog = false;

        const string kInterfaceName = "Self Tracker";
        const string kProductUltimateTracker0 = "VIVE Ultimate Tracker 0";
        const string kProductUltimateTracker1 = "VIVE Ultimate Tracker 1";
        const string kProductUltimateTracker2 = "VIVE Ultimate Tracker 2";
        const string kProductUltimateTracker3 = "VIVE Ultimate Tracker 3";
        const string kProductUltimateTracker4 = "VIVE Ultimate Tracker 4";
        private InputDeviceTracker.TrackerId m_TrackerId {
            get {
                // Self Tracker index starts from 2 though 0 & 1 are reserved for Wrist Tracker.
                if (description.product.Equals(kProductUltimateTracker0)) { return InputDeviceTracker.TrackerId.Tracker2; }
                if (description.product.Equals(kProductUltimateTracker1)) { return InputDeviceTracker.TrackerId.Tracker3; }
                if (description.product.Equals(kProductUltimateTracker2)) { return InputDeviceTracker.TrackerId.Tracker4; }
                if (description.product.Equals(kProductUltimateTracker3)) { return InputDeviceTracker.TrackerId.Tracker5; }
                if (description.product.Equals(kProductUltimateTracker4)) { return InputDeviceTracker.TrackerId.Tracker6; }

                return InputDeviceTracker.TrackerId.Tracker0;
            }
        }

        const string kUltimateTrackerSN0 = "VIVE_Ultimate_Tracker_0";
        const string kUltimateTrackerSN1 = "VIVE_Ultimate_Tracker_1";
        const string kUltimateTrackerSN2 = "VIVE_Ultimate_Tracker_2";
        const string kUltimateTrackerSN3 = "VIVE_Ultimate_Tracker_3";
        const string kUltimateTrackerSN4 = "VIVE_Ultimate_Tracker_4";

        const string kTrackerUsage0 = "Tracker 0";
        const string kTrackerUsage1 = "Tracker 1";
        const string kTrackerUsage2 = "Tracker 2";
        const string kTrackerUsage3 = "Tracker 3";
        const string kTrackerUsage4 = "Tracker 4";
        private string m_TrackerUsage {
            get {
                if (description.product.Equals(kProductUltimateTracker0)) { return kTrackerUsage0; }
                if (description.product.Equals(kProductUltimateTracker1)) { return kTrackerUsage1; }
                if (description.product.Equals(kProductUltimateTracker2)) { return kTrackerUsage2; }
                if (description.product.Equals(kProductUltimateTracker3)) { return kTrackerUsage3; }
                if (description.product.Equals(kProductUltimateTracker4)) { return kTrackerUsage4; }

                return "";
            }
        }

#if UNITY_EDITOR
        static ViveWaveSelfTracker()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            sb.Clear().Append(LOG_TAG).Append("Initialize() RegisterLayout with interface ").Append(kInterfaceName);
            DEBUG(sb);
            InputSystem.RegisterLayout(
                typeof(ViveWaveSelfTracker),
                matches: new InputDeviceMatcher()
                        .WithInterface(kInterfaceName)
            );

            var device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveSelfTracker);
            while (device != null)
            {
                sb.Clear().Append(LOG_TAG).Append("Initialize() Removes a ViveWaveSelfTracker device."); DEBUG(sb);
                InputSystem.RemoveDevice(device);
                device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveSelfTracker);
            }

            sb.Clear().Append(LOG_TAG).Append("Initialize() Adds ViveWaveSelfTracker device ").Append(kProductUltimateTracker0);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductUltimateTracker0, serial = kUltimateTrackerSN0 }
            );
            sb.Clear().Append(LOG_TAG).Append("Initialize() Adds ViveWaveSelfTracker device ").Append(kProductUltimateTracker1);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductUltimateTracker1, serial = kUltimateTrackerSN1 }
            );
            sb.Clear().Append(LOG_TAG).Append("Initialize() Adds ViveWaveSelfTracker device ").Append(kProductUltimateTracker2);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductUltimateTracker2, serial = kUltimateTrackerSN2 }
            );
            sb.Clear().Append(LOG_TAG).Append("Initialize() Adds ViveWaveSelfTracker device ").Append(kProductUltimateTracker3);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductUltimateTracker3, serial = kUltimateTrackerSN3 }
            );
            sb.Clear().Append(LOG_TAG).Append("Initialize() Adds ViveWaveSelfTracker device ").Append(kProductUltimateTracker4);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductUltimateTracker4, serial = kUltimateTrackerSN4 }
            );
        }

        [InputControl]
        public PoseControl pose { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            pose = GetChildControl<PoseControl>("pose");

            InputSystem.SetDeviceUsage(this, m_TrackerUsage);

            sb.Clear().Append(LOG_TAG).Append("FinishSetup() deviceId: ").Append(deviceId)
                .Append(", Usage: ").Append(m_TrackerUsage)
                .Append(", interfaceName: ").Append(description.interfaceName)
                .Append(", product: ").Append(description.product)
                .Append(", serial: ").Append(description.serial);
            DEBUG(sb);
        }

        public void OnUpdate()
        {
            if (Application.isEditor || !Application.isPlaying) { return; }

            logFrame++;
            logFrame %= 300;
            printIntervalLog = (logFrame == 0);

            if (!InputDeviceTracker.IsAvailable())
            {
                if (printIntervalLog) { sb.Clear().Append(LOG_TAG).Append("OnUpdate() No tracker connected."); DEBUG(sb); }
                return;
            }

            ViveWaveSelfTrackerState state = new ViveWaveSelfTrackerState();

            state.pose.isTracked = InputDeviceTracker.IsTracked(m_TrackerId);
            if (InputDeviceTracker.GetPosition(m_TrackerId, out Vector3 pos))
            {
                state.pose.trackingState |= UnityEngine.XR.InputTrackingState.Position;
                state.pose.position = pos;
            }
            if (InputDeviceTracker.GetRotation(m_TrackerId, out Quaternion rot))
            {
                state.pose.trackingState |= UnityEngine.XR.InputTrackingState.Rotation;
                state.pose.rotation = rot;
            }
            if (InputDeviceTracker.GetVelocity(m_TrackerId, out Vector3 vel))
            {
                state.pose.trackingState |= UnityEngine.XR.InputTrackingState.Velocity;
                state.pose.velocity = vel;
            }
            if (InputDeviceTracker.GetAngularVelocity(m_TrackerId, out Vector3 angularVel))
            {
                state.pose.trackingState |= UnityEngine.XR.InputTrackingState.AngularVelocity;
                state.pose.angularVelocity = angularVel;
            }

            if (printIntervalLog)
            {
                sb.Clear().Append(LOG_TAG).Append("OnUpdate() ").Append(m_TrackerId)
                    .Append(", product: ").Append(description.product)
                    .Append(", isTracked: ").Append(state.pose.isTracked)
                    .Append(", trackingState: ").Append(state.pose.trackingState)
                    .Append(", position (").Append(pos.x).Append(", ").Append(pos.y).Append(", ").Append(pos.z).Append(")");
                DEBUG(sb);
            }

            InputSystem.QueueStateEvent(this, state);
        }
    }
}
#endif