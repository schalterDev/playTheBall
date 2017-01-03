using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCollission : NetworkBehaviour {

	private NetworkInstanceId networkInstanceId;
	private PlayerInfoController playerInfoController;

	public bool shooting;
	private bool shootingBefore;

	private Rigidbody ball;
	private Transform shootingBar;

	// Use this for initialization
	void Start () {
		networkInstanceId = this.GetComponent<NetworkIdentity> ().netId;
		playerInfoController = this.GetComponent<PlayerInfoController>();

		shooting = false;
		shootingBefore = false;
	}


	void Update()
	{
		if (isLocalPlayer) {
			//Shoot the ball
			int inputCount = 1;

			// if the user want to use joystick only shoot wiht two touches
			if (PlayerPrefs.GetInt (PreferenceManager.CONTROL) == PreferenceManager.JOYSTICK) {
				inputCount = 2;
			}

			if (Input.GetKey (KeyCode.Space) || Input.touchCount >= inputCount) {
				if (!shootingBefore) {
					shootingBefore = true;
					CmdShoot (shootingBefore);
				}
			} else {
				if (shootingBefore) {
					shootingBefore = false;
					CmdShoot (shootingBefore);
				}
			}
		}
	}
		
	void OnTriggerEnter(Collider otherCollider)
	{
		if (isServer) {
			if (otherCollider.gameObject.CompareTag ("Pickup")) {
				//can only pass the name and not the object
				CollissionPickUp (this.gameObject.name, otherCollider.name);
			} 
		}
	}
		
	void OnCollisionEnter(Collision other)
	{
		if (isServer) {
			if (other.gameObject.CompareTag ("Ball")) {
				if(shooting)
					RpcShoot ();
			}
		}
	}
		
	void OnCollisionStay(Collision collisionInfo)
	{
		if (isServer) {
			if (collisionInfo.gameObject.CompareTag ("Ball")) {
				if (shooting) {
					// the client presses the buttons for shooting
					if (Time.time - playerInfoController.lastShoot > playerInfoController.actualShootingInterval) {
						shoot ();
					}
				}
			}
		}
	}
			
	[Server]
	private void shoot () {
		//Ball
		if (ball == null)
			ball = GameObject.Find ("Ball").GetComponent<Rigidbody> ();

		Vector3 direction = ball.transform.position - transform.position;
		ball.GetComponent<Rigidbody> ().AddForce (direction.normalized * playerInfoController.actualShootingPower, ForceMode.Impulse);

		playerInfoController.lastShoot = Time.time;

		RpcShoot ();
	}

	[Command]
	private void CmdShoot(bool shoot) {
		shooting = shoot;
	}

	[ClientRpc]
	private void RpcShoot() {
		//TODO only local player? no but then audiomanager has to know the position
		AudioManager.shootAudio ();
	}

	[Server]
	private void CollissionPickUp(string playerName, string pickupName) {
		Effect effect = new Effect (playerName, pickupName, Time.time);
		EffectManager.instance.addEffect (effect);

		AudioManager.pickupAudio ();
	}
}
