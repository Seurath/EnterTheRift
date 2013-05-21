using UnityEngine;
using System.Collections;

public class KeyholeScript : MonoBehaviour {
	public GameObject key;
	
	public string unlockedMessage = "KeyUnlocked";
	public string lockedMessage = "KeyLocked";
	
	public GameObject messageRecipient;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == key || other.gameObject.transform.parent == key)
		{
			if(messageRecipient != null)
			{
				messageRecipient.BroadcastMessage(unlockedMessage);
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject == key || other.gameObject.transform.parent == key)
		{
			if(messageRecipient != null)
			{
				messageRecipient.BroadcastMessage(lockedMessage);
			}
		}
	}
}
