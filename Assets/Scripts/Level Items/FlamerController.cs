using UnityEngine;
using System.Collections;

public class FlamerController : LevelItem {
	
	[SerializeField]
	private float fireLength;
	
	private BoxCollider flameCollider = null;
	private ParticleSystem flameParticles = null;
	
	public float FireLength
	{
		get { return fireLength; }
	}
	
	public BoxCollider FlameCollider
	{
		get
		{
			if (flameCollider == null)
				flameCollider = gameObject.GetComponent<BoxCollider>();
			
			return flameCollider;
		}
	}
	
	public ParticleSystem FlameParticleEmitter
	{
		get
		{
			if (flameParticles == null)
				flameParticles = gameObject.GetComponentInChildren<ParticleSystem>();
			
			return flameParticles;
		}
	}
	
	void Awake()
	{
		flameParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		Debug.Log(flameParticles);
	}
	
	void Start()
	{
		if (itemEnabled)
			FlameParticleEmitter.Play();
	}
	
	public void SetFlameLength(float length)
	{
		fireLength = length;
		
		Vector3 oldColliderSize = FlameCollider.size;
		Vector3 oldColliderPos = FlameCollider.center;
		
		Vector3 newColliderPos = new Vector3((fireLength * 0.5f ) + 1f , oldColliderPos.y, oldColliderPos.z);
		Vector3 newColliderSize = new Vector3(fireLength, oldColliderSize.y, oldColliderSize.z);
		
		float particleSpeed = FlameParticleEmitter.startSpeed;
		
		FlameParticleEmitter.startLifetime = fireLength / particleSpeed;
		
		FlameCollider.center = newColliderPos;
		FlameCollider.size = newColliderSize;
	}
	
}
