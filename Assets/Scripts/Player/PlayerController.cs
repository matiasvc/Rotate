using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Transform goalIndicator;
	public Transform goalIndicatorMesh;

	private Rigidbody playerRigidbody;
	private MeshRenderer[] meshRenderers;
	private Light playerLigth;

	private GameObject goalObject;

	void Awake() {
		playerRigidbody = gameObject.GetComponent<Rigidbody>();
		meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		playerLigth = gameObject.GetComponentInChildren<Light>();
	}

	void Start() {
		goalObject = LevelController.GoalObject;
		if ( goalObject == null) { goalIndicator.gameObject.SetActive(false); }
	}
	
	void OnEnable() {
		EventDispatcher.AddHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_DESTROY, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_TOGGLE_ACTIVE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_MOVE, HandleEvent);
		EventDispatcher.AddHandler(EventKey.PLAYER_APPLY_FORCE, HandleEvent);
	}
	
	void OnDisable() {
		EventDispatcher.RemoveHandler(EventKey.GAME_SET_ROTATION, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_DESTROY, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_TOGGLE_ACTIVE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_MOVE, HandleEvent);
		EventDispatcher.RemoveHandler(EventKey.PLAYER_APPLY_FORCE, HandleEvent);
	}

	void Update() {
		if ( goalObject != null ) {
			Vector3 goalRelativePosition = goalObject.transform.position - transform.position;
			float goalDirection = Mathf.Atan2( goalRelativePosition.z, goalRelativePosition.x ) * Mathf.Rad2Deg;
			
			goalIndicator.eulerAngles = new Vector3( 0.0f, -goalDirection + 90.0f, 0.0f );

			float indicatorSize = Mathf.InverseLerp(2.2f, 7f, goalRelativePosition.magnitude );
			goalIndicatorMesh.localScale = Vector3.one * indicatorSize;
		}
	}

	private void HandleEvent(string eventName, object param) {
		switch (eventName) {
		case EventKey.GAME_SET_ROTATION:
			gameObject.rigidbody.WakeUp();
			break;
		case EventKey.PLAYER_DESTROY:
			Death();
			break;
		case EventKey.PLAYER_TOGGLE_ACTIVE:
			ToggleActive((bool)param);
			break;
		case EventKey.PLAYER_MOVE:
			Move((Vector3)param);
			break;
		case EventKey.PLAYER_APPLY_FORCE:
			object[] paramArray = (object[])param;
			ApplyForce((Vector3)paramArray[0], (ForceMode)paramArray[1]);
			break;
		default:
			Debug.LogWarning("No handler for this event implemented.");
			break;
		}
	}
	
	private void ToggleActive(bool toggle) {
		if (!playerRigidbody.isKinematic)
			playerRigidbody.velocity = Vector3.zero;
		
		playerRigidbody.isKinematic = !toggle;
		
		foreach (MeshRenderer meshRenderer in meshRenderers)
			meshRenderer.enabled = toggle;
		
		playerLigth.enabled = toggle;
	}
	
	private void Move(Vector3 moveTo) {
		transform.position = moveTo;
	}
	
	private void Death() {
		ToggleActive(false);
		// Insert death animation here
		
		EventDispatcher.SendEvent(EventKey.PLAYER_SPAWN);
	}
	
	private void ApplyForce(Vector3 forceVector, ForceMode forceMode) {
		rigidbody.AddForce(new Vector3(forceVector.x, 0f, forceVector.z), forceMode);
	}
}
