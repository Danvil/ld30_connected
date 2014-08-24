using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldVoxelGenerator
{
	SLPerlinNoise.PerlinNoise3D perlin;

	Voxels.Voxel vAir, vWater, vLand, vBedRock;
		
	public WorldVoxelGenerator()
	{
		perlin = new SLPerlinNoise.PerlinNoise3D(0);
		vAir = Voxels.Voxel.Empty;
		vWater = new Voxels.Voxel(Voxels.Voxel.Solidness.Soft, new Color(0.35f,0.50f,0.75f));
		vLand = new Voxels.Voxel(Voxels.Voxel.Solidness.Normal, new Color(0.98f,0.98f,0.98f));
		vBedRock = new Voxels.Voxel(Voxels.Voxel.Solidness.Ultra, new Color(0.21f,0.22f,0.22f));
	}

	public Voxels.World Create(int sx, int sy, int sz, Vector3 scale, Func<int,int,int,Voxels.Voxel> f)
	{
		perlin.InitNoiseFunctions(0);
		Voxels.World w = new Voxels.World(scale);
		Int3 p = Int3.Zero;
		for(p.z=0; p.z<sz; p.z++) {
			for(p.y=0; p.y<sy; p.y++) {
				for(p.x=0; p.x<sx; p.x++) {
					w.Set(p, f(p.x,p.y,p.z));
				}
			}
		}
		return w;
	}

	const int WATER_HEIGHT = 0; // 4
	const float XY_SCALE = 2.0f;

	Voxels.Voxel FMiniMinecraft(int x, int y, int z)
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

	public Voxels.World CreateMiniMinecraft()
	{
		return Create(32,32,8,Vector3.one,FMiniMinecraft);
	}

	const int DISK_RAD = 12;

	Voxels.Voxel FDiscworld(int x, int y, int z)
	{
		int dx = x - DISK_RAD;
		int dy = y - DISK_RAD;
		float r = Mathf.Sqrt(dx*dx + dy*dy);
		if(r > DISK_RAD) {
			return vAir;
		}
		return FMiniMinecraft(x,y,z);
	}

	public Voxels.World CreateDiscworld()
	{
		Vector3 scale = Vector3.one;// new Vector3(4,4,4);
		// pass 1: solid
		Voxels.World vw = Create(2*DISK_RAD,2*DISK_RAD,4,scale,FDiscworld);
		// pass 2: bottom is bedrock
		foreach(Int3 i in vw.GetBottomVoxels()) {
			vw.Set(i, vBedRock);
		}
		return vw;
	}

}
