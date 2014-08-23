using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Voxels {

	public class Voxel
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
		
		public bool Dirty { get; set; }

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
			Dirty = true;
		}

		public Voxel Get(Int3 l)
		{
			return voxels[Index(l)];
		}

		public bool IsSolid(Int3 l)
		{
			return voxels[Index(l)].solid;
		}

		public Mesh CreateMesh(Vector3 scale)
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
			return md.CreateMesh(scale);
		}

		public Mesh CreateMesh()
		{
			return CreateMesh(Vector3.one);
		}

	};

	public class World
	{
		Dictionary<Int3,Chunk> chunks = new Dictionary<Int3,Chunk>();

		Vector3 scale = Vector3.one;

		public World(Vector3 scale)
		{
			this.scale = scale;
		}

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

		public bool Dirty {
			get {
				return chunks.Values.Any(c => c.Dirty);
			}
			set {
				foreach(var c in chunks.Values) {
					c.Dirty = value;
				}
			}
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

		public Dictionary<Int3,Mesh> RecreateDirty()
		{
			Dictionary<Int3,Mesh> result = new Dictionary<Int3,Mesh>();
			if(!Dirty) {
				return result;
			}
			foreach(var p in chunks) {
				if(p.Value.Dirty) {
					result[p.Key] = p.Value.CreateMesh(scale);
					p.Value.Dirty = false;
				}
			}
			Debug.Log(string.Format("Recreated {0} chunks", chunks.Count));
			return result;
		}

		public Dictionary<Int3,Mesh> CreateAll()
		{
			Dirty = true;
			return RecreateDirty();
		}

	}
}
