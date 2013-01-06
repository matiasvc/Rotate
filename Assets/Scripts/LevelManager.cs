using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager {
	
	public struct Level
	{
		public Level(string levelName, string sceneName, string thumbPath, int starScore1, int starScore2, int starScore3)
		{
			this.levelName = levelName;
			this.sceneName = sceneName;
			this.thumbPath = thumbPath;
			this.starScore1 = starScore1;
			this.starScore2 = starScore2;
			this.starScore3 = starScore3;
		}
		
		public string levelName;
		public string sceneName;
		public string thumbPath;
		public int starScore1;
		public int starScore2;
		public int starScore3;
	}
	
	private static List<Level> levels = new List<Level>();
	public static List<Level> Levels
	{
		get { return levels; }
	}
	
	static LevelManager()
	{
		TextAsset levelData = Resources.Load("levelData") as TextAsset;
		JSONObject jsonObject = new JSONObject(System.Text.RegularExpressions.Regex.Replace(levelData.text, @"\t|\n|\r|\s", ""));
		
		for (int i = 0; i < jsonObject.list.Count; i++)
		{
			JSONObject levelObject = jsonObject.list[i] as JSONObject;
			
			string levelName = (levelObject.list[0] as JSONObject).str;
			string sceneName = (levelObject.list[1] as JSONObject).str;
			string thumbPath = (levelObject.list[2] as JSONObject).str;
			int starScore1 = (int)(levelObject.list[3] as JSONObject).n;
			int starScore2 = (int)(levelObject.list[4] as JSONObject).n;
			int starScore3 = (int)(levelObject.list[5] as JSONObject).n;
			
			if (string.IsNullOrEmpty(levelName) || string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(thumbPath))
				Debug.LogError("Unable to parse level data file.");
			
			levels.Add(new Level(levelName, sceneName, thumbPath, starScore1, starScore2, starScore3));
			
		}
		
		Debug.Log(levels.Count);
	}
	
}
