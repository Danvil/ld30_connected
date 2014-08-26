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

	float OneOverTanAlpha
	{
		get { return 1.0f / Mathf.Tan(Mathf.Deg2Rad * camera.transform.rotation.eulerAngles.x); }
	}
	
	// Update is called once per frame
	void Update () {
		// scroll
		{
			float v = Input.GetAxis("Mouse ScrollWheel");
			float heightold = height;
			height -= Time.deltaTime * scrollRate * v;
			height = Mathf.Min(Mathf.Max(heightLow,height), heightHigh);
			float dz = (heightold - height) * OneOverTanAlpha;
			camera.transform.position = new Vector3(camera.transform.position.x,height,camera.transform.position.z+dz);
		}
		// center
		if(Input.GetKeyDown(KeyCode.G)) {
			Vector3 mp = Galaxy.Singleton.WorldsMeanPoint();
			height = heightHigh;
			float z = height * OneOverTanAlpha;
			camera.transform.position = mp + new Vector3(0,height,-z);
		}
		// home
		if(Input.GetKeyDown(KeyCode.H)) {
			Vector3 mp = Vector3.zero; // TODO get home world coordinates
			height = heightHigh;
			float z = height * OneOverTanAlpha;
			camera.transform.position = mp + new Vector3(0,height,-z);
		}
	}
}
