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

		float currentRechargeRate = energyRechargeRate;
		if (timeUntilCanShoot == 0.0f) {
			currentEnergy *= energyRechargeMultiplierWhenNotShooting;
		}
		currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRechargeRate*Time.deltaTime);

		if (shooting && timeUntilCanShoot == 0.0f && currentEnergy >= energyPerShot) {
			GameObject bulletGameObject = GameObject.Instantiate(
				cachedBullet,
				this.transform.position, 
				spaceshipModel.transform.rotation
			) as GameObject;
			
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