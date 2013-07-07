using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	private const OuyaSDK.OuyaPlayer PLAYER = OuyaSDK.OuyaPlayer.player1;
	
	private const float ROTATE_SPEED = 120.0f;
	
	void Update()
	{
		float rotationInput = 0f; 
		
		//input += Input.GetAxis("Horizontal") * 120f;
		rotationInput += OuyaInput.GetAxis( OuyaAxis.LX,OuyaPlayer.P01 );
		
		if ( Input.GetKey(KeyCode.A) ) { // Keyboard input for testing.
			rotationInput += 1.0f;
		}
		if ( Input.GetKey( KeyCode.D ) ) {
			rotationInput -= 1.0f;
		}
		
		rotationInput *= ROTATE_SPEED;
		
		if(Mathf.Abs(rotationInput) > Mathf.Epsilon) {
			EventDispatcher.SendEvent(EventKey.INPUT_ROTATE, rotationInput * Time.deltaTime);
		}
	}
	
}
