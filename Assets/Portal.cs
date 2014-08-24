using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Portal : MonoBehaviour {

	public Vector3 swirlPoint = new Vector3(0,-5,0);
	public Vector3 landPoint = new Vector3(0,-15,0);

	public float beamSpeed = 3.0f;

	public WorldGroup WorldGroup;

	public void SetColor(Color color)
	{
		this.GetComponentInChildren<Renderer>().material.color = 0.65f*color;
	}

	Dictionary<RobType,List<Robot>> robots = new Dictionary<RobType,List<Robot>>();

	Dictionary<RobType,UnityEngine.UI.Text> txtNum = new Dictionary<RobType,UnityEngine.UI.Text>();

	public void MoveUp(Team team, RobType rt)
	{
		var w = WorldGroup.World;
		Robot robot = w
			.FindRobots()
			.Where(r => r.Team == team && r.robType == rt)
			.RandomSample();
		if(!robot) {
			return;
		}
		robot.GetComponent<WorldItem>().MoveToSpace();
		System.Action<Robot> final = r => {
			robots[rt].Add(robot);
			UpdateNumText(rt);
		};
		StartCoroutine("Beam",
			new object[3]{ robot, transform.position + swirlPoint, final});
	}

	void UpdateNumText(RobType rt)
	{
		txtNum[rt].text = string.Format("{0}", robots[rt].Count);
	}

	public void MoveDown(Team team, RobType rt)
	{
		Robot robot = robots[rt]
			.Where(r => r.Team == team)
			.RandomSample();
		if(!robot) {
			return;
		}
		robots[rt].Remove(robot);
		UpdateNumText(rt);
		System.Action<Robot> final = r => {
			robot.GetComponent<WorldItem>().MoveToWorld(WorldGroup.World);
		};
		StartCoroutine("Beam", 
			new object[3]{ robot, transform.position + swirlPoint, final});
	}

	public Robot RemoveRobot(RobType rt) {
		Robot r = robots[rt].RandomSample();
		if(!r) {
			return r;
		}
		robots[rt].Remove(r);
		UpdateNumText(rt);
		return r;
	}

	public void AddRobot(Robot r) {
		robots[r.robType].Add(r);
		UpdateNumText(r.robType);
	}

	IEnumerator Beam(object[] p)
	{
		Robot robot = (Robot)p[0];
		Vector3 target = (Vector3)p[1];
		System.Action<Robot> final = (System.Action<Robot>)p[2];
		while(true) {
			Vector3 pos = robot.transform.position;
			Vector3 dir = target - pos;
			if(dir.magnitude < 1.0f) {
				break;
			}
			pos += Time.deltaTime * beamSpeed * dir.normalized;
			robot.transform.position = pos;
			yield return null;
		}
		final(robot);
	}

	// Use this for initialization
	void Start () {
		robots[RobType.HAUL] = new List<Robot>();
		robots[RobType.LASER] = new List<Robot>();

		GetComponentInChildren<UnityEngine.Canvas>().worldCamera = WorldSelector.Singleton.camera;

		txtNum[RobType.HAUL] = this.transform.Search("TextHaul").GetComponent<UnityEngine.UI.Text>();

		var btnHaulUp = this.transform.Search("ButtonHaulUp").GetComponent<UnityEngine.UI.Button>();
		btnHaulUp.onClick.AddListener(() => MoveUp(Globals.Singleton.playerTeam, RobType.HAUL));
		
		var btnHaulDown = this.transform.Search("ButtonHaulDown").GetComponent<UnityEngine.UI.Button>();
		btnHaulDown.onClick.AddListener(() => MoveDown(Globals.Singleton.playerTeam, RobType.HAUL));
		
		txtNum[RobType.LASER] = this.transform.Search("TextLaser").GetComponent<UnityEngine.UI.Text>();
		
		var btnLaserUp = this.transform.Search("ButtonLaserUp").GetComponent<UnityEngine.UI.Button>();
		btnLaserUp.onClick.AddListener(() => MoveUp(Globals.Singleton.playerTeam, RobType.LASER));
		
		var btnLaserDown = this.transform.Search("ButtonLaserDown").GetComponent<UnityEngine.UI.Button>();
		btnLaserDown.onClick.AddListener(() => MoveDown(Globals.Singleton.playerTeam, RobType.LASER));
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position + swirlPoint, 0.5f);
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(transform.position + landPoint, 0.5f);
	}

}
