// "WaveVR SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wave.Native
{
	public enum WVR_AppType
	{
		WVR_AppType_VRContent = 1,
		WVR_AppType_NonVRContent = 2,
	}

	public enum WVR_InitError
	{
		WVR_InitError_None = 0,
		WVR_InitError_Unknown = 1,
		WVR_InitError_NotInitialized = 2,
	}

	public enum WVR_EventType
	{
		/** common event region */
		WVR_EventType_Quit                               = 1000,    /**< Application Quit. */
		WVR_EventType_SystemInteractionModeChanged       = 1001,    /**< @ref WVR_InteractionMode changed; using @ref WVR_GetInteractionMode to get interaction mode. */
		WVR_EventType_SystemGazeTriggerTypeChanged       = 1002,    /**< @ref WVR_GazeTriggerType changed; using @ref WVR_GetGazeTriggerType to get gaze trigger type. */
		WVR_EventType_TrackingModeChanged                = 1003,    /**< Notification of changing tracking mode (3 Dof/6 Dof); using @ref WVR_GetDegreeOfFreedom can get current tracking mode.*/
		WVR_EventType_RecommendedQuality_Lower           = 1004,    /**< Notification recommended quality to Lower from runtime. */
		WVR_EventType_RecommendedQuality_Higher          = 1005,    /**< Notification recommended quality to Higher from runtime. */
		WVR_EventType_HandGesture_Changed                = 1006,    /**< Notification gesture changed. */
		WVR_EventType_HandGesture_Abnormal               = 1007,    /**< Notification gesture abnormal. */
		WVR_EventType_HandTracking_Abnormal              = 1008,    /**< Notification hand tracking abnormal. */
		WVR_EventType_ArenaChanged                       = 1009,    /**< Notification arena changed. */
		WVR_EventType_RenderingToBePaused                = 1010,
		WVR_EventType_RenderingToBeResumed               = 1011,
		WVR_EventType_SpectatingStarted                  = 1012,
		WVR_EventType_SpectatingStopped                  = 1013,
		WVR_EventType_SpatialAnchor_Changed              = 1101,
		WVR_EventType_CachedSpatialAnchor_Changed        = 1102,
		WVR_EventType_PersistedSpatialAnchor_Changed     = 1103,

        WVR_EventType_DeviceConnected = 2000,    /**< @ref WVR_DeviceType connected. */
		WVR_EventType_DeviceDisconnected                 = 2001,    /**< @ref WVR_DeviceType disconnected. */
		WVR_EventType_DeviceStatusUpdate                 = 2002,    /**< @ref WVR_DeviceType configure changed. */
		WVR_EventType_DeviceSuspend                      = 2003,    /**< When user takes off HMD*/
		WVR_EventType_DeviceResume                       = 2004,    /**< When user puts on HMD*/
		WVR_EventType_IpdChanged                         = 2005,    /**< The interpupillary distance has been changed; using @ref WVR_GetRenderProps can get current ipd. */
		WVR_EventType_DeviceRoleChanged                  = 2006,    /**< @ref WVR_DeviceType controller roles are switched. */
		WVR_EventType_BatteryStatusUpdate                = 2007,    /**< @ref WVR_DeviceType the battery status of device has changed; using @ref WVR_GetBatteryStatus to check the current status of the battery. */
		WVR_EventType_ChargeStatusUpdate                 = 2008,    /**< @ref WVR_DeviceType the charged status of device has changed; using @ref WVR_GetChargeStatus to check the current status of the battery in use. */
		WVR_EventType_DeviceErrorStatusUpdate            = 2009,    /**< @ref WVR_DeviceType device occurs some warning; using @ref WVR_GetDeviceErrorState to get the current error status from device service. */
		WVR_EventType_BatteryTemperatureStatusUpdate     = 2010,    /**< @ref WVR_DevcieType battery temperature of device has changed; using @ref WVR_GetBatteryTemperatureStatus to get the current battery temperature. */
		WVR_EventType_RecenterSuccess                    = 2011,    /**< Notification of recenter success for 6 DoF device*/
		WVR_EventType_RecenterFail                       = 2012,    /**< Notification of recenter fail for 6 DoF device*/
		WVR_EventType_RecenterSuccess3DoF                = 2013,    /**< Notification of recenter success for 3 DoF device*/
		WVR_EventType_RecenterFail3DoF                   = 2014,    /**< Notification of recenter fail for 3 DoF device*/
		WVR_EventType_ClearHmdTrackingMapDone            = 2015,    /**< Notification of the finish of clearing operation of HMD tracking map from device service*/
		WVR_EventType_InputDevMappingChanged			 = 2016,    /**< Notification for input device mapping table changed.*/
		WVR_EventType_BatteryPercentageUpdate			 = 2017,    /**< Notification for battery percentage update.*/
		WVR_EventType_DeviceInfoUpdate                   = 2018,

		WVR_EventType_PassThroughOverlayShownBySystem	 = 2100,    /**< Notification for passthrough overlay is shown by the system.*/
		WVR_EventType_PassThroughOverlayHiddenBySystem   = 2101,    /**< Notification for passthrough overlay is hidden by the system. */
		WVR_EventType_ControllerPoseModeChanged	         = 2102,
		WVR_EventType_ControllerPoseModeOffsetReady      = 2103,
		WVR_EventType_DeviceTableStaticLocked            = 2104,    /**< @ref WVR_DeviceType is on table static state. */
		WVR_EventType_DeviceTableStaticUnlocked          = 2105,    /**< @ref WVR_DeviceType is not on table static state. */

		WVR_EventType_Hand_EnhanceStable                 = 2900,    /**< Notification for Enhanced Hand Stability ON or OFF*/

		/** Input Event region */
		WVR_EventType_ButtonPressed                      = 3000,    /**< @ref WVR_InputId status change to pressed. */
		WVR_EventType_ButtonUnpressed                    = 3001,    /**< @ref WVR_InputId status change to unpressed */
		WVR_EventType_TouchTapped                        = 3002,    /**< @ref WVR_InputId status change to touched. */
		WVR_EventType_TouchUntapped                      = 3003,    /**< @ref WVR_InputId status change to untouched. */
		WVR_EventType_LeftToRightSwipe                   = 3004,    /**< Notification of swipe motion (move Left to Right) on touchpad */
		WVR_EventType_RightToLeftSwipe                   = 3005,    /**< Notification of swipe motion (move Right to Left) on touchpad */
		WVR_EventType_DownToUpSwipe                      = 3006,    /**< Notification of swipe motion (move Down to Up) on touchpad */
		WVR_EventType_UpToDownSwipe                      = 3007,    /**< Notification of swipe motion (move Up to Down) on touchpad */

		//WVR_EventType_GestureTrigger = 3010,
		//WVR_EventType_GestureRelease = 3011,

		/** Accessory events region */
		WVR_EventType_TrackerConnected = 4000,    /**< @ref WVR_TrackerId is connected. */
		WVR_EventType_TrackerDisconnected = 4001,    /**< @ref WVR_TrackerId is disconnected. */
		WVR_EventType_TrackerBatteryLevelUpdate = 4002,    /**< The battery level of @ref WVR_TrackerId has changed. Use @ref WVR_GetAcceBatteryLevel to check the current battery level. */
		WVR_EventType_TrackerRoleChanged = 4003,    /**< @ref WVR_TrackerId is role changed. */

		/** Input Event of the accessory region */
		WVR_EventType_TrackerButtonPressed = 5000,     /**< @ref WVR_InputId status of @ref WVR_TrackerId changed to pressed. */
		WVR_EventType_TrackerButtonUnpressed = 5001,     /**< @ref WVR_InputId status of @ref WVR_TrackerId changed to not pressed */
		WVR_EventType_TrackerTouchTapped = 5002,     /**< @ref WVR_InputId status of @ref WVR_TrackerId changed to touched. */
		WVR_EventType_TrackerTouchUntapped = 5003,     /**< @ref WVR_InputId status of @ref WVR_TrackerId changed to untouched. */
	}

	public enum WVR_PeripheralQuality
	{
		Low,
		Middle,
		High,
	}

	public enum WVR_FoveationMode
	{
		Disable,
		Enable,
		Default,
        Dynamic,
	}

    public enum WVR_DeviceType
	{
		WVR_DeviceType_Invalid				= 0,    /**< The device is unknown or not existing. */
		WVR_DeviceType_HMD					= 1,    /**< Head-Mounted Display. */
		WVR_DeviceType_Controller_Right		= 2,    /**< Right hand tracked controller. */
		WVR_DeviceType_Controller_Left		= 3,    /**< Left hand tracked controller. */
		WVR_DeviceType_Camera				= 4,    /**< Camera device */
		WVR_DeviceType_EyeTracking			= 5,    /**< Eye tracking. */
		WVR_DeviceType_HandGesture_Right	= 6,    /**< Right hand of gesture. */
		WVR_DeviceType_HandGesture_Left		= 7,    /**< Left hand gesture. */
		WVR_DeviceType_NaturalHand_Right	= 8,    /**< Right natural hand. */
		WVR_DeviceType_NaturalHand_Left		= 9,    /**< Left natural hand. */
		WVR_DeviceType_ElectronicHand_Right = 10,   /**< Right electronic hand. */
		WVR_DeviceType_ElectronicHand_Left	= 11,   /**< Left electronic hand. */
		WVR_DeviceType_Tracker				= 12,   /**< Tracker. */
		WVR_DeviceType_Lip					= 13,   /**< Lip and lower face detection device */
		WVR_DeviceType_EyeExpression		= 14,   /**< Eye expression */
	};

	public enum WVR_RecenterType
	{
		WVR_RecenterType_Disabled = 0,
		WVR_RecenterType_YawOnly = 1,
		WVR_RecenterType_YawAndPosition = 2,
		WVR_RecenterType_RotationAndPosition = 3,
		WVR_RecenterType_Position = 4,
	};

	public enum WVR_InputType
	{
		WVR_InputType_Button = 1 << 0,
		WVR_InputType_Touch = 1 << 1,
		WVR_InputType_Analog = 1 << 2,
	};

	public enum WVR_BatteryStatus
	{
		WVR_BatteryStatus_Unknown = 0,
		WVR_BatteryStatus_Normal = 1,
		WVR_BatteryStatus_Low = 2, //  5% <= Battery  < 15%
		WVR_BatteryStatus_UltraLow = 3, //  Battery < 5%
	}

	public enum WVR_ChargeStatus
	{
		WVR_ChargeStatus_Unknown = 0,
		WVR_ChargeStatus_Discharging = 1,
		WVR_ChargeStatus_Charging = 2,
		WVR_ChargeStatus_Full = 3,
	}

	public enum WVR_BatteryTemperatureStatus
	{
		WVR_BatteryTemperature_Unknown = 0,
		WVR_BatteryTemperature_Normal = 1,
		WVR_BatteryTemperature_Overheat = 2,
		WVR_BatteryTemperature_UltraOverheat = 3,
	}

	public enum WVR_DeviceErrorStatus : UInt64
	{
		WVR_DeviceErrorStatus_None						= 0,
		WVR_DeviceErrorStatus_BatteryOverheat			= 1,
		WVR_DeviceErrorStatus_BatteryOverheatRestore	= 1 << 1,
		WVR_DeviceErrorStatus_BatteryOvervoltage		= 1 << 2,
		WVR_DeviceErrorStatus_BatteryOvervoltageRestore = 1 << 3,
		WVR_DeviceErrorStatus_DeviceConnectFail			= 1 << 4,
		WVR_DeviceErrorStatus_DeviceConnectRestore		= 1 << 5,
		WVR_DeviceErrorStatus_DeviceLostTracking		= 1 << 6,
		WVR_DeviceErrorStatus_DeviceLostTrackingRestore = 1 << 7,
		WVR_DeviceErrorStatus_ChargeFail				= 1 << 8,
		WVR_DeviceErrorStatus_ChargeRestore				= 1 << 9,
	}

	public enum WVR_InputId
	{
		WVR_InputId_0     = 0,
		WVR_InputId_1     = 1,
		WVR_InputId_2     = 2,
		WVR_InputId_3     = 3,
		WVR_InputId_4     = 4,
		WVR_InputId_5     = 5,
		WVR_InputId_6     = 6,
		WVR_InputId_7     = 7,
		WVR_InputId_8     = 8,
		WVR_InputId_9     = 9,
		WVR_InputId_10    = 10,
		WVR_InputId_11    = 11,
		WVR_InputId_12    = 12,
		WVR_InputId_13    = 13,
		WVR_InputId_14    = 14,
		WVR_InputId_15    = 15,
		WVR_InputId_16    = 16,
		WVR_InputId_17    = 17,
		WVR_InputId_18    = 18,
		WVR_InputId_19    = 19,

		//alias group mapping
		WVR_InputId_Alias1_System          = WVR_InputId_0,  /**< System Button. */
		WVR_InputId_Alias1_Menu            = WVR_InputId_1,  /**< Menu Button. */
		WVR_InputId_Alias1_Grip            = WVR_InputId_2,  /**< Grip Button. */
		WVR_InputId_Alias1_DPad_Left       = WVR_InputId_3,  /**< DPad_Left Button in physical, or simulated by Touchpad Left pressed event. */
		WVR_InputId_Alias1_DPad_Up         = WVR_InputId_4,  /**< DPad_Up Button in physical, or  simulated by Touchpad Up pressed event. */
		WVR_InputId_Alias1_DPad_Right      = WVR_InputId_5,  /**< DPad_Right Button in physical, or simulated by Touchpad Right pressed event. */
		WVR_InputId_Alias1_DPad_Down       = WVR_InputId_6,  /**< DPad_Down Button in physical, or simulated by Touchpad Down pressed event. */
		WVR_InputId_Alias1_Volume_Up       = WVR_InputId_7,  /**< Volume_Up Button. */
		WVR_InputId_Alias1_Volume_Down     = WVR_InputId_8,  /**< Volume_Down Button. */
		WVR_InputId_Alias1_Bumper          = WVR_InputId_9,  /**< Bumper Button. */
		WVR_InputId_Alias1_A               = WVR_InputId_10, /**< Button A. */
		WVR_InputId_Alias1_B               = WVR_InputId_11, /**< Button B. */
		WVR_InputId_Alias1_X               = WVR_InputId_12, /**< X button. */
		WVR_InputId_Alias1_Y               = WVR_InputId_13, /**< Y button. */
		WVR_InputId_Alias1_Back            = WVR_InputId_14, /**< Hmd Back Button */
		WVR_InputId_Alias1_Enter           = WVR_InputId_15, /**< Hmd Enter Button */
		WVR_InputId_Alias1_Touchpad        = WVR_InputId_16, /**< Touchpad input device. */
		WVR_InputId_Alias1_Trigger         = WVR_InputId_17, /**< Trigger input device. */
		WVR_InputId_Alias1_Thumbstick      = WVR_InputId_18, /**< Thumbstick input device. */
		WVR_InputId_Alias1_Parking         = WVR_InputId_19, /**< Parking input device. */

		WVR_InputId_Max = 32,
	}

	public enum WVR_AnalogType
	{
		WVR_AnalogType_None = 0,
		WVR_AnalogType_2D = 1,
		WVR_AnalogType_1D = 2,
	}

	public enum WVR_Intensity
	{
		WVR_Intensity_Weak = 1,   /**< The Intensity of vibrate is Weak. */
		WVR_Intensity_Light = 2,   /**< The Intensity of vibrate is Light. */
		WVR_Intensity_Normal = 3,   /**< The Intensity of vibrate is Normal. */
		WVR_Intensity_Strong = 4,   /**< The Intensity of vibrate is Strong. */
		WVR_Intensity_Severe = 5,   /**< The Intensity of vibrate is Severe. */
	}

	public enum WVR_PoseOriginModel
	{
		WVR_PoseOriginModel_OriginOnHead = 0,
		WVR_PoseOriginModel_OriginOnGround = 1,
		WVR_PoseOriginModel_OriginOnTrackingObserver = 2,
		WVR_PoseOriginModel_OriginOnHead_3DoF = 3,
	}

	public enum WVR_CoordinateSystem {
		WVR_CoordinateSystem_Local               = 0, /**< The tracking data is based on local coordinate system. */
		WVR_CoordinateSystem_Global              = 1, /**< The tracking data is based on global coordinate system. */
	}

	public enum WVR_ArenaVisible
	{
		WVR_ArenaVisible_Auto = 0,  // show Arena while HMD out off bounds
		WVR_ArenaVisible_ForceOn = 1,  // always show Arena
		WVR_ArenaVisible_ForceOff = 2,  // never show Arena
	}

	public enum WVR_GraphicsApiType
	{
		WVR_GraphicsApiType_OpenGL = 1,
	}

	public enum WVR_ScreenshotMode
	{
		WVR_ScreenshotMode_Default,	  /**< Screenshot image is stereo. Just as show on screen*/
		WVR_ScreenshotMode_Raw,		  /**< Screenshot image has only single eye, and without distortion correction*/
		WVR_ScreenshotMode_Distorted
	}

	public enum WVR_SubmitError
	{
		WVR_SubmitError_None = 0,
		WVR_SubmitError_InvalidTexture = 400,
		WVR_SubmitError_ThreadStop = 401,
		WVR_SubmitError_BufferSubmitFailed = 402,
		WVR_SubmitError_Max = 65535
	}

	public enum WVR_SubmitExtend
	{
		WVR_SubmitExtend_Default = 0x0000,
		WVR_SubmitExtend_DisableDistortion = 0x0001,
		WVR_SubmitExtend_PartialTexture = 0x0010,
	}

	public enum WVR_TextureOptions
	{
		WVR_TextureOption_None = 0,
		WVR_TextureOption_HeadLocked = 1 << 0,
		WVR_TextureOption_Opaque = 1 << 1,
	}

	public enum WVR_TextureShape
	{
		WVR_TextureShape_Quad = 0,
		WVR_TextureShape_Cylinder = 1,
	}

	public enum WVR_TextureLayerType
	{
		WVR_TextureLayerType_Content = 0,
		WVR_TextureLayerType_Overlay = 1,
		WVR_TextureLayerType_Underlay = 2,
	}

	public enum WVR_Eye
	{
		WVR_Eye_Left = 0,
		WVR_Eye_Right = 1,
		WVR_Eye_Both = 2,
		WVR_Eye_None,
	}

	public enum WVR_TextureTarget
	{
		WVR_TextureTarget_2D = 0,
		WVR_TextureTarget_2D_ARRAY = 1,
		WVR_TextureTarget_2D_EXTERNAL = 2,
	}

	public enum WVR_TextureFormat
	{
		WVR_TextureFormat_RGBA
	}

	public enum WVR_TextureType
	{
		WVR_TextureType_UnsignedByte
	}

	public enum WVR_RenderError
	{
		WVR_RenderError_None = 0,
		WVR_RenderError_RuntimeInitFailed = 410,
		WVR_RenderError_ContextSetupFailed = 411,
		WVR_RenderError_DisplaySetupFailed = 412,
		WVR_RenderError_LibNotSupported = 413,
		WVR_RenderError_NullPtr = 414,
		WVR_RenderError_Max = 65535
	}

	public enum WVR_RenderConfig
	{
		WVR_RenderConfig_Default                    = 0,             /**< **WVR_RenderConfig_Default**: Runtime initialization reflects certain properties in device service. Such as single buffer mode and reprojection mechanism, the default settings are determined by device service or runtime config file on specific platform. The default color space is set as linear domain. */
		WVR_RenderConfig_Disable_SingleBuffer       = ( 1 << 0 ),    /**< **WVR_RenderConfig_Disable_SingleBuffer**: Disable single buffer mode in runtime. */
		WVR_RenderConfig_Disable_Reprojection       = ( 1 << 1 ),    /**< **WVR_RenderConfig_Disable_Reprojection**: Disable reprojection mechanism in runtime. */
		WVR_RenderConfig_sRGB                       = ( 1 << 2 ),    /**< **WVR_RenderConfig_sRGB**: Determine whether the color space is set as sRGB domain. */
	}

	public enum WVR_CameraImageType
	{
		WVR_CameraImageType_Invalid = 0,
		WVR_CameraImageType_SingleEye = 1,	 // the image is comprised of one camera
		WVR_CameraImageType_DualEye = 2,	 // the image is comprised of dual cameras
	}

	public enum WVR_CameraImageFormat
	{
		WVR_CameraImageFormat_Invalid = 0,
		WVR_CameraImageFormat_YUV_420 = 1, // the image format is YUV420
		WVR_CameraImageFormat_Grayscale = 2, // the image format is 8-bit gray-scale
	}

	public enum WVR_CameraPosition
	{
		WVR_CameraPosition_Invalid = 0,
		WVR_CameraPosition_left = 1,
		WVR_CameraPosition_Right = 2,
	}

	public enum WVR_OverlayError
	{
		WVR_OverlayError_None = 0,
		WVR_OverlayError_UnknownOverlay = 10,
		WVR_OverlayError_OverlayUnavailable = 11,
		WVR_OverlayError_InvalidParameter = 20,
	}

	public enum WVR_OverlayTransformType
	{
		WVR_OverlayTransformType_None,
		WVR_OverlayTransformType_Absolute,
		WVR_OverlayTransformType_Fixed,
	}

	public enum WVR_NumDoF
	{
		WVR_NumDoF_3DoF = 0,
		WVR_NumDoF_6DoF = 1,
	}

	public enum WVR_ArenaShape
	{
		WVR_ArenaShape_None = 0,
		WVR_ArenaShape_Rectangle = 1,
		WVR_ArenaShape_Round = 2,
	}

	public enum WVR_InteractionMode
	{
		WVR_InteractionMode_SystemDefault = 1,
		WVR_InteractionMode_Gaze = 2,
		WVR_InteractionMode_Controller = 3,
		WVR_InteractionMode_Hand = 4,
	}

	public enum WVR_GazeTriggerType
	{
		WVR_GazeTriggerType_Timeout = 1,
		WVR_GazeTriggerType_Button = 2,
		WVR_GazeTriggerType_TimeoutButton = 3,
	}

	public enum WVR_PerfLevel
	{
		WVR_PerfLevel_System = 0,			//!< System defined performance level (default)
		WVR_PerfLevel_Minimum = 1,			//!< Minimum performance level
		WVR_PerfLevel_Medium = 2,			//!< Medium performance level
		WVR_PerfLevel_Maximum = 3,			//!< Maximum performance level
		WVR_PerfLevel_NumPerfLevels
	}

	public enum WVR_RenderQuality
	{
		WVR_RenderQuality_Low = 1,		   /**< Low recommended render quality */
		WVR_RenderQuality_Medium = 2,		   /**< Medium recommended render quality */
		WVR_RenderQuality_High = 3,		   /**< High recommended render quality */
		WVR_RenderQuality_NumRenderQuality
	}

	public enum WVR_SimulationType
	{
		WVR_SimulationType_Auto = 0,
		WVR_SimulationType_ForceOn = 1,
		WVR_SimulationType_ForceOff = 2,
	}

	/**
	 * Enum containing flags indicating data valididty of an eye pose
	 */
	public enum WVR_EyePoseStatus
	{
		WVR_GazePointValid         = 1 << 0,    /**< Button input type */
		WVR_GazeVectorValid        = 1 << 1,    /**< Touch input type */
		WVR_EyeOpennessValid       = 1 << 2,    /**< Analog input type */
		WVR_EyePupilDilationValid  = 1 << 3,
		WVR_EyePositionGuideValid  = 1 << 4,
	}

    public enum WVR_AMCMode
    {
        Off = 0,  // AW off.  Default mode.
        Force_UMC = 1,  // UMC(ASW) always on.
        Auto = 2,  // AW will dynamically turn on or off according to the rendering and performance status.
        Force_PMC = 3,  // PMC will be always ON and UMC will be disabled.
    }

    public enum WVR_AMCState
    {
        Off,
        UMC,
        PMC
    }

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_RenderInitParams_t
	{
		public WVR_GraphicsApiType graphicsApi;
		public UInt64 renderConfig;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Matrix4f_t
	{
		public float m0; //float[4][4]
		public float m1;
		public float m2;
		public float m3;
		public float m4;
		public float m5;
		public float m6;
		public float m7;
		public float m8;
		public float m9;
		public float m10;
		public float m11;
		public float m12;
		public float m13;
		public float m14;
		public float m15;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Vector2f_t
	{
		public float v0;
		public float v1;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Vector3f_t
	{
		public float v0;  // float[3]
		public float v1;
		public float v2;

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Vector3f_t)
				return this == (WVR_Vector3f_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return v0.GetHashCode() ^ v1.GetHashCode() ^ v2.GetHashCode();
		}

		public static bool operator ==(WVR_Vector3f_t lhs, WVR_Vector3f_t rhs)
		{
			return lhs.v0 == rhs.v0 && lhs.v1 == rhs.v1 && lhs.v2 == rhs.v2;
		}

		public static bool operator !=(WVR_Vector3f_t lhs, WVR_Vector3f_t rhs)
		{
			return lhs.v0 != rhs.v0 || lhs.v1 != rhs.v1 || lhs.v2 != rhs.v2;
		}

		public WVR_Vector3f_t(float x, float y, float z)
		{
			v0 = x;
			v1 = y;
			v2 = z;
		}
		public static WVR_Vector3f_t Zero {
			get {
				return new WVR_Vector3f_t(0, 0, 0);
			}
		}

		// No coordination conversion
		public Vector3 ToVector3()
		{
			return new Vector3(v0, v1, v2);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CameraIntrinsic_t
	{
		public WVR_Vector2f_t focalLength;
		public WVR_Vector2f_t principalPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CameraInfo_t
	{
		public WVR_CameraImageType imgType;	// SINGLE OR STEREO image
		public WVR_CameraImageFormat imgFormat;
		public uint width;
		public uint height;
		public uint size;	   // The buffer size for raw image data
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Quatf_t
	{
		public float w;
		public float x;
		public float y;
		public float z;

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Quatf_t)
				return this == (WVR_Quatf_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return w.GetHashCode() ^ x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
		}

		public static bool operator ==(WVR_Quatf_t lhs, WVR_Quatf_t rhs)
		{
			return lhs.w == rhs.w && lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(WVR_Quatf_t lhs, WVR_Quatf_t rhs)
		{
			return lhs.w != rhs.w || lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		public WVR_Quatf_t(float in_x, float in_y, float in_z, float in_w)
		{
			x = in_x;
			y = in_y;
			z = in_z;
			w = in_w;
		}
		public static WVR_Quatf_t Identity {
			get {
				return new WVR_Quatf_t(0, 0, 0, 1);
			}
		}

		// No coordination conversion
		public Quaternion ToQuaternion()
		{
			return new Quaternion(x, y, z, w);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_PoseState_t	// [FieldOffset(164)]
	{
		[FieldOffset(0)] public bool IsValidPose;
		[FieldOffset(4)] public WVR_Matrix4f_t PoseMatrix;
		[FieldOffset(68)] public WVR_Vector3f_t Velocity;
		[FieldOffset(80)] public WVR_Vector3f_t AngularVelocity;
		[FieldOffset(92)] public bool Is6DoFPose;
		[FieldOffset(96)] public long PoseTimestamp_ns;
		[FieldOffset(104)] public WVR_Vector3f_t Acceleration;
		[FieldOffset(116)] public WVR_Vector3f_t AngularAcceleration;
		[FieldOffset(128)] public float PredictedMilliSec;
		[FieldOffset(132)] public WVR_PoseOriginModel OriginModel;
		[FieldOffset(136)] public WVR_Pose_t RawPose;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_DevicePosePair_t
	{
		[FieldOffset(0)] public WVR_DeviceType type;
		[FieldOffset(8)] public WVR_PoseState_t pose;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_TextureLayout_t
	{
		[FieldOffset(0)] public WVR_Vector2f_t leftLowUVs;
		[FieldOffset(8)] public WVR_Vector2f_t rightUpUVs;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_TextureBound_t
	{
		[FieldOffset(0)] public float uMin;
		[FieldOffset(4)] public float vMin;
		[FieldOffset(8)] public float uMax;
		[FieldOffset(12)] public float vMax;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TextureParams_t
	{
		public IntPtr id;
		public WVR_TextureTarget target;
		public WVR_TextureLayout_t layout;
		public IntPtr depth;
		public IntPtr projectionMatrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_LayerParams_t
    {
		public WVR_Eye eye;
		public IntPtr id;
		public WVR_TextureTarget target;
		public WVR_TextureLayout_t layout;
		public WVR_TextureOptions opts;
		public WVR_TextureShape shape;
		public WVR_TextureLayerType type;
		public uint compositionDepth;
		public IntPtr pose;
		public IntPtr poseTransform;
		public IntPtr scale;
		public uint width;
		public uint height;
		public float cylinderRadius;
		public IntPtr depth;
		public IntPtr projectionMatrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_LayerSetParams_t
	{
		public WVR_LayerParams_t textureL;
		public WVR_LayerParams_t textureR;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_RenderProps_t
	{
		public float refreshRate;
		public bool hasExternal;
		public float ipdMeter;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CommonEvent_t
	{
		public WVR_EventType type;
		public long timestamp;		 // Delivered time in nanoseconds
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_DeviceEvent_t
	{
		public WVR_CommonEvent_t common;
		public WVR_DeviceType type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_InputEvent_t
	{
		public WVR_DeviceEvent_t device;
		public WVR_InputId inputId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TrackerEvent_t
	{
		public WVR_CommonEvent_t common;
		public WVR_TrackerId trackerId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TrackerInputEvent
	{
		public WVR_TrackerEvent_t tracker;
		public WVR_InputId inputId;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_Event_t
	{
		[FieldOffset(0)] public WVR_CommonEvent_t common;
		[FieldOffset(0)] public WVR_DeviceEvent_t device;
		[FieldOffset(0)] public WVR_InputEvent_t input;
		[FieldOffset(0)] public WVR_TrackerEvent_t tracker;      /**< @ref WVR_TrackerEvent_t*/
		[FieldOffset(0)] public WVR_TrackerInputEvent trackerInput; /**< @ref WVR_TrackerInputEvent*/
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Axis_t
	{
		public float x;
		public float y;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_AnalogState_t
	{
		[FieldOffset(0)] public WVR_InputId id;
		[FieldOffset(4)] public WVR_AnalogType type;
		[FieldOffset(8)] public WVR_Axis_t axis;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_InputAttribute_t
	{
		public WVR_InputId id;
		public uint capability;
		public WVR_AnalogType axis_type;

	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_InputMappingPair_t
	{
		public WVR_InputAttribute_t destination;
		public WVR_InputAttribute_t source;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_Pose_t    // [FieldOffset(28)]
	{
		[FieldOffset(0)] public WVR_Vector3f_t position;
		[FieldOffset(12)] public WVR_Quatf_t rotation;

		public static bool operator ==(WVR_Pose_t lhs, WVR_Pose_t rhs)
		{
			return lhs.position == rhs.position && lhs.rotation == rhs.rotation;
		}

		public static bool operator !=(WVR_Pose_t lhs, WVR_Pose_t rhs)
		{
			return lhs.position != rhs.position || lhs.rotation != rhs.rotation;
		}

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Pose_t)
				return this == (WVR_Pose_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return position.GetHashCode() ^ rotation.GetHashCode();
		}

		public WVR_Pose_t(WVR_Vector3f_t in_pos, WVR_Quatf_t in_rot)
		{
			position = in_pos;
			rotation = in_rot;
		}

		public static WVR_Pose_t Identity
		{
			get
			{
				return new WVR_Pose_t(WVR_Vector3f_t.Zero, WVR_Quatf_t.Identity);
			}
		}

		// No coordination conversion
		public Pose ToPose()
		{
			return new Pose(position.ToVector3(), rotation.ToQuaternion());
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_OverlayPosition_t
	{
		public float x;
		public float y;
		public float z;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_OverlayBlendColor_t
	{
		public float r;
		public float g;
		public float b;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_OverlayTexture_t
	{
		public uint textureId;
		public uint width;
		public uint height;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ArenaRectangle_t
	{
		public float width;
		public float length;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ArenaRound_t
	{
		public float diameter;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_ArenaArea_t
	{
		[FieldOffset(0)] public WVR_ArenaRectangle_t rectangle;
		[FieldOffset(0)] public WVR_ArenaRound_t round;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Arena_t
	{
		public WVR_ArenaShape shape;
		public WVR_ArenaArea_t area;
	}

	public delegate void WVR_OverlayInputEventCallback(int overlayId, WVR_EventType type, WVR_InputId inputId);
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_OverlayInputEvent_t
	{
		public int overlayId;
		public IntPtr callback;
	}

	public struct WVR_RenderFoveationParams
	{
		public float focalX;
		public float focalY;
		public float fovealFov;
		public WVR_PeripheralQuality periQuality;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FBXInfo_t
	{
		//public char* name;
		public WVR_Matrix4f_t matrix;
		public uint verticeCount;
		public uint normalCount;
		public uint uvCount;
		public uint indiceCount;
		public IntPtr meshName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MeshInfo_t
	{
		public Vector3[] _vectice;
		public Vector3[] _normal;
		public Vector2[] _uv;
		public int[] _indice;
		public bool _active;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StencilMesh_t
	{
		public uint vertCount;	  // uint32_t
		public IntPtr vertData;	 // float*
		public uint triCount;	   // uint32_t
		public IntPtr indexData;	// uint16_t*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_RequestResult
	{
		public string mPermission;
		public bool mGranted;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_VertexBuffer
	{
		public IntPtr buffer;  // float*
		public uint size;
		public uint dimension;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_IndexBuffer
	{
		public IntPtr buffer; // uint*
		public uint size;
		public uint type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerCompInfo
	{
		public WVR_VertexBuffer vertices;
		public WVR_VertexBuffer normals;
		public WVR_VertexBuffer texCoords;
		public WVR_IndexBuffer indices;
		public uint texIndex;
		public WVR_Matrix4f_t localMat;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string name;  // char[64]
		public bool defaultDraw;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerTexBitmap
	{
		public IntPtr bitmap; // byte*
		public uint width;
		public uint height;
		public uint stride;
		public int format;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TouchPadPlane
	{
		public WVR_Vector3f_t u;
		public WVR_Vector3f_t v;
		public WVR_Vector3f_t w;
		public WVR_Vector3f_t center;
		public float floatingDistance;
		public float radius;
		public bool valid;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_BatteryLevelTable
	{
		public IntPtr texTable; // WVR_CtrlerTexBitmap*
		public IntPtr minTable; // int*
		public IntPtr maxTable; // int*
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerCompInfoTable
	{
		public IntPtr table; // WVR_CtrlerCompInfo*
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerTexBitmapTable
	{
		public IntPtr table; // WVR_CtrlerTexBitmap*
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerModel
	{
		public WVR_CtrlerCompInfoTable compInfos;
		public WVR_CtrlerTexBitmapTable bitmapInfos;
		public WVR_TouchPadPlane touchpadPlane;
		public WVR_BatteryLevelTable batteryLevels;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string name;  // char[256]
		public bool loadFromAsset;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerModelAnimPoseData
	{
		public WVR_Vector3f_t position;
		public WVR_Vector3f_t rotation;
		public WVR_Vector3f_t scale;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerModelAnimNodeData
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string name;
		public uint type;
		public uint blueEffect;
		public WVR_CtrlerModelAnimPoseData origin;
		public WVR_CtrlerModelAnimPoseData pressed;
		public WVR_CtrlerModelAnimPoseData minX;
		public WVR_CtrlerModelAnimPoseData maxX;
		public WVR_CtrlerModelAnimPoseData minY;
		public WVR_CtrlerModelAnimPoseData maxY;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_CtrlerModelAnimData
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string name;
		public IntPtr animDatas; // WVR_CtrlerModelAnimNodeData_t*
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_BoneIDBuffer
	{
		public IntPtr buffer; // int*
		public uint size;
		public uint dimension;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandModel
	{
		public WVR_VertexBuffer vertices;
		public WVR_VertexBuffer normals;
		public WVR_VertexBuffer texCoords;
		public WVR_VertexBuffer texCoord2s;
		public WVR_BoneIDBuffer boneIDs;
		public WVR_VertexBuffer boneWeights;
		public WVR_IndexBuffer indices;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
		public Matrix4x4[] jointInvTransMats;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
		public Matrix4x4[] jointTransMats;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
		public Matrix4x4[] jointLocalTransMats;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
		public uint[] jointParentTable;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
		public int[] jointUsageTable;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandRenderModel
	{
		public WVR_HandModel left;
		public WVR_HandModel right;
		public WVR_CtrlerTexBitmap handAlphaTex;
	}

	#region Eye Tracking
	/**
	 * @brief Structure containing flags indicating data validity of an eye pose
	 */
	public enum WVR_EyeTrackingStatus
	{
		WVR_GazeOriginValid                 = 1<<0,
		WVR_GazeDirectionNormalizedValid    = 1<<1,
		WVR_PupilDiameterValid              = 1<<2,
		WVR_EyeOpennessValid                = 1<<3,
		WVR_PupilPositionInSensorAreaValid  = 1<<4
	}

	/**
	 * @brief the single eye tracking data
	 *
	 * the single eye tracking data
	 */
	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_SingleEyeTracking_t	// [FieldOffset(48)]
	{
		/**< The bits containing all validity for this data. */
		[FieldOffset(0)] public ulong			eyeTrackingValidBitMask;
		/**< The point in the eye from which the gaze ray originates in meters. */
		[FieldOffset(8)] public WVR_Vector3f_t	gazeOrigin;
		/**< The normalized gaze direction of the eye in [0,1]. */
		[FieldOffset(20)] public WVR_Vector3f_t	gazeDirectionNormalized;
		/**< A value representing how open the eye is. */
		[FieldOffset(32)] public float			eyeOpenness;
		/**< The diameter of pupil in millimeters. */
		[FieldOffset(36)] public float			pupilDiameter;
		/**< The normalized position of a pupil in [0,1]. */
		[FieldOffset(40)] public WVR_Vector2f_t	pupilPositionInSensorArea;
	}

	/**
	 * @brief the combined eye tracking data
	 *
	 * the combined eye tracking data
	 */
	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_CombinedEyeTracking_t	// [FieldOffset(32)]
	{
		[FieldOffset(0)] public ulong			eyeTrackingValidBitMask;    // TODO
		[FieldOffset(8)] public WVR_Vector3f_t	gazeOrigin;
		[FieldOffset(20)] public WVR_Vector3f_t	gazeDirectionNormalized;
	}

	/**
	 * @brief the eye tracking data
	 *
	 * the eye tracking data
	 */
	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_EyeTracking_t	// [FieldOffset(136)]
	{
		[FieldOffset(0)] public WVR_SingleEyeTracking_t left;
		[FieldOffset(48)] public WVR_SingleEyeTracking_t right;
		[FieldOffset(96)] public WVR_CombinedEyeTracking_t combined;
		[FieldOffset(128)] public long timestamp;
	}
	#endregion

	public enum WVR_QualityStrategy
	{
		WVR_QualityStrategy_Default = 1,                    /**< Auto adjust CPU/GPU performane level if need. */
		WVR_QualityStrategy_SendQualityEvent = 1 << 1,      /**< Send recommended quality changed event if need. */
		WVR_QualityStrategy_AutoFoveation = 1 << 2,         /**< Auto adjust foveation rendering intensity if need. */
        WVR_QualityStrategy_AutoAMC = 1 << 3,               /**< Experimental function */
        WVR_QualityStrategy_Reserved_2 = 1 << 28,             /**< System reserved. */
        WVR_QualityStrategy_Reserved_1 = 1 << 29,             /**< System reserved. */
        WVR_QualityStrategy_Reserved = 1 << 30,             /**< System reserved. */
	}

	/**
	 * @brief the returned result of a function call for providing the information of failure.
	 */
	public enum WVR_Result
	{
		WVR_Success                              = 0,    /**< The result of the function call was successful. */
		WVR_Error_SystemInvalid                  = 1,    /**< The initialization was not finished or the feature was not started yet. */
		WVR_Error_InvalidArgument                = 2,    /**< One of the arguments was not appropriate for the function call. */
		WVR_Error_OutOfMemory                    = 3,    /**< A memory allocation has failed. */
		WVR_Error_FeatureNotSupport              = 4,    /**< The feature was not supported; either lack of some services or service does not support this feature. */
		WVR_Error_RuntimeVersionNotSupport       = 5,    /**< The runtime version is too old to support the function call. */
		WVR_Error_CameraNotAvailable             = 6,    /**< Camera was unavailable, cannot query camera related information. */
		WVR_Error_CameraPermissionNotGranted     = 7,    /**< The Android camera permission was not granted yet. */
		WVR_Error_DeviceDisconnected             = 8,    /**< The device is disconnected */
		WVR_Error_TrackerDisconnected            = 9,    /**< The tracker is disconnected. */

		WVR_Error_CtrlerModel_WrongDeviceType    = 100,  /**< Input wrong device type for asking controller model. */
		WVR_Error_CtrlerModel_DeviceDisconnected = 101,  /**< The controller device you want to get its model is disconnected. */
		WVR_Error_CtrlerModel_InvalidModel       = 102,  /**< We can't get model that can be use. */
		WVR_Error_CtrlerModel_Unknown            = 103,  /**< Unknown error. */
		WVR_Error_CtrlerModel_NoAnimationData    = 104,

		WVR_Error_InvalidRenderModel             = 110,

		WVR_Error_EyeTracking_NotInitial         = 200,  /**< The eye calibration procedure has not been initialized. */
		WVR_Error_EyeTracking_NotWorking         = 201,  /**< The operation of eye tracking is not working. */

		WVR_Error_HandTracking_FeatureNotRequested  = 300, /**< The AndroidManifest.xml of this VR App does not request hand tracking feature.*/
		WVR_Error_Tracker_FeatureNotRequested       = 301, /**< The AndroidManifest.xml of this VR App does not request tracker feature.*/
		WVR_Error_EyeTracking_FeatureNotRequested   = 302, /**< The AndroidManifest.xml of this VR App does not request eye tracking feature.*/
		WVR_Error_LipExpression_FeatureNotRequested = 303, /**< The AndroidManifest.xml of this VR App does not request lip expression feature.*/
		WVR_Error_EyeExpression_FeatureNotRequested = 304, /**< The AndroidManifest.xml of this VR App does not request eye expression feature.*/

		WVR_Error_Data_Invalid = 400   /**< Data is invalid or unavailabe at this moment, ex., visual occlusion. */
	}


	#region Hand
	/**
	 * @brief The gesture type
	 * @version API Level 6
	 */
	public enum WVR_HandGestureType
	{
		WVR_HandGestureType_Invalid = 0,    /**< The gesture is invalid. */
		WVR_HandGestureType_Unknown = 1,    /**< Unknow gesture type. */
		WVR_HandGestureType_Fist = 2,       /**< Represent fist gesture. */
		WVR_HandGestureType_Five = 3,       /**< Represent five gesture. */
		WVR_HandGestureType_OK = 4,         /**< Represent OK gesture. */
		WVR_HandGestureType_ThumbUp = 5,    /**< Represent thumb up gesture. */
		WVR_HandGestureType_IndexUp = 6,    /**< Represent index up gesture. */
		WVR_HandGestureType_Palm_Pinch = 7, /**< Represent inverse pinch gesture. */
		WVR_HandGestureType_Yeah = 8,       /**< Represent yeah gesture. */
		WVR_HandGestureType_Reserved1 = 32,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved2 = 33,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved3 = 34,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved4 = 35,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved5 = 36,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved6 = 37,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved7 = 38,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved8 = 39,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved9 = 40,   /**< Reserved gesture. */
		WVR_HandGestureType_Reserved10 = 41,   /**< Reserved gesture. */
	}

	/**
	 * @brief The type of hand tracker device.
	 * @version API Level 6
	 */
	public enum WVR_HandTrackerType
	{
		WVR_HandTrackerType_Natural = 1,
		WVR_HandTrackerType_Electronic = 2,
	};

	/**
	 * @brief The type of the hand model.
	 * @version API Level 6
	 */
	public enum WVR_HandModelType
	{
		WVR_HandModelType_WithoutController = 1 << 0,
		WVR_HandModelType_WithController = 1 << 1
	};

	/**
	 * @brief The flags that indicate data validity of one hand joint.
	 * @version API Level 6
	 */
	public enum WVR_HandJointValidFlag
	{
		WVR_HandJointValidFlag_PositionValid = 1 << 0,
		WVR_HandJointValidFlag_RotationValid = 1 << 1
	};

	/**
	 * @brief The conventions of hand joints
	 * @version API Level 6
	 */
	public enum WVR_HandJoint
	{
		WVR_HandJoint_Palm = 0,
		WVR_HandJoint_Wrist = 1,
		WVR_HandJoint_Thumb_Joint0 = 2,
		WVR_HandJoint_Thumb_Joint1 = 3,
		WVR_HandJoint_Thumb_Joint2 = 4,
		WVR_HandJoint_Thumb_Tip = 5,
		WVR_HandJoint_Index_Joint0 = 6,
		WVR_HandJoint_Index_Joint1 = 7,
		WVR_HandJoint_Index_Joint2 = 8,
		WVR_HandJoint_Index_Joint3 = 9,
		WVR_HandJoint_Index_Tip = 10,
		WVR_HandJoint_Middle_Joint0 = 11,
		WVR_HandJoint_Middle_Joint1 = 12,
		WVR_HandJoint_Middle_Joint2 = 13,
		WVR_HandJoint_Middle_Joint3 = 14,
		WVR_HandJoint_Middle_Tip = 15,
		WVR_HandJoint_Ring_Joint0 = 16,
		WVR_HandJoint_Ring_Joint1 = 17,
		WVR_HandJoint_Ring_Joint2 = 18,
		WVR_HandJoint_Ring_Joint3 = 19,
		WVR_HandJoint_Ring_Tip = 20,
		WVR_HandJoint_Pinky_Joint0 = 21,
		WVR_HandJoint_Pinky_Joint1 = 22,
		WVR_HandJoint_Pinky_Joint2 = 23,
		WVR_HandJoint_Pinky_Joint3 = 24,
		WVR_HandJoint_Pinky_Tip = 25,
	};

	/**
	 * 21Aug New.
	 * @brief The object type of hand hold
	 * @version API Level 7
	 */
	public enum WVR_HandHoldObjectType
	{
		WVR_HandHoldObjectType_None       = 0,  /**< The object type of hand hold is none. */
		WVR_HandHoldObjectType_Gun        = 1,  /**< The object type of hand hold is a gun. */
		WVR_HandHoldObjectType_OCSpray    = 2,  /**< The object type of hand hold is a OC spray. */
		WVR_HandHoldObjectType_LongGun    = 3,  /**< The object type of hand hold is a long gun. */
		WVR_HandHoldObjectType_Baton      = 4,  /**< The object type of hand hold is a baton. */
		WVR_HandHoldObjectType_FlashLight = 5   /**< The object type of hand hold is a flashlight. */
	}

	/**
	 * 21Aug New.
	 * @brief The object type of hand hold
	 * @version API Level 7
	 */
	public enum WVR_HandHoldRoleType
	{
		WVR_HandHoldRoleType_None     = 0,  /**< The role type of hand hold is none. */
		WVR_HandHoldRoleType_MainHold = 1,  /**< The role type of hand hold is main hold. */
		WVR_HandHoldRoleType_SideHold = 2   /**< The role type of hand hold is side hold. */
	}

	/**
	 * @brief The gesture information
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandGestureInfo_t
	{
		public ulong supportedMask; // bitmask of @WVR_HandGestureType
	}

	/**
	 * @brief The gesture data
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandGestureData_t
	{
		public long timestamp;
		public WVR_HandGestureType right;
		public WVR_HandGestureType left;
	}

	/**
	 * @brief The finger name
	 * @version API Level 6
	 */
	public enum WVR_FingerType
	{
		WVR_FingerType_Thumb = 1,    /**< Represent thumb finger. */
		WVR_FingerType_Index = 2,    /**< Represent index finger. */
		WVR_FingerType_Middle = 3,    /**< Represent middle finger. */
		WVR_FingerType_Ring = 4,    /**< Represent ring finger. */
		WVR_FingerType_Pinky = 5     /**< Represent pinky finger. */
	}

	/**
	 * 21Aug New.
	 * @brief The hand pose type definition
	 * @version API Level 7
	 */
	public enum WVR_HandPoseType
	{
		WVR_HandPoseType_Invalid = 0,  /**< The hand pose type is invalid. */
		WVR_HandPoseType_Pinch = 1,  /**< The hand pose type is pinch. */
		WVR_HandPoseType_Hold = 2,  /**< The hand pose type is hold. */
	}

	/**
	 * @brief The common pose state information
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandPoseStateBase_t
	{
		public WVR_HandPoseType type;         /**< The current hand pose type. */
	}

	/**
	 * @brief The pinch state
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandPosePinchState_t
	{
		public WVR_HandPoseStateBase_t state;   /**< Refer to @ref WVR_HandPoseStateBase_t */
		public WVR_FingerType finger;           /**< Move someone finger and thumb apart or bring them together.*/
		public float strength;                  /**< The value of ranges between 0 to 1 for each finger pich, 1 means pinch with the finger touching the thumb fully. */
		public WVR_Vector3f_t origin;           /**< The pinch origin. */
		public WVR_Vector3f_t direction;        /**< The pinch direction. */
		public bool isPinching;
	}

	/**
	 * 21Aug New.
	 * @brief The hand hold state.
	 * @version API Level 7
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandPoseHoldState_t
	{
		public WVR_HandPoseStateBase_t state;    /**< Refer to @ref WVR_HandPoseStateBase_t. */
		public WVR_HandHoldRoleType role;        /**< The role type of hand hold, refer to @ref WVR_HandHoldRoleType. */
		public WVR_HandHoldObjectType type;      /**< The object type of hand hold, refer to @ref WVR_HandHoldObjectType. */
	}

	/**
	 * 21Aug New.
	 * @brief The hand pose state
	 * @version API Level 7
	 */
	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_HandPoseState_t
	{
		[FieldOffset(0)] public WVR_HandPoseStateBase_t state;
		[FieldOffset(0)] public WVR_HandPosePinchState_t pinch;
		[FieldOffset(0)] public WVR_HandPoseHoldState_t hold;
	}

	/**
	 * @brief The hands pose information
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandPoseData_t
	{
		public long timestamp;  /**< The current time in milliseconds. */
		public WVR_HandPoseState_t right;      /**< The pose state of right hand, refer to @ref WVR_HandPoseState_t. */
		public WVR_HandPoseState_t left;       /**< The pose state of left hand, refer to @ref WVR_HandPoseState_t. */
	}

	/**
	 * @brief The information of hand tracker device.
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandTrackerInfo_t
	{
		public uint jointCount;               /**< The count of hand joints. */
		public ulong handModelTypeBitMask;    /**< Support how many types of @ref WVR_HandModelType. */
		public IntPtr jointMappingArray;      /**< The array corresponds to the conventions of hand joints. Refer to @ref WVR_HandJoint. */
		public IntPtr jointValidFlagArray;    /**< The array that indicates which data of each hand joint is valid, refer to @ref WVR_HandJointValidFlag. */
		public float pinchThreshold;          /**< The value of ranges between 0 to 1. */
		public float pinchOff;
	}

	/**
	 * @brief The grasp state
	 * @version API Level 16
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandGraspState_t
	{
		public bool isGrasp;     /**< A grasp is happening or not, the binary version of strength. */
		public float strength;   /**< Value from 0 to 1, with 1 represents a fully grasp. */
	}

	/**
	 * @brief The data structure of one hand.
	 * @version API Level 6
	 * C++ armeabi-v7a type size:
	 * bool =	1 -> to 4
	 * char =	1
	 * uint =	4
	 * long =	4
	 * float =	4
	 * double =	8
	 * WVR_Pose_t* = 4
	 * Vector3 = 12
	 * WVR_HandJointData_t = 16
	 *
	 * C# armeabi-v7a type size:
	 * bool =	1 -> to 4
	 * => bool = 1 byte, which NOT satisfy the armeabi memory alignment. Needs padding 3 bytes.
	 * float =	4
	 * uint =	4
	 * IntPtr =	4
	 * Vector3 = 12
	 * WVR_HandJointData_t = 28
	 *
	 * C++ arm64-v8a type size:
	 * bool =	1 -> to 4
	 * char =	1
	 * uint =	4 -> to 8
	 * long =	8
	 * float =	4
	 * double =	8
	 * WVR_Pose_t* = 8
	 * Vector3 = 12
	 * WVR_HandJointData_t = 36
	 *
	 * C# arm64-v8a type size:
	 * bool =	1 -> to 4
	 * float =	4
	 * => bool + float = 8, which satisfies the arm64 memory alignment.
	 * uint =	4 -> to 8
	 * => UInt32 = 4 bytes, which NOT satisfy the arm64 memory alignment. Needs padding 4 bytes.
	 * IntPtr =	8
	 * Vector3 = 12
	 * WVR_HandJointData_t = 36
	 */
	/**
	 * @brief The data structure of one hand.
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandJointData_t
	{
		public bool isValidPose;     /**< The label of valid(true)/invalid(false) pose. */
		public float confidence;     /**< The hand confidence value. */
		public uint jointCount;      /**< Specify the size of the @ref WVR_Pose_t array. */
		public IntPtr joints;        /**< The array of the @ref WVR_Pose_t. */
		public WVR_Vector3f_t scale; /**< defualt is 1. */
		public WVR_Vector3f_t wristLinearVelocity;
		public WVR_Vector3f_t wristAngularVelocity;
		public WVR_HandGraspState_t grasp;
	}

	/**
	 * @brief The data structure of one hand that copy from WVR_HandTrackingData_t since DP not support hand grasp.
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_DPHandJointData_t
	{
		public bool isValidPose;     /**< The label of valid(true)/invalid(false) pose. */
		public float confidence;     /**< The hand confidence value. */
		public uint jointCount;      /**< Specify the size of the @ref WVR_Pose_t array. */
		public IntPtr joints;        /**< The array of the @ref WVR_Pose_t. */
		public WVR_Vector3f_t scale; /**< defualt is 1. */
		public WVR_Vector3f_t wristLinearVelocity;
		public WVR_Vector3f_t wristAngularVelocity;
	}

	[StructLayout(LayoutKind.Sequential)]
    public struct HandJointData26
    {
        public bool isValidPose;
        public float confidence;
        public uint jointCount;
        public WVR_Pose_t j00;
        public WVR_Pose_t j01;
        public WVR_Pose_t j02;
        public WVR_Pose_t j03;
        public WVR_Pose_t j04;
        public WVR_Pose_t j05;
        public WVR_Pose_t j06;
        public WVR_Pose_t j07;
        public WVR_Pose_t j08;
        public WVR_Pose_t j09;
        public WVR_Pose_t j10;
        public WVR_Pose_t j11;
        public WVR_Pose_t j12;
        public WVR_Pose_t j13;
        public WVR_Pose_t j14;
        public WVR_Pose_t j15;
        public WVR_Pose_t j16;
        public WVR_Pose_t j17;
        public WVR_Pose_t j18;
        public WVR_Pose_t j19;
        public WVR_Pose_t j20;
        public WVR_Pose_t j21;
        public WVR_Pose_t j22;
        public WVR_Pose_t j23;
        public WVR_Pose_t j24;
        public WVR_Pose_t j25;
        public WVR_Vector3f_t scale;
		public WVR_Vector3f_t wristLinearVelocity;
		public WVR_Vector3f_t wristAngularVelocity;
	}

    /**
	 * @brief The data structure of the hand tracker data that contains both hands.
	 * @version API Level 6
	 */
    [StructLayout(LayoutKind.Sequential)]
	public struct WVR_HandTrackingData_t
	{
		public long timestamp;
		public WVR_HandJointData_t right;    /**< The hand tracker data of right hand, refer to @ref WVR_HandJointData_t. */
		public WVR_HandJointData_t left;     /**< The hand tracker data of left hand, refer to @ref WVR_HandJointData_t. */
	}

	/**
	 * @brief The data structure of the hand tracker data that copy from WVR_HandTrackingData_t since DP not support hand grasp.
	 * @version API Level 6
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_DPHandTrackingData_t
	{
		public long timestamp;
		public WVR_DPHandJointData_t right;    /**< The hand tracker data of right hand, refer to @ref WVR_HandJointData_t. */
		public WVR_DPHandJointData_t left;     /**< The hand tracker data of left hand, refer to @ref WVR_HandJointData_t. */
	}
	#endregion

	#region Tracker
	public enum WVR_TrackerId
	{
		WVR_TrackerId_0 = 0,
		WVR_TrackerId_1 = 1,
		WVR_TrackerId_2 = 2,
		WVR_TrackerId_3 = 3,
		WVR_TrackerId_4 = 4,
		WVR_TrackerId_5 = 5,
		WVR_TrackerId_6 = 6,
		WVR_TrackerId_7 = 7,
		WVR_TrackerId_8 = 8,
		WVR_TrackerId_9 = 9,
		WVR_TrackerId_10 = 10,
		WVR_TrackerId_11 = 11,
		WVR_TrackerId_12 = 12,
		WVR_TrackerId_13 = 13,
		WVR_TrackerId_14 = 14,
		WVR_TrackerId_15 = 15,
	}

	/**
	 * @brief The tracker role
	 * Describes the role of the tracker device.
	 * API Level 8 : 0 ~ 3, API Level 12 : 32~63, API Level 13 : 32~72
	 * @version API Level 12
	 **/
	public enum WVR_TrackerRole
	{
		WVR_TrackerRole_Undefined   = 0,
		WVR_TrackerRole_Standalone  = 1,
		WVR_TrackerRole_Pair1_Right = 2,
		WVR_TrackerRole_Pair1_Left  = 3,

		WVR_TrackerRole_Shoulder_Right = 32,
		WVR_TrackerRole_Upper_Arm_Right = 33,
		WVR_TrackerRole_Elbow_Right = 34,
		WVR_TrackerRole_Forearm_Right   = 35,
		WVR_TrackerRole_Wrist_Right     = 36,
		WVR_TrackerRole_Hand_Right = 37,
		WVR_TrackerRole_Thigh_Right     = 38,
		WVR_TrackerRole_Knee_Right = 39,
		WVR_TrackerRole_Calf_Right      = 40,
		WVR_TrackerRole_Ankle_Right     = 41,
		WVR_TrackerRole_Foot_Right = 42,

		WVR_TrackerRole_Shoulder_Left = 47,
		WVR_TrackerRole_Upper_Arm_Left  = 48,
		WVR_TrackerRole_Elbow_Left = 49,
		WVR_TrackerRole_Forearm_Left    = 50,
		WVR_TrackerRole_Wrist_Left      = 51,
		WVR_TrackerRole_Hand_Left = 52,
		WVR_TrackerRole_Thigh_Left      = 53,
		WVR_TrackerRole_Knee_Left = 54,
		WVR_TrackerRole_Calf_Left       = 55,
		WVR_TrackerRole_Ankle_Left      = 56,
		WVR_TrackerRole_Foot_Left = 57,

		WVR_TrackerRole_Chest = 62,
		WVR_TrackerRole_Waist = 63,

		WVR_TrackerRole_Camera = 71,
		WVR_TrackerRole_Keyboard = 72,
	}

	/**
	 * @brief The capabilities of the accessory type.
	 * @version API Level 8
	 */
	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_TrackerCapabilities
	{
		[FieldOffset(0)] public bool supportsOrientationTracking;
		[FieldOffset(1)] public bool supportsPositionTracking;
		[FieldOffset(2)] public bool supportsInputDevice;
		[FieldOffset(3)] public bool supportsHapticVibration;
		[FieldOffset(4)] public bool supportsBatteryLevel;
	}

	public delegate void WVR_TrackerInfoCallback(WVR_TrackerId trackerId, IntPtr cbInfo, ref UInt64 timestamp);

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TrackerInfoNotify
	{
		public WVR_TrackerInfoCallback callback;
	}
	#endregion

	#region Lip Expression
	public enum WVR_LipExpression {
		WVR_LipExpression_Jaw_Right = 0,
		WVR_LipExpression_Jaw_Left = 1,
		WVR_LipExpression_Jaw_Forward = 2,
		WVR_LipExpression_Jaw_Open = 3,
		WVR_LipExpression_Mouth_Ape_Shape = 4,
		WVR_LipExpression_Mouth_Upper_Right = 5,		// 5
		WVR_LipExpression_Mouth_Upper_Left = 6,
		WVR_LipExpression_Mouth_Lower_Right = 7,
		WVR_LipExpression_Mouth_Lower_Left = 8,
		WVR_LipExpression_Mouth_Upper_Overturn = 9,
		WVR_LipExpression_Mouth_Lower_Overturn = 10,	// 10
		WVR_LipExpression_Mouth_Pout = 11,
		WVR_LipExpression_Mouth_Smile_Right = 12,
		WVR_LipExpression_Mouth_Smile_Left = 13,
		WVR_LipExpression_Mouth_Sad_Right = 14,
		WVR_LipExpression_Mouth_Sad_Left = 15,			// 15
		WVR_LipExpression_Cheek_Puff_Right = 16,
		WVR_LipExpression_Cheek_Puff_Left = 17,
		WVR_LipExpression_Cheek_Suck = 18,
		WVR_LipExpression_Mouth_Upper_Upright = 19,
		WVR_LipExpression_Mouth_Upper_Upleft = 20,		// 20
		WVR_LipExpression_Mouth_Lower_Downright = 21,
		WVR_LipExpression_Mouth_Lower_Downleft = 22,
		WVR_LipExpression_Mouth_Upper_Inside = 23,
		WVR_LipExpression_Mouth_Lower_Inside = 24,
		WVR_LipExpression_Mouth_Lower_Overlay = 25,		// 25
		WVR_LipExpression_Tongue_Longstep1 = 26,
		WVR_LipExpression_Tongue_Left = 27,
		WVR_LipExpression_Tongue_Right = 28,
		WVR_LipExpression_Tongue_Up = 29,
		WVR_LipExpression_Tongue_Down = 30,				// 30
		WVR_LipExpression_Tongue_Roll = 31,
		WVR_LipExpression_Tongue_Longstep2 = 32,
		WVR_LipExpression_Tongue_Upright_Morph = 33,
		WVR_LipExpression_Tongue_Upleft_Morph = 34,
		WVR_LipExpression_Tongue_Downright_Morph = 35,	// 35
		WVR_LipExpression_Tongue_Downleft_Morph = 36,
		WVR_LipExpression_Max
	}
	#endregion

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Uuid
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] data; //WVR_UUID_SIZE 16

		public override bool Equals(object rhs)
		{
			var other = (WVR_Uuid)rhs;
			if (data == null || other.data == null || data.Length != other.data.Length) return false;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] != other.data[i]) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				foreach (byte byteData in data)
				{
					hash = hash * 31 + byteData.GetHashCode();
				}

				return hash;
			}
		}

		public static bool operator ==(WVR_Uuid lhs, WVR_Uuid rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(WVR_Uuid lhs, WVR_Uuid rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override string ToString()
		{
			return BitConverter.ToString(data).Replace("-", "");
		}
	}

	#region Scene Perception
	//Scene Perception enums
	public enum WVR_ScenePerceptionTarget
	{
		WVR_ScenePerceptionTarget_2dPlane = 0,           /**< Specifies to get 2d plane data. */
		WVR_ScenePerceptionTarget_3dObject = 1,           /**< Specifies to get 3d object data. */
		WVR_ScenePerceptionTarget_SceneMesh = 2,           /**< Specifies to get scene meshes of surrounding environment. */
		WVR_ScenePerceptionTarget_Max = 0x7FFFFFFF
	}

	public enum WVR_ScenePerceptionState
	{
		WVR_ScenePerceptionState_Empty = 0,
		WVR_ScenePerceptionState_Observing = 1,
		WVR_ScenePerceptionState_Paused = 2,
		WVR_ScenePerceptionState_Completed = 3,
		WVR_ScenePerceptionState_Max = 0x7FFFFFFF
	}

	public enum WVR_ScenePlaneType
	{
		WVR_ScenePlaneType_Unknown = 0,
		WVR_ScenePlaneType_HorizontalUpwardFacing = 1,
		WVR_ScenePlaneType_HorizontalDownwardFacing = 2,
		WVR_ScenePlaneType_Vertical = 3,
		WVR_ScenePlaneType_Max = 0x7FFFFFFF         // app sets WVR_ScenePlaneType_Max meaning no filer.
	}

	public enum WVR_ScenePlaneLabel
	{
		WVR_ScenePlaneLabel_Unknown = 0,
		WVR_ScenePlaneLabel_Floor = 1,
		WVR_ScenePlaneLabel_Ceiling = 2,
		WVR_ScenePlaneLabel_Wall = 3,
		WVR_ScenePlaneLabel_Desk = 4,
		WVR_ScenePlaneLabel_Couch = 5,
		WVR_ScenePlaneLabel_Door = 6,
		WVR_ScenePlaneLabel_Window = 7,
		WVR_ScenePlaneLabel_Stage = 8,
		WVR_ScenePlaneLabel_Max = 0x7FFFFFFF        // app sets WVR_ScenePlaneLabel_Max meaning no filer.
	}

	public enum WVR_SceneMeshType
	{
		WVR_SceneMeshType_VisualMesh = 0,           /**< Specifies to get visualization meshes. */
		WVR_SceneMeshType_ColliderMesh = 1,           /**< Specifies to get collider meshes. */
		WVR_SceneMeshType_Max = 0x7FFFFFFF
	}

	public enum WVR_SpatialAnchorTrackingState
	{
		WVR_SpatialAnchorTrackingState_Tracking,
		WVR_SpatialAnchorTrackingState_Paused,
		WVR_SpatialAnchorTrackingState_Stopped
	}

	//Scene perception structs
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ScenePlaneFilter
	{
		public WVR_ScenePlaneType planeType;
		public WVR_ScenePlaneLabel planeLabel;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SceneMesh
	{
		public UInt64 meshBufferId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Extent2Df
	{
		public float width;
		public float height;

		// No coordination conversion
		public Vector3 ToVector2()
		{
			return new Vector3(width, height);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Extent3Df
	{
		public float width;  // X
		public float height;  // Y
		public float depth;  // Z

		// No coordination conversion
		public Vector3 ToVector3()
		{
			return new Vector3(width, height, depth);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SemanticLabelName
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public char[] name; //WVR_MAX_SEMANTIC_LABEL_NAME_SIZE 256

		public override string ToString()
		{
			// C#'s new String(name[256]).Length will return 256, which is not correct.
			int nullTerminatorIndex = Array.IndexOf(name, '\0');
			if (nullTerminatorIndex <= 0)
				return "";
			return new string(name, 0, nullTerminatorIndex);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ScenePlane
	{
		public WVR_Uuid uuid;
		public WVR_Uuid parentUuid;
		public UInt64 meshBufferId;
		public WVR_Pose_t pose;
		public WVR_Extent2Df extent;
		public WVR_ScenePlaneType planeType;
		public WVR_ScenePlaneLabel planeLabel;
		public WVR_SemanticLabelName semanticName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SceneObject
	{
		public WVR_Uuid uuid;
		public WVR_Uuid parentUuid;
		public UInt64 meshBufferId;
		public WVR_Pose_t pose;
		public WVR_Extent3Df extent;
		public WVR_SemanticLabelName semanticName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SceneMeshBuffer
	{
		public UInt32 vertexCapacityInput;
		public UInt32 vertexCountOutput;
		public IntPtr vertexBuffer;     //WVR_Vector3f*
		public UInt32 indexCapacityInput;
		public UInt32 indexCountOutput;
		public IntPtr indexBuffer;			//uint32_t*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorCreateInfo
	{
		public WVR_Pose_t pose;
		public WVR_PoseOriginModel originModel;
		public WVR_SpatialAnchorName anchorName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorCacheInfo
	{
		public UInt64 spatialAnchor;  /* WVR_SpatialAnchor */
		public WVR_SpatialAnchorName cachedSpatialAnchorName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorFromCacheNameCreateInfo
	{
		public WVR_SpatialAnchorName cachedSpatialAnchorName;
		public WVR_SpatialAnchorName spatialAnchorName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorName
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public char[] name; //WVR_MAX_SPATIAL_ANCHOR_NAME_SIZE 256

		public override string ToString()
		{
			// C#'s new String(name[256]).Length will return 256, which is not correct.
			int nullTerminatorIndex = Array.IndexOf(name, '\0');
			if (nullTerminatorIndex <= 0)
				return "";
			return new string(name, 0, nullTerminatorIndex);
		}

		public WVR_SpatialAnchorName(string nameStr)
		{
			name = new char[256];
			var strLen = 0;
			if (nameStr != null)
			{
				strLen = nameStr.Length;
				nameStr.CopyTo(0, name, 0, Math.Min(strLen, name.Length));
			}
			// Make sure the rest element in the array are all zero.  Native may not think this is a null-terminated string.
			for (int i = strLen; i < name.Length; i++)
				name[i] = '\0';
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorState
	{
		public WVR_SpatialAnchorTrackingState trackingState;
		public WVR_Pose_t pose;
		public WVR_SpatialAnchorName anchorName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorPersistInfo
	{
		public UInt64 spatialAnchor;  /* WVR_SpatialAnchor */
		public WVR_SpatialAnchorName persistedSpatialAnchorName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_PersistedSpatialAnchorCountGetInfo
	{
		public UInt32 maximumTrackingCount;
		public UInt32 currentTrackingCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_SpatialAnchorFromPersistenceNameCreateInfo
	{
		public WVR_SpatialAnchorName persistedSpatialAnchorName;
		public WVR_SpatialAnchorName spatialAnchorName;
	}


	#endregion

	#region Trackable Marker

	public enum WVR_MarkerObserverTarget
	{
		WVR_MarkerObserverTarget_Aruco = 0,
		WVR_MarkerObserverTarget_Max = 0x7FFFFFFF
	}

	public enum WVR_MarkerObserverState
	{
		WVR_MarkerObserverState_Idle = 0,			/**< indicates that marker observer is idle */
		WVR_MarkerObserverState_Detecting = 1,		/**< indicates that the surrounding markers are detected */ // detecting the surrounding markers
		WVR_MarkerObserverState_Tracking = 2,		/**< indicates that the indicated markers are tracked */ // tracking the indicated markers
		WVR_MarkerObserverState_Max = 0x7FFFFFFF
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_MarkerName
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public char[] name;   //WVR_MAX_MARKER_NAME_SIZE 256

		public override string ToString()
		{
			// C#'s new String(name[256]).Length will return 256, which is not correct.
			int nullTerminatorIndex = Array.IndexOf(name, '\0');
			if (nullTerminatorIndex <= 0)
				return "";
			return new string(name, 0, nullTerminatorIndex);
		}
	}

	public enum WVR_MarkerTrackingState
	{
		WVR_MarkerTrackingState_Detected,       /**< Detected state */
		WVR_MarkerTrackingState_Tracked,        /**< Tracked state */
		WVR_MarkerTrackingState_Paused,         /**< Paused state */
		WVR_MarkerTrackingState_Stopped         /**< Stopped state */
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ArucoMarker
	{
		public WVR_Uuid uuid;					/**< indicates the uuid of the aruco marker */
		public UInt64 trackerId;				/**< indicates the tracker id of the aruco marker */
		public float size;						/**< indicates the size */
		public WVR_MarkerTrackingState state;	/**< indicates the state */
		public WVR_Pose_t pose;					/**< indicates the pose of the aruco marker */
		public WVR_MarkerName markerName;		/**< indicates the name */
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_TrackableMarkerCreateInfo
	{
		public WVR_Uuid uuid;
		public WVR_MarkerName markerName;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_ArucoMarkerData
	{
		public UInt64 trackerId;     /**< indicates the tracker id of the aruco marker */
		public float size;          /**< indicates the size */
	}

	public struct WVR_TrackableMarkerState
	{ 
		public WVR_MarkerObserverTarget target;        /**< indicates the assignment observer target specific data */
		public WVR_MarkerTrackingState state;			/**< indicates the state */
		public WVR_Pose_t pose;							/**< indicates the pose of the trackable marker */
		public WVR_MarkerName markerName;				/**< indicates the name */
	}
	#endregion

	#region Body Tracking
	public enum WVR_BodyTrackingType : UInt32
	{
		WVR_BodyTrackingType_Invalid = 0,
		WVR_BodyTrackingType_HMD = 1,
		WVR_BodyTrackingType_Controller = 2,
		WVR_BodyTrackingType_Hand = 3,
		WVR_BodyTrackingType_WristTracker = 4,
		WVR_BodyTrackingType_ViveSelfTracker = 5,
		WVR_BodyTrackingType_ViveSelfTrackerIM = 6,
	}

	public enum WVR_TrackedDeviceRole : Int32
	{
		WVR_TrackedDeviceRole_Invalid = -1,

		WVR_TrackedDeviceRole_Hip = 0,
		WVR_TrackedDeviceRole_Chest = 1,
		WVR_TrackedDeviceRole_Head = 2,

		WVR_TrackedDeviceRole_LeftElbow = 3,
		WVR_TrackedDeviceRole_LeftWrist = 4,
		WVR_TrackedDeviceRole_LeftHand = 5,
		WVR_TrackedDeviceRole_LeftHandheld = 6,

		WVR_TrackedDeviceRole_RightElbow = 7,
		WVR_TrackedDeviceRole_RightWrist = 8,
		WVR_TrackedDeviceRole_RightHand = 9,
		WVR_TrackedDeviceRole_RightHandheld = 10,

		WVR_TrackedDeviceRole_LeftKnee = 11,
		WVR_TrackedDeviceRole_LeftAnkle = 12,
		WVR_TrackedDeviceRole_LeftFoot = 13,

		WVR_TrackedDeviceRole_RightKnee = 14,
		WVR_TrackedDeviceRole_RightAnkle = 15,
		WVR_TrackedDeviceRole_RightFoot = 16,

		WVR_TrackedDeviceRole_Max,
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_BodyTrackingExtrinsic
	{
		[FieldOffset(0)] public WVR_TrackedDeviceRole role;
		[FieldOffset(4)] public WVR_BodyTrackingType deviceType;
		[FieldOffset(8)] public WVR_Pose_t pose;

		public WVR_BodyTrackingExtrinsic(WVR_TrackedDeviceRole in_role, WVR_BodyTrackingType in_type, WVR_Pose_t in_pose)
		{
			role = in_role;
			deviceType = in_type;
			pose = in_pose;
		}
	}
	#endregion

	public enum WVR_SupportedFeature {
		WVR_SupportedFeature_PassthroughImage   = 1 << 0,    /**< Passthrough image feature type */
		WVR_SupportedFeature_PassthroughOverlay = 1 << 1,    /**< Passthrough overlay feature type */

		WVR_SupportedFeature_HandTracking       = 1 << 4,    /**< Hand tracking feature type */
		WVR_SupportedFeature_HandGesture        = 1 << 5,    /**< Hand gesture feature type */
		WVR_SupportedFeature_ElectronicHand     = 1 << 6,    /**< Electronic hand feature type */
		WVR_SupportedFeature_ColorGamutsRGB     = 1 << 7,    /**< Color gamut sRGB */
		WVR_SupportedFeature_ColorGamutP3       = 1 << 8,    /**< Color gamut P3 */
		WVR_SupportedFeature_EyeTracking        = 1 << 9,    /**< Tracking of Eye */
		WVR_SupportedFeature_EyeExp             = 1 << 10,   /**< Expression of Eye; Wide, Squeeze, Frown*/
		WVR_SupportedFeature_LipExp             = 1 << 11,   /**< Expression of Lip; Jaw, Mouth, Cheek, Tongue*/
		WVR_SupportedFeature_Tracker            = 1 << 16,   /**< Tracker feature type */
		WVR_SupportedFeature_ScenePerception	= 1 << 17,   /**< Scene Perception feature type */
		WVR_SupportedFeature_Marker				= 1 << 18,   /**< Marker feature type */
	}

	#region Controller Pose Mode
	public enum WVR_ControllerPoseMode
	{
		WVR_ControllerPoseMode_Raw      = 0, /**< Raw mode (default). It would be the same as one of three other modes (Trigger/Panel/Handle). */
		WVR_ControllerPoseMode_Trigger	= 1, /**< Trigger mode: Controller ray is parallel to the trigger button of controller. */
		WVR_ControllerPoseMode_Panel	= 2, /**< Panel mode: Controller ray is parallel to the panel of controller. */
		WVR_ControllerPoseMode_Handle	= 3, /**< Handle mode: Controller ray is parallel to the handle of controller. */
	}
	#endregion

	public struct WVR_SpectatorState
	{
		public bool shouldRender;
	}

	public enum WVR_PassthroughImageQuality
	{
		DefaultMode = 0,  // default
		PerformanceMode = 1,
		QualityMode = 2,
	}
	
	public enum WVR_PassthroughImageFocus
	{
		Scale = 0,  // default
		View = 1
	}

	/**
	 * @brief Enum used for indicating the passthrough image's refresh rate.
	 */
	public enum WVR_PassthroughImageRate
	{
		Boost = 0,    // Default passthrough image's refresh rate (default).
		Normal = 1     // Reduce the passthrough image's refresh rate for performance improvement.
	}

	public delegate void WVR_RequestCompleteCallback(List<WVR_RequestResult> results);
	public delegate void WVR_RequestUsbCompleteCallback(bool result);
	public delegate void WVR_OnOEMConfigChanged();

	public class Interop
	{
		#region Interaction
		public static bool WVR_PollEventQueue(ref WVR_Event_t e)
		{
			return WVR_Base.Instance.PollEventQueue(ref e);
		}

		public static int WVR_GetInputDeviceCapability(WVR_DeviceType type, WVR_InputType inputType)
		{
			return WVR_Base.Instance.GetInputDeviceCapability(type, inputType);
		}

		public static WVR_AnalogType WVR_GetInputDeviceAnalogType(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_Base.Instance.GetInputDeviceAnalogType(type, id);
		}

		public static bool WVR_GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
			[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
		{
			return WVR_Base.Instance.GetInputDeviceState(type, inputMask, ref buttons, ref touches, analogArray, analogArrayCount);
		}

		public static int WVR_GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
		{
			return WVR_Base.Instance.GetInputTypeCount(type, inputType);
		}

		public static bool WVR_GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_Base.Instance.GetInputButtonState(type, id);
		}

		public static bool WVR_GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_Base.Instance.GetInputTouchState(type, id);
		}

		public static WVR_Axis_t WVR_GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
		{
			return WVR_Base.Instance.GetInputAnalogAxis(type, id);
		}

		public static void WVR_GetPoseState(WVR_DeviceType type, WVR_PoseOriginModel originModel, uint predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			WVR_Base.Instance.GetPoseState(type, originModel, predictedMilliSec, ref poseState);
		}

		public static void WVR_SetTextureBounds([In, Out] WVR_TextureBound_t[] textureBounds)
		{
			WVR_Base.Instance.SetTextureBounds(textureBounds);
		}

		public static void WVR_GetLastPoseIndex(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount, ref uint frameIndex)
		{
			WVR_Base.Instance.GetLastPoseIndex(originModel, poseArray, pairArrayCount, ref frameIndex);
		}
		public static void WVR_WaitGetPoseIndex(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount, ref uint frameIndex)
		{
			WVR_Base.Instance.WaitGetPoseIndex(originModel, poseArray, pairArrayCount, ref frameIndex);
		}
		public static System.IntPtr WVR_StoreRenderTextures(System.IntPtr[] texturesIDs, int size, bool eEye, WVR_TextureTarget target)
		{
			return WVR_Base.Instance.StoreRenderTextures(texturesIDs, size, eEye, target);
		}

		public static void WVR_GetSyncPose(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
		{
			WVR_Base.Instance.GetSyncPose(originModel, poseArray, pairArrayCount);
		}

		public static bool WVR_IsDeviceConnected(WVR_DeviceType type)
		{
			return WVR_Base.Instance.IsDeviceConnected(type);
		}

		public static void WVR_TriggerVibration(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity)
		{
			WVR_Base.Instance.TriggerVibration(type, id, durationMicroSec, frequency, intensity);
		}

		public static void WVR_TriggerVibrationScale(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, float amplitude)
		{
			WVR_Base.Instance.TriggerVibrationScale(type, id, durationMicroSec, frequency, amplitude);
		}

		public static void WVR_InAppRecenter(WVR_RecenterType recenterType)
		{
			WVR_Base.Instance.InAppRecenter(recenterType);
		}

		public static void WVR_SetScreenProtection(bool enabled)
		{
			WVR_Base.Instance.SetScreenProtection(enabled);
		}

		public static void WVR_SetNeckModelEnabled(bool enabled)
		{
			WVR_Base.Instance.SetNeckModelEnabled(enabled);
		}

		public static void WVR_SetNeckModel(WVR_SimulationType simulationType)
		{
			WVR_Base.Instance.SetNeckModel(simulationType);
		}

		public static void WVR_SetArmModel(WVR_SimulationType simulationType)
		{
			WVR_Base.Instance.SetArmModel(simulationType);
		}

		public static void WVR_SetArmSticky(bool stickyArm)
		{
			WVR_Base.Instance.SetArmSticky(stickyArm);
		}

		public static bool WVR_SetInputRequest(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size)
		{
			return WVR_Base.Instance.SetInputRequest(type, request, size);
		}

		public static bool WVR_GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair)
		{
			return WVR_Base.Instance.GetInputMappingPair(type, destination, ref pair);
		}

		public static uint WVR_GetInputMappingTable(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size)
		{
			return WVR_Base.Instance.GetInputMappingTable(type, table, size);
		}

		public static WVR_Arena_t WVR_GetArena()
		{
			return WVR_Base.Instance.GetArena();
		}

		[Obsolete("This API is deprecated and is no longer supported.", true)]
		public static bool WVR_SetArena(ref WVR_Arena_t arena)
		{
			return WVR_Base.Instance.SetArena(ref arena);
		}

		public static WVR_ArenaVisible WVR_GetArenaVisible()
		{
			return WVR_Base.Instance.GetArenaVisible();
		}

		public static void WVR_SetArenaVisible(WVR_ArenaVisible config)
		{
			WVR_Base.Instance.SetArenaVisible(config);
		}

		public static bool WVR_IsOverArenaRange()
		{
			return WVR_Base.Instance.IsOverArenaRange();
		}

		public static float WVR_GetDeviceBatteryPercentage(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetDeviceBatteryPercentage(type);
		}

		public static WVR_BatteryStatus WVR_GetBatteryStatus(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetBatteryStatus(type);
		}

		public static WVR_ChargeStatus WVR_GetChargeStatus(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetChargeStatus(type);
		}

		public static WVR_BatteryTemperatureStatus WVR_GetBatteryTemperatureStatus(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetBatteryTemperatureStatus(type);
		}

		public static float WVR_GetBatteryTemperature(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetBatteryTemperature(type);
		}

		// ------------- wvr_eyetracking.h -------------
		/**
		 * @brief Function to start eye tracking.
		 * @retval true starting camera is success.
		 * @retval false starting camera fail.
		 * @version API Level
		 */
		public static WVR_Result WVR_StartEyeTracking()
		{
			return WVR_Base.Instance.StartEyeTracking();
		}

		/**
		 * @brief Function to stop eye tracking.
		 * @version API Level
		 */
		public static void WVR_StopEyeTracking()
		{
			WVR_Base.Instance.StopEyeTracking();
		}

		public static WVR_Result WVR_GetEyeTracking(ref WVR_EyeTracking_t data, WVR_CoordinateSystem space = WVR_CoordinateSystem.WVR_CoordinateSystem_Global)
		{
			return WVR_Base.Instance.GetEyeTracking(ref data, space);
		}
		#endregion

		#region Hand
		/**
		 * @brief Use this function to get hand gesture configuration.
		 *
		 * Use this API to get hand gesture configuration.
		 * This API must be called by main thread.
		 *
		 * @param info of @ref WVR_HandGestureType.
		 * @retval WVR_Success Get information successfully.
		 * @retval Others @ref WVR_Result mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
		{
			return WVR_Base.Instance.GetHandGestureInfo(ref info);
		}

		/**
		 * @brief Use this function to start hand gesture feature.
		 *
		 * Use this API to start hand gesture module
		 * This API must be called by main thread.
		 *
		 * @param demands Bitmask of @ref WVR_HandGestureType.
		 * @retval WVR_Success start hand gesture feature successfully.
		 * @retval others @ref WVR_Result mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_StartHandGesture(ulong demands = 3UL)
		{
			return WVR_Base.Instance.StartHandGesture(demands);
		}

		/**
		 * @brief Use this function to stop hand gesture that you already started.
		 *
		 * Use this API to stop hand gesture that you already started,
		 * and release related hand gesture source.
		 * This API must be called by main thread.
		 * @version API Level 6
		 */
		public static void WVR_StopHandGesture()
		{
			WVR_Base.Instance.StopHandGesture();
		}

		/**
		 * @brief Use this function to get gesture data.
		 *
		 * Use this API to get hand gesture state from the hand gesture module.
		 * Use this API must be called by main thread.
		 *
		 * @param data The hand gesture data refer to @ref WVR_HandGestureData.
		 * @retval WVR_Success Successfully retrieved data.
		 * @retval others @ref WVR_Result the mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_GetHandGestureData(ref WVR_HandGestureData_t data)
		{
			return WVR_Base.Instance.GetHandGestureData(ref data);
		}

		/**
		 * @brief Use this function to start hand tracker.
		 *
		 * Use this API to start hand tracker module.
		 * This API must be called by main thread.
		 * @param type The type of hand tracker. Refer to @ref WVR_HandTrackerType.
		 * If type is WVR_HandTrackerType_Natural, please check WVR_DeviceType_NaturalHand_Right or WVR_DeviceType_NaturalHand_Left is connected before starting.
		 * Otherwise, please check WVR_DeviceType_ElectronicHand_Right or WVR_DeviceType_ElectronicHand_Left is connected before starting.
		 * @retval WVR_Success Start hand tracker successfully.
		 * @retval Others @ref WVR_Result mean failure.
		 *
		 * @version API Level 6
		 */
		public static WVR_Result WVR_StartHandTracking(WVR_HandTrackerType type)
		{
			return WVR_Base.Instance.StartHandTracking(type);
		}

		/**
		 * @brief Use this function to stop the hand tracker that was started.
		 *
		 * Use this API to stop hand tracker that already started,
		 * and release related resources of th hand tracker.
		 * This API must be called by the main thread.
		 * @param type The type of hand tracker. Refer to @ref WVR_HandTrackerType.
		 * @version API Level 6
		 */
		public static void WVR_StopHandTracking(WVR_HandTrackerType type)
		{
			WVR_Base.Instance.StopHandTracking(type);
		}

		/**
		 * @brief Use this function to get the hand tracking device.
		 *
		 * @param trackerType Specify the type of hand tracker. Refer to @ref WVR_HandTrackerType.
		 * @param modelType Specify the type of hand model. Refer to @ref WVR_HandModelType.
		 * @param originModel Specify the tracking universe of the origin tracking model. Refer to @ref WVR_PoseOriginModel.
		 * @param skeleton The hand skeleton data @ref WVR_HandTrackingData_t
		 * @param pose The hand pose state refer to @ref WVR_HandPoseData_t
		 * @retval WVR_Success Successfully retrieved data.
		 * @retval Others @ref WVR_Result mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_GetHandTrackingData(WVR_HandTrackerType trackerType,
			WVR_HandModelType modelType,
			WVR_PoseOriginModel originModel,
			ref WVR_HandTrackingData_t handTrackerData,
			ref WVR_HandPoseData_t pose)
		{
			return WVR_Base.Instance.GetHandTrackingData(trackerType, modelType, originModel, ref handTrackerData, ref pose);
		}

		/**
		 * @brief Use this function to get the number of hand joints.
		 *
		 * @param type The type of hand tracker. Refer to @ref WVR_HandTrackerType.
		 * @param jointCount Specify the number of the hand joints that the hand tracker provides.
		 * @retval WVR_Success Get information successfully.
		 * @retval Others @ref WVR_Result mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount)
		{
			return WVR_Base.Instance.GetHandJointCount(type, ref jointCount);
		}

		/**
		 * @brief Use this function to get the capability of hand tracker device.
		 * @param type The type of hand tracker. Refer to @ref WVR_HandTrackerType.
		 * @param info The information of hand tracker device. Refer to @ref WVR_HandTrackerInfo.
		 * @retval WVR_Success Get information successfully.
		 * @retval Others @ref WVR_Result mean failure.
		 * @version API Level 6
		 */
		public static WVR_Result WVR_GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info)
		{
			return WVR_Base.Instance.GetHandTrackerInfo(type, ref info);
		}

		public static bool WVR_ControllerSupportElectronicHand()
		{
			return WVR_Base.Instance.ControllerSupportElectronicHand();
		}

		public static void WVR_EnhanceHandStable(bool wear = false)
		{
			WVR_Base.Instance.EnhanceHandStable(wear);
		}
		public static bool WVR_IsEnhanceHandStable()
		{
			return WVR_Base.Instance.IsEnhanceHandStable();
		}

		[Obsolete("WVR_SetMixMode is deprecated. Use adding \"wave.feature.mixmode\" to AndroidManifest.xml use-feature instead.")]
		public static void WVR_SetMixMode(bool enable)
		{
			WVR_Base.Instance.SetMixMode(enable);
		}
		#endregion

		#region Controller Pose Mode
		public static bool WVR_SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
		{
			return WVR_Base.Instance.SetControllerPoseMode(type, mode);
		}
		public static bool WVR_GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
		{
			return WVR_Base.Instance.GetControllerPoseMode(type, ref mode);
		}
		public static bool WVR_GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
		{
			return WVR_Base.Instance.GetControllerPoseModeOffset(type, mode, ref translation, ref quaternion);
		}
		#endregion

		#region Tracker
		public static WVR_Result WVR_StartTracker()
		{
			return WVR_Base.Instance.StartTracker();
		}
		public static void WVR_StopTracker()
		{
			WVR_Base.Instance.StopTracker();
		}

		public static bool WVR_IsTrackerConnected(WVR_TrackerId trackerId)
		{
			return WVR_Base.Instance.IsTrackerConnected(trackerId);
		}

		public static WVR_TrackerRole WVR_GetTrackerRole(WVR_TrackerId trackerId)
		{
			return WVR_Base.Instance.GetTrackerRole(trackerId);
		}

		public static WVR_Result WVR_GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities)
		{
			return WVR_Base.Instance.GetTrackerCapabilities(trackerId, ref capabilities);
		}

		public static WVR_Result WVR_GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState)
		{
			return WVR_Base.Instance.GetTrackerPoseState(trackerId, originModel, predictedMilliSec, ref poseState);
		}

		public static Int32 WVR_GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType)
		{
			return WVR_Base.Instance.GetTrackerInputDeviceCapability(trackerId, inputType);
		}

		public static WVR_AnalogType WVR_GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_Base.Instance.GetTrackerInputDeviceAnalogType(trackerId, id);
		}

		public static bool WVR_GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_Base.Instance.GetTrackerInputButtonState(trackerId, id);
		}
		public static bool WVR_GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_Base.Instance.GetTrackerInputTouchState(trackerId, id);
		}
		public static WVR_Axis_t WVR_GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id)
		{
			return WVR_Base.Instance.GetTrackerInputAnalogAxis(trackerId, id);
		}

		public static float WVR_GetTrackerBatteryLevel(WVR_TrackerId trackerId)
		{
			return WVR_Base.Instance.GetTrackerBatteryLevel(trackerId);
		}

		public static WVR_Result WVR_TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f)
		{
			return WVR_Base.Instance.TriggerTrackerVibration(trackerId, durationMicroSec, frequency, amplitude);
		}
		public static IntPtr WVR_GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp)
		{
			return WVR_Base.Instance.GetTrackerExtendedData(trackerId, ref exDataSize, ref timestamp);
		}
		[Obsolete("Please use new WVR_GetTrackerExtendedData() with timestamp.")]
		public static IntPtr WVR_GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize)
		{
			UInt64 timestamp = 0;
			return WVR_GetTrackerExtendedData(trackerId, ref exDataSize, ref timestamp);
		}
		public static WVR_Result WVR_GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, ref IntPtr deviceName)
		{
			return WVR_Base.Instance.GetTrackerDeviceName(trackerId, ref nameSize, ref deviceName);
		}

		public static WVR_Result WVR_RegisterTrackerInfoCallback(ref WVR_TrackerInfoNotify notify)
		{
			return WVR_Base.Instance.RegisterTrackerInfoCallback(ref notify);
		}
		public static WVR_Result WVR_UnregisterTrackerInfoCallback()
		{
			return WVR_Base.Instance.UnregisterTrackerInfoCallback();
		}

		public static void WVR_SetFocusedTracker(int focusedTracker)
		{
			WVR_Base.Instance.SetFocusedTracker(focusedTracker);
		}
		public static int WVR_GetFocusedTracker()
		{
			return WVR_Base.Instance.GetFocusedTracker();
		}
		#endregion

		#region wvr_notifydeviceinfo.h
		public static WVR_Result WVR_StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize)
		{
			return WVR_Base.Instance.StartNotifyDeviceInfo(type, unBufferSize);
		}
		public static void WVR_StopNotifyDeviceInfo(WVR_DeviceType type)
		{
			WVR_Base.Instance.StopNotifyDeviceInfo(type);
		}
		public static void WVR_UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue)
		{
			WVR_Base.Instance.UpdateNotifyDeviceInfo(type, dataValue);
		}
		#endregion

		#region Lip Expression
		public static WVR_Result WVR_StartLipExp()
		{
			return WVR_Base.Instance.StartLipExp();
		}
		public static WVR_Result WVR_GetLipExpData([In, Out] float[] value)
		{
			return WVR_Base.Instance.GetLipExpData(value);
		}
		public static void WVR_StopLipExp()
		{
			WVR_Base.Instance.StopLipExp();
		}
		#endregion

		#region Scene Perception

		public static WVR_Result WVR_StartScene()
		{
			return WVR_Base.Instance.StartScene();
		}

		public static void WVR_StopScene()
		{
			WVR_Base.Instance.StopScene();
		}

		public static WVR_Result WVR_StartScenePerception(WVR_ScenePerceptionTarget target)
		{
			return WVR_Base.Instance.StartScenePerception(target);
		}

		public static WVR_Result WVR_StopScenePerception(WVR_ScenePerceptionTarget target)
		{
			return WVR_Base.Instance.StopScenePerception(target);
		}

		public static WVR_Result WVR_GetScenePerceptionState(WVR_ScenePerceptionTarget target, ref WVR_ScenePerceptionState state /* WVR_ScenePerceptionState* */)
		{
			return WVR_Base.Instance.GetScenePerceptionState(target, ref state);
		}

		public static WVR_Result WVR_GetScenePlanes([In, Out] WVR_ScenePlaneFilter[] planeFilter /* WVR_ScenePlaneFilter*,nullptr if no need filter. */, UInt32 planeCapacityInput, out UInt32 planeCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr planes /* WVR_ScenePlane* */)
		{
			return WVR_Base.Instance.GetScenePlanes(planeFilter, planeCapacityInput, out planeCountOutput, originModel, planes);
		}

		public static WVR_Result WVR_GetSceneMeshes(WVR_SceneMeshType meshType, UInt32 meshCapacityInput, out UInt32 meshCountOutput /* uint32_t* */, IntPtr meshes /* WVR_SceneMesh* */)
		{
			return WVR_Base.Instance.GetSceneMeshes(meshType, meshCapacityInput, out meshCountOutput, meshes);
		}

		public static WVR_Result WVR_GetSceneMeshBuffer(UInt64 meshBufferId, WVR_PoseOriginModel originModel, ref WVR_SceneMeshBuffer sceneMeshBuffer /* WVR_SceneMeshBuffer* */)
		{
			return WVR_Base.Instance.GetSceneMeshBuffer(meshBufferId, originModel, ref sceneMeshBuffer);
		}

		public static WVR_Result WVR_GetSceneObjects(UInt32 objectCapacityInput, out UInt32 objectCountOutput, WVR_PoseOriginModel originModel, IntPtr objects /* WVR_SceneObject* */)
		{
			return WVR_Base.Instance.GetSceneObjects(objectCapacityInput, out objectCountOutput, originModel, objects);
		}


		public static WVR_Result WVR_CreateSpatialAnchor([In, Out] WVR_SpatialAnchorCreateInfo[] createInfo /* WVR_SpatialAnchorCreateInfo* */, out UInt64 anchor /* WVR_SpatialAnchor* */)
		{
			return WVR_Base.Instance.CreateSpatialAnchor(createInfo, out anchor);
		}

		public static WVR_Result WVR_DestroySpatialAnchor(UInt64 anchor /* WVR_SpatialAnchor */)
		{
			return WVR_Base.Instance.DestroySpatialAnchor(anchor);
		}

		public static WVR_Result WVR_EnumerateSpatialAnchors(UInt32 anchorCapacityInput, out UInt32 anchorCountOutput /* uint32_t* */, out UInt64 anchors /* WVR_SpatialAnchor* */)
		{
			return WVR_Base.Instance.EnumerateSpatialAnchors(anchorCapacityInput, out anchorCountOutput, out anchors);
		}

		public static WVR_Result WVR_GetSpatialAnchorState(UInt64 anchor /* WVR_SpatialAnchor */, WVR_PoseOriginModel originModel, out WVR_SpatialAnchorState anchorState /* WVR_SpatialAnchorState* */)
		{
			return WVR_Base.Instance.GetSpatialAnchorState(anchor, originModel, out anchorState);
		}

		public static WVR_Result WVR_CacheSpatialAnchor(ref WVR_SpatialAnchorCacheInfo spatialAnchorPersistInfo)
		{
			return WVR_Base.Instance.CacheSpatialAnchor(ref spatialAnchorPersistInfo);
		}

		public static WVR_Result WVR_UncacheSpatialAnchor(ref WVR_SpatialAnchorName cachedSpatialAnchorName)
		{
			return WVR_Base.Instance.UncacheSpatialAnchor(ref cachedSpatialAnchorName);
		}

		public static WVR_Result WVR_EnumerateCachedSpatialAnchorNames(
			UInt32 cachedSpatialAnchorNamesCapacityInput,
			out UInt32 cachedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] cachedSpatialAnchorNames)
		{
			return WVR_Base.Instance.EnumerateCachedSpatialAnchorNames(
				cachedSpatialAnchorNamesCapacityInput,
				out cachedSpatialAnchorNamesCountOutput,
				 cachedSpatialAnchorNames);
		}

		public static WVR_Result WVR_ClearCachedSpatialAnchors()
		{
			return WVR_Base.Instance.ClearCachedSpatialAnchors();
		}

		public static WVR_Result WVR_CreateSpatialAnchorFromCacheName(ref WVR_SpatialAnchorFromCacheNameCreateInfo createInfo, out UInt64 anchor /* WVR_SpatialAnchor */)
		{
			return WVR_Base.Instance.CreateSpatialAnchorFromCacheName(ref createInfo, out anchor);
		}

		public static WVR_Result WVR_PersistSpatialAnchor(ref WVR_SpatialAnchorPersistInfo spatialAnchorPersistInfo)
		{
			return WVR_Base.Instance.PersistSpatialAnchor(ref spatialAnchorPersistInfo);
		}

		public static WVR_Result WVR_UnpersistSpatialAnchor(ref WVR_SpatialAnchorName persistedSpatialAnchorName)
		{
			return WVR_Base.Instance.UnpersistSpatialAnchor(ref persistedSpatialAnchorName);
		}

		public static WVR_Result WVR_EnumeratePersistedSpatialAnchorNames(
			UInt32 persistedSpatialAnchorNamesCapacityInput,
			out UInt32 persistedSpatialAnchorNamesCountOutput,
			[Out] WVR_SpatialAnchorName[] persistedSpatialAnchorNames)
		{
			return WVR_Base.Instance.EnumeratePersistedSpatialAnchorNames(
				persistedSpatialAnchorNamesCapacityInput,
				out persistedSpatialAnchorNamesCountOutput,
				persistedSpatialAnchorNames);
		}

		public static WVR_Result WVR_ClearPersistedSpatialAnchors()
		{
			return WVR_Base.Instance.ClearPersistedSpatialAnchors();
		}

		public static WVR_Result WVR_GetPersistedSpatialAnchorCount(
			ref WVR_PersistedSpatialAnchorCountGetInfo getInfo)
		{
			return WVR_Base.Instance.GetPersistedSpatialAnchorCount(ref getInfo);

		}

		public static WVR_Result WVR_CreateSpatialAnchorFromPersistenceName(
			ref WVR_SpatialAnchorFromPersistenceNameCreateInfo createInfo,
			out UInt64 anchor /* WVR_SpatialAnchor* */)
		{
			return WVR_Base.Instance.CreateSpatialAnchorFromPersistenceName(ref createInfo, out anchor);
		}

		public static WVR_Result WVR_ExportPersistedSpatialAnchor(
			ref WVR_SpatialAnchorName persistedSpatialAnchorName,
			UInt32 dataCapacityInput,
			out UInt32 dataCountOutput,
			[Out] byte[] data)
		{
			return WVR_Base.Instance.ExportPersistedSpatialAnchor(
				ref persistedSpatialAnchorName, dataCapacityInput, out dataCountOutput, data);
		}

		public static WVR_Result WVR_ImportPersistedSpatialAnchor(
			UInt32 dataCount,
			[In] byte[] data)
		{
			return WVR_Base.Instance.ImportPersistedSpatialAnchor(dataCount, data);
		}

		#endregion

		#region Body Tracking
		public static UInt32 WVR_GetBodyTrackingStandardPoseSize()
		{
			return WVR_Base.Instance.GetBodyTrackingStandardPoseSize();
		}
		public static bool WVR_GetBodyTrackingStandardPoseInfo([In, Out] WVR_BodyTrackingType[] types, [In, Out] UInt32[] counts)
		{
			return WVR_Base.Instance.GetBodyTrackingStandardPoseInfo(types, counts);
		}
		public static bool WVR_GetBodyTrackingStandardPose(WVR_BodyTrackingType type, [In, Out] UInt32[] ids, [In, Out] WVR_Pose_t[] poses)
		{
			return WVR_Base.Instance.GetBodyTrackingStandardPose(type, ids, poses);
		}

		public static UInt32 WVR_GetBodyTrackingExtrinsicCount()
		{
			return WVR_Base.Instance.GetBodyTrackingExtrinsicCount();
		}
		public static bool WVR_GetBodyTrackingExtrinsics([In, Out] WVR_BodyTrackingExtrinsic[] extrinsics, ref UInt32 count)
		{
			return WVR_Base.Instance.GetBodyTrackingExtrinsics(extrinsics, ref count);
		}

		#region Trackable Marker

		public static WVR_Result WVR_StartMarker()
		{
			return WVR_Base.Instance.StartMarker();
		}

		public static void WVR_StopMarker()
		{
			WVR_Base.Instance.StopMarker();
		}

		public static WVR_Result WVR_StartMarkerObserver(WVR_MarkerObserverTarget target)
		{
			return WVR_Base.Instance.StartMarkerObserver(target);
		}

		public static WVR_Result WVR_StopMarkerObserver(WVR_MarkerObserverTarget target)
		{
			return WVR_Base.Instance.StopMarkerObserver(target);
		}

		public static WVR_Result WVR_GetMarkerObserverState(WVR_MarkerObserverTarget target, out WVR_MarkerObserverState state)
		{
			return WVR_Base.Instance.GetMarkerObserverState(target, out state);
		}

		public static WVR_Result WVR_StartMarkerDetection(WVR_MarkerObserverTarget target)
		{
			return WVR_Base.Instance.StartMarkerDetection(target);
		}

		public static WVR_Result WVR_StopMarkerDetection(WVR_MarkerObserverTarget target)
		{
			return WVR_Base.Instance.StopMarkerDetection(target);
		}

		public static WVR_Result WVR_GetArucoMarkers(UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr markers /* WVR_ArucoMarker* */)
		{
			return WVR_Base.Instance.GetArucoMarkers(markerCapacityInput, out markerCountOutput, originModel, markers);
		}

		public static WVR_Result WVR_EnumerateTrackableMarkers(WVR_MarkerObserverTarget target, UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, IntPtr markerIds /* WVR_Uuid* */)
		{
			return WVR_Base.Instance.EnumerateTrackableMarkers(target, markerCapacityInput, out markerCountOutput, markerIds);
		}

		public static WVR_Result WVR_CreateTrackableMarker([In, Out] WVR_TrackableMarkerCreateInfo[] createInfo /* WVR_TrackableMarkerCreateInfo* */)
		{
			return WVR_Base.Instance.CreateTrackableMarker(createInfo);
		}

		public static WVR_Result WVR_DestroyTrackableMarker(WVR_Uuid markerId)
		{
			return WVR_Base.Instance.DestroyTrackableMarker(markerId);
		}

		public static WVR_Result WVR_StartTrackableMarkerTracking(WVR_Uuid markerId)
		{
			return WVR_Base.Instance.StartTrackableMarkerTracking(markerId);
		}

		public static WVR_Result WVR_StopTrackableMarkerTracking(WVR_Uuid markerId)
		{
			return WVR_Base.Instance.StopTrackableMarkerTracking(markerId);
		}

		public static WVR_Result WVR_GetTrackableMarkerState(WVR_Uuid markerId, WVR_PoseOriginModel originModel, out WVR_TrackableMarkerState state /* WVR_TrackableMarkerState* */)
		{
			return WVR_Base.Instance.GetTrackableMarkerState(markerId, originModel, out state);
		}

		public static WVR_Result WVR_GetArucoMarkerData(WVR_Uuid markerId, out WVR_ArucoMarkerData data /* WVR_ArucoMarkerData* */)
		{
			return WVR_Base.Instance.GetArucoMarkerData(markerId, out data);
		}

		#endregion
		#endregion

		public static ulong WVR_GetSupportedFeatures()
		{
			return WVR_Base.Instance.GetSupportedFeatures();
		}

		public static WVR_InitError WVR_Init(WVR_AppType eType)
		{
			return WVR_Base.Instance.Init(eType);
		}

		public static void WVR_PostInit()
		{
			WVR_Base.Instance.PostInit();
		}

		public static void WVR_Quit()
		{
			WVR_Base.Instance.Quit();
		}

		public static IntPtr WVR_GetInitErrorString(WVR_InitError error)
		{
			return WVR_Base.Instance.GetInitErrorString(error);
		}

		public static uint WVR_GetWaveRuntimeVersion()
		{
			return WVR_Base.Instance.GetWaveRuntimeVersion();
		}

		public static uint WVR_GetWaveSDKVersion()
		{
			return WVR_Base.Instance.GetWaveSDKVersion();
		}

		public static bool WVR_IsInputFocusCapturedBySystem()
		{
			return WVR_Base.Instance.IsInputFocusCapturedBySystem();
		}

		internal static WVR_RenderError WVR_RenderInit(ref WVR_RenderInitParams_t param)
		{
			return WVR_Base.Instance.RenderInit(ref param);
		}

		public static bool WVR_SetPerformanceLevels(WVR_PerfLevel cpuLevel, WVR_PerfLevel gpuLevel)
		{
			return WVR_Base.Instance.SetPerformanceLevels(cpuLevel, gpuLevel);
		}

		public static bool WVR_EnableAdaptiveQuality(bool enable, uint flags)
		{
			return WVR_Base.Instance.EnableAdaptiveQuality(enable, flags);
		}

		public static bool WVR_IsAdaptiveQualityEnabled()
		{
			return WVR_Base.Instance.IsAdaptiveQualityEnabled();
		}

		public static bool WVR_StartCamera(ref WVR_CameraInfo_t info)
		{
			return WVR_Base.Instance.StartCamera(ref info);
		}

		public static void WVR_StopCamera()
		{
			WVR_Base.Instance.StopCamera();
		}

		public static bool WVR_UpdateTexture(IntPtr textureid)
		{
			return WVR_Base.Instance.UpdateTexture(textureid);
		}

		public static bool WVR_GetCameraIntrinsic(WVR_CameraPosition position, ref WVR_CameraIntrinsic_t intrinsic)
		{
			return WVR_Base.Instance.GetCameraIntrinsic(position, ref intrinsic);
		}

		public static bool WVR_GetCameraFrameBuffer(IntPtr pFramebuffer, uint frameBufferSize)
		{
			return WVR_Base.Instance.GetCameraFrameBuffer(pFramebuffer, frameBufferSize);
		}

		public static bool WVR_GetFrameBufferWithPoseState(IntPtr frameBuffer, uint frameBufferSize, WVR_PoseOriginModel origin, uint predictInMs, ref WVR_PoseState_t poseState)
		{
			return WVR_Base.Instance.GetFrameBufferWithPoseState(frameBuffer, frameBufferSize, origin, predictInMs, ref poseState);
		}

		public static bool WVR_DrawTextureWithBuffer(IntPtr textureId, WVR_CameraImageFormat imgFormat, IntPtr frameBuffer, uint size, uint width, uint height, bool enableCropping, bool clearClampRegion)
		{
			return WVR_Base.Instance.DrawTextureWithBuffer(textureId, imgFormat, frameBuffer, size, width, height, enableCropping, clearClampRegion);
		}

		public static void WVR_ReleaseCameraTexture()
		{
			WVR_Base.Instance.ReleaseCameraTexture();
		}

		public static bool WVR_IsDeviceSuspend(WVR_DeviceType type)
		{
			return WVR_Base.Instance.IsDeviceSuspend(type);
		}

		public static void WVR_ConvertMatrixQuaternion(ref WVR_Matrix4f_t mat, ref WVR_Quatf_t quat, bool m2q)
		{
			WVR_Base.Instance.ConvertMatrixQuaternion(ref mat, ref quat, m2q);
		}

		public static WVR_NumDoF WVR_GetDegreeOfFreedom(WVR_DeviceType type)
		{
			return WVR_Base.Instance.GetDegreeOfFreedom(type);
		}

		public static void WVR_SetParameters(WVR_DeviceType type, IntPtr pchValue)
		{
			WVR_Base.Instance.SetParameters(type, pchValue);
		}

		public static uint WVR_GetParameters(WVR_DeviceType type, IntPtr pchValue, IntPtr retValue, uint unBufferSize)
		{
			return WVR_Base.Instance.GetParameters(type, pchValue, retValue, unBufferSize);
		}

		public static WVR_DeviceType WVR_GetDefaultControllerRole()
		{
			return WVR_Base.Instance.GetDefaultControllerRole();
		}
		[System.Obsolete("This is an obsolete function.", true)]
		public static bool WVR_SetInteractionMode(WVR_InteractionMode mode)
		{
			return WVR_Base.Instance.SetInteractionMode(mode);
		}

		public static WVR_InteractionMode WVR_GetInteractionMode()
		{
			return WVR_Base.Instance.GetInteractionMode();
		}

		public static bool WVR_SetGazeTriggerType(WVR_GazeTriggerType type)
		{
			return WVR_Base.Instance.SetGazeTriggerType(type);
		}

		public static WVR_GazeTriggerType WVR_GetGazeTriggerType()
		{
			return WVR_Base.Instance.GetGazeTriggerType();
		}

		public static bool WVR_GetDeviceErrorState(WVR_DeviceType dev_type, WVR_DeviceErrorStatus error_type)
		{
			return WVR_Base.Instance.GetDeviceErrorState(dev_type, error_type);
		}

		public static void WVR_GetRenderTargetSize(ref uint width, ref uint height)
		{
			WVR_Base.Instance.GetRenderTargetSize(ref width, ref height);
		}

		public static WVR_Matrix4f_t WVR_GetProjection(WVR_Eye eye, float near, float far)
		{
			return WVR_Base.Instance.GetProjection(eye, near, far);
		}

		public static void WVR_GetClippingPlaneBoundary(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom)
		{
			WVR_Base.Instance.GetClippingPlaneBoundary(eye, ref left, ref right, ref top, ref bottom);
		}

		public static void WVR_SetOverfillRatio(float ratioX, float ratioY)
		{
			WVR_Base.Instance.SetOverfillRatio(ratioX, ratioY);
		}

		public static WVR_Matrix4f_t WVR_GetTransformFromEyeToHead(WVR_Eye eye, WVR_NumDoF dof)
		{
			return WVR_Base.Instance.GetTransformFromEyeToHead(eye, dof);
		}

		public static WVR_SubmitError WVR_SubmitCompositionLayer([In, Out] WVR_LayerSetParams_t[] param)
		{
			return WVR_Base.Instance.SubmitCompositionLayers(param);
		}

		public static uint WVR_GetMaxCompositionLayerCount()
		{
			return WVR_Base.Instance.GetMaxCompositionLayerCount();
		}

		public static IntPtr WVR_CreateAndroidSurface(int width, int height, bool isProtected, [In, Out] WVR_TextureParams_t[] param)
		{
			return WVR_Base.Instance.CreateAndroidSurface(width, height, isProtected, param);
		}

		public static void WVR_DeleteAndroidSurface()
		{
			WVR_Base.Instance.DeleteAndroidSurface();
		}


		public static WVR_SubmitError WVR_SubmitFrame(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod)
		{
			return WVR_Base.Instance.SubmitFrame(eye, param, pose, extendMethod);
		}

		public static void WVR_SetSubmitParams(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod)
		{
			WVR_Base.Instance.SetSubmitParams(eye, param, pose, extendMethod);
		}

		public static void WVR_PreRenderEye(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			WVR_Base.Instance.PreRenderEye(eye, param, foveationParams);
		}

		public static bool WVR_RequestScreenshot(uint width, uint height, WVR_ScreenshotMode mode, IntPtr filename)
		{
			return WVR_Base.Instance.RequestScreenshot(width, height, mode, filename);
		}

		public static void WVR_RenderMask(WVR_Eye eye)
		{
			WVR_Base.Instance.RenderMask(eye);
		}

		public static bool WVR_GetRenderProps(ref WVR_RenderProps_t props)
		{
			return WVR_Base.Instance.GetRenderProps(ref props);
		}

		public static IntPtr WVR_ObtainTextureQueue(WVR_TextureTarget target, WVR_TextureFormat format, WVR_TextureType type, uint width, uint height, int level)
		{
			return WVR_Base.Instance.ObtainTextureQueue(target, format, type, width, height, level);
		}

		public static uint WVR_GetTextureQueueLength(IntPtr handle)
		{
			return WVR_Base.Instance.GetTextureQueueLength(handle);
		}

		public static WVR_TextureParams_t WVR_GetTexture(IntPtr handle, int index)
		{
			return WVR_Base.Instance.GetTexture(handle, index);
		}

		public static int WVR_GetAvailableTextureIndex(IntPtr handle)
		{
			return WVR_Base.Instance.GetAvailableTextureIndex(handle);
		}

		public static void WVR_ReleaseTextureQueue(IntPtr handle)
		{
			WVR_Base.Instance.ReleaseTextureQueue(handle);
		}

		public static bool WVR_IsRenderFoveationSupport()
		{
			return WVR_Base.Instance.IsRenderFoveationSupport();
		}

		public static void WVR_RenderFoveation(bool enable)
		{
			WVR_Base.Instance.RenderFoveation(enable);
		}

		public static WVR_Result WVR_RenderFoveationMode(WVR_FoveationMode mode)
		{
			return WVR_Base.Instance.RenderFoveationMode(mode);
		}

		public static WVR_Result WVR_SetFoveationConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			return WVR_Base.Instance.SetFoveationConfig(eye, foveationParams);
		}

		public static WVR_Result WVR_GetFoveationDefaultConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
		{
			return WVR_Base.Instance.GetFoveationDefaultConfig(eye, foveationParams);
		}

		public static bool WVR_IsRenderFoveationEnabled()
		{
			return WVR_Base.Instance.IsRenderFoveationEnabled();
		}

		public static bool WVR_IsRenderFoveationDefaultOn()
		{
			return WVR_Base.Instance.IsRenderFoveationDefaultOn();
		}

		public static bool WVR_IsPermissionInitialed()
		{
			return WVR_Base.Instance.IsPermissionInitialed();
		}

		public static bool WVR_ShowDialogOnScene()
		{
			return WVR_Base.Instance.ShowDialogOnScene();
		}

		public static bool WVR_IsPermissionGranted(string permission)
		{
			return WVR_Base.Instance.IsPermissionGranted(permission);
		}

		public static bool WVR_ShouldGrantPermission(string permission)
		{
			return WVR_Base.Instance.ShouldGrantPermission(permission);
		}

		public static void WVR_RequestPermissions(string[] permissions, WVR_RequestCompleteCallback cb)
		{
			WVR_Base.Instance.RequestPermissions(permissions, cb);
		}

		public static void WVR_RequestUsbPermission(WVR_RequestUsbCompleteCallback cb)
		{
			WVR_Base.Instance.RequestUsbPermission(cb);
		}



		public static string WVR_GetStringBySystemLanguage(string stringName)
		{
			return WVR_Base.Instance.GetStringBySystemLanguage(stringName);
		}

		public static string WVR_GetStringByLanguage(string stringName, string lang, string country)
		{
			return WVR_Base.Instance.GetStringByLanguage(stringName, lang, country);
		}

		public static string WVR_GetSystemLanguage()
		{
			return WVR_Base.Instance.GetSystemLanguage();
		}

		public static string WVR_GetSystemCountry()
		{
			return WVR_Base.Instance.GetSystemCountry();
		}

		public static void WVR_SetPosePredictEnabled(WVR_DeviceType type, bool enabled_position_predict, bool enable_rotation_predict)
		{
			WVR_Base.Instance.SetPosePredictEnabled(type, enabled_position_predict, enable_rotation_predict);
		}

		public static bool WVR_ShowPassthroughOverlay(bool show, bool delaySubmit = false, bool showIndicator = false)
		{
			return WVR_Base.Instance.ShowPassthroughOverlay(show, delaySubmit, showIndicator);
		}

		public static WVR_Result WVR_SetPassthroughOverlayAlpha(float alpha)
		{
			return WVR_Base.Instance.SetPassthroughOverlayAlpha(alpha);
		}


		public static WVR_Result WVR_ShowPassthroughUnderlay(bool show)
		{
			return WVR_Base.Instance.ShowPassthroughUnderlay(show);
		}

		public static void WVR_EnableAutoPassthrough(bool enable)
		{
			WVR_Base.Instance.EnableAutoPassthrough(enable);
		}

		public static bool WVR_IsPassthroughOverlayVisible()
		{
			return WVR_Base.Instance.IsPassthroughOverlayVisible();
		}

		public static WVR_Result WVR_SetProjectedPassthroughPose(ref WVR_Pose_t pose)
		{
			return WVR_Base.Instance.SetProjectedPassthroughPose(ref pose);
		}

		public static WVR_Result WVR_SetProjectedPassthroughMesh(float[] vertexBuffer, uint vertextCount, uint[] indices, uint indexCount)
		{
			return WVR_Base.Instance.SetProjectedPassthroughMesh(vertexBuffer, vertextCount, indices, indexCount);
		}

		public static WVR_Result WVR_SetProjectedPassthroughAlpha(float alpha)
		{
			return WVR_Base.Instance.SetProjectedPassthroughAlpha(alpha);
		}

		public static WVR_Result WVR_ShowProjectedPassthrough(bool show)
		{
			return WVR_Base.Instance.ShowProjectedPassthrough(show);
		}

		/**
		 * @brief Lets the developer know if WVR_DeviceType keeps static on table
		 *
		 * @param type Indicates what device type. (refer to @ref WVR_DeviceType)
		 * @return True means device keeps static; false means device is moving.
		 * @version API Level 8
		 */
		public static bool WVR_IsDeviceTableStatic(WVR_DeviceType type)
		{
			return WVR_Base.Instance.IsDeviceTableStatic(type);
		}

		public static void WVR_GetSpectatorRenderTargetSize(ref uint width, ref uint height)
		{
			WVR_Base.Instance.GetSpectatorRenderTargetSize(ref width, ref height);
		}

		public static void WVR_GetSpectatorClippingPlaneBoundary(ref float l, ref float r, ref float t, ref float b)
		{
			WVR_Base.Instance.GetSpectatorClippingPlaneBoundary(ref l, ref r, ref t, ref b);
		}

		public static bool WVR_PreSpectatorRender(ref WVR_SpectatorState state)
		{
			return WVR_Base.Instance.PreSpectatorRender(ref state);
		}

		public static bool WVR_SetChecker(bool enable)
		{
			return WVR_Base.Instance.SetChecker(enable);
		}

		public static WVR_Result WVR_GetAvailableFrameRates(out uint[] frameRates)
		{
			return WVR_Base.Instance.WVR_GetAvailableFrameRates(out frameRates);
		}

		public static WVR_Result WVR_GetFrameRate(ref uint frameRate)
		{
			return WVR_Base.Instance.WVR_GetFrameRate(ref frameRate);
		}

		public static WVR_Result WVR_SetFrameRate(uint frameRate)
		{
			return WVR_Base.Instance.WVR_SetFrameRate(frameRate);
		}

		public static bool WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality quality)
		{
			return WVR_Base.Instance.WVR_SetPassthroughImageQuality(quality);
		}

		public static bool WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus focus)
		{
			return WVR_Base.Instance.WVR_SetPassthroughImageFocus(focus);
		}

		/**
		 * Function to set passthrough image's refresh rate.
		 * The API adjusts the refresh rate of passthourgh image to the WaveVR Runtime,
		 * Affects the WVR_ShowPassthroughOverlay(), WVR_ShowPassthroughUnderlay and WVR_ShowProjectedPassthrough
		 */
		public static WVR_Result WVR_SetPassthroughImageRate(WVR_PassthroughImageRate rate)
		{
			return WVR_Base.Instance.SetPassthroughImageRate(rate);
		}


        public static void WVR_EnableHandleDisplayChanged(bool enable)
        {
            WVR_Base.Instance.EnableHandleDisplayChanged(enable);
        }

        #region Internal
        public static string WVR_DeployRenderModelAssets(int deviceIndex, string renderModelName)
		{
			return WVR_Base.Instance.DeployRenderModelAssets(deviceIndex, renderModelName);
		}

		public static void WVR_SetFocusedController(WVR_DeviceType focusController)
		{
			WVR_Base.Instance.SetFocusedController(focusController);
		}

		public static WVR_DeviceType WVR_GetFocusedController()
		{
			return WVR_Base.Instance.GetFocusedController();
		}

		public static float WVR_GetResolutionMaxScale()
		{
			return WVR_Base.Instance.GetResolutionMaxScale();
		}

		public static bool WVR_OpenMesh(string filename, ref uint sessionid, IntPtr errorCode, bool merge)
		{
			return WVR_Base.Instance.OpenMesh(filename, ref sessionid, errorCode, merge);
		}

		public static bool WVR_GetSectionCount(uint sessionid, ref uint sectionCount)
		{
			return WVR_Base.Instance.GetSectionCount(sessionid, ref sectionCount);
		}

		public static bool WVR_GetMeshData(uint sessionid, [In, Out] FBXInfo_t[] infoArray)
		{
			return WVR_Base.Instance.GetMeshData(sessionid, infoArray);
		}

		public static bool WVR_GetSectionData(uint sessionid, uint sectionIndiceIndex, [In, Out] Vector3[] vecticeArray, [In, Out] Vector3[] normalArray, [In, Out] Vector2[] uvArray, [In, Out] int[] indiceArray, ref bool active)
		{
			return WVR_Base.Instance.GetSectionData(sessionid, sectionIndiceIndex, vecticeArray, normalArray, uvArray, indiceArray, ref active);
		}

		public static void WVR_ReleaseMesh(uint sessiionid)
		{
			WVR_Base.Instance.ReleaseMesh(sessiionid);
		}

		public static string WVR_GetOEMConfigByKey(string key)
		{
			return WVR_Base.Instance.GetOEMConfigByKey(key);
		}

		public static void WVR_SetOEMConfigChangedCallback(WVR_OnOEMConfigChanged cb)
		{
			WVR_Base.Instance.SetOEMConfigChangedCallback(cb);
		}

		public static WVR_Result WVR_GetCurrentControllerModel(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone)
		{
			return WVR_Base.Instance.GetCurrentControllerModel(type, ref ctrlerModel, isOneBone);
		}

		public static void WVR_ReleaseControllerModel(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */)
		{
			WVR_Base.Instance.ReleaseControllerModel(ref ctrlerModel);
		}

		public static WVR_Result WVR_GetCurrentNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
		{
			return WVR_Base.Instance.GetCurrentNaturalHandModel(ref handModel);
		}

		public static void WVR_ReleaseNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
		{
			WVR_Base.Instance.ReleaseNaturalHandModel(ref handModel);
		}

		public static WVR_Result WVR_GetCtrlerModelAnimNodeData(WVR_DeviceType type, ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
		{
			return WVR_Base.Instance.GetCtrlerModelAnimNodeData(type, ref ctrlModelAnimData);
		}

		public static void WVR_ReleaseCtrlerModelAnimNodeData(ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
		{
			WVR_Base.Instance.ReleaseCtrlerModelAnimNodeData(ref ctrlModelAnimData);
		}

		public static void WVR_RenderFoveationMode()
        {
            throw new NotImplementedException();
        }

        public static WVR_Result WVR_SetAMCMode(WVR_AMCMode mode)
        {
            return WVR_Base.Instance.SetAMCMode(mode);
        }

        public static WVR_AMCMode WVR_GetAMCMode()
        {
            return WVR_Base.Instance.GetAMCMode();
        }

        public static WVR_AMCState WVR_GetAMCState()
        {
            return WVR_Base.Instance.GetAMCState();
        }

		public static WVR_Result WVR_SetFrameSharpnessEnhancementLevel(float level)
		{
			return WVR_Base.Instance.SetFrameSharpnessEnhancementLevel(level);
		}

		#endregion
		public class WVR_Base
		{
			private static WVR_Base instance = null;
			public static WVR_Base Instance
			{
				get
				{
					if (instance == null)
					{
#if !UNITY_EDITOR && UNITY_ANDROID
						instance = new WVR_Android();
#elif UNITY_EDITOR && UNITY_ANDROID
						bool EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);

						if (EnableDirectPreview)
						{
							Debug.Log("Initialize DirectPreview instance");
							instance = new WVR_DirectPreview();
						}
						else
						{
							Debug.Log("Initialize pure editor instance");
							instance = new WVR_Editor();
						}
#else
						instance = new WVR_Base();
#endif
					}
					return instance;
				}
			}

			#region Interaction
			// ------------- wvr_events.h -------------
			// Events: swipe, battery status.
			public virtual bool PollEventQueue(ref WVR_Event_t e)
			{
				return false;
			}

			// ------------- wvr_device.h -------------
			// Button types for which device is capable.
			public virtual int GetInputDeviceCapability(WVR_DeviceType type, WVR_InputType inputType)
			{
				return 0;
			}

			// Get analog type for which device.
			public virtual WVR_AnalogType GetInputDeviceAnalogType(WVR_DeviceType type, WVR_InputId id)
			{
				return WVR_AnalogType.WVR_AnalogType_None;
			}

			// Button press and touch state.
			public virtual bool GetInputDeviceState(WVR_DeviceType type, uint inputMask, ref uint buttons, ref uint touches,
				[In, Out] WVR_AnalogState_t[] analogArray, uint analogArrayCount)
			{
				return false;
			}

			// Count of specified button type.
			public virtual int GetInputTypeCount(WVR_DeviceType type, WVR_InputType inputType)
			{
				return 0;
			}

			// Button press state.
			public virtual bool GetInputButtonState(WVR_DeviceType type, WVR_InputId id)
			{
				return false;
			}

			// Button touch state.
			public virtual bool GetInputTouchState(WVR_DeviceType type, WVR_InputId id)
			{
				return false;
			}

			// Axis of analog button: touchpad (x, y), trigger (x only)
			public virtual WVR_Axis_t GetInputAnalogAxis(WVR_DeviceType type, WVR_InputId id)
			{
				WVR_Axis_t _T = new WVR_Axis_t();
				_T.x = 0.0f;
				_T.y = 0.0f;
				return _T;
			}

			public virtual void SetTextureBounds([In, Out] WVR_TextureBound_t[] textureBounds)
			{

			}

			// Get transform of specified device.
			public virtual void GetPoseState(WVR_DeviceType type, WVR_PoseOriginModel originModel, uint predictedMilliSec, ref WVR_PoseState_t poseState)
			{
			}
			public virtual void GetLastPoseIndex(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount, ref uint frameIndex)
			{

			}
			public virtual void WaitGetPoseIndex(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount, ref uint frameIndex)
			{

			}
			public virtual System.IntPtr StoreRenderTextures(System.IntPtr[] texturesIDs, int size, bool eEye, WVR_TextureTarget target)
			{
				return System.IntPtr.Zero;
			}

			// Get all attributes of pose of all devices.
			public virtual void GetSyncPose(WVR_PoseOriginModel originModel, [In, Out] WVR_DevicePosePair_t[] poseArray, uint pairArrayCount)
			{
			}

			// Device connection state.
			public virtual bool IsDeviceConnected(WVR_DeviceType type)
			{
				return false;
			}

			// Make device vibration.
			public virtual void TriggerVibration(WVR_DeviceType type, WVR_InputId id, uint durationMicroSec, uint frequency, WVR_Intensity intensity)
			{
			}

			public virtual void TriggerVibrationScale(WVR_DeviceType type, WVR_InputId inputId, uint durationMicroSec, uint frequency, float amplitude)
			{
			}

			// Recenter the "Virtual World" in current App.
			public virtual void InAppRecenter(WVR_RecenterType recenterType)
			{
			}

			// Enables or disables screen protection
			public virtual void SetScreenProtection(bool enabled)
			{
			}

			// Enables or disables use of the neck model for 3-DOF head tracking
			public virtual void SetNeckModelEnabled(bool enabled)
			{
			}

			// Decide Neck Model on/off/3dofOn
			public virtual void SetNeckModel(WVR_SimulationType simulationType)
			{
			}

			// Decide Arm Model on/off/3dofOn
			public virtual void SetArmModel(WVR_SimulationType simulationType)
			{
			}

			// Decide Arm Model behaviors
			public virtual void SetArmSticky(bool stickyArm)
			{
			}

			// bool WVR_SetInputRequest(WVR_DeviceType type, const WVR_InputAttribute* request, uint32_t size);
			public virtual bool SetInputRequest(WVR_DeviceType type, WVR_InputAttribute_t[] request, uint size)
			{
				return false;
			}

			// bool WVR_GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, WVR_InputMappingPair* pair);
			public virtual bool GetInputMappingPair(WVR_DeviceType type, WVR_InputId destination, ref WVR_InputMappingPair_t pair)
			{
				return false;
			}

			// uint32_t WVR_GetInputMappingTable(WVR_DeviceType type, WVR_InputMappingPair* table, uint32_t size);
			public virtual uint GetInputMappingTable(WVR_DeviceType type, [In, Out] WVR_InputMappingPair_t[] table, uint size)
			{
				return 0;
			}

			// ------------- wvr_arena.h -------------
			// Get current attributes of arena.
			public virtual WVR_Arena_t GetArena()
			{
				WVR_Arena_t _T = new WVR_Arena_t();
				return _T;
			}

			// Set up arena.
			[Obsolete("This API is deprecated and is no longer supported.", true)]
			public virtual bool SetArena(ref WVR_Arena_t arena)
			{
				return false;
			}

			// Get visibility type of arena.
			public virtual WVR_ArenaVisible GetArenaVisible()
			{
				return WVR_ArenaVisible.WVR_ArenaVisible_Auto;
			}

			// Set visibility type of arena.
			public virtual void SetArenaVisible(WVR_ArenaVisible config)
			{
			}

			// Check if player is over range of arena.
			public virtual bool IsOverArenaRange()
			{
				return false;
			}

			// ------------- wvr_status.h -------------
			// Battery electricity (%).
			public virtual float GetDeviceBatteryPercentage(WVR_DeviceType type)
			{
				return 1.0f;
			}

			// Battery life status.
			public virtual WVR_BatteryStatus GetBatteryStatus(WVR_DeviceType type)
			{
				return WVR_BatteryStatus.WVR_BatteryStatus_Normal;
			}

			// Battery is charging or not.
			public virtual WVR_ChargeStatus GetChargeStatus(WVR_DeviceType type)
			{
				return WVR_ChargeStatus.WVR_ChargeStatus_Full;
			}

			// Whether battery is overheat.
			public virtual WVR_BatteryTemperatureStatus GetBatteryTemperatureStatus(WVR_DeviceType type)
			{
				return WVR_BatteryTemperatureStatus.WVR_BatteryTemperature_Normal;
			}

			// Battery temperature.
			public virtual float GetBatteryTemperature(WVR_DeviceType type)
			{
				return 0.0f;
			}

			// ------------- wvr_eyetracking.h -------------
			/**
			 * @brief Function to start eye tracking.
			 * @retval true starting camera is success.
			 * @retval false starting camera fail.
			 * @version API Level
			 */
			public virtual WVR_Result StartEyeTracking()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			/**
			 * @brief Function to stop eye tracking.
			 * @version API Level
			 */
			public virtual void StopEyeTracking()
			{
			}

			/**
			 * @brief Function to eye tracking data.
			 *
			 * This API is used to get eye tracking data from the eye tracker module
			 * This API must be called by main thread.
			 *
			 * @param data the eye tracking data @ref WVR_EyeTracking
			 * @retval WVR_Success get data successfully.
			 * @retval others @ref WVR_Result mean failure.
			 * @version API Level 6
			 */
			public virtual WVR_Result GetEyeTracking(ref WVR_EyeTracking_t data, WVR_CoordinateSystem space)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			#endregion

			#region Hand
			public virtual WVR_Result GetHandGestureInfo(ref WVR_HandGestureInfo_t info)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual WVR_Result StartHandGesture(ulong demands)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual void StopHandGesture()
			{
			}

			public virtual WVR_Result GetHandGestureData(ref WVR_HandGestureData_t data)
			{
				data.timestamp = 0;
				data.right = WVR_HandGestureType.WVR_HandGestureType_Invalid;
				data.left = WVR_HandGestureType.WVR_HandGestureType_Invalid;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StartHandTracking(WVR_HandTrackerType type)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual void StopHandTracking(WVR_HandTrackerType type)
			{
			}

			public virtual WVR_Result GetHandJointCount(WVR_HandTrackerType type, ref uint jointCount)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetHandTrackerInfo(WVR_HandTrackerType type, ref WVR_HandTrackerInfo_t info)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetHandTrackingData(
				WVR_HandTrackerType trackerType,
				WVR_HandModelType modelType,
				WVR_PoseOriginModel originModel,
				ref WVR_HandTrackingData_t handTrackerData,
				ref WVR_HandPoseData_t pose)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual bool ControllerSupportElectronicHand() { return false; }

			public virtual void EnhanceHandStable(bool wear) {}
			public virtual bool IsEnhanceHandStable() { return false; }

			public virtual void SetMixMode(bool enable) { }
			#endregion

			#region Controller Pose Mode
			public virtual bool GetControllerPoseModeOffset(WVR_DeviceType type, WVR_ControllerPoseMode mode, ref WVR_Vector3f_t translation, ref WVR_Quatf_t quaternion)
			{
				return false;
			}
			public virtual bool SetControllerPoseMode(WVR_DeviceType type, WVR_ControllerPoseMode mode)
			{
				return false;
			}
			public virtual bool GetControllerPoseMode(WVR_DeviceType type, ref WVR_ControllerPoseMode mode)
			{
				return false;
			}
			#endregion

			#region Tracker
			public virtual WVR_Result StartTracker()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual void StopTracker()
			{
			}
			public virtual bool IsTrackerConnected(WVR_TrackerId trackerId)
			{
				return false;
			}
			public virtual WVR_TrackerRole GetTrackerRole(WVR_TrackerId trackerId)
			{
				return WVR_TrackerRole.WVR_TrackerRole_Undefined;
			}
			public virtual WVR_Result GetTrackerCapabilities(WVR_TrackerId trackerId, ref WVR_TrackerCapabilities capabilities)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual WVR_Result GetTrackerPoseState(WVR_TrackerId trackerId, WVR_PoseOriginModel originModel, UInt32 predictedMilliSec, ref WVR_PoseState_t poseState)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual Int32 GetTrackerInputDeviceCapability(WVR_TrackerId trackerId, WVR_InputType inputType)
			{
				return -1;
			}
			public virtual WVR_AnalogType GetTrackerInputDeviceAnalogType(WVR_TrackerId trackerId, WVR_InputId id)
			{
				return WVR_AnalogType.WVR_AnalogType_None;
			}
			public virtual bool GetTrackerInputButtonState(WVR_TrackerId trackerId, WVR_InputId id)
			{
				return false;
			}
			public virtual bool GetTrackerInputTouchState(WVR_TrackerId trackerId, WVR_InputId id)
			{
				return false;
			}
			public virtual WVR_Axis_t GetTrackerInputAnalogAxis(WVR_TrackerId trackerId, WVR_InputId id)
			{
				WVR_Axis_t axis2d;
				axis2d.x = 0;
				axis2d.y = 0;

				return axis2d;
			}
			public virtual float GetTrackerBatteryLevel(WVR_TrackerId trackerId)
			{
				return 0;
			}
			public virtual WVR_Result TriggerTrackerVibration(WVR_TrackerId trackerId, UInt32 durationMicroSec = 65535, UInt32 frequency = 0, float amplitude = 0.0f)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual IntPtr GetTrackerExtendedData(WVR_TrackerId trackerId, ref Int32 exDataSize, ref UInt64 timestamp)
			{
				return IntPtr.Zero;
			}
			public virtual WVR_Result GetTrackerDeviceName(WVR_TrackerId trackerId, ref UInt32 nameSize, ref IntPtr deviceName)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual WVR_Result RegisterTrackerInfoCallback(ref WVR_TrackerInfoNotify notify)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual WVR_Result UnregisterTrackerInfoCallback()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual void SetFocusedTracker(int focusedTracker) { }
			public virtual int GetFocusedTracker() { return -1; }
			#endregion

			#region wvr_notifydeviceinfo.h
			public virtual WVR_Result StartNotifyDeviceInfo(WVR_DeviceType type, UInt32 unBufferSize)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual void StopNotifyDeviceInfo(WVR_DeviceType type)
			{
			}
			public virtual void UpdateNotifyDeviceInfo(WVR_DeviceType type, IntPtr dataValue)
			{
			}
			#endregion

			#region Lip Expression
			public virtual WVR_Result StartLipExp()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual WVR_Result GetLipExpData([In, Out] float[] value)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}
			public virtual void StopLipExp()
			{
			}
			#endregion

			#region Scene Perception

			public virtual WVR_Result StartScene()
			{
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual void StopScene()
			{
				return;
			}

			public virtual WVR_Result StartScenePerception(WVR_ScenePerceptionTarget target)
			{
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result StopScenePerception(WVR_ScenePerceptionTarget target)
			{
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result GetScenePerceptionState( WVR_ScenePerceptionTarget target, ref WVR_ScenePerceptionState state /* WVR_ScenePerceptionState* */)
			{
#if UNITY_EDITOR
				state = WVR_ScenePerceptionState.WVR_ScenePerceptionState_Completed;
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result GetScenePlanes([In, Out] WVR_ScenePlaneFilter[] planeFilter /* WVR_ScenePlaneFilter*,nullptr if no need filter. */, UInt32 planeCapacityInput, out UInt32 planeCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr planes /* WVR_ScenePlane* */)
			{
				planeCountOutput = 0;
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result GetSceneMeshes(WVR_SceneMeshType meshType, UInt32 meshCapacityInput, out UInt32 meshCountOutput /* uint32_t* */, IntPtr meshes /* WVR_SceneMesh* */)
			{
				meshCountOutput = 0;
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result GetSceneMeshBuffer(UInt64 meshBufferId, WVR_PoseOriginModel originModel, ref WVR_SceneMeshBuffer sceneMeshBuffer /* WVR_SceneMeshBuffer* */)
			{
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result GetSceneObjects(UInt32 objectCapacityInput, out UInt32 objectCountOutput, WVR_PoseOriginModel originModel, IntPtr objects /* WVR_SceneObject* */)
			{
				objectCountOutput = 0;
#if UNITY_EDITOR
				return WVR_Result.WVR_Success;
#else
				return WVR_Result.WVR_Error_FeatureNotSupport;
#endif
			}

			public virtual WVR_Result CreateSpatialAnchor([In, Out] WVR_SpatialAnchorCreateInfo[] createInfo /* WVR_SpatialAnchorCreateInfo* */, out UInt64 anchor /* WVR_SpatialAnchor* */)
			{
				anchor = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result DestroySpatialAnchor(UInt64 anchor /* WVR_SpatialAnchor */)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result EnumerateSpatialAnchors(UInt32 anchorCapacityInput, out UInt32 anchorCountOutput /* uint32_t* */, out UInt64 anchors /* WVR_SpatialAnchor* */)
			{
				anchorCountOutput = 0;
				anchors = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetSpatialAnchorState(UInt64 anchor /* WVR_SpatialAnchor */, WVR_PoseOriginModel originModel, out WVR_SpatialAnchorState anchorState /* WVR_SpatialAnchorState* */)
			{
				anchorState = default(WVR_SpatialAnchorState);
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result CacheSpatialAnchor(ref WVR_SpatialAnchorCacheInfo spatialAnchorPersistInfo)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result UncacheSpatialAnchor(ref WVR_SpatialAnchorName cachedSpatialAnchorName)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result EnumerateCachedSpatialAnchorNames(
				UInt32 cachedSpatialAnchorNamesCapacityInput,
				out UInt32 cachedSpatialAnchorNamesCountOutput,
				[Out] WVR_SpatialAnchorName[] cachedSpatialAnchorNames)
			{
				cachedSpatialAnchorNamesCountOutput = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result ClearCachedSpatialAnchors()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result CreateSpatialAnchorFromCacheName(ref WVR_SpatialAnchorFromCacheNameCreateInfo createInfo, out UInt64 anchor /* WVR_SpatialAnchor */)
			{
				anchor = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result PersistSpatialAnchor(ref WVR_SpatialAnchorPersistInfo spatialAnchorPersistInfo)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result UnpersistSpatialAnchor(ref WVR_SpatialAnchorName persistedSpatialAnchorName)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result EnumeratePersistedSpatialAnchorNames(
				UInt32 persistedSpatialAnchorNamesCapacityInput,
				out UInt32 persistedSpatialAnchorNamesCountOutput,
				[Out] WVR_SpatialAnchorName[] persistedSpatialAnchorNames)
			{
				persistedSpatialAnchorNamesCountOutput = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result ClearPersistedSpatialAnchors()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetPersistedSpatialAnchorCount(
				ref WVR_PersistedSpatialAnchorCountGetInfo getInfo)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result CreateSpatialAnchorFromPersistenceName(
				ref WVR_SpatialAnchorFromPersistenceNameCreateInfo createInfo,
				out UInt64 anchor /* WVR_SpatialAnchor* */)
			{
				anchor = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result ExportPersistedSpatialAnchor(
				ref WVR_SpatialAnchorName persistedSpatialAnchorName,
				UInt32 dataCapacityInput,
				out UInt32 dataCountOutput,
				[Out] byte[] data)
			{
				dataCountOutput = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result ImportPersistedSpatialAnchor(
				UInt32 dataCount,
				[In] byte[] data)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

#endregion

			#region Trackable Marker

			public virtual WVR_Result StartMarker()
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual void StopMarker()
			{
				return;
			}

			public virtual WVR_Result StartMarkerObserver(WVR_MarkerObserverTarget target)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StopMarkerObserver(WVR_MarkerObserverTarget target)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetMarkerObserverState(WVR_MarkerObserverTarget target, out WVR_MarkerObserverState state)
			{
				state = WVR_MarkerObserverState.WVR_MarkerObserverState_Max;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StartMarkerDetection(WVR_MarkerObserverTarget target)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StopMarkerDetection(WVR_MarkerObserverTarget target)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetArucoMarkers(UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, WVR_PoseOriginModel originModel, IntPtr markers /* WVR_ArucoMarker* */)
			{
				markerCountOutput = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result EnumerateTrackableMarkers(WVR_MarkerObserverTarget target, UInt32 markerCapacityInput, out UInt32 markerCountOutput /* uint32_t* */, IntPtr markerIds /* WVR_Uuid* */)
			{
				markerCountOutput = 0;
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result CreateTrackableMarker([In, Out] WVR_TrackableMarkerCreateInfo[] createInfo /* WVR_TrackableMarkerCreateInfo* */)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result DestroyTrackableMarker(WVR_Uuid markerId)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StartTrackableMarkerTracking(WVR_Uuid markerId)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result StopTrackableMarkerTracking(WVR_Uuid markerId)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetTrackableMarkerState(WVR_Uuid markerId, WVR_PoseOriginModel originModel, out WVR_TrackableMarkerState state /* WVR_TrackableMarkerState* */)
			{
				state = default(WVR_TrackableMarkerState);
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetArucoMarkerData(WVR_Uuid markerId, out WVR_ArucoMarkerData data)
			{
				data = default(WVR_ArucoMarkerData);
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			#endregion

			#region Body Tracking
			public virtual UInt32 GetBodyTrackingStandardPoseSize() { return 0; }

			public virtual bool GetBodyTrackingStandardPoseInfo([In, Out] WVR_BodyTrackingType[] types, [In, Out] UInt32[] counts)
			{
				types[0] = WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker;
				counts[0] = 2;
				types[1] = WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker;
				counts[1] = 3;
				return true;
			}

			UInt32[] s_WristTrackerIds = {
				(UInt32)WVR_TrackerId.WVR_TrackerId_0,
				(UInt32)WVR_TrackerId.WVR_TrackerId_1
			};
			UInt32[] s_InsideOutTrackerIds = {
				(UInt32)WVR_TrackerId.WVR_TrackerId_2,
				(UInt32)WVR_TrackerId.WVR_TrackerId_3,
				(UInt32)WVR_TrackerId.WVR_TrackerId_4
			};
			public virtual bool GetBodyTrackingStandardPose(WVR_BodyTrackingType type, [In, Out] UInt32[] ids, [In, Out] WVR_Pose_t[] poses)
			{
				if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker) { ids = s_WristTrackerIds; }
				if (type == WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker) { ids = s_InsideOutTrackerIds; }

				return true;
			}

			#region Tracked Device Extrinsics
			// Head
			private static readonly WVR_BodyTrackingExtrinsic extHead = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Head,
					WVR_BodyTrackingType.WVR_BodyTrackingType_HMD,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.0f, -0.08f, 0.1f),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);

			// Wrist: Self Tracker or Wrist Tracker
			private static readonly WVR_BodyTrackingExtrinsic extLeftWrist_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftWrist,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.0f, -0.035f, -0.043f),
						new WVR_Quatf_t(0.0f, 0.707f, 0.0f, -0.707f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightWrist_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightWrist,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.0f, -0.035f, -0.043f),
						new WVR_Quatf_t(0.0f, -0.707f, 0.0f, -0.707f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extLeftWrist_WristTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftWrist,
					WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(-0.03f, 0.005f, -0.056f),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightWrist_WristTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightWrist,
					WVR_BodyTrackingType.WVR_BodyTrackingType_WristTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.03f, 0.005f, -0.056f),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);

			// Controller
			private static readonly WVR_BodyTrackingExtrinsic extLeftHandheld = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHandheld,
					WVR_BodyTrackingType.WVR_BodyTrackingType_Controller,
					new WVR_Pose_t(
						new WVR_Vector3f_t(-0.03f, -0.035f, 0.13f),
						new WVR_Quatf_t(-0.345273f, 0.639022f, -0.462686f, -0.508290f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightHandheld = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHandheld,
					WVR_BodyTrackingType.WVR_BodyTrackingType_Controller,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.03f, -0.035f, 0.13f),
						new WVR_Quatf_t(-0.345273f, -0.639022f, 0.462686f, -0.508290f)
					)
				);

			// Hand
			private static readonly WVR_BodyTrackingExtrinsic extLeftHand = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftHand,
					WVR_BodyTrackingType.WVR_BodyTrackingType_Hand,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0.094802f, 0.641923f, 0.071626f, -0.757508f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightHand = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightHand,
					WVR_BodyTrackingType.WVR_BodyTrackingType_Hand,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0.094802f, -0.641923f, 0.071626f, -0.757508f)
					)
				);

			// Hip: Self Tracker
			private static readonly WVR_BodyTrackingExtrinsic extHip = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_Hip,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);

			// Knee: Self Tracker IM
			private static readonly WVR_BodyTrackingExtrinsic extLeftKnee_SelfTrackerIM = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftKnee,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightKnee_SelfTrackerIM = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightKnee,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);

			// Ankle: Self Tracker or Self Tracker IM
			private static readonly WVR_BodyTrackingExtrinsic extLeftAnkle_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftAnkle,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.0f, -0.05f, 0.0f),
						new WVR_Quatf_t(-0.5f, 0.5f, -0.5f, 0.5f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightAnkle_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightAnkle,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0.0f, -0.05f, 0.0f),
						new WVR_Quatf_t(0.5f, 0.5f, -0.5f, -0.5f)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extLeftAnkle_SelfTrackerIM = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftAnkle,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightAnkle_SelfTrackerIM = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightAnkle,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTrackerIM,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extLeftFoot_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_LeftFoot,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0.13f),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			private static readonly WVR_BodyTrackingExtrinsic extRightFoot_SelfTracker = new WVR_BodyTrackingExtrinsic(
					WVR_TrackedDeviceRole.WVR_TrackedDeviceRole_RightFoot,
					WVR_BodyTrackingType.WVR_BodyTrackingType_ViveSelfTracker,
					new WVR_Pose_t(
						new WVR_Vector3f_t(0, 0, 0.13f),
						new WVR_Quatf_t(0, 0, 0, -1)
					)
				);
			#endregion

			const UInt32 extrinsicCount = 18;
			private readonly WVR_BodyTrackingExtrinsic[] s_TrackingExtrinsics = new WVR_BodyTrackingExtrinsic[18] {
				extHead,
				extLeftWrist_SelfTracker, extRightWrist_SelfTracker,
				extLeftWrist_WristTracker, extRightWrist_WristTracker,
				extLeftHandheld, extRightHandheld,
				extLeftHand, extRightHand,
				extHip,
				extLeftKnee_SelfTrackerIM, extRightKnee_SelfTrackerIM,
				extLeftAnkle_SelfTracker, extRightAnkle_SelfTracker,
				extLeftAnkle_SelfTrackerIM, extRightAnkle_SelfTrackerIM,
				extLeftFoot_SelfTracker, extRightFoot_SelfTracker,
			};
			public virtual UInt32 GetBodyTrackingExtrinsicCount()
			{
				return extrinsicCount;
			}
			public virtual bool GetBodyTrackingExtrinsics([In, Out] WVR_BodyTrackingExtrinsic[] extrinsics, ref UInt32 count)
			{
				count = extrinsicCount;
				for (UInt32 i = 0; i < count; i++)
					extrinsics[i] = s_TrackingExtrinsics[i];

				return true;
			}
			#endregion

			public virtual ulong GetSupportedFeatures()
			{
				return 0;
			}

			// wvr.h
			public virtual WVR_InitError Init(WVR_AppType eType)
			{
				return WVR_InitError.WVR_InitError_None;
			}

			public virtual void PostInit()
			{

			}

			public virtual void Quit()
			{
			}

			public virtual IntPtr GetInitErrorString(WVR_InitError error)
			{
				IntPtr t  = new IntPtr();
				return t;
			}

			public virtual uint GetWaveRuntimeVersion()
			{
				return 1;
			}

			public virtual uint GetWaveSDKVersion()
			{
				return 1;
			}

			// wvr_system.h
			public virtual bool IsInputFocusCapturedBySystem()
			{
				return false;
			}

			internal virtual WVR_RenderError RenderInit(ref WVR_RenderInitParams_t param)
			{
				return WVR_RenderError.WVR_RenderError_None;
			}

			// Set CPU and GPU performance level.
			internal virtual bool SetPerformanceLevels(WVR_PerfLevel cpuLevel, WVR_PerfLevel gpuLevel)
			{
				return true;
			}

			// Allow WaveVR SDK runtime to adjust render quality and CPU/GPU perforamnce level automatically.
			internal virtual bool EnableAdaptiveQuality(bool enable, uint flags)
			{
				return true;
			}

			// Check if adaptive quailty enabled.
			internal virtual bool IsAdaptiveQualityEnabled()
			{
				return false;
			}

            // wvr_camera.h
            public virtual bool StartCamera(ref WVR_CameraInfo_t info)
			{
				return false;
			}

            public virtual void StopCamera()
			{
			}

			public virtual bool UpdateTexture(IntPtr textureid)
			{
				return false;
			}

			public virtual bool GetCameraIntrinsic(WVR_CameraPosition position, ref WVR_CameraIntrinsic_t intrinsic)
			{
				return true;
			}

            public virtual bool GetCameraFrameBuffer(IntPtr pFramebuffer, uint frameBufferSize)
			{
				return false;
			}

            public virtual bool GetFrameBufferWithPoseState(IntPtr frameBuffer, uint frameBufferSize, WVR_PoseOriginModel origin, uint predictInMs, ref WVR_PoseState_t poseState)
			{
				return false;
			}

            public virtual bool DrawTextureWithBuffer(IntPtr textureId, WVR_CameraImageFormat imgFormat, IntPtr frameBuffer, uint size, uint width, uint height, bool enableCropping, bool clearClampRegion)
			{
				return false;
			}

            public virtual void ReleaseCameraTexture()
			{
			}

			// wvr_device.h
			public virtual bool IsDeviceSuspend(WVR_DeviceType type)
			{
				return false;
			}

			public virtual void ConvertMatrixQuaternion(ref WVR_Matrix4f_t mat, ref WVR_Quatf_t quat, bool m2q)
			{
			}

			public virtual WVR_NumDoF GetDegreeOfFreedom(WVR_DeviceType type)
			{
				return WVR_NumDoF.WVR_NumDoF_3DoF;
			}

			public virtual void SetParameters(WVR_DeviceType type, IntPtr pchValue)
			{
			}

			public virtual uint GetParameters(WVR_DeviceType type, IntPtr pchValue, IntPtr retValue, uint unBufferSize)
			{
				return 0;
			}

			public virtual WVR_DeviceType GetDefaultControllerRole()
			{
				return WVR_DeviceType.WVR_DeviceType_Invalid;
			}

			[System.Obsolete("This is an obsolete function.", true)]
			public virtual bool SetInteractionMode(WVR_InteractionMode mode)
			{
				return true;
			}

			public virtual WVR_InteractionMode GetInteractionMode()
			{
				return WVR_InteractionMode.WVR_InteractionMode_Controller;
			}

			public virtual bool SetGazeTriggerType(WVR_GazeTriggerType type)
			{
				return true;
			}

			public virtual WVR_GazeTriggerType GetGazeTriggerType()
			{
				return WVR_GazeTriggerType.WVR_GazeTriggerType_TimeoutButton;
			}

			public virtual bool GetDeviceErrorState(WVR_DeviceType dev_type, WVR_DeviceErrorStatus error_type)
			{
				return false; ;
			}

			// TODO
			public virtual void GetRenderTargetSize(ref uint width, ref uint height)
			{
			}

			public virtual WVR_Matrix4f_t GetProjection(WVR_Eye eye, float near, float far)
			{
				WVR_Matrix4f_t _T = new WVR_Matrix4f_t();
				return _T;
			}

			public virtual void GetClippingPlaneBoundary(WVR_Eye eye, ref float left, ref float right, ref float top, ref float bottom)
			{
			}

			public virtual void SetOverfillRatio(float ratioX, float ratioY)
			{
			}

			public virtual WVR_Matrix4f_t GetTransformFromEyeToHead(WVR_Eye eye, WVR_NumDoF dof)
			{
				WVR_Matrix4f_t _T = new WVR_Matrix4f_t();
				return _T;
			}

			public virtual WVR_SubmitError SubmitCompositionLayers([In, Out] WVR_LayerSetParams_t[] param)
			{
				return WVR_SubmitError.WVR_SubmitError_None;
			}

            public virtual uint GetMaxCompositionLayerCount()
            {
                return 0;
            }

			public virtual IntPtr CreateAndroidSurface(int width, int height, bool isProtected, [In, Out] WVR_TextureParams_t[] param)
			{
				return new IntPtr();
			}

			public virtual void DeleteAndroidSurface()
			{
			}

			public virtual WVR_SubmitError SubmitFrame(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod)
			{
				return WVR_SubmitError.WVR_SubmitError_None;
			}

			public virtual void SetSubmitParams(WVR_Eye eye, [In, Out] WVR_TextureParams_t[] param, [In, Out] WVR_PoseState_t[] pose, WVR_SubmitExtend extendMethod)
			{
			}

			public virtual void PreRenderEye(WVR_Eye eye, [Out] WVR_TextureParams_t[] param, [Out] WVR_RenderFoveationParams[] foveationParams)
			{
			}

			public virtual bool RequestScreenshot(uint width, uint height, WVR_ScreenshotMode mode, IntPtr filename)
			{
				return true;
			}

			public virtual void RenderMask(WVR_Eye eye)
			{
			}

			public virtual bool GetRenderProps(ref WVR_RenderProps_t props)
			{
				return true;
			}

			public virtual IntPtr ObtainTextureQueue(WVR_TextureTarget target, WVR_TextureFormat format, WVR_TextureType type, uint width, uint height, int level)
			{
				return new IntPtr();
			}

			public virtual uint GetTextureQueueLength(IntPtr handle)
			{
				return 0;
			}

			public virtual WVR_TextureParams_t GetTexture(IntPtr handle, int index)
			{
				return new WVR_TextureParams_t();
			}

			public virtual int GetAvailableTextureIndex(IntPtr handle)
			{
				return -1;
			}

			public virtual void ReleaseTextureQueue(IntPtr handle)
			{
			}

			public virtual bool IsRenderFoveationSupport()
			{
				return false;
			}

			public virtual void RenderFoveation(bool enable)
			{
			}

			public virtual WVR_Result RenderFoveationMode(WVR_FoveationMode mode)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result SetFoveationConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual WVR_Result GetFoveationDefaultConfig(WVR_Eye eye, [In, Out] WVR_RenderFoveationParams[] foveationParams)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual bool IsRenderFoveationEnabled()
			{
				return false;
			}

			public virtual bool IsRenderFoveationDefaultOn()
			{
				return false;
			}

			public virtual void SetPosePredictEnabled(WVR_DeviceType type, bool enabled_position_predict, bool enable_rotation_predict)
			{
			}

			public virtual bool ShowPassthroughOverlay(bool show, bool delaySubmit = false, bool showIndicator = false)
			{
				return false;
			}

			public virtual WVR_Result SetPassthroughOverlayAlpha(float alpha)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result ShowPassthroughUnderlay(bool show)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual void EnableAutoPassthrough(bool enable)
			{

			}

			public virtual bool IsPassthroughOverlayVisible()
			{
				return false;
			}

			public virtual bool SetChecker(bool enable)
			{
				return false;
			}

			public virtual WVR_Result SetProjectedPassthroughPose(ref WVR_Pose_t pose)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result SetProjectedPassthroughMesh(float[] vertexBuffer, uint vertextCount, uint[] indices, uint indexCount)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result SetProjectedPassthroughAlpha(float alpha)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result ShowProjectedPassthrough(bool show)
			{
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result GetCurrentControllerModel(WVR_DeviceType type, ref IntPtr ctrlerModel /* WVR_CtrlerModel* */, bool isOneBone)
			{
				ctrlerModel = IntPtr.Zero;
				return WVR_Result.WVR_Error_CtrlerModel_Unknown;
			}

			public virtual void ReleaseControllerModel(ref IntPtr ctrlerModel /* WVR_CtrlerModel* */)
			{

			}

			public virtual WVR_Result GetCurrentNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
			{
				handModel = IntPtr.Zero;
				return WVR_Result.WVR_Error_InvalidRenderModel;
			}

			public virtual void ReleaseNaturalHandModel(ref IntPtr handModel /* WVR_HandRenderModel* */)
			{
			}

			public virtual WVR_Result GetCtrlerModelAnimNodeData(WVR_DeviceType type, ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
			{
				ctrlModelAnimData = IntPtr.Zero;
				return WVR_Result.WVR_Error_CtrlerModel_Unknown;
			}

			public virtual void ReleaseCtrlerModelAnimNodeData(ref IntPtr ctrlModelAnimData /* WVR_CtrlerModelAnimData_t* */)
			{

			}

			public virtual WVR_Result SetAMCMode(WVR_AMCMode mode)
            {
                return WVR_Result.WVR_Error_FeatureNotSupport;
            }

            // Internal function.  Only be used for debug.
            public virtual WVR_AMCMode GetAMCMode()
            {
                return WVR_AMCMode.Off;
            }

            // Internal function.  Only be used for debug.
            public virtual WVR_AMCState GetAMCState()
            {
                return WVR_AMCState.Off;
            }

			public virtual WVR_Result SetFrameSharpnessEnhancementLevel(float level)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual bool IsDeviceTableStatic(WVR_DeviceType type)
			{
				return false;
			}

			public virtual void GetSpectatorRenderTargetSize(ref uint width, ref uint height)
			{
				width = 1920;
				height = 1080;
			}

			public virtual void GetSpectatorClippingPlaneBoundary(ref float l, ref float r, ref float t, ref float b)
			{
				l = -16.0f / 9.0f;
				r = 1;
				t = 1;
				b = -16.0f / 9.0f;
			}

			public virtual bool PreSpectatorRender(ref WVR_SpectatorState state)
			{
				state.shouldRender = false;
				return false;
			}

			public virtual WVR_Result WVR_GetAvailableFrameRates(out uint[] frameRates)
			{
				// Fake data for editor preview
				frameRates = new uint[] { 75, 90, 120 };
				return WVR_Result.WVR_Success;
			}

			public virtual WVR_Result WVR_GetFrameRate(ref uint frameRate)
			{
				// Fake data for editor preview
				frameRate = 90;
				return WVR_Result.WVR_Success;
			}
			public virtual WVR_Result WVR_SetFrameRate(uint frameRate)
			{
				return WVR_Result.WVR_Error_FeatureNotSupport;
			}

			public virtual bool WVR_SetPassthroughImageQuality(WVR_PassthroughImageQuality quality)
			{
				return false;
			}

			public virtual bool WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus focus)
			{
				return false;
			}

			public virtual WVR_Result SetPassthroughImageRate(WVR_PassthroughImageRate rate)
			{
				return WVR_Result.WVR_Success;
            }

            public virtual void EnableHandleDisplayChanged(bool enable)
            {
                return;
            }

			#region Internal
            public virtual string DeployRenderModelAssets(int deviceIndex, string renderModelName)
			{
				return "";
			}

			public virtual void SetFocusedController(WVR_DeviceType focusController)
			{

			}

			public virtual WVR_DeviceType GetFocusedController()
			{
				return WVR_DeviceType.WVR_DeviceType_Controller_Right;
			}

			public virtual float GetResolutionMaxScale()
			{
				return 0.9f;
			}

			public virtual bool OpenMesh(string filename, ref uint sessiionid, IntPtr errorCode, bool merge)
			{
				return false;
			}

			public virtual bool GetSectionCount(uint sessionid, ref uint sectionCount)
			{
				return false;
			}

			public virtual bool GetMeshData(uint sessionid, [In, Out] FBXInfo_t[] infoArray)
			{
				return false;
			}

			public virtual bool GetSectionData(uint sessionid, uint sectionIndiceIndex, [In, Out] Vector3[] vecticeArray, [In, Out] Vector3[] normalArray, [In, Out] Vector2[] uvArray, [In, Out] int[] indiceArray, ref bool active)
			{
				return false;
			}

			public virtual void ReleaseMesh(uint sessionid)
			{

			}

			public virtual bool IsPermissionInitialed()
			{
				return true;
			}

			public virtual bool ShowDialogOnScene()
			{
				return true;
			}

			public virtual bool IsPermissionGranted(string permission)
			{
				return true;
			}

			public virtual bool ShouldGrantPermission(string permission)
			{
				return false;
			}

			public virtual void RequestPermissions(string[] permissions, WVR_RequestCompleteCallback cb)
			{
				List<WVR_RequestResult> permissionResults = new List<WVR_RequestResult>();

				if (permissions != null)
				{
					for (int i = 0; i < permissions.Length; i++)
					{
						WVR_RequestResult rr;
						rr.mPermission = permissions[i];
						rr.mGranted = true;
						permissionResults.Add(rr);
					}
				}

				cb(permissionResults);
			}

			public virtual void RequestUsbPermission(WVR_RequestUsbCompleteCallback cb)
			{
				cb(true);
			}



			public virtual string GetStringBySystemLanguage(string stringName)
			{
				return stringName;
			}

			public virtual string GetStringByLanguage(string stringName, string lang, string country)
			{
				return stringName;
			}

			public virtual string GetSystemLanguage()
			{
				return "";
			}

			public virtual string GetSystemCountry()
			{
				return "";
			}

			public virtual string GetOEMConfigByKey(string key)
			{
				return "";
			}

			public virtual void SetOEMConfigChangedCallback(WVR_OnOEMConfigChanged cb)
			{

			}
			#endregion
		}
	}
} // namespace Wave.Native
