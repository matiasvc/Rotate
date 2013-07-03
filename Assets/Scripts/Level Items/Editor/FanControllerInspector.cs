using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FanController))]
public class FanControllerInspector : Editor {
	
	private FanController editorTarget = null;
	
	void OnEnable () {
		editorTarget = target as FanController;
	}
	
	public override void OnInspectorGUI () {
		bool newEnabled = EditorGUILayout.Toggle("Item Enabled", editorTarget.itemEnabled);
		float newLength = (float)EditorGUILayout.IntSlider("Airflow Length", Mathf.RoundToInt(editorTarget.airflowLength), 1, 8 );
		float newForce = EditorGUILayout.Slider("Force", editorTarget.fanForce, 5f, 40f);
		
		if ( GUI.changed ) {
			// Resize trigger collider
			editorTarget.fanTrigger.center = new Vector3(1.5f, 0f, (newLength * 0.5f) + 1f);
			editorTarget.fanTrigger.size = new Vector3(1.5f, 1f, newLength);
			
			// Adjust particle lifetime
			editorTarget.airflowParticles.startLifetime = newLength / editorTarget.airflowParticles.startSpeed;
			
			editorTarget.airflowLength = newLength;
			editorTarget.itemEnabled = newEnabled;
			editorTarget.fanForce = newForce;
			EditorUtility.SetDirty(editorTarget);
		}
	}
	
}
