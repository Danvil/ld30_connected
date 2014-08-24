using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour {

	World world;

	public float speed = 1.70f;
	public float distToGoal = 0.07f;
	public float fallingSpinAngularVelocity = 90.0f;

	public float maxLaserDist = 4.0f;

	public float maxHaulDist = 1.0f;

	enum Status { Failure, Success, Running };

	// Use this for initialization
	void Start ()
	{
		falling = GetComponent<Falling>();
		laser = GetComponentInChildren<LaserArm>();
		trunk = GetComponentInChildren<Trunk>();
		world = GetComponent<WorldItem>().world;

		GlobalInterface.Singleton.NumRobots += 1;
	}

	void OnDestroy()
	{
		GlobalInterface.Singleton.NumRobots -= 1;
	}

	// Update is called once per frame
	void Update()
	{
		// check if we find something to laser nearby :D
		if(laser) {
			if(ActionDesintegrateCheckCurrent() || ActionDesintegrateSelect()) {
				if(ActionDesintegrate()) {
					return;
				}
			}
			else {
				ActionDesintegrateStop();
			}
		}
		// haul stuff
		if(trunk) {
			if(ActionHaulCheckCurrent() || ActionHaulSelect()) {
				if(ActionHaulRun()) {
					return;
				}
			}
			else {
				ActionHaulStop();
			}
		}
		// move around
		if(ActionMove()) {
			return;
		}
		ActionFindNewGoal();
	}

	#region Hauling

	Trunk trunk;
	Pickable haulTarget;

	bool HaulIsValidTarget(Pickable u)
	{
		// has target
		if(!u) {
			return false;
		}
		// target is not dead
		if(u.Depleted) {
			return false;
		}
		// near enough
		if((this.transform.position - u.transform.position).magnitude > maxHaulDist) {
			return false;
		}
		return true;
	}

	bool ActionHaulCheckCurrent()
	{
		return HaulIsValidTarget(haulTarget);
	}

	bool ActionHaulSelect()
	{
		if(trunk.IsFull) {
			return false;
		}
		// find hightest value
		float maxValue = 0.0f;
		foreach(Pickable t in world.FindTopObjects<Pickable>(this.transform.position, maxHaulDist)) {
			if(t && HaulIsValidTarget(t)) {
				float value = trunk.MaxCanLoad(t.type, t.Amount);
				if(haulTarget == null || value > maxValue) {
					maxValue = value;
					haulTarget = t;
				}
			}
		}
		return haulTarget;
	}

	bool ActionHaulRun()
	{
		float delta = Mathf.Max(Time.deltaTime * trunk.loadRate, haulTarget.Amount);
		delta = trunk.Load(haulTarget.type, delta);
		haulTarget.Gather(delta);
		return delta > 0.0f;
	}

	bool ActionHaulStop()
	{
		haulTarget = null;
		return true;
	}

	#endregion

	#region ActionDesintegrate

	Destroyable laserTarget;
	LaserArm laser;
	float desintegrateCooldown = 0.0f;

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

	const float MINING_THRESHOLD = 0.8f;
	const float MINING_LOW_RATE = 0.1f;
	const float MINING_SLEEP = 1.0f;

	float lowMineProb = 0.0f;

	bool ActionDesintegrateSelect()
	{
		desintegrateCooldown -= Time.deltaTime;
		// find hightest value
		foreach(Destroyable t in world.FindTopObjects<Destroyable>(this.transform.position, maxLaserDist)) {
			if(t && IsValidLaserTarget(t)) {
				if(laserTarget == null || t.dropAmount > laserTarget.dropAmount) {
					laserTarget = t;
				}
			}
		}
		if(!laserTarget) {
			return false;
		}
		// shall we really?
		// check value
		if(laserTarget.dropAmount > MINING_THRESHOLD) {
			lowMineProb = 0.0f;
			return true;
		}
		else {
			lowMineProb += Time.deltaTime * MINING_LOW_RATE;
			if(desintegrateCooldown > 0) {
				laserTarget = null;
				return false;
			}
			if(Random.value > Mathf.Min(0.95f,lowMineProb)) {
				desintegrateCooldown = MINING_SLEEP;
				laserTarget = null;
				return false;
			}
			return true;
		}
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
		Vector3 dir = new Vector3(MathTools.Random(-1.00f,+1.00f), 0, MathTools.Random(-1.00f,+1.00f));
		goal = this.transform.position + MathTools.Random(2.00f,4.00f)*dir.normalized;
		return true;
	}

	public void SetNewPosition(Vector3 p)
	{
		this.transform.position = p;
		ActionFindNewGoal();
	}

	Falling falling;

	Vector3 goal;
	float heading;

	void SetRotation(float spin)
	{
		this.transform.rotation = Quaternion.AngleAxis(spin, Vector3.up);
			//* Quaternion.AngleAxis(-90.0f, new Vector3(1,0,0));
	}

	bool ActionMove()
	{
		// position
		Vector3 pos = this.transform.position;
		// fall
		if(falling && falling.IsFalling) {
			// fall down and spiral
			//heading += Time.deltaTime*fallingSpinAngularVelocity;
			//SetRotation(heading);
			// we are falling
			return true;
		}
		else {
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
				if(!falling || falling.CheckIfSafe(newpos)) {
					//newpos.y = topVoxel.z + 1.5f;
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
