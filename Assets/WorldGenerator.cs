using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldGenerator : MonoBehaviour
{
	public GameObject pfMineral;
	public GameObject pfMineralVoxel;
	public GameObject pfPlant;
	public GameObject pfRobotLaser;
	public GameObject pfRobotHauler;

	public int worldRadius = 16;
	public int worldHeight = 6;
	public int numMinerals = 100;
	public int numPlants = 50;
	public int numRobotsLaser = 10;
	public int numRobotsHauler = 5;

	SLPerlinNoise.PerlinNoise3D perlin;

	VoxelEngine.Voxel vAir, vWater, vLand, vBedRock;
		
	public WorldGenerator()
	{
		perlin = new SLPerlinNoise.PerlinNoise3D(0);
		vAir = VoxelEngine.Voxel.Empty;
		vWater = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Soft, new Color(0.35f,0.50f,0.75f));
		vLand = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Normal, new Color(0.98f,0.98f,0.98f));
		vBedRock = new VoxelEngine.Voxel(VoxelEngine.Voxel.Solidness.Ultra, new Color(0.21f,0.22f,0.22f));
	}

	public VoxelEngine.World Create(Int3 min, Int3 max, Vector3 scale, Func<int,int,int,VoxelEngine.Voxel> f)
	{
		perlin.InitNoiseFunctions(0);
		VoxelEngine.World w = new VoxelEngine.World(scale);
		Int3 p = Int3.Zero;
		for(p.z=min.z; p.z<max.z; p.z++) {
			for(p.y=min.y; p.y<max.y; p.y++) {
				for(p.x=min.x; p.x<max.x; p.x++) {
					w.Set(p, f(p.x,p.y,p.z));
				}
			}
		}
		return w;
	}

	const int WATER_HEIGHT = 0; // 4
	const float XY_SCALE = 2.0f;

	VoxelEngine.Voxel FMiniMinecraft(int x, int y, int z)
	{
		float q = Mathf.Max(0, 5*(1+perlin.Compute(XY_SCALE*x,XY_SCALE*y,0)));
		if(z > q) {
			if(z < WATER_HEIGHT) {
				return vWater;
			}
			else {
				return vAir;
			}
		}
		else {
			return vLand;
		}
	}

	public VoxelEngine.World CreateMiniMinecraft()
	{
		return Create(
			new Int3(0,0,0), new Int3(32,32,8),
			Vector3.one, FMiniMinecraft);
	}

	VoxelEngine.Voxel FDiscworld(int x, int y, int z, int radius)
	{
		float r = Mathf.Sqrt(x*x + y*y);
		if(r > radius) {
			return vAir;
		}
		return FMiniMinecraft(x,y,z);
	}

	public VoxelEngine.World CreateDiscworld(int radius, int height)
	{
		Vector3 scale = Vector3.one;// new Vector3(4,4,4);
		// pass 1: solid
		VoxelEngine.World vw = Create(
			new Int3(-radius,-radius,0), new Int3(radius,radius,height),
			scale,
			(x,y,z) => FDiscworld(x,y,z,radius));
		// pass 2: bottom is bedrock
		foreach(Int3 i in vw.GetBottomVoxels()) {
			vw.Set(i, vBedRock);
		}
		return vw;
	}

	public void Create(World world, Team initRobotTeam)
	{
		// first pass voxels
		VoxelEngine.World voxels = CreateDiscworld(worldRadius, worldHeight);
		world.Voxels = voxels;
		// second pass minerals
		// plant 100 minerals in random solid voxels
		foreach(Int3 p in voxels.GetSolidVoxels().RandomSample(numMinerals)) {
			GameObject go = (GameObject)Instantiate(pfMineral);
			go.transform.parent = world.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
			world.Add(go.GetComponent<Entity>());
		}
		// plant small minerals in every top voxel
		foreach(Int3 p in voxels.GetTopVoxels()) {
			GameObject go = (GameObject)Instantiate(pfMineralVoxel);
			go.transform.parent = world.transform;
			go.transform.localPosition = p.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
			world.Add(go.GetComponent<Entity>());
		}
		// third pass plants
		// plant 40 plants on top of random solid top voxels
		foreach(Int3 p in voxels.GetTopVoxels().RandomSample(numPlants)) {
			GameObject go = (GameObject)Instantiate(pfPlant);
			go.transform.parent = world.transform;
			world.Add(go.GetComponent<Entity>());
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
		}
		// robot laser
		foreach(Int3 p in voxels.GetTopVoxels().RandomSample(numRobotsLaser)) {
			GameObject go = (GameObject)Instantiate(pfRobotLaser);
			go.transform.parent = world.transform;
			Entity wi = go.GetComponent<Entity>();
			wi.MoveToWorld(world);
			wi.Team = initRobotTeam;
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Robot rob =	go.GetComponent<Robot>();
			rob.SetRandomGoal();
		}
		// robot hauler
		foreach(Int3 p in voxels.GetTopVoxels().RandomSample(numRobotsHauler)) {
			GameObject go = (GameObject)Instantiate(pfRobotHauler);
			go.transform.parent = world.transform;
			Entity wi = go.GetComponent<Entity>();
			wi.MoveToWorld(world);
			wi.Team = initRobotTeam;
			go.GetComponent<Falling>().SetNewLocalPosition(p.ToVector3() + new Vector3(0.5f,1,0.5f));
			Robot rob =	go.GetComponent<Robot>();
			rob.SetRandomGoal();
		}
	}

}
