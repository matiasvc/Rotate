using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundObjectController : MonoBehaviour {

	public Vector2 animationSpeed = Vector2.zero;

	private Vector2 animationPos = Vector2.zero;

	private MeshFilter _meshFilter = null;
	public MeshFilter meshFilter {
		get {
			if (_meshFilter == null) {
				_meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
			}
			return _meshFilter;
		}
	}

	private MeshRenderer _meshRenderer = null;
	public MeshRenderer meshRenderer {
		get {
			if (_meshRenderer == null) {
				_meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
			}
			return _meshRenderer;
		}
	}

	public int type = 0;
	public Vector2 tileSize = Vector2.one;

	public Rect planeOutline = new Rect( -1.5f, -1.5f, 3f, 3f );

	// Shape variables
	public string shapeData = "";

	void Update() {
		if ( !Mathf.Approximately(0f, animationSpeed.sqrMagnitude) && meshRenderer.renderer.isVisible ) {
			animationPos += animationSpeed * Time.deltaTime;
			meshRenderer.material.mainTextureOffset = animationPos;
		}
	}
}
