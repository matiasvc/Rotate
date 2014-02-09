using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTimer : MonoBehaviour {
	
	public enum TimerObjectType { WAIT, TOGGLE, ON, OFF, EXIT };
	
	public List<TimerObject> timerObjects = new List<TimerObject>();
	
	
	[System.Serializable]
	public class TimerObject {
		public TimerObjectType type = TimerObjectType.WAIT;
		public float wait = 0.0f;
	}
	
	void OnEnable() {
		StartCoroutine(TimerCoroutine());
	}
	
	private IEnumerator TimerCoroutine() {
		int currentObjectIndex = 0;
		
		while( true ) {
			if ( timerObjects.Count == 0 ) { yield return false; }
			TimerObject currentObject = timerObjects[currentObjectIndex];
			
			switch ( currentObject.type ) {
			case TimerObjectType.WAIT:
				yield return new WaitForSeconds( currentObject.wait );
				break;
			case TimerObjectType.TOGGLE:
				gameObject.SendMessage("ItemToggle");
				break;
			case TimerObjectType.ON:
				gameObject.SendMessage("ItemEnable");
				break;
			case TimerObjectType.OFF:
				gameObject.SendMessage("ItemDisable");
				break;
			case TimerObjectType.EXIT:
				yield return false;
				break;
			}
			
			currentObjectIndex++;
			if ( currentObjectIndex >= timerObjects.Count ) {
				currentObjectIndex = 0;
			}
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.DrawIcon( transform.position + transform.rotation * new Vector3(0.5f, 1f, 0.5f) , "timer.png", true );
	}
	
}
