using UnityEngine;
using System;
using System.Collections;

/* 
	A checkpoint is expected to have multiple SpawnPoints where a player spaceship will be spawned on death.
 */
[RequireComponent (typeof (Collider))]
public class Checkpoint : MonoBehaviour {
	
	public int id; 
	public SpawnPoint [] spawnPoints;


	void Awake() {
		this.gameObject.tag = "Checkpoint";
	}



	public static Checkpoint GetCheckpointByID(int id) {
		Checkpoint [] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
		for (int i = 0; i < checkpoints.Length; ++i) {
			if (checkpoints[i].id == id) {
				return checkpoints[i];
			}
		}
		throw new Exception("No checkpoint with id '" + id + "'");
	}



	public void SpawnSpaceship(Spaceship spaceship) {
		if (spawnPoints.Length == 0) {
			spaceship.transform.position = this.transform.position;
			throw new Exception (this.gameObject.name + " has no SpawnPoints assigned; spawning '" + spaceship.gameObject.name + "' at checkpoint's location instead!");
		}

		for (int i = 0; i < spawnPoints.Length; ++i) {
			SpawnPoint currentSpawnPoint = spawnPoints[i];
			if (currentSpawnPoint.available) {
				currentSpawnPoint.SpawnSpaceship(spaceship);
				return;
			}
		}

		spawnPoints[0].SpawnSpaceship(spaceship);
		throw new Exception("All SpawnPoints for '" + this.gameObject.name + " are taken; forcing spawn at SpawnPoint '" + spawnPoints[0] + "'!");


	}




}
