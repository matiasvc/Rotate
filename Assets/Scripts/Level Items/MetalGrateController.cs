using UnityEngine;
using System.Collections;

public class MetalGrateController : LevelItem {
	
	public Transform fanMesh;
	public Transform fanSmoke;
	private float fanSpeed = 75.0f;
	
	public override void ItemEnable ()
	{
		base.ItemEnable ();
	}
	
	public override void ItemDisable ()
	{
		base.ItemDisable ();
	}
	
	public override void ItemSwitch (bool setTo)
	{
		base.ItemSwitch (setTo);
	}
	
	public override void ItemToggle ()
	{
		base.ItemToggle ();
	}
	
	void OnEnable() {
		EventDispatcher.AddHandler( EventKey.GAME_SET_ROTATION, HandleEvent );
	}
	
	void OnDisable() {
		EventDispatcher.RemoveHandler( EventKey.GAME_SET_ROTATION, HandleEvent );
	}
	
	private void HandleEvent( string eventName, object param ) {
		switch (eventName) {
		case EventKey.GAME_SET_ROTATION:
			float rotation = (float)param;
			fanSmoke.transform.localEulerAngles = new Vector3( 270.0f , rotation - 90.0f, 0.0f );
			break;
		}
	}
	
	void Update() {
		fanMesh.RotateAroundLocal( Vector3.up, -fanSpeed * Mathf.Deg2Rad * Time.deltaTime );
	}
	
}
