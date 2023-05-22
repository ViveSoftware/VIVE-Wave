// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;

public class AimTheHead : MonoBehaviour
{
	void Update()
	{
		Camera head = Camera.main;
		if (head == null)
			return;
		var dir = (head.transform.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(dir);
	}
}