using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipGun : SpaceshipComponent {

	public Bullet bulletToUse;

	public float bufferTimeBetweenShots = 0.1f;
	public float timeUntilCanShoot = 0.0f;

	public float forceFactor = 1.0f;





	// Use this for initialization
	public override void Start () {
		base.Start();
	}





	// Update is called once per frame
	void Update () {
		
	}





	void FixedUpdate() {

		if (currentlyShooting && timeUntilCanShoot == 0.0f) {
			Debug.Log ("SpaceshipGun.transform.position: " + this.transform.position);
			Bullet bullet = GameObject.Instantiate(bulletToUse, this.transform.position, Quaternion.identity) as Bullet;
			bullet.direction = spaceshipModel.transform.forward;
			bullet.prefab = false;
		}

		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);

	}




}
