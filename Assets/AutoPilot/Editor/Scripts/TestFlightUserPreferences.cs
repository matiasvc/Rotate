using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public enum TestFlightSDKEmbedType
{
	DontAddTestflightSDK,
	ForTestflightTesters,
	ForAppStoreSubmission
}

[System.Serializable]
public class TestFlightUserPreferences
{	
	public string 				userAPIKey;
	public string[]				activeDistributionLists;
	public bool 				replaceSameBuilds;
	public bool					notifyUsers;
	public float				lastBuildAppTime;
	public float 				lastPackageIpaTime;
	public float				lastUploadTime;
	public string[]				messageHistory;
	public TestFlightSDKEmbedType testFlightSDKEmbedType;
	public TestFlightMobileProvision customProvisionProfile;
	//public string				xCodeConfig;
	public string 				xCodeSchema;
	public bool					dSYMBuild;
	public string 				developerIdentity;
	public string				ipaBuildPath;
	
	public TestFlightUserPreferences()
	{
		userAPIKey = "";
		activeDistributionLists = new string[] {"Devs", "QA"};
		replaceSameBuilds = true;
		notifyUsers = true;
		lastBuildAppTime = 30;
		lastPackageIpaTime = 5;
		lastUploadTime = 60;
		messageHistory = new string[0];
		testFlightSDKEmbedType = TestFlightSDKEmbedType.ForTestflightTesters;
		customProvisionProfile = null;
		//xCodeConfig = "";
		xCodeSchema = "";
		dSYMBuild = true;
		developerIdentity = "iPhone Developer";
		ipaBuildPath = FigureOutProjectName()+".ipa";
	}
	
	private static string FigureOutProjectName()
	{
		return new System.IO.DirectoryInfo(Application.dataPath).Parent.Name;
	}
	
	public bool UsingTestFlightSDK
	{
		get 
		{
			return testFlightSDKEmbedType != TestFlightSDKEmbedType.DontAddTestflightSDK;
		}
	}
	
	public string DistrobutionListsCSV 
	{	
		get 
		{
			string outputString = "";
			for(int i=0; i<activeDistributionLists.Length; ++i)
			{
				outputString += activeDistributionLists[i] + (i<activeDistributionLists.Length?",":"");
			}
			return outputString;
		}
	}
			        
	public void Save()
	{
		string path = "./TestFlightUserPreferences.xml";
		System.IO.Stream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestFlightUserPreferences));
		xmlSerializer.Serialize(fileStream, this);
		fileStream.Close();
	}
	
	public static TestFlightUserPreferences Load()
	{
		string path = "./TestFlightUserPreferences.xml";
		System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
		if(!fileInfo.Exists)
		{
			return new TestFlightUserPreferences();
		}
		
		System.IO.Stream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestFlightUserPreferences));
		TestFlightUserPreferences preferences = (TestFlightUserPreferences)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		
		return preferences;
	}
}

