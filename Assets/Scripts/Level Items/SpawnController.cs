using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {
	
	public float spawnRotation = 0f;
	
	public void Start()
	{
		Vector3 pos = transform.position;
		EventDispatcher.SendEvent(EventKey.PLAYER_SETCHECKPOINT, new Vector4(pos.x, pos.y, pos.z, spawnRotation));
	}
	
	void OnDrawGizmosSelected()
	{
		Vector2 rotVec = VectorEx.AngleToVector(spawnRotation);
		GizmoEx.DrawNormal(transform.position, new Vector3(rotVec.x, 0f, rotVec.y), 2f, Color.yellow);
	}
}
