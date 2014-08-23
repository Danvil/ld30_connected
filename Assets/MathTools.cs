using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MathTools
{
	public static Vector3 Limit(this Vector3 v, float maxlen)
	{
		if(maxlen == 0.0f) {
			return Vector3.zero;
		}
		float l = v.magnitude;
		if(l > maxlen) {
			return v * (maxlen/l);
		}
		else {
			return v;
		}
	}

	public static Vector3 CoeffMult(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
	}
}
