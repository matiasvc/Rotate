using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance = null;
	
	void Awake()
	{
		if ( Instance == null ) {
			Instance = this;
		} else if ( Instance != this ) {
			GameObject.Destroy( this.gameObject );
			return;
		}
		
		GameObject.DontDestroyOnLoad( this.gameObject );
		Application.targetFrameRate = 60;
	}
	
	void Start() {
		
	}
	
	void Update() {
		if ( OuyaInput.GetButtonDown( OuyaButton.LB, OuyaPlayer.P01 ) ) {
			HUDFPS hudFps = gameObject.GetComponent<HUDFPS>();
			hudFps.enabled = !hudFps.enabled;
		}
		
		if ( OuyaInput.GetButtonDown( OuyaButton.RB, OuyaPlayer.P01 ) ) {
			OuyaInputTester inputTester = gameObject.GetComponent<OuyaInputTester>();
			inputTester.enabled = !inputTester.enabled;
		}
	}
	
}
