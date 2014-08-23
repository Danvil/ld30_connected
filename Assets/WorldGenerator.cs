using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator
{
	Voxels.Voxel Evaluate(int x, int y, int z)
	{
		Voxels.Voxel b = new Voxels.Voxel();
		b.solid = true;
		b.color = Color.white;
		return b;
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
