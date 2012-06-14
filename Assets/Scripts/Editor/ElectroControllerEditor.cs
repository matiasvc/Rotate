using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ElectroController))]
public class ElectroControllerEditor : Editor {
	
	ElectroController editorTarget = null;
	
	void OnEnable ()
	{
		editorTarget = target as ElectroController;
	}
	
	void OnDisable ()
	{
		
	}
	
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
	}
	
}
