using UnityEngine;
using System.Collections;


public class PickupFlamethrower : Pickup {

	public PlaygroundParticles laserBeamEffect;
	public PlaygroundPresetLaser laserBeamEffectScript;
	
	public float damageRate = 30.0f;
	public float laserHitForce = 1000.0f;
	public float maxEnergy = 100f;
	public float currentEnergy;
	public float energyDrainRate = 50.0f;
	public float laserLength = 500.0f;
	private GameObject hitGameObject;

	private float damageCooldown = 0.0f;
	private float maxDamageCooldown = 0.25f;

	public override void Update() {
		if (!active) {
			laserBeamEffect.emissionRate = 0.0f;
			return;
		}
		if (spaceship != null && spaceship.shooting) {
			currentEnergy -= energyDrainRate*Time.deltaTime;
			laserBeamEffect.emissionRate = 1.0f;	
			
			Ray ray = new Ray(spaceship.spaceshipModelPitchYaw.transform.position, spaceship.spaceshipModelPitchYaw.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, laserLength)) {
				Debug.Log("ray hit: " + hit.collider.gameObject);
				
				/* Apply laser force. */
				if (hit.rigidbody != null) {
					hitGameObject = hit.collider.gameObject;
					hit.rigidbody.AddForceAtPosition(ray.direction*laserHitForce, hit.point);
				}
			}
		}
		else {
			hitGameObject = null;
			laserBeamEffect.emissionRate = 0.0f;
		}
	}


	void FixedUpdate() {
		damageCooldown = Mathf.Max(0.0f, damageCooldown-Time.deltaTime);
		if (!active) {
			return;
		}
		if (hitGameObject == null) {
			return;
		}
		if (damageCooldown <= 0.0f) {
			damageCooldown = maxDamageCooldown;
		}
		else {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));

		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageRate*maxDamageCooldown, spaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}


	public override bool IsEquippable() {
		return true;
	}


	public override bool ShouldDrop() {
		return currentEnergy <= 0.0f;
	}


	public override void OnPickup (Spaceship spaceship) {
		base.OnPickup (spaceship);
		laserBeamEffect.transform.localScale = Vector3.one;
		laserBeamEffect.sourceTransform = spaceship.spaceshipModelPitchYaw.transform;
		laserBeamEffectScript.laserMaxDistance = laserLength;
		currentEnergy = maxEnergy;
		active = true;
	}


	public override void OnDrop() {

	}
	
}
