using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelEditor : EditorWindow {
	
	private class LevelItem : Object {
		public string itemName;
		public Texture2D icon;
		public GameObject prefab;
	}
	
	public static LevelEditor window;
	
	private List<LevelItem> itemPrefabs;
	private List<LevelItem> levelPrefabs;
	
	[MenuItem ("Tools/Level Editor")]
	static void Init()
	{
		window = (LevelEditor)EditorWindow.GetWindow (typeof (LevelEditor));
	}
	
	void OnEnable ()
	{
		Refresh();
	}
	
	private void Refresh()
	{
		itemPrefabs = new List<LevelItem>();
		string[] itemPrefabPaths = Directory.GetFiles((@"Assets\Prefabs\Items\").Replace(@"\", Path.AltDirectorySeparatorChar.ToString()), "*.prefab");
		
		foreach( string objectPath in itemPrefabPaths ) // Build list of item prefabs
		{			
			LevelItem newItemObject = new LevelItem();
			
			newItemObject.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject) );
			newItemObject.itemName = newItemObject.prefab.name;
			
			string iconPath = @"Assets/Editor/Icons/" + newItemObject.itemName + ".png";
			newItemObject.icon = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D) );
			
			if(newItemObject.icon == null)
			{
				Debug.LogWarning("Level Editor Warning: Could not find a icon file for: \"" + newItemObject.itemName + "\" Path: " + iconPath);
				newItemObject.icon = (Texture2D)AssetDatabase.LoadAssetAtPath(@"Assets/Editor/Icons/placeholder.png", typeof(Texture2D) );
			}
			
			itemPrefabs.Add(newItemObject);
		}
		
		levelPrefabs = new List<LevelItem>();
		string[] levelPrefabPaths = Directory.GetFiles((@"Assets\Prefabs\Level\").Replace(@"\", Path.AltDirectorySeparatorChar.ToString()), "*.prefab");
		
		foreach( string objectPath in levelPrefabPaths ) // Build list of level prefabs
		{			
			LevelItem newLevelObject = new LevelItem();
			
			newLevelObject.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject) );
			newLevelObject.itemName = newLevelObject.prefab.name;
			
			string iconPath = @"Assets/Editor/Icons/" + newLevelObject.itemName + ".png";
			newLevelObject.icon = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D) );
			
			if(newLevelObject.icon == null)
			{
				Debug.LogWarning("Level Editor Warning: Could not find a icon file for: \"" + newLevelObject.itemName + "\" Path: " + iconPath);
				newLevelObject.icon = (Texture2D)AssetDatabase.LoadAssetAtPath(@"Assets/Editor/Icons/placeholder.png", typeof(Texture2D) );
			}
			
			levelPrefabs.Add(newLevelObject);
		}
	}
	
	void OnGUI ()
	{
		
		// Move Buttons
		float moveBtnSize = 50f;
		GUILayout.BeginHorizontal();
		
		if( GUILayout.Button("Rotate\nLeft", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentRot = selected.transform.localEulerAngles;
				float newRot = Mathf.Repeat(currentRot.y - 90f, 360f);
				selected.transform.localEulerAngles = new Vector3(currentRot.x, Mathf.Round(newRot), currentRot.z);
			}
		}
		
		if( GUILayout.Button("Z+", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.z + 1f);
				selected.transform.localPosition = new Vector3(currentPos.x, currentPos.y, newPos);
			}
		}
		
		if( GUILayout.Button("Rotate\nRigth", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentRot = selected.transform.localEulerAngles;
				float newRot = Mathf.Repeat(currentRot.y + 90f, 360f);
				selected.transform.localEulerAngles = new Vector3(currentRot.x, Mathf.Round(newRot), currentRot.z);
			}
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if( GUILayout.Button("X-", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.x - 1f);
				selected.transform.localPosition = new Vector3(newPos, currentPos.y, currentPos.z);
			}
		}
		
		if( GUILayout.Button("Zero", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				selected.transform.localPosition = new Vector3(0f, currentPos.y, 0f);
			}
		}
		
		if( GUILayout.Button("X+", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.x + 1f);
				selected.transform.localPosition = new Vector3(newPos, currentPos.y, currentPos.z);
			}
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if( GUILayout.Button("Y-", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.y - 1f);
				selected.transform.localPosition = new Vector3(currentPos.x, newPos, currentPos.z);
			}
		}
		
		if( GUILayout.Button("Z-", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.z - 1f);
				selected.transform.localPosition = new Vector3(currentPos.x, currentPos.y, newPos);
			}
		}
		
		if( GUILayout.Button("Y+", GUILayout.Width(moveBtnSize), GUILayout.Height(moveBtnSize)) )
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Vector3 currentPos = selected.transform.localPosition;
				float newPos = Mathf.Round(currentPos.y + 1f);
				selected.transform.localPosition = new Vector3(currentPos.x, newPos, currentPos.z);
			}
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if( GUILayout.Button("Rotate UV", GUILayout.Width(70f)) )
		{
			Debug.LogError("Not yet implemented");
			/*
			foreach( GameObject selected in Selection.gameObjects )
			{
				MeshFilter meshFilter = selected.GetComponentInChildren<MeshFilter>();
				
				Vector2[] oldUVPoints = meshFilter.mesh.uv;
				Vector2[] newUVPoints = new Vector2[oldUVPoints.Length];
				
				for (int i = 0; i < oldUVPoints.Length; i++)
				{
					Vector2 oldUVPoint = oldUVPoints[i];
					newUVPoints[i] = new Vector2(oldUVPoint.y, oldUVPoint.x);
				}
				
				meshFilter.mesh.uv = newUVPoints;
			}
			*/
		}
		
		GUILayout.EndHorizontal();
		
		if(GUILayout.Button("Refresh", GUILayout.Width(70f)))
		{
			Refresh();
		}
		
		GUILayout.Space(20f);
		
		{
			// Item Prefab buttons
			GUILayout.Label("Items");
			GUILayout.BeginHorizontal();
			
			float buttonSize = 60f;
			int btni = 0;
			int btnPerRow = Mathf.FloorToInt(window.position.width / buttonSize);
			
			for ( int i = 0; i < itemPrefabs.Count; i++ )
			{
				LevelItem levelItem = itemPrefabs[i];
				
				GUILayout.BeginVertical();
				
				if( GUILayout.Button(levelItem.icon, GUILayout.Height(buttonSize), GUILayout.Width(buttonSize)) )
				{
					string itemParentName = "_items";
					GameObject itemParent = GameObject.Find("/" + itemParentName);
					
					if(itemParent == null) // Instantiate new parent object if none was found in the scene.
					{
						itemParent = new GameObject(itemParentName);
					}
					
					GameObject go = (GameObject)EditorUtility.InstantiatePrefab(levelItem.prefab);
					go.transform.parent = itemParent.transform;
					
					Vector3 itemPos = Vector3.zero;
					
					if(Selection.gameObjects.Length > 0)
					{
						itemPos = Selection.gameObjects[0].transform.position;
					}
					
					go.transform.position = itemPos;
					
					Selection.objects = new Object[]{go}; // Select newly created object
				}
				GUILayout.Label(levelItem.itemName, GUILayout.Width(buttonSize));
				
				GUILayout.EndVertical();
				btni++;
				
				if(btni >= btnPerRow)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					btni = 0;
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		
		{
			// Level prefab buttons
			GUILayout.Label("Level");
			GUILayout.BeginHorizontal();
			
			float buttonSize = 65f;
			int btni = 0;
			int btnPerRow = Mathf.FloorToInt(window.position.width / (buttonSize + 3f));
			
			for ( int i = 0; i < levelPrefabs.Count; i++ )
			{
				LevelItem levelItem = levelPrefabs[i];
				
				GUILayout.BeginVertical();
				
				if( GUILayout.Button(levelItem.icon, GUILayout.Height(buttonSize), GUILayout.Width(buttonSize)) )
				{
					string itemParentName = "_level";
					GameObject itemParent = GameObject.Find("/" + itemParentName);
					
					if(itemParent == null) // Instantiate new parent object if none was found in the scene.
					{
						itemParent = new GameObject(itemParentName);
					}
					
					GameObject go = (GameObject)EditorUtility.InstantiatePrefab(levelItem.prefab);
					go.transform.parent = itemParent.transform;
					
					Vector3 itemPos = Vector3.zero;
					
					if(Selection.gameObjects.Length > 0)
					{
						itemPos = Selection.gameObjects[0].transform.position;
					}
					
					go.transform.position = itemPos;
					
					Selection.objects = new Object[]{go}; // Select newly created object
				}
				
				GUILayout.Label(levelItem.itemName, GUILayout.Width(buttonSize));
				
				GUILayout.EndVertical();
				
				btni++;
				
				if(btni >= btnPerRow)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					btni = 0;
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		
		
	}
	
}