using UnityEngine;
using System.Collections;


public class PickupHealth : Pickup {


	public float healthAmount = 50.0f;


	public override bool ShouldDrop() {
		return true;
	}


	public override void OnPickup(Spaceship spaceship) {
		base.OnPickup(spaceship);
		SpaceshipHealth health = spaceship.GetComponent<SpaceshipHealth>();
		health.currentHealth = Mathf.Min(health.currentHealth+healthAmount, health.maxHealth);
	}

}
