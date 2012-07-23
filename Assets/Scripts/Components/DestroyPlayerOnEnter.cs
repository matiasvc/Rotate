using UnityEngine;
using System.Collections;

public class DestroyPlayerOnEnter : MonoBehaviour {
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			EventDispatcher.SendEvent(EventKey.PLAYER_DESTROY);
		}
	}
	
}
