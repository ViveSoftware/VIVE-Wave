// "WaveVR SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.OpenXR;

namespace Wave.Essence.Tracker
{
	public class TrackerSimulatorPose : MonoBehaviour
	{
		#region Inspector
		[SerializeField]
		private TrackerId m_TrackerId = TrackerId.Tracker0;
		public TrackerId TrackerId { get { return m_TrackerId; } set { m_TrackerId = value; } }
		[SerializeField]
		private bool m_IsLeft = false;
		public bool IsLeft { get { return m_IsLeft; } set { m_IsLeft = value; } }
		#endregion

		#region ArmModel
		private readonly Vector3 CENTER_EYE_POSITION = new Vector3(0, 0.15f, 0.12f);
		// Position simulation variables.
		private Quaternion bodyRotation = Quaternion.identity;
		private Vector3 bodyDirection = Vector3.zero;
		private const float BODY_ANGLE_BOUND = 0.01f;
		private const float BODY_ANGLE_LIMITATION = 0.3f; // bound of controller angle in SPEC provided to provider.
		private uint framesOfFreeze = 0;                // if framesOfFreeze >= mFPS, means controller freezed.
		private const float FPS = 60.0f;
		private Vector3 v3ChangeArmXAxis => m_IsLeft ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
		private readonly Vector3 HEADTOELBOW_OFFSET = new Vector3(0.2f, -0.7f, 0f);
		private readonly Vector3 ELBOW_PITCH_OFFSET = new Vector3(-0.2f, 0.55f, 0.08f);
		private readonly Vector3 ELBOW_RAISE_OFFSET = new Vector3(0, 0, 0);
		private const float ELBOW_PITCH_ANGLE_MIN = 0;
		private const float ELBOW_PITCH_ANGLE_MAX = 60;
		private const float ELBOW_TO_XYPLANE_LERP_MIN = 0.45f;
		private const float ELBOW_TO_XYPLANE_LERP_MAX = 0.65f;
		private readonly Vector3 ELBOWTOWRIST_OFFSET = new Vector3(0.0f, 0.0f, 0.15f);
		private readonly Vector3 WRISTTOCONTROLLER_OFFSET = new Vector3(0.0f, 0.0f, 0.05f);
		/// controller lerp speed for smooth movement between with head position case and without head position case
		private float smoothMoveSpeed = 0.3f;
		private Vector3 controllerSimulatedPosition = Vector3.zero;
		private Vector3 currentPosition = Vector3.zero;
		private Quaternion lastRotation = Quaternion.identity;
		#endregion

		private void Update()
		{
			InputDeviceTracker.TrackerId id = (InputDeviceTracker.TrackerId)m_TrackerId;
			if (!InputDeviceTracker.IsAvailable(id) || !InputDeviceTracker.IsTracked(id))
			{
				return;
			}

			if (InputDeviceTracker.GetRotation(id, out Quaternion currentRotation))
			{
				UpdateControllerPose(currentRotation);
				currentPosition = Vector3.Lerp(currentPosition, controllerSimulatedPosition, smoothMoveSpeed);
				lastRotation = currentRotation;
				transform.SetPositionAndRotation(currentPosition, currentRotation);
			}
		}

		private void UpdateControllerPose(Quaternion currentRotation)
		{
			bodyRotation = Quaternion.identity;
			UpdateHeadAndBodyPose(currentRotation);
			ComputeControllerPose(currentRotation);
			controllerSimulatedPosition += CENTER_EYE_POSITION;
		}

		private void UpdateHeadAndBodyPose(Quaternion currentRotation)
		{
			// Determine the gaze direction horizontally.
			Vector3 gazeDirection = GetHeadForward();
			gazeDirection.y = 0;
			gazeDirection.Normalize();
			float _bodyLerpFilter = BodyRotationFilter(currentRotation);

			bodyDirection = Vector3.Slerp(bodyDirection, gazeDirection, _bodyLerpFilter);
			bodyRotation = Quaternion.FromToRotation(Vector3.forward, bodyDirection);
		}

		private float BodyRotationFilter(Quaternion currentRotation)
		{
			float _bodyLerpFilter = 0;

			Quaternion _rot_old = lastRotation;
			Quaternion _rot_new = currentRotation;
			float _rot_XY_angle_old = 0, _rot_XY_angle_new = 0;

			Vector3 _rot_forward = Vector3.zero;
			Quaternion _rot_XY_rotation = Quaternion.identity;

			_rot_forward = _rot_old * Vector3.forward;
			_rot_XY_rotation = Quaternion.FromToRotation(Vector3.forward, _rot_forward);
			_rot_XY_angle_old = Quaternion.Angle(_rot_XY_rotation, Quaternion.identity);

			_rot_forward = _rot_new * Vector3.forward;
			_rot_XY_rotation = Quaternion.FromToRotation(Vector3.forward, _rot_forward);
			_rot_XY_angle_new = Quaternion.Angle(_rot_XY_rotation, Quaternion.identity);

			float _diff_angle = _rot_XY_angle_new - _rot_XY_angle_old;
			_diff_angle = _diff_angle > 0 ? _diff_angle : -_diff_angle;

			_bodyLerpFilter = Mathf.Clamp((_diff_angle - BODY_ANGLE_BOUND) / BODY_ANGLE_LIMITATION, 0, 1.0f);

			framesOfFreeze = _bodyLerpFilter < 1.0f ? framesOfFreeze + 1 : 0;

			if (framesOfFreeze > FPS)
				_bodyLerpFilter = 0;
			return _bodyLerpFilter;
		}

		private void ComputeControllerPose(Quaternion currentRotation)
		{
			// if bodyRotation angle is θ, _inverseBodyRation is -θ
			// the operator * of Quaternion in Unity means concatenation, not multipler.
			// If quaternion qA has angle θ, quaternion qB has angle ε,
			// qA * qB will plus θ and ε which means rotating angle θ then rotating angle ε.
			// (_inverseBodyRotation * rotation of controller in world space) means angle ε subtracts angle θ.
			Quaternion _controllerRotation = Quaternion.Inverse(bodyRotation) * currentRotation;
			Vector3 _headPosition = GetHeadPosition();

			/// 1. simulated elbow offset = default elbow offset apply body rotation = body rotation (Quaternion) * elbow offset (Vector3)
			// Default left / right elbow offset.
			Vector3 _elbowOffset = Vector3.Scale(HEADTOELBOW_OFFSET, v3ChangeArmXAxis);
			// Default left / right elbow pitch offset.
			Vector3 _elbowPitchOffset = Vector3.Scale(ELBOW_PITCH_OFFSET, v3ChangeArmXAxis) + ELBOW_RAISE_OFFSET;

			// Use controller pitch to simulate elbow pitch.
			// Range from ELBOW_PITCH_ANGLE_MIN ~ ELBOW_PITCH_ANGLE_MAX.
			// The percent of pitch angle will be used to calculate the position offset.
			Vector3 _controllerForward = _controllerRotation * Vector3.forward;
			float _controllerPitch = 90.0f - Vector3.Angle(_controllerForward, Vector3.up); // 0~90
			float _controllerPitchRadio = (_controllerPitch - ELBOW_PITCH_ANGLE_MIN) / (ELBOW_PITCH_ANGLE_MAX - ELBOW_PITCH_ANGLE_MIN);
			_controllerPitchRadio = Mathf.Clamp(_controllerPitchRadio, 0.0f, 1.0f);

			// According to pitch angle percent, plus offset to elbow position.
			_elbowOffset += _elbowPitchOffset * _controllerPitchRadio;
			// Apply body rotation and head position to calculate final elbow position.
			_elbowOffset = _headPosition + bodyRotation * _elbowOffset;

			// Rotation from Z-axis to XY-plane used to simulated elbow & wrist rotation.
			Quaternion _controllerXYRotation = Quaternion.FromToRotation(Vector3.forward, _controllerForward);
			float _controllerXYRotationRadio = (Quaternion.Angle(_controllerXYRotation, Quaternion.identity)) / 180;
			// Simulate the elbow raising curve.
			float _elbowCurveLerpValue = ELBOW_TO_XYPLANE_LERP_MIN + (_controllerXYRotationRadio * (ELBOW_TO_XYPLANE_LERP_MAX - ELBOW_TO_XYPLANE_LERP_MIN));
			Quaternion _controllerXYLerpRotation = Quaternion.Lerp(Quaternion.identity, _controllerXYRotation, _elbowCurveLerpValue);


			/// 2. simulated wrist offset = default wrist offset apply elbow rotation = elbow rotation (Quaternion) * wrist offset (Vector3)
			// Default left / right wrist offset
			Vector3 _wristOffset = Vector3.Scale(ELBOWTOWRIST_OFFSET, v3ChangeArmXAxis);
			// elbow rotation + curve = wrist rotation
			// wrist rotation = controller XY rotation
			// => elbow rotation + curve = controller XY rotation
			// => elbow rotation = controller XY rotation - curve
			Quaternion _elbowRotation = bodyRotation * Quaternion.Inverse(_controllerXYLerpRotation) * _controllerXYRotation;
			// Apply elbow offset and elbow rotation to calculate final wrist position.
			_wristOffset = _elbowOffset + _elbowRotation * _wristOffset;


			/// 3. simulated controller offset = default controller offset apply wrist rotation = wrist rotation (Quat) * controller offset (V3)
			// Default left / right controller offset.
			Vector3 _controllerOffset = Vector3.Scale(WRISTTOCONTROLLER_OFFSET, v3ChangeArmXAxis);
			Quaternion _wristRotation = _controllerXYRotation;
			// Apply wrist offset and wrist rotation to calculate final controller position.
			_controllerOffset = _wristOffset + _wristRotation * _controllerOffset;

			controllerSimulatedPosition = /*bodyRotation */ _controllerOffset;
			//controllerSimulatedRotation = bodyRotation * _controllerRotation;
		}

		private Vector3 GetHeadPosition()
		{
			Vector3 hmdPosition = Vector3.zero;
			InputDeviceControl.GetPosition(InputDeviceControl.kHMDCharacteristics, out hmdPosition);
			return hmdPosition;
		}

		private Vector3 GetHeadForward()
		{
			Quaternion hmdRotation = Quaternion.identity;
			InputDeviceControl.GetRotation(InputDeviceControl.kHMDCharacteristics, out hmdRotation);
			return hmdRotation * Vector3.forward;
		}
	}
}