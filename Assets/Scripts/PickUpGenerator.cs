using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PickUpGenerator : NetworkBehaviour {

	[SerializeField] public GameObject Pickup;
	public GameObject ground;
    public float seconds;
    public float maxX, maxZ, y;

    private float multiplikator = 5;

	private int pickupCounter;
    private float lastPlacement;

	private Random rnd;

	// Use this for initialization
	void Start () {
		rnd = new Random();

        lastPlacement = 0;
		pickupCounter = 0;

        maxX *= multiplikator;
        maxZ *= multiplikator;
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer && GameManager.instance.getState() == GameManager.STARTET) {
			if (Time.time - lastPlacement >= seconds) {
				PlacePickup ();
				pickupCounter++;
			}
		}
	}

    void PlacePickup()
    {

		ClientScene.RegisterPrefab (Pickup);
        lastPlacement = Time.time;
		GameObject currentPickup = (GameObject) Instantiate(Pickup, GeneratedPosition(), Quaternion.identity);

		PickUp pickupScript = currentPickup.GetComponent<PickUp> ();

		currentPickup.transform.parent = ground.transform;

		pickupScript.effect = randomEffect();
		pickupScript.duration = randomEffectDuration ();
		pickupScript.strength = randomEffectStrength ();

		NetworkServer.Spawn (currentPickup);
    }

    Vector3 GeneratedPosition()
    {
        float x, z;
        x = Random.Range(-maxX, maxX);
        z = Random.Range(-maxZ, maxZ);
        return new Vector3(x, y, z);
    }

	private int randomEffect() {
		return Random.Range (0, PickUp.NUMBEREFFECTS);
	}

	private float randomEffectStrength() {
		float randomFloat = Random.Range (0, 50) / 100f;


		return randomFloat + 1.3f;
	}

	private float randomEffectDuration() {
		return Random.Range (7, 15) * 1f;
	}

}
