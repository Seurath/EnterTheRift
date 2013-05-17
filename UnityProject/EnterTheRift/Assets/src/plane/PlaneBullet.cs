using UnityEngine;
using System.Collections;

public class PlaneBullet : MonoBehaviour {
	
	
	public float speed = 10f;
	public GameObject explosion;
	public GameObject kill;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate(new Vector3(0,0, speed * Time.deltaTime));
	}
	
	void OnTriggerEnter(Collider other)  
	{
		Drone myDrone = other.gameObject.GetComponentInChildren<Drone>();
		if (myDrone != null)
		{
			//Debug.Log("HIT DRONE");
			myDrone.Death();
			GameObject.Instantiate(kill, this.transform.position, this.transform.rotation);
			Destroy(other.gameObject);
		}
		
		//Debug.Log("COLLISION ENTER");
		GameObject.Instantiate(explosion, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}
}
