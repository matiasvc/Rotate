using UnityEngine;
using System.Collections;

public class FlamerController : LevelItem {
	
	[SerializeField]
	private float fireLength;
	
	private BoxCollider flameCollider = null;
	private ParticleSystem flameParticles = null;
	
	private float enabledFlameLength = 0f;
	
	public float FireLength {
		get { return fireLength; }
	}
	
	public BoxCollider FlameCollider {
		get {
			if (flameCollider == null)
				flameCollider = gameObject.GetComponent<BoxCollider>();
			
			return flameCollider;
		}
	}
	
	public ParticleSystem FlameParticleEmitter {
		get {
			if (flameParticles == null)
				flameParticles = gameObject.GetComponentInChildren<ParticleSystem>();
			
			return flameParticles;
		}
	}
	
	void Awake() {
		flameParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		enabledFlameLength = fireLength;
	}
	
	void Start() {
		if (itemEnabled) {
			FlameParticleEmitter.Play();
		}
	}
	
	public override void ItemToggle () {
		base.ItemToggle ();
		
		if (itemEnabled) {
			StartCoroutine(ToggleCoroutine(true));
		} else {
			StartCoroutine(ToggleCoroutine(false));
		}
	}
	
	public IEnumerator ToggleCoroutine(bool toggleTo) {
		float length = 0.3f;
		float fromLength;
		float toLength;
		float t = 0f;
		
		if ( toggleTo) {
			FlameParticleEmitter.enableEmission = true;
			fromLength = 0f;
			toLength = enabledFlameLength;
		} else {
			fromLength = enabledFlameLength;
			toLength = 0f;
		}
		
		
		
		while ( t <= length ) {
			float lerp = Mathf.InverseLerp(0f, length, t);
			SetFlameLength(Mathf.Lerp(fromLength, toLength, lerp));
			yield return null;
			t += Time.deltaTime;
		}
		
		SetFlameLength(toLength);
		
		if ( !toggleTo ) {
			FlameParticleEmitter.enableEmission = false;
		}
	}
	
	public void SetFlameLength(float length) {
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
