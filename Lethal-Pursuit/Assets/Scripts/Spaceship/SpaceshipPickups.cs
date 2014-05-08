using UnityEngine;
using System.Collections;
using InControl;
using System;





public class SpaceshipPickups : SpaceshipComponent {
	
	public Pickup currentPickup = null;


	public override void Update () {
		if (currentPickup != null && currentPickup.ShouldDrop()) {
			currentPickup.OnDrop();
			GameObject.Destroy(currentPickup.gameObject);
			currentPickup = null;
			equippedItem = EquipType.DEFAULT_WEAPON;
			spaceship.EnableGun();
		}

		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {

			/* Swap guns if hitting bumpers. */
			if (swappingWeapon) {
				SwapEquippedItem();
				if (!NetworkManager.IsSinglePlayer()) {
					networkView.RPC("NetworkSwapEquippedItem", RPCMode.OthersBuffered);
				}				
			}
		}
	}


	public void SwapEquippedItem() {
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
	}


	[RPC]
	void NetworkSwapEquippedItem() {
		SwapEquippedItem();
	}


	public bool CanPickup(Pickup pickup) {
		return true;
	}


	public void GetPickup(Pickup pickup) {
		pickup.gameObject.transform.parent = this.transform;
		pickup.transform.localPosition = Vector3.zero;

		if (pickup.IsEquippable()) {
			if (currentPickup != null) {
				currentPickup.OnDrop();
				GameObject.Destroy(currentPickup.gameObject);
			}
			currentPickup = pickup;
			equippedItem = EquipType.SUB_WEAPON;
			spaceship.DisableGun();
		}
			
		pickup.OnPickup(this.spaceship);
		pickup.gameObject.SetActive(true);
	}
}
