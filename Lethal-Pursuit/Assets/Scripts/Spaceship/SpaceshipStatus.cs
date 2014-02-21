using UnityEngine;
using System.Collections;
using InControl;

public class SpaceshipStatus : SpaceshipComponent {


		public static float health = 100; /*health, damage, & status */
		private float maxHealth = 100;
		private float damageHalfHit = 50;
		private float damageOneHitKO = 100;
		private float damageBufferTime = 2;
		private float stall = 0;
		public Detonator detonator1;
		public Detonator detonator2;
		public DetonatorSound explosion;
		public Texture2D damageOverlay;

		private UILabel label;
		

	void Start() {
		
		health = 100;
		label = GetComponent<UILabel>();
	}
		
	void OnCollisionEnter(Collision interactibles) { /* hitting walls tagged as unpassable does 50 damage */

		if (interactibles.gameObject.tag == "Unpassable" ) {

			DecreaseHealth ();
			detonator1.Explode();
			detonator2.Explode();
			ShakeCamera(1, 1);
			fadeIn();
			}
		if (interactibles.gameObject.tag == "Health Pack") { //+50 health
				
				health = Mathf.Min (health+50, maxHealth);
			}
							
	}

			
	void DecreaseHealth (){
			
				if (health > 0) {
						health -= damageHalfHit;
						stall = 1;
						/* yield WaitForSeconds (damageBufferTime);  damage buffer code, not working atm, use invoke?*/
						stall = 0;
						
						}
				else if (health == 0) {
							
								//Debug.Log ("you died, scrub");
						}
					}
		

	public void ShakeCamera(float magnitude, float time) {

			Vector3 magnitudeVector = new Vector3 (magnitude, magnitude, 0.0f);
			iTween.ShakePosition (GetComponent<SpaceshipCamera>().gameObject, magnitudeVector, time);

		}

	public void fadeIn () {
		iTween.CameraFadeAdd(damageOverlay);
		iTween.CameraFadeFrom(0,.5f);
		iTween.CameraFadeTo(1.0f, 1.0f);



	}

	void Update() {


		label.text = health.ToString(); 


	}



}
