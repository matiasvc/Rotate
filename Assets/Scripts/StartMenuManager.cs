using UnityEngine;
using System.Collections;

public class StartMenuManager : MonoBehaviour {
	
//	public string[] levels;
	
	private enum MenuState {HIDDEN, MAIN, OPTIONS, ABOUT, LEVEL_SCREEN, SELECTED_LEVEL}
	private MenuState state = MenuState.HIDDEN;
	
	private float buttonHeigth = 50f;
	private float buttonWidth = 150f;
	
	private float titleHeigth = 50f;
	private float titleWidth = 150f;
	private Vector2 titlePos = new Vector2(0f, -200f);
	
	private float levelGridHeigth = 200f;
	private float levelGridWidth = 200f;
	
	private int levelGridRows = 4;
	private int levelGridColloms = 4;
	
	private float levelGridButtonHeigth = 60f;
	private float levelGridButtonWidth = 60f;
	
	private float levelGridTitleHeigth = 20f;
	private float levelGridTitleWidth = 60f;
	
	private LevelManager.Level selectedLevel;
	private Vector2 screenCenter;
	private Grid levelGrid;
	
	private int selected = 0;
	private bool selectedPress = false;
	
	protected void Awake() {
		screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
		state = MenuState.MAIN;
	}
	
	void Update() {
		if( OuyaInput.GetButtonDown( OuyaButton.O, OuyaPlayer.P01 ) ) {
			selectedPress = true;
		}
		
		if( OuyaInput.GetButtonDown( OuyaButton.DU, OuyaPlayer.P01 ) || OuyaInput.GetButtonDown( OuyaButton.DL, OuyaPlayer.P01 ) ) {
			selected--;
		}
		
		if( OuyaInput.GetButtonDown( OuyaButton.DD, OuyaPlayer.P01 ) || OuyaInput.GetButtonDown( OuyaButton.DR, OuyaPlayer.P01 ) ) {
			selected++;
		}
	}
	
	protected void OnGUI() {
		
		switch ( state ) {
		case MenuState.MAIN:
			GUI.Label( new Rect( screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - titleHeigth + titlePos.y, titleWidth, titleHeigth ), "MAIN MENU" );
			
			if ( selected > 2 ) { selected = 0; }
			if ( selected < 0 ) { selected = 2; }
			
			if ( GUI.Button( new Rect( screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f - buttonHeigth, buttonWidth, buttonHeigth), selected == 0 ? "X PLAY" : "PLAY" ) || selected == 0 && selectedPress ) {
				selected = 0;
				state = MenuState.LEVEL_SCREEN;
			}
			
			if ( GUI.Button( new Rect( screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f, buttonWidth, buttonHeigth), selected == 1 ? "X OPTIONS" : "OPTIONS" ) || selected == 1 && selectedPress ) {
				selected = 0;
				state = MenuState.OPTIONS;
			}
			
			if ( GUI.Button( new Rect( screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f + buttonHeigth, buttonWidth, buttonHeigth), selected == 2 ? "X ABOUT" : "ABOUT" ) || selected == 2 && selectedPress ) {
				selected = 0;
				state = MenuState.ABOUT;
			}
			
			break;
		case MenuState.OPTIONS:
			
			if ( selected > 0 ) { selected = 0; }
			if ( selected < 0 ) { selected = 0; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - titleHeigth + titlePos.y, titleWidth, titleHeigth), "OPTIONS");
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f + buttonHeigth, buttonWidth, buttonHeigth), selected == 0 ? "X BACK" : "BACK" )  || selected == 0 && selectedPress ) {
				selected = 0;
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.ABOUT:
			
			if ( selected > 0 ) { selected = 0; }
			if ( selected < 0 ) { selected = 0; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - titleHeigth + titlePos.y, titleWidth, titleHeigth), "ABOUT");
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f + buttonHeigth, buttonWidth, buttonHeigth), selected == 0 ? "X BACK" : "BACK" )  || selected == 0 && selectedPress ) {
				selected = 0;
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.LEVEL_SCREEN:
			
			if ( selected > LevelManager.Levels.Count ) { selected = 0; }
			if ( selected < 0 ) { selected = LevelManager.Levels.Count; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - titleHeigth + titlePos.y, titleWidth, titleHeigth), "LEVEL SCREEN");
			if (levelGrid == null) { // Initialize grid
				levelGrid = new Grid(screenCenter.x - levelGridWidth / 2, screenCenter.y - levelGridHeigth / 2, levelGridWidth, levelGridHeigth, levelGridColloms, levelGridRows);
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f + buttonHeigth + 30f, buttonWidth, buttonHeigth), selected == 0 ? "X BACK" : "BACK" )  || selected == 0 && selectedPress ) {
				selected = 0;
				state = MenuState.MAIN;
			}
			
			int i = 0;
			foreach(LevelManager.Level level in LevelManager.Levels) {
				
				Vector2 buttonPos = levelGrid.GetNodePostion(i);
				if(GUI.Button(new Rect(buttonPos.x - levelGridButtonWidth / 2f, buttonPos.y - levelGridButtonHeigth / 2f, levelGridButtonWidth, levelGridButtonHeigth), selected == i+1 ? "X "+(i+1).ToString() : (i+1).ToString() ) || selected == i+1 && selectedPress ) {
					selectedLevel = level;
					selected = 0;
					state = MenuState.SELECTED_LEVEL;
				}
				GUI.Label(new Rect(buttonPos.x - levelGridTitleWidth / 2f, buttonPos.y + levelGridTitleHeigth / 2f + levelGridTitleHeigth, levelGridTitleWidth, levelGridTitleHeigth), level.levelName);
				i++;
			}
			
			break;
		case MenuState.SELECTED_LEVEL:
			
			if ( selected > 1 ) { selected = 0; }
			if ( selected < 0 ) { selected = 1; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - titleHeigth + titlePos.y, titleWidth, titleHeigth), "SELECTED LEVEL SCREEN" );
			GUI.Label(new Rect(screenCenter.x - titleWidth + titlePos.x/ 2f, screenCenter.y - 5f + titlePos.y, titleWidth, titleHeigth), selectedLevel.levelName);
			
			if( GUI.Button( new Rect( screenCenter.x, screenCenter.y, buttonWidth, buttonHeigth), selected == 1 ? "X PLAY" : "PLAY") || selected == 1 && selectedPress ) {
				LevelManager.LoadLevel( selectedLevel );
			}
			
			if( GUI.Button( new Rect( screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f + buttonHeigth + 30f, buttonWidth, buttonHeigth ), selected == 0 ? "X BACK" : "BACK" )  || selected == 0 && selectedPress ) {
				selected = 0;
				state = MenuState.LEVEL_SCREEN;
			}
			break;
		}
		
		selectedPress = false;
	}
}
