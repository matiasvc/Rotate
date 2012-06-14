using UnityEngine;
using System.Collections;

public class SawBladeController : MonoBehaviour {
	
	public float rotateSpeed;
	
	void Update()
	{
		transform.RotateAroundLocal(Vector3.forward, rotateSpeed * Time.deltaTime);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
