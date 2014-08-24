using UnityEngine;
using System.Collections;

public class WorldGroup : MonoBehaviour {

	public World World { get; private set; }

	public Portal Portal { get; private set; }

	public WorldInterface gui { get; private set; }

	public float allegianceColorPadding = 0.2f;

	public const float ALLEGIANCE_THRESHOLD = 0.5f;

	public float allegianceRate = 0.05f;

	public float Allegiance { get; private set; }

	public Team Team { get; set; }

	public Color AllegianceColor
	{
		get
		{
			Color allegianceColorBlue = Globals.Singleton.TeamColor(Team.BLUE);
			Color allegianceColorRed = Globals.Singleton.TeamColor(Team.RED);
			Color allegianceColorNeutral = Globals.Singleton.TeamColor(Team.NEUTRAL);
			if(Team == Team.RED) {
				float p = 0.5f*(1.0f + Allegiance)*(1.0f - allegianceColorPadding);
				return Color.Lerp(allegianceColorRed, allegianceColorBlue, p);
			}
			if(Team == Team.BLUE) {
				float p = 0.5f*(1.0f - Allegiance)*(1.0f - allegianceColorPadding);
				return Color.Lerp(allegianceColorBlue, allegianceColorRed, p);				
			}
			if(Team == Team.NEUTRAL) {
				if(Allegiance >= 0) {
					float p = Allegiance;
					return Color.Lerp(allegianceColorNeutral, allegianceColorBlue, p);
				}
				else {
					float p = -Allegiance;
					return Color.Lerp(allegianceColorNeutral, allegianceColorRed, p);
				}
			}
			return Color.black;
		}
	}

	void UpdateAllegiance()
	{
		// count robots
		int numRed = 0, numBlue = 0;
		foreach(var r in World.FindRobots()) {
			if(r.Team == Team.RED) numRed ++;
			if(r.Team == Team.BLUE) numBlue ++;
		}
		// balance
		int balance = numBlue - numRed;
		float delta = allegianceRate * (float)balance;
		Allegiance += delta;
		if(Allegiance <= -1.0f) {
			Allegiance = -1.0f;
			Team = Team.RED;
		}
		if(Allegiance >= 1.0f) {
			Allegiance = 1.0f;
			Team = Team.BLUE;
		}
		// update color
		Portal.SetColor(AllegianceColor);
	}

	// Use this for initialization
	void Start () {
		World = GetComponentInChildren<World>();
		World.WorldGroup = this;
		Portal = GetComponentInChildren<Portal>();
		gui = GetComponentInChildren<WorldInterface>();
		gui.world = World;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateAllegiance();
	}
}
