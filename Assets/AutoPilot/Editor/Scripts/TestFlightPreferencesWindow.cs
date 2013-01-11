using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;

class TestFlightPreferencesWindow : TestFlightWindow
{
	public bool launchBuildOnSave = false;
	TestFlightPreferences preferences;
	Vector2 scrollPos = Vector2.zero;
	bool showUserPrefs = true;
	bool showTeamPrefs = true;
	bool showTeamLists = true;
	bool showUserLists = true;
	Texture logoTex = null;
	bool isClosing = false;
	bool isStartingBuild = false;

	TestFlightMobileProvision[] allProvisions = new TestFlightMobileProvision[0];
	TestFlightXCodeSchemas allSchemas = null;
	string[] allIdentities = new string[0];

	static int FieldWidth { get { return 125; } }
	
	[UnityEditor.MenuItem("File/AutoPilot: Testflight Build Preferences #&%b", true)]
	public static bool ValidateShowPreferencesWindow()
	{
		if(Application.platform != RuntimePlatform.OSXEditor)
			return false;
		
		return true;
	}
	
	[UnityEditor.MenuItem("File/AutoPilot: Testflight Build Preferences #&%b")]
	public static void ShowPreferencesWindow()
	{		
		TestFlightPreferencesWindow tfpw = GetWindow<TestFlightPreferencesWindow>(true);
		tfpw.minSize = new Vector2(800, 625);
		tfpw.Show();
	}
	
	public void OnEnable()
	{
		preferences = TestFlightPreferences.Load();
		logoTex = TestFlightResources.GetTextureResource("logo_small.png");
		title = "AutoPilot Preferences";
		
		allProvisions = TestFlightMobileProvision.EnumerateProvisions();
		allSchemas = TestFlightXCodeSchemas.Enumerate(preferences.teamPrefs.buildPath);
		allIdentities = TestFlightDeveloperIdentities.Enumerate();
	}
	
	public void OnDisable()
	{
		// we move TestFlightSDK.cs to the plugins folder if it's not there already
		// this ensures plugin support (we don't move the folder because that's bad for people using SVN)
		if(File.Exists("Assets/AutoPilot/Plugins/TestFlightSDK/TestFlightSDK.cs"))
		{
			Directory.CreateDirectory("Assets/Plugins/TestFlightSDK");
		
			if(File.Exists("Assets/Plugins/TestFlightSDK/TestFlightSDK.cs")) 
			{
				File.Delete("Assets/Plugins/TestFlightSDK/TestFlightSDK.cs");
			}
			AssetDatabase.MoveAsset("Assets/AutoPilot/Plugins/TestFlightSDK/TestFlightSDK.cs", "Assets/Plugins/TestFlightSDK/TestFlightSDK.cs");
			
			AssetDatabase.Refresh();
			Thread.Sleep(300);
		}
		
		// enable the SDK if in use 
		TestFlightBuildPipeline.UpdateSDKEnabledInBuild();
	}
		
		
	public void OnWizardOtherButton()
	{
		Close();
	}
	
	public void OnWizardCreate()
	{
		try
		{
			if(Path.GetFullPath(preferences.teamPrefs.buildPath) == Path.GetFullPath("."))
			{
				EditorUtility.DisplayDialog("Error: Invalid build path", "You can't set your build output folder to the project folder.", "Reset to default");
				preferences.teamPrefs.buildPath = "./builds/iPhone";
			}
		}
		catch(System.ArgumentException)
		{
			EditorUtility.DisplayDialog("Error: Invalid build path", "Not set to a valid path", "Reset to default");
			preferences.teamPrefs.buildPath = "./builds/iPhone";
		}		
		
		preferences.Save();
	}
	
	public void Update()
	{
		if(isClosing)
		{
			Close();
		}
		
		if(isStartingBuild)
		{
			TestFlightMessageWindow.CreateWindow();
		}
	}
	
	public void OnGUI()
	{	
		InitSkin();
		
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		
		GUI.Box(new Rect(0,0,FieldWidth+14, Screen.height-20), "");
		EditorGUILayout.Separator();
		
		OnGUI_TeamSettings();
		EditorGUILayout.Separator();

		OnGUI_UserSettings();	
		
		if(logoTex)
			GUI.DrawTexture(new Rect(Screen.width-logoTex.width, Screen.height-logoTex.height-30, logoTex.width, logoTex.height), logoTex);		
		
		GUILayout.BeginArea(new Rect(Screen.width-220, Screen.height-30, 220, 20));
		GUILayout.BeginHorizontal();

		if(GUILayout.Button(new GUIContent("Cancel", "Close this window without saving changes")))
		{
			OnWizardOtherButton();
			isClosing = true;
		}
		
		if(GUILayout.Button(new GUIContent("Save", "Save changes and close this window")))
		{
			OnWizardCreate();
			isClosing = true;
		}
		
		if(TestFlightBuildPipeline.CanBuild(preferences, allProvisions, allIdentities))
		{
			if(GUILayout.Button(new GUIContent("Save & Build", "Save changes, build and upload to testflight")))
			{
				OnWizardCreate();
				isClosing = true;
				isStartingBuild = true;
			}
		}
		else if(TestFlightBuildPipeline.MenuItemAllowed)
		{
			GUILayout.Box(new GUIContent("Save & Build", "You can't build & upload because your user or team API token hasn't been set"), GUILayout.Width(92), GUILayout.Height(17));
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		GUILayout.EndScrollView();
		
	}
	
	private void OnGUI_TeamSettings()
	{
		showTeamPrefs = EditorGUILayout.Foldout(showTeamPrefs, new GUIContent("Team Preferences", "These options should remain the same for your whole team.\n\n(They are saved in 'ProjectDir\\TestFlightTeamPreferences.xml' which you SHOULD check into your team's source control)."));
		if(!showTeamPrefs)
			return;
		
		EditorGUILayout.BeginHorizontal(); 
		GUILayout.Space(20); 
		EditorGUILayout.BeginVertical();
		
		preferences.teamPrefs.teamAPIKey = TextField(new GUIContent("Team Token", "The testflight API token for your team"), preferences.teamPrefs.teamAPIKey).Replace(" ","");
		if(preferences.teamPrefs.teamAPIKey.Length==0)
		{
			EditorGUILayout.BeginHorizontal(); 
			GUILayout.Space(FieldWidth);
			Color baseColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			GUILayout.Box("Please set your team API token!", "textarea");
			GUI.backgroundColor = baseColor;
			if(GUILayout.Button("Get your Team API Token...", GUILayout.Width(180)))
			{
				Application.OpenURL("https://testflightapp.com/dashboard/team/edit/");
			}
			EditorGUILayout.EndHorizontal(); 
		}
		
		GUILayout.BeginHorizontal();
		preferences.teamPrefs.buildPath = TextField(new GUIContent("Build Path", "The relative path AutoPilot will build your iPhone project at. (Relative to the current project directory)"), preferences.teamPrefs.buildPath).Replace(@"\", "/");
		if(GUILayout.Button(new GUIContent("..."), GUILayout.ExpandWidth(false)))
		{
			preferences.teamPrefs.buildPath = EditorUtility.SaveFolderPanel("Select Build Output Folder", preferences.teamPrefs.buildPath, "iPhone");
			preferences.teamPrefs.buildPath = EvaluateRelativePath(Path.GetFullPath("."), preferences.teamPrefs.buildPath);
		}

		try
		{
			if(Path.GetFullPath(preferences.teamPrefs.buildPath) == Path.GetFullPath("."))
			{
				EditorUtility.DisplayDialog("Error: Invalid build path", "You can't set your build output folder to the project folder.", "Reset to default");
				preferences.teamPrefs.buildPath = "./builds/iPhone";
			}
		}
		catch(System.ArgumentException)
		{
		}
		
		GUILayout.EndHorizontal();
		
		OnGUI_TeamSettings_DistLists();
		
		{
			GUILayout.BeginHorizontal();

			Color baseColor = GUI.backgroundColor;
			bool isSDKInstalled = TestFlightBuildPipeline.IsTestFlightSDKInstalled();
			string text = isSDKInstalled ? "Testflight SDK is installed":"Testflight SDK is not installed";
			string downloadButtonText = isSDKInstalled ? "Redownload...":"Download...";
			string installButtonText = isSDKInstalled ? "Reinstall...":"Install...";
			GUI.backgroundColor = isSDKInstalled ? Color.green:Color.yellow;
			
			TextField(new GUIContent("Testflight SDK"), text, 0, false);
			GUI.backgroundColor = baseColor;
			//GUILayout.Space(FieldWidth);
			if(GUILayout.Button(downloadButtonText, GUILayout.Width(120)))
			{
				Application.OpenURL("https://testflightapp.com/sdk/download/");
			}
			if(GUILayout.Button(installButtonText, GUILayout.Width(120)))
			{
				InstallSDK_Dialog();
			}
			GUILayout.EndHorizontal();
		}

				
		EditorGUILayout.EndVertical(); 
		EditorGUILayout.EndHorizontal(); 
	}
	
	static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
	{
	    string[] firstPathParts = mainDirPath.Trim(System.IO.Path.DirectorySeparatorChar).Split(System.IO.Path.DirectorySeparatorChar);
	    string[] secondPathParts = absoluteFilePath.Trim(System.IO.Path.DirectorySeparatorChar).Split(System.IO.Path.DirectorySeparatorChar);
	
	    int sameCounter = 0;
	    for (int i = 0; i < System.Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
	    {
	        if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
	        {
	            break;
	        }
	        sameCounter++;
	    }
	
	    if (sameCounter == 0)
	    {
	        return absoluteFilePath;
	    }
	
	    string newPath = System.String.Empty;
	    for (int i = sameCounter; i < firstPathParts.Length; i++)
	    {
	        if (i > sameCounter)
	        {
	            newPath += System.IO.Path.DirectorySeparatorChar;
	        }
	        newPath += "..";
	    }

	    for (int i = sameCounter; i < secondPathParts.Length; i++)
	    {
			if (newPath.Length > 0)
	        	newPath += System.IO.Path.DirectorySeparatorChar;
	        newPath += secondPathParts[i];
	    }
	    return newPath;
	} 	
	void InstallSDK_Dialog()
	{
		string path = EditorUtility.OpenFolderPanel("Browse to folder with TestFlight.h and libTestFlight.a", ".", "bacon");
		if(!File.Exists(path+"/TestFlight.h") || !File.Exists(path+"/libTestFlight.a"))
		{
			EditorUtility.DisplayDialog("Error", "Couldn't find both TestFlight.h and libTestFlight.a in that folder", "Ok");
		}
		else
		{
			TestFlightBuildPipeline.InstallSDKFrom(path);
		}
	}
	
	private void OnGUI_TeamSettings_DistLists()
	{
		Rect btnRect = new Rect(FieldWidth+22,GUILayoutUtility.GetLastRect().yMax+2, 180, 16);
		
		if(GUI.Button(btnRect, "Configure Distribution Lists..."))
		{
			Application.OpenURL("https://testflightapp.com/dashboard/team/");
		}
		

		Color baseColor = GUI.color;
		GUI.color = Color.green;
		GUI.Label(new Rect(btnRect.xMax+10, btnRect.yMin, Screen.width-(btnRect.xMax+20), 20), "Enter your distribution lists from testflightapp.com below");
		GUI.color = baseColor;
		
		preferences.teamPrefs.distributionLists = StringArrayField(new GUIContent("Distribution Lists", "The distribution lists you have configured on TestFlightApp.com for this project, click the button to configure lists on the website then enter their names here"), preferences.teamPrefs.distributionLists, ref showTeamLists, true);	
	}

	private void OnGUI_UserSettings()
	{
		showUserPrefs = EditorGUILayout.Foldout(showUserPrefs, new GUIContent("User Preferences", "These options change for each user, and store your current build settings as a user.\n\n(They are saved in 'ProjectDir\\TestFlightUserPreferences.xml' which you SHOULD NOT check into your team's source control)."));
		if(!showUserPrefs)
			return;
		
		EditorGUILayout.BeginHorizontal(); 
		GUILayout.Space(20); 
		EditorGUILayout.BeginVertical();
		
		preferences.userPrefs.userAPIKey = TextField(new GUIContent("User Token", "The testflight API token for your user account"), preferences.userPrefs.userAPIKey).Replace(" ","");
		if(preferences.userPrefs.userAPIKey.Length==0)
		{
			EditorGUILayout.BeginHorizontal(); 
			GUILayout.Space(FieldWidth);
			
			Color baseColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			GUILayout.Box("Please set your user API token!", "textarea");
			GUI.backgroundColor = baseColor;

			
			if(GUILayout.Button("Get your User API Token...", GUILayout.Width(180)))
			{
				Application.OpenURL("https://testflightapp.com/account/#api");
			}
			EditorGUILayout.EndHorizontal();
		}
		
		DrawSubUserSettings(preferences, ref showUserLists, false, allProvisions, allSchemas, allIdentities);
				
		EditorGUILayout.EndVertical(); 
		EditorGUILayout.EndHorizontal(); 
	}
	
	public static void DrawSubUserSettingsForIPA(TestFlightPreferences preferences, bool narrow, TestFlightMobileProvision[] availableProvisions, TestFlightXCodeSchemas availableSchemas, string[] availableIdentities)
	{
		DrawSubUserSettings_TestFlightSDK(preferences, narrow);
		DrawSubUserSettings_dSYM(preferences, narrow);		
		DrawSubUserSettings_BundleId(availableProvisions, availableIdentities);
		DrawSubUserSettings_Provisions(preferences, narrow, availableProvisions, availableIdentities);
		
		if(preferences.userPrefs.customProvisionProfile != null && preferences.userPrefs.customProvisionProfile.Certificates == null)
			DrawSubUserSettings_Identity(preferences, narrow, availableIdentities, availableProvisions);
		
		DrawSubUserSettings_XCodeSchema(preferences, narrow, availableSchemas);
		
		if(!narrow)
		{
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.Space(FieldWidth+4);
			GUILayout.Box("* Denotes settings that apply to both Autopilot and regular Unity builds");
			GUILayout.EndHorizontal();
		}
	}
	
	public static void DrawSubUserSettings(TestFlightPreferences preferences, ref bool showUserLists, bool narrow, TestFlightMobileProvision[] availableProvisions, TestFlightXCodeSchemas availableSchemas, string[] availableIdentities)
	{
		DrawSubUserSettings_TestFlightUpload(preferences, ref showUserLists);
		DrawSubUserSettingsForIPA(preferences, narrow, availableProvisions, availableSchemas,availableIdentities);
	}
	
	private static void DrawSubUserSettings_dSYM(TestFlightPreferences preferences, bool narrow)
	{
		preferences.userPrefs.dSYMBuild = Toggle(new GUIContent ("Build & include dSYM", "Builds symbols for this project.\nThis makes the bundle size significantly larger but can greatly increase the detail in TestFlight's crash reports.\n\nNote: even with a dSYM many crashes won't be reported if 'Fast but no exceptions' has been set in iPhone settings"), preferences.userPrefs.dSYMBuild);
	}
	
	private static void DrawSubUserSettings_TestFlightUpload(TestFlightPreferences preferences, ref bool showUserLists)
	{
		preferences.userPrefs.notifyUsers = Toggle(new GUIContent("Notify Users", "Send emails to any users added to this build? (All users if it's a new build)"), preferences.userPrefs.notifyUsers);
		preferences.userPrefs.replaceSameBuilds = Toggle(new GUIContent("Replace Builds", "If this build has the same version as one already uploaded, do we replace it? Or do we create a new build with the same version number?"), preferences.userPrefs.replaceSameBuilds);
		preferences.userPrefs.activeDistributionLists = StringToggleField(new GUIContent("Active Lists", "Which of our distribution lists will we send builds to?"), preferences.teamPrefs.distributionLists, preferences.userPrefs.activeDistributionLists, ref showUserLists);
	}

	private static void DrawSubUserSettings_TestFlightSDK (TestFlightPreferences preferences, bool narrow)
	{
		bool hasSDK = TestFlightBuildPipeline.IsTestFlightSDKInstalled();
		string testFlightDescriptionBox = "You can still use testflight to deploy builds on users devices,\nbut won't receieve session logs/crashdumps. Features such as checkpoints,\nuser feedback and questions will be unavailable";
		Color testFlightDescriptionBoxColor = Color.grey; 
		Texture testFlightDescriptionTexture = null;
		
		if(hasSDK)
		{
			GUILayout.BeginHorizontal();
			GUIContent[] options = new GUIContent[] { new GUIContent("Don't use TestFlight SDK"), new GUIContent("TestFlight SDK (for testers)",  TestFlightResources.TestflightSDKLogo), new GUIContent("TestFlight Live! (for App Store)", TestFlightResources.TestflightLiveLogo)}; 
			preferences.userPrefs.testFlightSDKEmbedType = (TestFlightSDKEmbedType)Popup(new GUIContent ("Testflight SDK*", "Integrate the Testflight SDK into this build?"), (int)preferences.userPrefs.testFlightSDKEmbedType, options, 0);
			GUILayout.EndHorizontal();
			
			switch(preferences.userPrefs.testFlightSDKEmbedType)
			{
			case TestFlightSDKEmbedType.DontAddTestflightSDK:
				testFlightDescriptionBoxColor = Color.grey;
				testFlightDescriptionBox = "TestFlight SDK Disabled.\n\n"+testFlightDescriptionBox;
				testFlightDescriptionTexture = null;
				break;
				
			case TestFlightSDKEmbedType.ForTestflightTesters:
				testFlightDescriptionBoxColor = Color.white;
				testFlightDescriptionBox = "TestFlight SDK for testers.\n\nAll features of the Testflight SDK can be used.\nAll session data will be referenced to the tester's TestFlight account.\n\nnote: You CANNOT use this mode of the Testflight SDK when submitting to the app store";
				testFlightDescriptionTexture = TestFlightResources.TestflightSDKLogo;
				break;
				
			case TestFlightSDKEmbedType.ForAppStoreSubmission:
				testFlightDescriptionBoxColor = Color.Lerp(Color.yellow, Color.red, 0.5f);
				testFlightDescriptionBox = "TestFlight SDK for Testflight Live! & App Store.\n\nNearly features of the Testflight SDK can be used,\nquestions and checkpoints can be placed the data for them cannot be collected.\nAll user data is anonymous, but you will still recieve full session and crash data.\n\nIf you submit a build in this mode to the asset store, you can use the Testflight Live! service.";
				testFlightDescriptionTexture = TestFlightResources.TestflightLiveLogo;
				break;
			}
		}
		else
		{
			Color baseColor = GUI.color;
			GUI.color = Color.grey;
			Toggle(new GUIContent("Add Testflight SDK", "You must install the testflight SDK in the preferences window to use this option"), false);
			GUI.color = baseColor;
			testFlightDescriptionBox = "Testflight SDK is not installed.\n\n" + testFlightDescriptionBox; 
		}
		
		if(!narrow)
		{
			GUI.color = testFlightDescriptionBoxColor;
			GUILayout.BeginHorizontal();
			GUILayout.Space(FieldWidth+4);
			GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(false));
			GUI.color = Color.white;
			GUILayout.Label(testFlightDescriptionTexture, GUILayout.ExpandWidth(false));
			GUILayout.Label(testFlightDescriptionBox, GUILayout.ExpandWidth(false));	
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
		}
	}
	
	private static void DrawSubUserSettings_Identity(TestFlightPreferences preferences, bool narrow, string[] availableIdentities, TestFlightMobileProvision[] availableProvisions)
	{
		List<string> identities = new List<string>();
		identities.AddRange(availableIdentities);
		
		GUILayout.BeginHorizontal();
		
		int current = Mathf.Max(0,identities.IndexOf(preferences.userPrefs.developerIdentity));		
		current = Mathf.Max(0, Popup(new GUIContent(narrow ? "Identity":"iOS Codesign Identity",
		                                            "The developer identity certificate to use when building the app.\n\nThis must be set to a 'Distribution' identity for Ad-hoc and App store builds."),
		                             				current, identities.ToArray(), narrow ? 5:0));
		
		if(!narrow)
		{
			GUI.backgroundColor = Color.yellow;
			GUILayout.Label("NOTE: must be set to a 'Distribution' identity for Ad-hoc and App store builds.","textfield");
			GUI.backgroundColor = Color.white;
		}
		
		GUILayout.EndHorizontal();
		
		if(availableIdentities.Contains(identities[current]))
			preferences.userPrefs.developerIdentity = identities[current];
	}

	static void DrawSubUserSettings_BundleId (TestFlightMobileProvision[] availableProvisions, string[] availableIdentities)
	{
		var bundleIdOptions = new List<string>();
		
		bundleIdOptions.Add("com.*");
		
		foreach(var p in availableProvisions)
		{		
			int start = p.AppIdentifier.IndexOf(".");
			
			if(start == -1)
				continue;
			
			var id = p.AppIdentifier.Substring(start+1);
			if(id == "*")
				id = "com.*";
			else if(p.Certificates != null && !System.Array.Exists(availableIdentities, m => p.Certificates.Contains(m)))
				continue;
			
			if(bundleIdOptions.Contains(id))
				continue;
			
			bundleIdOptions.Add(id);
		}
		
		bundleIdOptions.Sort((m,n) => m.Length.CompareTo(n.Length) );
		
		int prevBundle = 0;
		prevBundle = Mathf.Max(0,bundleIdOptions.FindLastIndex(m => PlayerSettings.bundleIdentifier.StartsWith(m.Replace("*",""))));
		
		var newChosenBundle = Popup(new GUIContent("Bundle Id*", "Select/edit your bundle Id from the list of bundle ids that match your provisioning profiles.\n\nYou are only able to select from provisioning profiles that match this bundle id."), prevBundle, bundleIdOptions.ToArray(), 0);
		
		if(bundleIdOptions.Count == 0)
			return;
		
		var newBundleId = bundleIdOptions[newChosenBundle];
		
		if(bundleIdOptions[newChosenBundle].EndsWith("*"))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(FieldWidth+4);
			EditorGUILayout.BeginHorizontal("box");
			
			var appsuffix = PlayerSettings.productName;
			if(PlayerSettings.bundleIdentifier.StartsWith(bundleIdOptions[prevBundle].Replace("*","")))
				appsuffix = PlayerSettings.bundleIdentifier.Remove(0, bundleIdOptions[prevBundle].Replace("*","").Length);
		
			newBundleId = bundleIdOptions[newChosenBundle].Replace("*","");			
			GUILayout.Label(newBundleId, GUILayout.ExpandWidth(false));
			
			newBundleId += GUILayout.TextField(appsuffix, GUILayout.ExpandWidth(false), GUILayout.MinWidth(100)); 
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();
		}
		
		PlayerSettings.bundleIdentifier = newBundleId;
	}
	
	private static void DrawSubUserSettings_Provisions(TestFlightPreferences preferences, bool narrow, TestFlightMobileProvision[] availableProvisions, string[] availableIdentities)
	{
		// Only add options we have identities for
		var identityProvisionOptions = new List<KeyValuePair<string, TestFlightMobileProvision>>();
		foreach(var identity in availableIdentities)
		{	
			foreach(var provision in availableProvisions)
			{
				if(!provision.MatchesBundleId(PlayerSettings.bundleIdentifier))
					continue;
				
				if(provision.Certificates == null)
				{
					identityProvisionOptions.Add(new KeyValuePair<string,TestFlightMobileProvision>("Unverified Identities", provision));
					continue;
				}
					
				if(!System.Array.Exists(provision.Certificates, m => m == identity))
					continue;
			
				identityProvisionOptions.Add(new KeyValuePair<string, TestFlightMobileProvision>(identity, provision));
			}
		}

		// craete a list of strings for the options we have
		var options = new List<string>();
		for(int i=0; i<identityProvisionOptions.Count; ++i)
		{
			var option = identityProvisionOptions[i];
		
			string output = option.Key+"/\n"
							+(option.Value.MatchesBundleId(PlayerSettings.bundleIdentifier)?"":"<Different Bundle Id> ")
							+option.Value.Name;
			
			options.Add(output);
		}

		int chosen = Mathf.Max(0,identityProvisionOptions.FindIndex(delegate(KeyValuePair<string, TestFlightMobileProvision> obj) {return obj.Value.Equals(preferences.userPrefs.customProvisionProfile);}));		
		
		var labelContent = new GUIContent(narrow?"Profile":"Provisioning Profile", "Select a specific iOS provisioning profile to use.\n\nTestflight can only intall your app on devices that are registered in this profile");
		chosen = PopupButton(labelContent, chosen, options.ToArray(), 0);
		
		if(identityProvisionOptions.Count != 0 && identityProvisionOptions[chosen].Value != null)
		{
			preferences.userPrefs.customProvisionProfile = identityProvisionOptions[chosen].Value;
			preferences.userPrefs.developerIdentity = identityProvisionOptions[chosen].Key;
		}
		
		GUI.color = Color.white;
	}
	
	private static void DrawSubUserSettings_XCodeSchema(TestFlightPreferences preferences, bool narrow, TestFlightXCodeSchemas availableSchemas)
	{
//		string[] configs = new string[] {"Release", "Debug"};
		string[] schemes = new string[] {"N/A"};
//		string defaultConfig = preferences.userPrefs.xCodeSchema;
		if(availableSchemas != null)
		{
//			configs = availableSchemas.Configs;
//			defaultConfig = availableSchemas.DefaultConfig;
			
			if(availableSchemas.Schemes.Length > 0) 
				schemes = availableSchemas.Schemes;
		}
		
		/*
		int currentConfig = System.Array.IndexOf(configs, preferences.userPrefs.xCodeConfig);
		if(currentConfig == -1)  currentConfig = Mathf.Max(0, System.Array.IndexOf(configs, defaultConfig));
		int newConfig = Mathf.Max(0, Popup(new GUIContent(narrow ? "Config":"XCode Configuration"), currentConfig, configs, narrow ? 5:0, narrow));
		preferences.userPrefs.xCodeConfig = newConfig < configs.Length ? configs[newConfig]:"";
		*/
	
		int currentScheme = Mathf.Max(0, System.Array.IndexOf(schemes, preferences.userPrefs.xCodeSchema));
		int newScheme = Mathf.Max(0, Popup(new GUIContent(narrow ? "Scheme":"XCode Build Sheme", "The Xcode scheme to build with for users with customized Xcode projects.\n\nThis can be left as N/A or Unity-iPhone if you haven't haven't added extra schemas to your Xcode project"), 
		                                   					currentScheme, schemes, narrow ? 5:0));
		preferences.userPrefs.xCodeSchema = newScheme < schemes.Length ? schemes[newScheme]:"";
	}
		                              
	private static string[] StringToggleField(GUIContent label, string[] possibleStrings, string[] currentStrings, ref bool foldedOut)
	{
		
		foldedOut = EditorGUILayout.Foldout(foldedOut, label);
		if(!foldedOut)
			return currentStrings;
		
		List<string> currentStringList = new List<string>();
		
		EditorGUILayout.BeginVertical();
		
		for(int i=0; i<possibleStrings.Length; ++i)
		{
		
			bool isEnabled = (System.Array.IndexOf(currentStrings, possibleStrings[i]) != -1);
			if(possibleStrings[i].Length > 0 && Toggle(new GUIContent(possibleStrings[i], "Should we send builds to this distribution list?"), isEnabled, 20))
			{
			   currentStringList.Add(possibleStrings[i]);
			}
		}
		EditorGUILayout.EndVertical();
		
		return currentStringList.ToArray();
	}
	
	private static string[] StringArrayField(GUIContent label, string[] strings, ref bool foldedOut, bool canAdd)
	{
		
		foldedOut = EditorGUILayout.Foldout(foldedOut, label);
		if(!foldedOut)
			return strings;

		List<string> stringList = new List<string>(strings);
		
		EditorGUILayout.BeginVertical();
		for(int i=0; i<stringList.Count; ++i)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(FieldWidth+4);
			stringList[i] = GUILayout.TextField(stringList[i]);
			if(GUILayout.Button(new GUIContent("-", "Remove this"), GUILayout.Width(20)))
			{
				stringList.RemoveAt(i);
				break;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		if(canAdd)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(FieldWidth+3);
			if(GUILayout.Button(new GUIContent("+", "Add a new"), GUILayout.Width(20)))
			{
				stringList.Add("New Distribution List");
			}
			
			EditorGUILayout.EndHorizontal();
		}		
		EditorGUILayout.EndVertical();
		
		return stringList.ToArray();
	}
	
	private static int Popup(GUIContent label, int currentOption, string[] options, int indent) 
	{
		var newOptions = new GUIContent[options.Length];
		for(int i=0; i<options.Length; ++i) 
			newOptions[i] = new GUIContent(options[i].Replace('/', '\\'));
		
		return Popup(label, currentOption, newOptions, indent);
	}
	
	private static int Popup(GUIContent label, int currentOption, GUIContent[] options, int indent) 
	{
		int output = 0;

		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
		GUILayout.Space(indent);
		
		GUILayout.Label(label, GUILayout.Width(FieldWidth-indent));
		output = EditorGUILayout.Popup(currentOption, options, "popup", GUILayout.MinWidth(200.0f), GUILayout.ExpandWidth(false)); 
		
		GUILayout.EndHorizontal();
		return output;
	}
	
	private static int PopupButton(GUIContent label, int currentOption, string[] options, int indent)
	{
		int output = 0;

		GUILayout.BeginHorizontal();
		GUILayout.Space(indent);
		
		string[] newOptions = new string[options.Length];
		for(int i=0; i<options.Length; ++i) 
			newOptions[i] = options[i].Replace('/', '\\');

		GUILayout.Label(label, GUILayout.Width(FieldWidth-indent));
		
		GUIContent content = new GUIContent(currentOption < options.Length ? options[currentOption]:"");
		Vector2 size = GUI.skin.GetStyle("button").CalcSize(content);
		size.x = Mathf.Max(size.x, 200);
		
		Rect r = new Rect(GUILayoutUtility.GetLastRect().xMax+4, GUILayoutUtility.GetLastRect().yMin+2.5f, size.x, size.y);
		GUI.Box(r, "","Button");
		
		output = EditorGUI.Popup(r, currentOption, options, "button");
		
		GUILayout.EndHorizontal();
		GUILayout.Space(size.y-10);
		
		return output;
	}
	
	private static System.Enum Enum(GUIContent label, System.Enum currentValue)
	{
		return Enum(label, currentValue, 0);
	}
	
	private static System.Enum Enum(GUIContent label, System.Enum currentValue, int indent)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(indent);
		GUILayout.Label(label, GUILayout.Width(FieldWidth-indent));
		System.Enum output = EditorGUILayout.EnumPopup(currentValue, GUILayout.ExpandWidth(false)); 
		GUILayout.EndHorizontal();
		return output;
	}
	
	private static bool Toggle(GUIContent label, bool currentValue)
	{
		return Toggle(label, currentValue, 0);
	}
	
	private static bool Toggle(GUIContent label, bool currentValue, int indent)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(indent);
		GUILayout.Label(label, GUILayout.Width(FieldWidth-indent));
		bool output = GUILayout.Toggle(currentValue, ""); 
		GUILayout.EndHorizontal();
		return output;
	}
	
	private static string TextField(GUIContent label, string text)
	{
		return TextField(label, text, 0, true);
	}
	
	private static string TextField(GUIContent label, string text, int indent, bool editable)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(indent);
		GUILayout.Label(label, GUILayout.Width(FieldWidth-indent));
		string output = text;

		if(editable)
			output = EditorGUILayout.TextField(new GUIContent(), text, "TextField");
		else 
			GUILayout.Box(text, "TextField");
		
		GUILayout.EndHorizontal();
		return output;
	}
}

