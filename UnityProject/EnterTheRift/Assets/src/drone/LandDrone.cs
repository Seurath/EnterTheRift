using UnityEngine;
using System.Collections;

public class LandDrone : MonoBehaviour
{
	
	public GameObject target;
	public float rotationRate;
	public float speed;
	public float distanceCutoffNear;
	public float distanceCutoffFar;
	public GameObject bullet;
	public GameObject killEffect;
	public float distance;
	// Use this for initialization
	void Start ()
	{
		//target = GameObject.Find ("fighter");
		target = GameObject.FindGameObjectWithTag("Player");
	}
	
	
	private float hitDelay = 0f;
	private int numHits = 0;
	public int hitsToKill = 3;
	// Update is called once per frame
	void Update ()
	{	
		hitDelay -= Time.deltaTime;
		if (target == null) {
			return;	
		}
		
		distance = Vector3.Distance (this.transform.position, target.transform.position);
	
		Vector3 targetDirection = this.target.transform.position - this.transform.position;
		Quaternion targetAngle = Quaternion.LookRotation (targetDirection);
		
		Quaternion resultDirection = Quaternion.RotateTowards (this.transform.rotation, targetAngle, Time.deltaTime * rotationRate);
		
		
		
		this.transform.rotation = resultDirection;
		
		
		if (distance > distanceCutoffNear && distance < distanceCutoffFar) {
			
				
			this.rigidbody.AddRelativeForce (Vector3.forward * speed * Time.deltaTime);
		}
		
		
	}
	
	
	void OnCollisionEnter(Collision collisionInfo)
	{
		
		
		if(collisionInfo.gameObject.CompareTag("Fist") && hitDelay <= 0f)
		{
			Debug.Log("Hit!");
			numHits ++;
			//rigidbody.AddForce (collisionInfo.impactForceSum);
			hitDelay = 1f;
			
			if (numHits >= hitsToKill)
			{
				GameObject.Instantiate(killEffect, this.transform.position, this.transform.rotation);
				Destroy(this.gameObject);	
			}
			
		}
	}
	
}
