using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlamerController))]
public class FlamerControllerEditor : Editor {
	
	FlamerController editorTarget = null;
	
	void OnEnable ()
	{
		editorTarget = target as FlamerController;
	}
	
	void OnDisable ()
	{
		
	}
	
	public override void OnInspectorGUI ()
	{
		float fireLength = (float)EditorGUILayout.IntSlider( Mathf.RoundToInt(editorTarget.fireLength), 1, 8 );
		
		if( !Mathf.Approximately( fireLength, editorTarget.fireLength ) )
		{
			Vector3 oldColliderPos = editorTarget.FlameCollider.center;
			Vector3 oldColliderSize = editorTarget.FlameCollider.size;
			
			Vector3 newColliderPos = new Vector3( ( fireLength * 0.5f ) + 1f , oldColliderPos.y, oldColliderPos.z );
			Vector3 newColliderSize = new Vector3( fireLength, oldColliderSize.y, oldColliderSize.z );
			
			float particleSpeed = editorTarget.FlameParticles.localVelocity.x;
			
			editorTarget.FlameParticles.maxEnergy = ( fireLength + 0.5f ) / particleSpeed;
			editorTarget.FlameParticles.minEnergy = ( fireLength - 0.5f ) / particleSpeed;
			
			editorTarget.FlameCollider.center = newColliderPos;
			editorTarget.FlameCollider.size = newColliderSize;
			
			editorTarget.fireLength = fireLength;
		}
		
//		DrawDefaultInspector ();
	}
	
}
