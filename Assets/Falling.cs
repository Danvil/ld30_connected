using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WorldItem))]
public class Falling : MonoBehaviour {

	public bool forceHeight = false;
	public float baseHeight = 0.0f;

	const float DEATH_HEIGHT = -100.0f;

	public bool IsFalling { get; private set; }

	Vector3 velocity = Vector3.zero;
	Int3 lastVoxelPos;
	Int3 topVoxel;

	public World world;

	public bool CheckIfSafe(Vector3 pos)
	{
		return world.Voxels.HasTopVoxel(pos.ToInt3());
	}

	// Use this for initialization
	void Start ()
	{
		IsFalling = false;
		world = GetComponent<WorldItem>().world;
	}
	
	// Update is called once per frame
	void Update()
	{
		// position
		Vector3 pos = this.transform.localPosition;
		Int3 ipos = pos.ToInt3();
		// find top voxel
		//if(ipos != lastVoxelPos) {
			lastVoxelPos = ipos;
			world.Voxels.TryGetTopVoxel(lastVoxelPos, out topVoxel);
		//}
		// test if falling
		IsFalling = (pos.y > topVoxel.z + 1.5f);
		if(IsFalling) {
			// fall down
			pos += Time.deltaTime * velocity;
			velocity += Time.deltaTime * world.gravity;
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
