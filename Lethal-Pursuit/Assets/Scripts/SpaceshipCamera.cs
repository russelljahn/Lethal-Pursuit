using UnityEngine;
using System.Collections;

public class SpaceshipCamera : MonoBehaviour {


	public GameObject spaceship;

	/* These are relative to camera's local position. */
	public Vector3 idlePosition = new Vector3(0.0f, 110.0f, -400.0f);
	public Vector3 leftPosition = new Vector3(0.0f, -20.0f, -350.0f);
	public Vector3 rightPosition = new Vector3(0.0f, -20.0f, -350.0f);
	public Vector3 upPosition = new Vector3(0.0f, -300.0f, -200.0f);
	public Vector3 downPosition = new Vector3(0.0f, 100.0f, 100.0f);
	public Vector3 upLeftPosition = new Vector3(-300.0f, -150.0f, -500.0f);
	public Vector3 upRightPosition = new Vector3(300.0f, -150.0f, -500.0f);
	public Vector3 downLeftPosition = new Vector3(-300.0f, 120.0f, -500.0f);
	public Vector3 downRightPosition = new Vector3(300.0f, 120.0f, -500.0f);

	public Vector3 idleRotation = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 leftRotation = new Vector3(0.0f, -45.0f, 0.0f);
	public Vector3 rightRotation = new Vector3(0.0f, 45.0f, 0.0f);
	public Vector3 upRotation = new Vector3(-80.0f, 0.0f, 0.0f);
	public Vector3 downRotation = new Vector3(80.0f, 0.0f, 0.0f);
	public Vector3 upLeftRotation = new Vector3(-80.0f, -45.0f, 0.0f);
	public Vector3 upRightRotation = new Vector3(-80.0f, 45.0f, 0.0f);
	public Vector3 downLeftRotation = new Vector3(80.0f, -45.0f, 0.0f);
	public Vector3 downRightRotation = new Vector3(80.0f, 45.0f, 0.0f);
	

	public float idlePanSpeed = 15.0f;
	public float leftRightPanSpeed = 2.0f;
	public float upDownPanSpeed = 1.0f;
	public float upLeftRightPanSpeed = 2.0f;
	public float downLeftRightPanSpeed = 2.0f;

	public float idleRotationSpeed = 2.0f;
	public float leftRightRotationSpeed = 2.0f;
	public float upDownRotationSpeed = 1.0f;
	public float diagonalRotationSpeed = 1.0f;
	

	private float xTilt;
	private float yTilt;

	

	void Start() {
//		this.transform.position = spaceship.transform.position + idlePosition;
//		this.transform.localPosition = distanceFromSpaceship;
		this.transform.localPosition = idlePosition;
	}




	void Update() {
		xTilt = Input.GetAxis("Horizontal");
		yTilt = Input.GetAxis("Vertical");

		/* Map keyboard axis amount to joystick axis amount. */
		if (Mathf.Abs(xTilt) > 0.5f && Mathf.Abs(yTilt) > 0.5f) {
			xTilt *= 0.5f;
			yTilt *= 0.5f;
		}

		Debug.Log ("xTilt: " + xTilt);
		Debug.Log ("yTilt: " + yTilt);
		
	}


	void FixedUpdate() {
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
