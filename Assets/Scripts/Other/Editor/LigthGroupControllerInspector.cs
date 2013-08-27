using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LigthGroupController))]
public class LigthGroupControllerInspector : Editor {
	
	private LigthGroupController editorTarget = null;
	private List<Light> lightList = null; 
	private Texture2D selectorIcon = null;
	
	private Vector3 gizmoPoint = Vector3.zero;
	
	void OnEnable () {
		editorTarget = target as LigthGroupController;
		lightList = new List<Light>( editorTarget.GetComponentsInChildren<Light>() );
		selectorIcon = ( Texture2D )AssetDatabase.LoadAssetAtPath( @"Assets/Editor/Icons/selectorIcon.png", typeof( Texture2D ) );
	}
	
	public override void OnInspectorGUI () {
		
		if ( lightList == null || lightList.Count == 0 ) {
			GUILayout.Label( new GUIContent( "Press Add Light" ) );
		}
		
		List<Light> deletionList = new List<Light>();
		
		foreach ( Light light in lightList ) {
			GUILayout.BeginHorizontal();
			
			GUILayout.Box( new GUIContent( "LIGHT DIRECTION" ), GUILayout.Width( 150.0f ), GUILayout.Height( 150.0f ) );
			Rect boxRect = GUILayoutUtility.GetLastRect();
			Vector2 boxCenter = new Vector2( boxRect.x + boxRect.width / 2.0f, boxRect.y + boxRect.height / 2.0f );
			
			
			if ( Event.current.type == EventType.mouseDown || Event.current.type == EventType.mouseDrag ) {
				Vector2 mousePos = Event.current.mousePosition;
				if ( boxRect.Contains( mousePos ) ) {
					Vector2 boxPos = new Vector2( boxRect.x, boxRect.y );
					SetLightDirection( light, (boxCenter - mousePos) * 0.015f );
				}
			}
			
			
			Vector2 lightDirPos = boxCenter + GetLightDirection( light ) * 150.0f;
			GUI.DrawTexture( new Rect( lightDirPos.x - 16.0f, lightDirPos.y - 16.0f, 32.0f, 32.0f ), selectorIcon );
			
			GUILayout.BeginVertical();
			light.name = GUILayout.TextField( light.name, GUILayout.Width( 100.0f ) );
			light.color = EditorGUILayout.ColorField( light.color, GUILayout.Width( 100.0f ) );
			light.intensity = EditorGUILayout.Slider( new GUIContent("Intensity"), light.intensity, 0.0f, 4.0f );
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		
	}
	
	private void SetLightDirection( Light light, Vector2 direction ) {
		direction = Vector2.ClampMagnitude( direction, 1.0f );
		Vector3 lookDirection = new Vector3( -direction.x, -Mathf.Sqrt( -(direction.x*direction.x) - (direction.y*direction.y) + 1.0f ), direction.y );
		light.transform.LookAt( light.transform.position + lookDirection );
		Repaint();
	}
	
	private Vector2 GetLightDirection( Light light ) {
		Vector3 lightDirection = light.transform.forward;
		return new Vector2( lightDirection.x, -lightDirection.z ) * 0.47f;
	}
}
