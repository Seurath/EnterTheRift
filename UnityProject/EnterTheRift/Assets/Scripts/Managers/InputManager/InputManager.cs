using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum InputId
{
	Undefined = -1,
	MouseAndKeyboard,
	Gamepad,
	Hydra
}

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
	
	[SerializeField] private InputId inputId;
	/// <summary>
	/// The type of input currently set, as defined by the InputId enum.
	/// </summary>
	public InputId InputId 
	{
		get { return this.inputId; }
		set { this.inputId = value; }
	}
	
	[SerializeField] private Vector3 offset;
	[SerializeField] private Vector2 leftStick;
	[SerializeField] private Vector2 rightStick;
	
	// Calibration
	
	[SerializeField] private bool canCalibrate = true;
	public bool CanCalibrate
	{
		get { return this.canCalibrate; }
		set { this.canCalibrate = value; }
	}
	public bool IsCalibrated { get; set; }
	
	/// <summary>
	/// Sixense input script that comes packed inside the Sixense Unity Plugin.
	/// </summary>
	/// <remarks>
	/// The reference to this script should be set inside ManagerFactory.
	/// </remarks>
	public SixenseInput SixenseInputScript { get; set; }
	
	/// <summary>
	/// Whether calls can be made to Sixense input.
	/// </summary>
	public bool HasSixenseInput
	{
		get { return this.SixenseInputScript != null; }
	}
	
	public int NumHyrdaControllers { get { return (int) HydraControllerId.Count; } }
	
	private IList<int> hydraBaseOffset = null;
	public IList<int> HydraBaseOffset
	{
		get {
			if (this.hydraBaseOffset == null)
			{
				this.hydraBaseOffset = new List<int>(HydraControllerId.Count.IntValue());
			}
			if (this.hydraBaseOffset.Count == 0)
			{
				// Initialize all Hydra controller callback objects.
				for (int i = 0; i < HydraControllerId.Count.IntValue(); i++)
				{
					this.hydraBaseOffset.Add(0);
				}
			}
			return this.hydraBaseOffset;
		}
	}
	
	
	#region Callbacks
	
	private List<HydraCallbacks> hydraCallbacks = null;
	public List<HydraCallbacks> HydraCallbacks
	{
		get {
			if (this.hydraCallbacks == null)
			{
				this.hydraCallbacks = new List<HydraCallbacks>(HydraControllerId.Count.IntValue());
			}
			if (this.hydraCallbacks.Count == 0)
			{
				// Initialize all Hydra controller callback objects.
				for (int i = 0; i < HydraControllerId.Count.IntValue(); i++)
				{
					this.hydraCallbacks.Add(new HydraCallbacks());
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
		if (this.InputId == InputId.Hydra
			&& this.HasSixenseInput)
		{
			SixenseInput.ControllerManagerEnabled = false;
		} else {
			SixenseInput.ControllerManagerEnabled = true;
			this.offset = new Vector3(0.0f, 0.0f, 1.0f);
		}
		this.IsCalibrated = false;
	}
	
	void Update () 
	{
		if (this.InputId == InputId.Hydra)
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
		int id = controllerId.IntValue();
		SixenseInput.Controller controller = SixenseInput.Controllers[(int) controllerId];
		
		UpdateHydraTrigger(controller, id);
		
		if (!this.IsCalibrated) { return; }
		
		// Update position data.
		UpdateHydraPosition(controller, id);
			
		// Update rotation data.
		UpdateHydraRotation(controller, id);
		
		// Update analog stick values.
		UpdateHydraAnalogStick(controller, id);
	}
	
	private void UpdateHydraTrigger (SixenseInput.Controller controller, int controllerId)
	{
		float triggerValue = controller.Trigger;
		
		if (triggerValue < InputManager.HydraSensitivity.TriggerPress)
		{
			// Trigger is not pressed.
			
			if (controller.Trigger < InputManager.HydraSensitivity.TriggerRelease) 
			{
				// Trigger is released.
				this.hydraCallbacks[controllerId].BroadcastTriggerReleaseAction(triggerValue);
			}
			return;
		}
		
		// Trigger is pressed.
		// First check for calibration.
		if (this.CanCalibrate
			&& !this.IsCalibrated)
		{
			CalibrateHydra(controller);
			return;
		}
		
		// Broatcast trigger press.
		this.hydraCallbacks[controllerId].BroadcastTriggerPressAction(triggerValue);
	}
	
	private void UpdateHydraPosition (SixenseInput.Controller controller, int controllerId)
	{
		Vector3 controllerPosition = controller.Position;
		Vector3 worldLocalPosition = new Vector3(
			controllerPosition.x * InputManager.HydraSensitivity.Position,
			controllerPosition.y * InputManager.HydraSensitivity.Position,
			controllerPosition.z * InputManager.HydraSensitivity.Position) - this.offset;
		
		this.hydraCallbacks[controllerId].BroadcastPositionAction(worldLocalPosition);
	}
	
	private void UpdateHydraRotation (SixenseInput.Controller controller, int controllerId)
	{
		Quaternion worldLocalRotation = new Quaternion(
			controller.Rotation.x,
			controller.Rotation.y,
			controller.Rotation.z,
			controller.Rotation.w);
		
		this.hydraCallbacks[controllerId].BroadcastRotationAction(worldLocalRotation);
	}
	
	private void UpdateHydraAnalogStick (SixenseInput.Controller controller, int controllerId)
	{
		Vector2 stick = new Vector2(controller.JoystickX, controller.JoystickY);
		this.hydraCallbacks[controllerId].BroadcastStickAction(stick);
	}
	
	#endregion Hydra Input Controls
	
	
	#region Gamepad Input Controls
	
	private void UpdateGamepad ()
	{
		float triggerValue = Input.GetAxis ("Triggers");

		
	}
	
	#endregion Gamepad Input Controls
	
	
	#region Calibration
	
	private void CalibrateHydra (SixenseInput.Controller controller)
	{
		this.offset = new Vector3(0.0f, 0.0f, controller.Position.z * InputManager.HydraSensitivity.Position);
		this.IsCalibrated = true;
	}
	
	#endregion Calibration
	
}

