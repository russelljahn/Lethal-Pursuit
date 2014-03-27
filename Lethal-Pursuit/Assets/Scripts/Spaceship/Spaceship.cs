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
	public float boostAmount;
	public float brakeAmount;
	public bool  shooting;
	public bool  boosting;
	public bool  strafing;
	public bool  braking;
	public bool  drifting;
	public bool  nosediving;
	public bool  idle;
	[HideInInspector]
	public bool  debugSelfDestruct;
	#endregion

	public Vector3 forward;
	public float heightAboveGround;

	public bool enforceHeightLimit = true;
	public float heightLimit = 50.0f; // Height limit above the ground.

	public float currentVelocity;
	public float maxVelocity = 150.0f;

	public bool mapKeyboardDiagonalAmountToAnalogueDiagonalAmount = false;

	public bool debugSelfDestructEnabled = false;


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
		xTiltLeft = InputManager.ActiveDevice.RightStickX.Value;		
		yTiltLeft = InputManager.ActiveDevice.RightStickY.Value;
		boostAmount = yTiltLeft;
//		brakeAmount = ;
		shooting = InputManager.ActiveDevice.Action3.State;
//		debugSelfDestruct = debugSelfDestructEnabled && InputManager.ActiveDevice.Action4.State;
		debugSelfDestruct = false;
		
		Debug.Log ("debugSelfDestructEnabled: " + debugSelfDestructEnabled);
		Debug.Log ("InputManager.ActiveDevice.Action4.State: " + InputManager.ActiveDevice.Action4.State);
		Debug.Log ("debugSelfDestruct: " + debugSelfDestruct);

		braking = false;
		boosting = false;
		drifting = false;
		nosediving = false;
		idle = false;

		if (boostAmount > 0 && brakeAmount == 0) {
			boosting = true;
		}
		else if (brakeAmount > 0) {
			drifting = (xTiltLeft != 0);
			nosediving = (yTiltLeft != 0);
			braking = (xTiltLeft == 0 && yTiltLeft == 0);
		}
		else if (boostAmount == 0 && brakeAmount == 0) {
			idle = true;
		}
		
		/* Map keyboard diagonal axis amount to joystick diagonal axis amount. */
		if (mapKeyboardDiagonalAmountToAnalogueDiagonalAmount) {
			if (Mathf.Abs(xTiltLeft) > 0.5f && Mathf.Abs(yTiltLeft) > 0.5f) {
				xTiltLeft *= 0.5f;
				yTiltLeft *= 0.5f;
			}
		}
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
