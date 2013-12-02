using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager {
	
	static LevelManager() {
		TextAsset levelData = Resources.Load("levelData") as TextAsset;
		JSONObject jsonObject = new JSONObject(System.Text.RegularExpressions.Regex.Replace(levelData.text, @"\t|\n|\r|\s", ""));
		
		for ( int i = 0; i < jsonObject.list.Count; i++ ) {
			JSONObject levelObject = jsonObject.list[i] as JSONObject;
			
			string levelName = ( levelObject.GetField( "levelName" ) as JSONObject ).str;
			string sceneName = ( levelObject.GetField( "sceneName" ) as JSONObject ).str;
			string thumbPath = ( levelObject.GetField( "thumbPath" ) as JSONObject ).str;
			
			if ( string.IsNullOrEmpty( levelName ) || string.IsNullOrEmpty( sceneName ) || string.IsNullOrEmpty( thumbPath ) ) {
				Debug.LogError("Unable to parse level data file.");
			}
			
			levels.Add( new Level( levelName, sceneName, thumbPath ) );
		}
	}
	
	public class Level {

		public Level( string levelName, string sceneName, string thumbPath ) {
			this.levelName = levelName;
			this.sceneName = sceneName;
			this.thumbPath = thumbPath;
		}
		
		public string levelName;
		public string sceneName;
		public string thumbPath;
	}
	
	private static List<Level> levels = new List<Level>();
	public static List<Level> Levels {
		get { return levels; }
	}
	
	private static Level previusLoadedLevel = null;
	
	public static void LoadMainMenu() {
		previusLoadedLevel = null;
		Application.LoadLevel( "mainMenu" );
	}
	
	public static void LoadLevel( Level level ) {
		Time.timeScale = 1f;
		
		previusLoadedLevel = level;
		Application.LoadLevel( level.sceneName );
	}
	
	public static void RestatLevel() {
		Time.timeScale = 1f;
		
		if ( previusLoadedLevel == null ) {
			Application.LoadLevel( Application.loadedLevel );
			return;
		}
		
		LoadLevel(previusLoadedLevel);
	}
	
	public static Level GetLevel( string levelName ) {

		foreach ( Level level in levels ) {
			if ( level.levelName == levelName ) {
				return level;
			}
		}
		return null;
	}
}
