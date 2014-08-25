using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	public static Globals Singleton;

	public Team playerTeam = Team.BLUE;
	public Team nonPlayerTeam = Team.RED;

	public Material pfTeamNeutral;
	public Material pfTeamRed;
	public Material pfTeamBlue;

	public Material TeamMaterial(Team t)
	{
		switch(t) {
			case Team.NEUTRAL: return pfTeamNeutral;
			case Team.RED: return pfTeamRed;
			case Team.BLUE: return pfTeamBlue;
			default: return null;
		}
	}

	public Color TeamColor(Team t)
	{
		return TeamMaterial(t).color;
	}

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
