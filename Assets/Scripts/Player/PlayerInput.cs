using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	private const OuyaSDK.OuyaPlayer PLAYER = OuyaSDK.OuyaPlayer.player1;
	
	private const float ROTATE_SPEED = 120.0f;
	
	void Update()
	{
		float input = 0f; 
		
		//input += Input.GetAxis("Horizontal") * 120f;
		input += OuyaInputManager.GetAxis( "LX", PLAYER ) * ROTATE_SPEED;
		
		if(Mathf.Abs(input) > Mathf.Epsilon)
			EventDispatcher.SendEvent(EventKey.INPUT_ROTATE, input * Time.deltaTime);
	}
	
}
