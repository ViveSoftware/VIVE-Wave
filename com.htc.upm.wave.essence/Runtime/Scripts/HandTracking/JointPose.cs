// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.Native;

namespace Wave.Essence.Hand
{
	public class JointPose : MonoBehaviour
	{
		#region Customized Settings
		[SerializeField]
		private bool m_IsLeft = false;
		public bool IsLeft { get { return m_IsLeft; } set { m_IsLeft = value; } }

		/*[SerializeField]
		private HandManager.TrackerType m_Tracker = HandManager.TrackerType.Electronic;
		public HandManager.TrackerType Tracker { get { return m_Tracker; } set { m_Tracker = value; } }*/

		[SerializeField]
		private HandManager.HandJoint m_Joint = HandManager.HandJoint.Palm;
		public HandManager.HandJoint Joint { get { return m_Joint; } set { m_Joint = value; } }

		[SerializeField]
		private bool m_HideWhenPoseInvalid = true;
		public bool HideWhenPoseInvalid { get { return m_HideWhenPoseInvalid; } set { m_HideWhenPoseInvalid = value; } }
		#endregion

		private MeshRenderer m_MeshRenderer = null;

		#region Joint Interface
		private Vector3 m_Position = Vector3.zero;
		public Vector3 Position {
			get {
				if (HandManager.Instance == null)
					return Vector3.zero;

				HandManager.Instance.GetJointPosition(/*m_Tracker, */m_Joint, ref m_Position, m_IsLeft);
				return m_Position;
			}
		}

		private Quaternion m_Rotation = Quaternion.identity;
		public Quaternion Rotation {
			get {
				if (HandManager.Instance == null)
					return Quaternion.identity;

				HandManager.Instance.GetJointRotation(/*m_Tracker, */m_Joint, ref m_Rotation, m_IsLeft);
				return m_Rotation;
			}
		}

		private bool m_Valid = false;
		public bool Valid {
			get {
				if (HandManager.Instance == null)
					return false;

				m_Valid = HandManager.Instance.IsHandPoseValid(/*m_Tracker, */m_IsLeft);
				return m_Valid;
			}
		}
		#endregion

		private void Start()
		{
			m_MeshRenderer = GetComponent<MeshRenderer>();
		}

		void Update()
		{
			if (m_HideWhenPoseInvalid && m_MeshRenderer != null)
				m_MeshRenderer.enabled = Valid;

			gameObject.transform.localPosition = Position;
			//DEBUG("Update() {" + gameObject.transform.position.x.ToString() + ", " + gameObject.transform.position.y.ToString() + ", " + gameObject.transform.position.z.ToString() + ")");
		}
	}
}
