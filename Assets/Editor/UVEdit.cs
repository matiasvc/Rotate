using UnityEngine;
using UnityEditor;

public class UVEdit : EditorWindow {
	public static UVEdit window;
	
	[MenuItem ("Tools/UV Edit")]
	static void Init()
	{
		window = (UVEdit)EditorWindow.GetWindow (typeof (UVEdit));
	}
	
	void OnGUI ()
	{
		if ( GUILayout.Button("Rotate Rigth",  GUILayout.MaxWidth(100f)) )
		{
			RotatesUV(Selection.gameObjects);
		}
		
		if( GUILayout.Button("Rotate Left", GUILayout.MaxWidth(100f)) )
		{
			
		}
	}
	
	private void RotatesUV ( GameObject[] gameObjects )
	{
		Undo.RegisterUndo(gameObjects, "Rotate UVs");
		
		foreach ( GameObject go in gameObjects )
		{
			if(go == null)
				continue;
			
			Mesh mesh = go.GetComponent<MeshFilter>().mesh;
			
			Vector2[] uvs = mesh.uv;
			
			for (int i = 0; i < uvs.Length; i++)
			{
				Vector2 uv = uvs[i];
				Vector2 newUv = uv;
				
				if(uv.x < 0.5f && uv.y < 0.5f) // Lower rigth
				{
					newUv.y = 1f - newUv.y;
				}
				else if(uv.x < 0.5f && uv.y > 0.5f) // Upper rigth
				{
					newUv.x = 1f - newUv.x;
				}
				else if(uv.x > 0.5f && uv.y < 0.5f) // Lower left
				{
					newUv.x = 1f - newUv.x;
				}
				else if(uv.x > 0.5f && uv.y > 0.5f) // Upper left
				{
					newUv.y = 1f - newUv.y;
				}
				else
				{
					Debug.Log("Error: " + uv);
				}
				
				Debug.Log("Old uv: " + uv + " New uv: " + newUv);
				
				uv = newUv;
			}
			
			go.GetComponent<MeshFilter>().sharedMesh = mesh;
		}
	}
}