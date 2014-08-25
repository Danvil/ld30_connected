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

	WorldGroup CreateWorld(Vector3 pos)
	{
		GameObject go = (GameObject)Instantiate(pfWorld);
		go.transform.position = pos;
		WorldGroup wg = go.GetComponent<WorldGroup>();
		wg.Team = Team.NEUTRAL;
		AddWorld(wg);
		return wg;
	}

	void GenerateGalaxyOne()
	{
		CreateWorld(Vector3.zero);
		worlds[0].World.initRobotTeam = Globals.Singleton.playerTeam;		
	}

	void GenerateGalaxySimple()
	{
		const float SPACE = 80.0f;
		const float RND = 0.2f;

		for(int y=0; y<2; y++) {
			for(int x=0; x<2; x++) {
				Vector3 pos = new Vector3(SPACE*x,0,SPACE*y) + SPACE * RND * new Vector3(Tools.Random(-1f,+1f),0,Tools.Random(-1f,+1f));
				CreateWorld(pos);
			}
		}

		worlds[0].World.initRobotTeam = Globals.Singleton.playerTeam;
		worlds[3].World.initRobotTeam = Globals.Singleton.nonPlayerTeam;

		AddConnection(worlds[0], worlds[1]);
		AddConnection(worlds[0], worlds[2]);
		AddConnection(worlds[3], worlds[1]);
		AddConnection(worlds[3], worlds[2]);

	}

	WorldGroup TryPlace(List<WorldGroup> open, float size, float rmin, float rmax)
	{
		float sizefudge = size;
		while(true) {
			var wg = open.RandomSample();
			float r = Tools.Random(rmin, rmax);
			float a = Tools.Random(0, 2.0f*Mathf.PI);
			Vector3 p = wg.transform.position + r * new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
			if(0 <= p.x && p.x <= sizefudge && 0 <= p.z && p.z <= sizefudge) {
				if(!worlds.Any(x => (x.transform.position - p).magnitude < rmin)) {
					return CreateWorld(p);
				}
			}
			sizefudge += 0.05f*size;
		}
	}

	void GenerateGalaxyOrganic()
	{
		const int NUM = 2;
		const float SIZE = 130.0f;
		const float R_MIN = 55.0f;
		const float R_MAX = 90.0f;


		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = new Vector3(SIZE,0,SIZE);

		// initial worlds
		CreateWorld(pos1);
		worlds[0].World.initRobotTeam = Globals.Singleton.playerTeam;
		CreateWorld(pos2);
		worlds[1].World.initRobotTeam = Globals.Singleton.nonPlayerTeam;

		// grow galaxy
		List<WorldGroup> open1 = new List<WorldGroup>();
		open1.Add(worlds[0]);
		List<WorldGroup> open2 = new List<WorldGroup>();
		open2.Add(worlds[1]);
		for(int i=0; i<NUM; i++) {
			open1.Add(TryPlace(open1, SIZE, R_MIN, R_MAX));
			open2.Add(TryPlace(open2, SIZE, R_MIN, R_MAX));
		}

		// connections
		for(int i=0; i<worlds.Count; i++) {
			var a = worlds[i];
			for(int j=i+1; j<worlds.Count; j++) {
				var b = worlds[j];
				float d = (a.transform.position - b.transform.position).magnitude;
				if(d <= R_MAX) {
					AddConnection(a, b);
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		GenerateGalaxyOrganic();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
