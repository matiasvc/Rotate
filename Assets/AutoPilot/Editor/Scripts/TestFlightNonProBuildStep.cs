using UnityEditor;
using UnityEngine;

public class TestFlightNonProBuildStep : TestFlightWindow
{
	private static TestFlightNonProBuildStep instance = null;
	public TestFlightPreferences preferences;
	public int step = 1;
	public string message;
	const int maxSteps = 4;
	Texture instructionTex = null;
	bool closing = false;
	bool buildIPAOnly = false;
	
	public static void DoBuild(string message, bool buildIPAOnly=false)
	{
		TestFlightNonProBuildStep window = GetWindow<TestFlightNonProBuildStep>(true);
		window.message = message;
		window.title = "Unity Basic Detected - Manual build step required";
		window.step = 1;
		window.preferences = TestFlightPreferences.Load();
		window.instructionTex = TestFlightResources.GetTextureResource("NonProBuildInstructions.jpg");
		window.minSize  = new Vector2(window.instructionTex.width, window.instructionTex.height); 
		window.position = new Rect(10, 10, window.minSize.x, window.minSize.y);
		window.buildIPAOnly = buildIPAOnly;
		
		TestFlightBuildPipeline.PreBuildPlayer(window.preferences);
		System.IO.Directory.CreateDirectory(window.preferences.teamPrefs.buildPath);
	}
	
	public static void TriggerBuildIfWaiting(string pathToBuiltProject)
	{
		if(instance == null)
			return;
		
		instance.preferences.teamPrefs.buildPath = pathToBuiltProject;
		instance.StartBuild();
		instance.Close();
		instance = null;
	}
	
	private void StartBuild()
	{
		TestFlightBuildWindow.DoBuild(message, false, buildIPAOnly, preferences);
	}
	
	public void OnEnable()
	{
		instance = this;
	}
	
	public void OnDisable()
	{
		instance = null;
	}

	public void OnGUI()
	{	
		if(instructionTex)
			GUI.DrawTexture(new Rect(0,Screen.height-instructionTex.height,instructionTex.width, instructionTex.height), instructionTex);		
	}
	
	public void Update()
	{
		if(closing)
		{
			Close();
		}
	}
	
	public void OnGUI_Step1()
	{
		EditorGUILayout.Separator();
		GUILayout.Label("- Select \"Build Settings...\" from the file menu");
		EditorGUILayout.Separator();
		GUILayout.Label("- Set platform to iOS");
	}
	
	public void OnGUI_Step2()
	{
		EditorGUILayout.Separator();
		GUILayout.Label("- Press the \"Build\" button");
		EditorGUILayout.Separator();
		GUILayout.Label("- Build your project into this folder:");
		EditorGUILayout.TextField(System.IO.Path.GetFullPath(preferences.teamPrefs.buildPath));
	}
	
	public void OnGUI_Step3()
	{
		GUILayout.Label("Wait for the player build process to finish, then press \"Next...\" to start AutoPilot");
	}
	
	public void PrevStep()
	{
		step = Mathf.Clamp(step-1, 0, maxSteps);	
	}
	
	public void NextStep()
	{
		step = Mathf.Clamp(step+1, 0, maxSteps);
	}
}