using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EffectManager : NetworkBehaviour {

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
	
	// Update is called once per frame
	void Update () {
		if(isServer)
			updateEffects ();

		if(isClient)
			clienUpdateEffects ();
	}

	[Server]
	public void addEffect(Effect effect) {

		effectsServer.Add (effect);

		PickUp pickup = effect.getPickup();
		string playerName = effect.getPlayer ();

		if (pickup.effect == PickUp.SPEED) {

			setSpeed (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.MAXSPEED) {

			setMaxSpeed (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.SHOOTINGPOWER) {

			setShootingPower (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.SHOOTINGINTERVAL) {

			setShootingInterval (playerName, pickup.strength);

		} else if (pickup.effect == PickUp.BIGGER) {

			setSize (playerName, pickup.strength);

		}
			
		RpcAddEffectToView (effect.getPlayer (), Time.time - effect.timeStarted, effect.pickupName);

		destroyPickup(effect.pickupName);
	}
		
	[Server]
	public void setShootingInterval(string playerName, float shootingIntervalMultiplikator) {
		playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
		playerInfoController.actualShootingInterval = PlayerInfoController.defaultShootingInterval * (1 / shootingIntervalMultiplikator);
	}

	[Server]
	public void setSpeed(string playerName, float speedMultiplikator) {
		
		playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
		playerInfoController.actualSpeed = PlayerInfoController.defaultSpeed * speedMultiplikator;

	}

	[Server]
	public void setMaxSpeed(string playerName, float maxSpeedMultiplikator) {

		playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
		playerInfoController.actualMaxSpeed = PlayerInfoController.defaultMaxSpeed * maxSpeedMultiplikator;

	}
		
	[Server]
	public void setShootingPower (string playerName, float shootingPowerMultiplikator) {

		playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
		playerInfoController.actualShootingPower = PlayerInfoController.defaultShootingPower * shootingPowerMultiplikator;
	}

	[Server]
	private void setSize(string playerName, float sizeMultiplikator) {

		playerInfoController = GameObject.Find(playerName).GetComponent<PlayerInfoController>();
		playerInfoController.actualSize = PlayerInfoController.defaultSize * sizeMultiplikator;
		playerInfoController.actualMass = PlayerInfoController.defaultMass * sizeMultiplikator;

	}

	// --------------- FOR GUI ---------------------
	private List<Image> effectsScreen;

	[ClientRpc]
	private void RpcAddEffectToView(string playerName, float timeSinceStarted, string pickupName) {

		//TODO error: NullReferenceException: Object reference not set to an instance of an object
		Effect effect = new Effect (playerName, pickupName, Time.time - timeSinceStarted);

		// if is local player
		if(PlayerInfoController.instance.playerName.Equals(effect.getPlayer())) {

			Debug.Log ("IsLocalPlayer effectmanager");

			//Check if same effect already exists
			int effectNumber = getEffectNumber(effect.getEffect());

			if (effectNumber != -1) {
				//delte old effect
				Destroy(effectsScreen[effectNumber].gameObject);

				effects.RemoveAt(effectNumber);
				effectsScreen.RemoveAt (effectNumber);
			}

			// Add new effect to screen
			GameObject currentEffect = (GameObject) Instantiate(effectPrefab) as GameObject;
			currentEffect.transform.SetParent (effectLayout.transform, false);

			Image effectImage = currentEffect.GetComponent<Image> ();
			Text text = currentEffect.GetComponentInChildren<Text> ();

			text.text = effect.getDescription ();

			effects.Add (effect);
			effectsScreen.Add (effectImage);
		}

		showPickupPartical (pickupName);
	}

	private void showPickupPartical(string pickupName) {
		GameObject pickup = GameObject.Find (pickupName);
		Instantiate (pickupPartical, new Vector3(pickup.transform.position.x, pickup.transform.position.y, pickup.transform.position.z), Quaternion.identity);
		pickup.gameObject.SetActive (false);
	}

	[Server]
	private void destroyPickup(string pickupName) {
		GameObject pickup = GameObject.Find (pickupName);
		//Destroy (pickup);
		//TODO dont destroy here or in Effects there will be an error because the object is not found any more
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
						setMaxSpeed (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SHOOTINGINTERVAL) {
						setShootingInterval (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SHOOTINGPOWER) {
						setShootingPower (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.SPEED) {
						setSpeed (effect.getPlayer (), 1f);
					} else if (effect.getEffect () == PickUp.BIGGER) {
						setSize (effect.getPlayer (), 1f);
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
