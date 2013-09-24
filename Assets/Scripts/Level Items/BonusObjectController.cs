using UnityEngine;
using System.Collections;

public class BonusObjectController : MonoBehaviour
{
	void Start() {
		LevelController.Instance.AddBonusObject(this);
	}

	void OnTriggerEnter()
	{
		EventDispatcher.SendEvent(EventKey.PLAYER_BONUS_OBJECT, this);
		Destroy(gameObject);
	}
}
