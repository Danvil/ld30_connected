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

	public int numPlantsMax = 100;
	public int NumPlants { get; set; }

	Team _team;
	public Team Team
	{ 
		get { return _team; }
		set {
			_team = value;
			gui.SetTeam(_team);
		}
	}

	string worldName;
	public string Name
	{
		get { return worldName; }
		set {
			worldName = value;
			gui.SetName(worldName);
		}
	}

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

	const float ALLEGIANCE_UPDATE_INTERVAl = 0.5f;

	IEnumerator UpdateAllegiance()
	{
		Color lastColor = Color.black;
		while(true) {
			// count robots
			int numRed = 0, numBlue = 0, numNeutral = 0;
			foreach(var r in World.FindRobots()) {
				if(r.entity.Team == Team.RED) numRed ++;
				if(r.entity.Team == Team.BLUE) numBlue ++;
				if(r.entity.Team == Team.NEUTRAL) numNeutral ++;
			}
			// balance
			int total = numRed + numBlue + numNeutral;
			int balance = numBlue - numRed;
			if(numBlue != total && numRed != total) {
				balance = 0;
			}
			float delta = ALLEGIANCE_UPDATE_INTERVAl * allegianceRate * (float)balance;
			Allegiance += delta;
			if(Allegiance <= -1.0f) {
				Allegiance = -1.0f;
				GlobalInterface.Singleton.GetTeamRessources(Team).numWorlds --;
				Team = Team.RED;
				GlobalInterface.Singleton.GetTeamRessources(Team).numWorlds ++;
			}
			if(Allegiance >= 1.0f) {
				Allegiance = 1.0f;
				GlobalInterface.Singleton.GetTeamRessources(Team).numWorlds --;
				Team = Team.BLUE;
				GlobalInterface.Singleton.GetTeamRessources(Team).numWorlds ++;
			}
			// update color
			Color newColor = AllegianceColor;
			if(newColor != lastColor) {
				Portal.SetColor(newColor);
				lastColor = newColor;
			}
			yield return new WaitForSeconds(ALLEGIANCE_UPDATE_INTERVAl);
		}
	}

	void Awake()
	{
		World = GetComponentInChildren<World>();
		World.WorldGroup = this;
		Portal = GetComponentInChildren<Portal>();
		Portal.WorldGroup = this;
		gui = GetComponentInChildren<WorldInterface>();
		gui.world = World;

		NumPlants = 0;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine("UpdateAllegiance");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
