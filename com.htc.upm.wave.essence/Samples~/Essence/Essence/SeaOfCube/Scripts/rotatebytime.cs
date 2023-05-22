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
using System.Collections;

public class rotatebytime : MonoBehaviour {
	// rotate 45 degree every second.
	public float AnglePerSecond = 45;
	// If set, not change every frame.
	public float ChangePerSecond = 0;
	private int count = 0;
	public bool AxisX = true;
	public bool AxisY = false;
	public bool AxisZ = true;
	float angle = 0;
	float timeAcc = 0;

	// To trigger reset in Editor, click this in inspector
	public bool reset = false;

	public bool useFixedTime = true;

	private Quaternion defaultRot;

	public void Start()
	{
		defaultRot = transform.localRotation;
	}

	public void Reset()
	{
		transform.localRotation = defaultRot;
	}

	// Update is called once per frame
	void Update() {
		if (reset) {
			reset = false;
			Reset();
		}

		float deltaTime = useFixedTime ? Time.fixedDeltaTime : Time.deltaTime;

		if (ChangePerSecond == 0)
		{
			angle += deltaTime * AnglePerSecond;
		}
		else
		{
			timeAcc += deltaTime;
			if (timeAcc >= ChangePerSecond)
			{
				angle += timeAcc * AnglePerSecond;
				timeAcc = 0;
			}
		}

		if (!AxisX && !AxisY && !AxisZ)
			return;

		angle %= 360;
		Vector3 axis = new Vector3(AxisX ? 1 : 0, AxisY ? 1 : 0, AxisZ ? 1 : 0);
		transform.localRotation = Quaternion.AngleAxis(angle, axis);
	}
}
