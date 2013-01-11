using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;


class TestFlightBuildWindow : TestFlightWindow
{
	public enum BuildStage { WaitingToStart, BuildProject, PackageIPA, Upload, Max};
	BuildStage currentStage 					= BuildStage.WaitingToStart;
	System.Diagnostics.Process currentProcess	= null;
	public TestFlightPreferences preferences	= null;
	string message								= "";
	string resultURL							= "";
	Texture logoTex 							= null;
	System.DateTime startedProcessTime			= System.DateTime.Now;
	public bool allowToLive = false; // we kill the window unless it was created by the user 
	string buildAppStageLog 					= "";
	int	   uploadAttempsRemaining				= 1;
	bool   buildIPAOnly							= false;
	
	private static TestFlightBuildWindow _instance = null;

	public static void DoIPABuild (TestFlightPreferences preferences, bool buildPlayer)
	{
		DoBuild("", buildPlayer, true, preferences);	
	}
	
	public static void DoBuild(string message, TestFlightPreferences preferences)
	{
		DoBuild(message, TestFlightBuildPipeline.HasPro(), false, preferences);
	}
	
	public static void DoBuild(string message, bool buildPlayer, bool buildIPAOnly, TestFlightPreferences preferences)
	{
		if(_instance!=null)
		{
			Debug.LogError("AutoPilot: cannot start, a build is already in progress"); 
			return;
		}
		
		if(TestFlightBuildPipeline.onPreBuild!=null)
			TestFlightBuildPipeline.onPreBuild(preferences);
		
		if(buildPlayer)
		{
			if(!TestFlightBuildPipeline.BuildPlayerIOS(preferences))
				return;
		}
		
		TestFlightBuildWindow window = TestFlightBuildWindow.CreateInstance<TestFlightBuildWindow>();
				
		window.preferences = preferences;
		window.allowToLive = true;
		window.message = message;
		window.buildIPAOnly = buildIPAOnly;

		window.position = new Rect(0, 0, 300, 140);
		window.minSize = new Vector2(window.position.width, window.position.height);
		window.maxSize = new Vector2(window.position.width+1, window.position.height+1);
		
		window.ShowPopup();
	}
	
	public static void AbortBuild()
	{
		if(_instance)
			_instance.Close();
	}
	
	public void OnEnable()
	{
		logoTex = TestFlightResources.GetTextureResource("logo_small.png");
		resultURL = "";
		_instance = this;
	}
	
	public void DoXCodeCheck()
	{
		// XCODE SELECT PRINT PATH CHECK
		System.Diagnostics.Process process = TestFlightBuildPipeline.StartProcess("xcode-select", "--print-path", true);
		process.WaitForExit(200);
		string currentFolder = process.StandardOutput.ReadLine();
		string path = currentFolder+"/usr/bin/xcrun";
		//Debug.Log(path);
		if(System.IO.File.Exists(path))
			return;
	
		string folder = "/Applications/Xcode.app/Contents/Developer";		
		if(!System.IO.Directory.Exists(folder))
			folder = "/Developer";
		
		EditorUtility.DisplayDialog("Warning", "XCode command line tools don't appear to be configured with your XCode path.\n" +
												"(Build will likely fail)\n\n" +
												"To fix:\n" +
												"1. Open the Terminal app and enter the following command:\n" +
												"\tsudo xcode-select -switch "+folder+"\n\n"+
		                            			"2. Then press the Ok button to continue", "Ok");
	}
	
	private void CloseProcess()
	{
		if(currentProcess == null)
			return;
				
		currentProcess.Kill();
	}
	
	public void OnDestroy()
	{
		if(currentProcess!=null && !currentProcess.HasExited)
		{
			CloseProcess();
		}
		
		EditorUtility.ClearProgressBar();
		_instance = null;
	}
	
	private void SetStage(BuildStage stage)
	{
		currentStage = stage;
		Repaint();
	}
	
	public void OnGUI()
	{
		InitSkin();
		
		if(logoTex)
			GUI.DrawTexture(new Rect(Screen.width-logoTex.width, Screen.height-logoTex.height, logoTex.width, logoTex.height), logoTex);		

		GUILayout.Label("AutoPilot stages");
		DrawStageLabel(BuildStage.BuildProject);
		DrawStageLabel(BuildStage.PackageIPA);
		if(!buildIPAOnly)
			DrawStageLabel(BuildStage.Upload);
		
		GUILayout.BeginArea(new Rect(5, Screen.height-25,60,20));
		if(GUILayout.Button("Abort"))
		{
			Stop();
			return;
		}		
		GUILayout.EndArea();
	}
	
	private static  string GetNameOfStage(BuildStage stage)
	{
		switch(stage)
		{
		case BuildStage.WaitingToStart: return 	"Starting engines";
		case BuildStage.BuildProject: return 	"Build .app";
		case BuildStage.PackageIPA: return 		"Package .ipa";
		case BuildStage.Upload: return 			"Upload to TestFlight";
		default: return "Just a moment";
		}
	}
	
	private string GetStageErrorMessages(BuildStage stage)
	{
		switch(stage)
		{
		case BuildStage.BuildProject: return 	"An error occured trying to build the .app file, please ensure you can manually build an iPhone project at the build location: "+preferences.teamPrefs.buildPath;
		case BuildStage.PackageIPA: return 		"An error occured while packaging the .ipa file, please retry this build... if that does not work build your iPhone project manually and ensure you are able to \"Build & Archive\" within XCode without errors.";
		case BuildStage.Upload: return 			"An error occured uploading to testflight. Please check that you can access testflightapp.com";
		default: return "";
		}
	}
	
	private string GetStageErrorSuggestion(BuildStage stage, string errorString)
	{
		switch(stage)
		{
		case BuildStage.BuildProject: 
			if(errorString.Contains("Reason for failure: -[__NSCFDictionary setObject:forKey:]: attempt to insert nil key"))
				return "1. The selected Developer Identity may be invalid, or not compatible with this project.\n" +
					   "2. If you selected the default 'iPhone Distribution' identity, XCode may not have a default for this. Try using a specific certificate.";
			else 	
				return "";
			
		case BuildStage.PackageIPA: 
			return "";
			
		case BuildStage.Upload: 
			if(errorString.Contains("curl: (6) Could not resolve host:"))
				return "1. Cannot connect to testfight. It may be down, or there may be network issues. Check that you can access testflightapp.com";
			if(errorString.Contains("Invalid Profile: developer build entitlements must have get-task-allow set to true."))
				return  "1. Attempting to upload Ad-hoc build using a 'Developer' identity certificate instead of the 'Distribution' identity it's built against";
			else 
				return "";
			
		default: 
			return "";
		}
	}
	
	private void DrawStageLabel(BuildStage targetStage)
	{
		string text = GetNameOfStage(targetStage);
		if(targetStage==currentStage)
			GUILayout.Label(text, (targetStage != currentStage) ? "Label":"Foldout");
		else 
			GUILayout.Toggle((int)targetStage<(int)currentStage, text);
	}
	
	public void OnInspectorUpdate()
	{
		
		if(!allowToLive)	
		{
			// prevent the window relaunching if unity was closed mid build
			Close();
			return;
		}
		
		if(EditorApplication.isCompiling)
			return;
		
		float t = Mathf.Clamp01((float)((System.DateTime.Now-startedProcessTime).TotalSeconds)/Mathf.Max(0.1f,GetLastBuildTime(currentStage)))-.01f;
		if(EditorUtility.DisplayCancelableProgressBar("AutoPilot progress", GetNameOfStage(currentStage), t))
		{
			Stop();
			return;
		}
		
		if(currentProcess != null)
		{
			if(currentStage == BuildStage.BuildProject && !currentProcess.StandardOutput.EndOfStream)
				buildAppStageLog += currentProcess.StandardOutput.ReadToEnd();
			
			if(!currentProcess.HasExited)
				return;
		}

		if(CheckProcessErrors())
		{
			Stop();
			return;
		}

		NextStage();
	}
	
	private float GetLastBuildTime(BuildStage stage)
	{
		switch(stage)
		{
		case BuildStage.WaitingToStart: return 2;
		case BuildStage.BuildProject: 	return preferences.userPrefs.lastBuildAppTime;
		case BuildStage.PackageIPA: 	return preferences.userPrefs.lastPackageIpaTime;
		case BuildStage.Upload: 		return preferences.userPrefs.lastUploadTime;
		}
		return -1;
	}
	
	private void SetBuildTime(BuildStage stage, float time)
	{
		switch(stage)
		{
		case BuildStage.BuildProject: 	
			preferences.userPrefs.lastBuildAppTime = time; 
			preferences.userPrefs.Save(); 
			break;
			
		case BuildStage.PackageIPA: 	
			preferences.userPrefs.lastPackageIpaTime = time; 
			preferences.userPrefs.Save(); 
			break;
			
		case BuildStage.Upload: 		
			preferences.userPrefs.lastUploadTime = time;
			preferences.userPrefs.Save(); 
			break;
		}
	}
	
	private bool CheckProcessErrors()
	{
		string errorString = "";
		int errorCode = 0;
		
		if(currentProcess == null || !currentProcess.HasExited)
			return false;
		
		if(currentProcess.ExitCode != 0) 
		{
			errorCode = currentProcess.ExitCode;			
			
			if(currentStage == BuildStage.BuildProject)
			{
				StringReader logFile = new System.IO.StringReader(buildAppStageLog);
				while(true)
				{
					string result = logFile.ReadLine();
					if(result == null)
						break;
						
					Match m = Regex.Match(result, @"\[BEROR\](.*)");
					if(m.Success)
					{
						errorString += m.Groups[1].Captures[0].Value+"\n";
						continue;
					}
					
					// 3 line compiple error
					// we just give the first line of this as formatting makes them unreadable
					m = Regex.Match(result, @":[0-9]+:[0-9]+: (error|warning):.*");
					if(m.Success)
					{
						errorString += result+"\n\n";
						continue;
					}
					
					// linker errors
					m = Regex.Match(result, @"Undefined symbols for architecture");
					if(m.Success)
					{
						do
						{
							errorString += result+"\n";
							result = logFile.ReadLine();
							m = Regex.Match(result, @"ld: symbol\(s\) not found for architecture");
						} while(result != null && !m.Success);
						errorString += "\n";
					}
				}		
			}
			else
				errorString = currentProcess.StandardError.ReadToEnd();
		}
		else if(currentStage == BuildStage.Upload)
		{	
			while(!currentProcess.StandardOutput.EndOfStream)
			{
				string str = currentProcess.StandardOutput.ReadLine();

				if(currentProcess.StandardOutput.EndOfStream)
				{
					if(str == "200")
						break;
					else 
						errorCode = int.Parse(str);
				}
				else 
				{
					errorString += str;
					
					string[] tokens = str.Split(new char[]{' ', ',', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
					if(tokens.Length == 2 && tokens[0] == "\"config_url\":")
					{
						resultURL = tokens[1].Trim(new char[]{'"'}).Replace("complete", "report");
					}
				}
			}
		}
	
		if(errorCode == 0)
			return false;
		else if(currentStage == BuildStage.Upload)
		{
			if(uploadAttempsRemaining <= 0)
			{
				string uploadSuggestion = GetStageErrorSuggestion(currentStage, errorString);
				int result = EditorUtility.DisplayDialogComplex("Trouble Uploading", 
					"There was a problem uploading to testlight.\n" +
					"This happens sometimes when their servers are busy, retrying the upload usually works after a few attempts." +
					"\n\nError Message:\n"+GetStageErrorMessages(currentStage)+
					(uploadSuggestion.Length > 0 ? ("\n\nKnown Causes for this error:\n"+uploadSuggestion):"")+
					"\n\nAdditional Output:\n"+errorString,
					"Retry",
					"Cancel",
					"Retry 10 times");
				
				Debug.LogError("AutoPilot: Stage "+currentStage+" failed:"+GetStageErrorMessages(currentStage)+"\nAdditional Output:"+errorString);
				
				if(result == 0)		
					uploadAttempsRemaining = 1;
				else if (result==2) 
					uploadAttempsRemaining = 10;
				else 
					return true;
			}
			
			if(uploadAttempsRemaining > 0)
			{
				currentStage = BuildStage.PackageIPA; // nextstage will be called, setting us to upload again
				return false;
			}
		}
		
		string suggestion = GetStageErrorSuggestion(currentStage, errorString);
		
		int option = EditorUtility.DisplayDialogComplex("AutoPilot: Build FAILED", "Stage "+currentStage+
		                            "\n\nError Message:\n"+GetStageErrorMessages(currentStage)+
		                            (suggestion.Length > 0 ? ("\n\nKnown Causes for this error:\n"+suggestion):"")+
		                            "\n\nIf you think this error is a bug with AutoPilot:\nPlease contact the developer (cratesmith@cratesmith.com) and provide the full error information from the console window."+
									"\n\nAdditional Output:\n"+errorString, "Ok", "Check on forums...", "Contact developer...");
		
	   switch (option) 
		{
	            // Ok
	            case 0:
	                break;
	            
			// Check forums
	            case 1:
	                Application.OpenURL("http://forum.unity3d.com/threads/100591-AutoPilot-for-Testflight-1.0-(deploy-iOS-builds-via-cloud!)-Now-on-the-asset-store");
	                break;
			
	            // Email developer
	            case 2:
					string bodyString = "1. Describe the issue in detail\n" +
										"\n" +
										"\n" +
										"2. List steps that caused this issue\n" +
										"\n" +
										"\n" +
										"3. How often does this happen?\n" +
										"\n" +
										"\n" +
										"Build Stage: " + currentStage + "\n" +
										"\n" +
										"Build Error:\n" + errorString;
										
					bodyString = Google.GData.Client.HttpUtility.UrlEncode(bodyString);
					Debug.Log(bodyString);
	                Application.OpenURL("mailto:cratesmith@cratesmith.com?subject=Autopilot%20Issue&body="+bodyString);
	                break;
		}

		
		Debug.LogError("AutoPilot: Stage "+currentStage+" failed:"+GetStageErrorMessages(currentStage)+"\nAdditional Output:"+errorString);
		return true;
	}
	
	private void NextStage()
	{
		if(currentProcess!=null)
		{
			if(!currentProcess.HasExited)
				currentProcess.Close();
		}
		
		SetBuildTime(currentStage, (float)(System.DateTime.Now-startedProcessTime).TotalSeconds);
		startedProcessTime = System.DateTime.Now;
		
		SetStage((BuildStage)((int)currentStage + 1));
				
		switch(currentStage)
		{			
		case BuildStage.BuildProject: 
			DoXCodeCheck();
			currentProcess = TestFlightBuildPipeline.BuildApp(preferences);
			buildAppStageLog = "";
			if(currentProcess == null)
				Stop();
			break; 
			
		case BuildStage.PackageIPA: 
			
			// sleep before doing this (Xcode may take some time to place the build in the folder)
			System.Threading.Thread.Sleep(300);

			if(buildIPAOnly)
			{
				currentProcess = TestFlightBuildPipeline.BuildIPA(preferences, preferences.userPrefs.ipaBuildPath);
				TestFlightBuildPipeline.ZipDSYMTo(preferences,preferences.userPrefs.ipaBuildPath);
			}
			else 
				currentProcess = TestFlightBuildPipeline.BuildIPA(preferences);
				
			if(currentProcess == null)
				Stop();
			break; 
			
		case BuildStage.Upload:
			if(buildIPAOnly)
			{
				EditorUtility.DisplayDialog("AutoPilot", "Build IPA complete", "Ok");
				TestFlightBuildPipeline.StartProcess("open", "-n -R \""+preferences.userPrefs.ipaBuildPath+"\"", false);
				Application.OpenURL(preferences.userPrefs.ipaBuildPath);
				Close();
			}
			else 
			{
				currentProcess = TestFlightBuildPipeline.TestFlightUpload(message, preferences);
				if(currentProcess == null)
				{
					Stop();
					return;
				}
				--uploadAttempsRemaining;
			}
			break; 
			
		case BuildStage.Max:
			if(resultURL.Length != 0)
			{
				EditorUtility.DisplayDialog("AutoPilot", "Build and upload complete", "Ok");
				Debug.Log("AutoPilot: Build & Upload complete");
				Application.OpenURL(resultURL);
			}
			Close();
			break;
		}
	}
	
	private void Stop()
	{		
		EditorUtility.DisplayDialog("AutoPilot", "Build Aborted", "Ok");
		Debug.Log("AutoPilot: Build & Upload aborted");
		Close();
	}
}