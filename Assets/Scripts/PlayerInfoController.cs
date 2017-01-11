using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInfoController : NetworkBehaviour {

	public static float defaultShootingInterval = 2;
	public static float defaultShootingPower = 2;
	public static float defaultSpeed = 8;
	public static float defaultMaxSpeed = 8;
	public static float defaultSize = 1;
	public static float defaultMass = 1;

	public static PlayerInfoController instance;

	[SyncVar] public float actualShootingInterval;
	[SyncVar] public float actualShootingPower;
	[SyncVar] public float actualSpeed;
	[SyncVar] public float actualMaxSpeed;
	[SyncVar] public float actualSize;
	[SyncVar] public float actualMass;

	public float lastShoot;

	[SyncVar] public bool firstTeam;
	[SyncVar] public string playerName;
	[SyncVar] public string playerNameVisibleOthers;
	[SyncVar] public Vector3 startPosition;

	private NetworkInstanceId playerNetID;

	public GameObject shootingBarPrefab;

	private GameObject shootingBar;
	private TextMesh nameTextMesh;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {

		actualShootingInterval = defaultShootingInterval;
		actualShootingPower = defaultShootingPower;
		actualSpeed = defaultSpeed;
		actualMaxSpeed = defaultMaxSpeed;
		actualSize = defaultSize;
		actualMass = defaultMass;

		rb = GetComponent<Rigidbody>();

		if (isLocalPlayer) {
			instance = this;
			setupCamera ();

			//Instantiate shootingBar
			lastShoot = Time.time;
			shootingBar = (GameObject) Instantiate (shootingBarPrefab);
		}

		nameTextMesh = (TextMesh)this.GetComponentInChildren (typeof(TextMesh));

		bool showNames = false;
		if (PlayerPrefs.GetInt (PreferenceManager.NAMES) >= 1)
			showNames = true;

		if(!showNames || isLocalPlayer)
			nameTextMesh.gameObject.SetActive (false);
	}

	private void setupCamera() {
		//Camera
		GameObject camera = GameObject.Find("Camera");
		CameraController cameraScript = (CameraController) camera.GetComponent(typeof(CameraController));
		cameraScript.SetPlayer(this.gameObject);
	}

	void Update () {

		//Update size and mass
		rb.mass = actualMass;
		this.gameObject.transform.localScale = new Vector3 (actualSize, actualSize, actualSize);

		//Set the shootingbar and names
		if (isLocalPlayer)
			UpdateShootingBar ();
		else
			UpdateName ();
	}

	private void UpdateShootingBar() {
		//Set the position
		Vector3 positionPlayer = transform.position;
		shootingBar.transform.position = new Vector3 (positionPlayer.x, positionPlayer.y - 0.4f, positionPlayer.z);
		shootingBar.transform.rotation = Quaternion.Euler (GroundRotator.rotationGround);

		//Set the fillvalue of the image
		float shootingPercent = (Time.time - lastShoot) / actualShootingInterval;
		shootingBar.GetComponentInChildren<Image>().fillAmount = shootingPercent;
	}

	void UpdateName() {
		if(nameTextMesh != null)
			nameTextMesh.text = playerNameVisibleOthers;
	}

	public void resetPosition() {
		transform.position = startPosition;

		Rigidbody rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
	}

	public static PlayerInfoController getInstance() {
		return instance;
	}

	// ---------- NETWORK ------------

	//First onServer is called then onclient

	//Is only called on the client side of this object
	public override void OnStartClient() {

		transform.name = playerName;

		if (firstTeam) {
			gameObject.tag = "FirstPlayer";
			GetComponent<MeshRenderer>().material.color = Color.green;

			if (isLocalPlayer) {
				Points.setFirstTeam (firstTeam);
			}
		} else {
			gameObject.tag = "SecondPlayer";
		}

		Debug.Log ("PlayerInfoController: OnStartClient: " + playerName + ", team: " + firstTeam);
	} 

	//Last player is true when the lastplayer was in the first team
	private static bool lastPlayer = false;
	//counts the already connected players
	private static int playerCounter = 0;
		
	// Is only called on the server-side for this client
	public override void OnStartServer () {
		//Set names for the players
		playerCounter++;
		playerName = "Player" + playerCounter.ToString();
		transform.name = playerName;

		//Set teams for the players
		firstTeam = !lastPlayer;
		lastPlayer = !lastPlayer;
		
		//Set new position
		Vector3 position = SpawnPoints.getNextSpawnPosition();
		startPosition = position;
		transform.position = position;

		Debug.Log ("Player: OnStartServer: " + playerCounter + ", team: " + firstTeam);
	}
}