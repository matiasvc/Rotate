using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TestFlightBuildPipeline
{	
	private static bool isUpdatingProject=false;
	private static bool menuItemAllowed=true;
		
	public static bool IsUpdatingProject { get { return isUpdatingProject; } }
	
	public static TestFlightBuildCallback onPreBuild;
	
	public static bool HasPro()
	{
		return UnityEditorInternal.InternalEditorUtility.HasPro();
	}
	
	public static bool MenuItemAllowed 
	{
		get {return menuItemAllowed;}
		set {menuItemAllowed = value;}
	}

	public static bool CanBuild(TestFlightPreferences preferences, TestFlightMobileProvision[] availableProvisions, string[] availableIdentities)
	{
		if(preferences.userPrefs.userAPIKey.Length==0)
			return false;
		
		if(preferences.teamPrefs.teamAPIKey.Length==0)
			return false;
		
		if(System.Array.IndexOf(availableProvisions, preferences.userPrefs.customProvisionProfile) == -1)
			return false;
		
		if(System.Array.IndexOf(availableIdentities, preferences.userPrefs.developerIdentity) == -1)
			return false;
		
		if(!TestFlightBuildPipeline.MenuItemAllowed)
			return false;
		
		return true;
	}

	private static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		
		foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if(e==null)
				continue;
			
			if(e.enabled)
				names.Add(e.path);
		}
		return names.ToArray();
	}
	
	[UnityEditor.MenuItem("File/AutoPilot: Build and Package as IPA", true)]
	static bool ValidateBuildAndPackageIPA()
	{
		if(Application.platform != RuntimePlatform.OSXEditor || !menuItemAllowed)
			return false;
		
		return true;
	}
	
	[UnityEditor.MenuItem("File/AutoPilot: Build and Package as IPA")]
	public static void BuildAndPackageIPA()
	{
		TestFlightBuildIPAWindow.CreateWindow();	
	}
	
	[UnityEditor.MenuItem("File/AutoPilot: Build and Upload to Testflight &%b", true)]
	static bool ValidateBuildAndUploadToTestFlight()
	{
		if(Application.platform != RuntimePlatform.OSXEditor || !menuItemAllowed)
			return false;
		
		TestFlightPreferences preferences = TestFlightPreferences.Load();
		return (preferences.teamPrefs.teamAPIKey.Length > 0 && preferences.userPrefs.userAPIKey.Length > 0);
	}
	
	/// <summary>
	/// Display the message window and then begin the build & upload process
	/// </summary>
	[UnityEditor.MenuItem("File/AutoPilot: Build and Upload to Testflight &%b")]
	public static void BuildAndUploadToTestFlight()
	{
		TestFlightMessageWindow.CreateWindow();
	}
	
	public static void BuildAndUploadToTestFlight(string message)
	{
		if(HasPro())
			TestFlightBuildWindow.DoBuild(message, TestFlightPreferences.Load());
		else 
			TestFlightNonProBuildStep.DoBuild(message);
	}
	
	public static bool IsTestFlightSDKInstalled()
	{
		return 	Directory.Exists("Assets/AutoPilot/Plugins/iOS") 
			    && File.Exists("Assets/AutoPilot/Plugins/iOS/TestFlight.h") 
				&& File.Exists("Assets/AutoPilot/Plugins/iOS/libTestFlight.a");
	}
	
	public static void UpdateSDKEnabledInBuild()
	{
		TestFlightPreferences preferences = TestFlightPreferences.Load();
		bool enabled = IsTestFlightSDKInstalled() && preferences.userPrefs.UsingTestFlightSDK;
		
		string[] files = Directory.GetFiles("Assets", "TestFlightSDK.cs", System.IO.SearchOption.AllDirectories);
		if(files.Length == 0)
			return;
		
		string path = files[0];
		
		StreamReader sReader = new StreamReader(path);
		string output = sReader.ReadToEnd();
		sReader.Close();
		
		string pattern = "(?://|)(#define TESTFLIGHT_SDK_ENABLED)";		
		output = (enabled ? "#define TESTFLIGHT_SDK_ENABLED":"") + Regex.Replace(output, pattern, "");
		
		StreamWriter sWriter = new StreamWriter(path, false);
		sWriter.Write(output);
		sWriter.Close();
		
		// if we're enabled, copy the native code to the Plugins/iOS folder
		if(enabled)
		{
			Directory.CreateDirectory("Assets/Plugins/iOS");		
			File.Copy("Assets/AutoPilot/Plugins/iOS/libTestFlight.a", "Assets/Plugins/iOS/libTestFlight.a",true);
			File.Copy("Assets/AutoPilot/Plugins/iOS/TestFlight.h", "Assets/Plugins/iOS/TestFlight.h",true);
			File.Copy("Assets/AutoPilot/Plugins/iOS/TestFlightSDKExport.mm", "Assets/Plugins/iOS/TestFlightSDKExport.mm",true);
		}
		else
		{
			AssetDatabase.DeleteAsset("Assets/Plugins/iOS/libTestFlight.a");
			AssetDatabase.DeleteAsset("Assets/Plugins/iOS/TestFlight.h");
			AssetDatabase.DeleteAsset("Assets/Plugins/iOS/TestFlightSDKExport.mm");
		}
		AssetDatabase.Refresh();
	}
	
	private static void ApplyTestflightSDKSettingToXcodeProject(TestFlightPreferences preferences)
	{
		bool enabled = IsTestFlightSDKInstalled() && preferences.userPrefs.UsingTestFlightSDK;
			
		// print a warning if the user or team token are empty
		if(enabled && preferences.userPrefs.userAPIKey.Length==0)
			Debug.LogWarning("Testflight SDK integration enabled, but no Testflight user API token set up in \"File->Autopilot Tetflight Build Preferences\". Testflight SDK will not be able to give data from this build.");
	
		if(enabled && preferences.teamPrefs.teamAPIKey.Length==0)
			Debug.LogWarning("Testflight SDK integration enabled, but no Testflight team API token set up in \"File->Autopilot Tetflight Build Preferences\". Testflight SDK will not be able to give data from this build.");
		
		// inject the TestFlight SDK
		string path = Path.GetFullPath(preferences.teamPrefs.buildPath)+"/Classes/AppController.mm";
		StreamReader sReader = new StreamReader(path);
		string output = sReader.ReadToEnd();
		sReader.Close();
		
		// 1. include the wrapper in AppController.mm
		string includeSubStr = "#include \"../Libraries/TestFlight.h\"\n";
		if(enabled)
		{
			if(!output.Contains(includeSubStr))
				output = includeSubStr + output;
		}
		else 
		{
			output = output.Replace(includeSubStr, "");
		}
		
		// 2. initialize the testflight sdk at application launch
		string testFlightLaunchPattern = @"(applicationDidFinishLaunching\:\(UIApplication\*\)application\s*\{)(?:.|\n)*TestFlight_TakeOff\(\);\n";
		if(enabled)
		{
			if(!Regex.IsMatch(output, testFlightLaunchPattern))
				output = Regex.Replace(output, @"(applicationDidFinishLaunching\:\(UIApplication\*\)application\s*\{)", "$1\n\tTestFlight_TakeOff();\n");
		}
		else 
		{
			output = Regex.Replace(output, testFlightLaunchPattern, "$1");
		}
		
		// 2.5 do the same for version 3.5's new launch function
		// didFinishLaunchingWithOptions:(NSDictionary*)launchOptions { tTestFlight_TakeOff();
		testFlightLaunchPattern = @"(didFinishLaunchingWithOptions\:\(NSDictionary\*\)launchOptions\s*{)(?:.|\n)*TestFlight_TakeOff\(\);\n";
		if(enabled)
		{
			if(!Regex.IsMatch(output, testFlightLaunchPattern))
				output = Regex.Replace(output, @"(didFinishLaunchingWithOptions\:\(NSDictionary\*\)launchOptions\s*\n{)", "$1\n\tTestFlight_TakeOff();\n");
		}
		else 
		{
			output = Regex.Replace(output, testFlightLaunchPattern, "$1");
		}
		
		string takeOffFunction = "";
		if(preferences.userPrefs.testFlightSDKEmbedType == TestFlightSDKEmbedType.ForTestflightTesters)
		{
			takeOffFunction += "\t[TestFlight setDeviceIdentifier:[[UIDevice currentDevice] uniqueIdentifier]];\n";
		}
		takeOffFunction += "\t[TestFlight takeOff:@\""+preferences.teamPrefs.teamAPIKey+"\"];\n"; 
				
		// 3. insert/replace the wrapper functions			
		string functions = "// BEGIN AUTOPILOT IMPLMENETATION - automatically generated code, edit at your own risk.\n" +
							"extern \"C\" void TestFlight_TakeOff()\n" +
						    "{\n" +
						    takeOffFunction +
							"}\n" +
						    "// END AUTOPILOT IMPLMENETATION\n";
		
		string functionPattern = "// BEGIN AUTOPILOT IMPLMENETATION(?:.|\n)*// END AUTOPILOT IMPLMENETATION\n";
		if(Regex.IsMatch(output, functionPattern))
		{
			output = Regex.Replace(output, functionPattern, enabled ? functions:"");
		}
		else if(enabled)
		{
			output += 	"\n\n"+functions;
		}
		
		// 4. insert/replace prototypes
		string prototypes = "// BEGIN AUTOPILOT PROTOTYPES - automatically generated code, edit at your own risk.\n" +
							"extern \"C\" void TestFlight_TakeOff();\n" +
							"// END AUTOPILOT PROTOTYPES\n";

		string prototypesPattern = "// BEGIN AUTOPILOT PROTOTYPES(?:.|\n)*// END AUTOPILOT PROTOTYPES\n";
		if(Regex.IsMatch(output, prototypesPattern))
		{
			output = Regex.Replace(output, prototypesPattern, enabled ? prototypes:"");
		}
		else if(enabled)
		{
			output = Regex.Replace(output, "(// --- OpenGLES .*)", prototypes+"\n$1");
		}
			
		StreamWriter sWriter = new StreamWriter(path, false);
		sWriter.Write(output);
		sWriter.Close();
	}
	
	public static void InstallSDKFrom(string path)
	{
		Directory.CreateDirectory("Assets/AutoPilot/Plugins/iOS/");
		
		File.Copy(path+"/TestFlight.h", "Assets/AutoPilot/Plugins/iOS/TestFlight.h", true);
		File.Copy(path+"/libTestFlight.a", "Assets/AutoPilot/Plugins/iOS/libTestFlight.a", true);
	}
	
	public static void PreBuildPlayer(TestFlightPreferences preferences)
	{
		// If testflight isn't installed... but it's in the Plugins/iOS folder. Install it from there.
		if(!IsTestFlightSDKInstalled() && HasPro() && 
		   (File.Exists("Assets/Plugins/iOS/TestFlight.h")
		    || File.Exists("Assets/Plugins/iOS/libTestFlight.a") ) )
		{
			InstallSDKFrom("Assets/Plugins/iOS");
		}

		// enable the SDK if in use 
		UpdateSDKEnabledInBuild();
	}
	
	[UnityEditor.Callbacks.PostProcessBuildAttribute]
	public static void PostBuildPlayerCallback(BuildTarget target, string pathToBuiltProject)
	{
//		TestFlightSDKSplashScreen.ShowSplashScreen();
		if(target == BuildTarget.iPhone)
		{
			TestFlightPreferences preferences = TestFlightPreferences.Load();
			preferences.teamPrefs.buildPath = pathToBuiltProject;
			PostBuildPlayer(preferences, true);
		}
		
		TestFlightNonProBuildStep.TriggerBuildIfWaiting(pathToBuiltProject);
	}
	
	public static void PostBuildPlayer(TestFlightPreferences preferences, bool buildResult)
	{
		// Tell them we're using the SDK
		if(IsTestFlightSDKInstalled() && preferences.userPrefs.testFlightSDKEmbedType != TestFlightSDKEmbedType.DontAddTestflightSDK)  
		{
			string type = "";
			if(preferences.userPrefs.testFlightSDKEmbedType == TestFlightSDKEmbedType.ForTestflightTesters)
				type =  "TestFlight Testers";
			else 
				type = "TestFlight Live! & App Store submission.";
			
			for(int i=0; i<100; ++i)
			{
				EditorUtility.DisplayProgressBar("Embedding Testflight SDK!", "Configuring Xcode project for "+ type, (float)i/100);
				System.Threading.Thread.Sleep(1);
			}
			System.Threading.Thread.Sleep(300);
			EditorUtility.ClearProgressBar();
		}
		
		// if we're running pro, inject the SDK
		ApplyTestflightSDKSettingToXcodeProject(preferences);
	}
	
	public static bool BuildPlayerIOS (TestFlightPreferences preferences)
	{
		PreBuildPlayer(preferences);
		
		bool buildResult = BuildPlayer(BuildTarget.iPhone, preferences.teamPrefs.buildPath, false);
		
		return buildResult;
	}
	
	public static void Abort()
	{
		TestFlightBuildWindow.AbortBuild();
	}
	
	private static bool BuildPlayer(BuildTarget target, string path, bool logging)
	{
		string errorStr = "";
		string fullpath = System.IO.Path.GetFullPath(path);
		if(logging)	Debug.Log("Starting Build: "+target.ToString()+"\n------------------\n------------------");
		
		string[] scenes = GetBuildScenes();
		if(scenes == null || scenes.Length==0 || path == null)
		{
			Debug.LogError("AutoPilot: Build failed, no scenes to build! Please add your scenes to Build Settings.");
			return false;
		}
		
		if(logging)	Debug.Log(string.Format("Path: \"{0}\"", path));
		for(int i=0; i<scenes.Length; ++i)
		{
			if(logging)	Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
		}		

		isUpdatingProject = true;
		Directory.CreateDirectory(fullpath);
		if( Directory.Exists(fullpath+"/Unity-iPhone.xcodeproj/") )
		{
			Debug.Log("AutoPilot: Existing xcode project found: appending");
			errorStr = BuildPipeline.BuildPlayer(scenes, System.IO.Path.GetFullPath(path), target, BuildOptions.AcceptExternalModificationsToPlayer);
		}
		else
		{
			Debug.Log("AutoPilot: No existing xcode project: building clean");
			errorStr = BuildPipeline.BuildPlayer(scenes, System.IO.Path.GetFullPath(path), target, BuildOptions.None);
		}
		
		isUpdatingProject = false;
		return errorStr.Length == 0;
	}
	
	public static System.Diagnostics.Process BuildApp(TestFlightPreferences preferences)
	{
		TestFlightXCodeSchemas schmeas = TestFlightXCodeSchemas.Enumerate(preferences.teamPrefs.buildPath);
		if(schmeas == null || schmeas.Schemes == null || System.Array.IndexOf(schmeas.Schemes, preferences.userPrefs.xCodeSchema) == -1)
			preferences.userPrefs.xCodeSchema = "";
		
		if(Directory.Exists(preferences.teamPrefs.OutputPath))
			Directory.Delete(preferences.teamPrefs.OutputPath, true);
			
		Directory.CreateDirectory(preferences.teamPrefs.OutputPath);
		
		string buildPath = preferences.teamPrefs.buildPath;
		string extraArgs = "";
		
		/*
		if(preferences.userPrefs.xCodeConfig.Length > 0)
			extraArgs += " -configuration "+preferences.userPrefs.xCodeConfig;
		*/
		
		if(preferences.userPrefs.xCodeSchema.Length > 0)
			extraArgs += " -scheme "+preferences.userPrefs.xCodeSchema;

		if(preferences.userPrefs.customProvisionProfile != null && preferences.userPrefs.customProvisionProfile.UUID != "")
		{
			extraArgs += " PROVISIONING_PROFILE=\""+preferences.userPrefs.customProvisionProfile.UUID+"\"";
		}
		
		if(preferences.userPrefs.dSYMBuild)
		{
			extraArgs += " DEBUG_INFORMATION_FORMAT=dwarf-with-dsym";
			extraArgs += " GCC_GENERATE_DEBUGGING_SYMBOLS=YES";
		}
		
		if(preferences.userPrefs.developerIdentity.Length > 0 && preferences.userPrefs.developerIdentity!="iPhone Developer")
		{
			extraArgs += " CODE_SIGN_IDENTITY=\""+preferences.userPrefs.developerIdentity+"\"";
		}
		
		extraArgs += " CONFIGURATION_BUILD_DIR=\""+Path.GetFullPath(preferences.teamPrefs.OutputPath)+"\"";
		string args = "clean build -project Unity-iPhone.xcodeproj"+extraArgs;
		Debug.Log("AutoPilot building .app: xcodebuild "+args);
		return StartProcess("xcodebuild", args, true, Path.GetFullPath(buildPath));
	}
	
	private static string FindBuiltAppName(string buildPath)
	{
		DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(buildPath));		
		
		if(di.Exists)
		{
			DirectoryInfo[] fis = di.GetDirectories("*.app");
			if(fis.Length>0)
				return fis[0].FullName;
		}
		
		return "MISSING";
	}
	
	private static string FindBuiltDSYMName(string buildPath)
	{
		DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(buildPath));		
		
		if(di.Exists)
		{
			DirectoryInfo[] fis = di.GetDirectories("*.dSYM");
			if(fis.Length>0)
				return fis[0].FullName;
		}
		
		return "MISSING";
	}

	private static string FindBuiltIPAName(string buildPath)
	{
		DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(buildPath));
		
		if(di.Exists)
		{
			FileInfo[] fis = di.GetFiles("*.ipa");
			if(fis.Length>0)
				return fis[0].FullName;
		}
		
		return "MISSING";
	}	
	
	public static System.Diagnostics.Process BuildIPA(TestFlightPreferences preferences)
	{
		return BuildIPA(preferences, "");
	}
	
	public static System.Diagnostics.Process BuildIPA(TestFlightPreferences preferences, string forceOutputPath)
	{		
		// 
		// FIXME: strip invalid characters from the product name
		//
		string searchPath = preferences.teamPrefs.OutputPath;
		string appPath = FindBuiltAppName(searchPath);
		
		while(!Directory.Exists(appPath))
		{
			string s = string.Format("Autopilot couldn't find your built .app at the following path:\n{0}\n\nPlease locate the .app to continue.\n\nContact the developer (cratesmith@cratesmith.com) if this happens often.", searchPath);
			if(!EditorUtility.DisplayDialog("AutoPilot: Whoops!", s, "Find file...", "Abort"))
			{
				Debug.Log("AutoPilot: User aborted");
				return null;
			}
			
			searchPath = EditorUtility.OpenFolderPanel("Locate folder containing your .app", preferences.teamPrefs.buildPath, ".app");
			
			if(searchPath.Length == 0)
			{
				Debug.Log("AutoPilot: User aborted");
				return null;
			}
			
			appPath = FindBuiltAppName(searchPath);
		}
		
		string outputPath = "";
		if(forceOutputPath.Length > 0)
		{
			new FileInfo(forceOutputPath).Directory.Create();
			outputPath = forceOutputPath;
		}
		else 
		{
			Directory.CreateDirectory(preferences.teamPrefs.OutputPath);
			outputPath = Path.GetFullPath(preferences.teamPrefs.OutputPath+Path.GetFileNameWithoutExtension(appPath)+".ipa");
		}
		
		// delete the IPA if it exists
		System.IO.FileInfo fi = new System.IO.FileInfo(outputPath);
		if(fi.Exists)
		{
			fi.Delete();
		}
		
		// build
		string filename = "xcrun";
		string args     = "-sdk iphoneos"
				 	    +" PackageApplication"
					    + " -v '"+appPath+"'"
						+ " -o '"+Path.GetFullPath(outputPath)+"'";
		
		Debug.Log("Autopilot packaging IPA: "+filename+" "+args);
		return StartProcess(filename, args, false);
	}
	
	public static string ZipDSYMTo(TestFlightPreferences preferences, string outputPath)
	{
		string dSymPath = FindBuiltDSYMName(preferences.teamPrefs.OutputPath);
		if(dSymPath != "MISSING" && preferences.userPrefs.dSYMBuild)
		{
			Debug.Log("AutoPilot: dSYM found, zipping for upload");	
			string zipName = outputPath + new DirectoryInfo(dSymPath).Name + ".zip";
			
			if(File.Exists(zipName))
				File.Delete(zipName);
			
			System.Diagnostics.Process zipProcess = System.Diagnostics.Process.Start("zip", string.Format("\"{0}\" \"{1}\" -r -9", zipName, dSymPath));
			zipProcess.WaitForExit();
			
			return zipName;
		}
		
		return "MISSING";
	}
	
	public static System.Diagnostics.Process TestFlightUpload(string message, TestFlightPreferences preferences)
	{		
		// 
		// FIXME: strip invalid characters from the product name
		//
		string ipaPath = FindBuiltIPAName(preferences.teamPrefs.OutputPath);
		string zipName = ZipDSYMTo(preferences, preferences.teamPrefs.OutputPath);
		
		//
		// FIXME: the message needs escape sequences for newlines
		//
		StreamWriter fs = new StreamWriter("TestFlightMessage.txt", false);
		fs.Write(message);
		fs.Close();

		while(!File.Exists(ipaPath))
		{
			string s = string.Format("Autopilot couldn't find your packaged .ipa at the following path:\n{0}\n\nPlease locate the .ipa to continue.\n\nContact the developer (cratesmith@cratesmith.com) if this happens often.", ipaPath);
			if(!EditorUtility.DisplayDialog("AutoPilot: Whoops!", s, "Find file...", "Abort"))
			{
				Debug.Log("AutoPilot: User aborted");
				return null;
			}
			
			ipaPath = EditorUtility.OpenFilePanel("Locate folder containing your .ipa", preferences.teamPrefs.buildPath, ".ipa");
			
			if(ipaPath.Length == 0)
			{
				Debug.Log("AutoPilot: User aborted");
				return null;
			}
		}

		string filename = "curl";
		string args = "https://testflightapp.com/api/builds.json"
		  			+ " -F file=@'"+ipaPath+"'"
		  			+ " -F api_token='"+preferences.userPrefs.userAPIKey+"'"
		  			+ " -F team_token='"+preferences.teamPrefs.teamAPIKey+"'"
		  			+ " -F notes=<'TestFlightMessage.txt'"
		  			+ " -F notify="+(preferences.userPrefs.notifyUsers?"True":"False")
		 		 	+ " -F distribution_lists='"+preferences.userPrefs.DistrobutionListsCSV+"'"
					+ " -F replace="+(preferences.userPrefs.replaceSameBuilds?"True":"False");
		
		if(zipName != "MISSING" && preferences.userPrefs.dSYMBuild)
		{
			Debug.Log("AutoPilot: dSYM found, zipping for upload");
			args += " -F dsym=@'"+zipName+"'";
		}
		
		args += " --silent"
				+ " --write-out '\n%{http_code}'"
				+ " --show-error";
		
		Debug.Log("Autopilot uploading: "+filename+" "+args);
		return StartProcess(filename, args, true);
	}
	
	public static void PrintProcessOutput(System.Diagnostics.Process process)
	{
		if(!process.HasExited)
			process.WaitForExit();
		
		if(process.StartInfo.RedirectStandardOutput)
		{
			while(!process.StandardOutput.EndOfStream)
				Debug.Log(process.StandardOutput.ReadLine());		
		}
			
		if(process.StartInfo.RedirectStandardError)
		{
			while(!process.StandardError.EndOfStream)
				Debug.LogWarning(process.StandardError.ReadLine());		
		}	
	}
	
	public static System.Diagnostics.Process StartProcess(string filename, string arguments, bool logStdOut)
	{
		return StartProcess(filename,arguments,logStdOut, ".");
	}
	
	private static System.Diagnostics.Process StartProcess(string filename, string arguments, bool logStdOut, string workingpath)
	{
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(filename, arguments);
		startInfo.RedirectStandardError 	= true; 
		startInfo.RedirectStandardOutput 	= logStdOut;
		startInfo.CreateNoWindow 			= false;
		startInfo.ErrorDialog 				= true;
		startInfo.UseShellExecute 			= false;
		startInfo.WorkingDirectory			= workingpath;
		System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);
		return process;
	}
}
