using UnityEngine;
using System.Collections;

public class MetalGrateController : MonoBehaviour {
	
	public Transform fanMesh;
	public Transform fanSmoke;
	private float fanSpeed = 75.0f;
	
	void OnEnable() {
		EventDispatcher.AddHandler( EventKey.GAME_SET_ROTATION, HandleEvent );
	}
	
	void OnDisable() {
		EventDispatcher.RemoveHandler( EventKey.GAME_SET_ROTATION, HandleEvent );
	}
	
	private void HandleEvent( string eventName, object param ) {
		switch (eventName) {
		case EventKey.GAME_SET_ROTATION:
			float rotation = (float)param;
			fanSmoke.transform.localEulerAngles = new Vector3( 270.0f , rotation - 90.0f, 0.0f );
			break;
		}
	}
	
	void Update() {
		fanMesh.RotateAroundLocal( Vector3.up, -fanSpeed * Mathf.Deg2Rad * Time.deltaTime );
	}
	
}
