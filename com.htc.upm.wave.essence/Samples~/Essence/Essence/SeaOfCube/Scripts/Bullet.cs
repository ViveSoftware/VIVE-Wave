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

public class Bullet : MonoBehaviour {
	public float timeout = 5;
	private float time = 0;
	// Use this for initialization
	void Start () {
		time = Time.time;
	}

	// Update is called once per frame
	void Update () {
		if (Time.time - time > timeout)
			Destroy(this.gameObject);
	}
}
