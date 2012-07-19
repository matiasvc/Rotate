using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CheckpointController))]
public class CheckpointControllerInspector : Editor {
	
	private CheckpointController editorTarget = null;
	
	void OnEnable ()
	{
		editorTarget = target as CheckpointController;
	}
	
	void OnDisable ()
	{
		
	}
	
	public override void OnInspectorGUI ()
	{
		editorTarget.checkpointLength = (float)EditorGUILayout.IntSlider( Mathf.RoundToInt(editorTarget.checkpointLength), 1, 8 );
		
		if (GUI.changed)
		{
			Undo.RegisterUndo(new Object[]{editorTarget, editorTarget.gameObject, editorTarget.otherCheckpoint.gameObject}, "Changed checkpoint length");
			
			editorTarget.otherCheckpoint.localPosition = new Vector3(editorTarget.checkpointLength + 2f, 0f, 1f);
			
			BoxCollider collider = editorTarget.GetComponent<BoxCollider>();
			
			Vector3 newColPos = collider.center;
			Vector3 newColSize = collider.size;
			
			newColPos = new Vector3(editorTarget.checkpointLength * 0.5f + 1f, newColPos.y, newColPos.z);
			newColSize = new Vector3(editorTarget.checkpointLength, newColSize.y, newColSize.z);
			
			collider.center = newColPos;
			collider.size = newColSize;
			
			EditorUtility.SetDirty(editorTarget);
			EditorUtility.SetDirty(collider);
		}
	}
	
}
