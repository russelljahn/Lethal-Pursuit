using UnityEngine;
using System;
using System.Collections;



public class SpawnManager {

	private static GameObject [] spawnPoints;
	private static int lastCheckpointID = 0;


	public static void GenerateSpawnPoints() {
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		lastCheckpointID = 0;
	}


	public static void SpawnSpaceship(Spaceship spaceship) {

		if (spawnPoints.Length > 0) {
			spaceship.transform.position = spawnPoints[lastCheckpointID].transform.position;
			spaceship.transform.rotation = spawnPoints[lastCheckpointID].transform.rotation;
			Debug.Log ("Spawning '" + spaceship + "' at SpawnPoint " + lastCheckpointID + "!");	
			lastCheckpointID = (++lastCheckpointID)%spawnPoints.Length;
		}
		else {
			Debug.LogError("SpawnManager: No SpawnPoints; Spawning '" + spaceship + "' at world origin!");
		}


	}


}
