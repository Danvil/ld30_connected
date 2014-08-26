using UnityEngine;
using System.Collections;

public class DrillingPlatform : MonoBehaviour {

	public float mineralsRate = 0.25f;
	public float gooRate = 0.15f;

	World world;

	// Use this for initialization
	void Start () {
		world = this.GetComponent<Entity>().world;
	}
	
	// Update is called once per frame
	void Update () {
		Team team = world.WorldGroup.Team;
		GlobalInterface.Singleton.GetTeamRessources(team).numMinerals += Time.deltaTime * mineralsRate;
		GlobalInterface.Singleton.GetTeamRessources(team).numGoo += Time.deltaTime * gooRate;
	}
}
