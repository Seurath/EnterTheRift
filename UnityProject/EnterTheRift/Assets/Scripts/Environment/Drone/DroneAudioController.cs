using UnityEngine;
using System.Collections;

public class DroneAudioController : MonoBehaviour
{

	[SerializeField] private AudioSource bulletSound;
	[SerializeField] private AudioSource deathSound;
	
	public void PlayBulletSound ()
	{
		if (this.bulletSound == null) { return; }
		this.bulletSound.Play();
	}
	
	public void PlayDeathSound ()
	{
		if (this.deathSound == null) { return; }
		this.deathSound.Play();
	}
	
}

