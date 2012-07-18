using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlamerController))]
public class FlamerControllerEditor : Editor {
	
	private FlamerController editorTarget = null;
	private float fireLength;
	
	private BoxCollider flameCollider;
	private ParticleEmitter flameParticles;
	
	void OnEnable ()
	{
		editorTarget = target as FlamerController;
		
		flameCollider = editorTarget.gameObject.GetComponent<BoxCollider>();
		flameParticles = editorTarget.gameObject.GetComponentInChildren<ParticleEmitter>();
		
		Debug.Log(flameCollider + " - " + flameParticles);
	}
	
	void OnDisable ()
	{
		
	}
	
	public override void OnInspectorGUI ()
	{
		editorTarget.fireLength = (float)EditorGUILayout.IntSlider(Mathf.RoundToInt(editorTarget.fireLength), 1, 8 );
		
		if (GUI.changed)
		{
			Undo.RegisterUndo(new Object[]{editorTarget, flameCollider, flameParticles}, "Changed flame length");
			
			Vector3 oldColliderPos = flameCollider.center;
			Vector3 oldColliderSize = flameCollider.size;
			
			Vector3 newColliderPos = new Vector3((editorTarget.fireLength * 0.5f ) + 1f , oldColliderPos.y, oldColliderPos.z);
			Vector3 newColliderSize = new Vector3(editorTarget.fireLength, oldColliderSize.y, oldColliderSize.z);
			
			float particleSpeed = flameParticles.localVelocity.x;
			
			flameParticles.maxEnergy = (editorTarget.fireLength + 0.5f) / particleSpeed;
			flameParticles.minEnergy = (editorTarget.fireLength - 0.5f) / particleSpeed;
			
			flameCollider.center = newColliderPos;
			flameCollider.size = newColliderSize;
			
			EditorUtility.SetDirty(editorTarget);
		}
		
//		DrawDefaultInspector ();
	}
	
}
