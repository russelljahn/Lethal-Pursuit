using UnityEngine;
using System.Collections;

public class HudEnemyIndicator : MonoBehaviour {

	public Spaceship spaceship;
	public SpaceshipHealth health;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (spaceship == null) {
			Spaceship ship = GameplayManager.spaceship;
			if (ship != null) {
				spaceship = ship.GetComponent<Spaceship>();
				health = ship.GetComponent<SpaceshipHealth>();
			}
		}
		else {

		}
	}
}
