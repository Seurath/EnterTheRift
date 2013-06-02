using System;
using UnityEngine;

public class HydraCallbacks
{
	public Action<Vector3> TriggerAction { get; private set; }
	public void RegisterTriggerAction (Action<Vector3> action)
	{
		this.TriggerAction += action;
	}
	public void UnregisterTriggerAction (Action<Vector3> action)
	{
		this.TriggerAction -= action;
	}
	
	public Action<Vector2> StickAction { get; private set; }
	public void RegisterStickAction (Action<Vector2> action)
	{
		this.StickAction += action;
	}
	public void UnregisterStickAction (Action<Vector2> action)
	{
		this.StickAction -= action;
	}
	
	public Action<Vector3> PositionAction { get; private set; }
	public void RegisterPositionAction (Action<Vector3> action)
	{
		this.PositionAction += action;
	}
	public void UnregisterPositionAction (Action<Vector3> action)
	{
		this.PositionAction -= action;
	}
	
	public Action<Vector3> RotationAction;
	public void RegisterRotationAction (Action<Vector3> action)
	{
		this.RotationAction += action;
	}
	public void UnregisterRotationAction (Action<Vector3> action)
	{
		this.RotationAction -= action;
	}
}
