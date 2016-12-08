using System;
using UnityEngine;
	
public class Effect {

	public float timeStarted;
	public string pickupName;
	private PickUp pickup;
	private string player;

	public bool enabled;

	public Effect (string player, PickUp pickup, float timeStarted) {
		this.player = player;
		this.pickup = pickup;
		this.timeStarted = timeStarted;
		this.pickupName = pickup.gameObject.name;

		enabled = true;
	}

	public Effect (string player, string pickupName, float timeStarted) {
		pickup = GameObject.Find (pickupName).GetComponent<PickUp> ();
		this.player = player;
		this.timeStarted = timeStarted;
		this.pickupName = pickupName;

		enabled = true;
	}

	public bool isOver() {
		if (timeStarted - Time.time > pickup.duration)
			return true;
		else
			return false;
	}

	public int getEffect() {
		return pickup.effect;
	}

	public string getPlayer() {
		return player;
	}

	public PickUp getPickup() {
		return pickup;
	}

	public float getStrength() {
		return pickup.strength;
	}

	public float getDuration() {
		return pickup.duration;
	}

	public float getPercentToPlay() {
		float timeAlreadyPlayed = Time.time - timeStarted;

		float percent = 1 - (timeAlreadyPlayed / pickup.duration);

		if (percent < 0)
			return 0;
		else
			return percent;
	}

	public string getDescription() {
		if (pickup.effect == PickUp.MAXSPEED) {
			return "MAX SPEED";
		} else if (pickup.effect == PickUp.SHOOTINGINTERVAL) {
			return "SHOOTING INTERVAL";
		} else if (pickup.effect == PickUp.SHOOTINGPOWER) {
			return ("SHOOTING POWER");
		} else if (pickup.effect == PickUp.SPEED) {
			return ("SPEED");
		} else if (pickup.effect == PickUp.BIGGER) {
			return ("BIGGER");
		}

		return "unknown";
	}

}
