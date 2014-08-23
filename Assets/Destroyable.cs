using UnityEngine;
using System.Collections;

public class Destroyable : MonoBehaviour {

	public float maxHealth = 30.0f;

	public float Health { get; set; }

	public bool Dead
	{
		get { return Health <= 0.0f; }
	}

	// Use this for initialization
	void Start()
	{
		Health = maxHealth;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
