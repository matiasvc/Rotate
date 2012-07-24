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
	    return new Vector2((float)Mathf.Sin(angle), (float)Mathf.Cos(angle));
	}
	
	 public static float VectorToAngle(Vector2 vector)
	{
	    return ((float)Mathf.Atan2(vector.y, vector.x)) * Mathf.Rad2Deg;
	}
	
	public static Vector2 Vector2Lerp (Vector2 v1, Vector2 v2, float value)
	{
        if (value > 1.0f)
            return v2;
        else if (value < 0.0f)
            return v1;
        return new Vector2 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value );      
    }
	
	public static Vector2 Vector2LerpUnclamped (Vector2 v1, Vector2 v2, float value)
	{
        return new Vector2 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value );      
    }
   
    public static Vector3 Vector3Lerp (Vector3 v1, Vector3 v2, float value)
	{
        if (value > 1.0f)
            return v2;
        else if (value < 0.0f)
            return v1;
        return new Vector3 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value,
                            v1.z + (v2.z - v1.z)*value );
    }
	
	public static Vector3 Vector3LerpUnclamped (Vector3 v1, Vector3 v2, float value)
	{
        return new Vector3 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value,
                            v1.z + (v2.z - v1.z)*value );
    }
   
    public static Vector4 Vector4Lerp (Vector4 v1, Vector4 v2, float value)
	{
        if (value > 1.0f)
            return v2;
        else if (value < 0.0f)
            return v1;
        return new Vector4 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value,
                            v1.z + (v2.z - v1.z)*value,
                            v1.w + (v2.w - v1.w)*value );
    }
	
	public static Vector4 Vector4LerpUnclamped (Vector4 v1, Vector4 v2, float value)
	{
        return new Vector4 (v1.x + (v2.x - v1.x)*value,
                            v1.y + (v2.y - v1.y)*value,
                            v1.z + (v2.z - v1.z)*value,
                            v1.w + (v2.w - v1.w)*value );
    }
}
