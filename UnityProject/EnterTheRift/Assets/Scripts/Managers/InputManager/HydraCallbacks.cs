using System;
using UnityEngine;

public class HydraCallbacks
{
	#region Trigger
	
	private Action<float> TriggerPressAction { get; set; }
	public void RegisterTriggerPressAction (Action<float> action)
	{
		this.TriggerPressAction += action;
	}
	public void UnregisterTriggerPressAction (Action<float> action)
	{
		this.TriggerPressAction -= action;
	}
	public void BroadcastTriggerPressAction (float input)
	{
		if (this.TriggerPressAction == null) { return; }
		this.TriggerPressAction(input);
	}
	
	private Action<float> TriggerReleaseAction { get; set; }
	public void RegisterTriggerReleaseAction (Action<float> action)
	{
		this.TriggerReleaseAction += action;
	}
	public void UnregisterTriggerReleaseAction (Action<float> action)
	{
		this.TriggerReleaseAction -= action;
	}
	public void BroadcastTriggerReleaseAction (float input)
	{
		if (this.TriggerReleaseAction == null) { return; }
		this.TriggerReleaseAction(input);
	}
	
	#endregion Trigger
	
	
	#region Stick
	
	private Action<Vector2> StickAction { get; set; }
	public void RegisterStickAction (Action<Vector2> action)
	{
		this.StickAction += action;
	}
	public void UnregisterStickAction (Action<Vector2> action)
	{
		this.StickAction -= action;
	}
	public void BroadcastStickAction (Vector2 input)
	{
		if (this.StickAction == null) { return; }
		this.StickAction(input);
	}
	
	#endregion Stick
	
	
	#region Position
	
	private Action<Vector3> PositionAction { get; set; }
	public void RegisterPositionAction (Action<Vector3> action)
	{
		this.PositionAction += action;
	}
	public void UnregisterPositionAction (Action<Vector3> action)
	{
		this.PositionAction -= action;
	}
	public void BroadcastPositionAction (Vector3 input)
	{
		if (this.PositionAction == null) { return; }
		this.PositionAction(input);
	}
	
	#endregion Position
	
	
	#region Rotation
	
	private Action<Quaternion> RotationAction { get; set; }
	public void RegisterRotationAction (Action<Quaternion> action)
	{
		this.RotationAction += action;
	}
	public void UnregisterRotationAction (Action<Quaternion> action)
	{
		this.RotationAction -= action;
	}
	public void BroadcastRotationAction (Quaternion input)
	{
		if (this.RotationAction == null) { return; }
		this.RotationAction(input);
	}
	
	#endregion Rotation
	
}
