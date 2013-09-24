using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CheckpointController))]
public class CheckpointControllerInspector : Editor {
	
	private CheckpointController editorTarget = null;
	
	void OnEnable () {
		editorTarget = target as CheckpointController;
	}
	
	public override void OnInspectorGUI () {
		base.OnInspectorGUI();
	}
	
}
