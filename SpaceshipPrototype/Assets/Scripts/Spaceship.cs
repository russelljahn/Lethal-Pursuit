using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {


	public Vector3 acceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 10000.0f, 10000.0f);

	public float tiltSpeed = 2.0f;

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
	



	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		flames = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
	}




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
			rigidbody.AddRelativeForce (
				transform.InverseTransformDirection(Vector3.forward)*acceleration.z*Time.deltaTime
			);
		}
		
		RaycastHit hit;
		
		float horizontalDistanceToRaycast = 200.0f;
		/* Left/Right movement if not going to collide... */
		if (!Physics.Raycast(pivot.transform.position, Vector3.right*xTilt, out hit, horizontalDistanceToRaycast)) {
			this.transform.Translate(
				transform.InverseTransformDirection(
					new Vector3(xTilt*Time.deltaTime*acceleration.x, 0.0f, 0.0f)
				)
			);
		}
		
		float verticalDistanceToRaycast = 70.0f;
		/* Up/Down movement if not going to collide... */
		if (!Physics.Raycast(pivot.transform.position, Vector3.up*yTilt, out hit, verticalDistanceToRaycast)) {
			this.transform.Translate(
				transform.InverseTransformDirection(
					new Vector3(0.0f, yTilt*Time.deltaTime*acceleration.y, 0.0f)
				)
			);
		}
	}




	void HandleTilt() {

		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		Vector3 targetRotationEuler = Vector3.zero;
		
		if (xTilt == 0) {
			if (yTilt == 0) {
				;
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
		
		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotationEuler), tiltSpeed*Time.deltaTime);
	}

	



}
