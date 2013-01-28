using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
	private static LevelController _instance;
	
	public GameObject playerGroupPrefab;
	public GameObject HUDGroupPrefab;
	
	public enum GameState {PLAYING, PAUSED};
	private GameState gameState = GameState.PLAYING;
	
	private float rotation = 0f;
	private float gravityForce = 9.81f;
	private Vector3 spawnPoint;
	private float spawnRotation;
	
	private float levelTime = 0;
	
	private int bonusObjectsCollected = 0;
	
	private GameObject playerGroup;
	private GameObject HUDGroup;
	
	public static float LevelTime
	{
		get { return _instance.levelTime; }
	}
	
	public static string LevelTimeString
	{
		get
		{
			int intTime = Mathf.FloorToInt(_instance.levelTime);
			
			int num1 = intTime / 100;
			int num2 = (intTime / 10) % 10;
			int num3 = intTime % 10;
			
			string timeString = num1.ToString() + ":" + num2.ToString() + ":" + num3.ToString();
			return timeString;
		}
	}
	
	public static int BonusObjectsCollected
	{
		get { return _instance.bonusObjectsCollected; }
	}
	
	public static GameState CurrentGameState
	{
		set
		{
			_instance.gameState = value;
			
			if (value == GameState.PAUSED)
				Time.timeScale = 0f;
			else
				Time.timeScale = 1f;
		}
		get { return _instance.gameState; }
	}
	
	void Awake()
	{
		_instance = this;
	}
	
	void Start()
	{
		playerGroup = GameObject.Instantiate(playerGroupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		HUDGroup = GameObject.Instantiate(HUDGroupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		SpawnPlayer();
	}
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SET_CHECKPOINT, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SPAWN, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_BONUS_OBJECT, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SET_CHECKPOINT, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SPAWN, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_BONUS_OBJECT, HandleEvent);
	}
	
	void Update()
	{
		if (LevelController.CurrentGameState == LevelController.GameState.PLAYING)
		{
			levelTime += Time.deltaTime;
		}
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.INPUT_ROTATE:
			Rotate((float)param);
			break;
		case EventKey.GAME_SET_ROTATION:
			SetRotation((float)param);
			break;
		case EventKey.PLAYER_SET_CHECKPOINT:
			Vector4 cpParam = (Vector4)param;
			SetCheckpoint(new Vector3(cpParam.x, cpParam.y, cpParam.z), cpParam.w);
			break;
		case EventKey.PLAYER_SPAWN:
			SpawnPlayer();
			break;
		case EventKey.PLAYER_BONUS_OBJECT:
			bonusObjectsCollected++;
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void SpawnPlayer()
	{
		EventDispatcher.SendEvent(EventKey.PLAYER_MOVE, spawnPoint); // Move player to start position
		EventDispatcher.SendEvent(EventKey.GAME_SET_ROTATION, spawnRotation); // Rotate to the level starting rotation
		EventDispatcher.SendEvent(EventKey.PLAYER_TOGGLE_ACTIVE, true); // Activate the player
	}
	
	private void Rotate(float rotationDelta)
	{
		float newRotation = rotation + rotationDelta;
		
		EventDispatcher.SendEvent(EventKey.GAME_SET_ROTATION, newRotation);
	}
	
	private void SetRotation(float degrees)
	{
		rotation = Mathf.Repeat(degrees, 360f);
		
		float rad = rotation * Mathf.Deg2Rad;
		Physics.gravity = new Vector3 (Mathf.Sin (rad), 0f, Mathf.Cos (rad)) * -gravityForce;
	}
	
	private void SetCheckpoint(Vector3 pos, float rot)
	{
		spawnPoint = pos;
		spawnRotation = rot;
	}
	
}
