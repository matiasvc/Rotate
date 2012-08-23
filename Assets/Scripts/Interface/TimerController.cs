using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour
{
	public Transform roll1;
	public Transform roll2;
	public Transform roll3;
	
	private float time = 0f;
	private int lastSetTime = 0; 
	
	
	private int roll1num = 0;
	private int roll2num = 0;
	private int roll3num = 0;
	
	void Update()
	{
		time += Time.deltaTime;
		
		if (Mathf.FloorToInt(time) > lastSetTime)
		{
			int intTime = Mathf.FloorToInt(time);
			
			int num1 = intTime / 100;
			int num2 = (intTime / 10) % 10;
			int num3 = intTime % 10;
			
			SetRollNum(1, num1);
			SetRollNum(2, num2);
			SetRollNum(3, num3);
			
			lastSetTime = Mathf.FloorToInt(time);
		}
	}
	
	private void SetRollNum(int roll, int num)
	{	
		if (roll == 1)
		{
			if (roll1num != num)
			{
				roll1num = num;
				StartCoroutine(SetRollNumCoroutine(roll1, 36f * num));
			}
		}
		else if (roll == 2)
		{
			if (roll2num != num)
			{
				roll2num = num;
				StartCoroutine(SetRollNumCoroutine(roll2, 36f * num));
			}
		}
		else if (roll == 3)
		{
			if (roll3num != num)
			{
				roll3num = num;
				StartCoroutine(SetRollNumCoroutine(roll3, 36f * num));
			}
		}
	}
	
	private IEnumerator SetRollNumCoroutine(Transform roll, float rotTo)
	{
		Vector3 oldRot = roll.localEulerAngles;
		float rotFrom = rotTo - 36f;
		float length = 0.4f;
		float t = 0f;
		
		while (t <= length)
		{
			float newRot = LerpEx.LerpUnclamped(rotFrom, rotTo, Easing.easing(Easing.EasingType.spring, 0f, 1f, Mathf.InverseLerp(0f, length, t)));
			roll.localEulerAngles = new Vector3(newRot, 0f, 0f);
			
			yield return null;
			t += Time.deltaTime;
		}
	}
	
}
