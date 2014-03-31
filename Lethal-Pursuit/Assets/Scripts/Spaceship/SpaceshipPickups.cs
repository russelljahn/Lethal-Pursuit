using UnityEngine;
using System.Collections;
using InControl;
using System;





public class SpaceshipPickups : SpaceshipComponent {
	
	public Pickup currentPickup = null;
	public float pickupSwapCooldown = 0.125f;
	public float remainingSwapCooldownTime;


	void Start () {
	
	}


	void Update () {
		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
			equippedItem = EquipType.DEFAULT_WEAPON;
		}

		remainingSwapCooldownTime = Mathf.Max(0.0f, remainingSwapCooldownTime-Time.deltaTime);

		/* Swap guns if hitting bumpers. */
		if (remainingSwapCooldownTime == 0.0f && swappingWeapon) {
			switch (equippedItem) {
				case EquipType.DEFAULT_WEAPON:
					if (currentPickup != null) {
						currentPickup.SetActive(true);
						spaceship.DisableGun();
						equippedItem = EquipType.SUB_WEAPON;	
					}
					break;
				case EquipType.SUB_WEAPON:
					currentPickup.SetActive(false);
					spaceship.EnableGun();
					equippedItem = EquipType.DEFAULT_WEAPON;
					break;
				default:
					throw new Exception("Unknown EquipType: " + equippedItem);
			}
			remainingSwapCooldownTime = pickupSwapCooldown;
		}

	}


	public bool CanPickup(Pickup pickup) {
		if (pickup is PickupHealth) {
			SpaceshipHealth health = spaceship.GetComponent<SpaceshipHealth>();
			return health.currentHealth < health.maxHealth;
		}
		else {
			return currentPickup == null;
		}
	}


	public void GetPickup(Pickup pickup) {
		pickup.gameObject.transform.parent = this.transform;

		// Basically checking if item is equippable. Need to eventually make less hacky.
		if (pickup.IsEquippable()) {
			currentPickup = pickup;
			equippedItem = EquipType.SUB_WEAPON;
		}
			
		pickup.OnPickup(this.spaceship);
		pickup.gameObject.SetActive(true);
	}
}
