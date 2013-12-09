using UnityEngine;
using System.Collections;

public class SawbladeController : MonoBehaviour {

	public float speed = 180.0f;

	private float rotation = 0.0f;
	private Transform meshTransform;

	void Awake() {
		meshTransform = transform.GetChild(0);
	}

	void Update() {
		rotation += speed * Time.deltaTime;
		rotation = Mathf.Repeat( rotation, 360.0f );
		meshTransform.localEulerAngles = new Vector3( 0.0f, rotation, 0.0f );
	}

}
