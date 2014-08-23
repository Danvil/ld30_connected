using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class CubeGenerator
{
	static Vector3[] CUBE_VERTICES = new Vector3[] {
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
	};

	static Vector3[] CUBE_NORMALS = new Vector3[] {
		new Vector3( 1.0f,  0.0f,  0.0f),
		new Vector3( 0.0f,  0.0f,  1.0f),
		new Vector3(-1.0f,  0.0f,  0.0f),
		new Vector3( 0.0f,  0.0f, -1.0f),
		new Vector3( 0.0f, -1.0f,  0.0f),
		new Vector3( 0.0f,  1.0f,  0.0f),
	};

	static int[] CUBE_NORMAL_INDICES = new int[] {
		1, 1, 1,
		2, 2, 2,
		3, 3, 3,
		4, 4, 4,
		5, 5, 5,
		6, 6, 6,
		1, 1, 1,
		2, 2, 2,
		3, 3, 3,
		4, 4, 4,
		5, 5, 5,
		6, 6, 6,
	};

	static int[] CUBE_INDICES = new int[] {
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

	public static void AddCube(this MeshData md, Int3 pos, Color color)
	{
		int n = md.vertices.Count;
		for(int i=0; i<CUBE_INDICES.Length; i++) {
			md.vertices.Add(CUBE_VERTICES[CUBE_INDICES[i]-1] + pos.ToVector3());
			md.normals.Add(CUBE_NORMALS[CUBE_NORMAL_INDICES[i]-1]);
			md.colors.Add(color);
			md.indices.Add(i + n);
		}
	}

	public static void AddCube(this MeshData md, Int3 pos)
	{
		md.AddCube(pos, Color.white);
	}
}
