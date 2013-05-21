using UnityEngine;
using System.Collections;

public class DroneBullet : MonoBehaviour {
	
	
	public float speed = 10f;
	public GameObject explosion;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate(new Vector3(0,0, speed * Time.deltaTime));
	}
	
	void OnTriggerEnter(Collider other)  
	{
		
		GameObject.Instantiate(explosion, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}
}
