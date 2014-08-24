using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour {

	public WorldGroup a, b;

	public GameObject pfConnectionCanvas;

	public float transitSpeed = 5.0f;

	GameObject guiA, guiB;

	LineRenderer line;

	void Send(WorldGroup src, WorldGroup dst, RobType rt)
	{
		Portal psrc = src.Portal;
		Portal pdst = dst.Portal;
		Robot r = psrc.RemoveRobot(rt);
		if(!r) {
			return;
		}
		StartCoroutine("Transit", 
			new object[] { r, psrc, pdst });
	}

	IEnumerator Transit(object[] p) {
		Robot r = (Robot)p[0];
		Portal psrc = (Portal)p[1];
		r.transform.position = psrc.transform.position;
		Portal pdst = (Portal)p[2];
		Vector3 target = pdst.transform.position;
		while(true) {
			Vector3 pos = r.transform.position;
			Vector3 dir = target - pos;
			if(dir.magnitude < 1.0f) {
				break;
			}
			pos += Time.deltaTime * transitSpeed * dir.normalized;
			r.transform.position = pos;
			yield return null;
		}
		pdst.AddRobot(r);
	}

	// Use this for initialization
	void Start () {
		line = GetComponentInChildren<LineRenderer>();

		guiA = (GameObject)Instantiate(pfConnectionCanvas);
		guiA.transform.parent = this.transform;
		guiB = (GameObject)Instantiate(pfConnectionCanvas);
		guiB.transform.parent = this.transform;

		guiA.GetComponent<UnityEngine.Canvas>().worldCamera = WorldSelector.Singleton.camera;
		guiB.GetComponent<UnityEngine.Canvas>().worldCamera = WorldSelector.Singleton.camera;

		UnityEngine.UI.Button x;

		x = guiA.transform.Search("ButtonHaul").GetComponent<UnityEngine.UI.Button>();
		x.onClick.AddListener(() => { Send(a, b, RobType.HAUL); });

		x = guiA.transform.Search("ButtonLaser").GetComponent<UnityEngine.UI.Button>();
		x.onClick.AddListener(() => { Send(a, b, RobType.LASER); });

		x = guiB.transform.Search("ButtonHaul").GetComponent<UnityEngine.UI.Button>();
		x.onClick.AddListener(() => { Send(b, a, RobType.HAUL); });

		x = guiB.transform.Search("ButtonLaser").GetComponent<UnityEngine.UI.Button>();
		x.onClick.AddListener(() => { Send(b, a, RobType.LASER); });


	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pa = a.Portal.transform.position;
		Vector3 pb = b.Portal.transform.position;

		guiA.transform.position = pa + 8.0f * (pb - pa).normalized + new Vector3(0,4,-2);
		guiB.transform.position = pb + 8.0f * (pa - pb).normalized + new Vector3(0,4,-2);


		line.SetPosition(0, pa);
		line.SetPosition(1, pb);
		line.SetColors(
			Globals.Singleton.TeamColor(a.Team),
			Globals.Singleton.TeamColor(b.Team));
	}
}
