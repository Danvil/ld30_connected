using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour {

	public WorldGroup a, b;

	LineRenderer line;

	// Use this for initialization
	void Start () {
		line = GetComponentInChildren<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		line.SetPosition(0, a.Portal.transform.position);
		line.SetPosition(1, b.Portal.transform.position);
		line.SetColors(
			Globals.Singleton.TeamColor(a.Team),
			Globals.Singleton.TeamColor(b.Team));
	}
}
