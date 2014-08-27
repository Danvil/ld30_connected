using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	public GameObject pfMineralVoxel;

	public GameObject pfFactory;
	public GameObject pfDriller;

	public int maxPickables = 100;
	public int maxRobots = 20;

	public Vector3 gravity = new Vector3(0,-9.81f,0);

	VoxelEngine.World voxels;
	public VoxelEngine.World Voxels
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
			if(!x.falling && !x.pickable && !Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
				continue;
			}
			yield return x;
		}
	}

	public IEnumerable<Entity> FindTopObjects()
	{
		foreach(var x in entities) {
			if(!x.falling && !x.pickable && !Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
				continue;
			}
			yield return x;
		}
	}

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

	bool _allowProduction;
	bool _allowProductionMaxNotReached = true;
	public bool AllowProduction
	{
		get { return _allowProduction && _allowProductionMaxNotReached; }
		set { _allowProduction = true; }
	}

	bool _allowMining;
	bool _allowMiningMaxNotReached = true;
	public bool AllowMining
	{
		get { return _allowMining && _allowMiningMaxNotReached; }
		set { _allowMining = true; }
	}

	public bool AllowHarvesting { get; set; }

	public void Add(Entity wi)
	{
		wi.world = this;
		entities.Add(wi);
		UpdateAllowMiningMaxNotReached();
	}

	public void Remove(Entity wi)
	{
		entities.Remove(wi);
		wi.world = null;
		UpdateAllowMiningMaxNotReached();
	}

	void UpdateAllowMiningMaxNotReached()
	{
		int curr1 = entities.Select(x => x.pickable).Where(x => x != null).Count();
		_allowMiningMaxNotReached = (curr1 <= maxPickables);
		int curr2 = entities.Select(x => x.robot).Where(x => x != null && x.entity.Team == WorldGroup.Team).Count();
		_allowProductionMaxNotReached = (curr2 <= maxRobots);
	}

	public void DestroyVoxel(Int3 v)
	{
		if(Voxels.Get(v).solid == VoxelEngine.Voxel.Solidness.Ultra) {
			return;
		}
		Voxels.Set(v, VoxelEngine.Voxel.Empty);
		// place mineral for voxel below
		GameObject go = (GameObject)Instantiate(pfMineralVoxel);
		go.transform.parent = this.transform;
		go.transform.localPosition = (v - Int3.Z).ToVector3() + new Vector3(0.5f,0.5f,0.5f);
		Add(go.GetComponent<Entity>());
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
