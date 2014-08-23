using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LaserArm))]
public class Robot : MonoBehaviour {

	public World world;

	public float speed = 1.70f;
	public float distToGoal = 0.07f;
	public float fallingSpinAngularVelocity = 90.0f;

	public float maxLaserDist = 5.0f;

	enum Status { Failure, Success, Running };

	// Use this for initialization
	void Start ()
	{
		laser = GetComponentInChildren<LaserArm>();
	}

	// Update is called once per frame
	void Update()
	{
		// check if we find something to laser nearby :D
		laser.LaserEnabled = false;
		if(ActionDesintegrateCheckCurrent()
			|| ActionDesintegrateSelect()
		) {
			if(ActionDesintegrate()) {
				return;
			}
		}
		else {
			ActionDesintegrateStop();
		}
		if(ActionMove()) {
			return;
		}
		ActionFindNewGoal();
	}

	#region ActionDesintegrate

	Destroyable laserTarget;
	LaserArm laser;

	bool IsValidLaserTarget(Destroyable u)
	{
		// has target
		if(!u) {
			return false;
		}
		// target is not dead
		if(u.Dead) {
			return false;
		}
		// near enough
		if((this.transform.position - u.transform.position).magnitude > maxLaserDist) {
			return false;
		}
		return true;
	}

	bool ActionDesintegrateCheckCurrent()
	{
		return IsValidLaserTarget(laserTarget);
	}

	bool ActionDesintegrateSelect()
	{
		foreach(var go in world.FindObjectsInSphere(this.transform.position, maxLaserDist)) {
			laserTarget = go.GetComponent<Destroyable>();
			if(laserTarget && IsValidLaserTarget(laserTarget)) {
				return true;
			}
		}
		return false;
	}

	bool ActionDesintegrate()
	{
		laser.LaserEnabled = true;
		laser.Target = laserTarget;
		return true;
	}

	bool ActionDesintegrateStop()
	{
		laser.LaserEnabled = false;
		return true;
	}

	#endregion
	
	#region ActionMove

	public bool ActionFindNewGoal()
	{
		// move randomly
		Vector3 dir = new Vector3(MathTools.Random(-3.00f,+3.00f), 0, MathTools.Random(-3.00f,+3.00f));
		goal = this.transform.position + dir;
		return true;
	}

	public void SetNewPosition(Vector3 p)
	{
		world.Voxels.TryGetTopVoxel(p.ToInt3(), out topVoxel);
		this.transform.position = p;
		ActionFindNewGoal();
	}

	Vector3 goal;
	Vector3 velocity = Vector3.zero;
	float heading;
	bool isFalling = false;
	Int3 topVoxel;

	void SetRotation(float spin)
	{
		this.transform.rotation = Quaternion.AngleAxis(spin, Vector3.up);
			//* Quaternion.AngleAxis(-90.0f, new Vector3(1,0,0));
	}

	bool ActionMove()
	{
		// position
		Vector3 pos = this.transform.position;
		// test if falling
		isFalling = (pos.y > topVoxel.z + 1.5f);
		// fall
		if(isFalling) {
			// fall down
			pos += Time.deltaTime * velocity;
			velocity += Time.deltaTime * world.gravity;
			this.transform.position = pos;
			heading += Time.deltaTime*fallingSpinAngularVelocity;
			SetRotation(heading);
			// check if we fall to death
			// FIXME
			// we are falling
			return true;
		}
		else {
			velocity = Vector3.zero;
			// move to goal
			Vector3 dir = goal - pos;
			dir.y = 0.0f;
			float dirlen = dir.magnitude;
			if(dirlen <= distToGoal) {
				// new goal
				return false;
			}
			else {
				// move to goal
				heading = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg;
				Vector3 newpos = pos + Time.deltaTime * speed * dir.normalized;
				Int3 newTopVoxel;
				if(world.Voxels.TryGetTopVoxel(newpos.ToInt3(), out newTopVoxel)) {
					newpos.y = topVoxel.z + 1.5f;
					topVoxel = newTopVoxel;
					goal.y = newpos.y;
					this.transform.position = newpos;
					SetRotation(heading);
					// still moving
					return true;
				}
				else {
					// can not reach goal
					return false;
				}
			}
		}
	}

	#endregion

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, goal);
	}
}
