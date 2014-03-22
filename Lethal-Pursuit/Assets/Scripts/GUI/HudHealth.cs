using UnityEngine;
using System;
using System.Collections;

public class HudHealth : MonoBehaviour {

	public Color healthyColor = Color.green;
	public Color injuredColor = Color.yellow;
	public Color criticalColor = Color.red;

	private SpaceshipHealth healthComponent;
	private UI2DSprite healthbarSprite;

	private Vector3 initialScale;
	private Vector3 currentScale;


	// Use this for initialization
	void Start () {
		healthbarSprite = GetComponent<UI2DSprite>();
		initialScale = this.transform.localScale;
		currentScale = initialScale;
	}
	
	// Update is called once per frame
	void Update () {

		if (healthComponent == null) {
			Spaceship ship = GameplayManager.spaceship;
			if (ship != null) {
				healthComponent = ship.GetComponent<SpaceshipHealth>();
			}
		}
		else {
			currentScale.x = healthComponent.currentHealth/healthComponent.maxHealth * initialScale.x;
			this.transform.localScale = currentScale;
	
			switch (healthComponent.state) {
				case HealthState.HEALTHY:
					healthbarSprite.color = healthyColor;
					break;
				case HealthState.INJURED:
					healthbarSprite.color = injuredColor;
					break;
				case HealthState.CRITICAL:
					healthbarSprite.color = criticalColor;
					break;
				default:
					throw new Exception("Unknown health state: " + healthComponent.state);
			}
		}
	}
}
