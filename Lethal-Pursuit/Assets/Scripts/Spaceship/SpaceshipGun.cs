using UnityEngine;
using InControl;
using System.Collections;

public enum EnergyState {
	FULL,
	DRAINED,
	CRITICAL
}

[RequireComponent(typeof(AudioSource))]
public class SpaceshipGun : SpaceshipComponent {

	public float maxEnergy = 100f;
	public float currentEnergy;

	public float energyPerShot = 10.0f;
	public float energyRechargeRate = 5.0f;
	public float energyRechargeMultiplierWhenNotShooting = 1.3f;
	public float cooldownBetweenShots = 0.1f;
	private float timeUntilCanShoot = 0.0f;
	
	public string bulletResourcePath = "Bullets/HexLaserBullet";

	public EnergyState state = EnergyState.FULL;
	public float energyRatioToBeDrained = 0.60f;
	public float energyRatioToBeCritical = 0.30f;

	private GameObject cachedBullet;
	public AudioClip shootingSound;
	
	public new bool enabled = true;
	public bool enabledLastFrame;
	
	
	public override void Start () {
		base.Start();
		currentEnergy = maxEnergy;
		cachedBullet = Resources.Load(bulletResourcePath, typeof(GameObject)) as GameObject;
		cachedBullet.SetActive(false);
	}
	
	
	public override void Update () {
		base.Update();	
		
		float fractionOfMaxEnergy = currentEnergy/maxEnergy;
		
		if (fractionOfMaxEnergy <= energyRatioToBeCritical) {
			this.state = EnergyState.CRITICAL;
		}
		else if (fractionOfMaxEnergy <= energyRatioToBeDrained) {
			this.state = EnergyState.DRAINED;
		}
		else {
			this.state = EnergyState.FULL;
		}
	}
	

	void FixedUpdate() {

		timeUntilCanShoot = Mathf.Max(0.0f, timeUntilCanShoot - Time.deltaTime);
		if (enabled && !enabledLastFrame) {
			timeUntilCanShoot = cooldownBetweenShots;
		}
		enabledLastFrame = enabled;

		float currentRechargeRate = energyRechargeRate;
		if (timeUntilCanShoot == 0.0f) {
			currentEnergy *= energyRechargeMultiplierWhenNotShooting;
		}
		currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRechargeRate*Time.deltaTime);

		if (!enabled) {
			return;
		}

		if (shooting && timeUntilCanShoot == 0.0f && currentEnergy >= energyPerShot) {

			GameObject bulletGameObject;
			if (NetworkManager.IsSinglePlayer()) {
				bulletGameObject = GameObject.Instantiate(
					cachedBullet,
					this.transform.position, 
					spaceshipModel.transform.rotation
				) as GameObject;
			}
			else {
				bulletGameObject = Network.Instantiate(
					cachedBullet,
					this.transform.position, 
					spaceshipModel.transform.rotation,
					666
				) as GameObject;
			}
			this.audio.PlayOneShot(shootingSound);

			Bullet bullet = bulletGameObject.GetComponent<Bullet>();
			bullet.direction = spaceshipModel.transform.forward;
			bullet.sourceSpaceship = spaceship;

			bulletGameObject.SetActive(true);
			
			timeUntilCanShoot = cooldownBetweenShots;
			currentEnergy -= energyPerShot;
			this.audio.PlayOneShot(shootingSound);
		}
	}
	

	void OnDestroy() {
		cachedBullet.SetActive(true);
	}
	
	
}