using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	public Vector3 alignedLaunchForce = new Vector3(0.0f, 0.0f, 1000.0f);
	public float upwardsLaunchForce = 250.0f;
	public Vector3 launchSpinForce = new Vector3(0.0f, 0.0f, 0.0f);
	
	public Projectile projectileType;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void FireProjectile()
	{
		if(projectileType != null)
		{
			Projectile newProjectile = (Projectile)Instantiate (projectileType, transform.position, transform.rotation);
			
			if(newProjectile != null)
			{
				newProjectile.rigidbody.AddForce(transform.TransformDirection (alignedLaunchForce) + new Vector3(0.0f, upwardsLaunchForce, 0.0f));
				newProjectile.rigidbody.AddRelativeTorque(launchSpinForce, ForceMode.VelocityChange);
			}
		}
	}
}
