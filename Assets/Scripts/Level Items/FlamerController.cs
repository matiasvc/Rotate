using UnityEngine;
using System.Collections;

public class FlamerController : LevelItem {
	
	[SerializeField]
	private float fireLength;
	
	private BoxCollider flameCollider = null;
	private ParticleEmitter flameParticleEmitter = null;
	
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
	
	public ParticleEmitter FlameParticleEmitter
	{
		get
		{
			if (flameParticleEmitter == null)
				flameParticleEmitter = gameObject.GetComponentInChildren<ParticleEmitter>();
			
			return flameParticleEmitter;
		}
	}
	
	public void SetFlameLength(float length)
	{
		fireLength = length;
		
		Vector3 oldColliderSize = FlameCollider.size;
		Vector3 oldColliderPos = FlameCollider.center;
		
		Vector3 newColliderPos = new Vector3((fireLength * 0.5f ) + 1f , oldColliderPos.y, oldColliderPos.z);
		Vector3 newColliderSize = new Vector3(fireLength, oldColliderSize.y, oldColliderSize.z);
		
		float particleSpeed = FlameParticleEmitter.localVelocity.x;
		
		FlameParticleEmitter.maxEnergy = (fireLength + 0.5f) / particleSpeed;
		FlameParticleEmitter.minEnergy = (fireLength - 0.5f) / particleSpeed;
		
		FlameCollider.center = newColliderPos;
		FlameCollider.size = newColliderSize;
	}
	
}
