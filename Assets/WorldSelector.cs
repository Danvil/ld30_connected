using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldSelector : MonoBehaviour {

	public static WorldSelector Singleton;

	public float heightLow = 30.0f;
	public float heightHigh = 70.0f;

	void Awake()
	{
		Singleton = this;
	}

	// Use this for initialization
	void Start () {
		height = camera.transform.position.y;
	}

	public float scrollRate = 10.0f;

	float height;
	
	// Update is called once per frame
	void Update () {
		// scroll
		{
			float v = Input.GetAxis("Mouse ScrollWheel");
			float heightold = height;
			height -= Time.deltaTime * scrollRate * v;
			height = Mathf.Min(Mathf.Max(heightLow,height), heightHigh);
			float x = camera.transform.position.x;
			float z = camera.transform.position.z * height / heightold;
			camera.transform.position = new Vector3(x,height,z);
		}
		// home
		if(Input.GetKeyDown(KeyCode.H)) {
			Vector3 mp = Galaxy.Singleton.WorldsMeanPoint();
			float z = heightHigh / Mathf.Tan(Mathf.Deg2Rad * camera.transform.rotation.eulerAngles.x);
			camera.transform.position = mp + new Vector3(0,heightHigh,-z);
			height = heightHigh;
		}
	}
}
