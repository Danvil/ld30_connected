using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	static WorldVoxelGenerator gen = new WorldVoxelGenerator();

	public GameObject pfMineral;
	public GameObject pfPlant;
	public GameObject pfGoo;
	public GameObject pfRobot;

	public Vector3 gravity = new Vector3(0,-9.81f,0);

	Voxels.World voxels;
	public Voxels.World Voxels
	{
		get { return voxels; }
		set {
			voxels = value;
			GetComponent<VoxelRenderer>().SetWorld(value);
		}
	}

	List<Item> items;

	void Generate()
	{
		// first pass voxels
		Voxels = gen.CreateDiscworld();
		// second pass minerals
		// plant 100 minerals in random solid voxels
		foreach(Int3 p in Voxels.GetSolidVoxels().RandomSample(100)) {
			GameObject go = (GameObject)Instantiate(pfMineral);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0,0.5f);
		}
		// third pass plants
		// plant 40 plants on top of random solid top voxels
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(40)) {
			GameObject go = (GameObject)Instantiate(pfPlant);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,1,0.5f);
		}
		// robot
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(3)) {
			GameObject go = (GameObject)Instantiate(pfRobot);
			Robot rob =	go.GetComponent<Robot>();
			rob.world = this;
			rob.SetNewPosition(this.transform.position + p.ToVector3() + new Vector3(0.5f,1,0.5f));
		}
	}

	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
