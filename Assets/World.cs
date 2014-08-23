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

	public int numMinerals = 100;
	public int numPlants = 50;
	public int numRobots = 10;

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

	List<GameObject> objects = new List<GameObject>();

	public IEnumerable<GameObject> FindObjectsInSphere(Vector3 pos, float r)
	{
		foreach(var x in objects) {
			if((x.transform.position - pos).magnitude < r) {
				yield return x;
			}
		}
	}

	void Generate()
	{
		// first pass voxels
		Voxels = gen.CreateDiscworld();
		// second pass minerals
		// plant 100 minerals in random solid voxels
		foreach(Int3 p in Voxels.GetSolidVoxels().RandomSample(numMinerals)) {
			GameObject go = (GameObject)Instantiate(pfMineral);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0,0.5f);
			objects.Add(go);
		}
		// third pass plants
		// plant 40 plants on top of random solid top voxels
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numPlants)) {
			GameObject go = (GameObject)Instantiate(pfPlant);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,1,0.5f);
			objects.Add(go);
		}
		// robot
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numRobots)) {
			GameObject go = (GameObject)Instantiate(pfRobot);
			Robot rob =	go.GetComponent<Robot>();
			rob.world = this;
			rob.SetNewPosition(this.transform.position + p.ToVector3() + new Vector3(0.5f,1,0.5f));
			objects.Add(go);
		}
	}

	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		List<GameObject> objectsNew = new List<GameObject>();
		foreach(var x in objects) {
			var dest = x.GetComponent<Destroyable>();
			if(dest && dest.Dead) {
				GameObject go = (GameObject)Instantiate(pfGoo);
				go.transform.parent = this.transform;
				go.transform.position = x.transform.position;
				objectsNew.Add(go);
				Destroy(x);
				continue;
			}
			objectsNew.Add(x);
		}
		objects = objectsNew;
	}
}
