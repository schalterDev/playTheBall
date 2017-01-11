using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

	public static int BEFORESTART = 0;
	public static int COUNTDOWN = 1;
	public static int STARTET = 2;
	public static int PAUSED = 3;
	public static int GAMEOVER = 4;

	[SyncVar] private int state;

	private int fontForCountdown;

	public float startCountdown;
	public int scoreToEnd;

	public int expectedNumberOfPlayers = 8;

	public GameObject countdownOverlay;
	public GameObject gameOverOverlay;
	public GameObject joystick;
	public Text gameOverText;

	private Text countdownText;
	private float startTime;
	private int actualNumberOfPlayers;

	private PlayerControlNetwork[] players;

	//Light
	public GameObject dayLight;
	public GameObject nightLight;

	public static GameManager instance;
	public bool winner;

	private static GameManager gameManagerInstance;

	public static GameManager getInstance() {
		return gameManagerInstance;
	}

	void Awake() {
		winner = false;
		instance = this;

		//Limit frame rate to 30
		Application.targetFrameRate = 30;
		//Disable Screen turns off
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		//Set lights if day or night
		int timeLight = PlayerPrefs.GetInt(PreferenceManager.LIGHT);
		if (timeLight == PreferenceManager.DAYLIGHT) {
			nightLight.SetActive (false);
			dayLight.SetActive (true);
		} else if (timeLight == PreferenceManager.NIGHTLIGHT) {
			dayLight.SetActive (false);
			nightLight.SetActive (true);
		}
	}

	// Use this for initialization
	void Start () {
		gameManagerInstance = this;

		checkControl ();

		actualNumberOfPlayers = 0;

		if(isServer)
			state = BEFORESTART;
		
		startTime = Time.time;
		countdownText = countdownOverlay.GetComponentInChildren<Text> ();

		fontForCountdown = countdownText.fontSize;

		expectedNumberOfPlayers = NetworkLobbyHook.numberPlayers;
	}

	void Update () {

		if (state == BEFORESTART) {
			startTime = Time.time;
			countdownOverlay.SetActive (true);
			countdownText.fontSize = 60;
			countdownText.text = "Waiting for other players...";

			if (isServer) {
				actualNumberOfPlayers = NetworkServer.connections.Count;

				if (actualNumberOfPlayers >= expectedNumberOfPlayers) {
					foreach (NetworkConnection connection in NetworkServer.connections) {
						if (!connection.isReady)
							return;
					}

					allPlayerConnected ();
				}
			}
		}

		//Show Countdown for starting of the game
		if (state == COUNTDOWN) {
			countdownOverlay.SetActive (true);

			if (Time.time - startTime < startCountdown) {
				countdownText.fontSize = fontForCountdown;
				countdownText.text = string.Format ("{0:0}", startCountdown - (Time.time - startTime) + 1f);
			} else { 
				//The end of the countdown
				state = GameManager.STARTET;
				countdownOverlay.SetActive(false);
			}

		} 

		if (state == STARTET) {
			countdownOverlay.SetActive (false);
		}

		if(state == PAUSED) {
			//Show the user that the server paused the game
			if (!isServer) {
				countdownText.fontSize = 80;
				countdownText.text = "Paused by server";
				countdownOverlay.SetActive (true);
			}
		}

		if(state == GAMEOVER) {
			gameOverOverlay.SetActive (true);

			if (winner) {
				gameOverText.text = "WINNER";
			}
		}
	}

	public void allPlayerConnected() {
		if (!isServer)
			return;
		
		state = COUNTDOWN;
	}

	[ClientRpc]
	public void RpcSetTimeScale(float timeScale) {
		Time.timeScale = timeScale;
	}

	public void pause() {
		if (!isServer) {
			UIManager.PauseLocal ();

			//TODO Show the user that he can not pause the game
			return;
		}
		
		state = PAUSED;
		RpcSetTimeScale (0f);

		UIManager.PauseGame ();
	}

	public void resume() {

		if (!isServer) {
			UIManager.ResumeLocal ();

			return;
		}
		
		state = STARTET;
		RpcSetTimeScale (1f);

		UIManager.ResumeGame ();
	}

	public void gameOver() {
		if (!isServer)
			return;
		
		RpcSetTimeScale (0f);

		if (Points.getPoints () == scoreToEnd) {
			winner = true;
		} else {
			winner = false;
		}

		state = GAMEOVER;
	}

	private void checkControl() {
		int control = PlayerPrefs.GetInt (PreferenceManager.CONTROL);

		if (control != PreferenceManager.JOYSTICK)
			Destroy (joystick);	 
	}

	public void goal() {
		//Check if game is over
		if (scoreToEnd <= Points.getHighestScore ()) {
			gameOver ();
			return;
		}

		//Reset all players
		foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("FirstPlayer")) {
			PlayerInfoController player = (PlayerInfoController) playerObject.GetComponent(typeof(PlayerInfoController));
			player.resetPosition ();
		}
		foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("SecondPlayer")) {
			PlayerInfoController player = (PlayerInfoController) playerObject.GetComponent(typeof(PlayerInfoController));
			player.resetPosition ();
		}

		if(isServer)
			CmdStartCountdown ();
	}

	public bool movementAllowed() {
		return state == STARTET;
	}

	[Command]
	private void CmdStartCountdown() {
		//Start Countdown
		RpcStartCountdown ();
	}

	[ClientRpc]
	public void RpcStartCountdown() {
		startTime = Time.time;
		state = GameManager.COUNTDOWN;	
	}

	public int getState() {
		return state;
	}
}