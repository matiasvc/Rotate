using UnityEngine;
using System.Collections;

public class ElectroController : LevelItem {
	
	[SerializeField]
	private Vector3[] electroPointsPos;
	
	public Vector3 ElectroPoint
	{
		get
		{
			return transform.position + ( ( transform.forward * 0.5f ) + ( transform.right * 0.85f ) );
		}
	}
	
	[SerializeField]
	private ElectroController electroTarget;
	
	public ElectroController ElectroTarget
	{
		get { return electroTarget; }
		set
		{
			int points = Mathf.FloorToInt( Vector3.Distance( ElectroPoint, value.ElectroPoint ) );
			if( points < 3 )
				points = 3;
			
			float lerpStep = 1f / (float)points;
			
			electroPointsPos = new Vector3[points];
			
			for (int i = 0; i < points; i++)
			{
				electroPointsPos[i] = Vector3.Lerp( ElectroPoint, value.ElectroPoint, lerpStep * i );
			}
		}
	}
	
}
