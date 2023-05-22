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

[RequireComponent(typeof(MeshFilter))]
public class RulerMesh : MonoBehaviour {
	public int segment = 10;
	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter>();
		List<Vector2> uvs = new List<Vector2>();
		List<Vector3> vertices = new List<Vector3>();
		var mesh = mf.mesh;
		if (mf.mesh == null)
		{
			return;
		}
		else
		{
			mesh.GetUVs(0, uvs);
			mesh.GetVertices(vertices);
			for (int i = 0; i < uvs.Capacity; i++)
			{
				float x = uvs[i].x;
				float y = uvs[i].y;

				if (y >= 1)
					y = segment;
				uvs[i] = new Vector2(x, y); 
			}
			mesh.SetUVs(0, uvs);
		}
	}
}
