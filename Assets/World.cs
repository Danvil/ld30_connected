using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VoxelRenderer))]
public class World : MonoBehaviour {

	public GameObject pfMineralVoxel;

	public GameObject pfFactory;
	public GameObject pfDriller;

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
			if(!x.falling && !Voxels.IsTopVoxelOrHigher(x.transform.position.ToInt3())) {
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
