using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Spaceship))]
[RequireComponent (typeof (Collider))]
public class SpaceshipControl : SpaceshipComponent {
	

	public float acceleration = 5.0f; 
	public float deaccelerationBrake = 500;
	public float deaccelerationDrift = 50;
	public float deaccelerationIdle = 500;


	public float xTiltSpeed = 1.5f;
	public float yTiltSpeed = 3.3f;


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
	public float driftingTurningRate = 300.0f;
	public float nosedivingRate = 2.75f;
	

	
	public float timeUntilMaxTurning = 2.6f;
	private float timeSinceStartedTurning = 0.0f;

	public Crosshairs crosshairs;

	public float zForward = 30.0f;
	
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	
	
	
	
	public override void Update () {
		base.Update();
	}
	
	
	
	
	// This happens at a fixed timestep
	void FixedUpdate () {
		HandleRotation();
		HandleMovement();
		HandleTilt();
		HandleFalling();
	}

	
	
	
	
	
	void HandleMovement() {

		/* Forward boost. */
		if (drifting || nosediving) {
			currentVelocity -= deaccelerationDrift;
		}
		else if (boosting) {
			currentVelocity += acceleration;
		}
		else if (braking) {
			currentVelocity -= deaccelerationBrake;
		}
		else if (idle) {
			currentVelocity -= deaccelerationIdle;
		} 

		currentVelocity = Mathf.Clamp(currentVelocity, 0f, maxVelocity);

		/* Boost forward. */
		rigidbody.MovePosition(
			rigidbody.position + Vector3.Slerp(Vector3.zero, forward*Time.deltaTime*currentVelocity, currentVelocity/maxVelocity)
		);

	}
	
	
	


	
	void HandleTilt() {

		float z = spaceshipModel.transform.position.z + zForward;

		Vector3 pointToLookAt = new Vector3(
			spaceshipModel.transform.position.x+crosshairs.transform.localPosition.x, 
			spaceshipModel.transform.position.y+crosshairs.transform.localPosition.y, 
			z
		);
		spaceshipModel.transform.LookAt(crosshairs.transform);

	}
	
	
	
	
	
	
	void HandleRotation() {

		if (xTilt != 0) {
			
			float turningRateForThisFrame = normalTurningRate;
			
			/* Allow drift turning if player is holding down brake. */
			if (drifting) {
				turningRateForThisFrame = driftingTurningRate;
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






	void HandleFalling() {

		if (enforceHeightLimit && spaceship.transform.position.y > maxHeightBeforeFalling) {
			rigidbody.MovePosition(
				Vector3.Slerp(
					rigidbody.position,
					rigidbody.position + Vector3.down*Time.deltaTime*fallingRate,
					Time.deltaTime
				)
			);
		}
	}
	
	
	
}