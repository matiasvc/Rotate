using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public enum GameState {PLAYING, NO_INPUT, PAUSED};
	
	private static GameState gameState;
	
	public static GameState CurrentGameState
	{
		get { return gameState; }
		
		set
		{
			switch(value)
			{
			case GameState.PLAYING:
				Time.timeScale = 1f;
				break;
			case GameState.NO_INPUT:
				
				break;
			case GameState.PAUSED:
				Time.timeScale = 0f;
				break;
			}
			
			gameState = value;
			Debug.Log("Changing Game State to: " + gameState);
		}
	}
	
	
	protected void Awake()
	{
		
	}
	
}
