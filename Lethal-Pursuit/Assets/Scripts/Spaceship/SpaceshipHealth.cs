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

	private bool damageOverlayCurrentlyAnimating = false;
	private Texture2D damageOverlayImage;
	public float damageOverlaySpeed = 0.5f;
	public float damageOverlayOpacity = 0.175f;

	public GameObject damageIndicator;
	public GameObject lastDamager;

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
		HandleDamageIndicator();
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



	void HandleDamageIndicator() {
		if (lastDamager != null) {
			damageIndicator.transform.LookAt(lastDamager.transform.position);
			Debug.Log ("lastDamager.transform.position: " + lastDamager.transform.position);
		}
		
	}
	
	
	
	// Implementing Damageable interface.
	public void ApplyDamage(float amount, GameObject damager, string message) {
		
		Debug.Log("My ipaddr is: " + Network.player.ipAddress);
		Debug.Log("damager ipaddr: " + damager.networkView.owner.ipAddress);

		Debug.Log (message);
		Debug.Log(string.Format("Damager: {0}, location: {1}", damager, damager.transform.position));
//		damageIndicator.transform.rotation = Quaternion.LookRotation(damager.transform.position-this.transform.position);
//		damageIndicator.transform.rotation = Quaternion.Euler(damageIndicator.transform.rotation.eulerAngles + new Vector3(0.0f, 90f, 0f));
		lastDamager = damager;
		if (!invulnerable) {
			Debug.Log ("Being hurt and not invincible!");
			if (networkView.isMine || NetworkManager.IsSinglePlayer()) {
				this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
				//lastHurtByPlayerID = -1;
				FadeInDamageOverlay();
			}
			else {
				int index = NetworkManager.GetPlayerIndex(damager.networkView.owner.ipAddress);
				
				// Need to check if not -1 for non existing game object
				
				networkView.RPC("NetworkTakeDamage", NetworkManager.GetPlayerList()[index], amount, NetworkManager.GetPlayerID());
				//networkView.RPC("NetworkApplyDamage", RPCMode.Others, amount);				
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
	private void NetworkTakeDamage(float amount, int playerID) {
		if (!invulnerable) {
			this.currentHealth = Mathf.Max(0.0f, this.currentHealth - amount);
			lastHurtByPlayerID = playerID;
			Debug.Log("Hurt by playerID: " + playerID);

			FadeInDamageOverlay();
		}
	}
	

	// Use this for initialization
	public void FadeInDamageOverlay () {
		if (damageOverlayCurrentlyAnimating) {
			return;
		}
		damageOverlayCurrentlyAnimating = true;

		Hashtable iTweenSettings = new Hashtable();
		iTweenSettings["oncomplete"] = "OnFinishedAnimatingDamageOverlay";
		iTweenSettings["oncompletetarget"] = this.gameObject;
		iTweenSettings["amount"] = damageOverlayOpacity;
		iTweenSettings["time"] = damageOverlaySpeed;
		iTweenSettings["easetype"] = "easeInOutQuad";

		if (damageOverlayImage == null) {
			damageOverlayImage = iTween.CameraTexture(Color.red);
			iTween.CameraFadeAdd(damageOverlayImage); // Replace w/ tex eventually
		}
		iTween.CameraFadeFrom(iTweenSettings);
	}


	public void OnFinishedAnimatingDamageOverlay() {
		damageOverlayCurrentlyAnimating = false;	
	}
	
}
