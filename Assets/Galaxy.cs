using UnityEngine;
using System.Collections;

public class Galaxy : MonoBehaviour {

	public GameObject pfWorld;

	// Use this for initialization
	void Start () {
	
		float SPACE = 50.0f;

		for(int i=-1; i<=+1; i++) {
			GameObject w = (GameObject)Instantiate(pfWorld);
			w.transform.position = new Vector3(SPACE*i,0,0);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
