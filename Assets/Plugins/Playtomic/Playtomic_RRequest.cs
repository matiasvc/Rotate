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
using System.Text;

public class Playtomic_RRequest : Playtomic_Responder
{
	private static string URLStub;
	private static string URLTail;
	
	public Playtomic_RRequest ()
	{
	}
	
	internal static void Initialise()
	{
		URLStub = (Playtomic.UseSSL ? "https://g" : "http://g") + Playtomic.GameGuid + ".api.playtomic.com";
		URLTail = "swfid=" + Playtomic.GameId;
	}
	
	public static IEnumerator SendReferrer(string referrer)
	{
		var r = new System.Random();
		string url = URLStub + "/tracker/r.aspx?" + URLTail + "&" + r.Next(10000000) + "Z";
		
		WWWForm post = new WWWForm();
		
		post.AddField("referrer", WWW.EscapeURL(referrer));
		
		WWW www = new WWW(url, post);
		yield return www;
	}
}

