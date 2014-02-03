using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {

	public Vector3 instantaneousVelocity = new Vector3(0.0f, 0.0f, 2500.0f);
	public Vector3 acceleration = new Vector3(160.0f, 160.0f, 160.0f);
	public Vector3 deacceleration = new Vector3(5000.0f, 5000.0f, 15000000);
	public Vector3 maxVelocity  = new Vector3(0.0f, 0.0f, 250.0f);

	public float xTiltSpeed = 1.5f;
	public float yTiltSpeed = 3.3f;


	public ParticleSystem boosterFlames;
	public ParticleSystem brakeFlames;
	public ParticleSystem driftFlames;
	public ParticleSystem hoverFlames;

	
	public GameObject spaceshipModel;


	public Vector3 rightTiltRotation = new Vector3(  0.0f,  0.0f,  -85.0f);
	public Vector3 leftTiltRotation  = new Vector3(  0.0f,  0.0f,   85.0f);

	public Vector3 upTiltRotation    = new Vector3(-40.0f,  2.0f,   -3.0f);
	public Vector3 downTiltRotation  = new Vector3( 4.0f,   2.0f,    3.0f);

	public Vector3 downRightTiltRotation  = new Vector3(  30.0f,  5.0f,  -40.0f);
	public Vector3 downLeftTiltRotation   = new Vector3(  30.0f, -5.0f,   40.0f);

	public Vector3 upRightTiltRotation    = new Vector3( -30.0f,  5.0f,  -80.0f);
	public Vector3 upLeftTiltRotation     = new Vector3( -30.0f, -5.0f,   80.0f);

	private Vector3 lastFrameTargetRotationEuler = Vector3.zero;


	public float normalTurningRate = 115.0f;
	public float brakingTurningRate = 300.0f;
	

	public float timeUntilCompleteStopAfterBoost = 1.0f;
	private float timeSinceLastBoost = 0.0f;

	public float timeUntilMaxTurning = 2.6f;
	private float timeSinceStartedTurning = 0.0f;

	public float timeUntilMaxBoostUpDown = 1.0f;
	private float timeSinceStartedBoostingUpDown = 0.0f;



	/* Tilt of analogue stick every frame. */
	private float xTilt;
	private float yTilt;



	// Use this for initialization
	void Start () {

	}





	void Update() {
		xTilt = Input.GetAxis("Horizontal");
		yTilt = Input.GetAxis("Vertical");

		/* Map keyboard diagonal axis amount to joystick diagonal axis amount. */
		if (Mathf.Abs(xTilt) > 0.5f && Mathf.Abs(yTilt) > 0.5f) {
			xTilt *= 0.5f;
			yTilt *= 0.5f;
		}

//		HandleParticles();
	}




	// This happens at a fixed timestep
	void FixedUpdate () {

		HandleRotation();
		HandleMovement();
		HandleTilt();



	}
	



	
	void HandleParticles() {

		if (Input.GetButton("Boost") && Input.GetButton("Brake")) {
			boosterFlames.gameObject.SetActive(false);
			brakeFlames.gameObject.SetActive(false);
			driftFlames.gameObject.SetActive(true);
			hoverFlames.gameObject.SetActive(false);
		}
		else if (Input.GetButton("Boost")) {
			boosterFlames.gameObject.SetActive(true);
			brakeFlames.gameObject.SetActive(false);
			driftFlames.gameObject.SetActive(false);
			hoverFlames.gameObject.SetActive(false);
		}
		else if (Input.GetButton("Brake")) {
			boosterFlames.gameObject.SetActive(false);
			brakeFlames.gameObject.SetActive(true);
			driftFlames.gameObject.SetActive(false);
			hoverFlames.gameObject.SetActive(false);
		}
		else {
			boosterFlames.gameObject.SetActive(false);
			brakeFlames.gameObject.SetActive(false);
			driftFlames.gameObject.SetActive(false);
			hoverFlames.gameObject.SetActive(true);
		}

	}





	void HandleMovement() {


		/* Constrain max velocity. */
		rigidbody.velocity = new Vector3(
			Mathf.Min(rigidbody.velocity.x, maxVelocity.x),
			Mathf.Min(rigidbody.velocity.y, maxVelocity.y),
			Mathf.Min(rigidbody.velocity.z, maxVelocity.z)
		);

		Vector3 forwardVector = spaceshipModel.transform.forward;
		forwardVector.y = 0.0f;

		if (yTilt != 0) {
			timeSinceStartedBoostingUpDown += Time.deltaTime;
		}
		else {
			timeSinceStartedBoostingUpDown = timeUntilMaxBoostUpDown;
		}


		/* Forward boost. */
		if (Input.GetButton("Boost") && !Input.GetButton("Brake")) {

			/* Boost forward. */
			rigidbody.MovePosition(rigidbody.position + forwardVector*Time.deltaTime*acceleration.z);

			/* Move up/down. The amount to move increases at an increasing rate so that you feel like you're pushing 
			 * into an up/down boost. */
			rigidbody.MovePosition(
				Vector3.Slerp(
					rigidbody.position,
					rigidbody.position + Vector3.up*yTilt*Time.deltaTime*acceleration.y,
					Mathf.Clamp01(timeSinceStartedBoostingUpDown/timeUntilMaxBoostUpDown)
				)
			);
			
			timeSinceLastBoost = 0.0f;
		}
		else {
			/* Boost forward at a decreasing rate (Eventually slow down). */
			rigidbody.MovePosition(
				Vector3.Slerp(
					rigidbody.position + forwardVector*Time.deltaTime*acceleration.z,
					rigidbody.position,
					Mathf.Clamp01(timeSinceLastBoost/timeUntilCompleteStopAfterBoost)
				)
			);
			/* Boost up/down at a decreasing rate (Eventually slow down). */
			rigidbody.MovePosition(
				Vector3.Slerp(
					rigidbody.position + Vector3.up*yTilt*Time.deltaTime*acceleration.y,
					rigidbody.position,
					Mathf.Clamp01(timeSinceLastBoost/timeUntilCompleteStopAfterBoost)
				)
			);

			timeSinceLastBoost += Time.deltaTime;
		}

	}





	void HandleTilt() {

		Vector3 targetRotationEuler = Vector3.zero;

		/* Based on analogue stick direction, figure out rotation state to blend to. */
		if (xTilt == 0) {
			if (yTilt == 0) {
				; // Idle state, target rotation is (0f, 0f, 0f).
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

			float turningRateForThisFrame = normalTurningRate;

			/* Allow drift turning if player is holding down brake. */
			if (Input.GetButton("Brake")) {
				turningRateForThisFrame = brakingTurningRate;
			}

//			Debug.Log("turningRate: " + turningRateForThisFrame);

			this.rigidbody.MoveRotation(
				Quaternion.Slerp (
					this.transform.localRotation,
					Quaternion.Euler(this.transform.localRotation.eulerAngles + Vector3.up*xTilt*turningRateForThisFrame*Time.deltaTime),
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
