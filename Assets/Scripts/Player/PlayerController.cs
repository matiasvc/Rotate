using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	private Rigidbody playerRigidbody;
	private MeshRenderer meshRenderer;
	private Light playerLigth;
	
	void Awake()
	{
		playerRigidbody = gameObject.GetComponent<Rigidbody>();
		meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
		playerLigth = gameObject.GetComponentInChildren<Light>();
	}
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.GAME_SETROTATION, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_DESTROY, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_TOGGLEACTIVE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_MOVE, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.GAME_SETROTATION, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_DESTROY, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_TOGGLEACTIVE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_MOVE, HandleEvent);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.GAME_SETROTATION:
			gameObject.rigidbody.WakeUp();
			break;
		case EventKey.PLAYER_DESTROY:
			Death();
			break;
		case EventKey.PLAYER_TOGGLEACTIVE:
			ToggleActive((bool)param);
			break;
		case EventKey.PLAYER_MOVE:
			Move((Vector3)param);
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void ToggleActive(bool toggle)
	{
		if (!playerRigidbody.isKinematic)
			playerRigidbody.velocity = Vector3.zero;
		
		playerRigidbody.isKinematic = !toggle;
		meshRenderer.enabled = toggle;
		playerLigth.enabled = toggle;
	}
	
	private void Move(Vector3 moveTo)
	{
		transform.position = moveTo;
	}
	
	private void Death()
	{
		ToggleActive(false);
		// Insert death animation here
		
		EventDispatcher.SendEvent(EventKey.PLAYER_SPAWN);
	}
}
