using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FaceCamera : NetworkBehaviour {

	public GameObject camera;

	void Start() {
		camera = GameObject.Find("Camera");
	}

	// Update is called once per frame
	void Update () {
		Vector3 positionCamera = camera.transform.position;
		Vector3 faceAt = new Vector3 (this.transform.position.x, positionCamera.y, positionCamera.z);

		this.transform.LookAt (faceAt);
		this.transform.Rotate (new Vector3 (0, 180, 0));
	}
}
