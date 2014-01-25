using UnityEngine;
using System.Collections;

public class CameraFollowSpaceship : MonoBehaviour {


	public Spaceship spaceship;
	public Vector3 distanceSpaceshipToCamera = new Vector3(0.0f, 0.0f, -350.0f);
	public float tiltSpeed = 1.0f;
	public float upDownPanSpeed = 1.0f;
	public Vector3 lookingDownPanPosition = new Vector3(0.0f, 10.0f, -350.0f);
	public float normalPanSpeed = 10.0f;
	public float leftRightPanSpeed = 50.0f;

	void Start() {
		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;
	}

	void FixedUpdate () {
//		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;

		if (Input.GetAxis("Vertical") > 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + lookingDownPanPosition, 
		        upDownPanSpeed*Time.deltaTime
			);
		}
		else if (Input.GetAxis("Horizontal") != 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + distanceSpaceshipToCamera, 
				leftRightPanSpeed*Time.deltaTime
				);
		}
		else {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + distanceSpaceshipToCamera, 
				normalPanSpeed*Time.deltaTime
			);
		}


		this.transform.rotation = Quaternion.Slerp(transform.localRotation, spaceship.transform.rotation, tiltSpeed*Time.deltaTime);
	}
}
