using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelEditor : EditorWindow {
	
	public class LevelItem : Object {
		public string itemName;
		
		private Texture2D _icon = null;
		public Texture2D icon
		{
			get
			{
				int i = 0;
				
				while(this._icon == null)
				{
					if(i > 25)
						break;
					
					_icon = EditorUtility.GetAssetPreview(this.prefab);
					i++;
				}
				
				if (this._icon == null)
					_icon = (Texture2D)AssetDatabase.LoadAssetAtPath(@"Assets/Editor/Icons/missingIcon.png", typeof(Texture2D));
				
				return _icon;
			}
		}
		
		
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
			
			itemPrefabs.Add(newItemObject);
		}
		
		levelPrefabs = new List<LevelItem>();
		string[] levelPrefabPaths = Directory.GetFiles((@"Assets\Prefabs\Level\").Replace(@"\", Path.AltDirectorySeparatorChar.ToString()), "*.prefab");
		
		foreach( string objectPath in levelPrefabPaths ) // Build list of level prefabs
		{			
			LevelItem newLevelObject = new LevelItem();
			
			newLevelObject.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject) );
			newLevelObject.itemName = newLevelObject.prefab.name;

			
			levelPrefabs.Add(newLevelObject);
		}
	}
	
	
	
	void OnGUI ()
	{
		DrawToolButtons();
		DrawItemButtons();
		DrawLevelButtons();
	}
	
	private void DrawToolButtons()
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
			Undo.RegisterUndo(Selection.gameObjects, "Rotate UVs");
			
			foreach( GameObject go in Selection.gameObjects )
			{
				MeshFilter meshFilter = go.GetComponentInChildren<MeshFilter>();
				Mesh mesh = (Mesh)Instantiate(meshFilter.sharedMesh);
				
				Vector2[] uvArray = mesh.uv;
				
				for (int i = 0; i < uvArray.Length; i++)
				{
					uvArray[i] = Quaternion.Euler(0f, 0f, 90f) * uvArray[i];
				}
				
				mesh.uv = uvArray;
				meshFilter.sharedMesh = mesh;
				
				EditorUtility.SetDirty(meshFilter);
			}
		}
		
		GUILayout.EndHorizontal();
		
		if(GUILayout.Button("Refresh", GUILayout.Width(70f)))
		{
			Refresh();
		}
	}
	
	private void DrawItemButtons()
	{
		GUILayout.Space(20f);
		GUILayout.Label("Items");
		GUILayout.BeginHorizontal();
		
		float buttonSize = 100f;
		int btni = 0;
		int btnPerRow = Mathf.FloorToInt(window.position.width / buttonSize);
		
		for ( int i = 0; i < itemPrefabs.Count; i++ )
		{
			LevelItem levelItem = itemPrefabs[i];
			
			GUILayout.BeginVertical();
			
			if( GUILayout.Button(levelItem.icon, GUILayout.Height(buttonSize), GUILayout.Width(buttonSize)) )
			{
				string itemParentName = "items";
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
	
	private void DrawLevelButtons()
	{
		// Level prefab buttons
		GUILayout.Label("Level");
		GUILayout.BeginHorizontal();
		
		float buttonSize = 100f;
		int btni = 0;
		int btnPerRow = Mathf.FloorToInt(window.position.width / (buttonSize + 3f));
		
		for ( int i = 0; i < levelPrefabs.Count; i++ )
		{
			LevelItem levelItem = levelPrefabs[i];
			
			GUILayout.BeginVertical();
			
			if( GUILayout.Button(levelItem.icon, GUILayout.Height(buttonSize), GUILayout.Width(buttonSize)) )
			{
				string itemParentName = "level";
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