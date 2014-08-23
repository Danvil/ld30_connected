using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
