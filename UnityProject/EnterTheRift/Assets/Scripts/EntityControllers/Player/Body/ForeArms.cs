using UnityEngine;
using System.Collections;

public class ForeArms : MonoBehaviour
{
	[SerializeField] private GameObject tracker = null;
	
	void Update ()
	{
		Vector3 angles = this.transform.localEulerAngles;
		this.transform.localEulerAngles = new Vector3(
			angles.x, 
			this.tracker.transform.eulerAngles.z, 
			angles.z);
	}
}
