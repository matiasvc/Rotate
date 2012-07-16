using UnityEngine;
using System.Collections;

public static class GizmoEx {
	
	public static void DrawGizmoCross(Vector3 markerPosition, float markSize, Color color)
    {
        Vector3 upper = new Vector3(markerPosition.x, markerPosition.y + (markSize * 0.5f), markerPosition.z);
        Vector3 lower = new Vector3(markerPosition.x, markerPosition.y - (markSize * 0.5f), markerPosition.z);
        Vector3 left = new Vector3(markerPosition.x - (markSize * 0.5f), markerPosition.y, markerPosition.z);
        Vector3 rigth = new Vector3(markerPosition.x + (markSize * 0.5f), markerPosition.y, markerPosition.z);
        
        Gizmos.color = color;
        Gizmos.DrawLine(upper, lower);
        Gizmos.DrawLine(left, rigth);
    }
	
	public static void DrawGizmoRect(Vector3 center, Vector2 rectArea, Color color)
    {
       
        Vector3 lowerLeft = new Vector3(center.x - (rectArea.x * 0.5f), center.y - (rectArea.y * 0.5f), center.z);
        Vector3 upperLeft = new Vector3(center.x - (rectArea.x * 0.5f), center.y + (rectArea.y * 0.5f), center.z);
        Vector3 lowerRigth = new Vector3(center.x + (rectArea.x * 0.5f), center.y - (rectArea.y * 0.5f), center.z);
        Vector3 upperRigth = new Vector3(center.x + (rectArea.x * 0.5f), center.y + (rectArea.y * 0.5f), center.z);
       
        Gizmos.color = color;
       
        Gizmos.DrawLine(lowerLeft, upperLeft);
        Gizmos.DrawLine(lowerRigth, upperRigth);
        Gizmos.DrawLine(upperRigth, upperLeft);
        Gizmos.DrawLine(lowerRigth, lowerLeft);
    }
	
}
