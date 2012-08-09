using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PlaneEditor : ScriptableWizard {
	
	private List<Vector3> splinePoints;
	
	[MenuItem ("Tools/Plane Wizard")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Plane Wizard", typeof(PlaneEditor), "Generate Plane");
	}
	
	void OnEnable()
	{
		splinePoints = new List<Vector3>();
	}
	
	void Update()
	{
		
	}
	
	void OnInspectorUpdate()
	{
		
	}
	
	void OnSceneGUI()
	{
		Debug.Log(Event.current.mousePosition);
	}
	
	void OnWizardUpdate()
	{
		
	}
	
	void OnWizardCreate()
	{
		
	}
	
}
