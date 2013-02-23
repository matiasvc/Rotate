using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelEditor : EditorWindow {
	
	private const float UISize = 18f;
	
	public class PrefabItem : Object {
		public string itemName;
		public string parent;
		public GameObject prefab;
		
		private Texture2D _icon = null;
		public Texture2D icon {
			get {
				do { // TODO: Fix potential infinite loop
					_icon = AssetPreview.GetAssetPreview(this.prefab);
				} while (AssetPreview.IsLoadingAssetPreviews());
				
				if (this._icon == null)
					_icon = (Texture2D)AssetDatabase.LoadAssetAtPath(@"Assets/Editor/Icons/missingIcon.png", typeof(Texture2D));
				
				return _icon;
			}
		}
	}
	
	private static LevelEditor _window = null;
	private static LevelEditor window
	{
		get
		{
			if (_window == null)
			{
				_window = (LevelEditor)EditorWindow.GetWindow (typeof (LevelEditor));
			}
			return _window;
		}
	}
	
	private int toolbarSelected = 0;
	
	private List<PrefabItem> itemPrefabs;
	private List<PrefabItem> levelPrefabs;
	
	[MenuItem ("Tools/Level Editor")]
	static void Init()
	{
		_window = (LevelEditor)EditorWindow.GetWindow (typeof (LevelEditor));
	}
	
	void OnEnable ()
	{
		BuildAssetLists();
	}
	
	private void BuildAssetLists()
	{
		itemPrefabs = new List<PrefabItem>();
		string[] itemPrefabPaths = Directory.GetFiles((@"Assets\Prefabs\Items\").Replace(@"\", Path.AltDirectorySeparatorChar.ToString()), "*.prefab");
		
		foreach( string objectPath in itemPrefabPaths ) // Build list of item prefabs
		{			
			PrefabItem newItemObject = new PrefabItem();
			
			newItemObject.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject) );
			newItemObject.itemName = newItemObject.prefab.name;
			newItemObject.parent = "items";
			
			itemPrefabs.Add(newItemObject);
		}
		
		levelPrefabs = new List<PrefabItem>();
		string[] levelPrefabPaths = Directory.GetFiles((@"Assets\Prefabs\Level\").Replace(@"\", Path.AltDirectorySeparatorChar.ToString()), "*.prefab");
		
		foreach( string objectPath in levelPrefabPaths ) // Build list of level prefabs
		{			
			PrefabItem newLevelObject = new PrefabItem();
			
			newLevelObject.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject) );
			newLevelObject.itemName = newLevelObject.prefab.name;
			newLevelObject.parent = "level";
			
			levelPrefabs.Add(newLevelObject);
		}
	}
	
	void OnGUI ()
	{
		
		DrawToolButtons();
		
		
		if (IsLevelInitialized()) {
			toolbarSelected = GUI.Toolbar(new Rect(0.25f, 10f, 18f, 1f).Multiply(UISize), toolbarSelected, new string[]{"Level", "Items", "Components", "Properties"});
			
			switch (toolbarSelected) {
			case 0:
				BuildAssetLists();
				DrawLevelButtons();
				break;
			case 1:
				BuildAssetLists();
				DrawItemButtons();
				break;
			case 2:
				
				break;
			case 3:
				
				break;
			}
			
		} else {
			if (GUI.Button(new Rect(0.25f, 10f, 18f, 1f).Multiply(UISize), new GUIContent("Initialize Level"))) {
				InitializeLevel();
			}
		}
	}
	
	
	private void MoveSelectedObjects(Vector3 displacement) {
		foreach (GameObject selected in Selection.gameObjects) {
			Vector3 oldPos = selected.transform.localPosition;
			Vector3 newPos = oldPos + displacement;
			selected.transform.localPosition = newPos;
		}
	}
	
	private void RotateSelectedObjects(Vector3 rotation) {
		foreach (GameObject selected in Selection.gameObjects) {
			Vector3 oldRot = selected.transform.localEulerAngles;
			Vector3 newRot = oldRot + new Vector3(	Mathf.Repeat(rotation.x, 360f),
													Mathf.Repeat(rotation.y, 360f),
													Mathf.Repeat(rotation.z, 360f)	);
			selected.transform.localEulerAngles = newRot;
		}
	}
	
	private void RotateSelectedUVs() {
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
	
	private void DrawToolButtons()
	{
		// Move Buttons
		Rect[] posRotButtons = RectEx.RectGrid(new Rect(0.25f, 0.25f, 9f, 9f), 3, 3);
		
		if (GUI.Button(posRotButtons[0].Multiply(UISize), new GUIContent("Rotate\nLeft", "Rotates selected objects counter-clockwise"))) {
			RotateSelectedObjects(new Vector3(0f, -90f, 0f));
		}
		
		if (GUI.Button(posRotButtons[1].Multiply(UISize), "+Z")) {
			MoveSelectedObjects(new Vector3(0f, 0f, 1f));
		}
		
		if (GUI.Button(posRotButtons[2].Multiply(UISize), "Rotate\nRigth")) {
			RotateSelectedObjects(new Vector3(0f, 90f, 0f));
		}
		
		
		if (GUI.Button(posRotButtons[3].Multiply(UISize), "-X")) {
			MoveSelectedObjects(new Vector3(-1f, 0f, 0f));
		}
		
		if (GUI.Button(posRotButtons[4].Multiply(UISize), "Zero")) {
			foreach( GameObject selected in Selection.gameObjects ) {
				Vector3 currentPos = selected.transform.localPosition;
				selected.transform.localPosition = new Vector3(0f, currentPos.y, 0f);
			}
		}
		
		if (GUI.Button(posRotButtons[5].Multiply(UISize), "+X")) {
			MoveSelectedObjects(new Vector3(1f, 0f, 0f));
		}
		
		if (GUI.Button(posRotButtons[6].Multiply(UISize), "-Y")) {
			MoveSelectedObjects(new Vector3(0f, -1f, 0f));
		}
		
		if (GUI.Button(posRotButtons[7].Multiply(UISize), "-Z")) {
			MoveSelectedObjects(new Vector3(0f, 0f, -1f));
		}
		
		if (GUI.Button(posRotButtons[8].Multiply(UISize), "+Y")) {
			MoveSelectedObjects(new Vector3(0f, 1f, 0f));
		}
		
		// Tool Buttons
		Rect[] toolButtons = RectEx.RectGrid(new Rect(10f, 0.25f, 8f, 9f), 2, 4);
		
		if (GUI.Button(toolButtons[0].Multiply(UISize), "Rotate UV")) {
			RotateSelectedUVs();
		}
		
		GUI.enabled = false;
		
		if (GUI.Button(toolButtons[1].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[2].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[3].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[4].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[5].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[6].Multiply(UISize), "----")) {
			
		}
		
		if (GUI.Button(toolButtons[7].Multiply(UISize), "----")) {
			
		}
		
		GUI.enabled = true;
		
		
	}
	
	private bool IsLevelInitialized()
	{	
		GameObject ligthGroup = GameObject.Find("LigthGroup");
		GameObject levelController = GameObject.Find("LevelController");
		GameObject gameManager = GameObject.Find("GameManager");
		
		if (ligthGroup == null || levelController == null || gameManager == null)
			return false;
		else
			return true;
	}
	
	private void InitializeLevel()
	{
		GameObject ligthGroup = GameObject.Find("LigthGroup");
		GameObject levelController = GameObject.Find("LevelController");
		GameObject gameManager = GameObject.Find("GameManager");
		
		if (ligthGroup == null)
			PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath(@"Assets/Prefabs/Other/LigthGroup.prefab", typeof(GameObject)));
		
		if (levelController == null)
			PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath(@"Assets/Prefabs/Other/LevelController.prefab", typeof(GameObject)));
		
		if (gameManager == null)
			PrefabUtility.InstantiatePrefab((GameObject)AssetDatabase.LoadAssetAtPath(@"Assets/Prefabs/Other/GameManager.prefab", typeof(GameObject)));
	}
	
	private void DrawComponentButtons() // TODO Update this
	{
		if (GUILayout.Button("Add Timer", GUILayout.Width(100f)))
		{
			foreach( GameObject selected in Selection.gameObjects )
			{
				Undo.RegisterUndo(selected, "Added ItemTimer.");
				selected.AddComponent<ItemTimer>();
			}
		}
		
	}
	
	private void DrawPrefabButton(Rect btnPos, PrefabItem prefabItem) {
		
		if (GUI.Button(btnPos, new GUIContent(prefabItem.icon))) {
			
			GameObject itemParent = GameObject.Find("/" + prefabItem.parent);
			
			if(itemParent == null) { // Instantiate new parent object if none was found in the scene.
				itemParent = new GameObject(prefabItem.parent);
			}
			
			GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefabItem.prefab);
			go.transform.parent = itemParent.transform;
			
			Vector3 itemPos = Vector3.zero;
			
			if(Selection.gameObjects.Length > 0) { // Spawn the new object at the position of the selected object if there is one.
				itemPos = Selection.gameObjects[0].transform.position;
			}
			
			go.transform.position = itemPos;
			Selection.objects = new Object[]{go}; // Select the newly created object.
		}
		GUI.Label(btnPos, new GUIContent(prefabItem.itemName));
	}
	
	private void DrawItemButtons() {
		float buttonSize = 3f;
		int btnCount = itemPrefabs.Count;
		int rowCount = Mathf.CeilToInt((float)btnCount / 6f);
		
		Rect[] itemBtnPosArray = RectEx.RectGrid(new Rect(0.25f, 12f, 18f, 3f * (float)rowCount), 6, rowCount);
		
		for (int i = 0; i < btnCount; i++) {
			DrawPrefabButton(itemBtnPosArray[i].Multiply(UISize), itemPrefabs[i]);
		}
	}
	
	private void DrawLevelButtons() {
		float buttonSize = 3f;
		int btnCount = levelPrefabs.Count;
		int rowCount = Mathf.CeilToInt((float)btnCount / 6f);
		
		Rect[] itemBtnPosArray = RectEx.RectGrid(new Rect(0.25f, 12f, 18f, 3f * (float)rowCount), 6, rowCount);
		
		for (int i = 0; i < btnCount; i++) {
			DrawPrefabButton(itemBtnPosArray[i].Multiply(UISize), levelPrefabs[i]);
		}
	}
	
}