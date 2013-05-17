using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PunchPlayer : MonoBehaviour 
{
	public Vector2 joystickLeft;
	public Vector2 joystickRight;
	
	public float moveSpeed = 4.0f;
	public float backwardsMultiplier = 0.5f;
	public float strafeMultiplier = 0.75f;
	
	public float turnSpeed = 4.0f;
	public Transform cameraMount;
	
	public float pitch = 0.0f;
	public Vector2 pitchClamp = new Vector2(-30.0f, 30.0f);
	
	private CharacterController cc;

	// Use this for initialization
	void Start () {
		cc = (CharacterController)GetComponent (typeof(CharacterController));
	}
	
	
	
	// Update is called once per frame
	void Update () {
		// Handle movement
		float xMovement = joystickLeft.x;
		xMovement *= moveSpeed * strafeMultiplier;
		
		float zMovement = joystickLeft.y;
		zMovement *= moveSpeed;
		if(zMovement < 0)
		{
			zMovement *= backwardsMultiplier;
		}
		
		Vector3 localMovementSpeed = new Vector3(xMovement, 0.0f, zMovement);
		cc.SimpleMove(transform.TransformDirection(localMovementSpeed));
		
		transform.Rotate (0.0f, joystickRight.x * turnSpeed, 0.0f);
		if(cameraMount != null)
		{
			pitch -= joystickRight.y * turnSpeed;
			if(pitch > pitchClamp.y)
			{
				pitch = pitchClamp.y;
			}
			else if(pitch < pitchClamp.x)
			{
				pitch = pitchClamp.x;
			}
			
			cameraMount.transform.localRotation = Quaternion.Euler (pitch, 0.0f, 0.0f);
		}
	}
	
}