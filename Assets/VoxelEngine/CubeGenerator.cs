using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class CubeGenerator
{
	static Vector3[] CUBE_VERTICES = new Vector3[] {
		new Vector3(1.0f, 0.0f, 0.0f), // 1
		new Vector3(1.0f, 0.0f, 1.0f), // 2
		new Vector3(0.0f, 0.0f, 1.0f), // 3
		new Vector3(0.0f, 0.0f, 0.0f), // 4
		new Vector3(1.0f, 1.0f, 0.0f), // 5
		new Vector3(1.0f, 1.0f, 1.0f), // 6
		new Vector3(0.0f, 1.0f, 1.0f), // 7
		new Vector3(0.0f, 1.0f, 0.0f), // 8
	};

	static Vector3[] CUBE_NORMALS = new Vector3[] {
		new Vector3( 1.0f,  0.0f,  0.0f),
		new Vector3( 0.0f,  0.0f,  1.0f),
		new Vector3(-1.0f,  0.0f,  0.0f),
		new Vector3( 0.0f,  0.0f, -1.0f),
		new Vector3( 0.0f, -1.0f,  0.0f),
		new Vector3( 0.0f,  1.0f,  0.0f),
	};

	static Vector3[] CUBE_UV = new Vector3[] {
		new Vector2( 0.0f,  0.0f),
		new Vector2( 0.0f,  1.0f),
		new Vector2( 1.0f,  0.0f),
		new Vector2( 1.0f,  1.0f),
	};

	static int[] CUBE_VERTEX_INDICES = new int[] {
		6, 2, 1,
		7, 3, 2,
		8, 4, 3,
		5, 1, 4,
		2, 3, 4,
		7, 6, 5,
		5, 6, 1,
		6, 7, 2,
		7, 8, 3,
		8, 5, 4,
		1, 2, 4,
		8, 7, 5,
	};

	static int[] CUBE_NORMAL_INDICES = new int[] {
		1, 1, 1, //  1
		2, 2, 2, //  2
		3, 3, 3, //  3
		4, 4, 4, //  4
		5, 5, 5, //  5
		6, 6, 6, //  6
		1, 1, 1, //  7
		2, 2, 2, //  8
		3, 3, 3, //  9
		4, 4, 4, // 10
		5, 5, 5, // 11
		6, 6, 6, // 12
	};

	static int[] CUBE_UV_INDICES = new int[] {
		4, 2, 1, //  1
		2, 1, 3, //  2
		3, 1, 2, //  3
		4, 3, 1, //  4
		4, 2, 1, //  5
		2, 4, 3, //  6

		3, 4, 1, //  7
		4, 2, 3, //  8
		4, 3, 2, //  9
		2, 4, 1, // 10
		3, 4, 1, // 11
		1, 2, 3, // 12
	};

	static int[,] CUBE_FACE_SELECTION = new int[,] {
		{ 1,  7 }, // +X
		{ 3,  9 }, // -X
		{ 6, 12 }, // +Y
		{ 5, 11 }, // -Y
		{ 2,  8 }, // +Z
		{ 4, 10 }, // -Z
	};

	static void AddVertex(this MeshData md, Vector3 pos, Color color, int n, int i)
	{
		md.vertices.Add(CUBE_VERTICES[CUBE_VERTEX_INDICES[i]-1] + pos);
		md.normals.Add(CUBE_NORMALS[CUBE_NORMAL_INDICES[i]-1]);
		md.uv.Add(CUBE_UV[CUBE_UV_INDICES[i]-1]);
		md.colors.Add(color);
		md.indices.Add(i + n);
	}

	public static void AddCube(this MeshData md, Int3 posi, Color color)
	{
		int n = md.vertices.Count;
		var pos = posi.ToVector3();
		for(int i=0; i<CUBE_VERTEX_INDICES.Length; i++) {
			md.AddVertex(pos, color, n, i);
		}
	}

	public static void AddCube(this MeshData md, Int3 posi)
	{
		md.AddCube(posi, Color.white);
	}

	public static void AddQuad(this MeshData md, Int3 posi, int a, Color color)
	{
		int n = md.vertices.Count;
		var pos = posi.ToVector3();
		int a1 = 3*(CUBE_FACE_SELECTION[a,0]-1);
		int a2 = 3*(CUBE_FACE_SELECTION[a,1]-1);
		var indices = new int[] { a1, a1+1, a1+2, a2, a2+1, a2+2 };
		foreach(int i in indices) {
			md.AddVertex(pos, color, n, i);
		}
	}

	public static void AddQuad(this MeshData md, Int3 posi, int a)
	{
		md.AddQuad(posi, a, Color.white);
	}

}
