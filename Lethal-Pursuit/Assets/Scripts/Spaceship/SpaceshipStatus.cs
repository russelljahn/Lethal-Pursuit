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
		
	
//			void OnCollisionEnter(Collision walls) { /* hitting walls tagged as unpassable does 50 damage */
//
//		if (walls.gameObject.tag == "Unpassable" && stall == 0) {
//			
//			DecreaseHealth ();
//			detonator1.Explode();
//			detonator2.Explode();
//			explosion.audio.Play();
//
//
//				
//				}
//			}
//			
//		void DecreaseHealth (){
//			
//			health -= damageHalfHit;
//			stall = 1;
//			/* yield WaitForSeconds (damageBufferTime);  damage buffer code, not working atm*/
//			stall = 0;
//			Debug.Log(health);
//
//			}





}
