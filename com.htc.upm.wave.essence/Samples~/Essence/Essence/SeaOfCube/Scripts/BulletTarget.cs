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

public class BulletTarget : MonoBehaviour {
	Rigidbody rb;
	float timeStart = 0;

	public void collisionEnter(Collision collision)
	{
		if (Mathf.Abs(collision.impulse.magnitude) < 0.1f)
			return;
		if (rb == null)
			rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
			rb.mass = 1;
			rb.drag = 0.8f;
			rb.angularDrag = 0.8f;
			rb.useGravity = false;
			timeStart = 0;
			rb.AddForceAtPosition(-collision.impulse, collision.contacts[0].point, ForceMode.Impulse);
			rb.AddForce(Physics.gravity * 0.1f, ForceMode.Acceleration);
		}
	}

	void FixedUpdate()
	{
		if (rb)
		{
			if (rb.velocity.magnitude < 0.001f && rb.angularVelocity.magnitude < 0.001f)
			{
				if (timeStart == 0)
				{
					timeStart = Time.time;
					return;
				}
				else if (Time.time - timeStart > 2)
				{
					timeStart = 0;
					var temp = rb;
					rb = null;
					Destroy(temp);
					Destroy(this);
				}
			}
		}
	}
}
