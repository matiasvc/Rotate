using UnityEngine;
using System.Collections;

public class FanController : LevelItem {
	
	public float airflowLength = 1f;
	public float fanForce = 20f;
	
	public Transform bladeTransform;
	
	public ParticleSystem _airflowParticles = null;
	public ParticleSystem airflowParticles {
		get {
			if ( _airflowParticles == null ) { _airflowParticles = gameObject.GetComponentInChildren<ParticleSystem>(); }
			return _airflowParticles;
		}
	}
	
	public BoxCollider _fanTrigger = null;
	public BoxCollider fanTrigger {
		get {
			if ( _fanTrigger == null ) { _fanTrigger = gameObject.GetComponentInChildren<BoxCollider>(); }
			return _fanTrigger;
		}
	}
	
	private bool fanEnabled = false;
	private float fanEnabledSpeed = 360f * 3f;
	private float dampening = 0.1f;
	private float motorForce = 360 * 3f;
	private float fanSpeed = 0f;
	private float fanRotation = 0f;
	
	private bool playerInTrigger = false;
	
	void Start() {
		if ( itemEnabled ) {
			fanSpeed = fanEnabledSpeed;
		}
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemEnable () {
		base.ItemEnable ();
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemDisable () {
		base.ItemDisable ();
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemSwitch (bool setTo) {
		base.ItemSwitch (setTo);
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemToggle () {
		base.ItemToggle ();
		airflowParticles.enableEmission = itemEnabled;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			playerInTrigger = true;
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			playerInTrigger = false;
		}
	}
	
	void Update () {
		fanRotation -= fanSpeed * Time.deltaTime;
		fanRotation = Mathf.Repeat(fanRotation, 360f);
		
		if ( itemEnabled ) {
			fanSpeed += motorForce * Time.deltaTime;
		} else {
			fanSpeed *= Mathf.Pow( dampening, Time.deltaTime );
		}
		
		fanSpeed = Mathf.Clamp(fanSpeed, 0f, fanEnabledSpeed);
		bladeTransform.localEulerAngles = new Vector3(0f, 0f, fanRotation);
	}
	
	void FixedUpdate()
	{
		if (itemEnabled && playerInTrigger) {
			EventDispatcher.SendEvent( EventKey.PLAYER_APPLY_FORCE, new object[]{transform.forward * fanForce, ForceMode.Force} );
		}
	}
	
	
}
