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
        const string LOG_TAG = "Wave.XR.ViveWaveWristTracker";
        static void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
        static bool printIntervalLog = false;
        static void INTERVAL(string msg) { if (printIntervalLog) DEBUG(msg); }

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
            DEBUG("Initialize() RegisterLayout with interface " + kInterfaceName);
            InputSystem.RegisterLayout(
                typeof(ViveWaveWristTracker),
                matches: new InputDeviceMatcher()
                        .WithInterface(kInterfaceName)
            );

            var device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveWristTracker);
            while (device != null)
            {
                DEBUG("Initialize() Removes a ViveWaveWristTracker device.");
                InputSystem.RemoveDevice(device);
                device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveWristTracker);
            }

            DEBUG("Initialize() Adds right ViveWaveWristTracker device " + kProductNameRight);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductNameRight, serial = kSerialNumberRight }
            );
            DEBUG("Initialize() Adds left ViveWaveWristTracker device " + kProductNameLeft);
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

            DEBUG("FinishSetup() deviceId: " + deviceId
                + ", IsLeft: " + IsLeft
                + ", interfaceName: " + description.interfaceName
                + ", product: " + description.product
                + ", serial: " + description.serial);
        }

        uint logFrame = 0;
        public void OnUpdate()
		{
            if (Application.isEditor || !Application.isPlaying) { return; }

            logFrame++;
            logFrame %= 300;
            printIntervalLog = (logFrame == 0);

            if (!InputDeviceTracker.IsAvailable())
            {
                INTERVAL("OnUpdate() No tracker connected.");
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

            INTERVAL("OnUpdate() " + m_TrackerId
                + ", product: " + description.product
                + ", isTracked: " + state.isTracked
                + ", trackingState: " + state.trackingState
                + ", position (" + pos.x.ToString() + ", " + pos.y.ToString() + ", " + pos.z.ToString() + ")");

            InputSystem.QueueStateEvent(this, state);
        }
    }
}
#endif