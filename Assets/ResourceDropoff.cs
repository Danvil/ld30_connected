using UnityEngine;
using System.Collections;

public class ResourceDropoff : MonoBehaviour {

	public Vector3 dropOffPoint = Vector3.zero;

	public Vector3 DropOffPointWorld
	{
		get { return this.transform.position + dropOffPoint; }
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(transform.position + dropOffPoint, 0.5f);
	}
}
