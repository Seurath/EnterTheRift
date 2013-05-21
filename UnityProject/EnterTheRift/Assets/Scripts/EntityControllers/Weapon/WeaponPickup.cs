using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {
	
	public Weapon weaponType;
	public float rotateSpeed = 5.0f;
	public float rearmDelay = 10.0f;
	
	private Weapon weaponInstance;
	private float rearmTimer = 0.0f;
	
	private void CreateWeapon()
	{
		if(weaponType)
		{
			weaponInstance = (Weapon)Instantiate (weaponType, transform.position, transform.rotation);
		}
	}
	
	// Use this for initialization
	void Start () {
		CreateWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		if(weaponInstance != null)
		{
			weaponInstance.transform.Rotate (0.0f, rotateSpeed, 0.0f);
		}
		else
		{
			rearmTimer -= Time.deltaTime;
			if(rearmTimer < 0.0f)
			{
				CreateWeapon ();
			}
		}
	}
	
	public Weapon GetWeaponInstance()
	{
		return(weaponInstance);
	}
	
	public void TakeWeapon()
	{
		weaponInstance = null;
		rearmTimer = rearmDelay;
	}
}
