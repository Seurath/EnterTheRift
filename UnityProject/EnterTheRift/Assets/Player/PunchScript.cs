using UnityEngine;
using System.Collections;

public class PunchScript : MonoBehaviour {
	private Camera playerCamera = null;
	
	// Use this for initialization
	void Start () {
		playerCamera = (Camera)GetComponentInChildren(typeof(Camera));
		if(playerCamera == null)
		{
			Debug.LogError ("Could not find Camera for PunchScript.");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown ("Fire2"))
		{
			Vector3 forwardVector;
			if(playerCamera != null)
			{
				forwardVector = playerCamera.transform.TransformDirection (Vector3.forward);
			}
			else
			{
				forwardVector = transform.TransformDirection (Vector3.forward);
			}
			
			RaycastHit[] results = Physics.RaycastAll(transform.position, forwardVector);
			
			foreach(RaycastHit result in results)
			{
				if(result.rigidbody != null)
				{
					
					result.rigidbody.AddForceAtPosition (forwardVector * 1000, result.point);
				}
				result.collider.BroadcastMessage ("Punched");
			}
		}
	}
}
