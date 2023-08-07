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

        [InputControl(name = "isTracked", layout = "Button", format = "BIT", offset = 0, displayName = "IsTracked")]
        public bool isTracked;

        [InputControl(name = "trackingState", layout = "Integer", format = "INT", offset = 4, displayName = "TrackingState")]
        public int trackingState;

        [InputControl(name = "position", layout = "Vector3", format = "VEC3", offset = 8, displayName = "Position")]
        public Vector3 position;

        [InputControl(name = "rotation", layout = "Quaternion", format = "QUAT", offset = 20, displayName = "Rotation")]
        public Quaternion rotation;

        [InputControl(name = "menuButton",      layout = "Button", format = "BIT", offset = 36, bit = 1, displayName = "MenuButton")]
        [InputControl(name = "primaryButton",   layout = "Button", format = "BIT", offset = 36, bit = 2, displayName = "PrimaryButton")]
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
        public ButtonControl isTracked { get; private set; }
        [InputControl]
        public IntegerControl trackingState { get; private set; }
        [InputControl]
        public Vector3Control position { get; private set; }
        [InputControl]
        public QuaternionControl rotation { get; private set; }
        [InputControl]
        public ButtonControl menuButton { get; private set; }
        [InputControl]
        public ButtonControl primaryButton { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            isTracked = GetChildControl<ButtonControl>("isTracked");
            trackingState = GetChildControl<IntegerControl>("trackingState");
            position = GetChildControl<Vector3Control>("position");
            rotation = GetChildControl<QuaternionControl>("rotation");
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

            state.isTracked = InputDeviceTracker.IsTracked(m_TrackerId);
            if (InputDeviceTracker.GetPosition(m_TrackerId, out Vector3 pos))
            {
                // POSITION_VALID_BIT = 0x0002
                // POSITION_TRACKED_BIT = 0x0008
                state.trackingState |= (0x0002 | 0x0008);
                state.position = pos;
            }
            if (InputDeviceTracker.GetRotation(m_TrackerId, out Quaternion rot))
			{
                // ORIENTATION_VALID_BIT = 0x0001
                // ORIENTATION_TRACKED_BIT = 0x0004
                state.trackingState |= (0x0001 | 0x0004);
                state.rotation = rot;
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
                    .Append(", isTracked: ").Append(state.isTracked)
                    .Append(", trackingState: ").Append(state.trackingState)
                    .Append(", position (").Append(pos.x).Append(", ").Append(pos.y).Append(", ").Append(pos.z).Append(")");
                DEBUG(sb);
            }

            InputSystem.QueueStateEvent(this, state);
        }
    }
}
#endif