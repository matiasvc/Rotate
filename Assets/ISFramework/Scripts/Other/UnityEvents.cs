using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityEvents : MonoBehaviour {
	
	public delegate void UnityEventDelegate();
	
	private static UnityEvents instance;
	public static UnityEvents Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("UnityEvents");
				instance = go.AddComponent<UnityEvents>() as UnityEvents;
				DontDestroyOnLoad(go);
			}
			
			return instance;
		}
	}
	
	public enum UnityEvent {	Update = 				0,
								LateUpdate = 			1,
								FixedUpdate = 			2,
								OnLevelWasLoaded = 		3,
								OnRenderObject = 		4,
								OnGUI = 				5,
								OnDrawGizmos = 			6,
								OnApplicationPause = 	7,
								OnApplicationFocus = 	8,
								OnApplicationQuit = 	9
							};
	
	private List<UnityEventDelegate>[] events = new List<UnityEventDelegate>[]
	{
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>(),
		new List<UnityEventDelegate>()
	};
	
	public static void AddListner(UnityEvent eventType, UnityEventDelegate eventDelegate)
	{
		Instance.events[(int)eventType].Add(eventDelegate);
	}
	
	public static void RemoveListner(UnityEvent eventType, UnityEventDelegate eventDelegate)
	{
		Instance.events[(int)eventType].Remove(eventDelegate);
	}
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	
	void Update()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.Update])
		{
			eventDelegate();
		}
	}
	
	void LateUpdate()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.LateUpdate])
		{
			eventDelegate();
		}
	}
	
	void FixedUpdate()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.FixedUpdate])
		{
			eventDelegate();
		}
	}
	
	void OnLevelWasLoaded()
	{		
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnLevelWasLoaded])
		{
			eventDelegate();
		}
	}
	
	void OnRenderObject()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnRenderObject])
		{
			eventDelegate();
		}
	}
	
	void OnGUI()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnGUI])
		{
			eventDelegate();
		}
	}
	
	void OnDrawGizmos()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnDrawGizmos])
		{
			eventDelegate();
		}
	}
	
	void OnApplicationPause()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnApplicationPause])
		{
			eventDelegate();
		}
	}
	
	void OnApplicationFocus()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnApplicationFocus])
		{
			eventDelegate();
		}
	}
	
	void OnApplicationQuit()
	{
		foreach (UnityEventDelegate eventDelegate in events[(int)UnityEvent.OnApplicationQuit])
		{
			eventDelegate();
		}
	}
	
}
