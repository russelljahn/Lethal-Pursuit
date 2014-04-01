using UnityEngine;
using System;
using System.Collections;

public class HudEnergy : MonoBehaviour {

	public Color fullColor = Color.cyan;
	public Color drainedColor = Color.blue;
	public Color criticalColor = Color.grey;

	private SpaceshipGun gunComponent;
	private UI2DSprite energybarSprite;

	private Vector3 initialScale;
	private Vector3 currentScale;


	// Use this for initialization
	void Start () {
		energybarSprite = GetComponent<UI2DSprite>();
		initialScale = this.transform.localScale;
		currentScale = initialScale;
	}
	
	// Update is called once per frame
	void Update () {

		if (gunComponent == null) {
			Spaceship ship = GameplayManager.spaceship;
			if (ship != null) {
				gunComponent = ship.gun;
			}
		}
		else {
			currentScale.x = gunComponent.currentEnergy/gunComponent.maxEnergy * initialScale.x;
			this.transform.localScale = currentScale;
	
			switch (gunComponent.state) {
				case EnergyState.FULL:
					energybarSprite.color = fullColor;
					break;
				case EnergyState.DRAINED:
					energybarSprite.color = drainedColor;
					break;
				case EnergyState.CRITICAL:
					energybarSprite.color = criticalColor;
					break;
				default:
					throw new Exception("Unknown Energy state: " + gunComponent.state);
			}
		}
	}
}
