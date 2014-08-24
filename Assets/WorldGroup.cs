using UnityEngine;
using System.Collections;

public class WorldGroup : MonoBehaviour {

	World world;

	WorldInterface gui;

	// Use this for initialization
	void Start () {
		world = GetComponentInChildren<World>();
		gui = GetComponentInChildren<WorldInterface>();
		gui.world = world;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
