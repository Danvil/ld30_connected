using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
	public List<Vector3> vertices = new List<Vector3>();
	public List<Vector3> normals = new List<Vector3>();
	public List<Vector2> uv = new List<Vector2>();
	public List<Color> colors = new List<Color>();
	public List<int> indices = new List<int>();

	public void Clear()
	{
		vertices.Clear();
		normals.Clear();
		uv.Clear();
		colors.Clear();
		indices.Clear();
	}

	public Mesh CreateMesh(Vector3 scale)
	{
		Mesh mesh = new Mesh();
		for(int i=0; i<vertices.Count; i++) {
			vertices[i] = vertices[i].CoeffMult(scale);
		}
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();
		mesh.colors = colors.ToArray();
		mesh.triangles = indices.ToArray();
		//mesh.RecalculateNormals();
		return mesh;
	}

	public Mesh CreateMesh()
	{
		return CreateMesh(Vector3.one);
	}

}

public static class MeshTools
{
	public static void SetMesh(this GameObject go, Mesh mesh)
	{
		var meshFilter = go.GetComponent<MeshFilter>();
		if(meshFilter) {
			meshFilter.mesh = mesh;
		}
		var meshCollider = go.GetComponent<MeshCollider>();
		if(meshCollider) {
			meshCollider.sharedMesh = mesh;
		}
	}
}
