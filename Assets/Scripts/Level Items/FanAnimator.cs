using UnityEngine;
using System.Collections;

public class FanAnimator : MonoBehaviour {
	
	public bool fanEnabled = false;
	
	private float fanEnabledSpeed = 360f * 3f;
	
	private float dampening = 0.1f;
	private float motorForce = 360 * 3f;
	
	private float speed = 0f;
	private float rotation = 0f;
	
	void Update () {
		rotation -= speed * Time.deltaTime;
		rotation = Mathf.Repeat(rotation, 360f);
		
		if (fanEnabled)
			speed += motorForce * Time.deltaTime;
		else
			speed *= Mathf.Pow(dampening, Time.deltaTime);
		
		speed = Mathf.Clamp(speed, 0f, fanEnabledSpeed);
		
		transform.localEulerAngles = new Vector3(0f, 0f, rotation);
	}
}
