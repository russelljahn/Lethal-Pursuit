using UnityEngine;
using System.Collections;

public class SpaceshipPickups : SpaceshipComponent {

	private Pickup currentPickup = null;

	void Start () {
	
	}


	void Update () {
	
	}


	public bool CanPickup(Pickup pickup) {
		return currentPickup == null;
	}


	public void GetPickup(Pickup pickup) {
		pickup.gameObject.transform.parent = this.transform.parent;
		currentPickup = pickup;
	}
}
