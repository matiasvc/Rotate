/* Playtomic Unity3d API
-----------------------------------------------------------------------
 Documentation is available at: 
 	https://playtomic.com/api/unity

 Support is available at:
 	https://playtomic.com/community 
 	https://playtomic.com/issues
 	https://playtomic.com/support has more options if you're a premium user

 Github repositories:
 	https://github.com/playtomic

You may modify this SDK if you wish but be kind to our servers.  Be
careful about modifying the analytics stuff as it may give you 
borked reports.

Pull requests are welcome if you spot a bug or know a more efficient
way to implement something.

Copyright (c) 2011 Playtomic Inc.  Playtomic APIs and SDKs are licensed 
under the MIT license.  Certain portions may come from 3rd parties and 
carry their own licensing terms and are referenced where applicable.
*/ 

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Playtomic_LogRequest
{
	private static int Failed = 0;
	private static List<Playtomic_LogRequest> Pool = new List<Playtomic_LogRequest>();
	
	private string Data = "";
	public bool Ready = false;

	public static Playtomic_LogRequest Create()
	{
		Playtomic_LogRequest request = null;
		if (Pool.Count > 0)
		{
			request = Pool[0];
			Pool.RemoveAt(0);
		}
		else
		{
			request = new Playtomic_LogRequest();
		}
		
		request.Data = "";
		request.Ready = false;
		return request;
	}
	
	
	public void MassQueue(List<string> data)
	{
		if(Failed > 3)
			return;
		
		for(int i=data.Count-1; i>-1; i--)
		{
			Data += (Data == "" ? "" : "~") + data[i];
			data.RemoveAt(i);

			if(Data.Length > 300)
			{
				Playtomic_LogRequest request = Create();
				request.MassQueue(data);
				
				Ready = true;
				Send();
				return;
			}
		}
		
		Playtomic.Log.Request = this;
	}		

	public void Queue(string data)
	{
		//Debug.Log("Adding event " + data);
		if(Failed > 3)
			return;
		
		if(Data.Length > 0)
			Data += "~";
		
		Data += data;

		if(Data.Length > 300 || data.StartsWith("v/") || data.StartsWith("t/"))
		{
			//Debug.Log("Ready");
			Ready = true;
		}
	}

	public void Send()
	{
		//Debug.Log("Sending (logrequest)");
		Playtomic.API.StartCoroutine(Playtomic_Request.SendStatistics(Data));
		Pool.Add(this);
	}
}