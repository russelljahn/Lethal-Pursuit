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
	public bool currentlyShooting;
	#endregion

	public float heightAboveGround;

	public float heightLimit = 300.0f;
	public float fractionOfHeightLimitToBeginSputtering = 0.8f;
	public float maxHeightBeforeFalling = 500.0f;
	public float fallingRate = -98.1f;
	

//	public static float health = 100; /*health, damage, & status */
//	private float maxHealth = 100;
//	private float damageHalfHit = 50;
//	private float damageOneHitKO = 100;
//	private float damageBufferTime = 2;
//	private float stall = 0;



	// Use this for initialization
	void Start () {
		gameplayManager = GameplayManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput();
		HandleHeightCheck();
	}



	void HandleInput() {
		xTilt = InputManager.ActiveDevice.LeftStickX.Value;		
		yTilt = InputManager.ActiveDevice.LeftStickY.Value;
		boostAmount = InputManager.ActiveDevice.RightTrigger.Value;
		brakeAmount = InputManager.ActiveDevice.LeftTrigger.Value;
		currentlyShooting = InputManager.ActiveDevice.Action3.State;

		
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
