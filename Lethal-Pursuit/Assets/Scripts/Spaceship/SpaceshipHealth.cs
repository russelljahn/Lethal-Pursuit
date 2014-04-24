using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public enum HealthState {
	HEALTHY,
	INJURED,
	CRITICAL
}

public class SpaceshipHealth : SpaceshipComponent, IDamageable {
	
	public float currentHealth = 100;
	public float maxHealth = 100;
	
	public HealthState state = HealthState.HEALTHY;
	public float respawnInvulnerabilityTime = 6.0f;
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

	public  LineRenderer enemyIndicator;
	private GameObject enemyToTrack;
	public float enemyIndicatorOpacity = 0.4f;
	public float timeToShowEnemyIndicator = 2.0f;
	private float remainingEnemyIndicatorTime;
	public float enemyIndicatorHideSpeed = 0.5f;

	public string deathExplosionResourcePath = "Explosions/ShipDeathExplosion_01";
	
	public GameObject currentDamager;
	public GameObject lastDamager;

	private SpaceshipMatchData matchData;



	public override void Start() {
		base.Start();
		currentHealth = maxHealth;
		matchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		damageOverlayImage = GameObject.FindGameObjectWithTag("DamageOverlay").GetComponent<UI2DSprite>();
		matchData = spaceship.GetComponent<SpaceshipMatchData>();

		if (damageOverlayImage == null) {
			Debug.Log("No UI2DSprite tagged with 'DamageOverlay' was found in the scene!");
		}
		Color indicatorColor = enemyIndicator.renderer.material.GetColor("_TintColor");
		indicatorColor.a = 0.0f;
		enemyIndicator.renderer.material.SetColor("_Tint", indicatorColor);
	
		/* Make local copy of material so multiple of the same ships don't reference the same one. */
		Material electricFieldCopy = Instantiate(electricField) as Material;
		electricField = electricFieldCopy;
		spaceshipShell.renderer.material = electricFieldCopy;
		spaceshipShell.GetComponent<ScrollUVLinear>().materialToScroll = electricFieldCopy;
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
		HandleEnemyIndicators();
		HandleDeath();
		lastDamager = currentDamager;
		currentDamager = null;
	}
	
	
	
	void HandleDeath() {
		if (IsDead()) {
			Debug.Log ("Player is dead!");
			matchData.lastKilledBy = currentDamager;

			Debug.Log ("deathExplosionResourcePath: " + deathExplosionResourcePath);

			// Spawn death explosion.
			GameObject deathExplosion;
			if (NetworkManager.IsSinglePlayer()) {
				deathExplosion = GameObject.Instantiate(
					Resources.Load(deathExplosionResourcePath) as GameObject,
					this.transform.position, 
					spaceshipModel.transform.rotation
				) as GameObject;
			}
			else {
				deathExplosion = Network.Instantiate(
					Resources.Load(deathExplosionResourcePath) as GameObject,
					this.transform.position, 
					spaceshipModel.transform.rotation,
					123
				) as GameObject;
			}

			SpawnManager.SpawnSpaceship(this.spaceship);
			currentHealth = maxHealth;
			timeUntilVulnerable = respawnInvulnerabilityTime;
			if (!NetworkManager.IsSinglePlayer() && currentDamager != this.gameObject) {
				matchManager.InformServerForKilledBy(lastHurtByPlayerID);
			}
		}
	}


	void HandleDamageOverlay() {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			if (damageOverlayImage == null) {
				return;
			}
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
		}
		electricField.SetColor("_TintColor", newShellColor);
	}



	void HandleEnemyIndicators() {
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			if (enemyIndicator == null) {
				return;
			}
			Color indicatorColor = enemyIndicator.renderer.material.GetColor("_TintColor");
			if (IsDead()) {
				indicatorColor.a = 0.0f;
				enemyIndicator.renderer.material.SetColor("_TintColor", indicatorColor);
				return;
			}
			if (currentDamager != null && currentDamager != lastDamager) {
				indicatorColor.a = enemyIndicatorOpacity;
				remainingEnemyIndicatorTime = timeToShowEnemyIndicator;
				enemyToTrack = currentDamager;
				enemyIndicator.renderer.material.SetColor("_TintColor", indicatorColor);	
				enemyIndicator.SetPosition(0, enemyToTrack.transform.position);
				enemyIndicator.SetPosition(1, spaceship.transform.position);
			}
			else if (enemyToTrack != null) {
				remainingEnemyIndicatorTime = Mathf.Max(0.0f, remainingEnemyIndicatorTime-Time.deltaTime);
				enemyIndicator.renderer.material.SetColor("_TintColor", indicatorColor);	
				enemyIndicator.SetPosition(0, enemyToTrack.transform.position);
				enemyIndicator.SetPosition(1, spaceship.transform.position);
			}
			if (remainingElectricFieldTime <= 0.0f) {
				indicatorColor.a = Mathf.Max(0.0f, indicatorColor.a-Time.deltaTime*enemyIndicatorHideSpeed);
				lastDamager = null;
			}
			enemyIndicator.renderer.material.SetColor("_TintColor", indicatorColor);
		}
	}
	
	

	// Implementing Damageable interface.
	public void ApplyDamage(float amount, GameObject damager, string message) {

		lastDamager = currentDamager;
		currentDamager = damager;

		Debug.Log(string.Format("I am '{0}', My ipaddr is '{1}'", this.gameObject, Network.player.ipAddress, damager));
		Debug.Log(string.Format ("My damager is '{0}', damager ipaddr is '{1}' ", damager, damager.networkView.owner.ipAddress));
		Debug.Log (message);
		Debug.Log ("Amount to damage: " + amount);

		Debug.Log("I am player " + NetworkManager.GetPlayerID());
		List<NetworkPlayer> playerList = NetworkManager.GetPlayerList();
		Debug.Log("Number of players: " + playerList.Count);
		foreach (NetworkPlayer player in playerList) {
			Debug.Log("IPAddr : " + player.ipAddress);
		}


		if (timeUntilVulnerable <= 0.0f) {
			if (networkView.isMine || NetworkManager.IsSinglePlayer()) {
				this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
				//lastHurtByPlayerID = -1;
			}
			else {
				int index = spaceship.ownerPlayerID;

				Spaceship ship = damager.GetComponent<Spaceship>();

				Debug.Log("What is this index? Who does it belong to? " + index);
				
				Debug.Log("Damager ipaddress: " + damager.networkView.owner.ipAddress);
				Debug.Log("Damager owned by player " + NetworkManager.GetPlayerIndex(damager.networkView.owner.ipAddress));
				Debug.Log("Damage about to be applied to player: " + spaceship.ownerPlayerID);

				if (ship != null) {
					networkView.RPC("NetworkTakeDamage", NetworkManager.GetPlayerList()[index], amount, ship.ownerPlayerID);
				}
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
