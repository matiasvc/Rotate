using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	
	private bool rigthButton = false;
	private bool leftButton = false;
	private bool deltaSwipeButton = false;
	private bool swipeButton = false;
	
	private float swipeDelta = 0f;
	
	private enum InputType {BUTTONS, DELTA_SWIPE, SWIPE};
	
	private InputType inputType = InputType.BUTTONS;
	
	void Start()
	{
		InputSwipe.RegisterOnSwipeListner(OnSwipe);
		InputSwipe.RegisterOnSwipeCompletedListner(OnSwipeComplete);
	}
	
	void Update()
	{
		float input = 0f; 
		
		input += Input.GetAxis("Horizontal") * 120f;
		
		if(rigthButton)
			input -= 120f;
		
		if(leftButton)
			input += 120f;
		
		input += swipeDelta;
		
		
		if(Mathf.Abs(input) > Mathf.Epsilon)
			EventDispatcher.SendEvent(EventKey.INPUT_ROTATE, input * Time.deltaTime);
	}
	
	void OnGUI()
	{
		Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		Vector2 buttonSize = new Vector2(300f, 100f);
		
		switch (inputType)
		{
		case InputType.BUTTONS:
			
			if(GUI.RepeatButton(new Rect(screenCenter.x - buttonSize.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "<----"))
				leftButton = true;
			else
				leftButton = false;
			
			if(GUI.RepeatButton(new Rect(screenCenter.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "---->"))
				rigthButton = true;
			else
				rigthButton = false;
			
			break;
		case InputType.DELTA_SWIPE:
			
			if (GUI.RepeatButton(new Rect(screenCenter.x - 300f, Screen.height - 100f, 600f, 100f), "<--SWIPE TO ROTATE (DELTA)-->"))
				deltaSwipeButton = true;
			else
				deltaSwipeButton = false;
			
			break;
		case InputType.SWIPE:
			
			if (GUI.RepeatButton(new Rect(screenCenter.x - 300f, Screen.height - 100f, 600f, 100f), "<--SWIPE TO ROTATE (POSITION)-->"))
				swipeButton = true;
			else
				swipeButton = false;
			
			break;
		}
		
		
		if(GUI.Button(new Rect(Screen.width - 50f, Screen.height - 50f, 50f, 50f), "O"))
		{
			switch (inputType)
			{
			case InputType.BUTTONS:
				inputType = InputType.DELTA_SWIPE;
				break;
			case InputType.DELTA_SWIPE:
				inputType = InputType.SWIPE;
				break;
			case InputType.SWIPE:
				inputType = InputType.BUTTONS;
				break;
			}
		}
	}
	
	private void OnSwipe(InputSwipe.Swipe swipe)
	{
		if (inputType == InputType.DELTA_SWIPE)
			swipeDelta = (swipe.frameDelta.x / Screen.width) * 6500f;
		else if (inputType == InputType.SWIPE)
			swipeDelta = (swipe.endPos.x / (Screen.width*0.5f) - 1f) * 160f;
	}
	
	private void OnSwipeComplete(InputSwipe.Swipe swipe)
	{
		swipeDelta = 0f;
	}
}
