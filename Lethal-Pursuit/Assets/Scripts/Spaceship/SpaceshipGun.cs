using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipGun : MonoBehaviour {


	private bool currentlyShooting;

	public SpaceshipControl spaceship;
	public Bullet bulletToUse;

	public float bufferTimeBetweenShots = 0.1f;
	public float timeUntilCanShoot = 0.0f;

	public float forceFactor = 1.0f;





	// Use this for initialization
	void Start () {
	
	}





	// Update is called once per frame
	void Update () {
		
	}





	void FixedUpdate() {
		currentlyShooting = InputManager.ActiveDevice.Action3.State;

		if (currentlyShooting && timeUntilCanShoot == 0.0f) {
			Debug.Log ("SpaceshipGun.transform.position: " + this.transform.position);
			Bullet bullet = GameObject.Instantiate(bulletToUse, this.transform.position, Quaternion.identity) as Bullet;
			bullet.direction = spaceship.spaceshipModel.transform.forward;
			bullet.prefab = false;
		}

		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);

	}




}
