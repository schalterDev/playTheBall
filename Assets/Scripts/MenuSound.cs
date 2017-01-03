using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour {

	private static MenuSound instance;

	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(transform.gameObject);
		} else
			Destroy (this.gameObject);
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void stopMusic() {
		if (instance != null) {
			Destroy (instance.gameObject);
			instance = null;
		}
	}
}
