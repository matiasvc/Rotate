using UnityEngine;
using System.Collections;

public static class EventKey {
	
	public const string PLAYER_RESPAWN = 		"PLAYER_RESPAWN";			// Called when the player should respawn
	public const string PLAYER_DESTROY =		"PLAYER_DESTROY";			// Called when the player should be destroyed
	
	public const string GAME_SETROTATION =		"GAME_SETROTATION";			// Called when the rotation of the level has been changed
	public const string GAME_LEVELCOMPLETE = 	"GAME_LEVELCOMPLETE";		// Called when the level has been completed
	
	public const string INPUT_PAUSE = 			"INPUT_PAUSE";				// Called when the pause button has been pressed
	public const string INPUT_ROTATE =			"INPUT_ROTATE";				// Called when the rotate input has been pressed
}
