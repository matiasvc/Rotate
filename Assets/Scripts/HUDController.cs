using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
	public Transform rotationGear;
	
	private float rotationSpeed = 0.7f;
	
	void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.INPUT_ROTATE, HandleEvent);
	}
	
	void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.INPUT_ROTATE, HandleEvent);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		switch(eventName)
		{
		case EventKey.INPUT_ROTATE:
			SetGearRotation((float)param);
			break;
		}
	}
	
	private void SetGearRotation(float rot)
	{
		Vector3 oldRot = rotationGear.localEulerAngles;
		rotationGear.localEulerAngles = new Vector3(oldRot.x, oldRot.y - (rot * rotationSpeed * Time.deltaTime), oldRot.z);
	}
	
}
