using UnityEngine;
using System.Collections;

public class PlaneController : MonoBehaviour
{
	[SerializeField] private PlaneAudioController audioController;
	
	public bool xboxMode;
	
	public float Thrust = 10f;
	public float liftStrength = 1f;
	public float rollRate = 1.0f;
	public float pitchRate = 1.0f;
	public float turnTorque = 1.0f;
	public float yawRate = 10f;
	
	public float reorientRate = 10f;
	
	private float horizontalAxis;
	private float verticalAxis;
	private float yawAxis;
	
	private float leftY;
	private float rightY;
	
	private float throttle;
	
	
	public float pitchLowerLimit = -15f;
	public float pitchUpperLimit = 15f;
	public float rollLowerLimit;
	public float rollUpperLimit;
	
	public GameObject bulletPrefab;
	public GameObject bulletSpawn;
	public GameObject bulletSpawn2;
	public float timeSinceLastSpawn = 0.25f;
	
	public GameObject tiltObject;
	
	private float tiltFix;
	
	public float fixOrientSpeed = 10f;
	
	public GameObject leftHand;
	public GameObject rightHand;
	
	// Use this for initialization
	void Start ()
	{
		SixenseInput.ControllerManagerEnabled = false;
	}

	private Vector3 lift;
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(xboxMode)
		{
			verticalAxis = Input.GetAxis("VerticalL");
			yawAxis = Input.GetAxis("HorizontalL");
			throttle = 1;
		} else{
		
			horizontalAxis = SixenseInput.Controllers[0].Rotation.y;
			verticalAxis = SixenseInput.Controllers[0].Rotation.x;
			
			yawAxis = SixenseInput.Controllers[0].Rotation.z;
			throttle = SixenseInput.Controllers[0].JoystickY;			
			tiltFix = SixenseInput.Controllers[0].JoystickX;
		}
		
		this.audioController.PlayEngineSound(throttle > 0);
		
		leftHand.transform.localRotation = Quaternion.Euler(new Vector3(verticalAxis * 30f, 0f, horizontalAxis * 100f));
		rightHand.transform.localRotation = Quaternion.Euler(new Vector3(verticalAxis * 30f, 0f, horizontalAxis * 100f));
		
		
		leftY = SixenseInput.Controllers[0].Position.y;
		rightY = SixenseInput.Controllers[1].Position.y;
		
		
		float angleOfAttack = -Mathf.Deg2Rad * Vector3.Dot (rigidbody.velocity, transform.up);
		Vector3 LiftVector = this.transform.up * angleOfAttack * liftStrength;
		
		
		
		
		this.rigidbody.AddRelativeForce (Vector3.forward * Thrust * throttle);
		this.rigidbody.AddRelativeTorque (new Vector3 (verticalAxis * pitchRate, yawAxis * yawRate, horizontalAxis * -rollRate));
		rigidbody.AddForce (LiftVector, ForceMode.Force);
		
		// determine the local roll.
		float zRot = this.transform.localRotation.eulerAngles.z;
		float currentRoll = 180f - Mathf.Abs (180f - zRot);
		if (zRot <= 180f) {
			currentRoll *= -1;
		}
		
	
	
		this.rigidbody.AddTorque (new Vector3 (0, currentRoll * turnTorque, 0));		
		
		timeSinceLastSpawn += Time.deltaTime;
		
		Quaternion level = Quaternion.Euler(new Vector3(this.transform.localRotation.eulerAngles.x,this.transform.localRotation.eulerAngles.y , 0));
		this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, level, reorientRate * Time.deltaTime);			
		
		if ((SixenseInput.Controllers[0].GetButton(SixenseButtons.FOUR) || SixenseInput.Controllers[0].GetButton(SixenseButtons.TRIGGER)) && timeSinceLastSpawn > 0.25f)
		{
			timeSinceLastSpawn = 0f;
			GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
			GameObject.Instantiate(bulletPrefab, bulletSpawn2.transform.position, bulletSpawn2.transform.rotation);
			
			this.audioController.PlayBulletSound();
		}
		
		tiltObject.transform.localRotation = Quaternion.Euler(new Vector3(0,0, -yawAxis * yawRate * 0.1f));
		
		if (SixenseInput.Controllers[0].GetButton(SixenseButtons.BUMPER))
		{
			level = Quaternion.Euler(0,0,0);
			this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, level, fixOrientSpeed * Time.deltaTime);			
		
		}
		
	}
}