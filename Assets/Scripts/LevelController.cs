using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
	private static LevelController _instance;
	
	public GameObject playerGroupPrefab;
	public GameObject HUDGroupPrefab;
	
	public enum GameState
	{
		PLAYING,
		PAUSED
	};

	private GameState gameState = GameState.PLAYING;
	
	private float rotation = 0f;
	private float gravityForce = 9.81f;
	private Vector3 spawnPoint;
	private float spawnRotation;
	
	private float levelTime = 0;

	private int bonusObjectsCollected = 0;
	private List<BonusObjectController> bonusObjects;

	private Transform playerTransform = null;

	public static float LevelTime {
		get { return _instance.levelTime; }
	}
	
	public static string LevelTimeString {
		get {
			float levelTime = _instance.levelTime;
			int intTime = Mathf.FloorToInt (_instance.levelTime);
			
			int num1 = Mathf.FloorToInt( levelTime / 60.0f );
			int num2 = Mathf.FloorToInt( levelTime % 60.0f );
			int num3 = Mathf.FloorToInt( (levelTime % 1.0f) * 10.0f );
		
			string timeString = num1.ToString() + ":" + num2.ToString() + ":" + num3.ToString();
			return timeString;
		}
	}

	public Transform PlayerTransform {
		get {
			return playerTransform;
		}
	}

	public void AddBonusObject (BonusObjectController bonusObject)
	{
		bonusObjects.Add (bonusObject);
	}

	public int BonusObjectsLeft() {
		return bonusObjects.Count - bonusObjectsCollected;
	}

	public static int BonusObjectsCollected {
		get { return _instance.bonusObjectsCollected; }
	}

	public static bool CollectedAllBonusObjects {
		get { return _instance.BonusObjectsLeft() == 0; }
	}

	public static GameState CurrentGameState {
		set {
			_instance.gameState = value;
			
			if (value == GameState.PAUSED) {
				Time.timeScale = 0f;
			} else {
				Time.timeScale = 1f;
			}
		}
		get { return _instance.gameState; }
	}

	private static GameObject _spawnObject = null;
	public static GameObject SpawnObject {
		get {
			if (_spawnObject == null) {
				SpawnController spawn = GameObject.FindObjectOfType<SpawnController>();
				_spawnObject = spawn != null ? spawn.gameObject : null;
			}
			return _spawnObject;
		}
	}

	private static GameObject _goalObject = null;
	public static GameObject GoalObject {
		get {
			if (_goalObject == null) {
				GoalController goal = GameObject.FindObjectOfType<GoalController>();
				_goalObject =  goal != null ? goal.gameObject : null;
			}
			return _goalObject;
		}
	}

	public static LevelController Instance {
		get { return _instance; }
	}

	void Awake () {
		_instance = this;
		bonusObjects = new List<BonusObjectController> ();
		GameObject playerGo = GameObject.Instantiate (playerGroupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		playerTransform = playerGo.GetComponentInChildren<PlayerController>().transform;
		GameObject.Instantiate (HUDGroupPrefab, Vector3.zero, Quaternion.identity);
	}
	
	void Start () {
		SpawnPlayer ();
	}
	
	void OnEnable () {
		EventDispatcher.AddHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SET_CHECKPOINT, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SPAWN, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_BONUS_OBJECT, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_LEVEL_COMPLETE, HandleEvent);
	}
	
	void OnDisable () {
		EventDispatcher.RemoveHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SET_CHECKPOINT, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SPAWN, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_BONUS_OBJECT, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_LEVEL_COMPLETE, HandleEvent);
	}
	
	void Update () {
		if (LevelController.CurrentGameState == LevelController.GameState.PLAYING) {
			levelTime += Time.deltaTime;
		}
	}
	
	private void HandleEvent (string eventName, object param) {
		switch (eventName) {
		case EventKey.INPUT_ROTATE:
			Rotate ((float)param);
			break;
		case EventKey.GAME_SET_ROTATION:
			SetRotation ((float)param);
			break;
		case EventKey.PLAYER_SET_CHECKPOINT:
			Vector4 cpParam = (Vector4)param;
			SetCheckpoint (new Vector3 (cpParam.x, cpParam.y, cpParam.z), cpParam.w);
			break;
		case EventKey.PLAYER_SPAWN:
			SpawnPlayer ();
			break;
		case EventKey.PLAYER_BONUS_OBJECT:
			bonusObjects.Remove ((BonusObjectController)param);
			bonusObjectsCollected++;
			if (bonusObjects.Count == 0) {
				EventDispatcher.SendEvent (EventKey.GAME_ENABLE_GOAL);
			}
			break;
		case EventKey.GAME_LEVEL_COMPLETE:
			gameState = GameState.PAUSED;
			break;
		default:
			Debug.LogWarning ("No handler for this event implemented.");
			break;
		}
	}
	
	private void SpawnPlayer () {
		EventDispatcher.SendEvent (EventKey.PLAYER_MOVE, spawnPoint); // Move player to start position
		EventDispatcher.SendEvent (EventKey.GAME_SET_ROTATION, spawnRotation); // Rotate to the level starting rotation
		EventDispatcher.SendEvent (EventKey.PLAYER_TOGGLE_ACTIVE, true); // Activate the player
	}
	
	private void Rotate (float rotationDelta) {
		float newRotation = rotation + rotationDelta;
		
		EventDispatcher.SendEvent (EventKey.GAME_SET_ROTATION, newRotation);
	}
	
	private void SetRotation (float degrees) {
		rotation = Mathf.Repeat (degrees, 360f);
		
		float rad = rotation * Mathf.Deg2Rad;
		Physics.gravity = new Vector3 (Mathf.Sin (rad), 0f, Mathf.Cos (rad)) * -gravityForce;
	}

	private void SetCheckpoint (Vector3 pos, float rot) {
		spawnPoint = pos;
		spawnRotation = rot;
	}

}
