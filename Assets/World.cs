using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	static WorldVoxelGenerator gen = new WorldVoxelGenerator();

	public GameObject pfMineral;
	public GameObject pfMineralVoxel;
	public GameObject pfPlant;
	public GameObject pfRobotLaser;
	public GameObject pfRobotHauler;

	public int worldRadius = 16;
	public int worldHeight = 6;
	public int numMinerals = 100;
	public int numPlants = 50;
	public int numRobotsLaser = 10;
	public int numRobotsHauler = 5;

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

	List<WorldItem> objects = new List<WorldItem>();

	public IEnumerable<WorldItem> FindTopObjects(Vector3 pos, float r)
	{
		foreach(var x in objects) {
			if((x.transform.position - pos).magnitude >= r) {
				continue;
			}
			if(!Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
				continue;
			}
			yield return x;
		}
	}

	public IEnumerable<T> FindTopObjects<T>(Vector3 pos, float r) where T : Component
	{
		foreach(var x in objects) {
			T t = x.GetComponent<T>();
			if(t == null) {
				continue;
			}
			if((x.transform.position - pos).magnitude >= r) {
				continue;
			}
			if(!Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
				continue;
			}
			yield return t;
		}
	}

	public void Add(WorldItem wi)
	{
		wi.world = this;
		objects.Add(wi);
	}

	public void Remove(WorldItem wi)
	{
		objects.Remove(wi);
		wi.world = null;
	}

	void Generate()
	{
		// first pass voxels
		Voxels = gen.CreateDiscworld(worldRadius, worldHeight);
		// second pass minerals
		// plant 100 minerals in random solid voxels
		foreach(Int3 p in Voxels.GetSolidVoxels().RandomSample(numMinerals)) {
			GameObject go = (GameObject)Instantiate(pfMineral);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
			Add(go.GetComponent<WorldItem>());
		}
		// plant small minerals in every voxel
		foreach(Int3 p in Voxels.GetTopVoxels()) {
			GameObject go = (GameObject)Instantiate(pfMineralVoxel);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
			Add(go.GetComponent<WorldItem>());
		}
		// third pass plants
		// plant 40 plants on top of random solid top voxels
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numPlants)) {
			GameObject go = (GameObject)Instantiate(pfPlant);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,1,0.5f);
			Add(go.GetComponent<WorldItem>());
		}
		// robot laser
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numRobotsLaser)) {
			GameObject go = (GameObject)Instantiate(pfRobotLaser);
			go.transform.parent = this.transform;
			Robot rob =	go.GetComponent<Robot>();
			rob.world = this;
			rob.SetNewPosition(this.transform.position + p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Add(go.GetComponent<WorldItem>());
		}
		// robot hauler
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numRobotsHauler)) {
			GameObject go = (GameObject)Instantiate(pfRobotHauler);
			go.transform.parent = this.transform;
			Robot rob =	go.GetComponent<Robot>();
			rob.world = this;
			rob.SetNewPosition(this.transform.position + p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Add(go.GetComponent<WorldItem>());
		}
	}

	public void DestroyVoxel(Int3 v)
	{
		if(Voxels.Get(v).solid == global::Voxels.Voxel.Solidness.Ultra) {
			return;
		}
		Voxels.Set(v, global::Voxels.Voxel.Empty);
		// place mineral for voxel below
		GameObject go = (GameObject)Instantiate(pfMineralVoxel);
		go.transform.parent = this.transform;
		go.transform.localPosition = (v - Int3.Z).ToVector3() + new Vector3(0.5f,0.5f,0.5f);
		Add(go.GetComponent<WorldItem>());
	}

	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		// List<GameObject> objectsNew = new List<GameObject>();
		// foreach(var x in objects) {
		// 	var dest = x.GetComponent<Destroyable>();
		// 	if(dest && dest.Dead) {
		// 		continue;
		// 	}
		// 	objectsNew.Add(x);
		// }
		// objects = objectsNew;
	}
}
