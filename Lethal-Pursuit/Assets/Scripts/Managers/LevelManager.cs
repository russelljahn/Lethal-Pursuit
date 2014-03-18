using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour {


	private Transform spawnPoint;
	private static int lastLevelPrefix;

	public Level loadedLevel;
	private static LevelManager singletonInstance;

	private static LevelManager instance {
		get {
			/* If first time accessing instance, then find it... */
			if (singletonInstance == null) {
				singletonInstance = FindObjectOfType(typeof (LevelManager)) as LevelManager;
			}
			
			/* If instance is null, then no GameManager exists in the scene, so create one. */
			if (singletonInstance == null) {
				GameObject obj = new GameObject("LevelManager");
				singletonInstance = obj.AddComponent(typeof (LevelManager)) as LevelManager;
				obj.name = "Level Manager";
				//Debug.Log ("Could not find a LevelManager object, so automatically generated one.");
			}
			
			return singletonInstance;
		}
	}

	
	public void Awake() {
		DontDestroyOnLoad(this);
	}


	public Level GetLevel(string levelName) {
		Level returnLevel;
		
		if (levelName.Equals("MainMenu")) {
			return new LevelMainMenu();
		}
		if (levelName.Equals("Tutorial")) {
			return new LevelTutorial();
		}
		if (levelName.Equals("Highway")) {
			return new LevelHighway();
		}
		else {
			throw new NotImplementedException("Level '" + levelName + "' is either not known or programmed in yet!");
		}
	}
	

	public static Level GetLoadedLevel() {
		if (instance.loadedLevel == null) {
			instance.loadedLevel = instance.GetLevel(Application.loadedLevelName);
		}
		return instance.loadedLevel;
	}


	public static void LoadLevel(string levelName) {
		
		Level levelToLoad = instance.GetLevel(levelName);
		Debug.Log("Loading level: " + levelToLoad);
		Application.LoadLevel(levelToLoad.sceneName);
	}

	
	public static void ReloadLevel() {
		Debug.Log("Reloading level: " + instance.loadedLevel);
		Application.LoadLevel(instance.loadedLevel.sceneName);
	}


	public static void NetworkLoadLevel(string levelName, int levelPrefix) {
		LevelManager.instance.StartCoroutine(LoadLevelHelper(levelName, levelPrefix));
	}


	private static IEnumerator LoadLevelHelper(string levelName, int levelPrefix) {
		Debug.Log("Loading level " + levelName + " with prefix " + levelPrefix);
		lastLevelPrefix = levelPrefix;
		
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);   
		
		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to 
		// objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		
		Level levelToLoad = instance.GetLevel(levelName);
		Debug.Log("Loading level: " + levelToLoad);
		Application.LoadLevel(levelToLoad.sceneName);
		
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Debug.Log("Loading complete");
		
		Debug.Log("load level DONE");
		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
		
		Debug.Log("sending load msg");
		// Notify our objects that the level and the network is ready
		foreach (GameObject go in FindObjectsOfType(typeof(GameObject)) ) {
			Debug.Log("sending load msg");
			go.SendMessage("OnNetworkLoadedLevel", levelToLoad, SendMessageOptions.DontRequireReceiver);  
		}
		
	}


	public static void LoadMainMenu() {
		
		if (Network.isClient) {
			Network.Disconnect();
		}
		else {
			NetworkManager.ServerCleanup();
			LoadLevel("MainMenu");
			//LevelManager.instance.networkView.RPC("LevelManager.LoadLevel", RPCMode.All);
		}
	}
	

	public static void Quit() {
		Application.Quit();
	}


	void OnLevelWasLoaded(int levelNumber) {
	
		Level level = GetLevel(Application.loadedLevelName);		
		Debug.Log("OnLevelWasLoaded() for Level: " + level);
		
		if (!NetworkManager.IsSinglePlayer()) {
			OnNetworkLoadedLevel(level);
		}
	}

	void OnNetworkLoadedLevel(Level level) {

		Debug.Log("Entered OnNetworkLoadedLevel: Level " + level);
		string sceneName = level.sceneName;

		if (!sceneName.Equals("MainMenu")) {
			GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
			spawnPoint = spawnPoints[0].transform;	
		}

		if (sceneName.Equals("MainMenu")) {
			;
		}
		else if (sceneName.Equals("Tutorial")) {
			SpawnPlayer();
		}
		else if (sceneName.Equals("Highway")) {
			SpawnPlayer();
		}
		else {
			throw new NotImplementedException("Level loading for level '" + level + "' has not been programmed in yet!");
		}
	}


	void SpawnPlayer() {
		GameObject spaceship = null;

		GameObject [] previousShipsInScene = GameObject.FindGameObjectsWithTag("Spaceship");
		for (int i = 0; i < previousShipsInScene.Length; ++i) {
			previousShipsInScene[i].SetActive(false);
		}

		
		if (NetworkManager.IsSinglePlayer()) {
			spaceship = Instantiate(
				Resources.Load ("Spaceships/Patriot 69Z"),
				spawnPoint.position, 
				spawnPoint.rotation) as GameObject;
		}
		else {
			spaceship = Network.Instantiate(
				Resources.Load (NetworkManager.GetShip()),
				spawnPoint.position, 
				spawnPoint.rotation,
				0) as GameObject;
		}
		
		if (NetworkManager.IsSinglePlayer()) {
			//Disable network view if having performance issues
		}

	}


}
