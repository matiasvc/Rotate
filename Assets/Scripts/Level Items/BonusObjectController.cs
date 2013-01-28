using UnityEngine;
using System.Collections;

public class BonusObjectController : MonoBehaviour
{	
	void OnTriggerEnter()
	{
		EventDispatcher.SendEvent(EventKey.PLAYER_BONUS_OBJECT);
		Destroy(gameObject);
	}
}
