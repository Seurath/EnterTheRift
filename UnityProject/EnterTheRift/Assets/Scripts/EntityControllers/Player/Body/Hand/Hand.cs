using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
	public bool useHydra = true;
	
	public int id;
	public PunchPlayer player;
	
	public Vector3 offset;
	
	private bool triggerDown = false;
	public bool canSetShoulders = true;
	public bool shoulderSet = false;
	
	public Vector2 leftStick;
	public Vector2 rightStick;
	
	public Vector3 handWorldPosition;
	
	public GameObject skeletalHand;
	public HandGrab dynamicCollider;
	
	public Transform cameraMount;
	
	public bool fistsDisabled = true;
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void Start(){
		if(useHydra)
		{
			SixenseInput.ControllerManagerEnabled = false;
		}
		else
		{
			offset = new Vector3(0.0f, 0.0f, 1.0f);
		}
		
		if(player == null)
		{
			Debug.LogError ("Hand needs a PunchPlayer");
		}
	}
	
	void Update () {
		// For use with hydra controller
		if(useHydra)
		{
			//position data
			SixenseInput.Controller controller = SixenseInput.Controllers[id];
			Vector3 controllerPosition = controller.Position;
			Vector3 desiredLocalPosition = new Vector3(
												controllerPosition.x * .005f,
												controllerPosition.y * .005f,
												controllerPosition.z * .005f) - offset;
			if(cameraMount != null)
			{
				desiredLocalPosition = cameraMount.transform.localRotation * desiredLocalPosition;
			}
			
			if(fistsDisabled)
			{
				if(id == 0)
				{
					desiredLocalPosition = new Vector3(-1.0f, -0.5f, 1.0f);
				}
				else
				{
					desiredLocalPosition = new Vector3(1.0f, -0.5f, 1.0f);
				}
			}
			transform.localPosition = desiredLocalPosition;
				
			//rotation data
			transform.localRotation = new Quaternion(
												controller.Rotation.x,
												controller.Rotation.y,
												controller.Rotation.z,
												controller.Rotation.w);
			
			if(fistsDisabled)
			{
				transform.localRotation = new Quaternion();
			}
			
			
			if(canSetShoulders)
			{
				if(controller.Trigger >= 0.9 && !triggerDown) {
					triggerDown = true;
					
					// Allow the user to set the base position by pressing the trigger
					if(!shoulderSet)
					{
						offset = new Vector3(0.0f, 0.0f, controllerPosition.z * 0.005f);
						shoulderSet = true;
					}
					else if(skeletalHand != null)
					{
						skeletalHand.animation.Play("fist");
						dynamicCollider.GrabItems();
					}
				}
				else if(triggerDown && controller.Trigger < 0.05) {
					triggerDown = false;
					if(skeletalHand != null)
					{
						skeletalHand.animation.Play ("unfist");
						dynamicCollider.LetGoOfItems();
					}
				}
			}
			
			// Send analog stick values to the player, for movement purposes
			if(id == 0) {
				leftStick = new Vector2(controller.JoystickX, controller.JoystickY);
			}
			else if(id == 1) {
				rightStick = new Vector2(controller.JoystickX, controller.JoystickY);
			}
		}
		else // For use with an xbox controller
		{
			float triggerValue = Input.GetAxis ("Triggers");

			if(id == 0)
			{
				if(triggerValue > 0.8f)
				{
					leftStick = new Vector2();
					transform.localPosition = new Vector3(
												(Mathf.Clamp (Input.GetAxis ("HorizontalL"), -0.75f, 0.75f) - 0.25f) * 2.0f,
												Mathf.Clamp (Input.GetAxis ("VerticalR"), -0.75f, 0.75f) * -2.0f,
												Mathf.Clamp (Input.GetAxis ("VerticalL"), -0.75f, 0.75f) + 0.25f) +
											  offset;
					
					if(!triggerDown && skeletalHand != null)
					{
						skeletalHand.animation.Play("fist");
						triggerDown = true;
					}
				}
				else
				{
					leftStick = new Vector2(Input.GetAxis ("HorizontalL"), Input.GetAxis ("VerticalL"));
					
					if(triggerDown && skeletalHand != null)
					{
						skeletalHand.animation.Play("unfist");
						triggerDown = false;
					}
				}
			}
			else if(id == 1)
			{
				if(triggerValue < -0.8f)
				{
					rightStick = new Vector2();
					transform.localPosition = new Vector3(
												(Mathf.Clamp (Input.GetAxis ("HorizontalL"), -0.75f, 0.75f) + 0.25f) * 2.0f,
												Mathf.Clamp (Input.GetAxis ("VerticalR"), -0.75f, 0.75f) * -2.0f,
												Mathf.Clamp (Input.GetAxis ("VerticalL"), -0.75f, 0.75f) + 0.25f) +
											  offset;
					
					if(!triggerDown && skeletalHand != null)
					{
						skeletalHand.animation.Play("fist");
						triggerDown = true;
					}
				}
				else
				{
					rightStick = new Vector2(Input.GetAxis ("HorizontalR"), Input.GetAxis("VerticalR"));
					
					if(triggerDown && skeletalHand != null)
					{
						skeletalHand.animation.Play("unfist");
						triggerDown = false;
					}
				}
			}
		}
		
		if(id == 0)
		{
			player.joystickLeft = leftStick;
		}
		else if(id == 1)
		{
			player.joystickRight = rightStick;
		}
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////