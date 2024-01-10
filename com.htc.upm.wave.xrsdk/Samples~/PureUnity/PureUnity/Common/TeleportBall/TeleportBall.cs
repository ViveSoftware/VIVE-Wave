using System.Collections;
using UnityEngine;
using VRSStudio.Common;

namespace Wave.XR.Sample.Input
{
	[RequireComponent(typeof(Rigidbody))]
	public class TeleportBall : MonoBehaviour
	{
		[Tooltip("Must have a reset point.  If null, reset to TeleportBall's parent.")]
		public Transform initPose;
		Rigidbody rb;
		Transform rig;
		readonly Button isAwake = new Button();

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			rb.Sleep();

			if (initPose == null)
				initPose = transform.parent;
		}

		bool GetCameraYawPose(Transform target, out Vector3 pos, out Quaternion rot)
		{
			if (target == null)
			{
				pos = Vector3.zero;
				rot = Quaternion.identity;
				return false;
			}
			var euler = target.rotation.eulerAngles;
			var forwardDir = Quaternion.Euler(0, euler.y, 0);
			pos = target.position;
			rot = forwardDir;
			return true;
		}

		// Return world position and rotation
		bool GetCameraYawPose(out Vector3 pos, out Quaternion rot)
		{
			if (Camera.main != null)
				return GetCameraYawPose(Camera.main.transform, out pos, out rot);
			else
				return GetCameraYawPose(null, out pos, out rot);
		}

		// Let camera's pose teleport to hitPose.
		void DoTeleport(Vector3 hitPoint, Vector3 hitVelocity)
		{
			UpdateCameraRig();
			if (rig == null) return;

			GetCameraYawPose(out Vector3 camPosW, out Quaternion camYawRotW);

			var matCam = Matrix4x4.TRS(camPosW, camYawRotW, Camera.main.transform.lossyScale); // localToWorld
			var matRig = rig.localToWorldMatrix;

			var telPos = hitPoint + Vector3.up * 0.01f;
			Quaternion telRot = camYawRotW;

			var matTel = Matrix4x4.TRS(telPos, telRot, Vector3.one);

			// matCam = matRig * matCamL  => matCamL = matRig.inv * matCam
			// matTel = matRig' * matCamL => matRig' = matTel * matCamL.inv

			var matCamL = matRig.inverse * matCam;
			var posCamL = matCamL * new Vector4(0, 0, 0, 1);
			posCamL.y = 0;
			matCamL = Matrix4x4.TRS(posCamL, matCamL.rotation, matCamL.lossyScale);
			var matRigNew = matTel * matCamL.inverse;

			var v4 = Vector3.zero;
			v4.z = 1;
			rig.position = matRigNew * new Vector4(0, 0, 0, 1);
			var telRotEuler = matRigNew.rotation.eulerAngles;
			// Keep horizontal.  Only allow y value change.
			telRotEuler.x = 0;
			telRotEuler.z = 0;
			rig.eulerAngles = telRotEuler;
		}

		bool reset = false;
		Vector3 hitVelocity;
		Vector3 hitPoint;

		void OnCollisionEnter(Collision collision)
		{
			if (collision.collider == null || !collision.collider.CompareTag("teleportable"))
				return;
			hitPoint = collision.GetContact(0).point; //.ClosestPoint(rb.position + Vector3.up * 0.1f);
			hitVelocity = collision.relativeVelocity;

			reset = true;
		}

		private void ResetTeleportBall()
		{
			transform.position = initPose.position;
			transform.rotation = initPose.rotation;

			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.position = initPose.position;
			rb.rotation = initPose.rotation;
			rb.ResetInertiaTensor();

			rb.Sleep();
			timer.Reset();
		}

		readonly Timer timer = new Timer(3);  // Hope user don't throw it too high
		readonly Timer timerTOG = new Timer(0.2f);  // Hope user don't throw it too high

		bool teleportOnGoing = false;
		private IEnumerator DoTeleport()
		{
			if (teleportOnGoing) yield break;
			teleportOnGoing = true;
			timerTOG.Set();

			ResetTeleportBall();

			yield return null;

			rb.isKinematic = true;
			DoTeleport(hitPoint, hitVelocity);

			yield return null;

			//rb.isKinematic = false;
			rb.Sleep();

			timerTOG.Reset();
			teleportOnGoing = false;

			// Hide teleport ball in spectator.
			RecursiveAction(transform, (t) => { t.gameObject.layer = LayerMask.NameToLayer("SpectatorHidden"); });
		}


		private IEnumerator DoResetBall()
		{
			if (teleportOnGoing) yield break;
			teleportOnGoing = true;
			timerTOG.Set();

			rb.isKinematic = true;

			ResetTeleportBall();

			yield return null;
			yield return null;

			//rb.isKinematic = false;
			rb.Sleep();

			timerTOG.Reset();
			teleportOnGoing = false;

			// Hide teleport ball in spectator.
			RecursiveAction(transform, (t) => { t.gameObject.layer = LayerMask.NameToLayer("SpectatorHidden"); });
		}

		private void Update()
		{
			if (reset)
			{
				StartCoroutine(DoTeleport());
				reset = false;
				return;
			}

			if (!timerTOG.IsPaused && timerTOG.Check())
			{
				teleportOnGoing = false;
			}

			isAwake.Set(!rb.IsSleeping());
			if (isAwake.IsDown || (isAwake.IsPressed && rb.isKinematic))
			{
				timer.Set();
			}

			if (!timer.IsPaused && timer.Check())
			{
				StartCoroutine(DoResetBall());
				return;
			}

			if (rb.velocity.sqrMagnitude > 2500) // 50ms
			{
				StartCoroutine(DoResetBall());
				return;
			}
		}

		private void UpdateCameraRig()
		{
			if (rig != null) return;

			//if (rig == null && WaveRig.Instance != null)
			//    rig = WaveRig.Instance.transform;
			if (rig == null && Camera.main != null)
				rig = Camera.main.transform.parent;
		}

		void RecursiveAction(Transform root, System.Action<Transform> action)
		{
			if (root == null) return;
			action(root);

			if (root.childCount == 0)
				return;

			for (int i = 0; i < root.childCount; i++)
			{
				RecursiveAction(root.GetChild(i), action);
			}

			return;
		}

		public void OnGrabbed()
		{
			// Show object in spectator.  Let viewer known what is player doing.
			RecursiveAction(transform, (t) => { t.gameObject.layer = LayerMask.NameToLayer("Default"); });
		}

		public void OnReleased()
		{
		}
	}

}
