using UnityEngine;
using System.Collections;
using InControl;


public class SpaceshipPickups : SpaceshipComponent {

	public Pickup currentPickup = null;
	public bool pickupIsActive = false;

	void Start () {
	
	}


	void Update () {

		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
		}

		/* Swap guns if hitting bumpers. */
		if (currentPickup != null && (InputManager.ActiveDevice.RightBumper.IsPressed || InputManager.ActiveDevice.LeftBumper.IsPressed)) {
			pickupIsActive = !pickupIsActive;
			currentPickup.SetActive(pickupIsActive);
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
