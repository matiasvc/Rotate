using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemTimer))]
public class ItemTimerInspector : Editor {
	
	private ItemTimer editorTarget = null;
	
	void OnEnbale () {
		editorTarget = target as ItemTimer;
	}
	
	
	public override void OnInspectorGUI () {
		base.OnInspectorGUI();
	}
}
