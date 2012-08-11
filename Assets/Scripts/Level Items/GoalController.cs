using UnityEngine;
using System.Collections;

public class GoalController : LevelItem {
	
	private bool playerInTrigger = false;
	private bool playerReachedGoal = false;
	private GameObject player;
	
	private float atractionRadius = 3f;
	private float atractionForce = 30f;
	private float goalRadius = 0.3f;
	
	private ParticleSystem particles;
	private Animation anim;
	
	private const string animName = "open";
	
	void Awake()
	{
		particles = gameObject.GetComponentInChildren<ParticleSystem>();
		anim = gameObject.GetComponentInChildren<Animation>();
	}
	
	void Start()
	{
		particles.Stop();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerInTrigger = true;
			player = other.gameObject;
			particles.Play();
			
			anim[animName].speed = 1f;
			anim[animName].normalizedTime = 0f;
			anim.Play();
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			playerInTrigger = false;
			player = null;
			particles.Stop();
			
			anim[animName].speed = -1f;
			anim[animName].normalizedTime = 1f;
			anim.Play();
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
				pullVector *= Mathf.Lerp(0f, atractionForce, pullLerp);
				
				player.rigidbody.AddForce(pullVector, ForceMode.Force);	
			}
			else
			{
				player.rigidbody.isKinematic = true;
				playerReachedGoal = true;
				EventDispatcher.SendEvent(EventKey.GAME_LEVELCOMPLETE);
			}
		}
	}
	
}
