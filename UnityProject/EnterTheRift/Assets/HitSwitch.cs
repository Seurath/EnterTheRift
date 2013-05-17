using UnityEngine;
using System.Collections;

public class HitSwitch : MonoBehaviour 
{
	[SerializeField] private AudioSource switchSound;
	
	public GameObject messageRecipient;
//	public string scriptType;
	public string switchHitMessage;
	
	public bool switchEnabled = true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(enabled && switchEnabled && messageRecipient != null)
		{
			Debug.Log ("HIT");
			messageRecipient.BroadcastMessage(switchHitMessage);
			PlaySwitchSound();
			enabled = false;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(switchEnabled && messageRecipient != null)
		{
//			messageRecipient.BroadcastMessage(switchHitMessage);
		}
	}
	
	void EnableSwitch()
	{
		PlaySwitchSound();
		switchEnabled = true;
	}
	
	void DisableSwitch()
	{
		switchEnabled = false;
	}
	
	private void PlaySwitchSound ()
	{
		if (this.switchSound == null) { return; }
		this.switchSound.Play();
	}
}
