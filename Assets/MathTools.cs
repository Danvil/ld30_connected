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
}
