using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject player;

    public float horizontalCamera = 2f;
	public float smoothTime = 2f;

    //private float firstXPosition;
	public bool isFirstTeam;

	public bool followYRotation;
    
	public Vector3 offset;
	public Vector3 rotation;

	// Use this for initialization
	void Start () {
		setNewPositon (transform.position);
		setNewRotation (transform.rotation.eulerAngles);
	}

    // Update is called once per frame
    void LateUpdate()
    {
        /*Mesh planeMesh = ground.GetComponent<MeshFilter>().mesh;
        Bounds bounds = planeMesh.bounds;
        // size in pixels
        float boundsX = ground.transform.localScale.x * bounds.size.x;
        float boundsZ = ground.transform.localScale.z * bounds.size.z;

        if player*/

        if (player != null) { 
			if (followYRotation) {
				Vector3 newRotation = new Vector3 (rotation.x, player.transform.rotation.eulerAngles.y, rotation.z);

				setNewRotation (newRotation);
			} else {
				setNewRotation (rotation);
			}

            float vectorX = player.transform.position.x;
            if (vectorX <= -horizontalCamera)
                vectorX = -horizontalCamera;
            else if (vectorX >= horizontalCamera)
                vectorX = horizontalCamera;

            Vector3 withoutHorizontal = player.transform.position;
            withoutHorizontal.x = vectorX;

			setNewPositon (withoutHorizontal + offset);

			//Set position and rotation

			transform.position = Vector3.Lerp (transform.position, shouldBePosition, smoothTime * Time.deltaTime);
			//transform.position = Vector3.SmoothDamp(transform.position, shouldBePosition, ref velocity, smoothTime);

			transform.rotation = Quaternion.Lerp (transform.rotation, shouldBeRotation, smoothTime * Time.deltaTime);

        }
	}

    public void SetPlayer(GameObject player)
    {
		this.player = player;

		Debug.Log ("SetPlayer: " + player.name + ", " + isFirstTeam);

		PlayerInfoController info = (PlayerInfoController)player.GetComponent (typeof(PlayerInfoController));
		isFirstTeam = info.firstTeam;

		if (isFirstTeam) {
			//Set the roation.y to 180
			setNewRotation (new Vector3(55f, 180f, transform.rotation.z));

			//First position of the camera
			//offset = new Vector3 (0, 10, 4.8f);
			setNewPositon (player.transform.position + offset);
		} else {
			//First position of the camera
			offset = new Vector3 (offset.x, offset.y, -offset.z);
			setNewRotation (new Vector3(rotation.x, 0f, rotation.z));
			setNewPositon (player.transform.position + offset);
		}
    }

	private Vector3 shouldBePosition;

	private void setNewPositon(Vector3 position) {
		shouldBePosition = position;
	}

	private Quaternion shouldBeRotation;

	private void setNewRotation(Vector3 rotation) {
		shouldBeRotation = Quaternion.Euler(rotation);
	}
}
