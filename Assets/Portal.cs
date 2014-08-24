using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	public void SetColor(Color color)
	{
		this.GetComponentInChildren<Renderer>().material.color = 0.65f*color;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

}
