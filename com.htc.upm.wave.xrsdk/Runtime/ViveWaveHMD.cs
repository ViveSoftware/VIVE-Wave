// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.Controls;
using System.Text;

namespace Wave.XR
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve, InputControlLayout(displayName = "VIVE HMD (Wave)", canRunInBackground = true)]
    public class ViveWaveHMD : XRHMD
    {
        const string LOG_TAG = "Wave.XR.ViveWaveHMD ";
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

        const string kInterfaceName = XRUtilities.InterfaceMatchAnyVersion;
        const string kManufacturer = "NA";
        const string kProductFocusHeadset = "Vive Focus";
        const string kProductFocusPlus = "Vive Focus Plus";
        const string kProductFocus3 = "WVR HMD";
        const string kProducts = "^(" + kProductFocusHeadset + ")|^(" + kProductFocusPlus + ")|^(" + kProductFocus3 + ")";

        /// <summary>
        /// Registers the <see cref="ViveWaveHMD"/> layout with the Input System.
        /// </summary>
#if UNITY_EDITOR
        static ViveWaveHMD()
        {
            InitializeInPlayer();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer()
        {
            sb.Clear().Append("InitializeInPlayer()"); DEBUG(sb);
            InputSystem.RegisterLayout(
                typeof(ViveWaveHMD),
                matches: new InputDeviceMatcher()
                        .WithInterface(kInterfaceName)
                        .WithManufacturer(kManufacturer)
                        .WithProduct(@kProducts));
        }

		#region Alias
		/**
         * To get these variable's name, check InputDevice.description.capabilities.
         * In capabilities, inputFeatures record each feature's name and commonusages.
         * e.g.
         *		XRDeviceDescriptor descriptor = XRDeviceDescriptor.FromJson(InputDevice.description.capabilities);
         *		descriptor.inputFeatures.name and inputFeatures.inputFeatures.usageHints
         *		Debug.Log(descriptor.inputFeatures.name, inputFeatures.inputFeatures.usageHints)
         *		name => Left Controller Primary2DAxis (Touchpad Axis)
         *		usageHints.content => Primary2DAxis	
         * The alias name is LeftControllerPrimary2DAxisTouchpadAxis and commonusages is Primary2DAxis.
         **/
		const string kDevice = "HMD";
		const string kTrackingState = "TrackingState";
		const string kIsTracked = "IsTracked";
		const string kDevicePosition = "Position";
		const string kDeviceRotation = "Rotation";
		const string kDeviceVelocity = "Velocity";
		const string kDeviceAngularVelocity = "AngularVelocity";
		const string kCenterEyePosition = "CenterEyePosition";
		const string kCenterEyeRotation = "CenterEyeRotation";
		const string kBatteryLevel = "BatteryPercentage";
		const string kUserPresence = "UserPresence";
		#endregion

		[Preserve, InputControl(alias = kDevice + kTrackingState)]
        public new IntegerControl trackingState { get; private set; }

        [Preserve, InputControl(alias = kDevice + kIsTracked)]
        public new ButtonControl isTracked { get; private set; }

        [Preserve, InputControl(alias = kDevice + kDevicePosition)]
        public new Vector3Control devicePosition { get; private set; }

        [Preserve, InputControl(alias = kDevice + kDeviceRotation)]
        public new QuaternionControl deviceRotation { get; private set; }

		[Preserve, InputControl(alias = kDevice + kDeviceVelocity)]
		public Vector3Control deviceVelocity { get; private set; }

		[Preserve, InputControl(alias = kDevice + kDeviceAngularVelocity)]
		public Vector3Control deviceAngularVelocity { get; private set; }

		[Preserve, InputControl(alias = kDevice + kCenterEyePosition)]
        public new Vector3Control centerEyePosition { get; private set; }

        [Preserve, InputControl(alias = kDevice + kCenterEyeRotation)]
        public new QuaternionControl centerEyeRotation { get; private set; }

		[Preserve, InputControl(alias = kDevice + kBatteryLevel)]
		public AxisControl batteryLevel { get; private set; }

		[Preserve, InputControl(alias = kUserPresence)]
		public ButtonControl userPresence { get; private set; }
		/// <summary>
		/// Internal call used to assign controls to the the correct element.
		/// </summary>
		protected override void FinishSetup()
        {
            sb.Clear().Append("FinishSetup() description deviceClass: ").Append(description.deviceClass)
                .Append(", interfaceName: ").Append(description.interfaceName)
                .Append(", manufacturer: ").Append(description.manufacturer)
                .Append(", product: ").Append(description.product)
                .Append(", serial: ").Append(description.serial)
                .Append(", version: ").Append(description.version);
            DEBUG(sb);

            base.FinishSetup();

            trackingState = GetChildControl<IntegerControl>("trackingState");
            isTracked = GetChildControl<ButtonControl>("isTracked");
            devicePosition = GetChildControl<Vector3Control>("devicePosition");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			centerEyePosition = GetChildControl<Vector3Control>("centerEyePosition");
            centerEyeRotation = GetChildControl<QuaternionControl>("centerEyeRotation");
			batteryLevel = GetChildControl<AxisControl>("batteryLevel");
			userPresence = GetChildControl<ButtonControl>("userPresence");
		}
	}
}
#endif