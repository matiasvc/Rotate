using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	
	public static LevelController Instance;
	
	private Vector2 respawnPoint;
	public Vector3 RespawnPoint
	{
		get
		{
			return new Vector3(respawnPoint.x, 0f, respawnPoint.y);
		}
		set
		{
			respawnPoint = new Vector2(value.x, value.z);
		}
	}
	
	private float startRotation = 0f;
	
	private float rotation;
	private float rotationSpeed = 120f;
	private float gravityForce = 9.8f;
	
	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		SetRotation(startRotation);
	}
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_RESPAWN, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.INPUT_ROTATE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_RESPAWN, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.INPUT_ROTATE:
			Rotate((float)param);
			break;
		case EventKey.PLAYER_RESPAWN:
			EventDispatcher.SendEvent(EventKey.GAME_SETROTATION, startRotation);
			break;
		case EventKey.GAME_SETROTATION:
			SetRotation((float)param);
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void Rotate(float rotationDelta)
	{
		float newRotation = rotation + rotationDelta * rotationSpeed * Time.deltaTime;
		
		EventDispatcher.SendEvent(EventKey.GAME_SETROTATION, newRotation);
	}
	
	private void SetRotation(float degrees)
	{
		rotation = degrees;
		
		float rad = rotation * Mathf.Deg2Rad;
		Physics.gravity = new Vector3 (Mathf.Sin (rad), 0f, Mathf.Cos (rad)) * -gravityForce;
	}
	
}
