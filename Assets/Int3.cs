using UnityEngine;

public class Int3
{
	public int x, y, z;

	public Int3()
	{ x = y = z = 0; }

	public Int3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Int3 operator+(Int3 a, Int3 b) 
	{ return new Int3(a.x + b.x, a.y + b.y, a.z + b.z); }

	public static Int3 operator-(Int3 a, Int3 b) 
	{ return new Int3(a.x - b.x, a.y - b.y, a.z - b.z); }

	public static Vector3 operator+(Vector3 a, Int3 b) 
	{ return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
	
	public static Vector3 operator-(Vector3 a, Int3 b) 
	{ return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
	
};
