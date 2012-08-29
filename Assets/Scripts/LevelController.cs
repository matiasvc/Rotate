using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	
	public static LevelController Instance;
	
	public GameObject playerGroupPrefab;
	public GameObject HUDGroupPrefab;
	
	private float rotation = 0f;
	private float gravityForce = 9.8f;
	private Vector3 spawnPoint;
	private float spawnRotation;
	
	private GameObject playerGroup;
	private GameObject HUDGroup;
	
	void Awake()
	{
		Instance = this;
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
		EventDispatcher.AddHandler(EventKey.GAME_SETROTATION, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SETCHECKPOINT, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_SPAWN, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_SETROTATION, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SETCHECKPOINT, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_SPAWN, HandleEvent);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.INPUT_ROTATE:
			Rotate((float)param);
			break;
		case EventKey.GAME_SETROTATION:
			SetRotation((float)param);
			break;
		case EventKey.PLAYER_SETCHECKPOINT:
			Vector4 cpParam = (Vector4)param;
			SetCheckpoint(new Vector3(cpParam.x, cpParam.y, cpParam.z), cpParam.w);
			break;
		case EventKey.PLAYER_SPAWN:
			SpawnPlayer();
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void SpawnPlayer()
	{
		EventDispatcher.SendEvent(EventKey.PLAYER_MOVE, spawnPoint); // Move player to start position
		EventDispatcher.SendEvent(EventKey.GAME_SETROTATION, spawnRotation); // Rotate to the level starting rotation
		EventDispatcher.SendEvent(EventKey.PLAYER_TOGGLEACTIVE, true); // Activate the player
	}
	
	private void Rotate(float rotationDelta)
	{
		float newRotation = rotation + rotationDelta * Time.deltaTime;
		
		EventDispatcher.SendEvent(EventKey.GAME_SETROTATION, newRotation);
	}
	
	private void SetRotation(float degrees)
	{
		rotation = degrees;
		
		float rad = rotation * Mathf.Deg2Rad;
		Physics.gravity = new Vector3 (Mathf.Sin (rad), 0f, Mathf.Cos (rad)) * -gravityForce;
	}
	
	private void SetCheckpoint(Vector3 pos, float rot)
	{
		spawnPoint = pos;
		spawnRotation = rot;
	}
	
}
