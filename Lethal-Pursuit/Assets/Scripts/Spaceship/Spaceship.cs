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

	#region input variables
	public float xTilt; /* Tilt of analogue stick every frame. */
	public float yTilt; /* Tilt of analogue stick every frame. */
	public float boostAmount;
	public float brakeAmount;
	public bool shooting;
	public bool boosting;
	public bool braking;
	public bool drifting;
	public bool nosediving;
	public bool idle;
	#endregion

	public Vector3 forward;
	public float heightAboveGround;

	public bool enforceHeightLimit = true;
	public float heightLimit = 300.0f;
	public float fractionOfHeightLimitToBeginSputtering = 0.8f;
	public float maxHeightBeforeFalling = 500.0f;
	public float fallingRate = -98.1f;

	public float currentVelocity;
	public float maxVelocity = 150.0f;
	




	void Start () {
		gameplayManager = GameplayManager.instance;
	}


	
	void FixedUpdate () {
		HandleInput();
		HandleHeightCheck();
	}



	void Update () {
		forward = spaceshipModel.transform.forward;
	}



	void HandleInput() {
		xTilt = InputManager.ActiveDevice.LeftStickX.Value;		
		yTilt = InputManager.ActiveDevice.LeftStickY.Value;
		boostAmount = InputManager.ActiveDevice.RightTrigger.Value;
		brakeAmount = InputManager.ActiveDevice.LeftTrigger.Value;
		shooting = InputManager.ActiveDevice.Action3.State;

		braking = false;
		boosting = false;
		drifting = false;
		nosediving = false;

		if (boostAmount > 0 && brakeAmount == 0) {
			boosting = true;
		}
		else if (brakeAmount > 0) {
			drifting = (xTilt != 0);
			nosediving = (yTilt != 0);
			braking = (xTilt == 0 && yTilt == 0);
		}
		else if (boostAmount == 0 && brakeAmount == 0) {
			idle = true;
		}
		
		
		/* Map keyboard diagonal axis amount to joystick diagonal axis amount. */
		if (Mathf.Abs(xTilt) > 0.5f && Mathf.Abs(yTilt) > 0.5f) {
			xTilt *= 0.5f;
			yTilt *= 0.5f;
		}
	}



	void HandleHeightCheck() {
		RaycastHit hit;
		Physics.Raycast(this.transform.position, Vector3.down, out hit);
		heightAboveGround = hit.distance;
	}


//	void OnCollisionEnter(Collision walls) { /* hitting walls tagged as unpassable does 50 damage */
//		
//		if (walls.gameObject.tag == "Unpassable" && stall == 0) {
//			
//			DecreaseHealth ();
//			
//		}
//	}
//	
//	void DecreaseHealth (){
//		
//		health -= damageHalfHit;
//		stall = 1;
//		/* yield WaitForSeconds (damageBufferTime);  damage buffer code, not working atm*/
//		stall = 0;
//	
//	}
	
	
}
