using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class Destroyable : MonoBehaviour {

	public Entity entity { get; private set; }

	public GameObject pfDropping;

	public float dropAmount = 1.0f;

	public bool destroyVoxel = false;

	public float deadScaleDecayRate = 2.0f;

	public float maxHealth = 30.0f;

	public bool isRobot = false;

	public float Health { get; set; }

	public bool Dead
	{
		get { return Health <= 0.0f; }
	}

	bool hasDropped = false;

	void Awake()
	{
		entity = GetComponent<Entity>();
		entity.destroyable = this;
	}

	// Use this for initialization
	void Start()
	{
		Health = maxHealth;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Dead && !hasDropped) {
			// create drop
			GameObject go = (GameObject)Instantiate(pfDropping);
			go.transform.parent = entity.world.transform;
			go.transform.position = this.transform.position;
			go.GetComponent<Pickable>().maxAmount = dropAmount;
			entity.world.Add(go.GetComponent<Entity>());
			// destroy voxel
			if(destroyVoxel) {
				var ip = this.transform.localPosition.ToInt3();
				entity.world.DestroyVoxel(ip);
			}
			// start fading
			hasDropped = true;
			StartCoroutine("Fade");
		}
	}

	IEnumerator Fade() {
		// fade out
		while(this.transform.localScale.magnitude > 0.1f) {
			this.transform.localScale *= (1.0f - deadScaleDecayRate*Time.deltaTime);
			yield return null;
		}
		// unlink from world
		entity.MoveToSpace();
		// finally destroy it completely
		Destroy(this.gameObject);
	}
}
