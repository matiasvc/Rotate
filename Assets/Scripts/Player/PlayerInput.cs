using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	private bool rigthButton = false;
	private bool leftButton = false;
	
	void Update()
	{
		float input = 0f; 
		
#if UNITY_EDITOR
		input = Input.GetAxis("Horizontal");
#endif
		
		if(rigthButton)
			input -= 1f;
		
		if(leftButton)
			input += 1f;
		
		if(Mathf.Abs(input) > Mathf.Epsilon)
			EventDispatcher.SendEvent(EventKey.INPUT_ROTATE, input);
	}
	
	void OnGUI()
	{
		Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		Vector2 buttonSize = new Vector2(300f, 100f);
		
		if(GUI.RepeatButton(new Rect(screenCenter.x - buttonSize.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "<----"))
		{
			leftButton = true;
		}
		else
		{
			leftButton = false;
		}
		
		if(GUI.RepeatButton(new Rect(screenCenter.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "---->"))
		{
			rigthButton = true;
		}
		else
		{
			rigthButton = false;
		}
	}
	
}
