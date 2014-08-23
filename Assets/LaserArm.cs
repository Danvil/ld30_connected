﻿using UnityEngine;
using System.Collections;

public class LaserArm : MonoBehaviour {

	LineRenderer laser;

	public float damageRate = 10.0f;

	public bool LaserEnabled
	{
		get { return laser.enabled; }
		set { laser.enabled = value; }
	}

	public Destroyable Target { get; set; }

	// Use this for initialization
	void Start () {
		laser = GetComponentInChildren<LineRenderer>();
		LaserEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(LaserEnabled) {
			Vector3 tp = Target.transform.position + new Vector3(0,0.5f,0);
			this.transform.rotation = Quaternion.FromToRotation(
				Vector3.forward, tp - this.transform.position);
			float r = (tp - this.transform.position).magnitude;
			laser.SetPosition(1, new Vector3(0,0,r));
			Target.Health -= Time.deltaTime * damageRate;
		}
		else {
			this.transform.localRotation = Quaternion.identity;
		}
	}
}
