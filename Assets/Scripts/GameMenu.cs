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
	
	protected void OnEnable()
	{
		EventDispatcher.AddHandler(EventKey.INPUT_PAUSE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.GAME_LEVELCOMPLETE, HandleEvent);
	}
	
	protected void OnDisable()
	{
		EventDispatcher.RemoveHandler(EventKey.INPUT_PAUSE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.GAME_LEVELCOMPLETE, HandleEvent);
	}
	
	protected void Start()
	{
		screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}
	
	private void HandleEvent(string eventName, object param)
	{
		if(eventName == EventKey.INPUT_PAUSE)
		{
			Toggle();
		}
		else if (eventName == EventKey.GAME_LEVELCOMPLETE)
		{
			state = MenuState.LEVEL_COMPLETE;
		}
	}
	
	private void Toggle()
	{
		if (state == MenuState.HIDDEN)
		{
			state = MenuState.MAIN;
			GameManager.CurrentGameState = GameManager.GameState.PAUSED;
		}
		else
		{
			state = MenuState.HIDDEN;
			GameManager.CurrentGameState = GameManager.GameState.PLAYING;
		}
			
	}
	
	private void Show()
	{
		state = MenuState.MAIN;
		GameManager.CurrentGameState = GameManager.GameState.PAUSED;
	}
	
	private void Hide()
	{
		state = MenuState.HIDDEN;
		GameManager.CurrentGameState = GameManager.GameState.PLAYING;
	}
	
	
	protected void OnGUI()
	{
		if(GUI.Button(new Rect(3f, 3f, 50f, 50f), "M"))
		{
			Toggle();
		}
		
		switch(state)
		{
		case MenuState.HIDDEN:
			
			break;
		case MenuState.MAIN:
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2, buttonWidth, buttonHeigth), "RESTART"))
			{
				state = MenuState.RESTART_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth / 2, buttonWidth, buttonHeigth), "MAIN MENU"))
			{
				state = MenuState.MAIN_MENU_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth * 1.5f, buttonWidth, buttonHeigth), "RETURN"))
			{
				state = MenuState.HIDDEN;
				GameManager.CurrentGameState = GameManager.GameState.PLAYING;
			}
			
			break;
		case MenuState.RESTART_CHECK:
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Are you shure you wish to restart the level.");
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth, screenCenter.y, buttonWidth, buttonHeigth), "YES"))
			{
				state = MenuState.HIDDEN;
				GameManager.CurrentGameState = GameManager.GameState.PLAYING;
				Application.LoadLevel(Application.loadedLevel);
			}
			
			if(GUI.Button(new Rect(screenCenter.x, screenCenter.y, buttonWidth, buttonHeigth), "NO"))
			{
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.MAIN_MENU_CHECK:
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Are you shure you wish to return to the Main Menu.");
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth, screenCenter.y, buttonWidth, buttonHeigth), "YES"))
			{
				state = MenuState.HIDDEN;
				GameManager.CurrentGameState = GameManager.GameState.PLAYING;
				Application.LoadLevel("mainMenu");
			}
			
			if(GUI.Button(new Rect(screenCenter.x, screenCenter.y, buttonWidth, buttonHeigth), "NO"))
			{
				state = MenuState.MAIN;
			}
			
			break;
		case MenuState.LEVEL_COMPLETE:
			
			GUI.Label(new Rect(screenCenter.x - titleWidth / 2f, screenCenter.y - 100f, titleWidth, titleHeigth), "Level complete.");
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y - buttonHeigth / 2, buttonWidth, buttonHeigth), "RESTART"))
			{
				state = MenuState.RESTART_CHECK;
			}
			
			if(GUI.Button(new Rect(screenCenter.x - buttonWidth / 2f, screenCenter.y + buttonHeigth / 2, buttonWidth, buttonHeigth), "MAIN MENU"))
			{
				state = MenuState.MAIN_MENU_CHECK;
			}
			
			break;
		}
	}
}
