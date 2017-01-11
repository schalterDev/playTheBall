using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlNetwork : NetworkBehaviour {

	private bool DEBUG = true;

    public float acceMulti;
    public float verticalAusgleich;

	public GameObject particle;

    private Rigidbody rb;
	private PlayerInfoController playerInfoController;

	private bool turnAxes;

	private int control;

	private Vector3 lastMovement;
	//only for server
	private Vector3 movement;
	private GameManager gameManager;

    void Start()
    {
		gameManager = GameManager.getInstance ();

		lastMovement = Vector3.zero;
		movement = Vector3.zero;

		//Find objects
		LoadObjects();

		if (!isLocalPlayer)
			return;
		
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

    private void LoadObjects()
    {
		rb = GetComponent<Rigidbody>();

		playerInfoController = gameObject.GetComponent<PlayerInfoController>();
		turnAxes = playerInfoController.firstTeam;

		control = PlayerPrefs.GetInt (PreferenceManager.CONTROL);

		rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
		if (isLocalPlayer) {
			if (GameManager.instance.getState () != GameManager.BEFORESTART
				&& GameManager.instance.getState () != GameManager.COUNTDOWN)
				MovingPhysik ();
		}

		if (isServer) {
			if (gameManager == null)
				gameManager = GameManager.getInstance ();

			if (gameManager != null && gameManager.movementAllowed ()) {
				float maxSpeed = playerInfoController.actualMaxSpeed;
				rb.AddForce (movement);
				//MAX Speed
				if (rb.velocity.magnitude > maxSpeed) {
					rb.velocity = rb.velocity.normalized * maxSpeed;
				}
			}
		}

    }
		
    private void MovingPhysik()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || DEBUG) {
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
			
		float speed = playerInfoController.actualSpeed;

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		if (turnAxes)
			movement = -1 * movement * speed;
		else
			movement = movement * speed;

		if(!movement.Equals(lastMovement)) {
			lastMovement = movement;	
			CmdSetMovement(movement);
		}
			
    }

	[Command]
	private void CmdSetMovement(Vector3 movement) {
		this.movement = movement;
	}
}
