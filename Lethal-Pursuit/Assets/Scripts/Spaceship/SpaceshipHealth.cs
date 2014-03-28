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
		
		public MatchManager matchManager;

		public int lastHurtByPlayerID = -1;


	public override void Start() {
		base.Start();
		currentHealth = maxHealth;
		matchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
	}



	public override void Update() {
		base.Update();

		timeUntilVulnerable = Mathf.Max(0.0f, timeUntilVulnerable-Time.deltaTime);
		invulnerable = timeUntilVulnerable > 0.0f;

		float fractionOfMaxHealth = currentHealth/maxHealth;
		
		if (debugSelfDestruct) {
			this.ApplyDamage(debugSelfDestructDamageRate, this.gameObject);
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

	}



	void HandleDeath() {

			Debug.Log ("Player is dead!");
			SpawnManager.SpawnSpaceship(this.spaceship);
			currentHealth = maxHealth;
			timeUntilVulnerable = respawnInvulnerabilityTime;
<<<<<<< HEAD
			matchManager.InformServerForKilledBy(lastHurtByPlayerID);
		}
=======

>>>>>>> 5d2353ada24dfbfbad2d973c89b8f0975f0e56af
	}



	// Implementing Damageable interface.
	public void ApplyDamage(float amount, GameObject damager) {
		if (!invulnerable) {
			if (networkView.isMine || NetworkManager.IsSinglePlayer()) {
				this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
				lastHurtByPlayerID = -1;
			}
			else {
				int index = NetworkManager.GetPlayerIndex(damager.networkView.owner.ipAddress);
<<<<<<< HEAD
				
				// Need to check if not -1 for non existing game object

				networkView.RPC("NetworkApplyDamage", NetworkManager.GetPlayerList()[index], amount, NetworkManager.GetPlayerID());
//				networkView.RPC("NetworkApplyDamage", RPCMode.Others, amount);
				
=======

				// Need to check if not -1 for non existing game object

				networkView.RPC("NetworkApplyDamage", NetworkManager.GetPlayerList()[index], amount);
>>>>>>> 5d2353ada24dfbfbad2d973c89b8f0975f0e56af

			}
		}
	}
	public bool IsDead() {
		return currentHealth <= 0.0f;
	}



	[RPC]
	private void NetworkApplyDamage(float amount, int playerID) {
		this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
<<<<<<< HEAD
		lastHurtByPlayerID = playerID;
		Debug.Log("Hurt by playerID: " + playerID);
=======
		if (IsDead()) {
			HandleDeath();
		}
>>>>>>> 5d2353ada24dfbfbad2d973c89b8f0975f0e56af
	}










}