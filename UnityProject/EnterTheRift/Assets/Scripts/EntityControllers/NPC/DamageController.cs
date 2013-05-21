using UnityEngine;
using System.Collections;

public class DamageController : MonoBehaviour {
	public float maxHealth = 10.0f;
	
	private float currentHealth;
	
	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Die()
	{
		Debug.Log ("DEAD!");
		// Maybe could spawn some sort of explosion effect?
		Destroy (gameObject);
	}
	
	public void NotifyDamage(float damage)
	{
		currentHealth -= damage;
		Debug.Log ("Damaged!  Health remaining: " + currentHealth.ToString ());
		
		if(currentHealth <= 0.0f)
		{
			Die();
		}
	}
}
