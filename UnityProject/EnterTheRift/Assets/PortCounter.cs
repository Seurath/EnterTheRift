using UnityEngine;
using System.Collections;

public class PortCounter : MonoBehaviour {
	
	public int counterTarget = 1;
	private int currentCounter = 0;
	
	public string counterReachedMessage = "CounterReached";
	public GameObject messageRecipient;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void IncrementCounter()
	{
		++currentCounter;
		if(currentCounter >= counterTarget)
		{
			if(messageRecipient != null)
			{
				messageRecipient.BroadcastMessage (counterReachedMessage);
			}
		}
	}
	
	public void DecrementCounter()
	{
		--currentCounter;
	}
}
