using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

		
	public GameObject parent;
	public GameObject kill;
	public float torqueStrength = 500f;
	
	void OnCollisionEnter(Collision collision)  
	{
		Collider other = collision.collider;
	
		other.attachedRigidbody.AddTorque(new Vector3(Random.value * torqueStrength, Random.value * torqueStrength, Random.value* torqueStrength));
		
		GameObject.Instantiate(kill, this.transform.position, this.transform.rotation);
		Destroy(parent);
		
	}
	
}
