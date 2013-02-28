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
using System.Globalization;

public class Playtomic_PlayerLevel
{
	public string LevelId;
	public string PlayerSource = "";
	public string PlayerId = "";
	public string PlayerName = "";
	public string Name;
	public string Data;
	public int Votes;
	public int Plays;
	public double Rating;
	public int Score;
	public DateTime SDate;
	public string RDate;
	public Dictionary<string, string> CustomData = new Dictionary<string, string>();
	
	public Playtomic_PlayerLevel() 
	{ 
		SDate = new DateTime();
		RDate = "Just now";
	}
	
	public string Thumbnail
	{
		get { return "http:/api.playtomic.com/playerlevels/thumb.aspx?swfid=" + Playtomic.GameId + "&guid=" + Playtomic.GameGuid + "&levelid=" + LevelId; }
	}
	
	// for JS
	public void AddCustomData(String field, String data) 
	{
    	CustomData.Add(field, data);   
	}
	
	public Hashtable GetCustomDataAsHashtable() 
	{

    	var result = new Hashtable();

    	foreach (string key in CustomData.Keys) 
    	{
    	    result.Add(key, CustomData[key]);
   		}

    	return result;
	}
	
	public void SetCustomData(Hashtable customDataAsHashtable) 
	{	
	    foreach (var key in customDataAsHashtable.Keys)
    	{
        	CustomData.Add(key.ToString(), customDataAsHashtable[key].ToString());
	    }
	}
}
