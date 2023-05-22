// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Settings;

namespace Wave.OpenXR
{
    public static class InputDeviceEye
    {
        const string LOG_TAG = "Wave.OpenXR.InputDeviceEye";
        static void DEBUG(string msg) { UnityEngine.Debug.Log(LOG_TAG + " " + msg); }

        public enum Expressions : UInt32
        {
            LEFT_BLINK = 0,
            LEFT_WIDE = 1,
            RIGHT_BLINK = 2,
            RIGHT_WIDE = 3,
            LEFT_SQUEEZE = 4,
            RIGHT_SQUEEZE = 5,
            LEFT_DOWN = 6,
            RIGHT_DOWN = 7,
            LEFT_OUT = 8,
            RIGHT_IN = 9,
            LEFT_IN = 10,
            RIGHT_OUT = 11,
            LEFT_UP = 12,
            RIGHT_UP = 13,
            MAX,
        }
        private static UInt32 Id(this Expressions exp) { return (UInt32)exp; }
        public static readonly Expressions[] s_EyeExpressions = new Expressions[(int)Expressions.MAX]
        {
            Expressions.LEFT_BLINK, // 0
            Expressions.LEFT_WIDE,
            Expressions.RIGHT_BLINK,
            Expressions.RIGHT_WIDE,
            Expressions.LEFT_SQUEEZE,
            Expressions.RIGHT_SQUEEZE, // 5
            Expressions.LEFT_DOWN,
            Expressions.RIGHT_DOWN,
            Expressions.LEFT_OUT,
            Expressions.RIGHT_IN,
            Expressions.LEFT_IN, // 10
            Expressions.RIGHT_OUT,
            Expressions.LEFT_UP,
            Expressions.RIGHT_UP,
        };

        public enum TrackingStatus : UInt32
        {
            NOT_START = 0,
            START_FAILURE = 1,
            STARTING = 2,
            STOPPING = 3,
            AVAILABLE = 4,
            UNSUPPORT = 5,
        }
        private static UInt32 Id(this TrackingStatus status) { return (UInt32)status; }

        public enum TrackingSpace : UInt32
        {
            Local = 0,
            World = 1,
        }
        public static UInt32 Id(this TrackingSpace space) { return (UInt32)space; }

        #region Wave XR Constants
        const string kIsEyeExpressionAvailable = "IsEyeExpressionAvailable";
        const string kHasEyeExpressionValue = "HasEyeExpressionValue";
        const string kLEFT_BLINK = "EYEEXPRESSION_LEFT_BLINK";    // 0
        const string kLEFT_WIDE = "EYEEXPRESSION_LEFT_WIDE";
        const string kRIGHT_BLINK = "EYEEXPRESSION_RIGHT_BLINK";
        const string kRIGHT_WIDE = "EYEEXPRESSION_RIGHT_WIDE";
        const string kLEFT_SQUEEZE = "EYEEXPRESSION_LEFT_SQUEEZE";
        const string kRIGHT_SQUEEZE = "EYEEXPRESSION_RIGHT_SQUEEZE"; // 5
        const string kLEFT_DOWN = "EYEEXPRESSION_LEFT_DOWN";
        const string kRIGHT_DOWN = "EYEEXPRESSION_RIGHT_DOWN";
        const string kLEFT_OUT = "EYEEXPRESSION_LEFT_OUT";
        const string kRIGHT_IN = "EYEEXPRESSION_RIGHT_IN";
        const string kLEFT_IN = "EYEEXPRESSION_LEFT_IN";       // 10
        const string kRIGHT_OUT = "EYEEXPRESSION_RIGHT_OUT";
        const string kLEFT_UP = "EYEEXPRESSION_LEFT_UP";
        const string kRIGHT_UP = "EYEEXPRESSION_RIGHT_UP";

        const string kEyesName = "Wave Eye Tracking";
        const string kEyesSerialNumber = "HTC-220926-EyeTracking";
        const InputDeviceCharacteristics kEyesCharacteristics = InputDeviceCharacteristics.EyeTracking;

        const string kEyeTrackingStatus = "EyeTrackingStatus";
        const string kEyeSpace = "EyeSpace";

        const string kCombinedEyeOriginValid = "CombinedEyeOriginValid";
        const string kCombinedEyeOriginX = "CombinedEyeOriginX";
        const string kCombinedEyeOriginY = "CombinedEyeOriginY";
        const string kCombinedEyeOriginZ = "CombinedEyeOriginZ";
        const string kCombinedEyeDirectionValid = "CombinedEyeDirectionValid";

        const string kLeftEyeOriginValid = "LeftEyeOriginValid";
        const string kLeftEyeDirectionValid = "LeftEyeDirectionValid";
        const string kLeftEyeOpennessValid = "LeftEyeOpennessValid";
        const string kLeftEyePupilDiameterValid = "LeftEyePupilDiameterValid";
        const string kLeftEyePupilDiameter = "LeftEyePupilDiameter";
        const string kLeftEyePupilPositionInSensorAreaValid = "LeftEyePupilPositionInSensorAreaValid";
        const string kLeftEyePupilPositionInSensorAreaX = "LeftEyePupilPositionInSensorAreaX";
        const string kLeftEyePupilPositionInSensorAreaY = "LeftEyePupilPositionInSensorAreaY";

        const string kRightEyeOriginValid = "RightEyeOriginValid";
        const string kRightEyeDirectionValid = "RightEyeDirectionValid";
        const string kRightEyeOpennessValid = "RightEyeOpennessValid";
        const string kRightEyePupilDiameterValid = "RightEyePupilDiameterValid";
        const string kRightEyePupilDiameter = "RightEyePupilDiameter";
        const string kRightEyePupilPositionInSensorAreaValid = "RightEyePupilPositionInSensorAreaValid";
        const string kRightEyePupilPositionInSensorAreaX = "RightEyePupilPositionInSensorAreaX";
        const string kRightEyePupilPositionInSensorAreaY = "RightEyePupilPositionInSensorAreaY";
        #endregion

        // Keys and Values size = 14
        static string[] s_ExpKeys = {
            kLEFT_BLINK, // 0
            kLEFT_WIDE,
            kRIGHT_BLINK,
            kRIGHT_WIDE,
            kLEFT_SQUEEZE,
            kRIGHT_SQUEEZE, // 5
            kLEFT_DOWN,
            kRIGHT_DOWN,
            kLEFT_OUT,
            kRIGHT_IN,
            kLEFT_IN, // 10
            kRIGHT_OUT,
            kLEFT_UP,
            kRIGHT_UP,
        };
        static float[] s_ExpValues = {
            0, // LEFT_BLINK = 0
            0, // LEFT_WIDE
            0, // RIGHT_BLINK
            0, // RIGHT_WIDE
            0, // LEFT_SQUEEZE
            0, // RIGHT_SQUEEZE = 5,
            0, // LEFT_DOWN
            0, // RIGHT_DOWN
            0, // LEFT_OUT
            0, // RIGHT_IN
            0, // LEFT_IN = 10,
            0, // RIGHT_OUT
            0, // LEFT_UP
            0, // RIGHT_UP
        };

        #region Wave XR Eye Tracking Interface
        /// <summary>
        /// Enables or disables the Eye Tracking feature.
        /// </summary>
        /// <param name="active">True for enable.</param>
        public static void ActivateEyeTracking(bool active)
        {
            WaveXRSettings settings = WaveXRSettings.GetInstance();
            if (settings != null)
            {
                // Check current Wave XR Eye Expression status before activation.
                settings.EnableEyeTracking = IsEyeTrackingAvailable();

                string caller = "TBD";
                var frame = new StackFrame(1, true);
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                        caller = method.Name;
                    else
                        caller = "No method.";
                }
                if (settings.EnableEyeTracking != active)
                {
                    settings.EnableEyeTracking = active;
                    DEBUG("ActivateEyeTracking() " + (settings.EnableEyeTracking ? "Activate." : "Deactivate.") + " from " + caller);
                    SettingsHelper.SetBool(WaveXRSettings.EnableEyeTrackingText, settings.EnableEyeTracking);
                }
                else
                {
                    DEBUG("ActivateEyeTracking() Eye Tracking is already " + (settings.EnableEyeTracking ? "enabled." : "disabled.") + " from " + caller);
                }
            }
        }

        /// <summary>
        /// Retrieves current Eye Tracking status.
        /// </summary>
        /// <returns>ID refers to <see cref="TrackingStatus">TrackingStatus</see>.</returns>
        public static TrackingStatus GetEyeTrackingStatus()
        {
            UInt32 id = 5; // UNSUPPORT
            SettingsHelper.GetInt(kEyeTrackingStatus, ref id);

            TrackingStatus status = TrackingStatus.UNSUPPORT;
            if (id == 0) { status = TrackingStatus.NOT_START; }
            if (id == 1) { status = TrackingStatus.START_FAILURE; }
            if (id == 2) { status = TrackingStatus.STARTING; }
            if (id == 3) { status = TrackingStatus.STOPPING; }
            if (id == 4) { status = TrackingStatus.AVAILABLE; }

            //Debug.Log(LOG_TAG + " GetEyeTrackingStatus() " + status);
            return status;
        }

        /// <summary>
        /// Checks if the Eye Expression feature is available currently.
        /// </summary>
        /// <returns>True for available.</returns>
        public static bool IsEyeTrackingAvailable()
        {
            TrackingStatus status = GetEyeTrackingStatus();
            return (status == TrackingStatus.AVAILABLE);
        }

        /// <summary>
        /// Sets up the eye tracking space.
        ///
        /// When the eye space is Local, the tracking position is a fixed value which presents the offset from HMD to eye.
        ///
        /// When the eye space is World, the tracking position is a value which presents (the offset from HMD to eye) + (the HMD position).
        /// </summary>
        /// <param name="space">Local or World space.</param>
        public static void SetEyeTrackingSpace(TrackingSpace space)
        {
            //Debug.Log(LOG_TAG + " SetEyeTrackingSpace() " + space);
            SettingsHelper.SetInt(kEyeSpace, space.Id());
        }

        /// <summary>
        /// Checks if an InputDevice is Wave Eye Tracking device.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>True for Wave Eye Tracking device.</returns>
        public static bool IsEyeTrackingDevice(InputDevice input)
        {
            if (input != null && input.name.Equals(kEyesName) && input.serialNumber.Equals(kEyesSerialNumber) && input.characteristics.Equals(kEyesCharacteristics))
                return true;

            return false;
        }

        private static bool IsEyeTrackingDeviceConnected(InputDevice input)
        {
            if (IsEyeTrackingDevice(input))
                return input.isValid;

            return false;
        }
        internal static List<InputDevice> s_InputDevices = new List<InputDevice>();
        /// <summary>
        /// Checks if a Wave Eye Tracking device is connected.
        /// </summary>
        /// <returns>True for connected.</returns>
        public static bool IsEyeTrackingDeviceConnected()
        {
            InputDevices.GetDevices(s_InputDevices);
            for (int i = 0; i < s_InputDevices.Count; i++)
            {
                if (IsEyeTrackingDeviceConnected(s_InputDevices[i]))
                    return true;
            }
            return false;
        }

        private static bool IsEyeTrackingTracked(InputDevice input)
        {
            if (IsEyeTrackingDeviceConnected(input))
            {
                if (input.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
                    return isTracked;
            }
            return false;
        }
        /// <summary>
        /// Checks if a Wave Eye Tracking device is tracked.
        /// </summary>
        /// <returns>True for tracked.</returns>
        public static bool IsEyeTrackingTracked()
        {
            InputDevices.GetDevices(s_InputDevices);
            for (int i = 0; i < s_InputDevices.Count; i++)
            {
                if (IsEyeTrackingTracked(s_InputDevices[i]))
                    return true;
            }
            return false;
        }

        internal static bool m_EyesValid = false;
        internal static Eyes m_Eyes;

        static float m_EyeTrackingFrame = 0;
        static bool m_CombinedEyeOriginValid = false;
        static Vector3 m_CombinedEyeOrigin = Vector3.zero;
        static bool m_CombinedEyeDirectionValid = false;
        static Vector3 m_CombinedEyeDirection = Vector3.zero;

        static bool m_LeftEyeOriginValid = false;
        static Vector3 m_LeftEyeOrigin = Vector3.zero;
        static bool m_LeftEyeDirectionValid = false;
        static Vector3 m_LeftEyeDirection = Vector3.zero;
        static bool m_LeftEyeOpennessValid = false;
        static float m_LeftEyeOpenness = 0;
        static bool m_LeftEyePupilDiameterValid = false;
        static float m_LeftEyePupilDiameter = 0;
        static bool m_LeftEyePupilPositionInSensorAreaValid = false;
        static Vector2 m_LeftEyePupilPositionInSensorArea = Vector2.zero;

        static bool m_RightEyeOriginValid = false;
        static Vector3 m_RightEyeOrigin = Vector3.zero;
        static bool m_RightEyeDirectionValid = false;
        static Vector3 m_RightEyeDirection = Vector3.zero;
        static bool m_RightEyeOpennessValid = false;
        static float m_RightEyeOpenness = 0;
        static bool m_RightEyePupilDiameterValid = false;
        static float m_RightEyePupilDiameter = 0;
        static bool m_RightEyePupilPositionInSensorAreaValid = false;
        static Vector2 m_RightEyePupilPositionInSensorArea = Vector2.zero;
        static void UpdateEyeTrackingData()
        {
            if (m_EyeTrackingFrame == Time.frameCount) { return; }
            m_EyeTrackingFrame = Time.frameCount;

            m_EyesValid = false;
            InputDevices.GetDevices(s_InputDevices);
            for (int i = 0; i < s_InputDevices.Count; i++)
            {
                if (IsEyeTrackingTracked(s_InputDevices[i]))
                {
                    if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.eyesData, out Eyes eyeData))
                    {
                        m_EyesValid = true;
                        m_Eyes = eyeData;
                        break;
                    }
                }
            }

            if (m_EyesValid)
            {
                SettingsHelper.GetBool(kCombinedEyeOriginValid, ref m_CombinedEyeOriginValid);
                SettingsHelper.GetFloat(kCombinedEyeOriginX, ref m_CombinedEyeOrigin.x);
                SettingsHelper.GetFloat(kCombinedEyeOriginY, ref m_CombinedEyeOrigin.y);
                SettingsHelper.GetFloat(kCombinedEyeOriginZ, ref m_CombinedEyeOrigin.z);
                SettingsHelper.GetBool(kCombinedEyeDirectionValid, ref m_CombinedEyeDirectionValid);
                if (m_Eyes.TryGetFixationPoint(out Vector3 fixationPoint)) { m_CombinedEyeDirection = fixationPoint; }

                SettingsHelper.GetBool(kLeftEyeOriginValid, ref m_LeftEyeOriginValid);
                if (m_Eyes.TryGetLeftEyePosition(out Vector3 leftPos)) { m_LeftEyeOrigin = leftPos; }
                SettingsHelper.GetBool(kLeftEyeDirectionValid, ref m_LeftEyeDirectionValid);
                if (m_Eyes.TryGetLeftEyeRotation(out Quaternion leftRot)) { m_LeftEyeDirection = leftRot * Vector3.forward; }
                SettingsHelper.GetBool(kLeftEyeOpennessValid, ref m_LeftEyeOpennessValid);
                if (m_Eyes.TryGetLeftEyeOpenAmount(out float leftOpen)) { m_LeftEyeOpenness = leftOpen; }
                SettingsHelper.GetBool(kLeftEyePupilDiameterValid, ref m_LeftEyePupilDiameterValid);
                SettingsHelper.GetFloat(kLeftEyePupilDiameter, ref m_LeftEyePupilDiameter);
                SettingsHelper.GetBool(kLeftEyePupilPositionInSensorAreaValid, ref m_LeftEyePupilPositionInSensorAreaValid);
                SettingsHelper.GetFloat(kLeftEyePupilPositionInSensorAreaX, ref m_LeftEyePupilPositionInSensorArea.x);
                SettingsHelper.GetFloat(kLeftEyePupilPositionInSensorAreaY, ref m_LeftEyePupilPositionInSensorArea.y);

                SettingsHelper.GetBool(kRightEyeOriginValid, ref m_RightEyeOriginValid);
                if (m_Eyes.TryGetRightEyePosition(out Vector3 rightPos)) { m_RightEyeOrigin = rightPos; }
                SettingsHelper.GetBool(kRightEyeDirectionValid, ref m_RightEyeDirectionValid);
                if (m_Eyes.TryGetRightEyeRotation(out Quaternion rightRot)) { m_RightEyeDirection = rightRot * Vector3.forward; }
                SettingsHelper.GetBool(kRightEyeOpennessValid, ref m_RightEyeOpennessValid);
                if (m_Eyes.TryGetRightEyeOpenAmount(out float rightOpen)) { m_RightEyeOpenness = rightOpen; }
                SettingsHelper.GetBool(kRightEyePupilDiameterValid, ref m_RightEyePupilDiameterValid);
                SettingsHelper.GetFloat(kRightEyePupilDiameter, ref m_RightEyePupilDiameter);
                SettingsHelper.GetBool(kRightEyePupilPositionInSensorAreaValid, ref m_RightEyePupilPositionInSensorAreaValid);
                SettingsHelper.GetFloat(kRightEyePupilPositionInSensorAreaX, ref m_RightEyePupilPositionInSensorArea.x);
                SettingsHelper.GetFloat(kRightEyePupilPositionInSensorAreaY, ref m_RightEyePupilPositionInSensorArea.y);
            }
            else
            {
                m_CombinedEyeOriginValid = false;
                m_CombinedEyeOrigin = Vector3.zero;
                m_CombinedEyeDirectionValid = false;
                m_CombinedEyeDirection = Vector3.zero;

                m_LeftEyeOriginValid = false;
                m_LeftEyeOrigin = Vector3.zero;
                m_LeftEyeDirectionValid = false;
                m_LeftEyeDirection = Vector3.zero;
                m_LeftEyeOpennessValid = false;
                m_LeftEyeOpenness = 0;
                m_LeftEyePupilDiameterValid = false;
                m_LeftEyePupilDiameter = 0;
                m_LeftEyePupilPositionInSensorAreaValid = false;
                m_LeftEyePupilPositionInSensorArea = Vector2.zero;

                m_RightEyeOriginValid = false;
                m_RightEyeOrigin = Vector3.zero;
                m_RightEyeDirectionValid = false;
                m_RightEyeDirection = Vector3.zero;
                m_RightEyeOpennessValid = false;
                m_RightEyeOpenness = 0;
                m_RightEyePupilDiameterValid = false;
                m_RightEyePupilDiameter = 0;
                m_RightEyePupilPositionInSensorAreaValid = false;
                m_RightEyePupilPositionInSensorArea = Vector2.zero;
            }
        }

        /// <summary>
        /// Retrieves the Unity XR Eyes data.
        /// </summary>
        /// <param name="eyeData">Data in <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">XR.Eyes</see>.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetEyeTrackingData(out Eyes eyeData)
        {
            UpdateEyeTrackingData();
            eyeData = m_Eyes;
            return m_EyesValid;
        }

        /// <summary>
        /// Retrieves the combined eye origin in Vector3.
        /// </summary>
        /// <param name="origin">Combined eye origin in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetCombinedEyeOrigin(out Vector3 origin)
        {
            UpdateEyeTrackingData();
            origin = m_CombinedEyeOrigin;
            return m_CombinedEyeOriginValid;
        }

        /// <summary>
        /// Retrieves the combined eye direction in Vector3 from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Fixation Point</see>.
        /// </summary>
        /// <param name="direction">Combined eye direction in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetCombinedEyeDirection(out Vector3 direction)
        {
            UpdateEyeTrackingData();
            direction = m_CombinedEyeDirection;
            return m_CombinedEyeDirectionValid;
        }

        /// <summary>
        /// Retrieves the left eye origin in Vecotr3 from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Left Eye Position</see>.
        /// </summary>
        /// <param name="origin">Left eye origin in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetLeftEyeOrigin(out Vector3 origin)
        {
            UpdateEyeTrackingData();
            origin = m_LeftEyeOrigin;
            return m_LeftEyeOriginValid;
        }

        /// <summary>
        /// Retrieves the left eye direction in Vector3 from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Left Eye Rotation</see>.
        /// </summary>
        /// <param name="direction">Left eye direction in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetLeftEyeDirection(out Vector3 direction)
        {
            UpdateEyeTrackingData();
            direction = m_LeftEyeDirection;
            return m_LeftEyeDirectionValid;
        }

        /// <summary>
        /// Retrieves the left eye openness in float from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Left Eye Open Amount</see>.
        /// </summary>
        /// <param name="openness">Left eye openness in float.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetLeftEyeOpenness(out float openness)
        {
            UpdateEyeTrackingData();
            openness = m_LeftEyeOpenness;
            return m_LeftEyeOpennessValid;
        }

        /// <summary>
        /// Retrieves the left eye pupil diameter.
        /// </summary>
        /// <param name="pupilDiameter">Left eye pupil diameter in float.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetLeftEyePupilDiameter(out float pupilDiameter)
        {
            UpdateEyeTrackingData();
            pupilDiameter = m_LeftEyePupilDiameter;
            return m_LeftEyePupilDiameterValid;
        }

        /// <summary>
        /// Retrieves the left eye pupil position in sensor area.
        /// </summary>
        /// <param name="pupilPosition">Left eye pupil position in Vector2.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetLeftEyePupilPositionInSensorArea(out Vector2 pupilPosition)
        {
            UpdateEyeTrackingData();
            pupilPosition = m_LeftEyePupilPositionInSensorArea;
            return m_LeftEyePupilPositionInSensorAreaValid;
        }

        /// <summary>
        /// Retrieves the right eye origin in Vector3.
        /// </summary>
        /// <param name="origin">Right eye origin in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetRightEyeOrigin(out Vector3 origin)
        {
            UpdateEyeTrackingData();
            origin = m_RightEyeOrigin;
            return m_RightEyeOriginValid;
        }

        /// <summary>
        /// Retrieves the right eye direction in Vector3 from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Right Eye Rotation</see>.
        /// </summary>
        /// <param name="direction">Right eye direction in Vector3.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetRightEyeDirection(out Vector3 direction)
        {
            UpdateEyeTrackingData();
            direction = m_RightEyeDirection;
            return m_RightEyeDirectionValid;
        }

        /// <summary>
        /// Retrieves the right eye openness in float from <see href="https://docs.unity3d.com/ScriptReference/XR.Eyes.html">Right Eye Open Amount</see>.
        /// </summary>
        /// <param name="openness">Right eye openness in float.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetRightEyeOpenness(out float openness)
        {
            UpdateEyeTrackingData();
            openness = m_RightEyeOpenness;
            return m_RightEyeOpennessValid;
        }

        /// <summary>
        /// Retrieves the right eye pupil diameter.
        /// </summary>
        /// <param name="pupilDiameter">Right eye pupil diameter in float.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetRightEyePupilDiameter(out float pupilDiameter)
        {
            UpdateEyeTrackingData();
            pupilDiameter = m_RightEyePupilDiameter;
            return m_RightEyePupilDiameterValid;
        }

        /// <summary>
        /// Retrieves the right eye pupil position in sensor area.
        /// </summary>
        /// <param name="pupilPosition">Right eye pupil position in Vector2.</param>
        /// <returns>True for valid data.</returns>
        public static bool GetRightEyePupilPositionInSensorArea(out Vector2 pupilPosition)
        {
            UpdateEyeTrackingData();
            pupilPosition = m_RightEyePupilPositionInSensorArea;
            return m_RightEyePupilPositionInSensorAreaValid;
        }
        #endregion

        #region Wave XR Eye Expression Interface
        /// <summary>
        /// Enables or disables the Eye Expression feature.
        /// </summary>
        /// <param name="active">True for enable.</param>
        public static void ActivateEyeExpression(bool active)
        {
            WaveXRSettings settings = WaveXRSettings.GetInstance();
            if (settings != null)
            {
                // Check current Wave XR Eye Expression status before activation.
                settings.EnableEyeExpression = IsEyeExpressionAvailable();

                string caller = "TBD";
                var frame = new StackFrame(1, true);
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                        caller = method.Name;
                    else
                        caller = "No method.";
                }
                if (settings.EnableEyeExpression != active)
                {
                    settings.EnableEyeExpression = active;
                    DEBUG("ActivateEyeExpression() " + (settings.EnableEyeExpression ? "Activate." : "Deactivate.") + " from " + caller);
                    SettingsHelper.SetBool(WaveXRSettings.EnableEyeExpressionText, settings.EnableEyeExpression);
                }
                else
                {
                    DEBUG("ActivateEyeExpression() Eye Expression is already " + (settings.EnableEyeExpression ? "enabled." : "disabled.") + " from " + caller);
                }
            }
        }

        /// <summary>
        /// Checks if the Eye Expression feature is available currently.
        /// </summary>
        /// <returns>True for available.</returns>
        public static bool IsEyeExpressionAvailable()
        {
            bool enabled = false;
            SettingsHelper.GetBool(kIsEyeExpressionAvailable, ref enabled);
            return enabled;
        }

        static bool m_HasEyeExpressionValue = false;
        /// <summary>
        /// Checks if the Eye Expression value is provided in runtime.
        /// </summary>
        /// <returns>True for value provided.</returns>
        public static bool HasEyeExpressionValue()
        {
            UpdateExpressionValues();
            return m_HasEyeExpressionValue;
        }

        /// <summary>
        /// Retrieves the value of an <see cref="Expressions">eye expression</see>.
        /// </summary>
        /// <param name="exp">An <see cref="Expressions">eye expression</see>.</param>
        /// <returns>A float value.</returns>
        public static float GetEyeExpressionValue(Expressions exp)
        {
            UpdateExpressionValues();
            return s_ExpValues[exp.Id()];
        }

        /// <summary>
        /// Retrieves all Eye Expression values in a float array. The values are sorted in the order of <see cref="Expressions">Eye Expressions</see>.
        /// </summary>
        /// <param name="values">All **Eye Expression** values in a float array</param>
        /// <returns>True for valid output value.</returns>
        public static bool GetEyeExpressionValues(out float[] values)
        {
            UpdateExpressionValues();
            values = s_ExpValues;
            return HasEyeExpressionValue();
        }

        static float m_EyeExpFrame = 0;
        static void UpdateExpressionValues()
        {
            if (m_EyeExpFrame == Time.frameCount) { return; }
            m_EyeExpFrame = Time.frameCount;

            SettingsHelper.GetBool(kHasEyeExpressionValue, ref m_HasEyeExpressionValue);

            for (int i = 0; i < s_EyeExpressions.Length; i++)
            {
                if (s_EyeExpressions[i] == Expressions.MAX) { continue; }

                if (m_HasEyeExpressionValue)
                    SettingsHelper.GetFloat(s_ExpKeys[i], ref s_ExpValues[i]);
                else
                    s_ExpValues[i] = 0;
            }
        }
        #endregion
    }
}