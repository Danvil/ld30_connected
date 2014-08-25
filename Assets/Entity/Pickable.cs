﻿using UnityEngine;
using System.Collections;

public enum PickableType
{
	GOO,
	MINERALS
}

[RequireComponent(typeof(Entity))]
public class Pickable : MonoBehaviour {

	public Entity entity { get; private set; }

	public PickableType type;

	public float maxAmount = 1.0f;

	public float Amount { get; set; }

	public float Gather(float x)
	{
		x = Mathf.Min(Amount, x);
		Amount -= x;
		return x;
	}

	public bool Depleted
	{
		get { return Amount <= 0.0f; }
	}

	public float AmountPercent
	{
		get { return Amount / maxAmount; }
	}

	void Awake()
	{
		entity = GetComponent<Entity>();
		entity.pickable = this;
	}

	// Use this for initialization
	void Start () {
		Amount = maxAmount;
	}
	
	// Update is called once per frame
	void Update () {
		float scl1 = Mathf.Sqrt(0.3f*maxAmount);
		float scl2 = 0.2f + 0.8f*Mathf.Sqrt(AmountPercent);
		this.transform.localScale = scl1 * scl2 * Vector3.one;
		if(Depleted) {
			Destroy(gameObject);
		}
	}
}