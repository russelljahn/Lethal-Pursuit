using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PickupPunkMissiles : Pickup {
//
//	public PlaygroundParticles laserBeamEffect;
//	public PlaygroundPresetLaser laserBeamEffectScript;

	
	private GameObject hitGameObject;
	
	public int numberOfShots = 5;
	public float cooldownBetweenShots = 0.1f;
	private float timeUntilCanShoot = 0.0f;

	public string bulletResourcePath = "Bullets/PunkMissileBullet";

	private GameObject cachedBullet;
	public AudioClip shootingSound;
	
	public bool enabled = true;
	


	void FixedUpdate() {
		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);
		
		if (!active) {
			return;
		}
		
		if (spaceship.shooting && timeUntilCanShoot == 0.0f && numberOfShots > 0) {
//			SpawnMissile();
			timeUntilCanShoot = cooldownBetweenShots;
			--numberOfShots;
		}
	}


	public void SpawnMissile() {
		GameObject bulletGameObject;
		if (NetworkManager.IsSinglePlayer()) {
			bulletGameObject = GameObject.Instantiate(
				cachedBullet,
				this.transform.position, 
				spaceship.spaceshipModel.transform.rotation
			) as GameObject;
		}
		else {
			bulletGameObject = Network.Instantiate(
				cachedBullet,
				this.transform.position, 
				spaceship.spaceshipModel.transform.rotation,
				777
			) as GameObject;
		}
		
		Bullet bullet = bulletGameObject.GetComponent<Bullet>();
		bullet.direction = spaceship.spaceshipModel.transform.forward;
		bullet.sourceSpaceship = spaceship;
		
		bulletGameObject.SetActive(true);
		this.audio.PlayOneShot(shootingSound);
	}


	public override bool IsEquippable() {
		return true;
	}


	public override bool ShouldDrop() {
		return numberOfShots <= 0;
	}


	public override void OnPickup (Spaceship spaceship) {
		base.OnPickup (spaceship);
		active = true;
	}


	public override void OnDrop() {

	}
	
}
