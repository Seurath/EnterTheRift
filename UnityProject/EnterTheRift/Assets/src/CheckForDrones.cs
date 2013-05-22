using UnityEngine;
using System.Collections;

public class CheckForDrones : MonoBehaviour {
	
	private float timeLeft = 0f;
	public GameObject levelEnd;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeLeft -= Time.deltaTime;
		
		if (timeLeft < 0f)
		{
			timeLeft = 1f;
			
			Object droneLeft = GameObject.FindObjectOfType(typeof(Drone));
			if(droneLeft == null)
			{
				Debug.Log("Drones are dead!");
				//Application.LoadLevel("JakTestLevel");
				levelEnd.SetActive(true);
			}
			
		}
	}
}
