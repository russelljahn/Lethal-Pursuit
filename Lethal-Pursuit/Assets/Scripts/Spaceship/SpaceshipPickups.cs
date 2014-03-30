using UnityEngine;
using System.Collections;
using InControl;


public class SpaceshipPickups : SpaceshipComponent {

	public Pickup currentPickup = null;
	public bool pickupIsActive = false;

	public float pickupSwapCooldown = 0.125f;
	public float remainingSwapCooldownTime;

	void Start () {
	
	}


	void Update () {
		remainingSwapCooldownTime = Mathf.Max(0.0f, remainingSwapCooldownTime-Time.deltaTime);

		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
		}

		/* Swap guns if hitting bumpers. */
		if (currentPickup != null && remainingSwapCooldownTime == 0.0f && (InputManager.ActiveDevice.RightBumper.IsPressed || InputManager.ActiveDevice.LeftBumper.IsPressed)) {
			pickupIsActive = !pickupIsActive;
			currentPickup.SetActive(pickupIsActive);
			remainingSwapCooldownTime = pickupSwapCooldown;
			if (!pickupIsActive) {
				spaceship.EnableGun();
			}
			else {
				spaceship.DisableGun();
			}
		}

	}


	public bool CanPickup(Pickup pickup) {
		return currentPickup == null;
	}


	public void GetPickup(Pickup pickup) {
		pickup.gameObject.transform.parent = this.transform;
		currentPickup = pickup;
		pickup.OnPickup(this.spaceship);
		pickup.gameObject.SetActive(true);
		pickupIsActive = true;
	}
}
