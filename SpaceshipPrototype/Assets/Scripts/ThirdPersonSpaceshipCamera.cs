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
	public float diagonalRotationSpeed = 1.5f;
	public float upDownRotationSpeed = 1.75f;
	public float leftRightRotationSpeed = 1.5f;
	

	void Start() {
		this.transform.position = spaceship.transform.position + idlePosition;
	}

	void FixedUpdate () {
//		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;
		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		if (yTilt > 0) {

			if (xTilt == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + yTilt*upPosition, 
			        upDownPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(yTilt*upRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + xTilt*yTilt*upLeftPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTilt*yTilt*upLeftRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + xTilt*yTilt*upRightPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTilt*yTilt*upRightRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
		}
		else if (yTilt < 0) {
			
			if (xTilt == 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + yTilt*downPosition, 
					upDownPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(yTilt*downRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt < 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + xTilt*yTilt*downLeftPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTilt*yTilt*downLeftRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt > 0) {
				this.transform.position = Vector3.Slerp(
					this.transform.position, 
					spaceship.transform.position + xTilt*yTilt*downRightPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.rotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTilt*yTilt*downRightRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
		}
		else if (xTilt == 0) {
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
		else if (xTilt < 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + idlePosition, 
				idlePanSpeed*Time.deltaTime
			);
			this.transform.rotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(xTilt*leftRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}
		else if (xTilt > 0) {
			this.transform.position = Vector3.Slerp(
				this.transform.position, 
				spaceship.transform.position + idlePosition, 
				idlePanSpeed*Time.deltaTime
			);
			this.transform.rotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(xTilt*rightRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}


//		this.transform.rotation = Quaternion.Slerp(transform.localRotation, spaceship.transform.rotation, tiltSpeed*Time.deltaTime);
	}
}
