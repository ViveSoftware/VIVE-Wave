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

public class BulletGenerator : MonoBehaviour {
	public GameObject bulletPrefab = null;
	public float period = 0.2f;
	public float speed = 15f;  // In m/s
	private float time;
	
	// Update is called once per frame
	void Update () {
		trigger();
	}

	void trigger()
	{
		if (Time.time - time < period)
			return;
		time = Time.time;
		GameObject inst = null;
		if (bulletPrefab != null)
			inst = Instantiate(bulletPrefab);
		var root = Camera.main.transform;
		inst.transform.position = root.position + root.forward * 0.6f - root.up * 0.2f;
		inst.transform.rotation = root.rotation;
		if (inst.GetComponent<Bullet>() == null)
			inst.AddComponent<Bullet>();
		var rigid = inst.GetComponent<Rigidbody>();
		if (rigid == null)
			return;
		rigid.velocity = root.forward * speed;
	}
}
