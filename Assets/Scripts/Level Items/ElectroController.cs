using UnityEngine;
using System.Collections;

public class ElectroController : LevelItem {
	
	public ElectroController connectedElectro = null;
	public ElectricBeamAnimator beamAnimator;
	
	public Vector3 BeamPoint {
		get { return transform.position + transform.rotation * new Vector3(0.85f, 0f, 0.5f); }
	}
	
	public override void ItemEnable () {
		base.ItemEnable();
	}
	
	public override void ItemDisable () {
		base.ItemDisable();
	}
	
	public override void ItemSwitch ( bool setTo ) {
		base.ItemSwitch( setTo );
	}
	
	public override void ItemToggle () {
		base.ItemToggle();
	}
	
	void OnDrawGizmos() {
		if ( connectedElectro != null ) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(BeamPoint, connectedElectro.BeamPoint);
		}
	}
	
}
