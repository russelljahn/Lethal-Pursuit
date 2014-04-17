using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Spaceship))]
[RequireComponent (typeof (Collider))]
public class SpaceshipControl : SpaceshipComponent, ITargetable {
	
	
	public float boostAcceleration = 10.0f;
	public float deaccelerationIdle = 0.92f;
	public float deaccelerationBraking = 4f;

	public HudCrosshairs crosshairs;
	public float normalTurnSpeed = 100.0f;
	public float driftTurnSpeed = 200.0f;
	private float currentTurnSpeed;
	
	public float driftTiltRate = 4f;
	public float normalTiltRate = 2.5f;
	public float tiltAlignRate = 2f;
	public float driftTiltMax = 70f;
	public float normalTiltMax = 35f;
	private float currentTilt;

	public float distanceForwardToCheckForCollision = 1000;
	
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	
	public override void Update () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			base.Update();
		}
	}


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
		if (boosting) {
			currentBoostVelocity = Mathf.Lerp(currentBoostVelocity, maxBoostVelocity, boostAcceleration*Time.deltaTime);
		}
		else if (braking) {
			currentBoostVelocity = Mathf.Lerp(currentBoostVelocity, 0, deaccelerationBraking*Time.deltaTime);
		}
		else if (idle) {
			currentBoostVelocity = Mathf.Lerp(currentBoostVelocity, 0, deaccelerationIdle*Time.deltaTime);
		} 
		currentBoostVelocity = Mathf.Clamp(currentBoostVelocity, 0, maxBoostVelocity);

		Vector3 adjustedForward = forward;
		
		/* Do some collision detection. If you're about to hit the environment, then adjust for it. */
		RaycastHit hit;
		
		if (Physics.Raycast(transform.position, forward, out hit, distanceForwardToCheckForCollision)) {
			
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
			rigidbody.position + Vector3.Slerp(
				Vector3.zero, 
				forward*currentBoostVelocity, 
				Time.deltaTime
			)
		);
		
	}

	
	void HandleTilt() {
		/* Handle drifting tilt. */
		if (drifting) {
			currentTilt = Mathf.Lerp(currentTilt, -xTiltLeftStick*driftTiltMax, driftTiltRate*Time.deltaTime);
		}
		else if (xTiltLeftStick != 0.0f) {
			currentTilt = Mathf.Lerp(currentTilt, -xTiltLeftStick*normalTiltMax, driftTiltRate*Time.deltaTime);
		}
		else {
			currentTilt = Mathf.Lerp(currentTilt, 0.0f, tiltAlignRate*Time.deltaTime);
		}
		spaceshipModel.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, currentTilt));
	}
	
	
	void HandleRotation() {

		if (drifting) {
			currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, driftTurnSpeed, Time.deltaTime*driftTurnSpeed);
		}
		else {
			currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, normalTurnSpeed, Time.deltaTime*driftTurnSpeed);
		}

		/* Left/right look rotation. */
		spaceshipModelRoot.transform.localRotation = Quaternion.Euler(
			spaceshipModelRoot.transform.localRotation.eulerAngles + new Vector3(0.0f, xTiltLeftStick*Time.deltaTime*currentTurnSpeed, 0.0f)
		);

		/* Up/down look rotation. */
		Vector3 newRotationX = spaceshipModelRoot.transform.localRotation.eulerAngles + new Vector3(-yTiltLeftStick*Time.deltaTime*currentTurnSpeed, 0.0f, 0.0f);
		if (newRotationX.x < 180 && newRotationX.x >= 0) {
			newRotationX.x = Mathf.Clamp(newRotationX.x, 0f, 85f);
		}
		else if (newRotationX.x < 0f) {
			newRotationX.x = Mathf.Clamp(newRotationX.x, -85f, 0f);
		}
		else {
			newRotationX.x = Mathf.Clamp(newRotationX.x, 275f, 360f);
		}
		spaceshipModelRoot.transform.localRotation = Quaternion.Euler(newRotationX);
	}

	
}

