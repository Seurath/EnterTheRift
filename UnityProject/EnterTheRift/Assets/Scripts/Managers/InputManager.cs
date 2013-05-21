using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour
{
	public enum HydraControllerId
	{
		Undefined = -1,
		Left = 0,
		Right = 1
	}
	
	[SerializeField] private Transform cameraMount = null;
	
	[SerializeField] private bool isUsingHydra = true;
	[SerializeField] private bool canSetShoulders = true;
	[SerializeField] private bool isShoulderSet = false;
	[SerializeField] private bool isTriggerDown = false;
	
	[SerializeField] private Vector3 offset;
	[SerializeField] private Vector2 leftStick;
	[SerializeField] private Vector2 rightStick;
	
	public Action<Vector2> LeftStickUpdatedAction;
	public Action<Vector2> RightStickUpdatedAction;
	
	
	#region MonoBehaviour
	
	void Start ()
	{
		if (this.isUsingHydra)
		{
			SixenseInput.ControllerManagerEnabled = false;
		}
		else
		{
			this.offset = new Vector3(0.0f, 0.0f, 1.0f);
		}
	}
	
	void Update () 
	{
		if (this.isUsingHydra)
		{
			// Using Razer Hydra controller.
			UpdateHydra(HydraControllerId.Left);
			UpdateHydra(HydraControllerId.Right);
		}
		else 
		{
			// Using gamepad controller.
			UpdateGamepad();
		}
	}
	
	#endregion MonoBehaviour
	
	
	#region Hydra Input Controls
	
	private void UpdateHydra (HydraControllerId controllerId)
	{
		SixenseInput.Controller controller = SixenseInput.Controllers[(int) controllerId];
		
		// Update position data.
		UpdateHydraPosition(controller);
			
		// Update rotation data.
		UpdateHydraRotation(controller);
		
		// Update analog stick values.
		UpdateHydraAnalogStick(controller, controllerId);
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
		
		this.transform.localPosition = desiredLocalPosition;
	}
	
	private void UpdateHydraRotation (SixenseInput.Controller controller)
	{
		transform.localRotation = new Quaternion(controller.Rotation.x,
												 controller.Rotation.y,
												 controller.Rotation.z,
												 controller.Rotation.w);
	}
	
	private void UpdateHydraAnalogStick (SixenseInput.Controller controller, HydraControllerId id)
	{
		if (id == HydraControllerId.Left) 
		{
			this.LeftStickUpdatedAction(new Vector2(controller.JoystickX, controller.JoystickY));
		}
		if (id == HydraControllerId.Right) 
		{
			this.RightStickUpdatedAction(new Vector2(controller.JoystickX, controller.JoystickY));
		}
	}
	
	#endregion Hydra Input Controls
	
	
	#region Gamepad Input Controls
	
	private void UpdateGamepad ()
	{
		float triggerValue = Input.GetAxis ("Triggers");

		
	}
	
	#endregion Gamepad Input Controls
	
}

