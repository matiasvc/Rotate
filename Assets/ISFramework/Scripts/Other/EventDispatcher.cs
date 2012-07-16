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
	private static Dictionary<string, List<object>> queuedEvents = new Dictionary<string, List<object>>();
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
			
		}
		
		handlerList.Add(handler);
		Debug.Log("Adding Event Handler: " + eventName);
		
		List<object> paramList;
		
		if (queuedEvents.TryGetValue(eventName, out paramList))
		{
			foreach(object param in paramList)
			{
				handler(eventName, param);
				Debug.Log("Added handler had a event queued. Calling: " + eventName);
			}
			
			queuedEvents.Remove(eventName);
		}
	}
	
	public static void RemoveHandler(string eventName, EventHandler handler)
	{
		if (lockAddAndRemove)
		{
			removeHandlersDeferred.Add(new EventHandlerPair(eventName, handler));
			return;
		}
		
		List<EventHandler> handlerList = null;
		
		if (handlerMap.TryGetValue(eventName, out handlerList))
		{
			handlerList.Remove(handler);
		}
		
		Debug.Log("Removing Event Handler: " + eventName); // TODO Call a different log if nothing were removed
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
	
	public static void SendEvent(string eventName, object param = null, bool queued = false)
	{
		lockAddAndRemove = true;
		
		List<EventHandler> handlerList = null;
		
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
			
//			if(handlerList.Count > 0)
//				Debug.Log("Sent event: " + eventName + " to " + handlerList.Count + " listners.");
		}
		
		if (handlerList == null || handlerList.Count == 0)
		{
			if(queued)
			{
				List<object> paramList;
				
				if (queuedEvents.TryGetValue(eventName, out paramList) == false)
				{
					paramList = new List<object>();
					queuedEvents.Add(eventName, paramList);
				}
				
				paramList.Add(param);
				
				Debug.LogWarning("No eventhandler exist for: " + eventName + " adding it to queue");
			}
			else
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
	
	public static void ClearQueue()
	{
		if (lockAddAndRemove)
		{
			Debug.LogError("Cannot clear queue while dispatching messages");
			return;
		}
		
		queuedEvents.Clear();
	}
}
