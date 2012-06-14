using UnityEngine;
using System.Collections;

public static class VectorEx {
	
	public struct Vector2I
	{
		public int x;
		public int y;
		
		public Vector2I(int __x, int __y)
		{
			x = __x;
			y = __y;
		}
		
		public override string ToString()
		{
			return x + "," + y;
		}
	}
	
	public static Vector2 RotateVector2(Vector2 vector ,float angle)
	{
		angle = angle * Mathf.Deg2Rad;
		
		float cs = Mathf.Cos(angle);
		float sn = Mathf.Sin(angle);
		
		float xt = (vector.x * cs - vector.y * sn);
		float yt = (vector.x * sn + vector.y * cs);
		
		return new Vector2(xt,yt);
	}
	
	public static Vector2 AngleToVector(float angle)
	{
		angle = angle * Mathf.Deg2Rad;
	    return new Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle));
	}
	
	 public static float VectorToAngle(Vector2 vector)
	{
	    return ((float)Mathf.Atan2(vector.y, vector.x)) * Mathf.Rad2Deg;
	}
}
