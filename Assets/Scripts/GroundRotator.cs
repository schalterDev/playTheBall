using UnityEngine;
using System.Collections;

public class GroundRotator : MonoBehaviour {

	public float rotateXTo;
	public float rotateYTo;
	public float rotateZTo;

	public float rotationSpeed = 2f;
	public float rotationlock = 0.1f;

	public bool rotateBack;

	public static Vector3 rotationGround;

	// Update is called once per frame
	void Update () {
		float newXRotation, newYRotation, newZRotation;
		Vector3 rotation = transform.rotation.eulerAngles; 

		rotation = changeRotationToNegativeNumbers (rotation);

		rotationGround = rotation;

		newXRotation = rotation.x;
		newYRotation = rotation.y;
		newZRotation = rotation.z;

		if (rotateBack) {

			if (rotation.x < -rotationlock)
				newXRotation = rotation.x + rotationSpeed * Time.deltaTime;
			else if(rotation.x > rotationlock)
				newXRotation = rotation.x - rotationSpeed * Time.deltaTime;

			if (rotation.y < -rotationlock)
				newYRotation = rotation.y + rotationSpeed * Time.deltaTime;
			else if (rotation.y > rotationlock)
				newYRotation = rotation.y - rotationSpeed * Time.deltaTime;

			if (rotation.z < -rotationlock)
				newZRotation = rotation.z + rotationSpeed * Time.deltaTime;
			else if(rotation.z > rotationlock)
				newZRotation = rotation.z - rotationSpeed * Time.deltaTime;

		} else {
			
			if (rotation.x < rotateXTo - rotationlock)
				newXRotation = rotation.x + rotationSpeed * Time.deltaTime;
			else if( rotation.x > rotateXTo + rotationlock)
				newXRotation = rotation.x - rotationSpeed * Time.deltaTime;

			if (rotation.y < rotateYTo)
				newYRotation = rotation.y + rotationSpeed * Time.deltaTime;
			else if( rotation.y > rotateYTo + rotationlock)
				newYRotation = rotation.y - rotationSpeed * Time.deltaTime;

			if (rotation.z < rotateZTo)
				newZRotation = rotation.z + rotationSpeed * Time.deltaTime;
			else if( rotation.z > rotateZTo + rotationlock)
				newZRotation = rotation.z - rotationSpeed * Time.deltaTime;
			
		}

		transform.rotation = Quaternion.Euler (
			new Vector3 (newXRotation, newYRotation, newZRotation));

	}

	private Vector3 changeRotationToNegativeNumbers(Vector3 rotation) {
		float x, y, z;
	
		if (rotation.x > 180)
			x = rotation.x - 360;
		else
			x = rotation.x;

		if (rotation.y > 180)
			y = rotation.y - 360;
		else
			y = rotation.y;

		if (rotation.z > 180)
			z = rotation.z - 360;
		else
			z = rotation.z;

		return new Vector3 (x, y, z);
	}
}
