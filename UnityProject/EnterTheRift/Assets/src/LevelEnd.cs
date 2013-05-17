using UnityEngine;
using System.Collections;

public class LevelEnd : MonoBehaviour {
	
	
	public void OnTriggerEnter(Collider collider)
	{
		if (this.enabled)
		{
			Application.LoadLevel("DansScene");
		}
	}
}
