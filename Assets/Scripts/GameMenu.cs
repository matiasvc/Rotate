using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {
	
	private enum MenuState {HIDDEN, MAIN, MAIN_MENU_CHECK, RESTART_CHECK, LEVEL_COMPLETE}
	private MenuState state = MenuState.HIDDEN;
	
	private float buttonHeigth = 50f;
	private float buttonWidth = 150f;
	
	private float titleHeigth = 50f;
	private float titleWidth = 200f;
	
	private Vector2 screenCenter;
	
	private int selected = 0;
	private bool selectedPress = false;
	private bool menuPress = false;
	
	protected void OnEnable() {
		EventDispatcher.AddHandler(EventKey.INPUT_PAUSE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_SHOW_COMPLETE_MENU, HandleEvent);
	}
	
	protected void OnDisable() {
		EventDispatcher.RemoveHandler(EventKey.INPUT_PAUSE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_SHOW_COMPLETE_MENU, HandleEvent);
	}
	
	protected void Start() {
		screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		if(eventName == EventKey.INPUT_PAUSE)
		{
			Toggle();
		}
		else if (eventName == EventKey.GAME_SHOW_COMPLETE_MENU)
		{
			state = MenuState.LEVEL_COMPLETE;
		}
	}
	
	void Update() {
		
		if( OuyaInput.GetButtonDown( OuyaButton.O, OuyaPlayer.P01 ) ) {
			selectedPress = true;
		}
		
		if( OuyaInput.GetButtonDown( OuyaButton.U, OuyaPlayer.P01 ) ) {
			menuPress = true;
		}
		
		if( OuyaInput.GetButtonDown( OuyaButton.DU, OuyaPlayer.P01 ) || OuyaInput.GetButtonDown( OuyaButton.DL, OuyaPlayer.P01 ) ) {
			selected++;
		}
		
		if( OuyaInput.GetButtonDown( OuyaButton.DD, OuyaPlayer.P01 ) || OuyaInput.GetButtonDown( OuyaButton.DR, OuyaPlayer.P01 ) ) {
			selected--;
		}
	}
	
	private void Toggle() {

		if (state == MenuState.HIDDEN) {
			state = MenuState.MAIN;
			LevelController.CurrentGameState = LevelController.GameState.PAUSED;
		} else {
			state = MenuState.HIDDEN;
			LevelController.CurrentGameState = LevelController.GameState.PLAYING;
		}
			
	}
	
	private void Show() {
		state = MenuState.MAIN;
		LevelController.CurrentGameState = LevelController.GameState.PAUSED;
	}
	
	private void Hide() {
		state = MenuState.HIDDEN;
		LevelController.CurrentGameState = LevelController.GameState.PLAYING;
	}
	
	
	protected void OnGUI() {
		if(GUI.Button(new Rect(3f, 3f, 50f, 50f), "M") || menuPress ) {
				Toggle();
		}
		
		switch(state) {
			
		case MenuState.HIDDEN:
			break;
		case MenuState.MAIN:
			
			if ( selected < 0 ) { selected = 2; }
			if ( selected > 2 ) { selected = 0; }
			
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2, buttonWidth, buttonHeigth), selected == 2 ? "--RESTART--" : "RESTART" ) || selected == 2 && selectedPress ) {
				state = MenuState.RESTART_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth / 2, buttonWidth, buttonHeigth), selected == 1 ? "--MAIN MENU--" : "MAIN MENU" ) || selected == 1 && selectedPress ) {
				state = MenuState.MAIN_MENU_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth * 1.5f, buttonWidth, buttonHeigth), selected == 0 ? "--RETURN--" : "RETURN" ) || selected == 0 && selectedPress ) {
				state = MenuState.HIDDEN;
				LevelController.CurrentGameState = LevelController.GameState.PLAYING;
			}
			
			break;
		case MenuState.RESTART_CHECK:
			
			if ( selected < 0 ) { selected = 1; }
			if ( selected > 1 ) { selected = 0; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Are you shure you wish to restart the level.");
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth, screenCenter.y, buttonWidth, buttonHeigth), selected == 1 ? "--YES--" : "YES") || selected == 1 && selectedPress ) {
				state = MenuState.HIDDEN;
				LevelManager.RestatLevel();
			}
			
			if(GUI.Button(new Rect(screenCenter.x, screenCenter.y, buttonWidth, buttonHeigth), selected == 0 ? "--NO--" : "NO") || selected == 0 && selectedPress ) {
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.MAIN_MENU_CHECK:
			
			if ( selected < 0 ) { selected = 1; }
			if ( selected > 1 ) { selected = 0; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Are you shure you wish to return to the Main Menu.");
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth, screenCenter.y, buttonWidth, buttonHeigth), selected == 1 ? "--YES--" : "YES") || selected == 1 && selectedPress ) {
				state = MenuState.HIDDEN;
				LevelManager.LoadMainMenu();
			}
			
			if(GUI.Button(new Rect(screenCenter.x, screenCenter.y, buttonWidth, buttonHeigth), selected == 0 ? "--NO--" : "NO") || selected == 0 && selectedPress ) {
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.LEVEL_COMPLETE:
			
			if ( selected < 0 ) { selected = 2; }
			if ( selected > 2 ) { selected = 0; }
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Level complete.");

			if ( LevelController.CollectedAllBonusObjects ) {
				GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 200f, titleWidth, titleHeigth), "Level time: " + LevelController.LevelTimeString );
			} else {
				GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 200f, titleWidth, titleHeigth), "Collect all energy orbs to get your level time on the high score" );
			}

			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2f, buttonWidth, buttonHeigth), selected == 2 ? "--RESTART--" : "RESTART" ) ) {
				state = MenuState.RESTART_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth / 2f, buttonWidth, buttonHeigth), selected == 1 ? "--MAIN MENU--" : "MAIN MENU" ) ) {
				state = MenuState.MAIN_MENU_CHECK;
			}

			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth * 1.5f, buttonWidth, buttonHeigth), selected == 0 ? "--NEXT LEVEL--" : "NEXT LEVEL" ) ) {
				LevelManager.LoadNextLevel();
			}
			
			break;
		}
		
		selectedPress = false;
		menuPress = false;
	}
}
