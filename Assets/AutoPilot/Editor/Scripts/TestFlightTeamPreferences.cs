
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
[System.Serializable]
public class TestFlightTeamPreferences
{	
	public string 				teamAPIKey;
	public string[]			 	distributionLists;
	public string				buildPath;
	
	public string				OutputPath { get {return buildPath+"/autopilot/"; } }
	
	public TestFlightTeamPreferences()
	{
		buildPath = "builds/iPhone";
		teamAPIKey = "";
		distributionLists = new string[] {"Devs", "QA"};
	}
	
	public void Save()
	{
		string path = "./TestFlightTeamPreferences.xml";
		System.IO.Stream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestFlightTeamPreferences));
		xmlSerializer.Serialize(fileStream, this);
		fileStream.Close();
	}
	
	public static TestFlightTeamPreferences Load()
	{
		string path = "./TestFlightTeamPreferences.xml";
		System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
		if(!fileInfo.Exists)
		{
			return new TestFlightTeamPreferences();
		}
		
		System.IO.Stream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestFlightTeamPreferences));
		TestFlightTeamPreferences preferences = (TestFlightTeamPreferences)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		
		return preferences;
	}
}
