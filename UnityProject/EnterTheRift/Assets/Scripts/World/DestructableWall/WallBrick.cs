using UnityEngine;
using System.Collections;

public class WallBrick : MonoBehaviour {

	public Color strongColor = new Color(64.0f, 64.0f, 64.0f);
	public Color weakColor = new Color(0.0f, 0.0f, 0.0f);
	
	public bool isWeak = false;
	
	public float destructionThreshold = 3.0f;
	
	private DestructibleWall wall = null;
	
	// Use this for initialization
	void Start () {
		if(!isWeak)
		{
			renderer.material.color = strongColor;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetWall(DestructibleWall wall)
	{
		this.wall = wall;
	}
	
	public void SetIsWeak(bool weak)
	{
		isWeak = weak;
		renderer.material.color = weakColor;
	}
	
	void OnCollisionEnter(Collision collisionInfo)
	{
		if(collisionInfo.gameObject.CompareTag("Fist") && collisionInfo.relativeVelocity.sqrMagnitude > destructionThreshold * destructionThreshold)
		{
			Punched ();
			rigidbody.AddForce (collisionInfo.impactForceSum);
		}
	}
	
	private void Punched()
	{
		if(isWeak && rigidbody.isKinematic)
		{
			rigidbody.isKinematic = false;
			wall.NotifyBrickPunched(this);
		}
	}
}
