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

namespace Wave.XR
{
    public struct ViveWaveWristTrackerState : IInputStateTypeInfo
	{
        // You must tag every state with a FourCC code for type
        // checking. The characters can be anything. Choose something that allows
        // you to easily recognize memory that belongs to your own Device.
        public FourCC format => new FourCC('W', 'A', 'V', 'E');

        [InputControl(name = "pose", layout = "Pose", format = "Pose", offset = 0, displayName = "Pose")]
        public PoseState pose;

        [InputControl(name = "menuButton",      layout = "Button", format = "BIT", offset = 60, bit = 1, displayName = "MenuButton")]
        [InputControl(name = "primaryButton",   layout = "Button", format = "BIT", offset = 60, bit = 2, displayName = "PrimaryButton")]
        public uint pressed;
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Vive Wrist Tracker (Wave)", commonUsages = new[] { "LeftHand", "RightHand" }, stateType = typeof(ViveWaveWristTrackerState), isGenericTypeOfDevice = true)]
    public class ViveWaveWristTracker : InputDevice, IInputUpdateCallbackReceiver
    {
        const string LOG_TAG = "Wave.XR.ViveWaveWristTracker ";
        private static StringBuilder m_sb = null;
        private static StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        static void DEBUG(StringBuilder msg)
        {
            msg.Insert(0, LOG_TAG);
            Debug.Log(msg);
        }
        uint logFrame = 0;
        static bool printIntervalLog = false;

        const string kInterfaceName = "Wrist Tracker";
        const string kProductNameRight = "Wave Tracker0";
        const string kSerialNumberRight = "HTC-211012-Tracker0";
        const string kProductNameLeft = "Wave Tracker1";
        const string kSerialNumberLeft = "HTC-211012-Tracker1";

        private bool IsLeft { get => description.product.Equals(kProductNameLeft); }
        private InputDeviceTracker.TrackerId m_TrackerId { get => IsLeft ? InputDeviceTracker.TrackerId.Tracker1 : InputDeviceTracker.TrackerId.Tracker0; }

#if UNITY_EDITOR
        static ViveWaveWristTracker()
		{
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
		{
            sb.Clear(); sb.Append("Initialize() RegisterLayout with interface ").Append(kInterfaceName);
            DEBUG(sb);
            InputSystem.RegisterLayout(
                typeof(ViveWaveWristTracker),
                matches: new InputDeviceMatcher()
                        .WithInterface(kInterfaceName)
            );

            var device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveWristTracker);
            while (device != null)
            {
                sb.Clear().Append("Initialize() Removes a ViveWaveWristTracker device."); DEBUG(sb);
                InputSystem.RemoveDevice(device);
                device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveWristTracker);
            }

            sb.Clear(); sb.Append("Initialize() Adds right ViveWaveWristTracker device ").Append(kProductNameRight);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductNameRight, serial = kSerialNumberRight }
            );
            sb.Clear(); sb.Append("Initialize() Adds left ViveWaveWristTracker device ").Append(kProductNameLeft);
            DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductNameLeft, serial = kSerialNumberLeft }
            );
        }

        [InputControl]
        public PoseControl pose { get; private set; }
        [InputControl]
        public ButtonControl menuButton { get; private set; }
        [InputControl]
        public ButtonControl primaryButton { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            pose = GetChildControl<PoseControl>("pose");
            menuButton = GetChildControl<ButtonControl>("menuButton");
            primaryButton = GetChildControl<ButtonControl>("primaryButton");

            InputSystem.SetDeviceUsage(this, (IsLeft ? CommonUsages.LeftHand : CommonUsages.RightHand));

            sb.Clear();
            sb.Append("FinishSetup() deviceId: ").Append(deviceId)
                .Append(", IsLeft: ").Append(IsLeft)
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
                //if (printIntervalLog) { sb.Clear(); sb.Append("OnUpdate() No tracker connected."); DEBUG(sb); }
                return;
            }

            ViveWaveWristTrackerState state = new ViveWaveWristTrackerState();

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

            if (InputDeviceTracker.ButtonDown(m_TrackerId, UnityEngine.XR.CommonUsages.menuButton, out bool menuButtonDown))
            {
                if (menuButtonDown)
                    state.pressed |= 1 << 1;
            }
            if (InputDeviceTracker.ButtonDown(m_TrackerId, UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButtonDown))
            {
                if (primaryButtonDown)
                    state.pressed |= 1 << 2;
            }

            if (printIntervalLog)
            {
                sb.Clear(); sb.Append("OnUpdate() ").Append(m_TrackerId)
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