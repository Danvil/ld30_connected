using UnityEngine;
using System.Collections;

public class Interface : MonoBehaviour {

	public UnityEngine.UI.Text txtWorlds;
	public UnityEngine.UI.Text txtRobots;
	public UnityEngine.UI.Text txtSap;

	private int numWorlds = 0;
	public int NumWorlds
	{
		get { return numWorlds; }
		set {
			numWorlds = value;
			txtWorlds.text = string.Format("{0}", numWorlds);
		}
	}

	private int numRobots = 0;
	public int NumRobots
	{
		get { return numRobots; }
		set {
			numRobots = value;
			txtRobots.text = string.Format("{0}", numRobots);
		}
	}
	
	private int numSap = 0;
	public int NumSap
	{
		get { return numSap; }
		set {
			numSap = value;
			txtSap.text = string.Format("{0}", numSap);
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		
	}
}
