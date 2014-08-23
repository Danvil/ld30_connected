using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Voxels {

public struct Voxel
{
	public bool solid;
	public Color color;

	public static Voxel Empty = new Voxel(false,Color.white);

	public Voxel(bool solid, Color color)
	{
		this.solid = solid;
		this.color = color;
	}
};

public class Chunk
{
	public const int S = 8;
	
	World world;

	Int3 pos;

	List<Voxel> voxels;

	public Chunk(World world, Int3 pos)
	{
		this.world = world;
		this.pos = S*pos;
		this.voxels = Enumerable.Repeat(Voxel.Empty, S*S*S).ToList();
	}
	
	int Index(Int3 p)
	{
		return p.x + S*(p.y + S*p.z);
	}

	public void Set(Int3 l, Voxel b)
	{
		voxels[Index(l)] = b;
	}

	public Voxel Get(Int3 l)
	{
		return voxels[Index(l)];
	}

	public bool IsSolid(Int3 l)
	{
		return voxels[Index(l)].solid;
	}

	public Mesh CreateMesh()
	{
		MeshData md = new MeshData();
		int i = 0;
		Int3 l = new Int3(0,0,0);
		for(l.z=0; l.z<S; l.z++) {
			for(l.y=0; l.y<S; l.y++) {
				for(l.x=0; l.x<S; l.x++,i++) {
					Voxel b = voxels[i];
					if(b.solid) {
						Int3 w = pos + l;
						if(!(  world.IsSolid(w + Int3.X)
							&& world.IsSolid(w - Int3.X)
							&& world.IsSolid(w + Int3.Y)
							&& world.IsSolid(w - Int3.Y)
							&& world.IsSolid(w + Int3.Z)
							&& world.IsSolid(w - Int3.Z)
						)) {
							md.AddCube(w, b.color);
						}
					}
				}
			}
		}
		return md.CreateMesh();
	}
};

public class World
{
	Dictionary<Int3,Chunk> chunks = new Dictionary<Int3,Chunk>();

	void Split(int w, out int c, out int l)
	{
		c = w / Chunk.S;
		l = w - Chunk.S*c;
		if(w < 0) {
			c -= 1;
			l += Chunk.S;
		}
		//Debug.Log(string.Format("w={0} c={1} l={2}", w, c, l));
	}

	void Split(Int3 w, out Int3 c, out Int3 l)
	{
		Split(w.x, out c.x, out l.x);
		Split(w.y, out c.y, out l.y);
		Split(w.z, out c.z, out l.z);
	}

	public void Set(Int3 p, Voxel b)
	{
		Int3 c = new Int3();
		Int3 l = new Int3();
		Split(p, out c, out l);
		if(!chunks.ContainsKey(c)) {
			chunks[c] = new Chunk(this, c);
		}
		chunks[c].Set(l,b);
	}

	public Voxel Get(Int3 p)
	{
		Int3 c = new Int3();
		Int3 l = new Int3();
		Split(p, out c, out l);
		if(!chunks.ContainsKey(c)) {
			return Voxel.Empty;
		}
		else {
			return chunks[c].Get(l);
		}
	}

	public bool IsSolid(Int3 p)
	{
		Int3 c = new Int3();
		Int3 l = new Int3();
		Split(p, out c, out l);
		if(!chunks.ContainsKey(c)) {
			return false;
		}
		else {
			return chunks[c].Get(l).solid;
		}
	}

	public IEnumerable<Mesh> CreateMesh()
	{
		return chunks.Values.Select(c => c.CreateMesh());
	}
}

}
