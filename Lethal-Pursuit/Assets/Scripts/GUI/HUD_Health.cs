using UnityEngine;
using System;
using System.Collections;

public class HUD_Health : MonoBehaviour {

	public Color healthyColor = Color.green;
	public Color injuredColor = Color.yellow;
	public Color criticalColor = Color.red;

	private SpaceshipStatus status;
	private UI2DSprite healthbarSprite;

	private Vector3 initialScale;
	private Vector3 currentScale;


	// Use this for initialization
	void Start () {
		status = GameObject.FindWithTag("Spaceship").GetComponent<SpaceshipStatus>();
		healthbarSprite = GetComponent<UI2DSprite>();
		initialScale = this.transform.localScale;
		currentScale = initialScale;
	}
	
	// Update is called once per frame
	void Update () {
		currentScale.x = status.currentHealth/status.maxHealth * initialScale.x;
		this.transform.localScale = currentScale;

		switch (status.healthState) {
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
				throw new Exception("Unknown health state: " + status.healthState);
		}
	}
}
