using UnityEngine;
using System.Collections;

public class WorldGroup : MonoBehaviour {

	public World World { get; private set; }

	public Portal Portal { get; private set; }

	public WorldInterface gui { get; private set; }

	// Use this for initialization
	void Start () {
		World = GetComponentInChildren<World>();
		Portal = GetComponentInChildren<Portal>();
		gui = GetComponentInChildren<WorldInterface>();
		gui.world = World;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
