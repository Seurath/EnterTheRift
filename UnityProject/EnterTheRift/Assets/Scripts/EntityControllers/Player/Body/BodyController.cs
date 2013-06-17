using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class BodyController : MonoBehaviour 
{
	[SerializeField] private float moveSpeed = 4.0f;
	[SerializeField] private float backwardsMultiplier = 0.5f;
	[SerializeField] private float strafeMultiplier = 0.75f;
	
	[SerializeField] private float turnSpeed = 4.0f;
	[SerializeField] private Transform cameraMount;
	
	[SerializeField] private float pitch = 0.0f;
	[SerializeField] private Vector2 pitchClamp = new Vector2(-30.0f, 30.0f);
	
	private CharacterController characterController;
	
	
	#region Initialization
	
	private void VerifySerializedFields ()
	{
		if (this.cameraMount == null) { Debug.LogError("Camera mount not set."); }
		if (this.characterController == null)
		{
			this.characterController = GetComponent<CharacterController>();
		}
	}
	
	private void RegisterInputCallbacks ()
	{
		ManagerFactory.InputManager.HydraCallbacks[(int)HydraControllerId.Left].RegisterStickAction(OnLeftStick);
		ManagerFactory.InputManager.HydraCallbacks[(int)HydraControllerId.Right].RegisterStickAction(OnRightStick);
	}
	
	#endregion Initialization
	
	
	#region MonoBehaviour
	
	void Awake ()
	{
		VerifySerializedFields();
	}
	
	void Start ()
	{
		RegisterInputCallbacks();
	}
	
	#endregion MonoBehaviour
	
	
	#region Input Control Callbacks
	
	
	private void OnLeftStick (Vector2 input)
	{
		// Handle movement
		float xMovement = input.x;
		xMovement *= this.moveSpeed * this.strafeMultiplier;
		
		float zMovement = input.y;
		zMovement *= this.moveSpeed;
		if (zMovement < 0)
		{
			zMovement *= this.backwardsMultiplier;
		}
		
		Vector3 localMovementSpeed = new Vector3(xMovement, 0.0f, zMovement);
		this.characterController.SimpleMove(this.transform.TransformDirection(localMovementSpeed));
	}
	
	private void OnRightStick (Vector2 input)
	{
		this.transform.Rotate(0.0f, input.x * turnSpeed, 0.0f);
		this.pitch -= input.y * this.turnSpeed;
		if (this.pitch > this.pitchClamp.y)
		{
			this.pitch = this.pitchClamp.y;
		}
		else if (this.pitch < this.pitchClamp.x)
		{
			this.pitch = this.pitchClamp.x;
		}
		
		this.cameraMount.transform.localRotation = Quaternion.Euler(this.pitch, 0.0f, 0.0f);
	}
	
	#endregion Input Control Callbacks
	
}
