using UnityEngine;
using System.Collections;

public class SpaceshipPickups : SpaceshipComponent {

	public Pickup currentPickup = null;

	void Start () {
	
	}


	void Update () {
		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
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
	}
}
