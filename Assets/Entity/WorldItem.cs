using UnityEngine;
using System.Collections;

public class WorldItem : MonoBehaviour {

	public World world;

	public void MoveToSpace()
	{
		this.transform.parent = null;
		world.Remove(this);
		world = null;
		var fall = GetComponent<Falling>();
		if(fall) {
			fall.enabled = false;
		}
	}

	public void MoveToWorld(World w)
	{
		world = w;
		this.transform.parent = world.transform;
		world.Add(this);
		var fall = GetComponent<Falling>();
		if(fall) {
			fall.enabled = true;
			fall.SetNewLocalPosition(this.transform.localPosition);
		}
		var robot = GetComponent<Robot>();
		if(robot) {
			robot.SetRandomGoal();
		}
	}

	private Team team;

	public Team Team {
		get { return team; }
		set
		{
			team = value;
			Robot r = GetComponent<Robot>();
			if(r) {
				r.UpdateTeamColor();
			}
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		if(world) {
			world.Remove(this);
		}
    }
}
