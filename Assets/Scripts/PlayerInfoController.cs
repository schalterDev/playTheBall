using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInfoController : NetworkBehaviour {

	public static float defaultShootingInterval = 2;
	public static float defaultShootingPower = 2;
	public static float defaultSpeed = 9;
	public static float defaultMaxSpeed = 9;

	public static bool iAmFirstTeam;

	public static string nameInScene;

	[SyncVar] public bool firstTeam;
	[SyncVar] public string playerName;
	private NetworkInstanceId playerNetID;

    private Vector3 offset;

	public float maxShootingInterval;
	private float lastShoot;
	public GameObject shootingBarPrefab;

	private GameObject shootingBar;
	private TextMesh textMesh;

	// Use this for initialization
	void Start () {
		lastPlayer = false;
		playerCounter = 0;

		if (isLocalPlayer) {
			nameInScene = gameObject.name;
			setup ();
		}

		textMesh = (TextMesh)this.GetComponentInChildren (typeof(TextMesh));

		bool showNames = false;
		if (PlayerPrefs.GetInt (PreferenceManager.NAMES) >= 1)
			showNames = true;

		if (isLocalPlayer) {
			//Instantiate shootingBar
			maxShootingInterval = GetComponent <PlayerCollission>().maxShootingInterval;
			lastShoot = Time.time;

			shootingBar = (GameObject) Instantiate (shootingBarPrefab);

			Points.setFirstTeam (firstTeam);
		} 

		if(!showNames || isLocalPlayer)
			textMesh.gameObject.SetActive (false);
	}

	private void setup() {
		//Camera
		GameObject camera = GameObject.Find("Camera");
		CameraController cameraScript = (CameraController) camera.GetComponent(typeof(CameraController));
		//cameraScript.SetPlayer(this.gameObject);
	}

	void Update () {
		//Set the position of shootingBar
		if (isLocalPlayer)
			UpdateShootingBar ();
		else
			UpdateName ();
	}

	public void shot() {
		lastShoot = Time.time;
	}

	void UpdateShootingBar() {
		//Set the position
		Vector3 positionPlayer = transform.position;
		shootingBar.transform.position = new Vector3 (positionPlayer.x, positionPlayer.y - 0.4f, positionPlayer.z);
		shootingBar.transform.rotation = Quaternion.Euler (GroundRotator.rotationGround);

		//Set the fillvalue of the image
		float shootingPercent = (Time.time - lastShoot) / maxShootingInterval;
		shootingBar.GetComponentInChildren<Image>().fillAmount = shootingPercent;
	}

	void UpdateName() {
		if(textMesh != null)
			textMesh.text = name;
	}
		

	private static bool lastPlayer = false;
	private static int playerCounter = 0;

	public override void OnStartClient() {
		playerCounter++;

		transform.name = "Player" + playerCounter.ToString ();

		firstTeam = !lastPlayer;
		lastPlayer = !lastPlayer;
			
		if (firstTeam) {
			gameObject.tag = "FirstPlayer";
			GetComponent<MeshRenderer>().material.color = Color.green;
		} else {
			gameObject.tag = "SecondPlayer";
		}
	} 
}