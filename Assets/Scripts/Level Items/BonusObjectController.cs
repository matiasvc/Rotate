using UnityEngine;
using System.Collections;

public class BonusObjectController : MonoBehaviour {

	public Transform meshTransform;
	private Transform playerTransform;

	void Start() {
		LevelController.Instance.AddBonusObject( this );
		playerTransform = LevelController.Instance.PlayerTransform;
	}

	void OnTriggerEnter() {
		EventDispatcher.SendEvent( EventKey.PLAYER_BONUS_OBJECT, this );
		gameObject.SetActive( false );
	}

	void LateUpdate() {
		Vector2 playerDirection = VectorEx.Vec3ToVec2( playerTransform.position - transform.position ).normalized;
		float playerDistance = Vector3.Distance( playerTransform.position, transform.position );
		Vector2 newPos = playerDirection * Easing.easeInExpo( 0f, 0.50f, Mathf.InverseLerp(4f, 1f, playerDistance ) );
		meshTransform.localPosition = VectorEx.Vec2ToVec3(newPos);
	}
}
