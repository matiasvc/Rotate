using UnityEngine;
using System.Collections;

public class GoalController : LevelItem {
	
	private bool playerInTrigger = false;
	private bool playerReachedGoal = false;
	private GameObject player;
	
	private float atractionRadius = 4f;
	private float atractionForce = 50f;
	private float goalRadius = 0.6f;
	
	private ParticleSystem particles;
	private Animation anim;
	
	private const string animName = "open";
	
	void Awake ()
	{
		particles = gameObject.GetComponentInChildren<ParticleSystem>();
		anim = gameObject.GetComponentInChildren<Animation>();
	}
	
	void Start ()
	{
		particles.Stop();
	}

	void OnEnable () {
		EventDispatcher.AddHandler( EventKey.GAME_ENABLE_GOAL, HandleEvent );
	}

	void OnDisable () {
		EventDispatcher.RemoveHandler( EventKey.GAME_ENABLE_GOAL, HandleEvent );
	}

	private void HandleEvent(string eventName, object param) {
		switch(eventName) {
		case EventKey.GAME_ENABLE_GOAL:
			ItemEnable();
			if ( playerInTrigger ) {
				EnableSuction();
			}
			break;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if ( other.gameObject.tag == "Player" ) {
			playerInTrigger = true;
			player = other.gameObject;
			if ( itemEnabled ) {
				EnableSuction();
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if ( other.gameObject.tag == "Player" ) {
			playerInTrigger = false;
			player = null;
			if ( itemEnabled ) {
				DisableSuction ();
			}
		}
	}
	
	void FixedUpdate()
	{
		if (playerInTrigger && !playerReachedGoal)
		{
			float distance = Vector3.Distance(player.transform.position, transform.position);
			
			if (distance > goalRadius)
			{
				Vector3 pullVector = (transform.position - player.transform.position).normalized;
				
				float pullLerp = Mathf.InverseLerp(atractionRadius, 0f, distance);
				pullVector *= Easing.easeInExpo( atractionForce * 0.25f, atractionForce, pullLerp );
				
				player.rigidbody.AddForce(pullVector, ForceMode.Force);	
			}
			else
			{
				player.rigidbody.isKinematic = true;
				playerReachedGoal = true;
				StartCoroutine(PlayerExitAnimation(player.rigidbody.velocity.magnitude, Vector3.Distance(player.transform.position, transform.position)));
			}
		}
	}
	
	private IEnumerator PlayerExitAnimation (float velocity, float distance)
	{
		
		float t = 0;
		float length = (distance / velocity) * 2.0f;
		
		Vector2 startPos = new Vector2(player.transform.position.x, player.transform.position.z);
		Vector2 goalPos = new Vector2(transform.position.x, transform.position.z);
		
		while (t <= length) {
			float xyLerp = Easing.easing(Easing.EasingType.linear, 0f, 1f, Mathf.InverseLerp(0f, length, t));
			Vector2 xyPos = Vector2.Lerp(startPos, goalPos, xyLerp);
			
			float zLerp = Easing.easing(Easing.EasingType.easeOutCirc, 0f, 1f, Mathf.InverseLerp(0f, length, t));
			float zPos = Mathf.Lerp(0f, -1.5f, zLerp);
			
			player.transform.position = new Vector3(xyPos.x, zPos, xyPos.y);
			
			yield return null;
			t += Time.deltaTime;
		}
		
		particles.Stop();
		
		anim[animName].speed = -1f;
		anim[animName].normalizedTime = 1f;
		anim.Play();
		
		while(anim.isPlaying)
			yield return null;
		
		EventDispatcher.SendEvent(EventKey.GAME_LEVEL_COMPLETE);
	}

	void OnDrawGizmosSelected () {
		Color oldColor = Gizmos.color;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, goalRadius);
		Gizmos.color = oldColor;
	}
	
	
	void EnableSuction () {
		particles.Play ();
		anim [animName].speed = 1f;
		anim [animName].normalizedTime = 0f;
		anim.Play ();
	}

	void DisableSuction () {
		particles.Stop ();
		anim [animName].speed = -1f;
		anim [animName].normalizedTime = 1f;
		anim.Play ();
	}
}
