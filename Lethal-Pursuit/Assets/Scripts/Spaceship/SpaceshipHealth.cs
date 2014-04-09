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
	
	public bool invulnerable = false;
	
	public MatchManager matchManager;
	
	public int lastHurtByPlayerID = -1;

	public Material electricField;
	public float electricFieldOpacity = 0.4f;
	public float timeToShowElectricField = 2.0f;
	private float remainingElectricFieldTime;
	public float electricFieldHideSpeed = 0.5f;

	private UI2DSprite damageOverlayImage;
	public float damageOverlayOpacity = 0.4f;
	public float timeToShowDamageOverlay = 2.0f;
	private float remainingDamageOverlayTime;
	public float damageOverlayHideSpeed = 0.5f;

	public GameObject currentDamager;
	public GameObject lastDamager;



	public override void Start() {
		base.Start();
		currentHealth = maxHealth;
		matchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
//		damageOverlayImage = GameObject.FindGameObjectWithTag("DamageOverlay").GetComponent<UI2DSprite>();

//		if (damageOverlayImage == null) {
//			Debug.Log("No UI2DSprite tagged with 'DamageOverlay' was found in the scene!");
//		}
	}
	
	
	
	public override void Update() {
		base.Update();
		
		timeUntilVulnerable = Mathf.Max(0.0f, timeUntilVulnerable-Time.deltaTime);
		invulnerable = timeUntilVulnerable > 0.0f;
		
		float fractionOfMaxHealth = currentHealth/maxHealth;
		
		if (fractionOfMaxHealth <= healthRatioToBeCritical) {
			this.state = HealthState.CRITICAL;
		}
		else if (fractionOfMaxHealth <= healthRatioToBeInjured) {
			this.state = HealthState.INJURED;
		}
		else {
			this.state = HealthState.HEALTHY;
		}

		HandleDamageOverlay();
		HandleDamageElectricField();
		HandleDeath();
		lastDamager = currentDamager;
		currentDamager = null;
	}
	
	
	
	void HandleDeath() {
		if (IsDead()) {
			Debug.Log ("Player is dead!");
			SpawnManager.SpawnSpaceship(this.spaceship);
			currentHealth = maxHealth;
			timeUntilVulnerable = respawnInvulnerabilityTime;
			if (!NetworkManager.IsSinglePlayer()) {
				matchManager.InformServerForKilledBy(lastHurtByPlayerID);
			}
		}
	}


	void HandleDamageOverlay() {
		if (damageOverlayImage == null) {
			return;
		}

		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			
			if (IsDead()) {
				damageOverlayImage.alpha = 0.0f;
				return;
			}

			if (currentDamager != null && currentDamager != lastDamager) {
				damageOverlayImage.alpha = damageOverlayOpacity;
				remainingDamageOverlayTime = timeToShowDamageOverlay;
			}
			else {
				remainingDamageOverlayTime = Mathf.Max(0.0f, remainingDamageOverlayTime-Time.deltaTime);
			}
			if (remainingDamageOverlayTime <= 0.0f) {
				damageOverlayImage.alpha = Mathf.Max(0.0f, damageOverlayImage.alpha-Time.deltaTime*damageOverlayHideSpeed);
				lastDamager = null;
			}
		}
	}


		
	void HandleDamageElectricField() {
		if (electricField == null) {
			return;
		}
		
		Color newShellColor = electricField.GetColor("_TintColor");
		
		if (IsDead()) {
			newShellColor.a = 0.0f;
			electricField.SetColor("_TintColor", newShellColor);	
			return;
		}
		
		if (currentDamager != null && currentDamager != lastDamager) {
			newShellColor.a = damageOverlayOpacity;
			remainingElectricFieldTime = timeToShowElectricField;
		}
		else {
			remainingElectricFieldTime = Mathf.Max(0.0f, remainingElectricFieldTime-Time.deltaTime);
		}
		if (remainingElectricFieldTime <= 0.0f) {
			newShellColor.a = Mathf.Max(0.0f, newShellColor.a-Time.deltaTime*damageOverlayHideSpeed);
			lastDamager = null;
		}
		electricField.SetColor("_TintColor", newShellColor);
	}
	

	// Implementing Damageable interface.
	public void ApplyDamage(float amount, GameObject damager, string message) {
		
		Debug.Log("My ipaddr is: " + Network.player.ipAddress);
		Debug.Log("damager ipaddr: " + damager.networkView.owner.ipAddress);
		Debug.Log (message);

		lastDamager = currentDamager;
		currentDamager = damager;
		if (!invulnerable) {
			if (networkView.isMine || NetworkManager.IsSinglePlayer()) {
				this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
				//lastHurtByPlayerID = -1;
			}
			else {
				int index = NetworkManager.GetPlayerIndex(damager.networkView.owner.ipAddress);
				
				// Need to check if not -1 for non existing game object
				networkView.RPC("NetworkTakeDamage", NetworkManager.GetPlayerList()[index], amount, NetworkManager.GetPlayerID());
				//networkView.RPC("NetworkApplyDamage", RPCMode.Others, amount);				
			}
		}
		else {
			Debug.Log ("Being hit BUT INVINCIBLE!");
		}
	}



	public bool IsDead() {
		return currentHealth <= 0.0f;
	}
	
	
	
	[RPC]
	private void NetworkTakeDamage(float amount, int playerID) {
		if (!invulnerable) {
			this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
			lastHurtByPlayerID = playerID;
			Debug.Log("Hurt by playerID: " + playerID);
		}
	}
	


	
}
