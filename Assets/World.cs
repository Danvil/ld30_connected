using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour {

	public GameObject pfVoxelChunk;

	static WorldGenerator gen = new WorldGenerator();

	void Create()
	{
		foreach(var chunk in gen.Create()) {
			GameObject go = (GameObject)Instantiate(pfVoxelChunk);
			go.transform.parent = this.transform;
			go.SetMesh(chunk);
		}
	}

	// Use this for initialization
	void Start () {
		Create();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
