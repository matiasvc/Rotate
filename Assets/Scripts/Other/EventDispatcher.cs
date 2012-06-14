using UnityEngine;
using System.Collections.Generic;


public static class EventDispatcher {
	
	struct EventHandlerPair
	{
		public string eventName;
		public EventHandler handler;
		
		public EventHandlerPair(string eventName, EventHandler handler)
		{
			this.eventName = eventName;
			this.handler = handler;
		}
	}
	
	public delegate void EventHandler(string eventName, object param);
	
	private static bool lockAddAndRemove = false;
	
	private static List<EventHandlerPair> removeHandlersDeferred = new List<EventHandlerPair>();
	private static EventHandlerPair currentHandler = new EventHandlerPair(null, null);
	
	private static Dictionary<string, List<EventHandler>> handlerMap = new Dictionary<string, List<EventHandler>>();
	
	public static void AddHandler(string eventName, EventHandler handler)
	{
		if (lockAddAndRemove)
		{
			Debug.LogError("Cannot add handler while dispatching messages");
			return;
		}
		
		List<EventHandler> handlerList;
		
		if (handlerMap.TryGetValue(eventName, out handlerList) == false)
		{
			handlerList = new List<EventHandler>();
			
			handlerMap.Add(eventName, handlerList);
			
			Debug.Log("Adding Event Handler: " + eventName);
		}
		
		handlerList.Add(handler);
	}
	
	public static void RemoveHandler(string eventName, EventHandler handler)
	{
		if (lockAddAndRemove)
		{
			removeHandlersDeferred.Add(new EventHandlerPair(eventName, handler));
			return;
		}
		
		List<EventHandler> handlerList;
		
		if (handlerMap.TryGetValue(eventName, out handlerList))
		{
			handlerList.Remove(handler);
			Debug.Log("Removing Event Handler: " + eventName);
		}
	}
	
	public static void RemoveCurrentHandler()
	{
		if (currentHandler.eventName == null || currentHandler.handler == null)
		{
			Debug.LogError("Currently not in a handler, use RemoveHandler instead!");
			return;
		}
		
		removeHandlersDeferred.Add(new EventHandlerPair(currentHandler.eventName, currentHandler.handler));
	}
	
	public static void SendEvent(string eventName)
	{
		SendEvent(eventName, null);
	}
	
	public static void SendEvent(string eventName, object param)
	{
		lockAddAndRemove = true;
		
		List<EventHandler> handlerList;
		
		if (handlerMap.TryGetValue(eventName, out handlerList))
		{
			foreach(EventHandler handler in handlerList)
			{
				currentHandler.eventName = eventName;
				currentHandler.handler = handler;
				
				handler(eventName, param);
				
				currentHandler.eventName = null;
				currentHandler.handler = null;
			}
			
			Debug.Log("Sent event: " + eventName + " to " + handlerList.Count.ToString() + " handlers");
		}
		else
		{
			Debug.LogWarning("No eventhandler exist for: " + eventName);
		}
		
		lockAddAndRemove = false;
		
		if (removeHandlersDeferred.Count > 0)
		{
			for (int i = 0; i < removeHandlersDeferred.Count; i++)
			{
				RemoveHandler(removeHandlersDeferred[i].eventName, removeHandlersDeferred[i].handler);
			}
			
			Debug.Log("Removed "  + removeHandlersDeferred.Count + " event handler(s) during event: " + eventName);
			
			removeHandlersDeferred.Clear();
		}
	}
}
