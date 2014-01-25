using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {

	public Vector3 instantaneousVelocity = new Vector3(000.0f, 0.0f, 1000.0f);
	public Vector3 acceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 deacceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 10000.0f, 10000.0f);

	public float xTiltSpeed = 2.0f;
	public float yTiltSpeed = 4.0f;

	private Rigidbody rigidbody;
	private ParticleSystem flames;
	public GameObject pivot;

	public Vector3 rightTiltRotation = new Vector3(  0.0f,  0.0f,  30.0f);
	public Vector3 leftTiltRotation  = new Vector3(  0.0f,  0.0f, -30.0f);
	public Vector3 upTiltRotation    = new Vector3(-30.0f,  0.0f,   0.0f);
	public Vector3 downTiltRotation  = new Vector3( 30.0f,  0.0f,   0.0f);

	public Vector3 downRightTiltRotation  = new Vector3(  30.0f,  0.0f,  30.0f);
	public Vector3 downLeftTiltRotation   = new Vector3(  30.0f,  0.0f, -30.0f);
	public Vector3 upRightTiltRotation    = new Vector3( -30.0f,  0.0f,  30.0f);
	public Vector3 upLeftTiltRotation     = new Vector3( -30.0f,  0.0f, -30.0f);

	private Vector3 lastTrameTargetRotationEuler;
	
	private bool boostedLastFrame = false;


	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		flames = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
		lastTrameTargetRotationEuler = Vector3.zero;
	}


	void Update() {

	}

	// This happens at a fixed timestep
	void FixedUpdate () {
	
		HandleParticles();
		HandleMovement();
		HandleTilt();

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
		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		/* Constrain max velocity. */
		rigidbody.velocity = new Vector3(
			Mathf.Min(rigidbody.velocity.x, maxVelocity.x),
			Mathf.Min(rigidbody.velocity.y, maxVelocity.y),
			Mathf.Min(rigidbody.velocity.z, maxVelocity.z)
		);

		/* Forward boost. */
		if (Input.GetButton("Boost")) {
			if (!boostedLastFrame) {
				rigidbody.velocity = instantaneousVelocity;
			}
			rigidbody.AddRelativeForce (
				transform.InverseTransformDirection(Vector3.forward)*acceleration.z*Time.deltaTime
			);
			boostedLastFrame = true;
		}
		else {
			boostedLastFrame = false;
			rigidbody.AddRelativeForce (
				transform.InverseTransformDirection(Vector3.back)*deacceleration.z*Time.deltaTime
			);
			if (rigidbody.velocity.z <= 0.0f) {
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, 0.0f);
			}
		}
		
		RaycastHit hit;
		
		float horizontalDistanceToRaycast = 200.0f;
		/* Do left/right movement if not going to collide... */
		if (!Physics.Raycast(pivot.transform.position, Vector3.right*xTilt, out hit, horizontalDistanceToRaycast)) {
			this.transform.Translate(
				transform.InverseTransformDirection(
					new Vector3(xTilt*Time.deltaTime*acceleration.x, 0.0f, 0.0f)
				)
			);
		}

//		rigidbody.MovePosition(
//			transform.InverseTransformDirection(
//				this.transform.position + new Vector3(xTilt*Time.deltaTime*acceleration.x, 0.0f, 0.0f)
//			)
//		);
		
		float verticalDistanceToRaycast = 70.0f;
		/* Do up/down movement if not going to collide... */
		if (!Physics.Raycast(pivot.transform.position, Vector3.up*yTilt, out hit, verticalDistanceToRaycast)) {
			this.transform.Translate(
				transform.InverseTransformDirection(
					new Vector3(0.0f, yTilt*Time.deltaTime*acceleration.y, 0.0f)
				)
			);
		}
//		rigidbody.MovePosition(
//			transform.InverseTransformDirection(
//				this.transform.position + new Vector3(0.0f, yTilt*Time.deltaTime*acceleration.y, 0.0f)
//			)
//		);
	}




	void HandleTilt() {

		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

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
		if (xTilt != 0) {
			transform.localRotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler((1.0f-.5f*(xTilt+1.0f))*lastTrameTargetRotationEuler + .5f*(xTilt+1.0f)*targetRotationEuler), 
				xTiltSpeed*Time.deltaTime
			);
		}
		if (yTilt != 0) {
			transform.localRotation = Quaternion.Slerp(
				transform.localRotation, 
				Quaternion.Euler((1.0f-.5f*(yTilt+1.0f))*lastTrameTargetRotationEuler + .5f*(yTilt+1.0f)*targetRotationEuler), 
				yTiltSpeed*Time.deltaTime
				);
		}
		if (xTilt == 0 && yTilt == 0) {
			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotationEuler), xTiltSpeed*Time.deltaTime);
		}
		lastTrameTargetRotationEuler = targetRotationEuler;
	}
	
}
