using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LineShapeController : MonoBehaviour
{
	private Plane levelPlane;
	private List<Vector3> pointList = new List<Vector3>();
	
	void Awake()
	{
		levelPlane = new Plane(Vector3.up, Vector3.zero);
	}
	
	[ExecuteInEditMode]
	void OnGUI()
	{
		Event e = Event.current;
		
		if (e.button == 0)
		{
			AddPoint();
		}
		
		Debug.Log("click");
	}
	
	private void AddPoint()
	{
		Debug.Log("click");
	}
}
