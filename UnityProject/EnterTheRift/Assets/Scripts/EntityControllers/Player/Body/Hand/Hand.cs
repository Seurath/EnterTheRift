using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand : MonoBehaviour 
{
	[SerializeField] private Animation handAnimation = null;
	[SerializeField] private HandGrab dynamicCollider = null;
	[SerializeField] private Vector3 handWorldPosition;
	
	public bool IsFistEnabled { get; set; }
	
	
	#region MonoBehaviour
	
	void Awake ()
	{
		Initialize();
	}
	
	#endregion MonoBehaviour
	
	
	#region Initialization
	
	private void Initialize ()
	{
		this.IsFistEnabled = false;
	}
	
	#endregion Initialization
	
	
	#region Hand Control
	
	public void SetHandFist (bool isEnabled)
	{
		if (this.IsFistEnabled == isEnabled) 
		{
			// The hand is already in the set state. 
			// No need to do anything.
			return; 
		}
		
		this.IsFistEnabled = isEnabled;
		bool isHandAnimationSet = this.handAnimation != null;
		
		if (isEnabled)
		{
			// Fist
			this.dynamicCollider.GrabItems();
			
			if (isHandAnimationSet)
			{
				this.handAnimation.Play("fist");
			}
		} 
		else 
		{
			// Unfist
			this.dynamicCollider.LetGoOfItems();
			
			if (isHandAnimationSet)
			{
				this.handAnimation.Play("unfist");
			}
		}
	}
	
	public void SetPosition (Vector3 position)
	{
		this.transform.localPosition = position;
	}
	
	public void SetRotation (Quaternion rotation)
	{
		this.transform.localRotation = rotation;
	}
	
	#endregion Hand Control
	
	
	#region Gamepad Input Controls
	
	/*
	private void UpdateGamepad ()
	{
		float triggerValue = Input.GetAxis ("Triggers");

		if (controllerId == 0)
		{
			if (triggerValue > 0.8f)
			{
				leftStick = new Vector2();
				transform.localPosition = new Vector3(
					(Mathf.Clamp (Input.GetAxis ("HorizontalL"), -0.75f, 0.75f) - 0.25f) * 2.0f,
					Mathf.Clamp (Input.GetAxis ("VerticalR"), -0.75f, 0.75f) * -2.0f,
					Mathf.Clamp (Input.GetAxis ("VerticalL"), -0.75f, 0.75f) + 0.25f) +
					offset;
				
				if (!isTriggerDown 
					&& handAnimation != null)
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
	*/
	
	#endregion Gamepad Input Controls
	

}
