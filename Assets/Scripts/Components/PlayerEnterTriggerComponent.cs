using UnityEngine;
using System.Collections;

public class PlayerEnterTriggerComponent : MonoBehaviour {
	
	protected void OnTriggerEnter(Collider other)
	{
		EventDispatcher.SendEvent("PLAYER_ENTER_TRIGGER", other);
	}
	
}
