using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip[] goals;
	public AudioClip[] stadion;
	public AudioClip[] shoot;
	public AudioClip[] knapp;
	public AudioClip[] endOfGame;
	public AudioClip[] pickup;

	public AudioSource stadionSource;
	public AudioSource stadionSource2;
	public AudioSource shootSource;
	public AudioSource goalSource;
	public AudioSource pickupSource;

	private static AudioManager instance;

	private static bool playStadionSoundBool = true;
	
	// Update is called once per frame
	void Update () {

	}

	void Start()
	{
		instance = this;

		stadionSource.loop = false;
		stadionSource2.loop = false;

		StartCoroutine( playStadionSound() );
	}

	IEnumerator playStadionSound()
	{
		while (playStadionSoundBool) {
			stadionSource.clip = getStadionAudio ();
			stadionSource.Play ();
			Debug.Log ("Play one: " + stadionSource.clip.length);
			yield return new WaitForSeconds (stadionSource.clip.length - 2);
			stadionSource2.clip = getStadionAudio ();
			stadionSource2.Play ();
			Debug.Log ("Play two: " + stadionSource2.clip.length);
			yield return new WaitForSeconds (stadionSource2.clip.length - 2);
		}
	}

	public static void pickupAudio() {
		instance.pickupSource.clip = instance.pickup [Random.Range (0, instance.pickup.Length)];
		instance.pickupSource.Play ();
	}

	public static void shootAudio() {
		instance.shootSource.clip = instance.shoot [Random.Range (0, instance.shoot.Length)];
		instance.shootSource.Play();
	}

	public static void knappAudio() {
		instance.goalSource.clip = instance.knapp [Random.Range (0, instance.knapp.Length)];
		instance.goalSource.Play();
	}

	private AudioClip getStadionAudio() {
		return stadion [Random.Range (0, instance.stadion.Length)];
	}

	public static void goalAudio() {
		instance.goalSource.clip = instance.goals [Random.Range (0, instance.goals.Length)];
		instance.goalSource.Play();
	}

	public static void endOfGameAudio() {
		AudioSource.PlayClipAtPoint (instance.endOfGame [Random.Range (0, instance.endOfGame.Length)], Vector3.zero);
	}

}
