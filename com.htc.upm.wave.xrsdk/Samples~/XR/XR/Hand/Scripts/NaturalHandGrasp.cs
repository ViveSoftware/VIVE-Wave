using UnityEngine;
using UnityEngine.XR;
using Wave.OpenXR;

namespace Wave.XR.Sample.Hand
{
	[DisallowMultipleComponent]
	public class NaturalHandGrasp : MonoBehaviour
	{
		[SerializeField]
		private bool m_IsLeft = false;
		public bool IsLeft { get { return m_IsLeft; } set { m_IsLeft = value; } }

		private Vector3 palmPosition = Vector3.zero;
		private Quaternion palmRotation = Quaternion.identity;
		private GameObject graspObject = null;
		private Vector3 graspObjectPosition = Vector3.zero;
		private Quaternion graspObjectRoation = Quaternion.identity;

		private void Update()
		{
			UpdateHandJoint();
			if ((graspObject || IsHandTouchCollider()) && (InputDeviceHand.IsHandGrasping(IsLeft)))
			{
				Vector3 pos = palmPosition - graspObjectPosition;
				Quaternion rot = graspObjectRoation * palmRotation;
				graspObject.transform.SetPositionAndRotation(pos, rot);
				graspObject.transform.LookAt(palmPosition);
			}
			else
			{
				graspObject = null;
			}
		}

		private void UpdateHandJoint()
		{
			Bone palmBone = InputDeviceHand.GetPalm(IsLeft);
			palmBone.TryGetPosition(out palmPosition);
			palmBone.TryGetRotation(out palmRotation);
		}

		private bool IsHandTouchCollider()
		{
			Vector3 center = palmPosition + new Vector3(palmPosition.x, 0.0f, palmPosition.z) / 10;
			Collider[] hitColliders = Physics.OverlapSphere(palmPosition, 0.05f);
			foreach (var hitCollider in hitColliders)
			{
				if (hitCollider.name == "SphereR" || hitCollider.name == "SphereL")
				{
					graspObject = hitCollider.gameObject;
					graspObjectPosition = palmPosition - graspObject.transform.position;
					graspObjectRoation = Quaternion.Inverse(graspObjectRoation);
					return true;
				}
			}
			return false;
		}
	}
}
