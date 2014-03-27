using UnityEngine;
using InControl;
using System.Collections;

/* 
	Contains data for a spaceship that other spaceship components may use.
 */
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class Spaceship : MonoBehaviour {

	public GameplayManager gameplayManager;
	public GameObject spaceshipModel;
	public Camera spaceshipCamera;
	public GameObject crosshairs;

	#region input variables
	public float xTiltLeft; /* Tilt of left analogue stick every frame. */
	public float yTiltLeft; /* Tilt of left analogue stick every frame. */
	public float xTiltRight; /* Tilt of analogue stick every frame. */
	public float yTiltRight; /* Tilt of analogue stick every frame. */
	public float strafeAmount;
	public float boostAmount;
	public float brakeAmount;
	public bool  shooting;
	public bool  boosting;
	public bool  strafing;
	public bool  braking;
	public bool  reversing;
	public bool  drifting;
	public bool  nosediving;
	public bool  idle;
	[HideInInspector]
	public bool  debugSelfDestruct;
	#endregion

	public Vector3 forward;
	public Vector3 right;
	public float heightAboveGround;

	public bool enforceHeightLimit = true;
	public float heightLimit = 50.0f; // Height limit above the ground.

	public float currentBoostVelocity;
	public float maxBoostVelocity = 150.0f;
	public float currentStrafeVelocity;
	public float maxStrafeVelocity = 150.0f;


	void Start () {
		gameplayManager = GameplayManager.instance;
	}

	void Awake() {
		if (!NetworkManager.IsSinglePlayer() && !networkView.isMine) {
			spaceshipCamera.gameObject.SetActive(false);
//			crosshairs.SetActive(false);
		}
	}

	
	void FixedUpdate () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			HandleInput();
			HandleHeightCheck();
		}
	}



	void Update () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			forward = spaceshipModel.transform.forward;
			right = spaceshipModel.transform.right;
		}
		else {
			SyncMovement();
		}
	}
	
	private void SyncMovement()
	{
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime/syncDelay);
		transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime/syncDelay);
	}


	void HandleInput() {
		xTiltLeft = InputManager.ActiveDevice.LeftStickX.Value;		
		yTiltLeft = InputManager.ActiveDevice.LeftStickY.Value;
		xTiltRight = InputManager.ActiveDevice.RightStickX.Value;		
		yTiltRight = InputManager.ActiveDevice.RightStickY.Value;
		boostAmount = yTiltLeft;
		strafeAmount = xTiltLeft;
		shooting = InputManager.ActiveDevice.RightTrigger.IsPressed;
		strafing = strafeAmount != 0;

		boosting = false;
		braking = false;
		reversing = false;
		drifting = false;
		nosediving = false;
		idle = false;


		if (boostAmount > 0) {
			boosting = true;
		}
		else if (boostAmount < 0) {
//			drifting = (xTiltLeft != 0);
//			nosediving = (yTiltLeft != 0);
//			braking = true;
			reversing = true;
		}

		idle = !boosting || !reversing || !strafing;

	}


	void HandleHeightCheck() {
		RaycastHit hit;
		Physics.Raycast(this.transform.position, -this.transform.up, out hit);
		heightAboveGround = hit.distance;
	}


	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		bool isShooting = false;
		
		if (stream.isWriting) {
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			isShooting = shooting;
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref isShooting);
		}
		else {
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref isShooting);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
			
			syncStartRotation = transform.rotation;
			syncEndRotation = syncRotation;
			
			shooting = isShooting;
		}
	}

	





















	
}
