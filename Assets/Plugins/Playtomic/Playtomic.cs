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

public class Playtomic : MonoBehaviour
{
	private long _gameid;
	private string _gameguid;
	private string _sourceUrl;
	private bool _useSSL;
	private Playtomic_Log _log;
	private Playtomic_Data _data;
	private Playtomic_Leaderboards _leaderboards;
	private Playtomic_PlayerLevels _playerlevels;
	private Playtomic_GeoIP _geoip;
	private Playtomic_Link _link;
	private Playtomic_GameVars _gamevars;
	private Playtomic_Parse _parse;
	
	private static Playtomic _instance = null;
	
	/// <summary>
	/// Initializes the API.  You must do this before anything else.  Get your credentials from the Playtomic dashboard.
	/// </summary>
	/// <param name="gameid">
	/// A <see cref="System.Int64"/>
	/// </param>
	/// <param name="gameguid">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="apikey">
	/// A <see cref="System.String"/>
	/// </param>
	public static void Initialize(long gameid, string gameguid, string apikey)
	{
		if(_instance != null)
			return;
			
		var go = new GameObject("playtomic");
		GameObject.DontDestroyOnLoad(go);
			
		_instance = go.AddComponent("Playtomic") as Playtomic;
		_instance._gameid = gameid;
		_instance._gameguid = gameguid;
		_instance._sourceUrl = string.IsNullOrEmpty(Application.absoluteURL) ? "http://localhost/" : Application.absoluteURL;
		_instance._log = new Playtomic_Log();
		_instance._data = new Playtomic_Data();
		_instance._leaderboards = new Playtomic_Leaderboards();
		_instance._playerlevels = new Playtomic_PlayerLevels();
		_instance._geoip = new Playtomic_GeoIP();
		_instance._link = new Playtomic_Link();
		_instance._gamevars = new Playtomic_GameVars();
		_instance._parse = new Playtomic_Parse();
		
		Playtomic_Request.Initialise();
		Playtomic_Data.Initialise(apikey);
		Playtomic_GameVars.Initialise(apikey);
		Playtomic_Leaderboards.Initialise(apikey);
		Playtomic_GeoIP.Initialise(apikey);
		Playtomic_PlayerLevels.Initialise(apikey);
		Playtomic_Parse.Initialise(apikey);
		Playtomic_RRequest.Initialise();
		
		//Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
	}
	
	public static void SetSSL()
	{
		if(_instance == null)
		{
			Debug.Log("Initialize the API before you SetSSL");
			return;
		}
		
		_instance._useSSL = true;
		Playtomic_Request.Initialise();
		Debug.Log("You are now using SSL for your api requests.  This feature is for premium users only, if your account is not premium the data you send will be ignored.");
	}
	
	public static void LogException( object sender, UnhandledExceptionEventArgs e)
	{
    	var exception = e.ExceptionObject.ToString();
    	Debug.Log("**** EXCEPTION ****");
    	Debug.Log(exception);
	}
	
	/*private static void Application_ThreadException( object sender, System.Threading.ThreadExceptionEventArgs e )
	{
		var exception = e.Exception.ToString();
		Debug.Log(exception);
	}*/
	
	public static bool UseSSL
	{
		get { return _instance._useSSL; }
	}
	
	public static long GameId
	{
		get { return _instance._gameid; }
	}
	
	public static string GameGuid
	{
		get { return _instance._gameguid; }
	}
	
	public static string SourceUrl
	{
		get { return _instance._sourceUrl; }
	}
	
	public static Playtomic API
	{
		get { return _instance; }
	}
	
	public static Playtomic_Log Log
	{
		get { return _instance._log; }
	}
	
	public static Playtomic_Data Data
	{
		get { return _instance._data; }
	}
	
	public static Playtomic_Leaderboards Leaderboards
	{
		get  { return _instance._leaderboards; }
	}
	
	public static Playtomic_PlayerLevels PlayerLevels
	{
		get { return _instance._playerlevels; }
	}
	
	public static Playtomic_GeoIP GeoIP
	{
		get { return _instance._geoip; }
	}
	
	public static Playtomic_Link Link
	{
		get { return _instance._link; }
	}
	
	public static Playtomic_GameVars GameVars
	{
		get { return _instance._gamevars; }
	}
	
	public static Playtomic_Parse Parse
	{
		get { return _instance._parse; }
	}
}