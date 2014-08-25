using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RessourceOverview
{
	public int numWorlds = 0;
	public int numRobots = 0;
	public float numMinerals = 0;
	public float numGoo = 0;
}

public class GlobalInterface : MonoBehaviour {

	public static GlobalInterface Singleton;

	public UnityEngine.UI.Text txtWorlds;
	public UnityEngine.UI.Text txtRobots;
	public UnityEngine.UI.Text txtMinerals;
	public UnityEngine.UI.Text txtGoo;

	Dictionary<Team,RessourceOverview> ressources = new Dictionary<Team,RessourceOverview>();

	public RessourceOverview GetTeamRessources(Team team)
	{ return ressources[team]; }

	public RessourceOverview PlayerRessources
	{ get { return GetTeamRessources(Globals.Singleton.playerTeam); } }

	public void ChangeNumWorlds(Team team, int delta)
	{ GetTeamRessources(team).numWorlds += delta; }

	public void ChangeNumRobots(Team team, int delta)
	{ GetTeamRessources(team).numRobots += delta; }

	public void ChangeNumMinerals(Team team, float delta)
	{ GetTeamRessources(team).numMinerals += delta; }

	public void ChangeNumGoo(Team team, float delta)
	{ GetTeamRessources(team).numGoo += delta; }

	void Awake()
	{
		Singleton = this;
	}
	
	// Use this for initialization
	void Start () {
		ressources[Team.BLUE] = new RessourceOverview();
		ressources[Team.RED] = new RessourceOverview();
		ressources[Team.NEUTRAL] = new RessourceOverview();
	}

	// Update is called once per frame
	void Update () {
		StartCoroutine("UpdateGui");
	}

	IEnumerator UpdateGui() {
		while(true) {
			txtWorlds.text = string.Format("Worlds: {0}", PlayerRessources.numWorlds);
			txtRobots.text = string.Format("Robots: {0}", PlayerRessources.numRobots);
			txtMinerals.text = string.Format("Minerals: {0:0.0}", PlayerRessources.numMinerals);
			txtGoo.text = string.Format("Goo: {0:0.0}", PlayerRessources.numGoo);
			yield return new WaitForSeconds(0.25f);
		}
	}
}
