using UnityEngine;
using System;
using System.Collections;

public class HudEnergy : MonoBehaviour {

	public Color defaultGunColor = Color.magenta;
	public Color flamethrowerColor = Color.red;
	public Color missilesColor = Color.yellow;
	public Color stickyMinesColor = Color.cyan;

	private SpaceshipGun gun;
	private SpaceshipPickups pickups;
	
	public UI2DSprite energybarSprite;

	public Vector3 initialScale;
	public Vector3 currentScale;


	// Use this for initialization
	void Start () {
		energybarSprite = GetComponent<UI2DSprite>();
		initialScale = this.transform.localScale;
		currentScale = initialScale;
	}
	
	// Update is called once per frame
	void Update () {

		if (gun == null || pickups == null) {
			Spaceship ship = GameplayManager.spaceship;
			if (ship != null) {
				gun = ship.gun;
				pickups = ship.pickups;
			}
		}
		else if (gun.enabled) {
			energybarSprite.color = defaultGunColor;
			currentScale.x = gun.currentEnergy/gun.maxEnergy * initialScale.x;	
		}
		else if (pickups.currentPickup != null) {
			if (pickups.currentPickup is PickupFlamethrower) {
				energybarSprite.color = flamethrowerColor;
				PickupFlamethrower auroraCannon = pickups.currentPickup as PickupFlamethrower;
				currentScale.x = auroraCannon.currentEnergy/auroraCannon.maxEnergy * initialScale.x;
			}
			else if (pickups.currentPickup is PickupMissiles) {
				energybarSprite.color = missilesColor;
				PickupMissiles missiles = pickups.currentPickup as PickupMissiles;
				currentScale.x = ((float)missiles.currentShots)/missiles.maxShots * initialScale.x;
			}
			else if (pickups.currentPickup is PickupStickyMines) {
				energybarSprite.color = stickyMinesColor;
				PickupStickyMines stickyMines = pickups.currentPickup as PickupStickyMines;
				currentScale.x = ((float)stickyMines.currentShots)/stickyMines.maxShots * initialScale.x;
			}
		}
		this.transform.localScale = currentScale;
	}
}
