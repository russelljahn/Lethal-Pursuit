using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipCamera : MonoBehaviour {


	public Spaceship spaceship;

	/* These are relative to camera's local position. */
	public Vector3 idlePosition = new Vector3(0.0f, 0.0f, 41.0f);
	public Vector3 leftPosition = new Vector3(0.0f,   0.0f, -295.0f);
	public Vector3 rightPosition = new Vector3(0.0f,  0.0f, -295.0f);
	public Vector3 upPosition = new Vector3(0.0f, -660.0f, -250.0f);
	public Vector3 downPosition = new Vector3(0.0f, 500.0f, -200.0f);
	public Vector3 upLeftPosition = new Vector3(-300.0f, -800.0f, -100.0f);
	public Vector3 upRightPosition = new Vector3(300.0f, -800.0f, -100.0f);
	public Vector3 downLeftPosition = new Vector3(-300.0f, 1000.0f, -1000.0f);
	public Vector3 downRightPosition = new Vector3(300.0f, 1000.0f, -1000.0f);

	public Vector3 idleRotation = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 leftRotation = new Vector3(0.0f, -45.0f, 5.0f);
	public Vector3 rightRotation = new Vector3(0.0f, 45.0f, 5.0f);
	public Vector3 upRotation = new Vector3(-60.0f, 0.0f, 0.0f);
	public Vector3 downRotation = new Vector3(60.0f, 0.0f, 0.0f);
	public Vector3 upLeftRotation = new Vector3(-200.0f, -0.0f, 0.0f);
	public Vector3 upRightRotation = new Vector3(-200.0f, 0.0f, 0.0f);
	public Vector3 downLeftRotation = new Vector3(200.0f, -0.0f, 0.0f);
	public Vector3 downRightRotation = new Vector3(200.0f, 0.0f, 0.0f);
	

	public float idlePanSpeed = 15.0f;
	public float leftRightPanSpeed = 1.0f;
	public float upDownPanSpeed = 1.0f;
	public float upLeftRightPanSpeed = 1.0f;
	public float downLeftRightPanSpeed = 1.0f;

	public float idleRotationSpeed = 1.0f;
	public float leftRightRotationSpeed = 1.3f;
	public float upDownRotationSpeed = 1.6f;
	public float diagonalRotationSpeed = 1.3f;



	public void SetSpaceship(Spaceship spaceship) {
		this.spaceship = spaceship;
		this.transform.parent = spaceship.transform;
		this.transform.localPosition = idlePosition;
	}



	void Start() {
		this.transform.localPosition = idlePosition;
	}

	


	void FixedUpdate() {

		float xTilt = spaceship.xTilt;
		float yTilt = spaceship.yTilt;
	
		float xTiltAbsolute = Mathf.Abs(xTilt);
		float yTiltAbsolute = Mathf.Abs(yTilt);	
		

		if (yTilt > 0) {

			if (xTilt == 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					upPosition, 
			        upDownPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(yTiltAbsolute*upRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt < 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					xTiltAbsolute*yTiltAbsolute*upLeftPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTiltAbsolute*yTiltAbsolute*upLeftRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt > 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					xTiltAbsolute*yTiltAbsolute*upRightPosition, 
					upLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTiltAbsolute*yTiltAbsolute*upRightRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
		}
		else if (yTilt < 0) {
			
			if (xTilt == 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					downPosition, 
					upDownPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(yTiltAbsolute*downRotation), 
					upDownRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt < 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					xTiltAbsolute*yTiltAbsolute*downLeftPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTiltAbsolute*yTiltAbsolute*downLeftRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
			else if (xTilt > 0) {
				this.transform.localPosition = Vector3.Slerp(
					this.transform.localPosition, 
					xTiltAbsolute*yTiltAbsolute*downRightPosition, 
					downLeftRightPanSpeed*Time.deltaTime
				);
				this.transform.localRotation = Quaternion.Slerp(
					transform.localRotation, 
					Quaternion.Euler(xTiltAbsolute*yTiltAbsolute*downRightRotation), 
					diagonalRotationSpeed*Time.deltaTime
				);
			}
		}
		if (xTilt == 0) {
			this.transform.localPosition = Vector3.Slerp(
				this.transform.localPosition, 
				idlePosition, 
				leftRightPanSpeed*Time.deltaTime
			);
			this.transform.localRotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(idleRotation), 
				idleRotationSpeed*Time.deltaTime
			);
		}
		else if (xTilt < 0) {
			this.transform.localPosition = Vector3.Slerp(
				this.transform.localPosition, 
				leftPosition, 
				leftRightPanSpeed*Time.deltaTime
			);
			this.transform.localRotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(xTiltAbsolute*leftRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}
		else if (xTilt > 0) {
			this.transform.localPosition = Vector3.Slerp(
				this.transform.localPosition, 
				rightPosition, 
				leftRightPanSpeed*Time.deltaTime
			);
			this.transform.localRotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler(xTiltAbsolute*rightRotation), 
				leftRightRotationSpeed*Time.deltaTime
			);
		}


//		this.transform.rotation = Quaternion.Slerp(transform.localRotation, spaceship.transform.rotation, tiltSpeed*Time.deltaTime);
	}
}
