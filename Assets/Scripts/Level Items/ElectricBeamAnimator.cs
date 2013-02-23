using UnityEngine;
using System.Collections;

public class ElectricBeamAnimator : MonoBehaviour {
	
	public LineRenderer lineRenderer;
	public Vector3[] positionArray;
	
	private float rndSphereSize = 0.5f;
	
	private const float maxPointDistance = 2f;
	
	public void SetUp ( Vector3 startPos, Vector3 endPos ) {
		// Line renderer
		int pointCount = Mathf.CeilToInt( Vector3.Distance( startPos, endPos )/maxPointDistance ) + 2;
		positionArray = new Vector3[pointCount];
		
		positionArray[0] = startPos;
		positionArray[pointCount-1] = endPos;
		
		Vector3 lineVector = endPos - startPos;
		
		for ( int i = 1; i < pointCount-1; i++ ) {
			float lerp = 1f/( pointCount-1 );
			positionArray[i] = Vector3.Lerp( startPos, endPos, lerp*i );
		}
		
		lineRenderer.SetVertexCount( pointCount );
		
		for ( int i = 0; i < pointCount; i++ ) {
			lineRenderer.SetPosition(i, positionArray[i]);
		}
		
		// Collider
		gameObject.transform.position = startPos;
		gameObject.transform.LookAt( endPos, Vector3.up );
		
		CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
		
		float length = ( endPos - startPos ).magnitude;
		
		capsuleCollider.height = length;
		capsuleCollider.center = new Vector3( 0f, 0f, length*0.5f );
		
		gameObject.SetActive( true );
	}
	
	public void Disable () {
		gameObject.SetActive( false);
	}
	
	void Update () {
		if ( !renderer.isVisible ) {
			return;
		}
		
		for (int i = 1; i < positionArray.Length-1; i++) {
			lineRenderer.SetPosition(i, positionArray[i] + Random.insideUnitSphere * rndSphereSize);
		}
	}
}
