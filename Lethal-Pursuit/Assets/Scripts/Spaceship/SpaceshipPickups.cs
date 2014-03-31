using UnityEngine;
using System.Collections;
using InControl;
using System;





public class SpaceshipPickups : SpaceshipComponent {
	
	public Pickup currentPickup = null;
	public bool pickupIsActive = false;

	public float pickupSwapCooldown = 0.125f;
	public float remainingSwapCooldownTime;

	void Start () {
	
	}


	void Update () {
		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
			equippedItem = ItemType.DEFAULT_WEAPON;
		}

		remainingSwapCooldownTime = Mathf.Max(0.0f, remainingSwapCooldownTime-Time.deltaTime);

		/* Swap guns if hitting bumpers. */
		if (remainingSwapCooldownTime == 0.0f && swappingWeapon) {
			switch (equippedItem) {
				case ItemType.DEFAULT_WEAPON:
					if (currentPickup != null) {
						currentPickup.SetActive(true);
						spaceship.DisableGun();
						equippedItem = ItemType.SUB_WEAPON;	
					}
					break;
				case ItemType.SUB_WEAPON:
					currentPickup.SetActive(false);
					spaceship.EnableGun();
					equippedItem = ItemType.DEFAULT_WEAPON;
					break;
				default:
					throw new Exception("Unknown ItemType: " + equippedItem);
			}
			remainingSwapCooldownTime = pickupSwapCooldown;
		}

	}


	public bool CanPickup(Pickup pickup) {
		if (pickup is PickupHealth) {
			SpaceshipHealth health = spaceship.GetComponent<SpaceshipHealth>();
			return health.currentHealth < health.maxHealth;
		}
		return currentPickup == null;
	}


	public void GetPickup(Pickup pickup) {
		pickup.gameObject.transform.parent = this.transform;
		if (!(pickup is PickupHealth)) {
			currentPickup = pickup;
			pickupIsActive = true;
		}
			
		pickup.OnPickup(this.spaceship);
		pickup.gameObject.SetActive(true);
	}
}
