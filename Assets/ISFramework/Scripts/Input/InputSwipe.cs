using UnityEngine;
using System.Collections.Generic;

public static class InputSwipe {
	
	private static bool initialized = false;
	
	const float MIN_SWIPE_LENGTH = 5;
	const int FREE_ID = -1;
	const float SWIPE_DEPTH = -1f;
	
	public delegate void OnSwipe(Swipe swipe);
	
	private static List<OnSwipe> onSwipeDelegates;
	private static List<OnSwipe> onSwipeCompleteDelegates;
	
	public class Swipe
	{
		public Swipe()
		{
			positions = new List<Vector2>();
		}
		
		public int id;
		
		public List<Vector2> positions;
		
		public float startTime;
		public float endTime;
		
		public Vector2 startPos
		{
			get { return this.positions[0]; }
		}
		
		public Vector2 endPos
		{
			get { return this.positions[positions.Count-1]; }
		}
		
		public Vector2 delta
		{
			get { return this.positions[positions.Count-1] - this.positions[0]; }
		}
		
		public Vector2 frameDelta
		{
			get { return this.positions[positions.Count-1] - this.positions[positions.Count-2]; }
		}
		
		public float length
		{
			get { return this.delta.magnitude; }
		}
		
		public float duration
		{
			get { return this.endTime - this.startTime; }
		}
		
		public float averageSpeed
		{
			get
			{
				float swipeLength = this.length;
				
				if(Mathf.Approximately(swipeLength, 0f))
					return 0f;
				else
					return swipeLength / duration;
			}
		}
		
		public bool used;
		public bool accepted;
	}
	
	private static Swipe[] swipes;
	
	public static void RegisterOnSwipeListner(OnSwipe onswipe)
	{
		if (!initialized)
			Initialize();
		
		if (onSwipeDelegates.Contains(onswipe))
			Debug.LogWarning("This delegat is already a listner.");
		else
			onSwipeDelegates.Add(onswipe);
	}
	
	public static void DeregisterOnSwipeListner(OnSwipe onswipe)
	{
		if (onSwipeDelegates.Contains(onswipe))
		{
			onSwipeDelegates.Remove(onswipe);
			
			if(onSwipeDelegates.Count == 0 && onSwipeCompleteDelegates.Count == 0)
				Deinitialize();
		}
		else
			Debug.LogWarning("Unable to remove delegate because it isnt in the list.");
	}
	
	public static void RegisterOnSwipeCompletedListner(OnSwipe onswipe)
	{
		if (!initialized)
			Initialize();
		
		if (onSwipeCompleteDelegates.Contains(onswipe))
			Debug.LogWarning("This delegat is already a listner.");
		else
			onSwipeCompleteDelegates.Add(onswipe);
	}
	
	public static void DeregisterOnSwipeCompletedListner(OnSwipe onswipe)
	{
		if (onSwipeCompleteDelegates.Contains(onswipe))
		{
			onSwipeCompleteDelegates.Remove(onswipe);
			
			if(onSwipeDelegates.Count == 0 && onSwipeCompleteDelegates.Count == 0)
				Deinitialize();
		}
		else
			Debug.LogWarning("Unable to remove delegate because it isnt in the list.");
	}
	
	private static void Initialize()
	{
		onSwipeDelegates = new List<OnSwipe>();
		onSwipeCompleteDelegates = new List<OnSwipe>();
		
		swipes = new Swipe[10];
		
		for (int i = 0; i < swipes.Length; i++)
		{
			swipes[i] = new InputSwipe.Swipe();
			swipes[i].id = FREE_ID;
		}
		
		InputManager.Register(HandleInput, SWIPE_DEPTH);
		
		initialized = true;
	}
	
	private static void Deinitialize()
	{
		onSwipeDelegates = null;
		onSwipeCompleteDelegates =  null;
		
		swipes = null;
		
		InputManager.Deregister(HandleInput);
		
		initialized = false;
	}
	
	private static bool HandleInput(InputEvent inputEvent, Vector2 pos)
	{
		bool eatInput = false;
		
		if (inputEvent == InputEvent.Down)
		{
			Swipe swipe = CreateSwipe(InputManager.CurrentEvent.index);
			
			if (swipe != null)
			{
				swipe.positions.Add(pos);
				swipe.startTime = Time.time;
				swipe.accepted = false;
				
			}
		}
		else if (inputEvent == InputEvent.Hold)
		{
			Swipe swipe = FindSwipe(InputManager.CurrentEvent.index);
			
			if (swipe != null)
			{
				swipe.positions.Add(pos);
				swipe.endTime = Time.time;
				
				if (swipe.length > MIN_SWIPE_LENGTH)
				{
					swipe.accepted = true;
				}
				
				eatInput = swipe.accepted;
				
				if (swipe.accepted
				    && swipe.used == false)
				{
					foreach (OnSwipe onSwipe in onSwipeDelegates)
						onSwipe(swipe);
				}
			}
		}
		else if (inputEvent == InputEvent.Up)
		{
			Swipe swipe = FindSwipe(InputManager.CurrentEvent.index);
			
			if (swipe != null)
			{
				swipe.positions.Add(pos);
				swipe.endTime = Time.time;
				
				if (swipe.length > MIN_SWIPE_LENGTH)
				{
					swipe.accepted = true;
				}
				
				eatInput = false;
				
				if (swipe.accepted
				    && swipe.used == false)
				{
					foreach (OnSwipe onSwipe in onSwipeCompleteDelegates)
						onSwipe(swipe);
				}
				
				FreeSwipe(swipe.id);
			}
		
		}
		
		return eatInput;
	}
	
	private static Swipe FindSwipe(int id)
	{
		for (int i = 0; i < swipes.Length; i++)
		{
			if (swipes[i].id == id)
			{
				return swipes[i];
			}
		}
		
		return null;
	}
	
	private static Swipe CreateSwipe(int id)
	{
		for (int i = 0; i < swipes.Length; i++)
		{
			if (swipes[i].id == FREE_ID)
			{
				swipes[i].id = id;
				swipes[i].used = false;
				return swipes[i];
			}
		}
		
		return null;
	}
	
	private static void FreeSwipe(int id)
	{
		for (int i = 0; i < swipes.Length; i++)
		{
			if (swipes[i].id == id)
			{
				swipes[i].id = FREE_ID;
			}
		}
		
	}
}
