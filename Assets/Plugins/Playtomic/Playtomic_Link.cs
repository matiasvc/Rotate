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
using System.Collections;
using System.Collections.Generic;

public class Playtomic_Link
{
	private List<string> Clicks = new List<string>();

	public bool Open(string url, string name, string group)
	{
		int unique = 0;
		int bunique = 0;
		int total = 0;
		int btotal = 0;
		int fail = 0;
		int bfail = 0;
		string key = url + "." + name;
		bool result;

		string baseurl = url;
		baseurl = baseurl.Replace("http://", "");
		
		if(baseurl.IndexOf("/") > -1)
			baseurl = baseurl.Substring(0, baseurl.IndexOf("/"));
			
		if(baseurl.IndexOf("?") > -1)
			baseurl = baseurl.Substring(0, baseurl.IndexOf("?"));				
			
		baseurl = "http://" + baseurl + "/";

		string baseurlname = baseurl;
		
		if(baseurlname.IndexOf("//") > -1)
			baseurlname = baseurlname.Substring(baseurlname.IndexOf("//") + 2);
		
		baseurlname = baseurlname.Replace("www.", "");

		if(baseurlname.IndexOf("/") > -1)
		{
			baseurlname = baseurlname.Substring(0, baseurlname.IndexOf("/"));
		}

		Application.OpenURL(url);

		if(Clicks.IndexOf(key) > -1)
		{
			total = 1;
		}
		else
		{
			total = 1;
			unique = 1;
			Clicks.Add(key);
		}

		if(Clicks.IndexOf(baseurlname) > -1)
		{
			btotal = 1;
		}
		else
		{
			btotal = 1;
			bunique = 1;
			Clicks.Add(baseurlname);
		}

		result = true;
		
		// if it failed, you would:
		// {
		//	fail = 1;
		//	bfail = 1;
		//	result = false;
		// }
		// but there's no way to detect failure in opening the URL right now (and failure may not be possible, there's no setting akin to Flash's popup-blocking setting)
					
		Playtomic.Log.Link(baseurl, baseurlname.ToLower(), "DomainTotals", bunique, btotal, bfail);
		Playtomic.Log.Link(url, name, group, unique, total, fail);
		Playtomic.Log.ForceSend();

		return result;
	}
}