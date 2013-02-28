/* Playtomic Unity3d API
-----------------------------------------------------------------------
 Documentation is available at: 
 	https://playtomic.com/api/unity

 Support is available at:
 	https://playtomic.com/community 
 	https://playtomic.com/issues
 	https://playtomic.com/support has more options if you"re a premium user

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Playtomic_Request
{
	public static Dictionary<String, Playtomic_Response> Requests = new Dictionary<String, Playtomic_Response>();
	private static String URLStub;
	private static String URLTail;
	private static String URL;
	
	public static void Initialise()
	{
		URLStub = (Playtomic.UseSSL ? "https://g" : "http://g") + Playtomic.GameGuid + ".api.playtomic.com/";
		URLTail = "swfid=" + Playtomic.GameId + "&js=y";
		URL = URLStub + "v3/api.aspx?" + URLTail;
		
        //URLStub = "http://127.0.0.1:3000/";
        //URLTail = "swfid=" + Playtomic.GameId + "&guid=" + Playtomic.GameGuid + "&js=y&debug=yes";
		//URL = URLStub + "v3/api.aspx?" + URLTail;
	}
	
	public static IEnumerator SendStatistics(string data)
	{
		//Debug.Log("Request created");
		WWWForm post = new WWWForm();
		post.AddField("x", "x");

		var r = new System.Random();
		var turl = URLStub + "tracker/q.aspx?q=" + data + "&swfid=" + Playtomic.GameId + "&url=" + Escape(Playtomic.SourceUrl) + "&" + r.Next(1000000) + "Z";
				
		var www = new WWW(turl, post);
		yield return www;
	}
	
	public static void Prepare(string section, string action, Dictionary<String, String> postdata, out string url, out WWWForm post)
	{
		Debug.Log (section + "\n" + action);
		var r = new System.Random();
		url = URL + "&r=" + r.Next(10000000) + "Z";
		var timestamp = (DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).TotalSeconds.ToString();
		var nonce = Playtomic_Encode.MD5(timestamp + Playtomic.SourceUrl + Playtomic.GameGuid);
	
		var pd = new ArrayList();
		pd.Add("nonce=" + nonce);
		pd.Add("timestamp=" + timestamp);
		
		if(postdata != null)
			foreach(string key in postdata.Keys)
				pd.Add(key + "=" + Escape(postdata[key]));

		GenerateKey("section", section, ref pd);
		GenerateKey("action", action, ref pd);
		GenerateKey("signature", nonce + timestamp + section + action + url + Playtomic.GameGuid, ref pd);
		
		var joined = "";
		pd.Sort ();
		
		foreach(var item in pd)
			joined += (joined == "" ? "" : "&") + item;
	
		Debug.Log(joined);
		
		post = new WWWForm();
		post.AddField("data", Escape(Playtomic_Encode.Base64(joined)));
	}
	
	public static Playtomic_Response Process(WWW www)
	{
		if(www == null)
			return Playtomic_Response.GeneralError(1);
		
		if (www.error != null)
			return Playtomic_Response.GeneralError(www.error);

		if (string.IsNullOrEmpty(www.text))
			return Playtomic_Response.Error(1);
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		
		if(!results.ContainsKey("Status") || !results.ContainsKey("ErrorCode"))
			return Playtomic_Response.GeneralError(1);
		
		var response = new Playtomic_Response();
		response.Success = ((int)(double)results["Status"] == 1);
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if(response.Success && results.ContainsKey("Data"))
		{
			if(results["Data"] is Hashtable)
				response.JSON = (Hashtable)results["Data"];
			
			if(results["Data"] is ArrayList)
				response.ARRAY = (ArrayList)results["Data"];
		}
		
		return response;
	}
	
	private static void GenerateKey(string name, string key, ref ArrayList arr)
	{
		var strarray = (string[]) arr.ToArray(typeof(string));
		Array.Sort(strarray);
		
		var joined = "";
		
		foreach(var item in strarray)
			joined += (joined == "" ? "" : "&") + item;
		
		arr.Add(name + "=" + Playtomic_Encode.MD5(joined + key));
	}
	
	private static string Escape(string str)
	{
		str = str.Replace("%25", "%");
		str = str.Replace("%3B", ";");
		str = str.Replace("%3F", "?");
		str = str.Replace("%2F", "/");
		str = str.Replace("%3A", ":");
		str = str.Replace("%23", "#");
		str = str.Replace("%26", "&");
		str = str.Replace("%3D", "=");
		str = str.Replace("%2B", "+");
		str = str.Replace("%24", "$");
		str = str.Replace("%2C", ",");
		str = str.Replace("%20", " ");
		str = str.Replace("%3C", "<");
		str = str.Replace("%3E", ">");
		str = str.Replace("%7E", "~");
		return str;
	}
}