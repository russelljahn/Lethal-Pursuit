using UnityEngine;
using System.Collections;

public class PickupPunkMissiles : Pickup {
//
//	public PlaygroundParticles laserBeamEffect;
//	public PlaygroundPresetLaser laserBeamEffectScript;

	
	private GameObject hitGameObject;
	
	public int numberOfShots = 5;
	public float cooldownBetweenShots = 0.25f;
	private float timeUntilCanShoot = 0.0f;

	public string missileResourcePath = "Bullets/PunkMissileBullet";

//	private GameObject cachedBullet;
	public AudioClip shootingSound;

	public Vector3 spawnOffsetFromGun = new Vector3(0.0f, 0.0f, 10.0f);
	public bool enabled = true;
	


	void FixedUpdate() {
		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);
		
		if (!active) {
			return;
		}
		
		if (spaceship.shooting && timeUntilCanShoot == 0.0f && numberOfShots > 0) {
			SpawnMissile();
			timeUntilCanShoot = cooldownBetweenShots;
			--numberOfShots;
		}
	}


	public void SpawnMissile() {
		GameObject missileGameObject;
		if (NetworkManager.IsSinglePlayer()) {
			missileGameObject = GameObject.Instantiate(
				Resources.Load(missileResourcePath),
				spaceship.gun.transform.position + spaceship.gun.transform.TransformDirection(spawnOffsetFromGun), 
				spaceship.gun.transform.rotation
			) as GameObject;
		}
		else {
			missileGameObject = Network.Instantiate(
				Resources.Load(missileResourcePath),
				spaceship.gun.transform.position + spaceship.gun.transform.TransformDirection(spawnOffsetFromGun), 
				spaceship.gun.transform.rotation,
				667
			) as GameObject;
		}
		
		Missile missile = missileGameObject.GetComponent<Missile>();
		missile.direction = spaceship.spaceshipModel.transform.forward;
		missile.sourceSpaceship = spaceship;
		
		missileGameObject.SetActive(true);
		spaceship.gun.audio.PlayOneShot(shootingSound);
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
