using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum RobType { HAUL, LASER };

[RequireComponent(typeof(WorldItem))]
public class Robot : MonoBehaviour {

	WorldItem wi;

	const float FIND_TOP_OBJS_COOLDOWN = 0.25f;

	public float speed = 1.70f;
	public float distToGoal = 0.07f;
	public float fallingSpinAngularVelocity = 90.0f;

	public float maxLaserDist = 3.0f;

	public float maxHaulDist = 1.0f;

	public float searchRadius = 4.0f;

	public RobType robType;

	enum Status { Failure, Success, Running };

	Team team;
	public Team Team
	{
		get { return team; }
		set {
			team = value;
			Material mat = Globals.Singleton.TeamMaterial(team);
			Transform t;

			if(renderer) renderer.material = mat;
			
			t = this.transform.FindChild("HaulerMesh");
			if(t) t.gameObject.renderer.material = mat;

			t = this.transform.FindChild("Arm");
			if(t) t.gameObject.renderer.material = mat;
			
		}
	}

	void Awake()
	{
		wi = GetComponent<WorldItem>();
		falling = GetComponent<Falling>();
		laser = GetComponentInChildren<LaserArm>();
		trunk = GetComponentInChildren<Trunk>();
	}

	// Use this for initialization
	void Start ()
	{
		GlobalInterface.Singleton.NumRobots += 1;
	}

	void OnDestroy()
	{
		GlobalInterface.Singleton.NumRobots -= 1;
	}

	// Update is called once per frame
	void Update()
	{
		if(!wi.world) {
			return;
		}
		// falling
		if(FallActionIsFalling()) {
			if(FallActionRun()) {
				return;
			}
		}
		// check if we find something to laser nearby :D
		if(laser) {
			if(DesintegrateActionHasValidDestroyable() || DesintegrateActionSelectDestroyable()) {
				DesintegrateActionMoveToDestroyable();
				if(DesintegrateActionIsDestroyableInRange()) {
					if(DesintegrateAction()) {
						return;
					}
				}
			}
			else {
				DesintegrateActionStop();
			}
		}
		// haul stuff
		if(trunk) {
			// check if we are near dropoff, if so unload completely
			if(!trunk.IsEmpty && HaulActionIsDropoffInRange()) {
				HaulActionUnload();
				return;
			}
			// collect
			if(HaulActionHasValidPickable() || ActionHaulSelectPickable()) {
				HaulActionMoveToPickable();
				if(HaulActionIsPickableInRange()) {
					if(HaulActionRun()) {
						return;
					}
				}
			}
			else {
				HaulActionCancel();
				// bring back
				if(HaulActionCheckIsDelivering()) {
					HaulActionMoveToDropoff();
				}
			}
		}
		// move around
		if(!MoveActionGoalReached()) {
			if(MoveActionMoveToGoal()) {
				return;
			}
		}
		ActionSetRandomGoal();
	}

	#region FallAction

	Falling falling;

	bool FallActionIsFalling()
	{
		return falling && falling.IsFalling;
	}

	bool FallActionRun()
	{
		return true;
	}

	#endregion

	#region HaulAction

	Trunk trunk;
	Pickable haulTarget;
	ResourceDropoff haulDropoff;

	bool HaulActionCheckIsDelivering()
	{
		if(!trunk.IsFull) {
			return false;
		}
		if(!wi.world.Building) {
			return false;
		}
		haulDropoff = wi.world.Building.GetComponent<ResourceDropoff>();
		return (haulDropoff != null);
	}

	bool HaulActionMoveToDropoff()
	{
		if(!haulDropoff) {
			return false;
		}
		goal = haulDropoff.DropOffPointWorld;
		return true;
	}

	float GoalTolerance
	{
		get
		{
			return distToGoal + 1.0f/20.0f * speed;
		}
	}

	bool HaulActionIsDropoffInRange()
	{
		if(!wi.world.Building) {
			return false;
		}
		haulDropoff = wi.world.Building.GetComponent<ResourceDropoff>();
		if(!haulDropoff) {
			return false;
		}
		float d = (this.transform.position.xz() - haulDropoff.DropOffPointWorld.xz()).magnitude;
		return d <= GoalTolerance;
	}

	bool HaulActionUnload()
	{
		float delta = trunk.Unload(Time.deltaTime * trunk.loadRate);
		if(trunk.Type == PickableType.GOO) {
			GlobalInterface.Singleton.NumGoo += delta;
		}
		if(trunk.Type == PickableType.MINERALS) {
			GlobalInterface.Singleton.NumMinerals += delta;
		}
		return true;
	}

	bool HaulActionHasValidPickable()
	{
		return haulTarget && !haulTarget.Depleted && trunk.MaxCanLoad(haulTarget) > 0;
	}

	float actionHaulReselectCooldown = 0.0f;

	bool ActionHaulSelectPickable()
	{
		if(trunk.IsFull) {
			return false;
		}
		actionHaulReselectCooldown -= Time.deltaTime;
		if(actionHaulReselectCooldown < 0.0f) {
			// find hightest value
			haulTarget = wi.world
				.FindTopObjects<Pickable>(this.transform.position, searchRadius)
				.Where(x => x != null && !x.Depleted)
				.FindBest(this.transform.position, t => trunk.MaxCanLoad(t));
			actionHaulReselectCooldown = FIND_TOP_OBJS_COOLDOWN;
		}
		else {
			haulTarget = null;
		}
		return haulTarget;
	}

	bool HaulActionIsPickableInRange()
	{
		return (this.transform.position.xz() - haulTarget.transform.position.xz()).magnitude <= maxHaulDist;
	}

	bool HaulActionMoveToPickable()
	{
		if(!haulTarget) {
			return false;
		}
		goal = haulTarget.transform.position;
		return true;
	}

	bool HaulActionRun()
	{
		float delta = Mathf.Min(Time.deltaTime * trunk.loadRate, haulTarget.Amount);
		delta = trunk.Load(haulTarget.type, delta);
		haulTarget.Gather(delta);
		return delta > 0.0f;
	}

	bool HaulActionCancel()
	{
		haulTarget = null;
		return true;
	}

	#endregion

	#region DesintegrateAction

	Destroyable laserTarget;
	LaserArm laser;
	float desintegrateCooldown = 0.0f;

	bool DesintegrateActionHasValidDestroyable()
	{
		return ValidDesintegrateDestroyable(laserTarget);
	}
	
	bool DesintegrateActionIsDestroyableInRange()
	{
		return (this.transform.position.xz() - laserTarget.transform.position.xz()).magnitude <= maxLaserDist;
	}

	bool DesintegrateActionMoveToDestroyable()
	{
		if(!laserTarget) {
			return false;
		}
		goal = laserTarget.transform.position;
		return true;
	}

	public float miningLowThreshold = 0.50f;
	public float miningLowProbIncRate = 0.35f;
	public float miningLowPause = 1.70f;

	float lowMineProb = 0.0f;

	bool ValidDesintegrateDestroyable(Destroyable x)
	{
		if(x == null || x.Dead) {
			return false;
		}
		if(wi.world.WorldGroup.Team != this.team) {
			// conquer the planet!
			Robot r = x.GetComponent<Robot>();
			if(r) {
				return (r.Team != this.Team);
			}
			else {
				return false;
			}
		}
		else {
			// only attack no-team robots
			Robot r = x.GetComponent<Robot>();
			if(r) {
				return (r.Team != this.Team);
			}
			// assume rest is mining
			// neutral team does not mine
			if(this.Team == Team.NEUTRAL) {
				return false;
			}
			return wi.world.AllowMining;
		}
	}

	float desintegrateActionReselectCooldown = 0.0f;

	bool DesintegrateActionSelectDestroyable()
	{
		desintegrateCooldown -= Time.deltaTime;
		desintegrateActionReselectCooldown -= Time.deltaTime;
		if(desintegrateCooldown <= 0) {
			// find hightest value
			laserTarget = wi.world
				.FindTopObjects<Destroyable>(this.transform.position, searchRadius)
				.Where(ValidDesintegrateDestroyable)
				.FindBest(this.transform.position, x => x.dropAmount);
			desintegrateActionReselectCooldown = FIND_TOP_OBJS_COOLDOWN;
		}
		else {
			laserTarget = null;
		}
		if(!laserTarget) {
			return false;
		}
		// shall we really?
		// check value
		if(laserTarget.dropAmount > miningLowThreshold) {
			lowMineProb = 0.0f;
			return true;
		}
		else {
			lowMineProb += Time.deltaTime * miningLowProbIncRate;
			if(desintegrateCooldown > 0) {
				laserTarget = null;
				return false;
			}
			if(Random.value > Mathf.Min(0.95f,lowMineProb)) {
				desintegrateCooldown = miningLowPause;
				laserTarget = null;
				return false;
			}
			return true;
		}
	}

	bool DesintegrateAction()
	{
		laser.LaserEnabled = true;
		laser.Target = laserTarget;
		return true;
	}

	bool DesintegrateActionStop()
	{
		laser.LaserEnabled = false;
		return true;
	}

	#endregion
	
	#region MoveAction

	public void SetRandomGoal()
	{
		ActionSetRandomGoal();
	}

	Vector3 goal;
	float heading;

	void SetRotation(float spin)
	{
		this.transform.rotation = Quaternion.AngleAxis(spin, Vector3.up);
			//* Quaternion.AngleAxis(-90.0f, new Vector3(1,0,0));
	}

	bool MoveActionGoalReached()
	{
		Vector3 dir = goal - this.transform.position;
		dir.y = 0.0f;
		return (dir.magnitude <= GoalTolerance);
	}

	bool MoveActionMoveToGoal()
	{
		Vector3 dir = goal - this.transform.position;
		dir.y = 0.0f;
		// move to goal
		heading = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg;
		Vector3 newpos = this.transform.localPosition + Time.deltaTime * speed * dir.normalized;
		if(falling.TrySetNewLocalPosition(newpos)) {
			//newpos.y = topVoxel.z + 1.5f;
			goal.y = newpos.y;
			SetRotation(heading);
			// still moving
			return true;
		}
		else {
			// can not reach goal
			return false;
		}
	}

	public bool ActionSetRandomGoal()
	{
		// move randomly
		Vector3 dir = new Vector3(MathTools.Random(-1.00f,+1.00f), 0, MathTools.Random(-1.00f,+1.00f));
		goal = this.transform.position + MathTools.Random(2.00f,4.00f)*dir.normalized;
		return true;
	}

	#endregion

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, goal);
	}
}
