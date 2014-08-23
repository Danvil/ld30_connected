using UnityEngine;

public struct Int3
{
	public int x, y, z;

	public Int3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Int3 X = new Int3(1,0,0);
	public static Int3 Y = new Int3(0,1,0);
	public static Int3 Z = new Int3(0,0,1);

	public static Int3 operator-(Int3 a) 
	{ return new Int3(-a.x, -a.y, -a.z); }

	public static Int3 operator*(int s, Int3 a) 
	{ return new Int3(s*a.x, s*a.y, s*a.z); }

	public static Int3 operator*(Int3 a, int s) 
	{ return new Int3(s*a.x, s*a.y, s*a.z); }

	public static Int3 operator+(Int3 a, Int3 b) 
	{ return new Int3(a.x + b.x, a.y + b.y, a.z + b.z); }

	public static Int3 operator-(Int3 a, Int3 b) 
	{ return new Int3(a.x - b.x, a.y - b.y, a.z - b.z); }

	public static Vector3 operator+(Vector3 a, Int3 b) 
	{ return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
	
	public static Vector3 operator-(Vector3 a, Int3 b) 
	{ return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
	
	public static Vector3 operator+(Int3 a, Vector3 b) 
	{ return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
	
	public static Vector3 operator-(Int3 a, Vector3 b) 
	{ return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
	
};
