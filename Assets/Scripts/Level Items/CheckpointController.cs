using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {
	
	public BeamController beamController;
	public Transform otherCheckpoint;
	
	public float checkpointLength;
	
	
	void Start()
	{
		beamController.GenerateBeamMesh(Mathf.RoundToInt(checkpointLength));
	}
}
