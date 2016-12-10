using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BallCollision : NetworkBehaviour {

    private Rigidbody rb;

    public GameObject goalOverlay;

    private float timeForReset = 2; //In seconds
    private float resetBallTime;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        resetBallTime = float.MaxValue;

		timeLastGoal = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        resetBallTime -= Time.deltaTime;

		if (isServer) {
			if (resetBallTime <= 0) {
				ResetBall ();
			}
		}
	}

	private float timeLastGoal;

    void OnCollisionEnter(Collision other)
    {
		if (isServer) {
			if (Time.time - timeLastGoal > timeForReset) {
				if (other.gameObject.CompareTag ("NorthGoal")) {
					resetBallTime = timeForReset;
					RpcGoal ();
					Points.firstTeamScores ();
					timeLastGoal = Time.time;
				} else if (other.gameObject.CompareTag ("SouthGoal")) {
					resetBallTime = timeForReset;
					RpcGoal ();
					Points.secondTeamScores ();
					timeLastGoal = Time.time;
				}
			}
		}
    }

	[ClientRpc]
	void RpcGoal() {
		goalOverlay.SetActive (true);
		AudioManager.goalAudio ();
	}

	[ClientRpc]
	void RpcReset() {
		goalOverlay.SetActive (false);
	}

    void ResetBall()
    {
        resetBallTime = float.MaxValue;
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(0, 0.5f, 0);
        goalOverlay.SetActive(false);

		GameManager.instance.goal ();

		RpcReset ();
    }
}
