using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	static WorldGenerator gen = new WorldGenerator();

	Voxels.World voxelWorld;

	// Use this for initialization
	void Start () {
		voxelWorld = gen.CreateDiscworld();
		GetComponent<VoxelRenderer>().SetWorld(voxelWorld);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
