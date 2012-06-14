using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {
	
	public GameObject playerPrefab;
	
	
	protected void Start()
	{
		GameObject playerGo;
		
		playerGo = GameObject.Instantiate(playerPrefab, transform.position, Quaternion.identity) as GameObject;
	}
}
