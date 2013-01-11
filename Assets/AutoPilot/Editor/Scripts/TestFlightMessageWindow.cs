using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class TestFlightMessageWindow : TestFlightWindow
{
	protected Texture logoTex = null;
	protected TestFlightPreferences preferences;
	protected TestFlightMobileProvision[] allProvisions = new TestFlightMobileProvision[0];
	protected TestFlightXCodeSchemas allSchemas = null;
	protected string[] allIdentities = new string[0];

	string message = "\n\n\n\n-- Built & Uploaded using AutoPilot";
	bool startBuild = false;
	bool closeWindow = false;
	int selectedMessage = 0;
	bool showUserLists = true;
	List<string> messageOptions = new List<string>();
	string newBundleVersion;
	
	public void OnEnable()
	{
		logoTex = TestFlightResources.GetTextureResource("logo_small.png");
		title = "Autopilot Build Options";
		preferences = TestFlightPreferences.Load();
		
		messageOptions = new List<string>();
		messageOptions.Add("New message...");
		messageOptions.AddRange(preferences.userPrefs.messageHistory);
		messageOptions.RemoveAll(m => m.Length == 0);
		messageOptions.Insert(1, "");		
		newBundleVersion = PlayerSettings.bundleVersion; 
		
		allProvisions = TestFlightMobileProvision.EnumerateProvisions();
		allSchemas = TestFlightXCodeSchemas.Enumerate(preferences.teamPrefs.buildPath);
		allIdentities = TestFlightDeveloperIdentities.Enumerate();
	}
	
	public static void CreateWindow()
	{
		TestFlightMessageWindow window = GetWindow<TestFlightMessageWindow>(true);
		window.position = new Rect(10, 10, 900, 400);
		window.minSize  = new Vector2(600, 440); 
	}	
	
	public virtual void OnGUI()
	{
		InitSkin();
				
		if(logoTex)
			GUI.DrawTexture(new Rect(Screen.width-logoTex.width-10, Screen.height-logoTex.height-20, logoTex.width, logoTex.height), logoTex);		
		
		OnGUI_MessageArea(440);
		OnGUI_BuildOptions(440);
		OnGUI_Buttons();

	}

	protected void OnGUI_DrawBundleVersion ()
	{
		GUILayout.Label("Bundle Version:");
		
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
		GUILayout.Space(5);
		GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Previous Version", GUILayout.Width(125));
		GUI.color = Color.grey;
		GUILayout.TextField(PlayerSettings.bundleVersion);
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Build Version", GUILayout.Width(125));
		newBundleVersion = GUILayout.TextField(newBundleVersion);
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	protected void OnGUI_BuildOptions(float width=360, bool useThinStyle=true, float startHeight=0)
	{		
		Rect r = new Rect(Screen.width-width, 10+startHeight, width-10, 60);
		//GUI.Box(new Rect(r.xMin, r.yMin+20, r.width+5, r.height-20), " ");

		GUILayout.BeginArea(r);	
		OnGUI_DrawBundleVersion ();
		GUILayout.EndArea();	
		
		r = new Rect(Screen.width-width, 90+startHeight, width-10, Screen.height-110);
		//GUI.Box(new Rect(r.xMin, r.yMin+20, r.width+5, r.height-20), " ");
		GUILayout.BeginArea(r);			
		GUILayout.Label("Build Options:");
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.Space(5);
		GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		TestFlightPreferencesWindow.DrawSubUserSettings(preferences, ref showUserLists, useThinStyle, allProvisions, allSchemas, allIdentities);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	protected void OnGUI_Buttons(string buildText = "Build & Upload")
	{
		GUILayout.BeginArea(new Rect(Screen.width-190, Screen.height-20, 180, 20));
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Cancel"))
		{
			closeWindow = true;
		}
		
		if(!TestFlightBuildPipeline.CanBuild(preferences, allProvisions, allIdentities))
		{
			GUI.color = Color.grey;
			GUILayout.Box(buildText, "button");
			GUI.color = Color.white;
		}
		else if(GUILayout.Button(buildText))
		{
			startBuild = true;
			
			PlayerSettings.bundleVersion = newBundleVersion;
			
			if(selectedMessage == 0)
			{
				messageOptions[0] = message;
				messageOptions.RemoveAll(m => m.Length == 0);	
			}
			
			// clean out any duplicates before saving
			List<string> newMessageOptions = new List<string>();
			foreach(string s in messageOptions)
			{
				if(!newMessageOptions.Contains(s))
					newMessageOptions.Add(s);
			}
			preferences.userPrefs.messageHistory = newMessageOptions.ToArray();

			preferences.Save();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	protected void OnGUI_MessageArea(float buildAreaWidth=200)
	{
		GUILayout.BeginArea(new Rect(10,10,Screen.width-buildAreaWidth-20, Screen.height-30));
		float boxHeight = Screen.height-60;
		
		GUILayout.Label("What's changed in this build:");
		string[] popupOptions = new string[Mathf.Min(32,messageOptions.Count)];
		
		for(int i=0; i<popupOptions.Length; ++i)
		{
			string s = messageOptions[i].Trim();
			s = s.Substring(0, Mathf.Min(s.Length,64));
			s = s.Replace('-', '_');
			s = s.Replace('&', '_');
			s = s.Replace('^', '_');
			s = s.Replace('\r', ' ');
			s = s.Replace('\n', ' ');
			s = s.Replace('/', '\\');
			popupOptions[i] = s;
		}
		
		selectedMessage = EditorGUILayout.Popup(selectedMessage, popupOptions, "Popup");
		if(selectedMessage > 0)
		{
			message = messageOptions[selectedMessage];
			GUI.color = Color.grey;
			GUILayout.Box(message, "TextArea", GUILayout.Width(Screen.width-10), GUILayout.Height(boxHeight));
			GUI.color = Color.white;
		}
		else 
		{
			message = GUILayout.TextArea(message, GUILayout.Width(Screen.width-10), GUILayout.Height(boxHeight));
		}

		GUILayout.EndArea();
	}
	
	public void Update()
	{
		if(startBuild || closeWindow)
			Close();

		if(startBuild)
		{
			OnStartBuild ();
		}
			
	}

	protected virtual void OnStartBuild ()
	{
		if(TestFlightBuildPipeline.HasPro())
			TestFlightBuildWindow.DoBuild(message, TestFlightPreferences.Load());
		else 
			TestFlightNonProBuildStep.DoBuild(message);
	}
}
