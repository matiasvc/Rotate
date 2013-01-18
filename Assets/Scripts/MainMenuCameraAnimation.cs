using UnityEngine;
using System.Collections;

public class MainMenuCameraAnimation : MonoBehaviour {
	
	public float xSpeed = 1f;
	public float xMagnitude = 1f;
	
	public float ySpeed = 1f;
	public float yMagnitude = 1f;
	
	public float zSpeed = 1f;
	public float zMagnitude = 1f;
	
	private Vector3 startRot;
	
	void Start () {
		startRot = transform.localEulerAngles;
	}
		
	
	void Update () {
		
		transform.localEulerAngles = new Vector3(	Mathf.Sin(Time.timeSinceLevelLoad * xSpeed) * xMagnitude,
													Mathf.Sin(Time.timeSinceLevelLoad * ySpeed) * yMagnitude,
													Mathf.Sin(Time.timeSinceLevelLoad * zSpeed) * zMagnitude) + startRot;
	
	}
}
