using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NpcMovement : MonoBehaviour {
	public string playerTag = "Player";	
	public float movementSpeed = 5.0f;
	public float rotateSpeed = 5.0f;
	
	private CharacterController cc = null;
	
	// Use this for initialization
	void Start () {
		cc = (CharacterController)GetComponent (typeof(CharacterController));
		if(cc == null)
		{
			Debug.LogError ("No character controller for NpcMovement script");
		}
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindWithTag(playerTag);
		if(player == null)
		{
			// No player.. just idle or something
			return;
		}
		
		transform.LookAt (player.transform.position);
		
		cc.SimpleMove (transform.TransformDirection(new Vector3(0.0f, 0.0f, movementSpeed)));
	}
}
