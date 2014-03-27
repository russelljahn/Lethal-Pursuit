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
		public float respawnInvulnerabilityTime = 3.0f;
		private float timeUntilVulnerable = 0.0f;

		public float healthRatioToBeInjured = 0.60f;
		public float healthRatioToBeCritical = 0.30f;

		public float debugSelfDestructDamageRate = 1.0f;
		public bool invulnerable = false;
		


	public override void Start() {
		base.Start();
		currentHealth = maxHealth;
	}



	public override void Update() {
		base.Update();

		timeUntilVulnerable = Mathf.Max(0.0f, timeUntilVulnerable-Time.deltaTime);
		invulnerable = timeUntilVulnerable > 0.0f;

		float fractionOfMaxHealth = currentHealth/maxHealth;
		
		if (false) {
			this.ApplyDamage(debugSelfDestructDamageRate, gameObject, "Applying damage due to self-destruct!");
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
		if (IsDead()) {
			Debug.Log ("Player is dead!");
			SpawnManager.SpawnSpaceship(this.spaceship);
			currentHealth = maxHealth;
			timeUntilVulnerable = respawnInvulnerabilityTime;
		}
	}



	// Implementing Damageable interface.
	public void ApplyDamage(float amount, GameObject damager, string message) {

		Debug.Log (message);
		if (!invulnerable) {
			Debug.Log ("Being hurt and not invincible!");
			if (networkView.isMine || NetworkManager.IsSinglePlayer()) {
				this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
			}
			else {
//				int index = NetworkManager.GetPlayerIndex(damager.networkView.owner.ipAddress);
//				Debug.Log ("index: " + index);
//				Debug.Log ("NetworkManager.GetPlayerList()[index]: " + NetworkManager.GetPlayerList()[index]);
//				Debug.Log ("NetworkManager.GetPlayerList()[index].ipAddress: " + NetworkManager.GetPlayerList()[index].ipAddress);
				
				// Need to check if not -1 for non existing game object

//				networkView.RPC("NetworkApplyDamage", NetworkManager.GetPlayerList()[0], amount);
				networkView.RPC("NetworkApplyDamage", RPCMode.Others, amount);
				

			}
		}
		else {
			Debug.Log ("Being hurt BUT INVINCIBLE!");
		}
	}
	public bool IsDead() {
		return currentHealth <= 0.0f;
	}



	[RPC]
	private void NetworkApplyDamage(float amount) {
		this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
	}










}