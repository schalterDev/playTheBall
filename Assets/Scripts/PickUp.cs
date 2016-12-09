using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PickUp : NetworkBehaviour {

	public static int NUMBEREFFECTS = 5;

	public static int SPEED = 0;
	public static int MAXSPEED = 1;
	public static int SHOOTINGPOWER = 2;
	public static int SHOOTINGINTERVAL = 3;
	public static int BIGGER = 4;
	public static int ROTATEWORLD = 5;

	[SyncVar] string uniqueName;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;

	[SyncVar] public float duration;
	[SyncVar] public int effect;
	[SyncVar] public float strength;
	[SyncVar] public bool positiv;

	private static int pickupCounter = 0;

	void Start() {
		myTransform = transform;
		MakeUniqueName ();
		pickupCounter++;
	}

	void Update() {
		if (myTransform.name == "" || myTransform.name == "Pickup(Clone)") {
			myTransform.name = uniqueName;
		}
	}

	private void MakeUniqueName() {
		uniqueName = "Pickup" + pickupCounter;
	}

}
