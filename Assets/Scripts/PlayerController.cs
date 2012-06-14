using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public Transform cameraGroupTransform;
	public Transform cameraTransform;
	public Transform playerTransform;
	
	private float rotationSpeed = 120f;
	
	private float rotation = 0;
	private float gravity = 9.8f;
	
	private float cameraFollowRate = 4f;
	//private float cameraRotateAmt = 10f;
	
	private bool rigthButton = false;
	private bool leftButton = false;
	
	protected void Awake()
	{
		EventDispatcher.AddHandler("PLAYER_ENTER_TRIGGER", PlayerEnterTrigger);
	}
	
	protected void Start()
	{
		LevelController.Instance.RespawnPoint = transform.position;
	}
	
	protected void Update ()
	{
		float input = Input.GetAxis("Horizontal");
		
		if(rigthButton)
			input -= 1f;
		
		if(leftButton)
			input += 1f;
		
		rotation += (input * rotationSpeed) * Time.deltaTime;
		cameraGroupTransform.localEulerAngles = new Vector3(cameraGroupTransform.localEulerAngles.x, rotation, cameraGroupTransform.localEulerAngles.z);
		
		float rad = rotation * Mathf.Deg2Rad;
		Physics.gravity = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * -gravity;
		
		playerTransform.rigidbody.WakeUp();
	}
	
	protected void LateUpdate()
	{
		/*
		Vector2 camRot = new Vector2(cameraGroupTransform.localPosition.x, cameraGroupTransform.localPosition.z) - new Vector2(playerTransform.localPosition.x, playerTransform.localPosition.z);
		
		float rotRad = rotation * Mathf.Deg2Rad;
		
		float cs = Mathf.Cos(rotRad);
		float sn = Mathf.Sin(rotRad);
		
		float xt = camRot.x * cs - camRot.y * sn;
		float yt = camRot.x * sn + camRot.y * cs;
		
		camRot = new Vector2(xt, yt);
		camRot *= -cameraRotateAmt;
		
		cameraTransform.localEulerAngles = new Vector3(camRot.x, cameraTransform.localEulerAngles.y, camRot.y);
		*/
		
		cameraGroupTransform.position = Vector3.Lerp(cameraGroupTransform.position, playerTransform.position, cameraFollowRate * Time.deltaTime);
	}
	
	protected void PlayerEnterTrigger(string eventName, object param)
	{
		Collider other;
		
		if(param is Collider)
		{
			other = param as Collider;
		}
		else
		{
			Debug.LogError("Object is not of type: Collider");
			return;
		}
		
		if(other.tag == "DamagePlayer")
		{
			Death();
		}
	}
	
	protected void OnGUI()
	{
		Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		Vector2 buttonSize = new Vector2(300f, 100f);
		
		if(GUI.RepeatButton(new Rect(screenCenter.x - buttonSize.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "<----"))
		{
			leftButton = true;
		}
		else
		{
			leftButton = false;
		}
		
		if(GUI.RepeatButton(new Rect(screenCenter.x, Screen.height - buttonSize.y, buttonSize.x, buttonSize.y), "---->"))
		{
			rigthButton = true;
		}
		else
		{
			rigthButton = false;
		}
	}
	
	private void Death()
	{
		Vector3 respawnPoint = LevelController.Instance.RespawnPoint;
		
		playerTransform.position = respawnPoint;
		rotation = 0f;
		playerTransform.rigidbody.velocity = Vector3.zero;
		
		
		EventDispatcher.SendEvent("PLAYER_RESPAWN", respawnPoint);
	}
}
