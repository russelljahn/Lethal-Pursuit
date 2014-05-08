using UnityEngine;
using InControl;
using System.Collections;


public enum EquipType {
	DEFAULT_WEAPON,
	SUB_WEAPON,
	//ITEM
}


/* 
	Contains input and misc spaceship data that's updated every frame for spaceship components to use.
 */
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class Spaceship : MonoBehaviour, ITargetable {
	
	public GameplayManager gameplayManager;
	public GameObject spaceshipModelPitchYaw;
	public GameObject spaceshipModelRoll;
	public GameObject spaceshipMesh;
	public GameObject spaceshipShell;
	public Camera spaceshipCamera;
	public GameObject crosshairs;
	public SpaceshipGun gun;
	public SpaceshipPickups pickups;
	public SpaceshipControl controls;
	public new string name;

	private float invertYAmount = 1.0f;

	public GameObject [] effects;

	#region input variables
	public float xTiltLeftStick; /* Tilt of left analogue stick every frame. */
	public float yTiltLeftStick; /* Tilt of left analogue stick every frame. */
	public float xTiltRightStick; /* Tilt of analogue stick every frame. */
	public float yTiltRightStick; /* Tilt of analogue stick every frame. */
	public bool  shooting;
	public bool  boosting;
	public bool  braking;
	public bool  drifting;
	public bool  idle;
	public bool  swappingWeapon;
	public EquipType equippedItem;
	#endregion

	public bool isVisible = true;
	public bool controlsEnabled = true;

	public bool selectPressedLastFrame;
	
	public Vector3 forward;
	public Vector3 right;
	
	public float currentBoostVelocity;
	public float maxBoostVelocity = 150.0f;

	public int ownerPlayerID; //NetworkManager.GetPlayerIndex(spaceshipMesh.networkView.owner.ipAddress);
	
	
	void Start () {
		gameplayManager = GameplayManager.instance;
	}


	void Awake() {
		if (!NetworkManager.IsSinglePlayer() && !networkView.isMine) {
			spaceshipCamera.gameObject.SetActive(false);
			crosshairs.SetActive(false);
		}
		ownerPlayerID = NetworkManager.GetPlayerIndex(this.networkView.owner.ipAddress);
	}
	
	
	void FixedUpdate () {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			HandleInput();
			if (selectPressedLastFrame && !InputManager.ActiveDevice.GetControl(InputControlType.Select).IsPressed) {
				Debug.Log ("Flipping Y-Axis Invert...");
				GameplayManager.invertYAxis = !GameplayManager.invertYAxis;
			}
			selectPressedLastFrame = InputManager.ActiveDevice.GetControl(InputControlType.Select).IsPressed;
		}

	}
	
	
	void Update () {
		// Update visibility
		spaceshipMesh.renderer.enabled = isVisible;
		collider.enabled = isVisible;
		for (int i = 0; i < effects.Length; ++i) {
			effects[i].SetActive(isVisible);
		}
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			crosshairs.gameObject.SetActive(isVisible);
		}

		// Update if controls are currently disabled/enabled
		controls.enabled = controlsEnabled;
		shooting = shooting && controlsEnabled;
			
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			forward = spaceshipModelRoll.transform.forward;
			right = spaceshipModelRoll.transform.right;
		}
		else {
			SyncMovement();
		}

	}


	void HandleInput() {
		if (GameplayManager.invertYAxis) {
			invertYAmount = 1.0f;
		}
		else {
			invertYAmount = -1.0f;
		}

		xTiltLeftStick = InputManager.ActiveDevice.LeftStickX.Value;
		yTiltLeftStick = InputManager.ActiveDevice.LeftStickY.Value * invertYAmount;
		xTiltRightStick = InputManager.ActiveDevice.RightStickX.Value;
		yTiltRightStick = InputManager.ActiveDevice.RightStickY.Value;
		shooting = InputManager.ActiveDevice.RightTrigger.IsPressed && controlsEnabled;
		drifting = InputManager.ActiveDevice.LeftTrigger.IsPressed;
		braking = InputManager.ActiveDevice.Action3.IsPressed;
		boosting = !braking && InputManager.ActiveDevice.Action1.IsPressed;
		idle = !boosting && !braking;
		swappingWeapon = InputManager.ActiveDevice.LeftBumper.WasReleased || InputManager.ActiveDevice.RightBumper.WasReleased;
	}


	public void EnableGun() {
		gun.enabled = true;
	}


	public void DisableGun() {
		gun.enabled = false;
	}
	

	/* 
		Network syncing stuff.
	 */
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;


	private void SyncMovement() {
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime/syncDelay);
		transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime/syncDelay);
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		bool isShooting = false;
		bool isSwappingWeapon = false;
		bool equippedDefaultGun = false;
		bool serializedControlsEnabled = false;
		bool serializedIsVisible = false;

		int ownerID = -1;

		if (stream.isWriting) {
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			isShooting = shooting;
			isSwappingWeapon = swappingWeapon;
			equippedDefaultGun = (equippedItem == EquipType.DEFAULT_WEAPON);
			serializedIsVisible = isVisible;
			serializedControlsEnabled = controlsEnabled;

			if (Network.isServer) {
				ownerID = ownerPlayerID;
			}
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref isShooting);
			stream.Serialize(ref isSwappingWeapon);
			stream.Serialize(ref equippedDefaultGun);
			stream.Serialize(ref serializedIsVisible);
			stream.Serialize(ref serializedControlsEnabled);
			
			if (Network.isServer) {
				Debug.Log("Serializing ownerID to all instances");
				stream.Serialize(ref ownerID);
			}
		}
		else {
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref isShooting);
			stream.Serialize(ref isSwappingWeapon);
			stream.Serialize(ref equippedDefaultGun);
			stream.Serialize(ref serializedIsVisible);
			stream.Serialize(ref serializedControlsEnabled);

			if (Network.isClient) {
				stream.Serialize(ref ownerID);
			}

			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
			
			syncStartRotation = transform.rotation;
			syncEndRotation = syncRotation;
			
			shooting = isShooting;
			swappingWeapon = isSwappingWeapon;
			if (equippedDefaultGun) {
				equippedItem = EquipType.DEFAULT_WEAPON;
			}
			else {
				equippedItem = EquipType.SUB_WEAPON;
			}

			isVisible = serializedIsVisible;
			controlsEnabled = serializedControlsEnabled;

			if (Network.isClient) {
				ownerPlayerID = ownerID;
			}
		}
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}