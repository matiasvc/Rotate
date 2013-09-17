using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlamerController))]
public class FlamerControllerInspector : Editor {
	
	private FlamerController editorTarget = null;
	private float fireLength;
	
	void OnEnable ()
	{
		editorTarget = target as FlamerController;
	}
	
	public override void OnInspectorGUI ()
	{

		float newLength = (float)EditorGUILayout.IntSlider("Flame length", Mathf.RoundToInt(editorTarget.FireLength), 1, 8 );
		
		if (newLength != editorTarget.FireLength){
			Undo.RegisterUndo(new Object[]{editorTarget, editorTarget.FlameCollider, editorTarget.FlameParticleEmitter}, "Changed flame length");
			editorTarget.SetFlameLength(newLength);
			EditorUtility.SetDirty(editorTarget);
		}
		
	}
	
}
