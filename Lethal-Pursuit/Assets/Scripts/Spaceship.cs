using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {

	public Vector3 instantaneousVelocity = new Vector3(0.0f, 0.0f, 2500.0f);
	public Vector3 acceleration = new Vector3(3000.0f, 2500.0f, 13000.0f);
	public Vector3 deacceleration = new Vector3(0.0f, 0.0f, 150000.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 4000.0f, 8000.0f);

	public float xTiltSpeed = 0.5f;
	public float yTiltSpeed = 4.0f;

	private Rigidbody rigidbody;
	public ParticleSystem flames;
	public GameObject spaceshipModel;


	public Vector3 rightTiltRotation = new Vector3(  0.0f,  15.0f,  -60.0f);
	public Vector3 leftTiltRotation  = new Vector3(  0.0f,  -15.0f, 60.0f);

	public Vector3 upTiltRotation    = new Vector3(-45.0f,  0.0f,   0.0f);
	public Vector3 downTiltRotation  = new Vector3( 45.0f,  0.0f,   0.0f);

	public Vector3 downRightTiltRotation  = new Vector3(  30.0f,  0.0f,  -60.0f);
	public Vector3 downLeftTiltRotation   = new Vector3(  30.0f,  0.0f, 60.0f);

	public Vector3 upRightTiltRotation    = new Vector3( -30.0f,  0.0f,  -60.0f);
	public Vector3 upLeftTiltRotation     = new Vector3( -30.0f,  0.0f, 60.0f);

	private Vector3 lastFrameTargetRotationEuler;


	public float turningRate = 1.0f;

	private bool boostedLastFrame = false;

	public float timeUntilCompleteStopAfterBoost = 2.0f;
	private float timeSinceLastBoost = 0.0f;

	public float timeUntilMaxTurning = 2.0f;
	private float timeSinceStartedTurning = 0.0f;


	/* Tilt of analogue stick every frame. */
	private float xTilt;
	private float yTilt;

	private bool collidingDown = false;
	private bool collidingForward = false;



	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		lastFrameTargetRotationEuler = Vector3.zero;
	
	}





	void Update() {
		xTilt = Input.GetAxis("Horizontal");
		yTilt = Input.GetAxis("Vertical");
		HandleParticles();
	}




	// This happens at a fixed timestep
	void FixedUpdate () {

		HandleRotation();
		HandleMovement();
		HandleTilt();



	}



	void HandleCollisionDetection() {
//		RaycastHit hit;
//		float distanceToRaycastDown = 10.0f;
//		float distanceToRaycastForward = 10.0f;
//		
//		
//		if (Physics.Raycast(this.transform.position, Vector3.down, out hit, distanceToRaycastDown)) {
//			collidingDown = hit.collider.CompareTag("Unpassable");
//		}
//		if (Physics.Raycast(this.transform.position, spaceshipModel.transform.forward, out hit, distanceToRaycastForward)) {
//			collidingForward = hit.collider.CompareTag("Unpassable");
//		}
	}

	void OnCollisionStay(Collision collision) {
		Debug.Log (this.gameObject.name + " collided with " + collision.collider.gameObject.name + "!");
		for (int i = 0; i < collision.contacts.Length; ++i) {
			ContactPoint contactPoint = collision.contacts[i];
			Vector3 shipToContactDirection = contactPoint.point - this.transform.position;
			if (shipToContactDirection.y < 0) {
				Debug.Log ("Colliding down!");
				collidingDown = true;
				break;
			}
		}
		collidingDown = false;
	}


	
	void HandleParticles() {

		if (Input.GetButton("Boost")) {
			flames.enableEmission = true;
		}
		else {
			flames.enableEmission = false;
		}
	}




	void HandleMovement() {

//		Debug.Log ("transform.forward: " + transform.forward);

		/* Constrain max velocity. */
		rigidbody.velocity = new Vector3(
			Mathf.Min(rigidbody.velocity.x, maxVelocity.x),
			Mathf.Min(rigidbody.velocity.y, maxVelocity.y),
			Mathf.Min(rigidbody.velocity.z, maxVelocity.z)
		);

		Vector3 forwardVector = spaceshipModel.transform.forward;
		forwardVector.y = 0.0f;

		/* Forward boost. */
		if (Input.GetButton("Boost")) {

			/* Boost forward. */
			if (!collidingForward) {
				rigidbody.MovePosition(rigidbody.position + forwardVector*Time.deltaTime*acceleration.z);
			}
			/* Move up/down. */
			if (!collidingDown) {
				rigidbody.MovePosition(rigidbody.position + Vector3.up*yTilt*Time.deltaTime*acceleration.y);
			}

			timeSinceLastBoost = 0.0f;
			boostedLastFrame = true;
		}
		else {
			/* Boost forward at a decreasing rate (Eventually slow down). */
			if (!collidingForward) {
				rigidbody.MovePosition(
					Vector3.Slerp(
						rigidbody.position + forwardVector*Time.deltaTime*acceleration.z,
						rigidbody.position,
						timeSinceLastBoost/timeUntilCompleteStopAfterBoost
					)
				);
			}
			/* Boost up/down at a decreasing rate (Eventually slow down). */
			if (!collidingDown) {
				rigidbody.MovePosition(
					Vector3.Slerp(
						rigidbody.position + Vector3.up*yTilt*Time.deltaTime*acceleration.y,
						rigidbody.position,
						timeSinceLastBoost/timeUntilCompleteStopAfterBoost
					)
				);
			}




			timeSinceLastBoost += Time.deltaTime;
			boostedLastFrame = false;
		}

	}




	void HandleTilt() {

		Vector3 targetRotationEuler = Vector3.zero;

		/* Based on analogue stick direction, figure out rotation state to blend to. */
		if (xTilt == 0) {
			if (yTilt == 0) {
				; // Idle state, total rotation of zero
			}
			else if (yTilt < 0) {
				targetRotationEuler = downTiltRotation;
			}
			else if (yTilt > 0) {
				targetRotationEuler = upTiltRotation;
			}
		}
		else if (xTilt < 0) {
			if (yTilt == 0) {
				targetRotationEuler = leftTiltRotation;
			}
			else if (yTilt < 0) {
				targetRotationEuler = downLeftTiltRotation;
			}
			else if (yTilt > 0) {
				targetRotationEuler = upLeftTiltRotation;
			}
		}
		else if (xTilt > 0) {
			if (yTilt == 0) {
				targetRotationEuler = rightTiltRotation;
			}
			else if (yTilt < 0) {
				targetRotationEuler = downRightTiltRotation;
			}
			else if (yTilt > 0) {
				targetRotationEuler = upRightTiltRotation;
			}
		}
	
		/* Blend from current rotation towards target rotation. */
		if (yTilt != 0) {
			spaceshipModel.transform.localRotation = Quaternion.Slerp(
				spaceshipModel.transform.localRotation, 
				Quaternion.Euler((1.0f-.5f*(yTilt+1.0f))*lastFrameTargetRotationEuler + .5f*(yTilt+1.0f)*targetRotationEuler), 
				yTiltSpeed*Time.deltaTime
				);
		}
		if (xTilt != 0) {
			spaceshipModel.transform.localRotation = Quaternion.Slerp(
				spaceshipModel.transform.localRotation, 
				Quaternion.Euler((1.0f-.5f*(xTilt+1.0f))*lastFrameTargetRotationEuler + .5f*(xTilt+1.0f)*targetRotationEuler), 
				xTiltSpeed*Time.deltaTime
			);
		}
		if (xTilt == 0 && yTilt == 0) {
			spaceshipModel.transform.localRotation = Quaternion.Slerp(
				spaceshipModel.transform.localRotation, 
				Quaternion.Euler(targetRotationEuler), 
				xTiltSpeed*Time.deltaTime
			);
		}
		lastFrameTargetRotationEuler = targetRotationEuler;
	}






	void HandleRotation() {


		if (xTilt != 0) {
			this.rigidbody.MoveRotation(
				Quaternion.Slerp (
					this.transform.localRotation,
					Quaternion.Euler(this.transform.localRotation.eulerAngles + Vector3.up*xTilt*turningRate*Time.deltaTime),
					Mathf.Clamp01(timeSinceStartedTurning/timeUntilMaxTurning)
				)
			);
			timeSinceStartedTurning += Time.deltaTime;
		}
		else {
			timeSinceStartedTurning = 0.0f;
		}

	}





		
}
