using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WorldItem))]
public class Falling : MonoBehaviour {

	WorldItem wi;

	public bool forceHeight = false;
	public float baseHeight = 0.0f;

	const float DEATH_HEIGHT = -100.0f;

	public bool IsFalling { get; private set; }

	Vector3 velocity = Vector3.zero;
	Int3 topVoxel;

	public bool TrySetNewLocalPosition(Vector3 pos)
	{
		Int3 newTopVoxel;
		if(wi.world.Voxels.TryGetTopVoxel(pos.ToInt3(), out newTopVoxel)) {
			// safe
			topVoxel = newTopVoxel;
			this.transform.localPosition = pos;
			return true;
		}
		else {
			// not safe
			return false;
		}
	}

	public void SetNewLocalPosition(Vector3 pos)
	{
		wi.world.Voxels.TryGetTopVoxel(pos.ToInt3(), out topVoxel);
		this.transform.localPosition = pos;
	}

	void Awake()
	{
		wi = GetComponent<WorldItem>();
	}

	// Use this for initialization
	void Start ()
	{
		IsFalling = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		// position
		Vector3 pos = this.transform.localPosition;
		// test if falling
		IsFalling = (pos.y > topVoxel.z + 1.5f);
		if(IsFalling) {
			// fall down
			pos += Time.deltaTime * velocity;
			velocity += Time.deltaTime * wi.world.gravity;
			// check if we fall to death
			if(pos.y < DEATH_HEIGHT) {
				Destroy(gameObject);
			}
		}
		else {
			if(forceHeight) {
				pos.y = topVoxel.z + 1.0f + baseHeight;
			}
			velocity = Vector3.zero;
		}
		this.transform.localPosition = pos;
	}
}
