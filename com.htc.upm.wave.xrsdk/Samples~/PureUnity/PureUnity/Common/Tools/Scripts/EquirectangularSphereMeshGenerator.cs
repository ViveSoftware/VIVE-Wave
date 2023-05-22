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

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EquirectangularSphereMeshGenerator : MonoBehaviour {
	public int longitudeSections = 72;  // vertical lines
	public int latitudeSections = 36;

	public float radiusOfSphere = 10;
	public int startLongitude = 0;

	//private PointCloudGenerator pcg = new PointCloudGenerator();

	void Start() {
		MeshFilter mf = GetComponent<MeshFilter>();

		mf.mesh = CreateMesh();
	}

	Mesh CreateMesh()
	{
		startLongitude = Mathf.Clamp(startLongitude, 0, 359);
		longitudeSections = Mathf.Clamp(longitudeSections, 8, 144);
		latitudeSections = Mathf.Clamp(latitudeSections, 4, 72);
		radiusOfSphere = Mathf.Clamp(radiusOfSphere, 1, 20);

		List<Vector3> vertices = new List<Vector3>();
		List<int> indices = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		float longStep = 360.0f / longitudeSections;
		float latStep = 180.0f / latitudeSections;
		for (int i = 0; i < longitudeSections + 1; i++)
		{
			for (int j = 0; j < latitudeSections + 1; j++)
			{
				float longitude = longStep * i - startLongitude;
				float latitude = 90 - latStep * j;
				Vector3 pos = Quaternion.Euler(latitude, longitude, 0) * new Vector3(0, 0, -radiusOfSphere);
				Debug.Log("[" + longitude + "," + latitude + "] pos "+ pos);
				
				vertices.Add(pos);
				uvs.Add(new Vector2(i / (float)longitudeSections, 1 - j / (float)latitudeSections));
			}
		}

		for (int i = 0; i < longitudeSections; i++)
		{
			int STEP = latitudeSections + 1;
			for (int j = 0; j < latitudeSections; j++)
			{
				int i1 = i * STEP;
				int i2 = (i + 1) * STEP;
				int j1 = j;
				int j2 = j + 1;

				// ad -> longitude
				// bc ^ latitude
				int a, b, c, d;
				a = i1 + j1;
				b = i1 + j2;
				c = i2 + j2;
				d = i2 + j1;

				indices.Add(a);
				indices.Add(c);
				indices.Add(b);

				indices.Add(c);
				indices.Add(a);
				indices.Add(d);
			}
		}

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetUVs(0, uvs);
		mesh.name = "SphereMeshInnerUV";

		return mesh;
	}
}
