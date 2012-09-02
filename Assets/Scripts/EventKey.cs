using UnityEngine;
using System.Collections;

public static class EventKey
{
	public const string PLAYER_SPAWN = 			"PLAYER_SPAWN";				// Called when the player should respawn
	public const string PLAYER_DESTROY =		"PLAYER_DESTROY";			// Called when the player should be destroyed
	public const string PLAYER_SETCHECKPOINT =	"PLAYER_SETCHECKPOINT";		// Called when the player reaches a checkpoint
	public const string PLAYER_TOGGLEACTIVE =	"PLAYER_TOGGLEACTIVE";		// Called to toggle the player on and off
	public const string PLAYER_MOVE =			"PLAYER_MOVE";				// Called to move the player to a position
	public const string PLAYER_BONUSOBJECT =	"PLAYER_BONUSOBJECT";		// Cakked when the player picks up a bonus object
	
	public const string GAME_SETROTATION =		"GAME_SETROTATION";			// Called when the rotation of the level should be changed
	public const string GAME_LEVELCOMPLETE = 	"GAME_LEVELCOMPLETE";		// Called when the level has been completed
	
	public const string INPUT_PAUSE = 			"INPUT_PAUSE";				// Called when the pause button has been pressed
	public const string INPUT_ROTATE =			"INPUT_ROTATE";				// Called when the rotate input has been pressed
}
