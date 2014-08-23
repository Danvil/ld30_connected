using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
	public List<Vector3> vertices = new List<Vector3>();
	public List<Vector3> normals = new List<Vector3>();
	public List<Color> colors = new List<Color>();
	public List<int> indices = new List<int>();

	public void Clear()
	{
		vertices.Clear();
		normals.Clear();
		colors.Clear();
		indices.Clear();
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.colors = colors.ToArray();
		//mesh.RecalculateNormals();
		return mesh;
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
