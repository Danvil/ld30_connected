﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class Falling : MonoBehaviour {

	public Entity entity { get; private set; }

	public bool forceHeight = false;
	public float baseHeight = 0.0f;

	const float DEATH_HEIGHT = -100.0f;

	public bool IsFalling { get; private set; }

	Vector3 velocity = Vector3.zero;
	Int3 topVoxel;

	void ComputeTopVoxel()
	{
		if(!entity.world) return;
		entity.world.Voxels.TryGetTopVoxel(this.transform.localPosition.ToInt3(), out topVoxel);
	}

	public bool TrySetNewLocalPosition(Vector3 pos)
	{
		Int3 newTopVoxel;
		if(entity.world.Voxels.TryGetTopVoxel(pos.ToInt3(), out newTopVoxel)) {
			// safe
			topVoxel = newTopVoxel;
			this.transform.localPosition = pos;
			return true;
		}
		else {
			// not safe
			return false;
		}
	}

	public void SetNewLocalPosition(Vector3 pos)
	{
		this.transform.localPosition = pos;
		ComputeTopVoxel();
	}

	void Awake()
	{
		entity = GetComponent<Entity>();
		entity.falling = this;
	}

	// Use this for initialization
	void Start ()
	{
		ComputeTopVoxel();
		IsFalling = false;
		StartCoroutine("UpdateTopVoxel");
	}
	
	// Update is called once per frame
	void Update()
	{
		// position
		Vector3 pos = this.transform.localPosition;
		// test if falling
		IsFalling = (pos.y > topVoxel.z + 1.5f);
		if(IsFalling) {
			// fall down
			pos += Time.deltaTime * velocity;
			velocity += Time.deltaTime * entity.world.gravity;
			// check if we fall to death
			if(pos.y < DEATH_HEIGHT) {
				Destroy(gameObject);
			}
		}
		else {
			if(forceHeight) {
				pos.y = topVoxel.z + 1.0f + baseHeight;
			}
			velocity = Vector3.zero;
		}
		this.transform.localPosition = pos;
	}

	IEnumerator UpdateTopVoxel()
	{
		while(true) {
			ComputeTopVoxel();
			yield return new WaitForSeconds(1.0f);
		}
	}
}
