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
	
		float SPACE = 50.0f;

		for(int i=-1; i<=+1; i++) {
			GameObject go = (GameObject)Instantiate(pfWorld);
			go.transform.position = new Vector3(SPACE*i,0,0);
			WorldGroup wg = go.GetComponentInChildren<WorldGroup>();
			AddWorld(wg);
		}

		AddConnection(worlds[0], worlds[1]);
		AddConnection(worlds[1], worlds[2]);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
