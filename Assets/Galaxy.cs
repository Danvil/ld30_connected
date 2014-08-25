using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Galaxy : MonoBehaviour {

	public static Galaxy Singleton;

	public GameObject pfWorld;
	public GameObject pfConnection;

	List<WorldGroup> worlds = new List<WorldGroup>();

	List<Connection> connections = new List<Connection>();

	void AddWorld(WorldGroup w)
	{
		worlds.Add(w);
	}

	void AddConnection(WorldGroup a, WorldGroup b)
	{
		GameObject go = (GameObject)Instantiate(pfConnection);
		go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;
		Connection c = go.GetComponent<Connection>();
		c.a = a;
		c.b = b;
		connections.Add(c);
	}

	public IEnumerable<WorldGroup> GetWorlds()
	{
		return worlds;
	}

	public IEnumerable<Connection> GetConnections(WorldGroup w)
	{
		foreach(var c in connections) {
			if(c.a == w || c.b == w) {
				yield return c;
			}
		} 
	}

	public Vector3 WorldsMeanPoint()
	{
		return worlds.Select(w => w.World.transform.position).Aggregate((a,b)=>a+b) / (float)worlds.Count;
	}

	void Awake()
	{
		Singleton = this;
	}

	// Use this for initialization
	void Start () {
	
		const float SPACE = 80.0f;
		const float RND = 0.2f;

		for(int y=0; y<2; y++) {
			for(int x=0; x<2; x++) {
				GameObject go = (GameObject)Instantiate(pfWorld);
				go.transform.position = new Vector3(SPACE*x,0,SPACE*y) + SPACE * RND * new Vector3(Tools.Random(-1f,+1f),0,Tools.Random(-1f,+1f));
				WorldGroup wg = go.GetComponent<WorldGroup>();
				wg.Team = Team.NEUTRAL;
				AddWorld(wg);
			}
		}

		worlds[0].World.initRobotTeam = Globals.Singleton.playerTeam;
		worlds[3].World.initRobotTeam = Globals.Singleton.nonPlayerTeam;

		AddConnection(worlds[0], worlds[1]);
		AddConnection(worlds[0], worlds[2]);
		AddConnection(worlds[3], worlds[1]);
		AddConnection(worlds[3], worlds[2]);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
