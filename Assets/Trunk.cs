using UnityEngine;
using System.Collections;

public class Trunk : MonoBehaviour {

	public Material matGoo;
	public Material matMinerals;

	Material SelectMaterial(PickableType type)
	{
		switch(type) {
			case PickableType.GOO: return matGoo;
			case PickableType.MINERALS: return matMinerals;
			default: return null;
		}
	}

	PickableType type;
	public PickableType Type
	{
		get { return type; }
		private set
		{
			type = value;
			this.GetComponentInChildren<Renderer>().material = SelectMaterial(value);
		}
	}

	public float loadRate = 1.0f;

	public float maxLoad = 5.0f;

	float load;

	public float MaxCanLoad(PickableType type, float v)
	{
		if(load == 0.0f || type == Type) {
			return Mathf.Min(RemainingCapacity, v);
		}
		else {
			return 0.0f;
		}
	}

	public float MaxCanLoad(Pickable p)
	{
		if(load == 0.0f || p.type == Type) {
			return Mathf.Min(RemainingCapacity, p.Amount);
		}
		else {
			return 0.0f;
		}
	}

	void SetLoadHeight()
	{
		Vector3 pos = this.transform.localPosition;
		pos.y = 0.5f * LoadPercent;
		this.transform.localPosition = pos;
	}

	public float Load(PickableType type, float v)
	{
		if(load == 0.0f || type == Type) {
			float delta = MaxCanLoad(type, v);
			if(delta > 0) {
				Type = type;
			}
			load += delta;
			SetLoadHeight();
			return delta;
		}
		else {
			return 0.0f;
		}
	}

	public float Unload(float v)
	{
		float delta = Mathf.Min(v, load);
		load -= delta;
		SetLoadHeight();
		return delta;
	}

	public float LoadPercent
	{
		get { return load / maxLoad; }
	}

	public float RemainingCapacity
	{
		get { return maxLoad - load; }
	}

	public bool IsFull
	{
		get { return LoadPercent == 1.0f; }
	}

	public bool IsEmpty
	{
		get { return LoadPercent == 0.0f; }
	}

	// Use this for initialization
	void Start () {
		load = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
