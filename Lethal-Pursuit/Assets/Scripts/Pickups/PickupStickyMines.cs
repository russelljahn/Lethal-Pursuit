using UnityEngine;
using System.Collections;

public class PickupStickyMines : Pickup {

	private GameObject hitGameObject;
	
	public int maxShots = 5;
	public int  currentShots;
	public float cooldownBetweenShots = 0.3f;
	private float timeUntilCanShoot = 0.0f;

	public string mineResourcePath = "Bullets/StickyMineBullet";
	public AudioClip shootingSound;

	public Vector3 spawnOffsetFromGun = new Vector3(0.0f, 0.0f, -50.0f);
	public new bool enabled = true;
	

	void FixedUpdate() {

		if (!active) {
			return;
		}

		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);
		
		if (spaceship.shooting && timeUntilCanShoot == 0.0f && currentShots > 0) {
			SpawnStickyMine();
			timeUntilCanShoot = cooldownBetweenShots;
			--currentShots;
		}
	}


	public void SpawnStickyMine() {
		GameObject mineGameObject;
		if (NetworkManager.IsSinglePlayer()) {
			mineGameObject = GameObject.Instantiate(
				Resources.Load(mineResourcePath),
				spaceship.gun.transform.position + spaceship.spaceshipMesh.transform.TransformDirection(spawnOffsetFromGun),
				spaceship.gun.transform.rotation
			) as GameObject;
		}
		else if (spaceship.networkView.isMine) {
			mineGameObject = Network.Instantiate(
				Resources.Load(mineResourcePath),
				spaceship.gun.transform.position + spaceship.spaceshipMesh.transform.TransformDirection(spawnOffsetFromGun),
				spaceship.gun.transform.rotation,
				668
			) as GameObject;
		}
		else {
			spaceship.gun.audio.PlayOneShot(shootingSound);
			return;
		}
		
		StickyMine stickyMine = mineGameObject.GetComponent<StickyMine>();
		stickyMine.sourceSpaceship = spaceship;
		
		mineGameObject.SetActive(true);
		spaceship.gun.audio.PlayOneShot(shootingSound);
	}


	public override bool IsEquippable() {
		return true;
	}


	public override bool ShouldDrop() {
		return currentShots <= 0;
	}


	public override void OnPickup (Spaceship spaceship) {
		base.OnPickup (spaceship);
		currentShots = maxShots;
		active = true;
	}


	public override void OnDrop() {

	}
	
}
