using UnityEngine;
using System.Collections;

public class PistonController : LevelItem {
	
	private float pushForce = 20.0f;
	public bool directional = false;
	public float directionAngle = 0f;
	
	private Vector3 _directionVector;
	
	private Vector3 directionVector
	{
		get
		{
#if UNITY_EDITOR
			Vector2 editorTempVector = VectorEx.RotateVector2(transform.right, directionAngle);
			return new Vector3(editorTempVector.x, editorTempVector.y, 0f);
#else
			if(_directionVector == null)
			{
				Vector2 tempVector = VectorEx.RotateVector2(transform.right, directionAngle);
				_directionVector = new Vector3(tempVector.x, tempVector.y, 0f);
				
				return _directionVector;
			}
			else
				return _directionVector;
#endif
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(directional)
		{
			other.rigidbody.velocity = directionVector * pushForce;
			animation.Play("push");
		}
		else
		{
			if(other.gameObject.tag == "Player")
			{
				other.rigidbody.AddForce(transform.right * pushForce, ForceMode.VelocityChange);
				animation.Play("push");
			}
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if(directional)
		{
			Vector3 drawFrom = transform.TransformPoint(gameObject.GetComponent<SphereCollider>().center);
			Vector3 drawTo = drawFrom + (directionVector * 2.5f);
			
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(drawFrom, drawTo);
		}
	}
	
}
