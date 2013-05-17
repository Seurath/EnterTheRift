using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandGrab : MonoBehaviour {
	
	[SerializeField] private AudioSource grabSound;
	
	public List<Transform> itemsInReach = new List<Transform>();
	public List<Transform> heldItems = new List<Transform>();
	
	public LayerMask[] ungrabbable;
	
	
	
	public void GrabItems()
	{
		foreach(Transform item in itemsInReach)
		{
			if(item.rigidbody != null)
			{
				FixedJoint joint = item.gameObject.AddComponent<FixedJoint>();
				if(joint != null)
				{
					joint.connectedBody = rigidbody;
					heldItems.Add (item);
					PlayGrabSound();
				}
			}
		}
	}
	
	public void LetGoOfItems()
	{
		foreach(Transform item in heldItems)
		{
			FixedJoint joint = item.gameObject.GetComponent<FixedJoint>();
			if(joint != null)
			{
				Destroy(joint);
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(!itemsInReach.Contains (other.transform) && other.rigidbody != null && !other.rigidbody.isKinematic)
		{
			foreach(LayerMask mask in ungrabbable)
			{
				int maskVal = mask.value;
				int layerPowd = (1 << other.gameObject.layer);
				if(mask.value == layerPowd)
				{
					return;
				}
			}
			
			itemsInReach.Add (other.transform);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		itemsInReach.Remove (other.transform);
	}
	
	private void PlayGrabSound ()
	{
		if (this.grabSound == null) { return; }
		this.grabSound.Play(); 
	}
}
