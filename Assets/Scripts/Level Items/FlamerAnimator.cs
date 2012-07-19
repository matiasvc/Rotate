using UnityEngine;
using System.Collections;

public class FlamerAnimator : MonoBehaviour {
	
	public AnimationCurve animationCurve;
	
	private FlamerController flameController;
	
	private float animationLength;
	private float animationTime = 0f;
	
	void Awake()
	{
		flameController = gameObject.GetComponent<FlamerController>();
		
		if (flameController == null || animationCurve.length < 2)
		{
			Debug.LogError("ERROR! Unable to setup.");
			this.enabled = false;
		}
		
		animationLength = animationCurve.keys[animationCurve.keys.Length-1].time;
	}
	
	void Update()
	{
		flameController.SetFlameLength(animationCurve.Evaluate(animationTime));
		animationTime += Time.deltaTime;
	}
}
