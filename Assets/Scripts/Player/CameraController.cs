using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public Transform player;
	
	private float cameraFollowRate = 4f;
	//private float cameraRotateAmt = 10f;
	
	void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, player.position, cameraFollowRate * Time.deltaTime);
	}
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.GAME_SETROTATION, HandleEvent);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch (eventName)
		{
		case EventKey.GAME_SETROTATION:
			SetRotation((float)param);
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void SetRotation(float rotation)
	{
		Vector3 oldRot = transform.localEulerAngles;
		transform.localEulerAngles = new Vector3(oldRot.x, rotation, oldRot.z);
	}
	
}
