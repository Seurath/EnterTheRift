using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum HydraControllerId
{
	Undefined = -1,
	Left = 0,
	Right = 1,
	Count
}

public class InputManager : MonoBehaviour
{
	public static class HydraSensitivity
	{
		public const float TriggerPress = 0.9f;
		public const float TriggerRelease = 0.05f;
		public const float Position = 0.005f;
	}
	
	[SerializeField] private Transform cameraMount = null;
	[SerializeField] private bool isUsingHydra = true;
	
	[SerializeField] private Vector3 offset;
	[SerializeField] private Vector2 leftStick;
	[SerializeField] private Vector2 rightStick;
	
	/// <summary>
	/// Sixense input script that comes packed inside the Sixense Unity Plugin.
	/// </summary>
	/// <remarks>
	/// The reference to this script should be set inside ManagerFactory.
	/// </remarks>
	public SixenseInput sixenseInputScript { get; set; }
	
	/// <summary>
	/// Whether calls can be made to Sixense input.
	/// </summary>
	public bool HasSixenseInput
	{
		get { return this.sixenseInputScript != null; }
	}
	
	public int NumHyrdaControllers { get { return (int) HydraControllerId.Count; } }
	
	
	#region Callbacks
	
	private List<HydraCallbacks> hydraCallbacks = new List<HydraCallbacks>((int) HydraControllerId.Count);
	public List<HydraCallbacks> HydraCallbacks
	{
		get {
			if (this.hydraCallbacks.Count == 0)
			{
				// Initialize all Hydra controller callback objects.
				for (int i = 0; i < this.hydraCallbacks.Count; i++)
				{
					this.hydraCallbacks[i] = new HydraCallbacks();
				}
			}
			return this.hydraCallbacks;
		}
	}
	
	private GamepadCallbacks gamepadCallbacks = new GamepadCallbacks();
	public GamepadCallbacks GamepadCallbacks
	{
		get { return this.gamepadCallbacks; }
	}
	
	#endregion Callbacks
	
	
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
			this.hydraCallbacks[(int)id].StickAction(new Vector2(controller.JoystickX, controller.JoystickY));
		}
		if (id == HydraControllerId.Right) 
		{
			this.hydraCallbacks[(int)id].StickAction(new Vector2(controller.JoystickX, controller.JoystickY));
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

