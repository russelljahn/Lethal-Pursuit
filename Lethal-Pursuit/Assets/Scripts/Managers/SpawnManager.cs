using UnityEngine;
using System;
using System.Collections;



public class SpawnManager : MonoBehaviour {

	private static GameObject [] spawnPoints;
	private static int lastCheckpointID = 0;

	public static float normalSpawnWait = 5.9f;

	public bool respawning = false;
	
	private static SpawnManager singletonInstance = null;

	private static SpawnManager instance {
		get {
			/* If first time accessing instance, then find it... */
			if (singletonInstance == null) {
				singletonInstance = FindObjectOfType(typeof (SpawnManager)) as SpawnManager;
			}
			
			/* If instance is null, then no SpawnManager exists in the scene, so create one. */
			if (singletonInstance == null) {
				GameObject obj = new GameObject("SpawnManager");
				singletonInstance = obj.AddComponent(typeof (SpawnManager)) as SpawnManager;
				obj.name = "SpawnManager";
			}
			return singletonInstance;
		}
	}


	public static void GenerateSpawnPoints() {
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		if (!NetworkManager.IsSinglePlayer()) {
			lastCheckpointID = NetworkManager.GetPlayerID()%spawnPoints.Length;
		}
	}


	public static void SpawnSpaceship(Spaceship spaceship) {
		
		if (spawnPoints.Length > 0) {
			instance.networkView.RPC("DisableShipsRespawn", RPCMode.All, spaceship);
			//instance.StartCoroutine(HandleSpawnWait(spaceship));	

			if (NetworkManager.IsSinglePlayer()) {
				lastCheckpointID = (++lastCheckpointID)%spawnPoints.Length;	
			}
			else {
				lastCheckpointID = NetworkManager.GetPlayerID()%spawnPoints.Length;
			}
		}
		else {
			Debug.LogError("SpawnManager: No SpawnPoints; Spawning '" + spaceship + "' at world origin!");
		}
	}


	private static IEnumerator HandleSpawnWait(Spaceship spaceship) {
		SpaceshipMatchData matchData = spaceship.GetComponent<SpaceshipMatchData>();
		SpaceshipControl controls = spaceship.GetComponent<SpaceshipControl>();
		Collider collider = spaceship.GetComponent<Collider>();
		
		matchData.spawnTimeRemaining = normalSpawnWait;
		
		spaceship.spaceshipModelRoot.gameObject.SetActive(false);
		controls.enabled = false;
		collider.enabled = false;

		while (matchData.spawnTimeRemaining > 0.0f) {
			yield return null;
			matchData.spawnTimeRemaining = Mathf.Max(0.0f, matchData.spawnTimeRemaining-Time.deltaTime);
		}

		spaceship.transform.position = spawnPoints[lastCheckpointID].transform.position;
		spaceship.transform.rotation = spawnPoints[lastCheckpointID].transform.rotation;

		spaceship.spaceshipModelRoot.gameObject.SetActive(true);
		controls.enabled = true;
		collider.enabled = true;

		Debug.Log ("Spawning '" + spaceship + "' at SpawnPoint " + lastCheckpointID + "!");
	}

	[RPC]
	private void DisableShipsRespawn(Spaceship spaceship) {
		instance.StartCoroutine(HandleSpawnWait(spaceship));	
	}

	                                                       


}
