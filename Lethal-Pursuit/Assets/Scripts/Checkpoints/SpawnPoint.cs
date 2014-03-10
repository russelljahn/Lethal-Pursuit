using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SpawnPoint : MonoBehaviour {

	public bool available;
	private int numSpaceships;

	// Use this for initialization
	void Start () {
		this.available = true;
		this.collider.isTrigger = true;
	}


	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.CompareTag("Spaceship")) {
			++numSpaceships;
			available = false;
		}
	}


	void OnTriggerExit(Collider collider) {
		if (collider.gameObject.CompareTag("Spaceship")) {	
			--numSpaceships;

			if (numSpaceships == 0) {
				available = true;
			}
			else if (numSpaceships < 0) {
				throw new Exception("Error; Negative number of spaceships in SpawnPoint: " + numSpaceships);
			}
		}
	}


	public void SpawnSpaceship(Spaceship spaceship) {
		spaceship.transform.position = this.transform.position;
	}

}
