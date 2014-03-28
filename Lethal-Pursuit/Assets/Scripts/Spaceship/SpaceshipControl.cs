using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Spaceship))]
[RequireComponent (typeof (Collider))]
public class SpaceshipControl : SpaceshipComponent {
	
	
	public float boostAcceleration = 10.0f;
	public float strafeAcceleration = 5.0f; 
	
	public float normalExtraAccelerationY = 5.0f; 
	public float nosedivingExtraAccelerationY = 30.0f; 
	
	
	public float deaccelerationBrake = 500;
	public float deaccelerationDrift = 50;
	public float deaccelerationIdle = 500;
	
	
	public float xTiltLeftSpeed = 1.5f;
	public float yTiltLeftSpeed = 3.3f;
	
	
	public float normalTurningRate = 115.0f;
	public float driftingTurningRate = 300.0f;
	public float normalPitchRate = 300.0f;
	public float nosedivingPitchRate = 300.0f;
	
	public float timeUntilMaxTurning = 2.6f;
	private float timeSinceStartedTurning = 0.0f;
	
	public float timeUntilMaxPitch = 2.6f;
	private float timeSinceStartedPitch = 0.0f;
	
	public HudCrosshairs crosshairs;
	public float lookSpeed = 1.0f;
	
	public float driftTiltMax = 90f;
	private float currentDriftTilt = 0f;
	public float driftTiltRate = 2.5f;
	public float driftAlignRate = 1.5f;
	
	public float nosediveTiltMax = 90f;
	private float currentNosediveTilt = 0f;
	public float nosediveTiltRate = 2.5f;
	public float nosediveAlignRate = 1.5f;
	
	public float distanceToRaycastForward = 1000;
	
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	
	
	
	
	public override void Update () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			base.Update();
		}
	}
	
	
	
	
	// This happens at a fixed timestep
	void FixedUpdate () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			HandleRotation();
			HandleMovement();
			HandleTilt();
		}
	}
	
	
	
	
	
	
	void HandleMovement() {
		
		spaceship.rigidbody.velocity = Vector3.zero;
		
		/* Adjust velocities based on current spaceship behavior. */
		if (strafing) {
			currentStrafeVelocity += strafeAcceleration;
		}
		else {
			currentStrafeVelocity = Mathf.Lerp(currentStrafeVelocity, 0, currentStrafeVelocity*Time.deltaTime);
		}
		if (boosting) {
			currentBoostVelocity += boostAcceleration;
		}
		else if (reversing) {
			currentBoostVelocity -= boostAcceleration;
		}
		else if (idle) {
			currentBoostVelocity = Mathf.Lerp(currentBoostVelocity, 0, deaccelerationBrake*Time.deltaTime);
		} 
		
		currentBoostVelocity = Mathf.Clamp(currentBoostVelocity, -maxBoostVelocity, maxBoostVelocity);
		currentStrafeVelocity = Mathf.Clamp(currentStrafeVelocity, -maxStrafeVelocity, maxStrafeVelocity);
		
		
		Vector3 adjustedForward = forward;
		
		/* Do some collision detection. If you're about to hit the environment, then adjust for it. */
		RaycastHit hit;
		
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
		//		rigidbody.MovePosition(
		//			rigidbody.position + Vector3.Slerp(Vector3.zero, adjustedForward*Time.deltaTime*currentVelocity, Mathf.Abs(currentVelocity)/maxVelocity)
		//		);
		rigidbody.MovePosition(
			rigidbody.position + Vector3.Slerp(Vector3.zero, forward*Mathf.Abs(boostAmount)*currentBoostVelocity, Time.deltaTime)
			);
		
		//		rigidbody.MovePosition(
		//			rigidbody.position + Vector3.Slerp(Vector3.zero, right*strafeAmount*currentStrafeVelocity, Time.deltaTime)	
		//		);
		
		/* Strafe left and right. */
		
		/* Additional boost up/down. */
		//		rigidbody.MovePosition(
		//			rigidbody.position + Vector3.Slerp(Vector3.zero, right*Time.deltaTime*currentVelocity, Mathf.Abs(currentVelocity)/maxVelocity)
		//		);;
		
		
	}
	
	
	
	
	
	
	void HandleTilt() {
		//		return;
		
		//		float pitchRateThisFrame = normalPitchRate;
		
		//		if (nosediving) {
		//			pitchRateThisFrame = nosedivingPitchRate;
		//		}
		
		/* Handle drifting tilt. */
		if (xTiltLeft != 0.0f) {
			currentDriftTilt = Mathf.Lerp(currentDriftTilt, -xTiltLeft*driftTiltMax, driftTiltRate*Time.deltaTime);
		}
		else {
			currentDriftTilt = Mathf.Lerp(currentDriftTilt, 0.0f, driftAlignRate*Time.deltaTime);
		}
		/* Handle nosediving tilt. */
		//		if (yTiltLeft != 0.0f) {
		//			currentNosediveTilt = Mathf.Lerp(currentNosediveTilt, -yTiltLeft*nosediveTiltMax, nosediveTiltRate*Time.deltaTime);
		//		}
		//		else {
		//			currentNosediveTilt = Mathf.Lerp(currentNosediveTilt, 0.0f, nosediveAlignRate*Time.deltaTime);
		//		}
		
		
		//		spaceshipModel.transform.RotateAround(spaceshipModel.transform.position, Vector3.up, xTiltLeft*Time.deltaTime*xTiltLeftSpeed);
		//		spaceship.transform.RotateAround(spaceshipModel.transform.position, Vector3.right, yTiltLeft*Time.deltaTime*yTiltLeftSpeed);
		
		
		//		spaceshipModel.transform.localRotation = Quaternion.Euler(
		//			new Vector3(0.0f, 0.0f, currentDriftTilt)
		//		);
		
		
	}
	
	
	
	
	
	
	void HandleRotation() {
		
		//		Vector3 v = new Vector3(xTiltLeft, xTiltRight, 0.0f);
		
		//		spaceshipModel.transform.forward = Vector3.RotateTowards(spaceshipModel.transform.forward, spaceshipModel.transform.InverseTransformPoint(v), lookSpeed*Time.deltaTime, lookSpeed*Time.deltaTime);
		//		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Clamp01(lookSpeed*Time.deltaTime));
		
		if (xTiltLeft != 0.0f || xTiltRight != 0.0f) {
			timeSinceStartedTurning += Time.deltaTime;
		}
		else {
			timeSinceStartedTurning = 0.0f;
		}
		
		spaceshipModel.transform.localRotation = Quaternion.Euler(
			spaceshipModel.transform.localRotation.eulerAngles + new Vector3(0.0f, xTiltLeft*Time.deltaTime*lookSpeed, 0.0f)
			);
		
		Vector3 newRotationX = spaceshipModel.transform.localRotation.eulerAngles + new Vector3(-yTiltLeft*Time.deltaTime*lookSpeed, 0.0f, 0.0f);
		//		Debug.Log ("newRotationX.x: " + newRotationX.x);
		
		if (newRotationX.x < 180 && newRotationX.x >= 0) {
			newRotationX.x = Mathf.Clamp(newRotationX.x, 0f, 85f);
		}
		else if (newRotationX.x < 0f) {
			newRotationX.x = Mathf.Clamp(newRotationX.x, -85f, 0f);
		}
		else {
			newRotationX.x = Mathf.Clamp(newRotationX.x, 275f, 360f);
		}
		
		spaceshipModel.transform.localRotation = Quaternion.Euler(
			newRotationX
			);
		
		//		this.transform.RotateAround(this.transform.position, Vector3.right, yTiltRight*Time.deltaTime*lookSpeed);
		
		return;
		if (xTiltLeft != 0) {
			float turningRateForThisFrame = normalTurningRate;
			
			/* Allow drift turning if player is holding down brake. */
			if (drifting) {
				turningRateForThisFrame = driftingTurningRate;
			}
			
			this.rigidbody.MoveRotation(
				Quaternion.Slerp (
				this.transform.localRotation,
				Quaternion.Euler(this.transform.localRotation.eulerAngles + Vector3.up*xTiltLeft*turningRateForThisFrame*Time.deltaTime),
				Mathf.Clamp01(timeSinceStartedTurning/timeUntilMaxTurning)
				)
				);
			timeSinceStartedTurning += Time.deltaTime;
		}
		else {
			timeSinceStartedTurning = 0.0f;
		}
		
		
		//		if (yTiltLeft != 0) {
		//			float pitchRateForThisFrame = normalPitchRate;
		//			
		//			/* Allow drift turning if player is holding down brake. */
		//			if (nosediving) {
		//				pitchRateForThisFrame = nosedivingPitchRate;
		//			}
		//			
		//			this.rigidbody.MoveRotation(
		//				Quaternion.Slerp (
		//					this.transform.localRotation,
		//					Quaternion.Euler(this.transform.localRotation.eulerAngles + Vector3.left*yTiltLeft*pitchRateForThisFrame*Time.deltaTime),
		//					Mathf.Clamp01(timeSinceStartedPitch/timeUntilMaxPitch)
		//				)
		//			);
		//			timeSinceStartedPitch += Time.deltaTime;
		//		}
		//		else {
		//			timeSinceStartedPitch = 0.0f;
		//		}
		
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