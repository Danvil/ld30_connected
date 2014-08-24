using UnityEngine;
using System.Collections;

public class WorldItem : MonoBehaviour {

	public World world;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		world.Remove(this);
    }
}
