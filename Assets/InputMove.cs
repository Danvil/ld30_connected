using UnityEngine;
using System.Collections;

[AddComponentMenu("Danvil/Input Move")]
public class InputMove : MonoBehaviour {
	
	public float speed = 1.0f;
	public bool relativeMove = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dz = Input.GetAxis("Vertical");
		float dx = Input.GetAxis("Horizontal");
		Vector3 delta = Time.deltaTime*speed*(new Vector3(dx,0,dz)).Limit(1.0f);
		if(relativeMove) {
			delta = this.transform.rotation * delta;
		}
		this.transform.position += delta;

	}
}
