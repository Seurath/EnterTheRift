using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class BodyController : MonoBehaviour 
{
	[SerializeField] private CharacterController characterController = null;
	[SerializeField] private Transform cameraMount = null;
	[SerializeField] private Hand leftHand = null;
	[SerializeField] private Hand rightHand = null;
	
	[SerializeField] private float moveSpeed = 4.0f;
	[SerializeField] private float backwardsMultiplier = 0.5f;
	[SerializeField] private float strafeMultiplier = 0.75f;
	[SerializeField] private float turnSpeed = 4.0f;
	[SerializeField] private float pitch = 0.0f;
	[SerializeField] private Vector2 pitchClamp = new Vector2(-30.0f, 30.0f);
	
	private InputManager inputManager;
	
	
	#region Initialization
	
	private void VerifySerializedFields ()
	{
		if (this.cameraMount == null) { Debug.LogError("Camera mount not set."); }
		if (this.leftHand == null) { Debug.LogError("Left hand not set."); }
		if (this.rightHand == null) { Debug.LogError("Right hand not set."); }
		if (this.characterController == null)
		{
			this.characterController = GetComponent<CharacterController>();
		}
	}
	
	private void RegisterInputCallbacks ()
	{
		this.inputManager = ManagerFactory.InputManager;
		
		if (this.inputManager.HasSixenseInput)
		{
			RegisterHydraInputCallbacks();
		}
	}
	
	private void RegisterHydraInputCallbacks ()
	{
		int leftId = HydraControllerId.Left.IntValue();
		HydraCallbacks leftCallbacks = this.inputManager.HydraCallbacks[leftId];
		leftCallbacks.RegisterStickAction(OnLeftStick);
		leftCallbacks.RegisterTriggerPressAction(OnLeftTriggerPress);
		leftCallbacks.RegisterTriggerReleaseAction(OnLeftTriggerRelease);
		leftCallbacks.RegisterPositionAction(OnLeftHydraPosition);
		leftCallbacks.RegisterRotationAction(OnLeftHydraRotation);
		
		int rightId = HydraControllerId.Right.IntValue();
		HydraCallbacks rightCallbacks = this.inputManager.HydraCallbacks[rightId];
		rightCallbacks.RegisterStickAction(OnRightStick);
		rightCallbacks.RegisterTriggerPressAction(OnRightTriggerPress);
		rightCallbacks.RegisterTriggerReleaseAction(OnRightTriggerRelease);
		rightCallbacks.RegisterPositionAction(OnRightHydraPosition);
		rightCallbacks.RegisterRotationAction(OnRightHydraRotation);
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
	
	#region Triggers
	
	private void OnLeftTriggerPress (float input)
	{
		this.leftHand.SetHandFist(true);
	}
	
	private void OnLeftTriggerRelease (float input)
	{
		this.leftHand.SetHandFist(false);
	}
	
	private void OnRightTriggerPress (float input)
	{
		this.rightHand.SetHandFist(true);
	}
	
	private void OnRightTriggerRelease (float input)
	{
		this.rightHand.SetHandFist(false);
	}
	
	#endregion Triggers
	
	
	#region Sticks
	
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
	
	#endregion Sticks
	
	
	#region Position
	
	private void OnLeftHydraPosition (Vector3 input)
	{
		this.leftHand.SetPosition(input);
	}
	
	private void OnRightHydraPosition (Vector3 input)
	{
		this.rightHand.SetPosition(input);
	}
	
	#endregion Position
	
	
	#region Rotation
	
	private void OnLeftHydraRotation (Quaternion input)
	{
		this.leftHand.SetRotation(input);
	}
	
	private void OnRightHydraRotation (Quaternion input)
	{
		this.rightHand.SetRotation(input);
	}
	
	#endregion Rotation
	
	#endregion Input Control Callbacks
	
}
