using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour {


	public enum LEVEL {
		MAIN_MENU,
		TUTORIAL
	};



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


	public Level loadedLevel;



	public void Awake() {
		DontDestroyOnLoad(this);
	}


	public static Level GetLoadedLevel() {
		if (instance.loadedLevel == null) {
			instance.loadedLevel = instance.GetLevel(Application.loadedLevelName);
		}
		return instance.loadedLevel;
	}


	public static void LoadLevel(LevelManager.LEVEL level) {
		Level levelToLoad = instance.GetLevel(level);
		Debug.Log("Loading level: " + levelToLoad);
		Application.LoadLevel(levelToLoad.sceneName);
		
	}

	
	private Level GetLevel(LevelManager.LEVEL level) {
		Level returnLevel;

		switch (level) {
			case LEVEL.MAIN_MENU:
				return new LevelMainMenu();
				break;
			case LEVEL.TUTORIAL:
				return new LevelTutorial();
				break;
			default:
				throw new NotImplementedException("Level loading for level '" + level + "' has not been programmed in yet!");
		}
	}


	private Level GetLevel(string levelName) {
		Level returnLevel;
		
		if (levelName.Equals("MainMenu")) {
			return new LevelMainMenu();
		}
		if (levelName.Equals("Tutorial")) {
			return new LevelTutorial();
		}
		else {
			throw new NotImplementedException("Level '" + levelName + "' is either not known or programmed in yet!");
		}
	}
	
	
	public static void Quit() {
		Application.Quit();
	}




}
