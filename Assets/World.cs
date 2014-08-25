using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	static WorldVoxelGenerator gen = new WorldVoxelGenerator();

	public GameObject pfFactory;
	public GameObject pfDriller;

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

	List<Entity> entities = new List<Entity>();


	public IEnumerable<Robot> FindRobots()
	{
		return entities
			.Select(e => e.robot)
			.Where(r => r != null);
	}

	public IEnumerable<Entity> FindTopObjects(Vector3 pos, float r)
	{
		foreach(var x in entities) {
			if((x.transform.position - pos).magnitude >= r) {
				continue;
			}
			if(!Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
				continue;
			}
			yield return x;
		}
	}

	// public IEnumerable<T> FindTopObjects<T>(Vector3 pos, float r) where T : Component
	// {
	// 	foreach(var x in entities) {
	// 		T t = x.GetComponent<T>();
	// 		if(t == null) {
	// 			continue;
	// 		}
	// 		if((x.transform.position - pos).magnitude >= r) {
	// 			continue;
	// 		}
	// 		if(!Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
	// 			continue;
	// 		}
	// 		yield return t;
	// 	}
	// }

	public WorldGroup WorldGroup { get; set; }

	public GameObject Building { get; private set; }

	public ResourceDropoff ResourceDropoff { get; private set; }

	public void BuildFactory()
	{
		PlaceBuilding(pfFactory);
	}

	public void BuildDriller()
	{
		PlaceBuilding(pfDriller);
	}

	void PlaceBuilding(GameObject pf)
	{
		if(Building) {
			return;
		}
		Building = (GameObject)Instantiate(pf);
		ResourceDropoff = Building.GetComponent<ResourceDropoff>();
		int h = Voxels.GetTopVoxelHeight(Int3.Zero);
		Building.transform.parent = this.transform;
		Building.transform.localPosition = new Vector3(0,h+1,0);
		Add(Building.GetComponent<Entity>());
	}

	public bool AllowProduction { get; private set; }

	public void ToogleProduction(bool v)
	{
		AllowProduction = v;
	}

	public bool AllowMining { get; private set; }

	public void ToogleMining(bool v)
	{
		AllowMining = v;
	}

	public void Add(Entity wi)
	{
		wi.world = this;
		entities.Add(wi);
	}

	public void Remove(Entity wi)
	{
		entities.Remove(wi);
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
			Add(go.GetComponent<Entity>());
		}
		// plant small minerals in every top voxel
		foreach(Int3 p in Voxels.GetTopVoxels()) {
			GameObject go = (GameObject)Instantiate(pfMineralVoxel);
			go.transform.parent = this.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
			Add(go.GetComponent<Entity>());
		}
		// third pass plants
		// plant 40 plants on top of random solid top voxels
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numPlants)) {
			GameObject go = (GameObject)Instantiate(pfPlant);
			go.transform.parent = this.transform;
			Add(go.GetComponent<Entity>());
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
		}
		// robot laser
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numRobotsLaser)) {
			GameObject go = (GameObject)Instantiate(pfRobotLaser);
			go.transform.parent = this.transform;
			Entity wi = go.GetComponent<Entity>();
			wi.MoveToWorld(this);
			wi.Team = initRobotTeam;
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Robot rob =	go.GetComponent<Robot>();
			rob.SetRandomGoal();
		}
		// robot hauler
		foreach(Int3 p in Voxels.GetTopVoxels().RandomSample(numRobotsHauler)) {
			GameObject go = (GameObject)Instantiate(pfRobotHauler);
			go.transform.parent = this.transform;
			Entity wi = go.GetComponent<Entity>();
			wi.MoveToWorld(this);
			wi.Team = initRobotTeam;
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Robot rob =	go.GetComponent<Robot>();
			rob.SetRandomGoal();
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
		Add(go.GetComponent<Entity>());
	}

	public Team initRobotTeam = Team.NEUTRAL;

	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
	}
}
