using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand : MonoBehaviour 
{
	private enum ControllerId
	{
		Undefined = -1,
		Left = 0,
		Right = 1
	}
	
	[SerializeField] private bool isUsingHydra = true;
	[SerializeField] private ControllerId controllerId = ControllerId.Undefined;
	[SerializeField] private PunchPlayer player = null;
	[SerializeField] private Animation handAnimation = null;
	[SerializeField] private HandGrab dynamicCollider = null;
	[SerializeField] private Transform cameraMount = null;
	
	[SerializeField] private Vector3 offset;
	[SerializeField] private Vector2 leftStick;
	[SerializeField] private Vector2 rightStick;
	[SerializeField] private Vector3 handWorldPosition;
	
	public bool FistsDisabled { get; set; }
	public bool CanSetShoulders { get; set; }
	public bool IsShoulderSet { get; set; }
	
	private bool isTriggerDown = false;
	
	
	#region MonoBehaviour
	
	void Awake ()
	{
		Initialize();
	}
	
	void Start ()
	{
		if (this.isUsingHydra)
		{
			SixenseInput.ControllerManagerEnabled = false;
		}
		else
		{
			offset = new Vector3(0.0f, 0.0f, 1.0f);
		}
		
		if (player == null)
		{
			Debug.LogError ("Hand needs a PunchPlayer attached.");
		}
	}
	
	void Update () 
	{
		if (this.isUsingHydra)
		{
			// Using Razer Hydra controller.
			UpdateHydra();
		}
		else 
		{
			// Using gamepad controller.
			UpdateGamepad();
		}
		
		if (this.controllerId == ControllerId.Left)
		{
			this.player.joystickLeft = this.leftStick;
		}
		if (this.controllerId == ControllerId.Right)
		{
			this.player.joystickRight = this.rightStick;
		}
	}
	
	#endregion MonoBehaviour
	
	
	#region Initialization
	
	private void Initialize ()
	{
		this.FistsDisabled = true;
		this.CanSetShoulders = true;
	}
	
	#endregion Initialization
	
	
	#region Hydra Input Controls
	
	private void UpdateHydra ()
	{
		SixenseInput.Controller controller = SixenseInput.Controllers[(int) this.controllerId];
		
		// Position data.
		UpdateHydraPosition(controller);
			
		// Rotation data.
		UpdateHydraRotation(controller);
		
		if (this.CanSetShoulders)
		{
			if (controller.Trigger >= 0.9f 
				&& !this.isTriggerDown) 
			{
				this.isTriggerDown = true;
				
				// Allow the user to set the base position by pressing the trigger.
				if (!IsShoulderSet)
				{
					this.offset = new Vector3(0.0f, 0.0f, controller.Position.z * 0.005f);
					this.IsShoulderSet = true;
				}
				else if (handAnimation != null)
				{
					this.handAnimation.Play("fist");
					this.dynamicCollider.GrabItems();
				}
			}
			else if (isTriggerDown && controller.Trigger < 0.05) {
				isTriggerDown = false;
				if(handAnimation != null)
				{
					handAnimation.Play ("unfist");
					dynamicCollider.LetGoOfItems();
				}
			}
		}
		
		// Send analog stick values to the player, for movement purposes.
		UpdateHydraAnalogStick(controller);
	}
	
	private void UpdateHydraPosition (SixenseInput.Controller controller)
	{
		Vector3 controllerPosition = controller.Position;
		Vector3 desiredLocalPosition = new Vector3(controllerPosition.x * .005f,
												   controllerPosition.y * .005f,
												   controllerPosition.z * .005f) - offset;
		if (this.cameraMount != null)
		{
			desiredLocalPosition = cameraMount.transform.localRotation * desiredLocalPosition;
		}
		
		if (this.FistsDisabled)
		{
			if (controllerId == ControllerId.Left)
			{
				desiredLocalPosition = new Vector3(-1.0f, -0.5f, 1.0f);
			}
			if (controllerId == ControllerId.Right)
			{
				desiredLocalPosition = new Vector3(1.0f, -0.5f, 1.0f);
			}
		}
		
		this.transform.localPosition = desiredLocalPosition;
	}
	
	private void UpdateHydraRotation (SixenseInput.Controller controller)
	{
		transform.localRotation = new Quaternion(controller.Rotation.x,
												 controller.Rotation.y,
												 controller.Rotation.z,
												 controller.Rotation.w);
		if (this.FistsDisabled)
		{
			this.transform.localRotation = new Quaternion();
		}
	}
	
	private void UpdateHydraAnalogStick (SixenseInput.Controller controller)
	{
		if (controllerId == ControllerId.Left) 
		{
			this.leftStick = new Vector2(controller.JoystickX, controller.JoystickY);
		}
		else if (controllerId == ControllerId.Right) 
		{
			this.rightStick = new Vector2(controller.JoystickX, controller.JoystickY);
		}
	}
	
	#endregion Hydra Input Controls
	
	
	#region Gamepad Input Controls
	
	private void UpdateGamepad ()
	{
		float triggerValue = Input.GetAxis ("Triggers");

		if(controllerId == 0)
		{
			if(triggerValue > 0.8f)
			{
				leftStick = new Vector2();
				transform.localPosition = new Vector3(
											(Mathf.Clamp (Input.GetAxis ("HorizontalL"), -0.75f, 0.75f) - 0.25f) * 2.0f,
											Mathf.Clamp (Input.GetAxis ("VerticalR"), -0.75f, 0.75f) * -2.0f,
											Mathf.Clamp (Input.GetAxis ("VerticalL"), -0.75f, 0.75f) + 0.25f) +
										  offset;
				
				if(!isTriggerDown && handAnimation != null)
				{
					handAnimation.Play("fist");
					isTriggerDown = true;
				}
			}
			else
			{
				leftStick = new Vector2(Input.GetAxis ("HorizontalL"), Input.GetAxis ("VerticalL"));
				
				if(isTriggerDown && handAnimation != null)
				{
					handAnimation.Play("unfist");
					isTriggerDown = false;
				}
			}
		}
		else if (this.controllerId == ControllerId.Right)
		{
			if(triggerValue < -0.8f)
			{
				rightStick = new Vector2();
				transform.localPosition = new Vector3(
											(Mathf.Clamp (Input.GetAxis ("HorizontalL"), -0.75f, 0.75f) + 0.25f) * 2.0f,
											Mathf.Clamp (Input.GetAxis ("VerticalR"), -0.75f, 0.75f) * -2.0f,
											Mathf.Clamp (Input.GetAxis ("VerticalL"), -0.75f, 0.75f) + 0.25f) +
										  offset;
				
				if(!isTriggerDown && handAnimation != null)
				{
					handAnimation.Play("fist");
					isTriggerDown = true;
				}
			}
			else
			{
				rightStick = new Vector2(Input.GetAxis ("HorizontalR"), Input.GetAxis("VerticalR"));
				
				if(isTriggerDown && handAnimation != null)
				{
					handAnimation.Play("unfist");
					isTriggerDown = false;
				}
			}
		}
	}
	
	#endregion Gamepad Input Controls
	
}
