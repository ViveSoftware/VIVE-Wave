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

[RequireComponent(typeof(Rigidbody))]
public class ResetOrigin : MonoBehaviour {
	public GameObject origin;
	private Vector3 originalPosition;
	private Quaternion originalRotation;

	private Vector3 originalRigidbodyPosition;
	private Quaternion originalRigidbodyRotation;

	// Use this for initialization
	void Start () {
		originalPosition = origin.transform.localPosition;
		originalRotation = origin.transform.localRotation;
		var rb = GetComponent<Rigidbody>();
		originalRigidbodyPosition = rb.transform.localPosition;
		originalRigidbodyRotation = rb.transform.localRotation;
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("reset_origin"))
		{
			// reset all rotation, position, and motion.
			var rb = GetComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.transform.localPosition = originalRigidbodyPosition;
			rb.transform.localRotation = originalRigidbodyRotation;
			origin.transform.localPosition = originalPosition;
			origin.transform.localRotation = originalRotation;
			rb.isKinematic = false;
		}
	}
}
