using UnityEngine;
using System.Collections;

public class TweenHelper : MonoBehaviour
{
	
	public delegate void TweenFloatDelegate(float value);
	public delegate void TweenColorDelegate(Color value);
	public delegate void TweenVector2Delegate(Vector2 value);
	public delegate void TweenVector3Delegate(Vector3 value);
	public delegate void TweenVector4Delegate(Vector4 value);
	public delegate void TweenQuaternionDelegate(Quaternion value);
	
	public static void Tween(float fromValue, float toValue, float length, Easing.EasingType easingType, TweenFloatDelegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	public static void Tween(Color fromValue, Color toValue, float length, Easing.EasingType easingType, TweenColorDelegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	public static void Tween(Vector2 fromValue, Vector2 toValue, float length, Easing.EasingType easingType, TweenVector2Delegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	public static void Tween(Vector3 fromValue, Vector3 toValue, float length, Easing.EasingType easingType, TweenVector3Delegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	public static void Tween(Vector4 fromValue, Vector4 toValue, float length, Easing.EasingType easingType, TweenVector4Delegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	public static void Tween(Quaternion fromValue, Quaternion toValue, float length, Easing.EasingType easingType, TweenQuaternionDelegate callback)
	{
		TweenHelper tweenHelper = CreateTweenHelperObject();
		tweenHelper.StartCoroutine(tweenHelper.TweenCoroutine(fromValue, toValue, length, easingType, callback));
	}
	
	private static TweenHelper CreateTweenHelperObject()
	{
		GameObject go = new GameObject("TweenHelper");
		DontDestroyOnLoad(go);
		return go.AddComponent<TweenHelper>();
	}
	
	public IEnumerator TweenCoroutine(float fromValue, float toValue, float length, Easing.EasingType easingType, TweenFloatDelegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.LerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
	public IEnumerator TweenCoroutine(Color fromValue, Color toValue, float length, Easing.EasingType easingType, TweenColorDelegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.ColorLerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
	public IEnumerator TweenCoroutine(Vector2 fromValue, Vector2 toValue, float length, Easing.EasingType easingType, TweenVector2Delegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.Vector2LerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
	public IEnumerator TweenCoroutine(Vector3 fromValue, Vector3 toValue, float length, Easing.EasingType easingType, TweenVector3Delegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.Vector3LerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
	public IEnumerator TweenCoroutine(Vector4 fromValue, Vector4 toValue, float length, Easing.EasingType easingType, TweenVector4Delegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.Vector4LerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
	public IEnumerator TweenCoroutine(Quaternion fromValue, Quaternion toValue, float length, Easing.EasingType easingType, TweenQuaternionDelegate callback)
	{
		float t = 0f;
		
		while (t <= length)
		{
			float timeLerp = Mathf.InverseLerp(0f, length, t);
			float lerp = Easing.easing(easingType, 0f, 1f, timeLerp);
			callback(LerpEx.QuaternionLerpUnclamped(fromValue, toValue, lerp));
			
			yield return null;
			t += Time.deltaTime;
		}
		
		Destroy(gameObject);
	}
	
}
