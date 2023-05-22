// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneratorAutoAim : MonoBehaviour
{
	public GameObject target = null;
	public GameObject bulletPrefab = null;
	GameObject trashCan = null;

	public float period = 0.2f;
	private float time;

	//public Vector3 sourceInTrajectorySpace;
	//public Vector3 targetInTrajectorySpace;

	private void Start()
	{
		if (trashCan == null)
			trashCan = new GameObject("TrashCan");
	}
	void Update()
	{
		if (target == null)
			return;

		Shoot();
	}

	//void AimTarget()
	//{
	//	var direction = target.transform.position - transform.position;
	//	var targetDirInHorizontal = new Vector3(direction.x, 0, direction.z);
	//	var lookDir = Vector3.Cross(targetDirInHorizontal, Vector3.up);

	//	var mToWorldSpace = Matrix4x4.LookAt(transform.position, transform.position - lookDir, Vector3.up);
	//	var mToTrajectorySpace = mToWorldSpace.inverse;

	//	sourceInTrajectorySpace = mToTrajectorySpace * transform.position;
	//	targetInTrajectorySpace = mToTrajectorySpace * target.transform.position;
	//}

	// https://segmentfault.com/a/1190000018336439
	public Vector3 InitVeolocity(Vector3 start, Vector3 end, float height = 10, float gravity = -9.8f)
	{
		float topY = Mathf.Max(start.y, end.y) + height;
		float d1 = topY - start.y;
		float d2 = topY - end.y;
		float g2 = 2 / -gravity;
		float t1 = Mathf.Sqrt(g2 * d1);
		float t2 = Mathf.Sqrt(g2 * d2);
		float t = t1 + t2;
		float vX = (end.x - start.x) / t;
		float vZ = (end.z - start.z) / t;
		float vY = -gravity * t1;
		Vector3 v0 = new Vector3(vX, vY, vZ);
		return v0;
	}

	void Shoot()
	{
		if (Time.time - time < period)
			return;
		time = Time.time;
		GameObject inst = null;
		if (bulletPrefab != null)
			inst = Instantiate(bulletPrefab);
		inst.transform.position = transform.position;
		inst.transform.rotation = transform.rotation;
		inst.transform.SetParent(trashCan.transform, true);
		if (inst.GetComponent<Bullet>() == null)
			inst.AddComponent<Bullet>();
		var rigid = inst.GetComponent<Rigidbody>();
		if (rigid == null)
			return;

		rigid.velocity = InitVeolocity(transform.position, target.transform.position, 0);
	}
}
