using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SimpleWASD_MouseLook : MonoBehaviour {
	
	public float moveSpeed = 4.0f;
	public float backwardsMultiplier = 0.5f;
	public float strafeMultiplier = 0.75f;
	public Vector2 mouseSensitivity = new Vector2(1.0f, 1.0f);
	public bool invertY = true;
	
	private CharacterController cc = null;
	private Camera playerCamera = null;
	
	// Use this for initialization
	void Start () {
		cc = (CharacterController)GetComponent (typeof(CharacterController));
		if(cc == null)
		{
			Debug.LogError ("Could not find CharacterController for SimpleWASD_MouseLook script.");
			enabled = false;
		}
		
		playerCamera = (Camera)GetComponentInChildren(typeof(Camera));
		if(playerCamera == null)
		{
			Debug.LogError ("Could not find Camera for SimpleWASD_MouseLook script.");
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float xMovement = Input.GetAxis ("HorizontalL");
		
		xMovement *= moveSpeed * strafeMultiplier;
		
		float zMovement = Input.GetAxis ("VerticalL");
		zMovement *= moveSpeed;
		if(zMovement < 0)
		{
			zMovement *= backwardsMultiplier;
		}
		
		float xLook = Input.GetAxis ("HorizontalR");
		xLook *= mouseSensitivity.x;
		float yLook = Input.GetAxis ("VerticalR");
		yLook *= mouseSensitivity.y;
		if(invertY)
		{
			yLook *= -1.0f;
		}
		
		transform.Rotate (0.0f, xLook, 0.0f);
		
		Vector3 localMovementSpeed = new Vector3(xMovement, 0.0f, zMovement);
		cc.SimpleMove(transform.TransformDirection(localMovementSpeed));
		
		playerCamera.transform.Rotate (yLook, 0.0f, 0.0f);
	}
}
