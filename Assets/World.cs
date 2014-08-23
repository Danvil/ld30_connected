using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

	static WorldGenerator gen = new WorldGenerator();

	// Use this for initialization
	void Start () {
		gameObject.SetMesh(gen.Create());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
