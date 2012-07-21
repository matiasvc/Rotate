using UnityEngine;
using System.Collections.Generic;

public enum InputEvent
{
	Down,
	Hold,
	Up,
	None
}

public static class InputManager 
{
	private static bool initialized = false;
	
	public static bool allowInput = true;
	
	private static bool didProcessInput = false;
	
	public delegate bool InputHandlerDelegate(InputEvent inputEvent, Vector2 pos);
	
	[System.Serializable]
	private class InputHandler
	{
		public InputHandlerDelegate handler;
		public float depth;
		
		public InputHandler(InputHandlerDelegate handler, float depth)
		{
			this.handler = handler;
			this.depth = depth;
		}
	}
	
	private static List<InputHandler> buttons = new List<InputHandler>();
	
	public struct InputEventData
	{
		public InputEvent type;
		public Vector2 pos;
		public int index;
	}
	
	private static InputEventData[] inputQueue = new InputEventData[10];
	
	private static InputEventData currentInput;
	
	private static int frame = 0;
	
	public const int INPUTINDEX_MOUSE = 100;
	
	public static int Frame
	{
		get
		{
			return frame;
		}
	}
	
	public static void Register(InputHandlerDelegate handler, float depth)
	{		
		if (!initialized)
		{
			UnityEvents.AddListner(UnityEvents.UnityEvent.Update, Update);
			initialized = true;
		}
		
		buttons.Add(new InputHandler(handler, depth));
		
		buttons.Sort((a, b) => a.depth.CompareTo(b.depth));
	}
	
	public static void Deregister(InputHandlerDelegate handler)
	{
		int index = buttons.FindIndex(o => o.handler == handler);
		
		if (index >= 0)
		{
			buttons.RemoveAt(index);
		}
		
		if (buttons.Count == 0)
		{
			UnityEvents.RemoveListner(UnityEvents.UnityEvent.Update, Update);
			initialized = false;
		}
	}
	
	public static void Update()
	{
		
		if (!allowInput)
		{
			return;
		}
		
		frame++;
		
		//Fill input queue
		int currentInputNum = 0;
		
#if UNITY_IPHONE && !UNITY_EDITOR
		if (Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				
				if (touch.phase == TouchPhase.Began)
				{
					if (currentInputNum < inputQueue.Length)
					{
						inputQueue[currentInputNum].type = InputEvent.Down;
						inputQueue[currentInputNum].pos = touch.position;
						inputQueue[currentInputNum].index = touch.fingerId;
						
						currentInputNum++;
					}
				}
				else if (touch.phase != TouchPhase.Ended 
					&& touch.phase != TouchPhase.Canceled)
				{
					if (currentInputNum < inputQueue.Length)
					{
						inputQueue[currentInputNum].type = InputEvent.Hold;
						inputQueue[currentInputNum].pos = touch.position;
						inputQueue[currentInputNum].index = touch.fingerId;
						
						currentInputNum++;
					}
				}
				else if (touch.phase == TouchPhase.Ended 
					|| touch.phase == TouchPhase.Canceled)
				{
					if (currentInputNum < inputQueue.Length)
					{
						inputQueue[currentInputNum].type = InputEvent.Up;
						inputQueue[currentInputNum].pos = touch.position;
						inputQueue[currentInputNum].index = touch.fingerId;
						
						currentInputNum++;
					}
				}
			}
		}
#else		
		if (Input.GetMouseButtonDown(0))
		{
			if (currentInputNum < inputQueue.Length)
			{
				inputQueue[currentInputNum].type = InputEvent.Down;
				inputQueue[currentInputNum].pos = Input.mousePosition;
				inputQueue[currentInputNum].index = INPUTINDEX_MOUSE;
				
				currentInputNum++;
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (currentInputNum < inputQueue.Length)
			{
				inputQueue[currentInputNum].type = InputEvent.Hold;
				inputQueue[currentInputNum].pos = Input.mousePosition;
				inputQueue[currentInputNum].index = INPUTINDEX_MOUSE;
				
				currentInputNum++;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (currentInputNum < inputQueue.Length)
			{
				inputQueue[currentInputNum].type = InputEvent.Up;
				inputQueue[currentInputNum].pos = Input.mousePosition;
				inputQueue[currentInputNum].index = INPUTINDEX_MOUSE;
				
				currentInputNum++;
			}
		}
#endif		
		didProcessInput = false;
		
		//Process input queue
		for (int e = 0; e < currentInputNum; e++)
		{
			//Debug.Log("Handling event: " + inputQueue[e].type + " " + Time.time);
			
			for (int i = 0; i < buttons.Count; i++)
			{
				currentInput = inputQueue[e];
				didProcessInput = buttons[i].handler(inputQueue[e].type, inputQueue[e].pos);
				
				currentInput.index = -1;
				currentInput.pos = Vector2.zero;
				currentInput.type = InputEvent.None;
				
				if (didProcessInput)
				{
					break;
				}
			}
		}
	}
	
	public static InputEventData CurrentEvent
	{
		get
		{
			return currentInput;
		}
	}
	
	public static bool DidProcessInput
	{
		get
		{
			return didProcessInput;
		}
	}
}
