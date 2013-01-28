using UnityEngine;
using System.Collections;

public class CheckpointOnEnter : MonoBehaviour {
	
	public Vector3 checkpointOffset = Vector3.zero;
	public float checkpointRotation = 0f;
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Vector3 checkpointPos = transform.position + checkpointOffset;
			
			EventDispatcher.SendEvent(EventKey.PLAYER_SET_CHECKPOINT, new Vector4(checkpointPos.x, checkpointPos.y, checkpointPos.z, checkpointRotation + transform.localEulerAngles.y));
		}
	}
	
	void OnDrawGizmosSelected()
	{
		Vector2 rotVec = VectorEx.AngleToVector(checkpointRotation + transform.localEulerAngles.y);
		GizmoEx.DrawNormal(transform.position + checkpointOffset, new Vector3(rotVec.x, 0f, rotVec.y), 1f, Color.yellow);
	}
	
}
