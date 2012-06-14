using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	private KeyCode pause = KeyCode.Escape;
	
	protected void Update()
	{
		if(Input.GetKeyDown(pause))
		{
			EventDispatcher.SendEvent("INPUT_PAUSE");
		}
	}
}
