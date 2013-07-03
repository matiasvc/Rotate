using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ElectroController))]
public class ElectroControllerInspector : Editor {
	
	private ElectroController editorTarget = null;
	
	void OnEnable () {
		editorTarget = target as ElectroController;
	}
	
	void OnDisable () {
		
	}
	
	public override void OnInspectorGUI () {
		
		editorTarget.connectedElectro = EditorGUILayout.ObjectField( "Connected electro", editorTarget.connectedElectro, typeof( ElectroController ), true ) as ElectroController;
		
		if ( editorTarget.connectedElectro != null )
		{
			if ( GUILayout.Button( new GUIContent( "Remove Connection" ) ) ) {
				editorTarget.connectedElectro = null;
				editorTarget.beamAnimator.Disable();
				SceneView.RepaintAll();
			}
		} else if ( !SceneObjectSelector.inUse ) {
			if ( GUILayout.Button( new GUIContent( "Set Connection" ) ) ) {
				SceneObjectSelector.Setup<ElectroController>( ConnectElectro );
			}	
		} else {
			if ( GUILayout.Button( new GUIContent( "Cancel" ) ) ) {
				SceneObjectSelector.Deconstruct();
			}
			GUILayout.Label( new GUIContent( "Click on a blue sphere in the scene\nto connect to that electro." ) );
		}
	}
	
	private void ConnectElectro (object connectTo ) {
		editorTarget.connectedElectro = (ElectroController)connectTo;
		EditorUtility.SetDirty( editorTarget );
		SetUpBeam();
	}
	
	private void SetUpBeam () {
		Vector3 fromPos = editorTarget.BeamPoint;
		Vector3 toPos = editorTarget.connectedElectro.BeamPoint;
		editorTarget.beamAnimator.SetUp( fromPos, toPos );
		EditorUtility.SetDirty( editorTarget.beamAnimator );
	}
	
}
