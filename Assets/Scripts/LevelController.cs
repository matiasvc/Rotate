using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	
	public static LevelController Instance;
	
	private Vector2 respawnPoint;
	
	public Vector3 RespawnPoint
	{
		get
		{
			return new Vector3(respawnPoint.x, 0f, respawnPoint.y);
		}
		set
		{
			respawnPoint = new Vector2(value.x, value.z);
		}
	}
	
	protected void Awake()
	{
		Instance = this;
	}
	
}
