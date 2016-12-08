using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Points : NetworkBehaviour {

	public Text pointsText;

	[SyncVar] private int pointsFirstTeam;
	[SyncVar] private int pointsSecondTeam;

	private static Points instance;

	private static bool firstTeam;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		pointsFirstTeam = 0;
		pointsSecondTeam = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (firstTeam)
			pointsText.text = "<b>" + pointsFirstTeam + "</b>:" + pointsSecondTeam;
		else
			pointsText.text = "<b>" + pointsSecondTeam + "</b>:" + pointsFirstTeam;
	}

	[Server]
	public static void firstTeamScores() {
		instance.pointsFirstTeam++;
	}

	[Server]
	public static void secondTeamScores() {
		instance.pointsSecondTeam++;
	}

	[Server]
	public static void resetScores() {
		instance.pointsFirstTeam = 0;
		instance.pointsSecondTeam = 0;
	}

	public static void setFirstTeam(bool firstTeamBool) {
		firstTeam = firstTeamBool;
	}

	public static int getHighestScore() {
		if (instance.pointsFirstTeam > instance.pointsSecondTeam)
			return instance.pointsFirstTeam;
		else
			return instance.pointsSecondTeam;
	}
}
