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

public class HammerMove : MonoBehaviour {

	[SerializeField]
	float HammerLength = 0.5f;

	[SerializeField]
	float FloorDistance = 0.3f;

	public Transform target;

	public float AngleTouchGround = 0.0f;  // in radians

	void Start()
	{
		AngleTouchGround = Mathf.Asin(FloorDistance / HammerLength);

		if (contactPointIndicator)
		{
			contactPointIndicatorOriginalPostion = contactPointIndicator.transform.position;
		}
	}

	/*
	 *         A
	 *        /|\
	 *       / | \
	 *  ____/__|__\____
	 *     /   |   \
	 *    O    |    O
	 *         O
	 */

	float HammerAngle
	{
		get
		{
			return Mathf.Abs(Vector3.Angle(transform.forward, Vector3.down) / 180 * Mathf.PI);
		}
	}

	Vector3 ContactPoint
	{
		get
		{
			return transform.position + transform.forward * (Mathf.Tan(HammerAngle) * FloorDistance);
		}
	}

	bool firstContact = true;
	Vector3 virtualContactPosition = Vector3.zero;
	Vector3 virtualTargetPosition = Vector3.zero;
	float originalY = 0;
	float activatedHammerLength = 0;

	public float currentAngle = 0;

	public GameObject contactPointIndicator = null;
	public Vector3 contactPointIndicatorOriginalPostion = Vector3.zero;

	// Update is called once per frame
	void Update() {
		currentAngle = HammerAngle;
		if (currentAngle < AngleTouchGround)
		{
			if (firstContact)
			{
				virtualContactPosition = ContactPoint;
				if (contactPointIndicator != null)
					contactPointIndicator.transform.position = virtualContactPosition;

				// The hammer may go deeper into the ground when detected.  Only calculate the part on the ground.
				activatedHammerLength = Mathf.Abs(Mathf.Tan(currentAngle) * FloorDistance);

				// Calculate a virtual target position.  Not to use HammerMove's GameObject's position.  Because it might be moved by HammerMove together.
				virtualTargetPosition = virtualContactPosition - transform.forward * (Mathf.Cos(currentAngle) * activatedHammerLength);

				originalY = target.position.y;
				firstContact = false;
				return;
			}

			// from contact point go back to the target.position along with the hammer's direction.
			var newPosition = virtualContactPosition - transform.forward * (Mathf.Cos(currentAngle) * activatedHammerLength);

			// Calculate distance
			var diff = newPosition - virtualTargetPosition;
			target.position += diff;
			virtualTargetPosition += diff;
		}
		else
		{
			if (!firstContact)
			{
				firstContact = true;

				// Remove contactPointIndicator
				contactPointIndicator.transform.position = contactPointIndicatorOriginalPostion;

				// Do final caculation by using max angle.
				var newPosition = virtualContactPosition - transform.forward * (Mathf.Cos(AngleTouchGround) * activatedHammerLength);

				// Calculate distance
				var diff = newPosition - virtualTargetPosition;
				var pos = target.position + diff;
				// Force goto the original ground
				pos.y = originalY;
				target.position = pos;

				virtualTargetPosition += diff;

				return;
			}
		}
	}
}
