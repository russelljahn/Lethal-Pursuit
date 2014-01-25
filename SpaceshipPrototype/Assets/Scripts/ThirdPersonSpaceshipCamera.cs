using UnityEngine;
using System.Collections;

public class ThirdPersonSpaceshipCamera : MonoBehaviour {


	public Spaceship spaceship;
	public float tiltSpeed = 1.0f;

	public Vector3 idlePosition = new Vector3(0.0f, 100.0f, -285.0f);
	public Vector3 idleRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 upPosition = new Vector3(0.0f, 140.0f, -285.0f);
	public Vector3 upRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 downPosition = new Vector3(0.0f, 140.0f, -285.0f);
	public Vector3 downRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 leftRotation = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 rightRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 upLeftPosition = new Vector3(-140.0f, 140.0f, -350.0f);
	public Vector3 upLeftRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 upRightPosition = new Vector3(140.0f, 140.0f, -350.0f);
	public Vector3 upRightRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 downLeftPosition = new Vector3(-140.0f, 140.0f, -350.0f);
	public Vector3 downLeftRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 downRightPosition = new Vector3(140.0f, 140.0f, -350.0f);
	public Vector3 downRightRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public float idlePanSpeed = 15.0f;
	public float upDownPanSpeed = 20.0f;
	public float leftRightPanSpeed = 10.0f;
	public float upLeftRightPanSpeed = 20.0f;
	public float downLeftRightPanSpeed = 20.0f;

	public float idleRotationSpeed = 1.5f;
	public float upDownRotationSpeed = 1.75f;
	public float leftRightRotationSpeed = 1.5f;
	

	void Start() {
		this.transform.position = spaceship.transform.position + idlePosition;
	}

	void FixedUpdate () {
//		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;

		if (Input.GetAxis("Vertical") > 0) {

			if (Input.GetAxis("Horizontal") == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + upPosition, 
			        upDownPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(upRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + upLeftPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(upLeftRotation), 
					tiltSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + upRightPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(upRightRotation), 
					tiltSpeed*Time.deltaTime
				);
			}
		}
		else if (Input.GetAxis("Vertical") < 0) {
			
			if (Input.GetAxis("Horizontal") == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + downPosition, 
					upDownPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(downRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + downLeftPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(downLeftRotation), 
					tiltSpeed*Time.deltaTime
				);
			}
			else if (Input.GetAxis("Horizontal") > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + downRightPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(downRightRotation), 
					tiltSpeed*Time.deltaTime
				);
			}
		}
		else if (Input.GetAxis("Horizontal") == 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + idlePosition, 
				leftRightPanSpeed*Time.deltaTime
			);
			this.transform.rotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(idleRotation), 
				idleRotationSpeed*Time.deltaTime
			);
		}
		else if (Input.GetAxis("Horizontal") < 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + idlePosition, 
				idlePanSpeed*Time.deltaTime
			);
			this.transform.rotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(leftRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}
		else if (Input.GetAxis("Horizontal") > 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + idlePosition, 
				idlePanSpeed*Time.deltaTime
			);
			this.transform.rotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(rightRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}


//		this.transform.rotation = Quaternion.Slerp(transform.localRotation, spaceship.transform.rotation, tiltSpeed*Time.deltaTime);
	}
}
