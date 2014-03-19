using UnityEngine;
using System.Collections;
using InControl;

public enum HealthState {
	HEALTHY,
	INJURED,
	CRITICAL
}

public class SpaceshipHealth : SpaceshipComponent, IDamageable {

		public float currentHealth = 100;
		public float maxHealth = 100;
	
		public HealthState state = HealthState.HEALTHY;

		public float healthRatioToBeInjured = 0.60f;
		public float healthRatioToBeCritical = 0.30f;

		public SpaceshipRaceData raceData;

		public float debugSelfDestructDamageRate = 1.0f;
		


	public override void Start() {
		base.Start();
		currentHealth = maxHealth;
		raceData = GetComponent<SpaceshipRaceData>();
	}



	public override void Update() {
		base.Update();
		float fractionOfMaxHealth = currentHealth/maxHealth;
		
		if (debugSelfDestruct) {
			this.ApplyDamage(debugSelfDestructDamageRate);
		}

		if (fractionOfMaxHealth <= healthRatioToBeCritical) {
			this.state = HealthState.CRITICAL;
		}
		else if (fractionOfMaxHealth <= healthRatioToBeInjured) {
			this.state = HealthState.INJURED;
		}
		else {
			this.state = HealthState.HEALTHY;
		}

		HandleDeath();

	}



	void HandleDeath() {
		if (currentHealth <= 0.0f) {
			Debug.Log ("Player is dead!");
			raceData.lastCheckpoint.SpawnSpaceship(this.spaceship);
			currentHealth = maxHealth;
		}
	}



	// Implementing Damageable interface.
	public void ApplyDamage(float amount) {
		this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
	}

		


}