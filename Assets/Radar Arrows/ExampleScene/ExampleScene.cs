using UnityEngine;
using System.Collections;

public class ExampleScene : MonoBehaviour {

	public GameObject[] objectTypes;
	
	/// <summary>
	/// Instantiates a random object from the objectTypes array.
	/// </summary>
	public void SpawnObject() {
		int typeNum = Random.Range(0, objectTypes.Length);
		Instantiate(objectTypes[typeNum],
				new Vector3(Random.Range(-100,40), Random.Range(-100,100),Random.Range(-100,100)), 
		        Quaternion.identity);
	}
	
	void Update () {
		//Input controls
		transform.Translate(Vector3.up*((Input.GetKey(KeyCode.W)?1:0)-(Input.GetKey(KeyCode.S)?1:0))/2);
		transform.Translate(Vector3.right*((Input.GetKey(KeyCode.D)?1:0)-(Input.GetKey(KeyCode.A)?1:0))/2);
		transform.Translate(Vector3.forward*((Input.GetKey(KeyCode.Space)?1:0)-(Input.GetKey(KeyCode.LeftAlt)?1:0))/2);
		transform.Rotate(Vector3.up*((Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.LeftArrow)?1:0)));
		transform.Rotate(Vector3.right*((Input.GetKey(KeyCode.DownArrow)?1:0)-(Input.GetKey(KeyCode.UpArrow)?1:0)));
	}
}
