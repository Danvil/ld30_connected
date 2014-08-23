using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator
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

	List<Vector3> vertices = new List<Vector3>();
	List<Vector3> normals = new List<Vector3>();
	List<Color> colors = new List<Color>();
	List<int> indices = new List<int>();

	void Start()
	{
		vertices.Clear();
		indices.Clear();
	}

	void AddCube(Int3 pos, Color color)
	{
		int n = vertices.Count;
		for(int i=0; i<CUBE_INDICES.Length; i++) {
			vertices.Add(CUBE_VERTICES[CUBE_INDICES[i]-1] + pos);
			normals.Add(CUBE_NORMALS[CUBE_NORMAL_INDICES[i]-1]);
			colors.Add(color);
			indices.Add(i + n);
		}
	}

	void AddCube(Int3 pos)
	{
		AddCube(pos, Color.white);
	}

	Mesh GetMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.colors = colors.ToArray();
		//mesh.RecalculateNormals();
		return mesh;
	}

	public Mesh Create()
	{
		Start();
		AddCube(new Int3(0,0,0));
		AddCube(new Int3(1,0,0));
		AddCube(new Int3(2,0,0));
		AddCube(new Int3(2,0,1), new Color(1.00f,0.50f,0.00f));
		return GetMesh();
	}

}
