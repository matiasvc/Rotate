using UnityEngine;
using System.Collections;

public static class GizmoEx {
	
	private const int CURCLE_SUBDIVISIONS = 64;
	
	public static void DrawCross(Vector3 pos, float size, Color color)
    {
        Vector3 upper 	= new Vector3(pos.x,					pos.y + (size * 0.5f),	pos.z);
        Vector3 lower 	= new Vector3(pos.x,					pos.y - (size * 0.5f),	pos.z);
        Vector3 left 	= new Vector3(pos.x - (size * 0.5f),	pos.y,					pos.z);
        Vector3 rigth 	= new Vector3(pos.x + (size * 0.5f),	pos.y,					pos.z);
		Vector3 front	= new Vector3(pos.x,					pos.y,					pos.z + (size * 0.5f));
		Vector3 back	= new Vector3(pos.x,					pos.y,					pos.z - (size * 0.5f));
        
		Color oldColor = Gizmos.color;
        Gizmos.color = color;
		
        Gizmos.DrawLine(upper, lower);
        Gizmos.DrawLine(left, rigth);
		Gizmos.DrawLine(front, back);
		
		Gizmos.color = oldColor;
    }
	
	public static void DrawRect(Vector3 position, Vector3 direction, Rect rect, Color color)
	{
		DrawRect(position, Quaternion.LookRotation(direction), rect, color);
	}
	
	public static void DrawRect(Vector3 position, Quaternion direction, Rect rect, Color color)
    {
       
        Vector3 bottomLeft 		= new Vector3(rect.x,				rect.y - rect.height,	0f);
        Vector3 topLeft 		= new Vector3(rect.x,				rect.y,					0f);
        Vector3 bottomRigth 	= new Vector3(rect.x - rect.width,	rect.y - rect.height,	0f);
        Vector3 topRigth 		= new Vector3(rect.x - rect.width,	rect.y,					0f);
		
		Matrix4x4 matrix = Matrix4x4.TRS(position, direction, Vector3.one);
		
		bottomLeft 		= matrix.MultiplyPoint(bottomLeft);
		topLeft 		= matrix.MultiplyPoint(topLeft);
		bottomRigth 	= matrix.MultiplyPoint(bottomRigth);
		topRigth 		= matrix.MultiplyPoint(topRigth);
		
		Color oldColor = Gizmos.color;
        Gizmos.color = color;
       
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRigth, topRigth);
        Gizmos.DrawLine(topRigth, topLeft);
        Gizmos.DrawLine(bottomRigth, bottomLeft);
		
		Gizmos.color = oldColor;
    }
	
	public static void DrawCircle(Vector3 position, Vector3 direction, float size, Color color)
	{
		DrawCircle(position, Quaternion.LookRotation(direction), size, color);
	}
	
	public static void DrawCircle(Vector3 position, Quaternion direction, float size, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		Matrix4x4 matrix = Matrix4x4.TRS(position, direction, Vector3.one);
		Vector3 lastPos = Vector3.zero;
		float slice = 2f * Mathf.PI / CURCLE_SUBDIVISIONS;
		
		for (int i = 0; i <= CURCLE_SUBDIVISIONS; i++)
		{
			float angle = slice * i;
			Vector3 newPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * size;
			newPos = matrix.MultiplyPoint(newPos);
			
			if (lastPos != Vector3.zero)
				Gizmos.DrawLine(lastPos, newPos);
			
			lastPos = newPos;
		}
		
		Gizmos.color = oldColor;
	}
	
}
