using UnityEngine;
using System.Collections;

public class GlobalInterface : MonoBehaviour {

	public static GlobalInterface Singleton;

	public UnityEngine.UI.Text txtWorlds;
	public UnityEngine.UI.Text txtRobots;
	public UnityEngine.UI.Text txtMinerals;
	public UnityEngine.UI.Text txtGoo;

	private int numWorlds = 0;
	public int NumWorlds
	{
		get { return numWorlds; }
		set {
			numWorlds = value;
			if(!txtWorlds) return;
			txtWorlds.text = string.Format("Worlds: {0}", numWorlds);
		}
	}

	private int numRobots = 0;
	public int NumRobots
	{
		get { return numRobots; }
		set {
			numRobots = value;
			if(!txtRobots) return;
			txtRobots.text = string.Format("Robots: {0}", numRobots);
		}
	}
	
	private float numMinerals = 0;
	public float NumMinerals
	{
		get { return numMinerals; }
		set {
			numMinerals = value;
			if(!txtMinerals) return;
			txtMinerals.text = string.Format("Minerals: {0:0.0}", numMinerals);
		}
	}
	
	private float numGoo = 0;
	public float NumGoo
	{
		get { return numGoo; }
		set {
			numGoo = value;
			if(!txtGoo) return;
			txtGoo.text = string.Format("Goo: {0:0.0}", numGoo);
		}
	}

	void Awake()
	{
		Singleton = this;
	}
	
	// Use this for initialization
	void Start () {
		NumWorlds = 0;
		NumRobots = 0;
		NumMinerals = 0;
		NumGoo = 0;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
