using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelEngine
{

	public class Voxel
	{
		public enum Solidness { Air, Soft, Normal, Ultra };

		public Solidness solid;
		public Color color;

		public bool IsAir { get { return solid == Solidness.Air; } }
		public bool IsSolid { get { return !IsAir; } }

		public static Voxel Empty = new Voxel(Solidness.Air, Color.white);

		public Voxel(Solidness solid, Color color)
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

		HashSet<Int3> solidVoxels = new HashSet<Int3>();

		public bool Dirty { get; set; }

		public Chunk(World world, Int3 pos)
		{
			this.world = world;
			this.pos = S*pos;
			this.voxels = Enumerable.Repeat(Voxel.Empty, S*S*S).ToList();
			solidVoxels.Clear();
		}
		
		int ToIndex(Int3 p)
		{
			return p.x + S*(p.y + S*p.z);
		}

		Int3 FromIndex(int i)
		{
			int z = i / (S*S);
			int y = (i - S*z) / S;
			int x = i - S*(y + S*z);
			return new Int3(x,y,z);
		}

		public void Set(Int3 l, Voxel b)
		{
			int idx = ToIndex(l);
			voxels[idx] = b;
			if(b.IsSolid) {
				solidVoxels.Add(l);
			}
			else {
				solidVoxels.Remove(l);
			}
			Dirty = true;
		}

		public Voxel Get(Int3 l)
		{
			return voxels[ToIndex(l)];
		}

		public bool IsAir(Int3 l)
		{
			return voxels[ToIndex(l)].IsAir;
		}

		public bool IsSolid(Int3 l)
		{
			return !IsAir(l);
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
						if(b.IsSolid) {
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

		public IEnumerable<Int3> GetSolidVoxels()
		{
			return solidVoxels.AsEnumerable();
		}

	};

	public class World
	{
		Dictionary<Int3,Chunk> chunks = new Dictionary<Int3,Chunk>();

		Vector3 scale = Vector3.one;

		Int3 min = Int3.Zero;
		Int3 max = Int3.Zero;

		public World(Vector3 scale)
		{
			this.scale = scale;
		}

		void Split(int w, out int c, out int l)
		{
			if(w >= 0) {
				c = w/Chunk.S;
			}
			else {
				c = (w + 1)/Chunk.S - 1;
			}
			l = w - Chunk.S*c;
			//Debug.Log(string.Format("w={0} c={1} l={2}", w, c, l));
		}

		void Split(Int3 w, out Int3 c, out Int3 l)
		{
			Split(w.x, out c.x, out l.x);
			Split(w.y, out c.y, out l.y);
			Split(w.z, out c.z, out l.z);
		}

		public int Combine(int c, int l)
		{
			return Chunk.S*c + l;
		}

		public Int3 Combine(Int3 c, Int3 l)
		{
			return new Int3(
				Combine(c.x, l.x),
				Combine(c.y, l.y),
				Combine(c.z, l.z)
			);
		}

		public bool Dirty
		{
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
			min = min.Min(p);
			max = max.Max(p);
			Int3 c = new Int3();
			Int3 l = new Int3();
			Split(p, out c, out l);
			if(!chunks.ContainsKey(c)) {
				chunks[c] = new Chunk(this, c);
			}
			chunks[c].Set(l,b);
			// mark neighbours as dirty
			Chunk chunk = null;
			if(l.x == Chunk.S-1 && chunks.TryGetValue(c+Int3.X, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.x == 0 && chunks.TryGetValue(c-Int3.X, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.y == Chunk.S-1 && chunks.TryGetValue(c+Int3.Y, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.y == 0 && chunks.TryGetValue(c-Int3.Y, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.z == Chunk.S-1 && chunks.TryGetValue(c+Int3.Z, out chunk)) {
				chunk.Dirty = true;
			}
			if(l.z == 0 && chunks.TryGetValue(c-Int3.Z, out chunk)) {
				chunk.Dirty = true;
			}
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
				return chunks[c].Get(l).IsSolid;
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
			Debug.Log(string.Format("Voxels: Recreated {0}/{1} chunks", result.Count, chunks.Count));
			return result;
		}

		public Dictionary<Int3,Mesh> CreateAll()
		{
			Dirty = true;
			return RecreateDirty();
		}

		public IEnumerable<Int3> GetSolidVoxels()
		{
			return chunks.SelectMany(x => x.Value.GetSolidVoxels().Select(l => Combine(x.Key, l)));
		}

		public IEnumerable<Int3> GetBottomVoxels()
		{
			Dictionary<Int2,Int3> bottom = new Dictionary<Int2,Int3>();
			foreach(Int3 p in GetSolidVoxels()) {
				Int2 key = new Int2(p.x,p.y);
				Int3 current;
				if(bottom.TryGetValue(key, out current)) {
					if(current.z > p.z) {
						current.z = p.z;
					}
				}
				else {
					current = p;
				}
				bottom[key] = current;
			}
			return bottom.Values.AsEnumerable();
		}

		public IEnumerable<Int3> GetTopVoxels()
		{
			Dictionary<Int2,Int3> topVoxels = new Dictionary<Int2,Int3>();
			foreach(Int3 p in GetSolidVoxels()) {
				Int2 key = new Int2(p.x,p.y);
				Int3 current;
				if(topVoxels.TryGetValue(key, out current)) {
					if(current.z < p.z) {
						current.z = p.z;
					}
				}
				else {
					current = p;
				}
				topVoxels[key] = current;
			}
			return topVoxels.Values.AsEnumerable();
		}

		public bool TryGetTopVoxel(Int2 p2, out Int3 top)
		{
			Int3 p = new Int3(p2.x,p2.y,0);
			for(p.z=max.z; p.z>=min.z; p.z--) {
				if(IsSolid(p)) {
					top = p;
					return true;
				}
			}
			top = new Int3(p2.x, p2.y, int.MinValue);
			return false;
		}

		public bool HasTopVoxel(Int3 p)
		{
			Int3 top;
			return TryGetTopVoxel(p, out top);
		}

		public bool TryGetTopVoxel(Int3 p, out Int3 top)
		{
			return TryGetTopVoxel(p.xy, out top);
		}

		public bool IsTopVoxel(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return top == p;
		}

		public bool IsTopVoxelOrHigher(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return p.z >= top.z;
		}

		public int GetTopVoxelHeight(Int3 p)
		{
			Int3 top;
			TryGetTopVoxel(p.xy, out top);
			return top.z;
		}

	}
}
