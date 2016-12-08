using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCollission : NetworkBehaviour {

	private NetworkInstanceId networkInstanceId;

	public float shootingPower;
	public float maxShootingInterval; //in seconds

	private Rigidbody ball;
	private Transform shootingBar;

	private float lastShoot;

	// Use this for initialization
	void Start () {
		lastShoot = Time.time;

		loadObjects ();

		networkInstanceId = this.GetComponent<NetworkIdentity> ().netId;
	}

	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}
	}
		
	private void loadObjects() {
		//Ball
		//ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
	}
		
	void OnTriggerEnter(Collider other)
	{
		if (isServer) {
			if (other.gameObject.CompareTag ("Pickup")) {
				//can only pass the name
				CollissionPickUp (this.gameObject.name, other.name);
			} 
		}
	}
		
	void OnCollisionEnter(Collision other)
	{
		if (isServer) {
			if (other.gameObject.CompareTag ("Ball")) {
				RpcShoot ();
			}
		}
	}
		
	void OnCollisionStay(Collision collisionInfo)
	{
		if (isServer) {
			if (collisionInfo.gameObject.CompareTag ("Ball")) {
				RpcShoot ();
			}
		}
	}

	[ClientRpc]
	private void RpcShoot () {
		if (!isLocalPlayer)
			return;

		//Shoot the ball
		int inputCount = 1;

		// if the user want to use joystick only shoot wiht two touches
		if (PlayerPrefs.GetInt (PreferenceManager.CONTROL) == PreferenceManager.JOYSTICK) {
			inputCount = 2;
		}

		if (Input.GetKey (KeyCode.Space) || Input.touchCount >= inputCount) {
			if (Time.time - lastShoot > maxShootingInterval) {
				CmdShoot ();
				lastShoot = Time.time;

				base.GetComponent<PlayerInfoController>().shot();
			}
		}
	}

	[Command]
	public void CmdShoot()
	{
		//Ball
		if (ball == null)
			ball = GameObject.Find ("Ball").GetComponent<Rigidbody> ();

		Vector3 direction = ball.transform.position - transform.position;
		ball.GetComponent<Rigidbody> ().AddForce (direction.normalized * shootingPower, ForceMode.Impulse);
	}

	private void CollissionPickUp(string playerName, string pickupName) {
		Effect effect = new Effect (playerName, pickupName, Time.time);

		EffectManager.instance.addEffect (effect);
	}
}
