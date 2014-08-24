using UnityEngine;
using System.Collections;

public class WorldSelector : MonoBehaviour {

	public static WorldSelector Singleton;

	void Awake()
	{
		Singleton = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
