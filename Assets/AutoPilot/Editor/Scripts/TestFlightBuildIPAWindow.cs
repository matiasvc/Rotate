using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class TestFlightBuildIPAWindow : TestFlightMessageWindow
{
	new public static void CreateWindow()
	{
		TestFlightMessageWindow window = GetWindow<TestFlightBuildIPAWindow>(true);
		window.position = new Rect(10, 10, 600, 300);
		window.minSize  = new Vector2(600, 300); 
		window.maxSize  = new Vector2(1024, 300); 
	}	
	
	protected void OnGUI_BuildLocation()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(5);
			
		GUILayout.BeginVertical();
		GUILayout.Label("Output Location:");
		GUILayout.BeginHorizontal("box", GUILayout.MaxWidth(Screen.width-15));
		preferences.userPrefs.ipaBuildPath = GUILayout.TextField(preferences.userPrefs.ipaBuildPath);
		if(GUILayout.Button("...", GUILayout.Width(50)))
		{
			string result = EditorUtility.SaveFilePanel("Save IPA package To...", 
											System.IO.Path.GetDirectoryName(preferences.userPrefs.ipaBuildPath),
											System.IO.Path.GetFileName(preferences.userPrefs.ipaBuildPath),
											"ipa");
			
			if(result != null && result.Length > 0)
				preferences.userPrefs.ipaBuildPath = result;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
	}
	
	public override void OnGUI ()
	{		
		InitSkin();
		
		OnGUI_BuildLocation();

		OnGUI_DrawBundleVersion();
		
		GUILayout.Label("Build Options:");
		GUILayout.BeginVertical("box", GUILayout.Width(Screen.width-15));
		TestFlightPreferencesWindow.DrawSubUserSettingsForIPA(preferences, true, allProvisions, allSchemas, allIdentities);
		GUILayout.Space(5);
		GUILayout.EndVertical();
		
		if(logoTex)
			GUI.DrawTexture(new Rect(Screen.width-logoTex.width-10, Screen.height-logoTex.height-20, logoTex.width, logoTex.height), logoTex);
	
		OnGUI_Buttons("Build & Package");
	}
	
	protected override void OnStartBuild ()
	{		
  		if(TestFlightBuildPipeline.HasPro())
			TestFlightBuildWindow.DoIPABuild(preferences, true);
		else 
			TestFlightNonProBuildStep.DoBuild("", true);	
	}
}
