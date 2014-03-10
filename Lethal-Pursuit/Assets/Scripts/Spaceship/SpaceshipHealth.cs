using UnityEngine;
using System.Collections;
using InControl;

public enum HealthState {
	HEALTHY,
	INJURED,
	CRITICAL
}

public class SpaceshipHealth : SpaceshipComponent {

		public float currentHealth = 100;
		public float maxHealth = 100;
	
		public HealthState state = HealthState.HEALTHY;

		public float healthRatioToBeInjured = 0.60f;
		public float healthRatioToBeCritical = 0.30f;

		public SpaceshipRaceData raceData;

//		public Detonator detonator1;
//		public Detonator detonator2;
//		public DetonatorSound explosion;
//
//		private UILabel label;
		

	void Start() {
		currentHealth = maxHealth;
		raceData = GetComponent<SpaceshipRaceData>();
	}


	void Update() {

		float fractionOfMaxHealth = currentHealth/maxHealth;
		
		if (debugSelfDestruct) {
			this.currentHealth = 0.0f;
		}
		else if (fractionOfMaxHealth <= healthRatioToBeCritical) {
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
			this.transform.position = raceData.lastCheckpoint.transform.position;
			currentHealth = maxHealth;
		}
	}
		
//	void OnCollisionEnter(Collision interactibles) { /* hitting walls tagged as unpassable does 50 damage */
//
//		if (interactibles.gameObject.tag == "Unpassable" ) {
//
//			DecreaseHealth ();
//			detonator1.Explode();
//			detonator2.Explode();
//			ShakeCamera(1, 1);
//			fadeIn();
//			}
//		if (interactibles.gameObject.tag == "Health Pack") { //+50 health
//				
//				health = Mathf.Min (health+50, maxHealth);
//			}
//							
//	}
//
//			
//	void DecreaseHealth (){
//			
//				if (health > 0) {
//						health -= damageHalfHit;
//						stall = 1;
//						/* yield WaitForSeconds (damageBufferTime);  damage buffer code, not working atm, use invoke?*/
//						stall = 0;
//						
//						}
//				else if (health == 0) {
//							
//								//Debug.Log ("you died, scrub");
//						}
//					}
//		
//
//	public void ShakeCamera(float magnitude, float time) {
//
//			Vector3 magnitudeVector = new Vector3 (magnitude, magnitude, 0.0f);
//			iTween.ShakePosition (GetComponent<SpaceshipCamera>().gameObject, magnitudeVector, time);
//
//		}
//
//	public void fadeIn () {
//		iTween.CameraFadeAdd(damageOverlay);
//		iTween.CameraFadeFrom(0,.5f);
//		iTween.CameraFadeTo(1.0f, 1.0f);
//
//
//
//	}
//
//	void Update() {
//
//
//		label.text = health.ToString(); 
//
//
//	}



}
