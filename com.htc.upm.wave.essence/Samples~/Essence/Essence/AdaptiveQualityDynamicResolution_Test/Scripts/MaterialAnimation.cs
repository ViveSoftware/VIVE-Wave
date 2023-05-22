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

namespace AQDR
{
	[RequireComponent(typeof(MeshRenderer))]
	public class MaterialAnimation : MonoBehaviour {

		MeshRenderer mr;

		[SerializeField]
		[Range(0.5f, 10)]
		float switchInterval = 1.5f;

		float startTime = -1;
		int currentIndex = 0;

		[Tooltip("Start from index 0")]
		[SerializeField]
		List<Material> materials = new List<Material>();

		private void OnEnable()
		{
			if (materials.Count <= 0)
			{
				enabled = false;
				return;
			}

			currentIndex = 0;
			startTime = Time.unscaledTime;
			mr = GetComponent<MeshRenderer>();
			mr.material = materials[0];
		}

		// Update is called once per frame
		void Update () {
			// No need animation
			if (materials.Count < 2)
				return;

			if (Time.unscaledTime - startTime < switchInterval)
				return;

			startTime = Time.unscaledTime;
			currentIndex = ++currentIndex % materials.Count;
			mr.material = materials[currentIndex];
		}
	}
}
