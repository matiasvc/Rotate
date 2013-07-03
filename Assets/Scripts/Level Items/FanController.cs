using UnityEngine;
using System.Collections;

public class FanController : LevelItem {
	
	public float airflowLength = 1f;
	public float fanForce = 20f;
	
	
	public ParticleSystem airflowParticles
	{
		get
		{
			if ( _airflowParticles == null )
			{
				_airflowParticles = gameObject.GetComponentInChildren<ParticleSystem>();
			}
			return _airflowParticles;
		}
	}
	public FanAnimator fanAnimator
	{
		get
		{
			if ( _fanAnimator == null )
			{
				_fanAnimator = gameObject.GetComponentInChildren<FanAnimator>();
			}
			return _fanAnimator;
		}
	}
	public BoxCollider fanTrigger
	{
		get
		{
			if ( _fanTrigger == null )
			{
				_fanTrigger = gameObject.GetComponentInChildren<BoxCollider>();
			}
			return _fanTrigger;
		}
	}
	
	// Property caching variables
	public ParticleSystem _airflowParticles = null;
	public FanAnimator _fanAnimator = null;
	public BoxCollider _fanTrigger = null;
	
	private bool playerInTrigger = false;
	
	void Start()
	{
		fanAnimator.fanEnabled = itemEnabled;
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemEnable () {
		base.ItemEnable ();
		fanAnimator.fanEnabled = itemEnabled;
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemDisable () {
		base.ItemDisable ();
		fanAnimator.fanEnabled = itemEnabled;
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemSwitch (bool setTo) {
		base.ItemSwitch (setTo);
		fanAnimator.fanEnabled = itemEnabled;
		airflowParticles.enableEmission = itemEnabled;
	}
	
	public override void ItemToggle () {
		base.ItemToggle ();
		fanAnimator.fanEnabled = itemEnabled;
		airflowParticles.enableEmission = itemEnabled;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
			playerInTrigger = true;
	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
			playerInTrigger = false;
	}
	
	void FixedUpdate()
	{
		if (itemEnabled && playerInTrigger)
			EventDispatcher.SendEvent( EventKey.PLAYER_APPLY_FORCE, new object[]{transform.forward * fanForce, ForceMode.Force} );
	}
	
	private void ToogleFan(bool setTo)
	{
		
	}
}
