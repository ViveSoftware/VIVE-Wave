using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothVertices : MonoBehaviour
{
	Vector3[] vertices;
	Vector3[] normals;
	int[] triangles;

	// Start is called before the first frame update
	void Start()
    {
		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = mf.mesh;
		vertices = mesh.vertices;
		normals = mesh.normals;
		triangles = mesh.GetTriangles(0);

		var m = transform.localToWorldMatrix;
		int N = triangles.Length / 3;
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int idx = triangles[i * 3 + j];
				var p = vertices[idx];
				var p0 = new Vector3(p.x, 0, p.z).normalized;
				var n = normals[idx];
				var n0 = n;
				if (Mathf.Abs(Vector3.Dot(n, p0)) > 0.2f)
					n0 = Vector3.Project(n, p0);
				n0 = n0.normalized;
				normals[idx] = n0;
			}
		}

		mesh.normals = normals;
	}

    // Update is called once per frame
    void Update()
    {
		//var m = transform.localToWorldMatrix;
		//int N = triangles.Length / 3;
		//for (int i = 0; i < N; i++)
		//{
		//	for (int j = 0; j < 3; j++)
		//	{
		//		int idx = triangles[i * 3 + j];
		//		var p = vertices[idx];
		//		var p0 = vertices[idx].normalized;
		//		var n = normals[idx] * 0.5f;
		//		var n0 = Vector3.Project(n, p0);
		//		n0 = n0.normalized;
		//		var pw = m * p;
		//		var nw = m * (p + n);
		//		var nw0 = m * (p + n0);
		//		Debug.DrawLine(pw, nw);
		//		Debug.DrawLine(pw, nw0, Color.red);
		//	}
		//}
	}
}
