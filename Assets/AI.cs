using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : MonoBehaviour {

	Team myteam;

	Team enemy;

	// Use this for initialization
	void Start () {
		myteam = Globals.Singleton.nonPlayerTeam;
		enemy = Globals.Singleton.playerTeam;
	}

	// Update is called once per frame
	void Update () {
		// get worlds
		WorldGroup[] worlds = Galaxy.Singleton.GetWorlds().ToArray();
		// count factories and drillers
		int numFactories = 0;
		int numDrillers = 0;
		foreach(var wg in worlds) {
			if(wg.Team == myteam && wg.World.Building) {
				if(wg.World.Building.GetComponent<DrillingPlatform>()) {
					numDrillers += 1;
				}
				else {
					numFactories += 1;
				}
			}
		}
		// build something
		foreach(var wg in worlds) {
			if(wg.Team == myteam) {
				// make sure we built a building
				if(!wg.World.Building) {
					if(numFactories > numDrillers) {
						wg.World.BuildDriller();
					}
					else {
						wg.World.BuildFactory();
					}
				}
			}
		}
		// move robots around
		foreach(var wg in worlds) {
			int numMyHaulers = 0;
			int numMyLaser = 0;
			foreach(var r in wg.World.FindRobots()) {
				if(r.wi.Team == myteam && r.robType == RobType.HAUL) numMyHaulers ++;
				if(r.wi.Team == myteam && r.robType == RobType.LASER) numMyLaser ++;
			}
			if(numMyHaulers > 5) {
				wg.Portal.MoveUp(myteam, RobType.LASER);
			}
		}		
	}
}
