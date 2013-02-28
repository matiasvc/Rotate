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
using System.Security.Cryptography;
using System.Text;

public class Playtomic_GeoIP : Playtomic_Responder
{
	public Playtomic_GeoIP() { }
	
	private static string SECTION;
	private static string LOOKUP;
	
	internal static void Initialise(string apikey)
	{
		SECTION = Playtomic_Encode.MD5("geoip-" + apikey);
		LOOKUP = Playtomic_Encode.MD5("geoip-lookup-" + apikey);
	}
	
	public IEnumerator Lookup()
	{
		string url;
		WWWForm post;
		
		Playtomic_Request.Prepare(SECTION, LOOKUP, null, out url, out post);
		
		WWW www = new WWW(url, post);
		yield return www;
		
		var response = Playtomic_Request.Process(www);
	
		if (response.Success)
		{
			var data = (Hashtable)response.JSON;

			foreach(string key in data.Keys)
			{
				var name = WWW.UnEscapeURL(key);
				var value = WWW.UnEscapeURL((string)data[key]);
				response.Data.Add(name, value);
			}
		}
		
		SetResponse(response, "Lookup");
	}
}