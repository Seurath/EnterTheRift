using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{

	public float timeLeft;
	
	// Update is called once per frame
	void Update ()
	{
		timeLeft -= Time.deltaTime;
		if (timeLeft < 0.0f) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
