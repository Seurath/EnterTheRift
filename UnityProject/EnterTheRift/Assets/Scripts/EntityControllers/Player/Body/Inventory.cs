using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
	// Might want to expand this to allow a dude to hold more weapons... whatev
	private Weapon currentWeapon = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && currentWeapon != null)
		{
			currentWeapon.FireProjectile();
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		WeaponPickup weaponPickup = (WeaponPickup)other.GetComponent(typeof(WeaponPickup));
		if(weaponPickup != null)
		{
			Weapon pickupsWeapon = weaponPickup.GetWeaponInstance();
			if(pickupsWeapon != null)
			{
				Destroy(currentWeapon);
				currentWeapon = pickupsWeapon;
				weaponPickup.TakeWeapon();
				
				currentWeapon.transform.parent = this.transform;
				// TODO:  data drive weapon offset somehow.  Presumably this will be attached to a bone or something
				currentWeapon.transform.localPosition = new Vector3(1.0f, -0.05f, 1.0f);
				currentWeapon.transform.localRotation = new Quaternion();
			}
		}
	}
}
