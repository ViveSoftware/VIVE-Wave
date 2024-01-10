// "Wave SDK 
// © 2023 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Wave.OpenXR;

namespace Wave.XR.Sample.Hand
{
	public class JointPose : MonoBehaviour
	{
		public enum HandJoint
		{
			Wrist = 0,
			Palm = 1,
			Thumb = 2,
			Index = 3,
			Middle = 4,
			Ring = 5,
			Pinky = 6
		}

		#region Customized Settings
		[SerializeField]
		private bool m_IsLeft = false;
		public bool IsLeft { get { return m_IsLeft; } set { m_IsLeft = value; } }

		[SerializeField]
		private HandJoint m_Joint;
		public HandJoint Joint { get { return m_Joint; } set { m_Joint = value; } }

		[SerializeField]
		private List<Transform> m_JointPose;
		public List<Transform> JointsPose { get { return m_JointPose; } set { m_JointPose = value; } }

		[SerializeField]
		private bool m_HideWhenPoseInvalid = true;
		public bool HideWhenPoseInvalid { get { return m_HideWhenPoseInvalid; } set { m_HideWhenPoseInvalid = value; } }
		#endregion

		private List<MeshRenderer> m_MeshRenderers = new List<MeshRenderer>();

		private void Start()
		{
			foreach (Transform jointPose in m_JointPose)
			{
				m_MeshRenderers.Add(jointPose.GetComponent<MeshRenderer>());
			}
		}

		void Update()
		{
			bool isTracked = InputDeviceHand.IsTracked(m_IsLeft);
			if (m_HideWhenPoseInvalid)
			{
				foreach (MeshRenderer mesh in m_MeshRenderers)
				{
					if (mesh != null) { mesh.enabled = isTracked; }
				}
			}

			if (isTracked)
			{
				List<Bone> bones = new List<Bone>();
				switch (m_Joint)
				{
					case HandJoint.Wrist:
						bones.Add(InputDeviceHand.GetWrist(IsLeft));
						break;
					case HandJoint.Palm:
						bones.Add(InputDeviceHand.GetPalm(IsLeft));
						break;
					case HandJoint.Thumb:
						bones = InputDeviceHand.GetFingerBones(IsLeft, HandFinger.Thumb);
						break;
					case HandJoint.Index:
						bones = InputDeviceHand.GetFingerBones(IsLeft, HandFinger.Index);
						break;
					case HandJoint.Middle:
						bones = InputDeviceHand.GetFingerBones(IsLeft, HandFinger.Middle);
						break;
					case HandJoint.Ring:
						bones = InputDeviceHand.GetFingerBones(IsLeft, HandFinger.Ring);
						break;
					case HandJoint.Pinky:
						bones = InputDeviceHand.GetFingerBones(IsLeft, HandFinger.Pinky);
						break;
				}

				for (int i = 0; i < bones.Count; i++)
				{
					if (m_JointPose[i] != null)
					{
						bones[i].TryGetPosition(out Vector3 position);
						bones[i].TryGetRotation(out Quaternion rotation);
						m_JointPose[i].SetPositionAndRotation(position, rotation);
					}
				}
			}
		}
	}
}
