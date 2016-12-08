using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EffectManager : NetworkBehaviour {

	private string ownPlayerName;

	public List<Effect> effects;
	public List<Effect> effectsServer;

	private static PlayerInfoController playerInfoController;
	private static PlayerControlNetwork playerControlNetwork;
	private static PlayerCollission playerCollission;

	public static EffectManager instance;

	public VerticalLayoutGroup effectLayout;
	public GameObject effectPrefab;
	public GameObject pickupPartical;

	void Awake() {
		instance = this;

		effects = new List<Effect> ();
		effectsServer = new List<Effect> ();
		effectsScreen = new List<Image> ();
	}

	// Use this for initialization
	void Start () {
		ownPlayerName = PlayerInfoController.nameInScene;
	}
	
	// Update is called once per frame
	void Update () {
		if(isServer)
			updateEffects ();

		clienUpdateEffects ();
	}

	[Server]
	public void addEffect(Effect effect) {
		ownPlayerName = PlayerInfoController.nameInScene;

		PickUp pickup = effect.getPickup();
		string playerName = effect.getPlayer ();

		if (pickup.effect == PickUp.SPEED) {

			CmdSetSpeed (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.MAXSPEED) {

			CmdSetMaxSpeed (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.SHOOTINGPOWER) {

			CmdSetShootingPower (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.SHOOTINGINTERVAL) {

			CmdSetShootingInterval (playerName, pickup.strength);

		} /*else if (pickup.effect == PickUp.BIGGER) {

			CmdSetSize (playerName, pickup.strength);

		}*/

		if (isServer) {
			RpcAddEffectToView (effect.getPlayer (), Time.time - effect.timeStarted, effect.pickupName);
			effectsServer.Add (effect);
		}
	}
		
	[Command]
	public void CmdSetShootingInterval(string playerName, float shootingIntervalMultiplikator) {

		RpcSetShootingInterval (playerName, shootingIntervalMultiplikator);

		//if (ownPlayerName.Equals (playerName)) {
			playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
			playerCollission = GameObject.Find(playerName).GetComponent<PlayerCollission>(); 

			playerCollission.maxShootingInterval = PlayerInfoController.defaultShootingInterval * (1 / shootingIntervalMultiplikator);

			playerInfoController.maxShootingInterval = PlayerInfoController.defaultShootingInterval * (1 / shootingIntervalMultiplikator);
		//}
	}

	[ClientRpc]
	public void RpcSetShootingInterval(string playerName, float shootingIntervalMultiplikator) {

		if (ownPlayerName.Equals (playerName)) {
			playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
			playerCollission = GameObject.Find(playerName).GetComponent<PlayerCollission>(); 

			playerCollission.maxShootingInterval = PlayerInfoController.defaultShootingInterval * (1 / shootingIntervalMultiplikator);

			playerInfoController.maxShootingInterval = PlayerInfoController.defaultShootingInterval * (1 / shootingIntervalMultiplikator);
		}
	}

	[Command]
	public void CmdSetSpeed(string playerName, float speedMultiplikator) {

		//if (ownPlayerName.Equals (playerName)) {

			playerControlNetwork = GameObject.Find(playerName).GetComponent<PlayerControlNetwork>();

			playerControlNetwork.speed = PlayerInfoController.defaultSpeed * speedMultiplikator;
		//}
	}

	[Command]
	public void CmdSetMaxSpeed(string playerName, float maxSpeedMultiplikator) {

		//if (ownPlayerName.Equals (playerName)) {

			playerControlNetwork = GameObject.Find(playerName).GetComponent<PlayerControlNetwork>();

			playerControlNetwork.maxSpeed = PlayerInfoController.defaultMaxSpeed * maxSpeedMultiplikator;
		//}
	}
		
	[Command]
	public void CmdSetShootingPower (string playerName, float shootingPowerMultiplikator) {

		playerCollission = GameObject.Find(playerName).GetComponent<PlayerCollission>(); 

		//if (ownPlayerName.Equals (playerName)) {
			playerCollission.shootingPower = PlayerInfoController.defaultShootingPower * shootingPowerMultiplikator;
		//}
	}

	[Command]
	private void CmdSetSize(string playerName, float sizeMultiplikator) {
		//if (ownPlayerName.Equals (playerName)) {
			GameObject player = GameObject.Find (playerName);

			player.transform.localScale = new Vector3 (sizeMultiplikator, sizeMultiplikator, sizeMultiplikator);
			player.GetComponent<Rigidbody> ().mass = sizeMultiplikator;
		//}
	}

	// --------------- FOR GUI ---------------------
	private List<Image> effectsScreen;

	[ClientRpc]
	private void RpcAddEffectToView(string playerName, float timeSinceStarted, string pickupName) {
		ownPlayerName = PlayerInfoController.nameInScene;

		Effect effect = new Effect (playerName, pickupName, Time.time - timeSinceStarted);

		if(ownPlayerName.Equals(effect.getPlayer())) {
			//Check if same effect already exists
			int effectNumber = getEffectNumber(effect.getEffect());

			if (effectNumber != -1) {
				//delte old effect
				Destroy(effectsScreen[effectNumber].gameObject);

				effects.RemoveAt(effectNumber);
				effectsScreen.RemoveAt (effectNumber);
			}

			GameObject currentEffect = (GameObject) Instantiate(effectPrefab) as GameObject;
			currentEffect.transform.SetParent (effectLayout.transform, false);

			Image effectImage = currentEffect.GetComponent<Image> ();
			Text text = currentEffect.GetComponentInChildren<Text> ();

			text.text = effect.getDescription ();

			effects.Add (effect);
			effectsScreen.Add (effectImage);
		}

		destroyPickup (pickupName);
	}

	private void destroyPickup(string pickupName) {
		GameObject pickup = GameObject.Find (pickupName);

		Instantiate (pickupPartical, new Vector3(pickup.transform.position.x, pickup.transform.position.y, pickup.transform.position.z), Quaternion.identity);

		Destroy (pickup);
	}

	[Server]
	private void updateEffects() {
		for (int i = 0; i < effectsServer.Count; i++) {

			Effect effect = effectsServer[i];

			float percent = effect.getPercentToPlay ();

			if (percent == 0f) {
				if (effect.enabled) {
					//Stop effect

					if (effect.getEffect () == PickUp.MAXSPEED) {
						CmdSetMaxSpeed (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SHOOTINGINTERVAL) {
						CmdSetShootingInterval (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SHOOTINGPOWER) {
						CmdSetShootingPower (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SPEED) {
						CmdSetSpeed (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.BIGGER) {
						CmdSetSize (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.ROTATEWORLD) {
						//TODO
					}

					effect.enabled = false;
				} else {
					effectsServer.RemoveAt (i);
					i--;
				}
			}

		}
	}
		
	private void clienUpdateEffects() {
		for (int i = 0; i < effects.Count; i++) {

			Effect effect = effects[i];
			Image image = effectsScreen [i];

			float percent = effect.getPercentToPlay ();

			if (percent == 0f) {
				if (effect.enabled) {
					effect.enabled = false;
				}

				//Remove object but first animate destroy
				//effects.RemoveAt(i);
				//effectsScreen.RemoveAt (i);
				//i--;

				image.gameObject.SetActive (false);
				//Destroy (image.gameObject);
			} else {
				image.fillAmount = percent;
			}

		}
	}

	private int getEffectNumber(int effectCode) {

		for (int i = 0; i < effects.Count; i++) {
			Effect effect = effects [i];

			if (effect.getEffect () == effectCode)
				return i;
		}

		return -1;
	}
}
