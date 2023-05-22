// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

// Created by Quaker Chung
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudGenerator
{
	private List<Vector3> vertices;
	private List<Vector3> normals;
	private List<int> indices;
	private List<int> triangles;
	private List<Vector2> uvs;
	private int triangleCount;

	private float sqrt3 = Mathf.Sqrt(3);

	public PointCloudGenerator()
	{
		Reset();
	}

	public void Reset()
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		indices = new List<int>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		triangleCount = 0;
	}

	private List<Vector3> CreateTriangle(Vector3 pos, Vector3 normal, float scale)
	{
		// We want to make an equilateral triangle.

		Vector3 up;
		// We can't use two same vector to make a plane. Choose another vector if it happened.
		if (normal.x == 0 && normal.y == 1 && normal.z == 0)
			up = new Vector3(0, 0, 1);
		else
			up = new Vector3(0, 1, 0);

		Vector3 t1, t2, t3, t4;
		// Recreate a coordinate system.
		t1 = Vector3.Cross(normal, up).normalized;
		t2 = Vector3.Cross(normal, t1).normalized;

		// t1 will be the triangle's first vectors.
		// make another two vectors of the equilateral triangle
		// -sin(30) * t1 + cos(30) * t2
		float sqrt3 = Mathf.Sqrt(3);
		t3 = -0.5f * t1 - 0.5f * sqrt3 * t2;
		t4 = -0.5f * t1 + 0.5f * sqrt3 * t2;

		t1 = t1 * scale + pos;
		t3 = t3 * scale + pos;
		t4 = t4 * scale + pos;

		//Debug.Log("t1" + t1);
		//Debug.Log("t2" + t2);
		//Debug.Log("t3" + t3);
		//Debug.Log("t4" + t4);

		List<Vector3> output = new List<Vector3>(3)
		{
			t1,
			t3,
			t4
		};
		return output;
	}

	public void AddPoint(Vector3 position, Vector3 normal, float scale = 0.01f)
	{
		vertices.AddRange(CreateTriangle(position, normal, 0.05f));
		normals.Add(normal);
		normals.Add(normal);
		normals.Add(normal);
		int index = triangleCount * 3;
		indices.Add(index);
		indices.Add(index + 1);
		indices.Add(index + 2);
		triangles.Add(index);
		triangles.Add(index + 1);
		triangles.Add(index + 2);
		float radius = 0.5f; // The texture should have a circle in uv center.
		uvs.Add(new Vector2(0 + radius, 0.5f - 2 * radius));
		uvs.Add(new Vector2(0.5f - sqrt3 * radius, 0.5f + radius));
		uvs.Add(new Vector2(0.5f + sqrt3 * radius, 0.5f + radius));
		triangleCount++;
	}

	public Mesh GenerateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetTriangles(triangles.ToArray(), 0);
		mesh.SetUVs(0, uvs);

		return mesh;
	}
}
