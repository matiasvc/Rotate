using UnityEngine;
using System.Collections;

public class FanController : LevelItem {
	
	public float airflowLength = 1f;
	public float fanForce = 20f;
	
	public ParticleSystem airflowParticles;
	public FanAnimator fanAnimator;
	public BoxCollider fanTrigger;
	
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
			EventDispatcher.SendEvent(EventKey.PLAYER_APPLY_FORCE, new object[]{transform.forward * fanForce, ForceMode.Force}, false);
	}
	
	private void ToogleFan(bool setTo)
	{
		
	}
}
