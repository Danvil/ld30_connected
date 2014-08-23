using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour {

	public World world;

	public float speed = 1.70f;
	public float distToGoal = 0.07f;
	public float fallingSpinAngularVelocity = 90.0f;

	Vector3 goal;
	Vector3 velocity = Vector3.zero;
	float heading;
	bool isFalling = false;
	Int3 topVoxel;

	// Use this for initialization
	void Start () {
	}

	public void FindNewGoal()
	{
		// move randomly
		Vector3 dir = new Vector3(MathTools.Random(-3.00f,+3.00f), 0, MathTools.Random(-3.00f,+3.00f));
		goal = this.transform.position + dir;
	}

	public void SetNewPosition(Vector3 p)
	{
		world.Voxels.TryGetTopVoxel(p.ToInt3(), out topVoxel);
		this.transform.position = p;
		FindNewGoal();
	}
	
	// Update is called once per frame
	void Update () {
		// position
		Vector3 pos = this.transform.position;
		Int3 ipos = pos.ToInt3();
		// test if falling
		isFalling = (pos.y > topVoxel.z + 1);
		// fall
		if(isFalling) {
			pos += Time.deltaTime * velocity;
			velocity += Time.deltaTime * world.gravity;
			this.transform.position = pos;
			heading += Time.deltaTime*fallingSpinAngularVelocity;
			SetRotation(heading);
		}
		else {
			velocity = Vector3.zero;
			// move to goal
			Vector3 dir = goal - pos;
			dir.y = 0.0f;
			float dirlen = dir.magnitude;
			if(dirlen <= distToGoal) {
				// new goal
				FindNewGoal();
			}
			else {
				// move to goal
				heading = Mathf.Atan2(-dir.x,-dir.z) * Mathf.Rad2Deg;
				Vector3 newpos = pos + Time.deltaTime * speed * dir.normalized;
				Int3 newTopVoxel;
				if(world.Voxels.TryGetTopVoxel(newpos.ToInt3(), out newTopVoxel)) {
					newpos.y = topVoxel.z + 1;
					topVoxel = newTopVoxel;
					goal.y = newpos.y;
					this.transform.position = newpos;
					SetRotation(heading);
				}
				else {
					// can not reach goal.
					FindNewGoal();
				}
			}
		}
	}

	void SetRotation(float spin)
	{
		this.transform.rotation = Quaternion.AngleAxis(spin, Vector3.up)
			* Quaternion.AngleAxis(-90.0f, new Vector3(1,0,0));
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, goal);
	}
}
