using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wave.Essence.Sample.Hand
{
	public enum HandJoint
	{
		Palm,
		Wrist,
		Thumb_Joint0,
		Thumb_Joint1,
		Thumb_Joint2,
		Thumb_Tip,
		Index_Joint0,
		Index_Joint1,
		Index_Joint2,
		Index_Joint3,
		Index_Tip,
		Middle_Joint0,
		Middle_Joint1,
		Middle_Joint2,
		Middle_Joint3,
		Middle_Tip,
		Ring_Joint0,
		Ring_Joint1,
		Ring_Joint2,
		Ring_Joint3,
		Ring_Tip,
		Pinky_Joint0,
		Pinky_Joint1,
		Pinky_Joint2,
		Pinky_Joint3,
		Pinky_Tip,
	};

	/*
		This script help apply Wave’s tracking data to many kinds of model design.  Not all model can be satisfied, but this script help you try it easily.

		Some models didn’t have all the joints, for example, lacking of these joints: Index 0, Middle 0, Ring 0, and Pinky 0.  You can mapping your model’s exist joints by this script, and ignore the lack joints.  This script may not provide a best solution but result is acceptable.  If you want a best solution, please redesign your model for Wave's tracking data.

		This script can also make any redirection transform to an joints to apply the hand tracking data of Wave.
	 */
    public class HandController : MonoBehaviour
	{
		public bool isLeft;

		InputDevice hid;
		InputDevice crid;
		InputDevice clid;

		Matrix4x4 handTransformMatrix;  // hand to tracking space, or wrist to tracking space.
		Matrix4x4 handTransformMatrixInverse;
		Transform targetRoot;

		[Tooltip("Fill Hand Forward Axis with the axis on hand (wrist joint) object as finger pointed direction.")]
		public AxisDirection handForwardAxis = AxisDirection.ZPositive;

		[Tooltip("Fill Hand Up Axis with the axis on hand (wrist joint) object as hand's back's direction.")]
		public AxisDirection handUpAxis = AxisDirection.YPositive;

		[Tooltip("In some case, the hand direction is not simply the X, Y, or Z.  " +
			"If you uncheck useSimpleDir, HandController will try to find the most possible direction from the hand object's transform.  " +
			"This is an experimental feature.  You need find the best choice for your model through test.")]
		public bool useSimpleDir = true;
		public Transform axisReference = null;

		[Tooltip("If you don't want use this component's Update(), you can call uncheck \"Do Update\" and invoke HandController.UpdatePoses() manually.")]
		public bool doUpdate = true;

		[Tooltip("Remove forearm's roll rotation from the wrist joint's rotation.  In hand tracking, the wrist implied the forearm's roll rotation.")]
		public bool removeForearmRoll = false;

		[Tooltip("Used for removeForearmRoll and restore the netural hand.  Fill the elbow's transform.")]
		public Transform elbow = null;

		[Tooltip("Used for removeForearmRoll.  Fill forearm Forward Axis with the axis on hand (wrist joint) object as finger pointed direction.")]
		public AxisDirection elbowForwardAxis = AxisDirection.ZPositive;

		[Tooltip("Used for removeForearmRoll.  Fill forearm Up Axis with the axis on hand (wrist joint) object as hand's back's direction.")]
		public AxisDirection elbowUpAxis = AxisDirection.YPositive;

        [Tooltip("If has BodyTracking, not to control the wrist.")]
        public bool hasBodyTracking = false;

		public enum RootPose
		{
			CameraRigPose,
			ArmPose,
		};

		[Tooltip("Which root pose you want to apply to the hand.  The tracking data will be based on Camera Rig.  You can try ArmPose if you have special application.")]
		public RootPose applyPose = RootPose.CameraRigPose;

		[Tooltip("Not to base on forearm's direction.  Should be checked when Apply Pose is Camera Rig Pose."), ]
		public bool notBaseOnForearm = true;

		[Tooltip("If you want to apply hand model's pose according the the camera rig.  Set the camera rig's transform here.")]
		public Transform cameraRig;

		[Tooltip("Apply hand's tracking position.")]
		public bool applyHandPosition = true;

		[Tooltip("When applyHandPosition, if applyAppPosition is true, apply all joint's tracking position, or just Wrist's.")]
		public bool applyAllPosition = false;

		// The last elbow pose in tracking space when the hand pose is still valid.
		private Pose lastElbowPoseT = new Pose();

		#region legacy
		[HideInInspector]
		public Transform bonePalm;  // 0
		[HideInInspector]
		public Transform boneWrist;  // 1
		[HideInInspector]
		public Transform boneThumb_Joint0;  // 2
		[HideInInspector]
		public Transform boneThumb_Joint1;  // 3
		[HideInInspector]
		public Transform boneThumb_Joint2;  // 4
		[HideInInspector]
		public Transform boneThumb_Tip;  // 5
		[HideInInspector]
		public Transform boneIndex_Joint0;  // 6
		[HideInInspector]
		public Transform boneIndex_Joint1;  // 7
		[HideInInspector]
		public Transform boneIndex_Joint2;  // 8
		[HideInInspector]
		public Transform boneIndex_Joint3;  // 9
		[HideInInspector]
		public Transform boneIndex_Tip;  // 10
		[HideInInspector]
		public Transform boneMiddle_Joint0;  // 11
		[HideInInspector]
		public Transform boneMiddle_Joint1;  // 12
		[HideInInspector]
		public Transform boneMiddle_Joint2;  // 13
		[HideInInspector]
		public Transform boneMiddle_Joint3;  // 14
		[HideInInspector]
		public Transform boneMiddle_Tip;  // 15
		[HideInInspector]
		public Transform boneRing_Joint0;  // 16
		[HideInInspector]
		public Transform boneRing_Joint1;  // 17
		[HideInInspector]
		public Transform boneRing_Joint2;  // 18
		[HideInInspector]
		public Transform boneRing_Joint3;  // 19
		[HideInInspector]
		public Transform boneRing_Tip;  // 20
		[HideInInspector]
		public Transform bonePinky_Joint0;  // 21
		[HideInInspector]
		public Transform bonePinky_Joint1;  // 22
		[HideInInspector]
		public Transform bonePinky_Joint2;  // 23
		[HideInInspector]
		public Transform bonePinky_Joint3;  // 24
		[HideInInspector]
		public Transform bonePinky_Tip;  // 25


		private Transform GetLegacyTransformById(int id)
		{
			switch (id)
			{
				case 0: return bonePalm;  // 0
				case 1: return boneWrist;  // 1
				case 2: return boneThumb_Joint0;  // 2
				case 3: return boneThumb_Joint1;  // 3
				case 4: return boneThumb_Joint2;  // 4
				case 5: return boneThumb_Tip;  // 5
				case 6: return boneIndex_Joint0;  // 6
				case 7: return boneIndex_Joint1;  // 7
				case 8: return boneIndex_Joint2;  // 8
				case 9: return boneIndex_Joint3;  // 9
				case 10: return boneIndex_Tip;  // 10
				case 11: return boneMiddle_Joint0;  // 11
				case 12: return boneMiddle_Joint1;  // 12
				case 13: return boneMiddle_Joint2;  // 13
				case 14: return boneMiddle_Joint3;  // 14
				case 15: return boneMiddle_Tip;  // 15
				case 16: return boneRing_Joint0;  // 16
				case 17: return boneRing_Joint1;  // 17
				case 18: return boneRing_Joint2;  // 18
				case 19: return boneRing_Joint3;  // 19
				case 20: return boneRing_Tip;  // 20
				case 21: return bonePinky_Joint0;  // 21
				case 22: return bonePinky_Joint1;  // 22
				case 23: return bonePinky_Joint2;  // 23
				case 24: return bonePinky_Joint3;  // 24
				case 25: return bonePinky_Tip;  // 25
				default: return null;
			}
		}

		private void ResetegacyTransformById(int id)
		{
			switch (id)
			{
				case 0: bonePalm = null; return;  // 0
				case 1: boneWrist = null; return;  // 1
				case 2: boneThumb_Joint0 = null; return;  // 2
				case 3: boneThumb_Joint1 = null; return;  // 3
				case 4: boneThumb_Joint2 = null; return;  // 4
				case 5: boneThumb_Tip = null; return;  // 5
				case 6: boneIndex_Joint0 = null; return;  // 6
				case 7: boneIndex_Joint1 = null; return;  // 7
				case 8: boneIndex_Joint2 = null; return;  // 8
				case 9: boneIndex_Joint3 = null; return;  // 9
				case 10: boneIndex_Tip = null; return;  // 10
				case 11: boneMiddle_Joint0 = null; return;  // 11
				case 12: boneMiddle_Joint1 = null; return;  // 12
				case 13: boneMiddle_Joint2 = null; return;  // 13
				case 14: boneMiddle_Joint3 = null; return;  // 14
				case 15: boneMiddle_Tip = null; return;  // 15
				case 16: boneRing_Joint0 = null; return;  // 16
				case 17: boneRing_Joint1 = null; return;  // 17
				case 18: boneRing_Joint2 = null; return;  // 18
				case 19: boneRing_Joint3 = null; return;  // 19
				case 20: boneRing_Tip = null; return;  // 20
				case 21: bonePinky_Joint0 = null; return;  // 21
				case 22: bonePinky_Joint1 = null; return;  // 22
				case 23: bonePinky_Joint2 = null; return;  // 23
				case 24: bonePinky_Joint3 = null; return;  // 24
				case 25: bonePinky_Tip = null; return;  // 25
				default: return;
			}
		}

		public void ProcessLegacyTransform()
		{
			bool hasLegacy = false;
			for (int i = 0; i < 26; i++)
			{
				var t = GetLegacyTransformById(i);
				if (t != null)
				{
					hasLegacy = true;
					break;
				}
			}
			if (!hasLegacy)
				return;

			Debug.Log("Has legacy HandController.");

			// Do jointDataList initialize
			if (jointDataList == null)
				jointDataList = new JointData[26];

			for (int i = 0; i < 26; i++)
			{
				if (jointDataList[i] != null && jointDataList[i].isInitialized == true)
					throw new System.Exception("Please reset this component.");
			}

			for (int i = 0; i < 26; i++)
			{
				var jd = jointDataList[i];
				if (jd == null)
					jd = jointDataList[i] = new JointData();

				if (!jd.isInitialized)
					jd.SetJointData(i);

				var t = GetLegacyTransformById(i);
				if (t != null && jd.transform == null)
					jd.transform = t;
				ResetegacyTransformById(i);
			}

#if UNITY_EDITOR
			Debug.LogWarning("Upgraded from legacy HandController.  Please save this scene.");
			EditorUtility.SetDirty(this);
#endif
		}
		#endregion

		public static readonly string[] jointNames = {
			"Palm",  // 0
			"Wrist",  // 1
			"Thumb 0",  // 2
			"Thumb 1",  // 3
			"Thumb 2",  // 4
			"Thumb Tip",  // 5
			"Index 0",  // 6
			"Index 1",  // 7
			"Index 2",  // 8
			"Index 3",  // 9
			"Index Tip",  // 10
			"Middle 0",  // 11
			"Middle 1",  // 12
			"Middle 2",  // 13
			"Middle 3",  // 14
			"Middle Tip",  // 15
			"Ring 0",  // 16
			"Ring 1",  // 17
			"Ring 2",  // 18
			"Ring 3",  // 19
			"Ring Tip",  // 20
			"Pinky 0",  // 21
			"Pinky 1",  // 22
			"Pinky 2",  // 23
			"Pinky 3",  // 24
			"Pinky Tip"  // 25
		};

		// Make sure all element in this array has instance, even not used.
		[HideInInspector]
		public JointData[] jointDataList = new JointData[26];

		public enum AxisDirection
		{
			XPositive,
			XNegative,
			YPositive,
			YNegative,
			ZPositive,
			ZNegative
		}

		public enum JointDirection
		{
			HandDirection,
			SpecificDirection
		}

		[System.Serializable]
		public class JointData
		{
			public bool isInitialized = false;
			public HandJoint joint = HandJoint.Wrist;
			public int jointId = 1;
			public Transform transform = null;
			public Transform axisReference = null;

			public bool hasPose = false;
			public Vector3 positionT;  // Tracking space in Unity's coordinate system
			public Quaternion rotationT;  // Tracking space in Unity's coordinate system
			public Vector3 positionOE;  // Original pose in Elbow space
			public Quaternion rotationOE;  // Original pose in Elbow space

			public JointDirection direction = JointDirection.HandDirection;
			public bool useSimpleDir = true;
			public AxisDirection spUp = AxisDirection.YPositive;
			public AxisDirection spForward = AxisDirection.ZPositive;

			public Matrix4x4 transformMatrix = Matrix4x4.identity;
			public Matrix4x4 transformMatrixInverse = Matrix4x4.identity;

			public void SetJointData(int jointId)
			{
				this.jointId = jointId;
				this.joint = (HandJoint)jointId;
				this.transform = null;
				positionT = Vector3.zero;
				rotationT = Quaternion.identity;

				direction = JointDirection.HandDirection;
				spUp = AxisDirection.YPositive;
				spForward = AxisDirection.ZPositive;
				hasPose = false;
				isInitialized = true;
			}
		}

		private void Awake()
		{
			ProcessLegacyTransform();
		}

		private void OnEnable()
		{
			InputDevices.deviceConnected += OnDeviceConnected;
		}

		private void OnDisable()
		{
			InputDevices.deviceConnected -= OnDeviceConnected;
		}

		void OnDeviceConnected(InputDevice dev)
		{
			UpdateInputDevice(dev);
		}

		void UpdateInputDevice(InputDevice dev)
		{
			if (!dev.isValid) return;

			InputDeviceCharacteristics chars = InputDeviceCharacteristics.None;
			chars |= isLeft ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;
			chars |= InputDeviceCharacteristics.HandTracking;

			if ((dev.characteristics & chars) == chars)
				hid = dev;

			chars = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
			if ((dev.characteristics & chars) == chars)
				clid = dev;

			chars = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
			if ((dev.characteristics & chars) == chars)
				clid = dev;
		}

		void Start()
		{
			UpdateBones();
			if (elbow != null)
			{
				lastElbowPoseT.rotation = Quaternion.Inverse(targetRoot.rotation) * elbow.rotation;
			}
		}

		Transform GetCameraRig()
		{
            if (cameraRig != null) return cameraRig;

            //if (cameraRig == null && WaveRig.Instance != null)
            //    cameraRig = WaveRig.Instance.transform;
            if (cameraRig == null && Camera.main != null)
                cameraRig = Camera.main.transform.parent;
			return cameraRig;
        }

		List<InputDevice> inputDevices = new List<InputDevice>();

		void CheckInputDevice()
		{
			if (!hid.isValid || !clid.isValid || !crid.isValid)
			{
				InputDevices.GetDevices(inputDevices);
				foreach(var dev in inputDevices)
				{
					UpdateInputDevice(dev);
				}
			}
		}

		bool IsCtrlsTracked()
		{
			bool isTracked = false;
			if (crid.isValid)
				crid.TryGetFeatureValue(CommonUsages.isTracked, out isTracked);
			if (isTracked) return true;
			if (!isTracked && clid.isValid)
				clid.TryGetFeatureValue(CommonUsages.isTracked, out isTracked);
			return isTracked;
		}

		bool IsTracked()
		{
			if (!hid.isValid) return false;
			if (hid.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
				return isTracked;
			return false;
		}

        public void UpdateBones()
		{
			if (jointDataList == null || jointDataList.Length != 26 || jointDataList[0] == null)
			{
				Debug.LogError("jointDataList is not initialized");
#if UNITY_EDITOR
				if (!Application.isEditor)
#endif
					enabled = false;
				return;
			}

			if (jointDataList[1] == null ||  jointDataList[1].transform == null)
			{
				Debug.LogError("Wrist's bone is not assigned");
#if UNITY_EDITOR
				if (!Application.isEditor)
#endif
					enabled = false;
				return;
			}

			var boneWrist = jointDataList[1].transform;

			if (applyPose == RootPose.ArmPose)
				targetRoot = boneWrist.parent;
			else
			{
				var rig = GetCameraRig();
				if (!rig)
					return;
				targetRoot = GetCameraRig();
			}

			if (axisReference == null)
				useSimpleDir = true;
			if (useSimpleDir)
				handTransformMatrix = GetTransformMatrix(handUpAxis, handForwardAxis);
			else
				handTransformMatrix = GetTransformMatrix(axisReference);
			handTransformMatrixInverse = handTransformMatrix.inverse;

			//DebugMatrix("M_h2tr" + (isLeft ? "_l" : "_r"), transformMatrix);
			//DebugMatrix("M_tr2h" + (isLeft ? "_l" : "_r"), transformMatrixInverse);

			//if (useThumbDirection && boneThumb_Joint0 != null)
			//{
			//	if (useSimpleDir)
			//		transformMatrixThumb = GetTransformMatrix(thumbUpAxis, thumbForwardAxis);
			//	else
			//		transformMatrixThumb = GetTransformMatrix(thumbUpAxis, thumbForwardAxis, boneThumb_Joint0);
			//	//DebugMatrix("M_th2tr" + (isLeft ? "_l" : "_r"), transformMatrixThumb);
			//}

			for (int i = 0; i < jointDataList.Length; i++)
			{
				Debug.Log("process joint " + i);
				var jd = jointDataList[i];
				if (jd.transform == null)
					continue;
				if (jd.direction == JointDirection.HandDirection)
					continue;
				if (jd.useSimpleDir)
					jd.transformMatrix = GetTransformMatrix(jd.spUp, jd.spForward);
				else
					jd.transformMatrix = GetTransformMatrix(jd.axisReference);
			}


			// Record elbow pose or wrist pose
			if (elbow != null)
			{
				// Elbow rotation means the forearm's rotation.
				var elbowRotInv = Quaternion.Inverse(elbow.rotation);

				// Convert the neutral hand joint poses to WVR's tracking pose, and save them.
				for (int i = 0; i < jointDataList.Length; i++)
				{
					var jd = jointDataList[i];
					if (jd.transform == null)
						continue;

					jd.rotationOE = elbowRotInv * jd.transform.rotation;
				}
			}
			else if (jointDataList[1].transform != null)
			{
                // Elbow rotation means the forearm's rotation.
                var wristRotInv = Quaternion.Inverse(jointDataList[1].transform.rotation);

                // Convert the neutral hand joint poses to WVR's tracking pose, and save them.
                for (int i = 0; i < jointDataList.Length; i++)
                {
                    var jd = jointDataList[i];
                    if (jd.transform == null)
                        continue;

                    jd.rotationOE = wristRotInv * jd.transform.rotation;
                }
            }
        }

		public Vector4 DirToVector4(AxisDirection dir)
		{
			switch (dir)
			{
				case AxisDirection.XPositive: return new Vector4(1, 0, 0, 0);
				case AxisDirection.YPositive: return new Vector4(0, 1, 0, 0);
				case AxisDirection.ZPositive: return new Vector4(0, 0, 1, 0);
				case AxisDirection.XNegative: return new Vector4(-1, 0, 0, 0);
				case AxisDirection.YNegative: return new Vector4(0, -1, 0, 0);
				case AxisDirection.ZNegative: return new Vector4(0, 0, -1, 0);
				default:
					throw new System.ArgumentOutOfRangeException("Use value in AxisDirection.");
			}
		}

		// LookAt matrix
		public Matrix4x4 GetTransformMatrix(AxisDirection upDir, AxisDirection forwardDir)
		{
			if (upDir == forwardDir)
				throw new System.ArgumentException("upDir should not equal to forwardDir");

			var up = DirToVector4(upDir);
			var forward = DirToVector4(forwardDir);
			var rightV3 = Vector3.Cross(up, forward).normalized;
			var right = new Vector4(rightV3.x, rightV3.y, rightV3.z, 0);

			Matrix4x4 m = new Matrix4x4(right, up, forward, new Vector4(0, 0, 0, 1));
			return m.transpose;
		}

		public Matrix4x4 GetTransformMatrix(Transform axisRef)
		{
			var m1 = axisRef.parent.localToWorldMatrix;
			m1[0, 3] = 0;
			m1[0, 2] = 0;
			m1[0, 1] = 0;
			var m2 = axisRef.localToWorldMatrix;
			m2[0, 3] = 0;
			m2[0, 2] = 0;
			m2[0, 1] = 0;
			var m = m1.inverse * m2;

			return m.transpose;
		}

		public Vector4 FindCorrespondVector(Vector3 expectedDir, Transform target)
		{
			Vector4[] dirs =
			{
				target.forward,
				-target.forward,
				target.up,
				-target.up,
				target.right,
				-target.right,
			};
			Vector4 minV = Vector3.zero;
			float minA = 360;
			foreach (var dir in dirs)
			{
				float a = Vector3.Angle(dir, expectedDir);
				if (a < minA)
				{
					minA = a;
					minV = dir;
				}
			}
			minV.w = 0;
			return minV;
		}

		// Find suitable axes from target transform.  Transfrom's forward may not perfectly be +Z, -X, or etc.
		public Matrix4x4 GetTransformMatrix(AxisDirection upDir, AxisDirection forwardDir, Transform target)
		{
			if (upDir == forwardDir)
				throw new System.ArgumentException("upDir should not equal to forwardDir");

			var up = FindCorrespondVector(DirToVector4(upDir), target);
			var forward = FindCorrespondVector(DirToVector4(forwardDir), target);
			var rightV3 = Vector3.Cross(up, forward).normalized;
			var right = new Vector4(rightV3.x, rightV3.y, rightV3.z, 0);

			//StringBuilder sb = new StringBuilder();
			//sb.AppendFormat("up {0:F6} {1:F6} {2:F6}", up.x, up.y, up.z).AppendLine();
			//sb.AppendFormat("forward {0:F6} {1:F6} {2:F6}", forward.x, forward.y, forward.z).AppendLine();
			//sb.AppendFormat("right {0:F6} {1:F6} {2:F6}", rightV3.x, rightV3.y, rightV3.z);
			//Debug.Log(sb.ToString());

			Matrix4x4 m = new Matrix4x4(right, up, forward, new Vector4(0, 0, 0, 1));
			return m.transpose;
		}

		float timeAccToUpdateSize = 0;

		public void UpdateSize()
		{
			timeAccToUpdateSize += Time.unscaledDeltaTime;
			if (timeAccToUpdateSize > 1)
			{
				timeAccToUpdateSize = 0;
			}
		}

		float timeAccForInvalidPose = -0.1f;

		private void UpdateValidPoses()
		{
			var rootRot = Matrix4x4.Rotate(targetRoot.rotation);
			var rootL2W = targetRoot.localToWorldMatrix;

			for (int i = 0; i < jointDataList.Length; i++)
			{
				var jd = jointDataList[i];
				if (jd.transform == null)
					continue;

				if (!jd.hasPose) continue;

				if (hasBodyTracking && jd.joint == HandJoint.Wrist)
					continue;

				Matrix4x4 rotMat = Matrix4x4.Rotate(jd.rotationT);

				if (jd.direction == JointDirection.HandDirection)
				{
					// Common process
					if (notBaseOnForearm)
						jd.transform.rotation = (rootRot * (rotMat * handTransformMatrix)).rotation;
					else
						jd.transform.rotation = (rootRot * (handTransformMatrixInverse * rotMat * handTransformMatrix)).rotation;
				}
				else if (jd.direction == JointDirection.SpecificDirection)
				{
					// Special process
					if (notBaseOnForearm)
						jd.transform.rotation = (rootRot * rotMat * jd.transformMatrix).rotation;
					else
						jd.transform.rotation = (rootRot * handTransformMatrixInverse * rotMat * jd.transformMatrix).rotation;
				}

				if (applyHandPosition)
				{
					if (applyAllPosition || jd.joint == HandJoint.Wrist)
					{
						jd.transform.position = rootL2W * new Vector4(jd.positionT.x, jd.positionT.y, jd.positionT.z, 1);
					}
				}
			}
		}

		private void RestoreNeutralPoseElbow(float progress)
		{
			if (elbow == null) return;

			// if no valid pose, let the hand poses become neutral.
			Quaternion lastElbowRotTInv = Quaternion.Inverse(lastElbowPoseT.rotation);
			Quaternion elbowRot = elbow.rotation;
			Quaternion htmRot = handTransformMatrix.rotation;
			Quaternion htmRotInv = handTransformMatrixInverse.rotation;

			for (int i = 0; i < jointDataList.Length; i++)
			{
				var jd = jointDataList[i];
				if (jd.transform == null)
					continue;

				if (hasBodyTracking && jd.joint == HandJoint.Wrist)
					continue;

				// Get axis corrected rotationT
				Quaternion rotationT = Quaternion.identity;
				if (jd.direction == JointDirection.HandDirection)
				{
					// Common process
					if (notBaseOnForearm)
						rotationT = jd.rotationT * htmRot;
					else
						rotationT = htmRotInv * jd.rotationT * htmRot;
				}
				else if (jd.direction == JointDirection.SpecificDirection)
				{
					// Special process
					if (notBaseOnForearm)
						rotationT = jd.rotationT * jd.transformMatrix.rotation;
					else
						rotationT = htmRotInv * jd.rotationT * jd.transformMatrix.rotation;
				}

				// the local rotation in Elbow space
				var lastRotE = lastElbowRotTInv * rotationT;
				// jd.rotationOF has the original rotation of the joint in forearm space with corrected axes.
				Quaternion currentLocalRot = Quaternion.Slerp(lastRotE, jd.rotationOE, progress);
				// Based on current elbow
				jd.transform.rotation = elbowRot * currentLocalRot;
			}
		}

		private void RestoreNeutralPoseWrist(float progress)
		{
			var wrist = jointDataList[1];
			if (wrist.transform == null) return;

			// if no valid pose, let the hand poses become neutral.
			Quaternion lastWristRotTInv = Quaternion.Inverse(wrist.transform.rotation);
			Quaternion wristRot = wrist.transform.rotation;
			Quaternion htmRot = handTransformMatrix.rotation;
			Quaternion htmRotInv = handTransformMatrixInverse.rotation;

			for (int i = 0; i < jointDataList.Length; i++)
			{
				var jd = jointDataList[i];
				if (jd.transform == null)
					continue;

				if (hasBodyTracking && jd.joint == HandJoint.Wrist)
					continue;

				// Get axis corrected rotationT
				Quaternion rotationT = Quaternion.identity;
				if (jd.direction == JointDirection.HandDirection)
				{
					// Common process
					if (notBaseOnForearm)
						rotationT = jd.rotationT * htmRot;
					else
						rotationT = htmRotInv * jd.rotationT * htmRot;
				}
				else if (jd.direction == JointDirection.SpecificDirection)
				{
					// Special process
					if (notBaseOnForearm)
						rotationT = jd.rotationT * jd.transformMatrix.rotation;
					else
						rotationT = htmRotInv * jd.rotationT * jd.transformMatrix.rotation;
				}

				// the local rotation in Elbow space
				var lastRotE = lastWristRotTInv * rotationT;
				// jd.rotationOF has the original rotation of the joint in forearm space with corrected axes.
				Quaternion currentLocalRot = Quaternion.Slerp(lastRotE, jd.rotationOE, progress);
				// Based on current elbow
				jd.transform.rotation = wristRot * currentLocalRot;
			}
		}

		private void RestoreNeutralPose(float progress, bool hasCtrl = false)
		{
			if (progress > 1) return;

			if (hasCtrl)
			{
				// Prior not to control the wrist
				if (hasBodyTracking)
					RestoreNeutralPoseWrist(progress);
				else if (elbow != null)
					RestoreNeutralPoseElbow(progress);
				return;
			}

			if (elbow != null)
				RestoreNeutralPoseElbow(progress);
			else
				RestoreNeutralPoseWrist(progress);
		}

		List<UnityEngine.XR.Bone>[] fingerBones = new List<UnityEngine.XR.Bone>[]
		{
			new List<Bone>(),  // Thumb
			new List<Bone>(),  // Index
			new List<Bone>(),  // Middle
			new List<Bone>(),  // Ring
			new List<Bone>(),  // Pinky
		};

		bool isCtrlsTrackedLastFrame = false;

		public void UpdatePoses()
		{
			// The pose can be invalid if and only if pick up controllers.
			CheckInputDevice();
			bool isValid = hid.isValid;
			bool isTracked = IsTracked();
			bool isCtrlsTracked = !isTracked && IsCtrlsTracked();
			bool needUpdatePose = isValid && isTracked;
			//Debug.Log($"Quaker isValid={isValid} isTracked={isTracked} isCtrlsTracked={isCtrlsTracked}");

			UpdateSize();

			// Get poses
			if (isTracked) {
				hid.TryGetFeatureValue(CommonUsages.handData, out UnityEngine.XR.Hand handData);
				bool ret = false;
				ret |= handData.TryGetRootBone(out UnityEngine.XR.Bone wristBone);
				ret |= handData.TryGetFingerBones(HandFinger.Thumb,  fingerBones[0]);
				ret |= handData.TryGetFingerBones(HandFinger.Index,  fingerBones[1]);
				ret |= handData.TryGetFingerBones(HandFinger.Middle, fingerBones[2]);
				ret |= handData.TryGetFingerBones(HandFinger.Ring,   fingerBones[3]);
				ret |= handData.TryGetFingerBones(HandFinger.Pinky,  fingerBones[4]);
				if (!ret)
					Debug.LogError("Failed to get finger bones.");

				for (int i = 0; i < 26; i++)
					jointDataList[i].hasPose = false;

				// Skip Palm (0) and fingers' tips (5, 10, 15, 20, 25)

				// Wrist
				jointDataList[1].hasPose |= wristBone.TryGetPosition(out jointDataList[1].positionT);
				jointDataList[1].hasPose |= wristBone.TryGetRotation(out jointDataList[1].rotationT);

				// Thumb
				jointDataList[2].hasPose |= fingerBones[0][0].TryGetPosition(out jointDataList[2].positionT);
				jointDataList[2].hasPose |= fingerBones[0][0].TryGetRotation(out jointDataList[2].rotationT);
				jointDataList[3].hasPose |= fingerBones[0][1].TryGetPosition(out jointDataList[3].positionT);
				jointDataList[3].hasPose |= fingerBones[0][1].TryGetRotation(out jointDataList[3].rotationT);
				jointDataList[4].hasPose |= fingerBones[0][2].TryGetPosition(out jointDataList[4].positionT);
				jointDataList[4].hasPose |= fingerBones[0][2].TryGetRotation(out jointDataList[4].rotationT);

				// Index
				jointDataList[6].hasPose |= fingerBones[1][0].TryGetPosition(out jointDataList[6].positionT);
				jointDataList[6].hasPose |= fingerBones[1][0].TryGetRotation(out jointDataList[6].rotationT);
				jointDataList[7].hasPose |= fingerBones[1][1].TryGetPosition(out jointDataList[7].positionT);
				jointDataList[7].hasPose |= fingerBones[1][1].TryGetRotation(out jointDataList[7].rotationT);
				jointDataList[8].hasPose |= fingerBones[1][2].TryGetPosition(out jointDataList[8].positionT);
				jointDataList[8].hasPose |= fingerBones[1][2].TryGetRotation(out jointDataList[8].rotationT);
				jointDataList[9].hasPose |= fingerBones[1][3].TryGetPosition(out jointDataList[9].positionT);
				jointDataList[9].hasPose |= fingerBones[1][3].TryGetRotation(out jointDataList[9].rotationT);

				// Middle
				jointDataList[11].hasPose |= fingerBones[2][0].TryGetPosition(out jointDataList[11].positionT);
				jointDataList[11].hasPose |= fingerBones[2][0].TryGetRotation(out jointDataList[11].rotationT);
				jointDataList[12].hasPose |= fingerBones[2][1].TryGetPosition(out jointDataList[12].positionT);
				jointDataList[12].hasPose |= fingerBones[2][1].TryGetRotation(out jointDataList[12].rotationT);
				jointDataList[13].hasPose |= fingerBones[2][2].TryGetPosition(out jointDataList[13].positionT);
				jointDataList[13].hasPose |= fingerBones[2][2].TryGetRotation(out jointDataList[13].rotationT);
				jointDataList[14].hasPose |= fingerBones[2][3].TryGetPosition(out jointDataList[14].positionT);
				jointDataList[14].hasPose |= fingerBones[2][3].TryGetRotation(out jointDataList[14].rotationT);

				// Ring
				jointDataList[16].hasPose |= fingerBones[3][0].TryGetPosition(out jointDataList[16].positionT);
				jointDataList[16].hasPose |= fingerBones[3][0].TryGetRotation(out jointDataList[16].rotationT);
				jointDataList[17].hasPose |= fingerBones[3][1].TryGetPosition(out jointDataList[17].positionT);
				jointDataList[17].hasPose |= fingerBones[3][1].TryGetRotation(out jointDataList[17].rotationT);
				jointDataList[18].hasPose |= fingerBones[3][2].TryGetPosition(out jointDataList[18].positionT);
				jointDataList[18].hasPose |= fingerBones[3][2].TryGetRotation(out jointDataList[18].rotationT);
				jointDataList[19].hasPose |= fingerBones[3][3].TryGetPosition(out jointDataList[19].positionT);
				jointDataList[19].hasPose |= fingerBones[3][3].TryGetRotation(out jointDataList[19].rotationT);

				// Pinky
				jointDataList[21].hasPose |= fingerBones[4][0].TryGetPosition(out jointDataList[21].positionT);
				jointDataList[21].hasPose |= fingerBones[4][0].TryGetRotation(out jointDataList[21].rotationT);
				jointDataList[22].hasPose |= fingerBones[4][1].TryGetPosition(out jointDataList[22].positionT);
				jointDataList[22].hasPose |= fingerBones[4][1].TryGetRotation(out jointDataList[22].rotationT);
				jointDataList[23].hasPose |= fingerBones[4][2].TryGetPosition(out jointDataList[23].positionT);
				jointDataList[23].hasPose |= fingerBones[4][2].TryGetRotation(out jointDataList[23].rotationT);
				jointDataList[24].hasPose |= fingerBones[4][3].TryGetPosition(out jointDataList[24].positionT);
				jointDataList[24].hasPose |= fingerBones[4][3].TryGetRotation(out jointDataList[24].rotationT);
			}

			if (!isTracked)
				timeAccForInvalidPose += Time.unscaledDeltaTime * 1.7f;  // TODO: Let time scale can be customized.
			else
				timeAccForInvalidPose = -0.5f;  // User can really keep the same pose for a while...

			if (!isTracked)
			{
				// Controller show up
				if (isCtrlsTracked)
				{
					// Only do restore once
					if (!isCtrlsTrackedLastFrame)
						RestoreNeutralPose(1, true);
				}
				// Just lost tracking
				else
				{
					RestoreNeutralPose(timeAccForInvalidPose);
				}
			}
			else
				UpdateValidPoses();

			// Only update the last pose when the pose is valid.
			if (elbow != null && isTracked)
			{
				// Get the tracking space elbow pose.
				lastElbowPoseT.rotation = Quaternion.Inverse(targetRoot.rotation) * elbow.rotation;
			}

			isCtrlsTrackedLastFrame = isCtrlsTracked;
		}


        private void Update()
		{
			if (!doUpdate)
				return;
			UpdatePoses();
		}

		//private static void DebugMatrix(string name, Matrix4x4 m, bool show = true)
		//{
		//	if (!show) return;
		//	StringBuilder sb = new StringBuilder(160);
		//	sb.AppendFormat("Matrix {0,-16}", name).AppendLine();
		//	sb.AppendFormat("/ {0:F6} {1:F6} {2:F6} {3:F6} \\", m.m00, m.m01, m.m02, m.m03).AppendLine();
		//	sb.AppendFormat("| {0:F6} {1:F6} {2:F6} {3:F6} |", m.m10, m.m11, m.m12, m.m13).AppendLine();
		//	sb.AppendFormat("| {0:F6} {1:F6} {2:F6} {3:F6} |", m.m20, m.m21, m.m22, m.m23).AppendLine();
		//	sb.AppendFormat("\\ {0:F6} {1:F6} {2:F6} {3:F6} /", m.m30, m.m31, m.m32, m.m33);
		//	Debug.Log(sb.ToString());
		//}
	}


#if UNITY_EDITOR
	[CustomEditor(typeof(HandController))]
	public class HandControlerEditor : UnityEditor.Editor
	{
		static bool foldoutJointData = false;
		static bool foldoutTransforms = true;

		static readonly GUIContent spContent = new GUIContent(
			"spDir",
			"A joint if not set Specific Direction will default apply the " +
			"transform which is made by hand's forward and up direction.  " + 
			"Set Specific Direction if this joint has different define to the hand.");

		public void LayoutJointData(HandController.JointData jointData)
		{
			if (jointData.transform == null)
				return;

			GUILayout.Label(HandController.jointNames[jointData.jointId]);
			EditorGUI.indentLevel++;
			jointData.direction = (HandController.JointDirection)EditorGUILayout.EnumPopup(spContent, jointData.direction);
			//jointData.jointId = EditorGUILayout.IntField("id", jointData.jointId);
			//jointData.joint = (HandManager.HandJoint)EditorGUILayout.EnumPopup("joint", jointData.joint);
			if (jointData.direction == HandController.JointDirection.SpecificDirection)
			{
				jointData.useSimpleDir = EditorGUILayout.Toggle("Use simple dir", jointData.useSimpleDir);
				if (jointData.useSimpleDir)
				{
					if (jointData.spForward == jointData.spUp)
						EditorGUILayout.HelpBox("Forward dir should not equal to Up dir.", MessageType.Error);
					jointData.spForward = (HandController.AxisDirection)EditorGUILayout.EnumPopup("Forward dir", jointData.spForward);
					jointData.spUp = (HandController.AxisDirection)EditorGUILayout.EnumPopup("Up dir", jointData.spUp);
				}
				else
				{
					if (jointData.axisReference == null)
					{
						// Find child with name
						jointData.axisReference = jointData.transform.Find("HC_AxisRef");
					}
					if (jointData.axisReference == null)
					{
						if (GUILayout.Button("Create axis reference obj"))
						{
							var obj = new GameObject("HC_AxisRef");
							obj.transform.SetParent(jointData.transform, false);
							jointData.axisReference = obj.transform;
						}
					}
					if (jointData.axisReference != null)
						jointData.axisReference = (Transform)EditorGUILayout.ObjectField("Axis Reference", jointData.axisReference, typeof(Transform), true);
				}
			}
			EditorGUI.indentLevel--;
		}

		Animator AnimatorReference;

		public override void OnInspectorGUI()
		{
			HandController handController = (HandController)target;
			base.OnInspectorGUI();
			const int N = 26;

			if (handController.applyPose == HandController.RootPose.CameraRigPose && handController.cameraRig == null)
				EditorGUILayout.HelpBox("CameraRig does not set.  Will search CameraRig in runtime", MessageType.Warning);

			if (handController.handForwardAxis == handController.handUpAxis)
				EditorGUILayout.HelpBox("Hand Forward Axis should not equal to Hand Up Axis.", MessageType.Error);

			if (!handController.useSimpleDir && handController.axisReference == null)
				EditorGUILayout.HelpBox("handController does not set.", MessageType.Error);

			handController.ProcessLegacyTransform();

			if (handController.jointDataList == null)
			{
				handController.jointDataList = new HandController.JointData[N];
			}
			for (int i = 0; i < N; i++)
			{
				var jd = handController.jointDataList[i];
				if (jd == null)
					jd = handController.jointDataList[i] = new HandController.JointData();
				if (!jd.isInitialized)
					jd.SetJointData(i);
			}

			if (handController.jointDataList[1].transform == null)
				EditorGUILayout.HelpBox("Wrist's transform field must not null.", MessageType.Error);

			if (foldoutTransforms = EditorGUILayout.Foldout(foldoutTransforms, "Joints Transforms"))
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < N; i++)
				{
					var jd = handController.jointDataList[i];
					jd.transform = (Transform)EditorGUILayout.ObjectField(
						HandController.jointNames[jd.jointId], jd.transform, typeof(Transform), true);
				}
				EditorGUI.indentLevel--;
			}

			if (foldoutJointData = EditorGUILayout.Foldout(foldoutJointData, "Joint data"))
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < N; i++)
				{
					var jd = handController.jointDataList[i];
					LayoutJointData(jd);
				}
				EditorGUI.indentLevel--;
			}

			
			HandController script = (HandController)target;
			AnimatorReference = (Animator)EditorGUILayout.ObjectField(label: "Reference", AnimatorReference, typeof(Animator), true);
			
			if (GUILayout.Button("Update", GUILayout.Height(40)))
			{
				Debug.Log("Modify Root!");
				Transform tmp;
				if (script.isLeft)
				{
					#region Left
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftHand);
					if (tmp == null)
						Debug.LogError("tmp is null. Drag the animator to AnimatorReference.  If still no use, try Create UnityEngine.Avatar button of humanoid.");
					if (tmp)
					{
						script.jointDataList[1].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
					if (tmp)
					{
						script.jointDataList[2].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
					if (tmp)
					{
						script.jointDataList[3].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
					if (tmp)
					{
						script.jointDataList[4].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
					if (tmp)
					{
						script.jointDataList[7].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
					if (tmp)
					{
						script.jointDataList[8].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
					if (tmp)
					{
						script.jointDataList[9].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
					if (tmp)
					{
						script.jointDataList[12].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
					if (tmp)
					{
						script.jointDataList[13].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
					if (tmp)
					{
						script.jointDataList[14].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftRingProximal);
					if (tmp)
					{
						script.jointDataList[17].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
					if (tmp)
					{
						script.jointDataList[18].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftRingDistal);
					if (tmp)
					{
						script.jointDataList[19].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
					if (tmp)
					{
						script.jointDataList[22].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
					if (tmp)
					{
						script.jointDataList[23].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
					if (tmp)
					{
						script.jointDataList[24].transform = tmp;
					}
					#endregion
				}
				else
				{
					#region Right
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightHand);
					if (tmp)
					{
						script.jointDataList[1].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightThumbProximal);
					if (tmp)
					{
						script.jointDataList[2].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
					if (tmp)
					{
						script.jointDataList[3].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightThumbDistal);
					if (tmp)
					{
						script.jointDataList[4].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightIndexProximal);
					if (tmp)
					{
						script.jointDataList[7].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
					if (tmp)
					{
						script.jointDataList[8].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightIndexDistal);
					if (tmp)
					{
						script.jointDataList[9].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
					if (tmp)
					{
						script.jointDataList[12].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
					if (tmp)
					{
						script.jointDataList[13].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
					if (tmp)
					{
						script.jointDataList[14].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightRingProximal);
					if (tmp)
					{
						script.jointDataList[17].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
					if (tmp)
					{
						script.jointDataList[18].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightRingDistal);
					if (tmp)
					{
						script.jointDataList[19].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightLittleProximal);
					if (tmp)
					{
						script.jointDataList[22].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
					if (tmp)
					{
						script.jointDataList[23].transform = tmp;
					}
					tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightLittleDistal);
					if (tmp)
					{
						script.jointDataList[24].transform = tmp;
					}
					#endregion
				}
			}

			if (GUI.changed)
				EditorUtility.SetDirty(target);

			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}
