using UnityEngine;
using System.Collections;

public class ItemTimer : MonoBehaviour
{
	public float startDelay = 0f;
	public float toggleDelay = 1f;
	
	void Start()
	{
		StartCoroutine(TimerCoroutine());
	}
	
	private IEnumerator TimerCoroutine()
	{
		yield return new WaitForSeconds(startDelay);
		
		while(true)
		{
			gameObject.SendMessage("ItemToggle");
			yield return new WaitForSeconds(toggleDelay);
		}
	}
	
	void OnDrawGizmos() // Vector3(-0.5f, 1f, 0.5f)
	{
		Gizmos.DrawIcon(transform.position + transform.rotation * new Vector3(0.5f, 1f, 0.5f) , "timer.png", true);
	}
	
}
