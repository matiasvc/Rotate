using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class TestFlightPreferences
{
	public TestFlightTeamPreferences teamPrefs;
	public TestFlightUserPreferences userPrefs;
	
	public static TestFlightPreferences Load()
	{
		TestFlightPreferences prefs = new TestFlightPreferences();
		prefs.teamPrefs = TestFlightTeamPreferences.Load();
		prefs.userPrefs = TestFlightUserPreferences.Load();
		return prefs;
	}
	
	public void Save()
	{
		teamPrefs.Save();
		userPrefs.Save();
	}
}