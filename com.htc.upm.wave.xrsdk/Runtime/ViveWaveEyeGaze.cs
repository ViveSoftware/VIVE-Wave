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
    public struct ViveWaveEyeGazeState : IInputStateTypeInfo
    {
        // You must tag every state with a FourCC code for type
        // checking. The characters can be anything. Choose something that allows
        // you to easily recognize memory that belongs to your own Device.
        public FourCC format => new FourCC('W', 'A', 'V', 'E');

        [InputControl(name = "isTracked", layout = "Button", format = "BIT", displayName = "IsTracked")]
        public bool isTracked;

        [InputControl(name = "trackingState", layout = "Integer", format = "INT", displayName = "TrackingState")]
        public int trackingState;

        [InputControl(name = "position", layout = "Vector3", format = "VEC3", displayName = "Position")]
        public Vector3 position;

        [InputControl(name = "rotation", layout = "Quaternion", format = "QUAT", displayName = "Rotation")]
        public Quaternion rotation;
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Vive Eye Gaze (Wave)", stateType = typeof(ViveWaveEyeGazeState), isGenericTypeOfDevice = true)]
    public class ViveWaveEyeGaze : InputDevice, IInputUpdateCallbackReceiver
    {
        const string LOG_TAG = "Wave.XR.ViveWaveEyeGaze ";
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
        static bool printIntervalLog = false;

        const string kInterfaceName = "Eye Gaze";
        const string kProductName = "WVR_EYE_GAZE";

#if UNITY_EDITOR
        static ViveWaveEyeGaze()
        {
            sb.Clear().Append("ctor()"); DEBUG(sb);
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize( )
		{
            sb.Clear().Append("Initialize() RegisterLayout with interface ").Append(kInterfaceName); DEBUG(sb);
            InputSystem.RegisterLayout(
                typeof(ViveWaveEyeGaze),
                matches: new InputDeviceMatcher()
                        .WithInterface(kInterfaceName)
            );

            var device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveEyeGaze);
            while (device != null)
            {
                sb.Clear().Append("Initialize() Removes a ViveWaveEyeGaze device."); DEBUG(sb);
                InputSystem.RemoveDevice(device);
                device = InputSystem.devices.FirstOrDefault(x => x is ViveWaveEyeGaze);
            }

            sb.Clear().Append("Initialize() Adds a ViveWaveEyeGaze device."); DEBUG(sb);
            InputSystem.AddDevice(
                new InputDeviceDescription { interfaceName = kInterfaceName, product = kProductName }
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

        protected override void FinishSetup()
        {
            base.FinishSetup();

            isTracked = GetChildControl<ButtonControl>("isTracked");
            trackingState = GetChildControl<IntegerControl>("trackingState");
            position = GetChildControl<Vector3Control>("position");
            rotation = GetChildControl<QuaternionControl>("rotation");
        }

        uint logFrame = 0;
		public void OnUpdate()
		{
            if (Application.isEditor || !Application.isPlaying) { return; }

            logFrame++;
            logFrame %= 300;
            printIntervalLog = (logFrame == 0);

            var status = InputDeviceEye.GetEyeTrackingStatus();
            if (status != InputDeviceEye.TrackingStatus.AVAILABLE)
			{
                //if (printIntervalLog) { sb.Clear().Append("OnUpdate() Eye Tracking status is ").Append(status.ToString()); DEBUG(sb); }
                return;
			}

            //InputDeviceEye.SetEyeTrackingSpace(InputDeviceEye.TrackingSpace.World); // World space eye tracking.

            ViveWaveEyeGazeState state = new ViveWaveEyeGazeState();
            state.isTracked = InputDeviceEye.IsEyeTrackingTracked();
            if (InputDeviceEye.GetCombinedEyeOrigin(out Vector3 origin))
            {
                // POSITION_VALID_BIT = 0x0002
                // POSITION_TRACKED_BIT = 0x0008
                state.trackingState |= (0x0002 | 0x0008);
                state.position = origin;
            }
            if (InputDeviceEye.GetCombinedEyeDirection(out Vector3 direction))
            {
                // ORIENTATION_VALID_BIT = 0x0001
                // ORIENTATION_TRACKED_BIT = 0x0004
                state.trackingState |= (0x0001 | 0x0004);
                state.rotation = Quaternion.LookRotation(direction);
            }
            if (printIntervalLog)
            {
                sb.Clear().Append("OnUpdate() isTracked: ").Append(state.isTracked)
                    .Append(", trackingState: ").Append(state.trackingState)
                    .Append(", origin (").Append(origin.x).Append(", ").Append(origin.y).Append(", ").Append(origin.z).Append(")")
                    .Append(", direction (").Append(direction.x).Append(", ").Append(direction.y).Append(", ").Append(direction.z).Append(")");
                DEBUG(sb);
            }
            InputSystem.QueueStateEvent(this, state);
        }
	}
}
#endif
