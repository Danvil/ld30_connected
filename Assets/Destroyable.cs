using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WorldItem))]
public class Destroyable : MonoBehaviour {

	public GameObject pfDropping;

	public float dropAmount = 1.0f;

	public bool destroyVoxel = false;

	public float decayRate = 2.0f;

	public float maxHealth = 30.0f;

	public float Health { get; set; }

	public bool Dead
	{
		get { return Health <= 0.0f; }
	}

	WorldItem wi;

	bool hasDropped = false;

	// Use this for initialization
	void Start()
	{
		wi = GetComponent<WorldItem>();
		Health = maxHealth;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Dead && !hasDropped) {
			// create drop
			GameObject go = (GameObject)Instantiate(pfDropping);
			go.transform.parent = wi.world.transform;
			go.transform.position = this.transform.position;
			go.GetComponent<Pickable>().maxAmount = dropAmount;
			wi.world.Add(go.GetComponent<WorldItem>());
			// destroy voxel
			if(destroyVoxel) {
				var ip = this.transform.localPosition.ToInt3();
				wi.world.DestroyVoxel(ip);
			}
			// start fading
			StartCoroutine("Fade");
			hasDropped = true;
		}
	}

	IEnumerator Fade() {
		while(this.transform.localScale.magnitude > 0.1f) {
			this.transform.localScale *= (1.0f - decayRate*Time.deltaTime);
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
