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
	[Preserve, InputControlLayout(displayName = "VIVE Controller (Wave)", commonUsages = new[] { "LeftHand", "RightHand" }, canRunInBackground = true)]
	public class ViveWaveController : XRControllerWithRumble
	{
		const string LOG_TAG = "Wave.XR.ViveWaveController ";
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

		public const string kProductFocus3Left = "WVR_CR_Left_001";
		public const string kProductFocus3Right = "WVR_CR_Right_001";

		/// <summary>
		/// Registers the <see cref="ViveWaveController"/> layout with the Input System.
		/// </summary>
#if UNITY_EDITOR
		static ViveWaveController()
		{
			InitializeInPlayer();
		}
#endif

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeInPlayer()
		{
			sb.Clear().Append("InitializeInPlayer() ").Append(kProductFocus3Left); DEBUG(sb);
			InputSystem.RegisterLayout(
				typeof(ViveWaveController),
				matches: new InputDeviceMatcher()
						.WithInterface(kInterfaceName)
						.WithManufacturer(kManufacturer)
						.WithProduct(kProductFocus3Left));

			sb.Clear().Append("InitializeInPlayer() ").Append(kProductFocus3Right); DEBUG(sb);
			InputSystem.RegisterLayout(
				typeof(ViveWaveController),
				matches: new InputDeviceMatcher()
						.WithInterface(kInterfaceName)
						.WithManufacturer(kManufacturer)
						.WithProduct(kProductFocus3Right));
		}

		#region Aliases
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
		const string kControllerR = "RightController", kControllerL = "LeftController";
		const string kPrimary2DAxis = "Primary2DAxisTouchpadAxis";
		const string kPrimary2DAxisClick = "Primary2DAxisClickTouchpad";
		const string kPrimary2DAxisTouch = "Primary2DAxisTouchTouchpadTouch";
		const string kTriggerButton = "TriggerButton";
		const string kTrigger = "TriggerTriggerAxis";
		const string kGripButton = "GripButton";
		const string kGrip = "Grip";
		const string kMenuButton = "MenuButton";
		const string kPrimaryButton = "PrimaryButton";
		const string kPrimaryTouch = "PrimaryTouch";
		const string kSecondaryButton = "SecondaryButton";
		const string kSecondaryTouch = "SecondaryTouch";
		const string kIsTracked = "IsTracked";
		const string kTrackingState = "TrackingState";
		const string kDevicePosition = "Position";
		const string kDeviceRotation = "Rotation";
		const string kDeviceVelocity = "Velocity";
		const string kDeviceAngularVelocity = "AngularVelocity";
		const string kBatteryLevel = "BatteryPercentage";
		#endregion

		/**
		 * If the public variable's name is NOT a system variable's name, the variable will be added as an XR binding path.
		 * 
		 * BTW, a valid xr binding path is: {xr_input_name}. e.g. trigger axis is {trigger}.
		 * The newly added and invalid binding pathes do NOT include the braces "{" and "}".
		 * 
		 * You can refer to <see href="https://docs.unity3d.com/Manual/xr_input.html">XR Input</see> about the valid XR binding path.
		 * 
		 * Here lists all valid pathes: {primary2DAxis}, {primary2DAxisClick}, {primary2DAxisTouch},
		 * {triggerButton}, {trigger}, {gripButton}, {grip}, {menuButton}, {primaryButton}, {primaryTouch}, {secondaryButton} and {secondaryTouch}.
		 * 
		 * To make the invalid binding path be valid we have to specify CORRECT corresponding aliases.
		 * But we have no idea about the CORRECT aliases of
		 * 
		 * - primary2DAxisClick
		 * - primary2DAxisTouch
		 **/

		/// <summary> Represents the thumbstick axis in Vector2 where x {-1, 1} is positive at right side and y {-1, 1} is positive at top side. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kPrimary2DAxis, kControllerL + kPrimary2DAxis })]
		public Vector2Control thumbstick { get; private set; }
		/// <summary> Represents the Thumbstick key is pressed.</summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kPrimary2DAxisClick, kControllerL + kPrimary2DAxisClick })]
		public ButtonControl thumbstickClicked { get; private set; }
		/// <summary> Represents the Thumbstick key is touched. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kPrimary2DAxisTouch, kControllerL + kPrimary2DAxisTouch })]
		public ButtonControl thumbstickTouched { get; private set; }

		/// <summary> Represents the Trigger key is pressed. Note the "triggerTouched" is NOT supported in <see href="https://hub.vive.com/storage/app/doc/en-us/UnityXR/UnityXRButton.html">Unity XR Input</see>. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kTriggerButton, kControllerL + kTriggerButton })]
		public ButtonControl triggerPressed { get; private set; }
		/// <summary> Represents the trigger axis in float {0, 1} where 1 means the Trigger key is fully pressed. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kTrigger, kControllerL + kTrigger })]
		public AxisControl trigger { get; private set; }

		/// <summary> Represents the Grip key is pressed. Note the "gripTouched" is NOT supported in <see href="https://hub.vive.com/storage/app/doc/en-us/UnityXR/UnityXRButton.html">Unity XR Input</see>. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kGripButton, kControllerL + kGripButton })]
		public ButtonControl gripPressed { get; private set; }
		/// <summary> Represents the grip axis in float {0, 1} where 1 means the Grip key is fully pressed. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kGrip, kControllerL + kGrip })]
		public AxisControl grip { get; private set; }

		/// <summary> Represents the Left Menu key is pressed. Note only left controller has a menu key. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerL + kMenuButton })]
		public ButtonControl menu { get; private set; }

		/// <summary> Represents the A key in right controller or X key in left controller is pressed. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kPrimaryButton, kControllerL + kPrimaryButton })]
		public ButtonControl primaryButton { get; private set; }
		/// <summary> Represents the A key in right controller or X key in left controller is touched. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kPrimaryTouch, kControllerL + kPrimaryTouch })]
		public ButtonControl primaryTouched { get; private set; }

		/// <summary> Represents the B key in right controller or Y key in left controller is pressed. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kSecondaryButton, kControllerL + kSecondaryButton })]
		public ButtonControl secondaryButton { get; private set; }
		/// <summary> Represents the B key in right controller or Y key in left controller is touched. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kSecondaryTouch, kControllerL + kSecondaryTouch })]
		public ButtonControl secondaryTouched { get; private set; }

		/// <summary> Checks if the left/right controller is tracked. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kIsTracked, kControllerL + kIsTracked })]
		new public ButtonControl isTracked { get; private set; }
		/// <summary> Represents the tracking state in <see href="https://docs.unity3d.com/ScriptReference/XR.InputTrackingState.html">InputTrackingState</see> of left/right controller. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kTrackingState, kControllerL + kTrackingState })]
		new public IntegerControl trackingState { get; private set; }
		/// <summary> Represents the position in Vector3 of left/right controller. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kDevicePosition, kControllerL + kDevicePosition })]
		new public Vector3Control devicePosition { get; private set; }
		/// <summary> Represents the rotation in Quaternion of left/right controller. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kDeviceRotation, kControllerL + kDeviceRotation })]
		new public QuaternionControl deviceRotation { get; private set; }
		/// <summary> Represents the velocity in Vector3 of left/right controller. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kDeviceVelocity, kControllerL + kDeviceVelocity })]
		public Vector3Control deviceVelocity { get; private set; }
		/// <summary> Represents the angular velocity in Vector3 of left/right controller. </summary>
		[Preserve, InputControl(aliases = new[] { kControllerR + kDeviceAngularVelocity, kControllerL + kDeviceAngularVelocity })]
		public Vector3Control deviceAngularVelocity { get; private set; }

		/// <summary> Represents the batter percentage in float {0, 1} where 1 means fully charged. </summary>
		[Preserve, InputControl(aliases = new[] { "Right" + kBatteryLevel, "Left" + kBatteryLevel })]
		public AxisControl batteryLevel { get; private set; }

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

			thumbstick = GetChildControl<Vector2Control>("thumbstick");
			trigger = GetChildControl<AxisControl>("trigger");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			grip = GetChildControl<AxisControl>("grip");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			menu = GetChildControl<ButtonControl>("menu");
			primaryButton = GetChildControl<ButtonControl>("primaryButton");
			secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
			thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
			thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");

			isTracked = GetChildControl<ButtonControl>("isTracked");
			trackingState = GetChildControl<IntegerControl>("trackingState");
			devicePosition = GetChildControl<Vector3Control>("devicePosition");
			deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			batteryLevel = GetChildControl<AxisControl>("batteryLevel");
		}
	}
}
#endif