using UnityEngine;
using System.Collections;

public class ThirdPersonSpaceshipCamera : MonoBehaviour {


	public GameObject spaceship;

	public Vector3 idlePosition = new Vector3(0.0f, 100.0f, -200.0f);
	public Vector3 idleRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 upPosition = new Vector3(0.0f, -100.0f, -285.0f);
	public Vector3 upRotation = new Vector3(-60.0f, 0.0f, 0.0f);

	public Vector3 downPosition = new Vector3(0.0f, 140.0f, -285.0f);
	public Vector3 downRotation = new Vector3(0.0f, 0.0f, 0.0f);

	public Vector3 leftRotation = new Vector3(0.0f, 25.0f, 0.0f);
	public Vector3 rightRotation = new Vector3(-60.0f, 0.0f, 0.0f);

	public Vector3 upLeftPosition = new Vector3(0.0f, 0.0f, -200.0f);
	public Vector3 upLeftRotation = new Vector3(60.0f, 100.0f, 0.0f);

	public Vector3 upRightPosition = new Vector3(0.0f, 0.0f, -200.0f);
	public Vector3 upRightRotation = new Vector3(-60.0f, 100.0f, 0.0f);

	public Vector3 downLeftPosition = new Vector3(0.0f, 0.0f, -200.0f);
	public Vector3 downLeftRotation = new Vector3(60.0f, -100.0f, 0.0f);

	public Vector3 downRightPosition = new Vector3(0.0f, 0.0f, -200.0f);
	public Vector3 downRightRotation = new Vector3(-60.0f, -100.0f, 0.0f);

	public float idlePanSpeed = 15.0f;
	public float upDownPanSpeed = 10.0f;
	public float leftRightPanSpeed = 10.0f;
	public float upLeftRightPanSpeed = 10.0f;
	public float downLeftRightPanSpeed = 10.0f;

	public float idleRotationSpeed = 2.0f;
	public float diagonalRotationSpeed = 2.5f;
	public float upDownRotationSpeed = 1.75f;
	public float leftRightRotationSpeed = 1.5f;

	private float xTilt;
	private float yTilt;

	public float rotateSpeed = 1.0f;


	public Vector3 distanceFromSpaceship;

	void Start() {
//		this.transform.position = spaceship.transform.position + idlePosition;
//		this.transform.localPosition = distanceFromSpaceship;
	}




	void Update() {
		xTilt = Input.GetAxis("Horizontal");
		yTilt = Input.GetAxis("Vertical");
	}




	void FixedUpdate () {
//		this.transform.localPosition = spaceship.transform.position + spaceship.transform.InverseTransformDirection(distanceFromSpaceship);
//		Vector3 newDirection = Vector3.RotateTowards(transform.forward, spaceship.transform.forward, Mathf.PI*rotateSpeed, rotateSpeed);
//		transform.rotation = Quaternion.LookRotation(newDirection);

		return;

//		float dot = Vector3.Dot(this.transform.forward, spaceship.transform.forward);

//		Quaternion rotation = Quaternion.AngleAxis(-dot*Time.deltaTime*rotateSpeed, Vector3.up);
		transform.RotateAround(spaceship.transform.position, Vector3.up, Time.deltaTime*rotateSpeed);

//		this.transform.rotation = rotation;
//		this.transform.rotation = Quaternion.LookRotation(spaceship.transform.forward);
//		transform.LookAt(spaceship.transform.position);
		return;

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
