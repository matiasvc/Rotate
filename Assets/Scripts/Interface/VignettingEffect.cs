using UnityEngine;
using System.Collections;

public class VignettingEffect : MonoBehaviour {
	
	public Material vignetteMat;
	
	void Awake() {
		GameObject go = new GameObject("vignetting");
		go.transform.parent = transform;
		go.layer = LayerMask.NameToLayer("Player");
		
		MeshFilter meshFilter = go.AddComponent<MeshFilter>();
		Mesh mesh = MeshEx.CreateQuad();
		
		Camera cam = gameObject.camera;
		
		Vector3 camPointA = cam.ScreenToWorldPoint( new Vector3( 0.0f, 0.0f, 1.0f ) );
		Vector3 camPointB = cam.ScreenToWorldPoint( new Vector3( Screen.width, Screen.height, 1.0f ) );
		
		Vector2 vignetteScale = new Vector2( camPointB.x - camPointA.x, camPointB.z - camPointA.z );
		Debug.Log( camPointA );
		Debug.Log( camPointB );
		Debug.Log( vignetteScale );
		
		//MeshEx.TransformMesh( mesh, Vector3.zero, Quaternion.identity, new Vector3( vignetteScale.x, vignetteScale.y, 1.0f) );
		meshFilter.mesh = mesh;
		
		go.transform.localPosition = transform.InverseTransformPoint(camPointA);
		go.transform.localScale = new Vector3( vignetteScale.x, 1.0f ,vignetteScale.y );
		
		go.AddComponent<MeshRenderer>();
		go.renderer.material = vignetteMat;
	}
	
}
