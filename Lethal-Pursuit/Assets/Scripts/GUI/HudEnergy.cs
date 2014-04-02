using UnityEngine;
using System;
using System.Collections;

public class HudEnergy : MonoBehaviour {

	public Color defaultGunColor = Color.cyan;
	public Color auroraCannonColor = Color.cyan;
	public Color punkMissilesColor = Color.cyan;
	
//	public Color drainedColor = Color.blue;
//	public Color criticalColor = Color.grey;

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
			if (pickups.currentPickup is PickupAuroraCannon) {
				energybarSprite.color = auroraCannonColor;
				PickupAuroraCannon auroraCannon = pickups.currentPickup as PickupAuroraCannon;
				currentScale.x = auroraCannon.currentEnergy/auroraCannon.maxEnergy * initialScale.x;
			}
			else if (pickups.currentPickup is PickupPunkMissiles) {
				energybarSprite.color = punkMissilesColor;
				PickupPunkMissiles punkMissiles = pickups.currentPickup as PickupPunkMissiles;
				currentScale.x = ((float)punkMissiles.currentShots)/punkMissiles.maxShots * initialScale.x;
			}
		}
		this.transform.localScale = currentScale;
	}
}
