using UnityEngine;
using System.Collections;

public class SpikesController : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
	}
	
}