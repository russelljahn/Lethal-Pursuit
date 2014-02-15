using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipGun : SpaceshipComponent {

	public string bulletResourcePath = "Bullets/TestLaz0r";

	public float cooldownBetweenShots = 0.1f;
	private float timeUntilCanShoot = 0.0f;

	public float forceFactor = 1.0f;





	// Use this for initialization
	public override void Start () {
		base.Start();
	}





	// Update is called once per frame
	public override void Update () {
		base.Update();	
	}





	void FixedUpdate() {

		if (shooting && timeUntilCanShoot == 0.0f) {

			GameObject bulletGameObject = GameObject.Instantiate(
				Resources.Load(bulletResourcePath, typeof(GameObject)), 
				this.transform.position, 
				Quaternion.identity
			) as GameObject;

			Bullet bullet = bulletGameObject.GetComponent<Bullet>();
			bullet.direction = spaceshipModel.transform.forward;
			bullet.speed = 1.25f*spaceship.maxVelocity;
			bullet.sourceSpaceship = spaceship;

			timeUntilCanShoot = cooldownBetweenShots;
		}
		else {
			timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);
		}
	}




}
