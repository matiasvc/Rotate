using UnityEngine;
using System.Collections;

public class FlamerController : LevelItem {
	
	public float fireLength;
	
	private ParticleEmitter flameParticles;
	private BoxCollider itemCollider;
	private BoxCollider flameCollider;
	
	// TODO Clean up all this properties shit
	
	public ParticleEmitter FlameParticles
	{
		get
		{
			if( flameParticles == null )
			{
				flameParticles = gameObject.GetComponentInChildren<ParticleEmitter>();
			}
			
			return flameParticles;
		}
	}
	
	public BoxCollider ItemCollider
	{
		get
		{
			if( itemCollider == null )
			{
				BoxCollider[] tempArray = gameObject.GetComponentsInChildren<BoxCollider>();
				
				foreach( BoxCollider boxCollider in tempArray )
				{
					if( boxCollider != (BoxCollider)gameObject.collider )
					{
						itemCollider = boxCollider;
						break;
					}
				}
			}
			
			return itemCollider;
		}
	}
	
	public BoxCollider FlameCollider
	{
		get
		{
			if( flameCollider == null )
			{
				flameCollider = (BoxCollider)gameObject.collider;
			}
			
			return flameCollider;
		}
	}
	
	
	void Awake ()
	{
		
	}
	
	void Start ()
	{
		
	}
	
}
