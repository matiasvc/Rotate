using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlamerController))]
public class FlamerControllerEditor : Editor {
	
	private FlamerController editorTarget = null;
	private float fireLength;
	
	void OnEnable ()
	{
		editorTarget = target as FlamerController;
	}
	
	void OnDisable ()
	{
		
	}
	
	public override void OnInspectorGUI () // TODO Check this method when less tired
	{
		editorTarget.fireLength = (float)EditorGUILayout.IntSlider( Mathf.RoundToInt(editorTarget.fireLength), 1, 8 );
		
		if( GUI.changed )
		{	
			Vector3 oldColliderPos = editorTarget.FlameCollider.center;
			Vector3 oldColliderSize = editorTarget.FlameCollider.size;
			
			Vector3 newColliderPos = new Vector3( ( editorTarget.fireLength * 0.5f ) + 1f , oldColliderPos.y, oldColliderPos.z );
			Vector3 newColliderSize = new Vector3( editorTarget.fireLength, oldColliderSize.y, oldColliderSize.z );
			
			float particleSpeed = editorTarget.FlameParticles.localVelocity.x;
			
			editorTarget.FlameParticles.maxEnergy = ( editorTarget.fireLength + 0.5f ) / particleSpeed;
			editorTarget.FlameParticles.minEnergy = ( editorTarget.fireLength - 0.5f ) / particleSpeed;
			
			editorTarget.FlameCollider.center = newColliderPos;
			editorTarget.FlameCollider.size = newColliderSize;
			
			EditorUtility.SetDirty(editorTarget);
		}
		
//		DrawDefaultInspector ();
	}
	
}
