using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlNetwork : NetworkBehaviour {

    public float speed;
    public float maxSpeed;

    public float acceMulti;
    public float verticalAusgleich;

	public GameObject particle;

    private Rigidbody rb;

	[SyncVar] public Vector3 startPosition;
	private bool turnAxes;

	private int control;

    void Start()
    {
		if (!isLocalPlayer)
			return;
		
		//Find objects
		LoadObjects();

        rb = GetComponent<Rigidbody>();

		timelastPartical = Time.time;
		positionLastPartical = transform.position;
    }

	public float timeForPartical = 0.5f;
	public float minDistanceForPartical = 0.1f;
	public float maxDistanceForPartical = 0.5f;
	private float timelastPartical;
	private Vector3 positionLastPartical;

	void Update() {
		//Start partical system when player moveded more than 0,5 units or after 0,5 seconds
		if (Time.time - timelastPartical > timeForPartical) {
			float distance = (positionLastPartical - transform.position).magnitude;
			if (distance > minDistanceForPartical) {
				showParticals ();
			}
		} else {
			float distance = (positionLastPartical - transform.position).magnitude;
			if (distance > maxDistanceForPartical) {
				showParticals ();
			}
		}
	}

	private void showParticals() {
		Instantiate (particle, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), Quaternion.identity);
		timelastPartical = Time.time;
		positionLastPartical = transform.position;
	}

    void LoadObjects()
    {
        //Camera
        GameObject camera = GameObject.Find("Camera");
        CameraController cameraScript = (CameraController) camera.GetComponent(typeof(CameraController));
        cameraScript.SetPlayer(this.gameObject);

		PlayerInfoController info = (PlayerInfoController) this.gameObject.GetComponent (typeof(PlayerInfoController));
		turnAxes = info.firstTeam;

		control = PlayerPrefs.GetInt (PreferenceManager.CONTROL);

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

		if (GameManager.instance.getState() != GameManager.BEFORESTART
			&& GameManager.instance.getState() != GameManager.COUNTDOWN)
        	MovingPhysik();
    }

    void MovingPhysik()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			//use accelormeter
			if (control == PreferenceManager.ACCELERATOR) {
				moveHorizontal = Input.acceleration.x * acceMulti;
				moveVertical = Input.acceleration.y * acceMulti + verticalAusgleich;

				if (moveHorizontal > 1)
					moveHorizontal = 1;
				else if (moveHorizontal < -1)
					moveHorizontal = -1;
				if (moveVertical > 1)
					moveVertical = 1;
				else if (moveVertical < -1)
					moveVertical = -1;
			} else if(control == PreferenceManager.JOYSTICK) {
				//Use joystick
				moveHorizontal = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis ("Horizontal");
				moveVertical = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis ("Vertical");

				if (moveHorizontal > 1)
					moveHorizontal = 1;
				else if (moveHorizontal < -1)
					moveHorizontal = -1;
				if (moveVertical > 1)
					moveVertical = 1;
				else if (moveVertical < -1)
					moveVertical = -1;
			}
		}

		if (!(moveHorizontal == 0f && moveVertical == 0f)) {
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			if (turnAxes)
				CmdMove (-1 * movement * speed);
			else
				CmdMove (movement * speed);
		}
			
    }

	// ---------- NETWORK ------------
	public override void OnStartServer () {
		//Set new position
		Vector3 position = SpawnPoints.getNextSpawnPosition();

		startPosition = position;

		transform.position = position;
	}

	[Command]
	public void CmdMove(Vector3 movement) {
		rb = GetComponent<Rigidbody>();

		rb.AddForce(movement);
		//MAX Speed
		if (rb.velocity.magnitude > maxSpeed)
		{
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}
		
	public void resetPosition() {
		transform.position = startPosition;

		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
	}
}
