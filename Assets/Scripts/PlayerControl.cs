using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float speed;
    public float shootingPower;
    public float maxShootingInterval; //in seconds
    public float maxSpeed;

    public float acceMulti;
    public float verticalAusgleich;

    private Rigidbody ball;
    private Transform shootingBar;
    private GameObject playerInfo;

    private Rigidbody rb;

    private float lastShoot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastShoot = 0;

        //Find objects
        LoadObjects();

        //Disable Screen turns off
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void LoadObjects()
    {
        //Ball
        ball = GameObject.Find("Ball").GetComponent<Rigidbody>();

        //Camera
        GameObject camera = GameObject.Find("Camera");
        CameraController cameraScript = (CameraController) camera.GetComponent(typeof(CameraController));
        //cameraScript.SetPlayer(this.gameObject);

    }

    void Update()
    {
        UpdateShootingBar();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        /*if (Application.platform == RuntimePlatform.Android | Application.platform == RuntimePlatform.IPhonePlayer)
        {
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
        }*/

		//Use joystick
		moveHorizontal = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Horizontal");
		moveVertical = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Vertical");

		if (moveHorizontal > 1)
			moveHorizontal = 1;
		else if (moveHorizontal < -1)
			moveHorizontal = -1;
		if (moveVertical > 1)
			moveVertical = 1;
		else if (moveVertical < -1)
			moveVertical = -1;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);

        //MAX Speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
        } 
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            //Shoot the ball
            shoot();
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("Ball"))
        {
            //Shoot the ball
             shoot();
        }
    }

    void shoot()
    {
        if (Input.GetKey(KeyCode.Space) || Input.touchCount >= 1)
        {
            if (Time.time - lastShoot > maxShootingInterval)
            {
                lastShoot = Time.time;
                Vector3 direction = ball.transform.position - transform.position;
                ball.AddForce(direction * shootingPower);
            }
        }
    }

    void UpdateShootingBar()
    {
        float shootingPercent = (Time.time - lastShoot) / maxShootingInterval;
        shootingBar.GetComponent<Image>().fillAmount = shootingPercent;
    }
}
