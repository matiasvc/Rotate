using UnityEngine;
using System.Collections;

public class PistonController : MonoBehaviour {
	
	private float pushForce = 20.0f;
	public bool directional = false;
	public float directionAngle = 0f;
	
	private Vector3 _directionVector;
	
	private Vector3 directionVector
	{
		get
		{
#if UNITY_EDITOR
			Vector2 editorTempVector = RotateVector2(transform.right, directionAngle);
				
			return new Vector3(editorTempVector.x, editorTempVector.y, 0f);
#endif
			
			if(_directionVector == null)
			{
				Vector2 tempVector = RotateVector2(transform.right, directionAngle);
				_directionVector = new Vector3(tempVector.x, tempVector.y, 0f);
				
				return _directionVector;
			}
			else
				return _directionVector;
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
	
	
	private Vector2 RotateVector2(Vector2 vector ,float angle)
	{
		angle = angle * Mathf.Deg2Rad;
		
		float cs = Mathf.Cos(angle);
		float sn = Mathf.Sin(angle);
		
		float xt = (vector.x * cs - vector.y * sn);
		float yt = (vector.x * sn + vector.y * cs);
		
		return new Vector2(xt,yt);
	}
}
