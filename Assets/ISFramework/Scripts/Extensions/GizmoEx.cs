using UnityEngine;
using System.Collections;

public static class GizmoEx
{
	
	private const int CIRCLE_SUBDIVISIONS = 64;
	
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
		
		Matrix4x4 matrix = Matrix4x4.TRS(position, direction, Vector3.one * size);
		Vector3 lastPos = Vector3.zero;
		float slice = 2f * Mathf.PI / CIRCLE_SUBDIVISIONS;
		
		for (int i = 0; i <= CIRCLE_SUBDIVISIONS; i++)
		{
			float angle = slice * i;
			Vector3 newPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
			newPos = matrix.MultiplyPoint(newPos);
			
			if (lastPos != Vector3.zero)
				Gizmos.DrawLine(lastPos, newPos);
			
			lastPos = newPos;
		}
		
		Gizmos.color = oldColor;
	}
	
	public static void DrawNormal(Vector3 position, Vector3 direction, float size, Color color)
	{
		DrawNormal(position, Quaternion.LookRotation(direction), size, color);
	}
	
	public static void DrawNormal(Vector3 position, Quaternion direction, float size, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		Matrix4x4 matrix = Matrix4x4.TRS(position, direction, Vector3.one * size);
		
		Vector3 baseLeft =			new Vector3(-0.5f,		0f,			0f);
		Vector3 baseRigth = 		new Vector3(0.5f,		0f,			0f);
		Vector3 baseTop = 			new Vector3(0f,			0.5f, 		0f);
		Vector3 baseBottom = 		new Vector3(0f,			-0.5f,		0f);
		
		Vector3 arrowBottom =		new Vector3(0f,			0f,			0f);
		Vector3 arrowTop =			new Vector3(0f,			0f,			1f);
		
		Vector3 arrowPointLeft =	new Vector3(-0.25f,		0f,			0.75f);
		Vector3 arrowPointRigth =	new Vector3(0.25f,		0f,			0.75f);
		Vector3 arrowPointTop =		new Vector3(0f,			0.25f,		0.75f);
		Vector3 arrowPointBottom = 	new Vector3(0f,			-0.25f,		0.75f);
		
		baseLeft = matrix.MultiplyPoint(baseLeft);
		baseRigth = matrix.MultiplyPoint(baseRigth);
		baseTop = matrix.MultiplyPoint(baseTop);
		baseBottom = matrix.MultiplyPoint(baseBottom);
		
		arrowBottom = matrix.MultiplyPoint(arrowBottom);
		arrowTop = matrix.MultiplyPoint(arrowTop);
		
		arrowPointLeft = matrix.MultiplyPoint(arrowPointLeft);
		arrowPointRigth = matrix.MultiplyPoint(arrowPointRigth);
		arrowPointTop = matrix.MultiplyPoint(arrowPointTop);
		arrowPointBottom = matrix.MultiplyPoint(arrowPointBottom);
		
		Gizmos.DrawLine(baseLeft, baseRigth);
		Gizmos.DrawLine(baseTop, baseBottom);
		Gizmos.DrawLine(arrowBottom, arrowTop);
		Gizmos.DrawLine(arrowPointLeft, arrowTop);
		Gizmos.DrawLine(arrowPointRigth, arrowTop);
		Gizmos.DrawLine(arrowPointTop, arrowTop);
		Gizmos.DrawLine(arrowPointBottom, arrowTop);
		
		Gizmos.color = oldColor;
	}
	
	public static void DrawCameraLineHorizontal(float pos, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		Vector3 p0 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0f, pos, 1f));
		Vector3 p1 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1f, pos, 1f));
		
		Gizmos.DrawLine(p0, p1);
		
		Gizmos.color = oldColor;
	}
	
	public static void DrawCameraLineVertical(float pos, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		Vector3 p0 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(pos, 0f, 1f));
		Vector3 p1 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(pos, 1f, 1f));
		
		Gizmos.DrawLine(p0, p1);
		
		Gizmos.color = oldColor;
	}
	
	public static void DrawCameraSafeArea(float safeSize, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		safeSize = safeSize * 0.5f;
		
		Vector3 p0 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(safeSize, safeSize, 1f));
		Vector3 p1 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1f-safeSize, safeSize, 1f));
		Vector3 p2 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1f-safeSize, 1f-safeSize, 1f));
		Vector3 p3 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(safeSize, 1f-safeSize, 1f));
		
		Gizmos.DrawLine(p0, p1);
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p0);
		
		Gizmos.color = oldColor;
	}
	
	public static void DrawCameraSafeArea(Vector2 safeSize, Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;
		
		safeSize = safeSize * 0.5f;
		
		Vector3 p0 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(safeSize.x, safeSize.y, 1f));
		Vector3 p1 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1f-safeSize.x, safeSize.y, 1f));
		Vector3 p2 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1f-safeSize.x, 1f-safeSize.y, 1f));
		Vector3 p3 = Camera.mainCamera.ViewportToWorldPoint(new Vector3(safeSize.x, 1f-safeSize.y, 1f));
		
		Gizmos.DrawLine(p0, p1);
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p0);
		
		Gizmos.color = oldColor;
	}
}
