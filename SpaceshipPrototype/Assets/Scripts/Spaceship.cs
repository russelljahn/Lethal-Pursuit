using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {


	public Vector3 acceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 10000.0f, 10000.0f);

	private Rigidbody rigidbody;
	private ParticleSystem flames;
	public GameObject pivot;

	private Vector3 rightTiltRotation = new Vector3( 0.0f,  0.0f,  30.0f);
	private Vector3 leftTiltRotation  = new Vector3( 0.0f,  0.0f, -30.0f);
	private Vector3 upTiltRotation    = new Vector3(-30.0f, 0.0f,   0.0f);
	private Vector3 downTiltRotation  = new Vector3( 30.0f, 0.0f,   0.0f);

	private Vector3 downRightTiltRotation  = new Vector3(  30.0f, 0.0f,  30.0f);
	private Vector3 downLeftTiltRotation   = new Vector3(  30.0f, 0.0f, -30.0f);
	private Vector3 upRightTiltRotation    = new Vector3( -30.0f, 0.0f,  30.0f);
	private Vector3 upLeftTiltRotation     = new Vector3( -30.0f, 0.0f, -30.0f);
	

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		flames = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
	}
	

	void FixedUpdate () {
	
		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		float zForce;

		rigidbody.velocity = new Vector3(
			Mathf.Min(rigidbody.velocity.x, maxVelocity.x),
			Mathf.Min(rigidbody.velocity.y, maxVelocity.y),
			Mathf.Min(rigidbody.velocity.z, maxVelocity.z)
		);

		if (Input.GetButton("Boost")) { //Spacebar by default will make it move forward
			rigidbody.AddRelativeForce (
				transform.InverseTransformDirection(Vector3.forward)*acceleration.z*Time.deltaTime
			);
//			flames.loop = true;
		}
		else {
//			flames.loop = false;
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


		if (xTilt == 0) {
			if (yTilt == 0) {
				this.transform.localRotation = Quaternion.identity;
			}
			else if (yTilt < 0) {
				this.transform.localRotation = Quaternion.Euler(downTiltRotation);
			}
			else if (yTilt > 0) {
				this.transform.localRotation = Quaternion.Euler(upTiltRotation);
			}
		}
		else if (xTilt < 0) {
			if (yTilt == 0) {
				this.transform.localRotation = Quaternion.Euler(leftTiltRotation);
			}
			else if (yTilt < 0) {
				this.transform.localRotation = Quaternion.Euler(downLeftTiltRotation);
			}
			else if (yTilt > 0) {
				this.transform.localRotation = Quaternion.Euler(upLeftTiltRotation);
			}
		}
		else if (xTilt > 0) {
			if (yTilt == 0) {
				this.transform.localRotation = Quaternion.Euler(rightTiltRotation);
			}
			else if (yTilt < 0) {
				this.transform.localRotation = Quaternion.Euler(downRightTiltRotation);
			}
			else if (yTilt > 0) {
				this.transform.localRotation = Quaternion.Euler(upRightTiltRotation);
			}
		}


//		Vector3 rotationAngle = transform.localRotation.eulerAngles;
//
//		float zAngle = transform.localRotation.eulerAngles.z;
//
//		Debug.Log ("zAngle: " + zAngle);
//
//		if ((zAngle >= 345.0f && zAngle <= 360.0f) || (zAngle <= 15.0f && zAngle >= -15.0f)) {
//			Debug.Log ("Able to tilt ship up/down!");
//			transform.RotateAround(
//				pivot.transform.position, 
//				Vector3.right, 
//				-Time.deltaTime*yTilt*acceleration.x*0.01f
//			);
//
//		}
	
		

	}
}
