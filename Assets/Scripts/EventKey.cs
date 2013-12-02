using UnityEngine;
using System.Collections;

public static class EventKey
{
	public const string PLAYER_SPAWN = 			"PLAYER_SPAWN";				// Respawn the player
	public const string PLAYER_DESTROY =		"PLAYER_DESTROY";			// Destroy the player
	public const string PLAYER_SET_CHECKPOINT =	"PLAYER_SET_CHECKPOINT";	// Set checkpoint
	public const string PLAYER_TOGGLE_ACTIVE =	"PLAYER_TOGGLE_ACTIVE";		// Toggle the player on and off
	public const string PLAYER_MOVE =			"PLAYER_MOVE";				// Instantly move the player to a position
	public const string PLAYER_BONUS_OBJECT =	"PLAYER_BONUSOBJECT";		// Player picked up a bonus object
	public const string PLAYER_APPLY_FORCE =	"PLAYER_APPLY_FORCE";		// Apply a physics force to the player rigidbody [Vector3 forceDirection, ForceMode forceMode]
	
	public const string GAME_SET_ROTATION =		"GAME_SET_ROTATION";		// Sets level rotation
	public const string GAME_LEVEL_COMPLETE = 	"GAME_LEVEL_COMPLETE";		// Level completed
	public const string GAME_SHOW_COMPLETE_MENU = "GAME_SHOW_COMPLETE_MENU";// Show the end of level menu
	public const string GAME_ENABLE_GOAL =      "GAME_ENABLE_GOAL";         // Enables the goal in the level
	
	public const string INPUT_PAUSE = 			"INPUT_PAUSE";				// Pause button was pressed
	public const string INPUT_ROTATE =			"INPUT_ROTATE";				// Rotate button was pressed
}
