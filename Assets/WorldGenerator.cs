using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator
{
	SLPerlinNoise.PerlinNoise3D perlin;

	Voxels.Voxel vAir, vWater, vLand;
		
	public WorldGenerator()
	{
		perlin = new SLPerlinNoise.PerlinNoise3D();
		vAir = Voxels.Voxel.Empty;
		vWater = new Voxels.Voxel(true, new Color(0.50f,0.55f,0.95f));
		vLand = new Voxels.Voxel(true, new Color(0.98f,0.98f,0.98f));
	}

	Voxels.Voxel Evaluate(int x, int y, int z)
	{
		float q = Mathf.Max(0, 5*(1+perlin.Compute(x,y,0)));
		if(z > q) {
			if(z < 4) {
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

	public IEnumerable<Mesh> Create()
	{
		Voxels.World w = new Voxels.World();
		const int SIZE_X = 32;
		const int SIZE_Y = 32;
		const int SIZE_Z = 8;
		for(int z=0; z<SIZE_Z; z++) {
			for(int y=0; y<SIZE_Y; y++) {
				for(int x=0; x<SIZE_X; x++) {
					w.Set(new Int3(x,y,z), Evaluate(x,y,z));
				}
			}
		}
		// md.AddCube(new Int3(0,0,0));
		// md.AddCube(new Int3(1,0,0));
		// md.AddCube(new Int3(2,0,0));
		// md.AddCube(new Int3(2,0,1), new Color(1.00f,0.50f,0.00f));
		return w.CreateMesh();
	}

}
