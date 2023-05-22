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
using System.Collections.Generic;

public class RuntimeCube : MonoBehaviour {
	public int x = 13;
	public int y = 13;
	public int z = 13;
	public float size = 0.2f;
	public float gap = 2f;
	[Tooltip("When 0, each face has a quad.  When 1, each face has four quad, and etc.")]
	[Range(0, 10)]
	public int meshSplit = 0;

	private void OnValidate()
	{
		int verticesPerCube = (2 + meshSplit) * (2 + meshSplit) * 6;
		var verticesCount = x* y *z * verticesPerCube;
		if (verticesCount > 65536)
		{
			Debug.LogWarning("Vertices is too many: " + verticesCount);
		}
		else
		{
			Debug.Log("Vertices count is " + verticesCount);
		}
	}

	// Use this for initialization
	void OnEnable () {
		var meshfilter = GetComponent<MeshFilter>();
		meshfilter.mesh = createMesh(x, y, z, size, gap, meshSplit);
	}

	class MeshData
	{
		public int x;
		public int y;
		public int z;
		public float size;
		public float gap;
		public int currentIdx;
		public List<Vector3> vertices;
		public List<Vector2> uv;
		public List<Vector3> normals;
		public List<int> indices;
		public Vector3 position;
	}

	private static Mesh createMesh(int x, int y, int z, float size, float gap, int meshSplit)
	{
		print("Create Mesh");
		Mesh mesh = new Mesh();
		const int verticesCount = 14;
		const int indicesCount = 18;
		int cubes = x * y * z;
		if (cubes == 0)
			return null;

		MeshData data = new MeshData();
		data.x = x;
		data.y = y;
		data.z = z;
		data.size = size;
		data.gap = gap;
		data.currentIdx = 0;
		data.vertices = new List<Vector3>(cubes * verticesCount);
		data.uv = new List<Vector2>(cubes * verticesCount);
		data.normals = new List<Vector3>(cubes * verticesCount);
		data.indices = new List<int>(cubes * indicesCount);

		print("Begin loop Mesh");
		for (int i = 0; i < x; i++)
		{
			float dx = (-((x - 1f) / 2f) + i) * gap;
			for (int j = 0; j < y; j++)
			{
				float dy = (-((y - 1f) / 2f) + j) * gap;
				for (int k = 0; k < z; k++)
				{
					float dz = (-((z - 1f) / 2f) + k) * gap;
					data.position = new Vector3(dx, dy, dz);
					CreateACube3(ref data, meshSplit);
				}
			}
		}

		mesh.Clear();  // This is important
		mesh.vertices = data.vertices.ToArray();
		mesh.uv = data.uv.ToArray();
		//mesh.normals = data.normals.ToArray();
		mesh.normals = null;
		mesh.SetIndices(data.indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.name = "CubemapCubes";
		return mesh;
	}

	private static Vector3 Bilinear(Vector3[] quad, Vector2 t)
	{
		var v1 = Vector3.Lerp(quad[0], quad[1], t.x);
		var v2 = Vector3.Lerp(quad[3], quad[2], t.x);
		return Vector3.Lerp(v1, v2, t.y);
	}

	private static Vector2 Bilinear(Vector2[] quad, Vector2 t)
	{
		var v1 = Vector2.Lerp(quad[0], quad[1], t.x);
		var v2 = Vector2.Lerp(quad[3], quad[2], t.x);
		return Vector2.Lerp(v1, v2, t.y);
	}

	// MeshTopology are Quads, and input follow right hand rule.
	private static void GenerateFace(ref MeshData data, Vector3 [] vertices, Vector3 normal, Vector2 [] uvs, int split = 0)
	{
		split = Mathf.Clamp(split, 0, 10);
		int N = split + 2;
		Vector2 t = new Vector2(0, 0);
		float step = 1.0f / (N - 1);
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				t.x = step * i;
				t.y = step * j;
				// Create Middle point
				data.vertices.Add(Bilinear(vertices, t));
				data.normals.Add(normal);
				data.uv.Add(Bilinear(uvs, t));
			}
		}
		Debug.Log("data.vertices Count " + data.vertices.Count);

		// Indices for split = 1, N = 3
		// 012
		// 345
		// 678
		int M = N - 1;
		for (int i = 0; i < M; i++)
		{
			for (int j = 0; j < M; j++)
			{
				int g = i + 1;
				int h = j + 1;
				// in left hand rule
				data.indices.Add(i + N * j + data.currentIdx);
				data.indices.Add(g + N * j + data.currentIdx);
				data.indices.Add(g + N * h + data.currentIdx);
				data.indices.Add(i + N * j + data.currentIdx);
				data.indices.Add(g + N * h + data.currentIdx);
				data.indices.Add(i + N * h + data.currentIdx);
			}
		}

		data.currentIdx += N * N;
		Debug.Log("data.currentIdx " + data.currentIdx);
	}

	// This cube will have normal face inside.  It is used to show 360 video with texture of 6 face projection.
	private static void CreateACubeFor6FaceVideo(ref MeshData data, int split)
	{
		/*
		     H-----G
		    /|    /|
		   D-+---C |
		   | |   | |
		   | E---+-F
		   |/    |/
		   A-----B
		*/

		//  DB CB HF GF HU DU
		//  AB BB EF FF GU CU
		//  FD BD GR CR DL HL
		//  ED AD FR BR AL EL

		// We need 6 face and each face has its own normal.  This means we need 36 vertices.
		float size = data.size;
		Vector3 pos = data.position;

		var A = pos + new Vector3(-size, -size, -size);
		var B = pos + new Vector3(size, -size, -size);
		var C = pos + new Vector3(size, size, -size);
		var D = pos + new Vector3(-size, size, -size);
		var E = pos + new Vector3(-size, -size, size);
		var F = pos + new Vector3(size, -size, size);
		var G = pos + new Vector3(size, size, size);
		var H = pos + new Vector3(-size, size, size);

		// Texture (0,0) is from left bottom.
		const float ustep = 1 / 3.0f;
		const float vstep = 1 / 2.0f;

		// uv
		// 02 12 12 22 22 32
		// 01 11 11 21 21 31
		// 00 10 10 20 20 30


		// vertex
		//  DB CB HF GF HU DU
		//  AB BB EF FF GU CU
		//  FD BD GR CR DL HL
		//  ED AD FR BR AL EL
		var uvDB = new Vector2(0 * ustep, 2 * vstep);
		var uvCB = new Vector2(1 * ustep, 2 * vstep);
		var uvHF = new Vector2(1 * ustep, 2 * vstep);
		var uvGF = new Vector2(2 * ustep, 2 * vstep);
		var uvHU = new Vector2(2 * ustep, 2 * vstep);
		var uvDU = new Vector2(3 * ustep, 2 * vstep);

		var uvAB = new Vector2(0 * ustep, 1 * vstep);
		var uvBB = new Vector2(1 * ustep, 1 * vstep);
		var uvEF = new Vector2(1 * ustep, 1 * vstep);
		var uvFF = new Vector2(2 * ustep, 1 * vstep);
		var uvGU = new Vector2(2 * ustep, 1 * vstep);
		var uvCU = new Vector2(3 * ustep, 1 * vstep);

		var uvFD = new Vector2(0 * ustep, 1 * vstep);
		var uvBD = new Vector2(1 * ustep, 1 * vstep);
		var uvGR = new Vector2(1 * ustep, 1 * vstep);
		var uvCR = new Vector2(2 * ustep, 1 * vstep);
		var uvDL = new Vector2(2 * ustep, 1 * vstep);
		var uvHL = new Vector2(3 * ustep, 1 * vstep);

		var uvED = new Vector2(0 * ustep, 0 * vstep);
		var uvAD = new Vector2(1 * ustep, 0 * vstep);
		var uvFR = new Vector2(1 * ustep, 0 * vstep);
		var uvBR = new Vector2(2 * ustep, 0 * vstep);
		var uvAL = new Vector2(2 * ustep, 0 * vstep);
		var uvEL = new Vector2(3 * ustep, 0 * vstep);


		var NU = new Vector3(0, -1, 0);  // Up
		var ND = new Vector3(0, 1, 0);  // Down
		var NL = new Vector3(1, 0, 0);  // Left
		var NR = new Vector3(-1, 0, 0);  // Right
		var NF = new Vector3(0, 0, -1);  // Forward
		var NB = new Vector3(0, 0, 1);  // Backward

		//  DB CB HF GF GU CU
		//  AB BB EF FF HU DU
		//  FB BB GR CR DL HL
		//  EB AB FR BR AL EL

		// Face Up:
		GenerateFace(ref data, new Vector3[] { C, D, H, G }, NU, new Vector2[] { uvCU, uvDU, uvHU, uvGU }, split);

		// Face Down:
		GenerateFace(ref data, new Vector3[] { A, B, F, E }, ND, new Vector2[] { uvAD, uvBD, uvFD, uvED }, split);

		// Face Left:
		GenerateFace(ref data, new Vector3[] { A, E, H, D }, NL, new Vector2[] { uvAL, uvEL, uvHL, uvDL }, split);

		// Face Right:
		GenerateFace(ref data, new Vector3[] { B, C, G, F }, NR, new Vector2[] { uvBR, uvCR, uvGR, uvFR }, split);

		// Face Forward:
		GenerateFace(ref data, new Vector3[] { E, F, G, H }, NF, new Vector2[] { uvEF, uvFF, uvGF, uvHF }, split);

		// Face Backward:
		GenerateFace(ref data, new Vector3[] { A, D, C, B }, NB, new Vector2[] { uvAB, uvDB, uvCB, uvBB }, split);
	}


	// The createACube didn't generate the normal well. It has bad light effect.  This version 3 imporve it.
	private static void CreateACube3(ref MeshData data, int split)
	{
		/*
		     H-----G
		    /|    /|
		   D-+---C |
		   | |   | |
		   | E---+-F
		   |/    |/
		   A-----B
		*/
		// We need 6 face and each face has its own normal.  This means we need 36 vertices.
		float size = data.size;
		Vector3 pos = data.position;

		var A = pos + new Vector3(-size, -size, -size);
		var B = pos + new Vector3(size, -size, -size);
		var C = pos + new Vector3(size, size, -size);
		var D = pos + new Vector3(-size, size, -size);
		var E = pos + new Vector3(-size, -size, size);
		var F = pos + new Vector3(size, -size, size);
		var G = pos + new Vector3(size, size, size);
		var H = pos + new Vector3(-size, size, size);

		// Texture (0,0) is from left bottom.
		const float ustep = 1 / 4.0f;
		const float vstep = 1 / 3.0f;
		//     13  23
		// 02  12  22  32  42
		// 01  11  21  31  41
		//     10  10

		//     H2 G2
		//  H3 D  C  G  H
		//  E2 A  B  F  E
		//     E3 F2
		var uvA = new Vector2(ustep, vstep);
		var uvB = new Vector2(2 * ustep, vstep);
		var uvC = new Vector2(2 * ustep, 2 * vstep);
		var uvD = new Vector2(ustep, 2 * vstep);
		var uvE = new Vector2(1, vstep);
		var uvF = new Vector2(3 * ustep, vstep);
		var uvG = new Vector2(3 * ustep, 2 * vstep);
		var uvH = new Vector2(1, 2 * vstep);
		var uvE2 = new Vector2(0, vstep);
		var uvE3 = new Vector2(ustep, 0);
		var uvF2 = new Vector2(2 * ustep, 0);
		var uvG2 = new Vector2(2 * ustep, 1);
		var uvH2 = new Vector2(ustep, 1);
		var uvH3 = new Vector2(0, 2 * vstep);

		var NU = new Vector3(0, 1, 0);  // Up
		var ND = new Vector3(0, -1, 0);  // Down
		var NL = new Vector3(-1, 0, 0);  // Left
		var NR = new Vector3(1, 0, 0);  // Right
		var NF = new Vector3(0, 0, 1);  // Forward
		var NB = new Vector3(0, 0, -1);  // Backward

		// Face Up:
		GenerateFace(ref data, new Vector3[] { C, G, H, D }, NU, new Vector2[] { uvC, uvG2, uvH2, uvD }, split);

		// Face Down:
		GenerateFace(ref data, new Vector3[] { A, E, F, B }, ND, new Vector2[] { uvA, uvE3, uvF2, uvB }, split);

		// Face Left:
		GenerateFace(ref data, new Vector3[] { A, D, H, E }, NL, new Vector2[] { uvA, uvD, uvH3, uvE2 }, split);

		// Face Right:
		GenerateFace(ref data, new Vector3[] { B, F, G, C }, NR, new Vector2[] { uvB, uvF, uvG, uvC}, split);

		// Face Forward:
		GenerateFace(ref data, new Vector3[] { E, H, G, F }, NF, new Vector2[] { uvE, uvH, uvG, uvF}, split);

		// Face Backward:
		GenerateFace(ref data, new Vector3[] { A, B, C, D }, NB, new Vector2[] { uvA, uvB, uvC, uvD }, split);
	}

	private static void CreateACube(ref MeshData data)
	{
		/*
		     H-----G
		    /|    /|
		   D-+---C |
		   | |   | |
		   | E---+-F
		   |/    |/
		   A-----B
		*/
		float size = data.size;
		Vector3 pos = data.position;

		var A = pos + new Vector3(-size, -size, -size);
		var B = pos + new Vector3(size, -size, -size);
		var C = pos + new Vector3(size, size, -size);
		var D = pos + new Vector3(-size, size, -size);
		var E = pos + new Vector3(-size, -size, size);
		var F = pos + new Vector3(size, -size, size);
		var G = pos + new Vector3(size, size, size);
		var H = pos + new Vector3(-size, size, size);

		// Texture (0,0) is from left bottom.
		const float ustep = 1 / 4.0f;
		const float vstep = 1 / 3.0f;
		//     13  23
		// 02  12  22  32  42
		// 01  11  21  31  41
		//     10  10

		//     H2 G2
		//  H3 D  C  G  H
		//  E2 A  B  F  E
		//     E3 F2
		var uvA = new Vector2(ustep, vstep);
		var uvB = new Vector2(2 * ustep, vstep);
		var uvC = new Vector2(2 * ustep, 2 * vstep);
		var uvD = new Vector2(ustep, 2 * vstep);
		var uvE = new Vector2(1, vstep);
		var uvF = new Vector2(3 * ustep, vstep);
		var uvG = new Vector2(3 * ustep, 2 * vstep);
		var uvH = new Vector2(1, 2 * vstep);
		var uvE2 = new Vector2(0, vstep);
		var uvE3 = new Vector2(ustep, 0);
		var uvF2 = new Vector2(2 * ustep, 0);
		var uvG2 = new Vector2(2 * ustep, 1);
		var uvH2 = new Vector2(ustep, 1);
		var uvH3 = new Vector2(0, 2 * vstep);

		var NA = new Vector3(-1, -1, -1);
		var NB = new Vector3(1, -1, -1);
		var NC = new Vector3(1, 1, -1);
		var ND = new Vector3(-1, 1, -1);
		var NE = new Vector3(-1, -1, 1);
		var NF = new Vector3(1, -1, 1);
		var NG = new Vector3(1, 1, 1);
		var NH = new Vector3(-1, 1, 1);

		// 0 ~ 3
		data.vertices.Add(A);
		data.normals.Add(NA);
		data.uv.Add(uvA);
		data.vertices.Add(B);
		data.normals.Add(NB);
		data.uv.Add(uvB);
		data.vertices.Add(C);
		data.normals.Add(NC);
		data.uv.Add(uvC);
		data.vertices.Add(D);
		data.normals.Add(ND);
		data.uv.Add(uvD);

		// 4 ~ 7
		data.vertices.Add(E);
		data.normals.Add(NE);
		data.uv.Add(uvE);
		data.vertices.Add(F);
		data.normals.Add(NF);
		data.uv.Add(uvF);
		data.vertices.Add(G);
		data.normals.Add(NG);
		data.uv.Add(uvG);
		data.vertices.Add(H);
		data.normals.Add(NH);
		data.uv.Add(uvH);

		// 8 ~ 13
		data.vertices.Add(E);
		data.normals.Add(NE);
		data.uv.Add(uvE2);
		data.vertices.Add(H);
		data.normals.Add(NH);
		data.uv.Add(uvH3);
		data.vertices.Add(H);
		data.normals.Add(NH);
		data.uv.Add(uvH2);
		data.vertices.Add(G);
		data.normals.Add(NG);
		data.uv.Add(uvG2);
		data.vertices.Add(F);
		data.normals.Add(NF);
		data.uv.Add(uvF2);
		data.vertices.Add(E);
		data.normals.Add(NA);
		data.uv.Add(uvE3);

		// Clockwise
		int[] indices = {
			0,2,1, 0,3,2, // Front C
			4,5,6, 4,6,7, // Back B
			8,9,3, 8,3,0, // Left U
			1,2,6, 1,6,5, // Right E
			3,10,11, 3,11,2, // Top
			0,1,12, 0,12,13 // Bottom
		};
		foreach (int i in indices)
		{
			data.indices.Add(i+data.currentIdx);
		}
		data.currentIdx += 14;
	}
}
