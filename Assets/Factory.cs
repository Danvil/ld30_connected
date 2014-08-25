using UnityEngine;
using System.Collections;

public class Factory : MonoBehaviour {

	public Vector3 createPoint = Vector3.zero;

	public Vector3 exitPoint = new Vector3(3,0,0);

	public GameObject pfBlueprint;

	public float constructionRate = 10.0f;

	public bool enableConstruction = true;

	public float costsMinerals = 10.0f;
	public float costsGoo = 5.0f;

	GameObject construct;

	World world;

	// Use this for initialization
	void Start()
	{
		world = this.GetComponent<WorldItem>().world;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(enableConstruction && !construct && pfBlueprint) {
			var gis = GlobalInterface.Singleton;
			if(gis.NumMinerals >= costsMinerals && gis.NumGoo >= costsGoo) {
				gis.NumMinerals -= costsMinerals;
				gis.NumGoo -= costsGoo;
				StartCoroutine("Construct");
			}
		}
	}

	IEnumerator Construct()
	{
		Debug.LogWarning("Starting construction");
		// create
		construct = (GameObject)Instantiate(pfBlueprint);
		construct.transform.parent = this.transform;
		construct.transform.localPosition = createPoint;
		construct.transform.localScale = 0.1f * Vector3.one;
		foreach(var s in construct.GetComponents<MonoBehaviour>()) {
			s.enabled = false;
		}
		// construction
		float t = constructionRate;
		while(t > 0) {
			if(world.AllowProduction) {
				t -= Time.deltaTime;
				float p = 1.0f - t / constructionRate;
				construct.transform.localScale = (0.1f+0.9f*p) * Vector3.one;
			}
			yield return null;
		}
		construct.transform.localScale = Vector3.one;
		// let loose
		construct.transform.localPosition = exitPoint;
		construct.transform.parent = world.transform;
		WorldItem wi = construct.GetComponent<WorldItem>();
		wi.MoveToWorld(world);
		wi.Team = world.WorldGroup.Team;
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
