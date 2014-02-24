using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Spaceship))]
[RequireComponent (typeof (Collider))]
public class SpaceshipControl : SpaceshipComponent {
	

	public float acceleration = 5.0f; 
	public float normalExtraAccelerationY = 5.0f; 
	public float nosedivingExtraAccelerationY = 30.0f; 
	
	
	public float deaccelerationBrake = 500;
	public float deaccelerationDrift = 50;
	public float deaccelerationIdle = 500;


	public float normalTurningRate = 115.0f;
	public float driftingTurningRate = 300.0f;
	
	
	public float timeUntilMaxTurning = 2.6f;
	private float timeSinceStartedTurning = 0.0f;

	public HUD_Crosshairs crosshairs;
	public float lookSpeed = 1.0f;

	public float normalTiltMax = 15f;
	public float driftTiltMax = 90f;

	private float currentDriftTilt = 0f;
	public float normalTiltRate = 2.0f;
	public float driftTiltRate = 2.5f;
	public float driftAlignRate = 1.5f;


	
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
	}

	
	
	
	
	
	void HandleMovement() {

		/* Adjust velocity based on current spaceship behavior. */
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


		float currentExtraAccelerationY = normalExtraAccelerationY;
		if (nosediving) {
			currentExtraAccelerationY = nosedivingExtraAccelerationY;
		}

		currentVelocity = Mathf.Clamp(currentVelocity, 0f, maxVelocity);


		/* Do height checking to see if spaceship should be allowed to boost higher vertically. */
		bool noUpwardsMovementThisFrame = enforceHeightLimit && heightAboveGround >= heightLimit;
		Vector3 adjustedForward = forward;
		float extraAccelerationThisFrameY = currentExtraAccelerationY;

		if (noUpwardsMovementThisFrame) {
			adjustedForward.y = Mathf.Min(0.0f, adjustedForward.y);
			extraAccelerationThisFrameY = 0.0f;
		}

		/* Do some collision detection. If you're about to hit the environment, then adjust for it. */
		RaycastHit hit;
		float distanceToRaycastForward = 20.0f;
		if (Physics.Raycast(transform.position, forward, out hit, distanceToRaycastForward)) {

			if (hit.collider.gameObject.CompareTag("Unpassable")) {

				float angleBetweenForwardAndDown = Vector3.Dot(forward, Vector3.down);
				float angleBetweenGroundAndLeft = Vector3.Dot(hit.normal, Vector3.left);

				/* If spaceship is ramming directly into the ground. */
				if (Mathf.Abs(angleBetweenForwardAndDown) <= 1 && Mathf.Abs(angleBetweenGroundAndLeft) <= Mathf.Epsilon) {
					adjustedForward.y = 0.0f;
				}
				/* Otherwise, spaceship is ramming into arbitrary other terrain, eg. wall or incline. */
				else {
					adjustedForward = Vector3.Reflect(adjustedForward, hit.normal);
				
					this.rigidbody.MoveRotation(
						Quaternion.Slerp (
							this.transform.localRotation,
							Quaternion.Euler(adjustedForward),
							Time.deltaTime
						)
					);
				}
			}
		}

		/* Boost forward. */
		rigidbody.MovePosition(
			rigidbody.position + Vector3.Slerp(Vector3.zero, adjustedForward*Time.deltaTime*currentVelocity, currentVelocity/maxVelocity)
		);

		/* Additional boost up/down. */
		rigidbody.MovePosition(
			rigidbody.position + Vector3.Slerp(Vector3.zero, Vector3.up*yTilt*Time.deltaTime*extraAccelerationThisFrameY, currentVelocity/maxVelocity)
		);


	}
	
	
	


	
	void HandleTilt() {
		

		Vector3 newDirection = Vector3.Slerp(
			spaceshipModel.transform.forward, 
			crosshairs.transform.position-spaceshipModel.transform.position, 
			Time.deltaTime*lookSpeed
		);

		/* Handle drifting tilt. */
		if (drifting) {
			currentDriftTilt = Mathf.Lerp(currentDriftTilt, -xTilt*driftTiltMax, driftTiltRate*Time.deltaTime);
		}
		else if (xTilt != 0) {
			currentDriftTilt = Mathf.Lerp(currentDriftTilt, -xTilt*normalTiltMax, normalTiltRate*Time.deltaTime);
		}
		else {
			currentDriftTilt = Mathf.Lerp(currentDriftTilt, 0.0f, driftAlignRate*Time.deltaTime);
		}


		spaceshipModel.transform.forward = newDirection;

		spaceshipModel.transform.localRotation = Quaternion.Euler(
			spaceshipModel.transform.localRotation.eulerAngles + new Vector3(0.0f, 0.0f, currentDriftTilt)
		);
		

	}

	
	
	
	
	
	void HandleRotation() {

		if (xTilt != 0) {
			
			float turningRateForThisFrame = normalTurningRate;
			
			/* Allow drift turning if player is holding down brake. */
			if (drifting) {
				turningRateForThisFrame = driftingTurningRate;
			}
						
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






//	void HandleFalling() {
//
//		if (enforceHeightLimit && spaceship.transform.position.y > maxHeightBeforeFalling) {
//			rigidbody.MovePosition(
//				Vector3.Slerp(
//					rigidbody.position,
//					rigidbody.position + Vector3.down*Time.deltaTime*fallingRate,
//					Time.deltaTime
//				)
//			);
//		}
//	}
	
	
	
}