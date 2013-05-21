using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float damage = 5.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		DamageController damageController = (DamageController)collision.gameObject.GetComponent (typeof(DamageController));
		if(damageController != null)
		{
			damageController.NotifyDamage(damage);
		}
	}
}
