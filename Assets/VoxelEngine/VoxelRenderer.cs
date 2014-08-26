using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VoxelRenderer : MonoBehaviour
{
	public GameObject pfVoxelChunk;

	VoxelEngine.World world;

	Dictionary<Int3,GameObject> chunks = new Dictionary<Int3,GameObject>();

	public void SetWorld(VoxelEngine.World world)
	{
		this.world = world;
		Create();
	}

	void Create()
	{
		// clear old
		foreach(var p in chunks) {
			Destroy(p.Value);
		}
		chunks.Clear();
		// get all meshes
		Dictionary<Int3,Mesh> meshes = world.CreateAll();
		// create gameobjects
		foreach(var p in meshes) {
			GameObject go = (GameObject)Instantiate(pfVoxelChunk);
			go.SetMesh(p.Value);
			go.transform.parent = this.transform;
			go.transform.localPosition = new Vector3(0,0,0);
			chunks[p.Key] = go;
		}
	}

	void Recreate()
	{
		foreach(var p in world.RecreateDirty()) {
			chunks[p.Key].SetMesh(p.Value);
		}
	}

	void Instantiate()
	{
	}			

	void Update()
	{
		Recreate();
	}
}
