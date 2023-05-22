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

public class SeaOfCube : MonoBehaviour {
	public GameObject cubePrefab = null;
	public float gap = 2;
	public int size = 15;
	public float scale = 1;
	public bool RandomPose = false;

	// Use this for initialization
	void Start () {
		if (cubePrefab == null)
			return;
		float half = (size - 1) / 2f;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					GameObject cube = (GameObject) Instantiate(cubePrefab, transform);
					cube.transform.localScale = new Vector3(scale, scale, scale);
					cube.transform.position = new Vector3(i - half, j - half, k - half) * gap;
					if (RandomPose)
						cube.transform.rotation = Random.rotation;
				}
			}
		}
	}
}
