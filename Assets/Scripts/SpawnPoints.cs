using UnityEngine;
using System.Collections;

public class SpawnPoints : MonoBehaviour {

	private static int spawnPosition;
	public static Transform[] spawnPoints;

	// Use this for initialization
	void Awake () {
		spawnPosition = 0;
		spawnPoints = gameObject.GetComponentsInChildren<Transform>();
	}
	
	public static Vector3 getNextSpawnPosition() {

		Debug.Log ("GetNextSpawnPosition");
		
		if (spawnPoints != null) {
			spawnPosition++;
			if (spawnPosition < spawnPoints.Length) {
				return spawnPoints [spawnPosition].position;
			}
		}

		return new Vector3 (0f, 0.5f, 0f);
	}
}
