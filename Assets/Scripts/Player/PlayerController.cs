using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	void Start()
	{
		LevelController.Instance.RespawnPoint = transform.position;
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.GAME_SETROTATION:
			gameObject.rigidbody.WakeUp();
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void Death()
	{
		Vector3 respawnPoint = LevelController.Instance.RespawnPoint;
		
		transform.position = respawnPoint;
		transform.rigidbody.velocity = Vector3.zero;
		
		EventDispatcher.SendEvent(EventKey.PLAYER_RESPAWN, respawnPoint);
	}
}
