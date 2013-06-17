using System;
using UnityEngine;

public class HydraCallbacks
{
	private Action<Vector3> TriggerAction { get; set; }
	public void RegisterTriggerAction (Action<Vector3> action)
	{
		this.TriggerAction += action;
	}
	public void UnregisterTriggerAction (Action<Vector3> action)
	{
		this.TriggerAction -= action;
	}
	public void BroadcastTriggerAction (Vector3 input)
	{
		if (this.TriggerAction == null) { return; }
		this.TriggerAction(input);
	}
	
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
}
