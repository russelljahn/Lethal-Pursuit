using UnityEngine;
using System.Collections;

public class CameraFollowSpaceship : MonoBehaviour {


	public Spaceship spaceship;
	public float tiltSpeed = 1.0f;
	public Vector3 distanceSpaceshipToCamera = new Vector3(0.0f, 100.0f, -285.0f);
	public Vector3 lookingDownPanPosition = new Vector3(0.0f, 140.0f, -285.0f);
	public Vector3 topLeftPanPosition = new Vector3(-140.0f, 140.0f, -350.0f);
	public Vector3 topRightPanPosition = new Vector3(140.0f, 140.0f, -350.0f);
	public Vector3 downLeftPanPosition = new Vector3(-140.0f, 140.0f, -350.0f);
	public Vector3 downRightPanPosition = new Vector3(140.0f, 140.0f, -350.0f);
	public float normalPanSpeed = 10.0f;
	public float upDownPanSpeed = 10.0f;
	public float leftRightPanSpeed = 50.0f;
	public float topLeftRightPanSpeed = 50.0f;
	public float downLeftRightPanSpeed = 50.0f;
	

	void Start() {
		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;
	}

	void FixedUpdate () {
//		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;

		if (Input.GetAxis("Vertical") > 0) {

			if (Input.GetAxis("Horizontal") == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + lookingDownPanPosition, 
			        upDownPanSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + topLeftPanPosition, 
					topLeftRightPanSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + topRightPanPosition, 
					topLeftRightPanSpeed*Time.deltaTime
				);
			}
		}
		else if (Input.GetAxis("Vertical") < 0) {
			
			if (Input.GetAxis("Horizontal") == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + lookingDownPanPosition, 
					upDownPanSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + downLeftPanPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + downRightPanPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
			}
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
