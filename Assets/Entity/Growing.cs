using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Entity))]
public class Growing : MonoBehaviour {

	public float replicateRate = 0.05f;
	public float replicateMinGrowth = 0.8f;
	public float replicateDistance = 2.1f;
	public float neighboursAreaRadius = 2.5f;
	public float neighbourMinDist = 0.5f;
	public int neihboursMax = 3;
	public float growthRate = 0.05f;
	public float growthMax = 1.20f;

	public Entity Entity { get; private set; }

	public float Growth { get; set; }

	void Awake()
	{
		Growth = 0.0f;
		Entity = GetComponent<Entity>();
		Entity.growing = this;
		this.transform.localRotation *= Quaternion.AngleAxis(Tools.Random(0,360), Vector3.up);
	}

	bool CanReplicate()
	{
		return
			(Growth >= replicateMinGrowth)
			&& Entity.world.WorldGroup.NumPlants <= Entity.world.WorldGroup.numPlantsMax;
	}

	void Replicate()
	{
		// find seed position
		Vector3 delta = Tools.RandomInRing(0.5f, replicateDistance);
		// check if supported
		if(!this.Entity.world.Voxels.HasTopVoxel((this.transform.localPosition + delta).ToInt3())) {
			return;
		}
		Vector3 pos = this.transform.position + delta;
		// get near plants
		var nb = Entity.world
			.FindTopObjects(pos, neighboursAreaRadius)
			.Select(e => e.growing)
			.Where(x => x != null)
			.ToArray();
		// check if too many plants are close
		if(nb.Length > neihboursMax) {
			return;
		}
		// check if one plant is too near
		if(nb.Any(x => (x.transform.position - pos).magnitude < neighbourMinDist)) {
			return;
		}
		// clone self
		GameObject go = (GameObject)Instantiate(this.gameObject);
		go.transform.parent = this.Entity.world.transform;
		go.transform.position = pos;
		go.GetComponent<Entity>().MoveToWorld(this.Entity.world);
	}

	void SetScale()
	{
		this.transform.localScale = (0.1f + 0.9f*Growth) * Vector3.one;
	}

	// Use this for initialization
	void Start () {
		SetScale();
	}
	
	// Update is called once per frame
	void Update () {
		if(Entity.destroyable && Entity.destroyable.Dead) {
			return;
		}
		// growth
		Growth = Mathf.Min(growthMax, Growth + Time.deltaTime * growthRate);
		SetScale();
		// replication
		if(CanReplicate()) {
			if(Tools.PoissonTest(Time.deltaTime * replicateRate)) {
				Replicate();
			}
		}
	}
}
