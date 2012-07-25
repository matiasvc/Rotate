using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public Transform player;
	public Transform camTilt;
	public Transform camRot;
	
	private float cameraFollowRate = 3f;
	private float cameraTiltRate = 2f;
	
	private Vector3 playerVector = Vector3.zero;
	
	void LateUpdate()
	{
		playerVector = player.position - camTilt.position;
		
		transform.position = Vector3.Lerp(transform.position, player.position, cameraFollowRate * Time.deltaTime);;
		
		Quaternion goalRot = Quaternion.FromToRotation(Vector3.down, playerVector);
		camTilt.rotation = Quaternion.Lerp(camTilt.rotation, goalRot, cameraTiltRate * Time.deltaTime);
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
		camRot.localEulerAngles = new Vector3(oldRot.x, rotation, oldRot.z);
	}
	
}
