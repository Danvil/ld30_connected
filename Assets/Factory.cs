using UnityEngine;
using System.Collections;

public class Factory : MonoBehaviour {

	public Vector3 createPoint = Vector3.zero;

	public Vector3 exitPoint = new Vector3(3,0,0);

	public GameObject pfBlueprint;

	public float constructionRate = 10.0f;

	public bool enableConstruction = true;

	GameObject construct;

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(enableConstruction && !construct && pfBlueprint) {
			StartCoroutine("Construct");
		}
	}

	IEnumerator Construct()
	{
		Debug.LogWarning("Starting construction");
		// create
		construct = (GameObject)Instantiate(pfBlueprint);
		construct.transform.parent = this.transform;
		construct.transform.localPosition = createPoint;
		foreach(var s in construct.GetComponents<MonoBehaviour>()) {
			s.enabled = false;
		}
		// construction
		float t = constructionRate;
		while(t > 0) {
			t -= Time.deltaTime;
			float p = 1.0f - t / constructionRate;
			construct.transform.localScale = p * Vector3.one;
			yield return null;
		}
		// let loose
		construct.transform.localPosition = exitPoint;
		construct.transform.parent = null;
		foreach(var s in construct.GetComponents<MonoBehaviour>()) {
			s.enabled = true;
		}
		Debug.LogWarning("Finished construction");
		construct = null;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + createPoint, 0.5f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position + exitPoint, 0.5f);
	}
}
