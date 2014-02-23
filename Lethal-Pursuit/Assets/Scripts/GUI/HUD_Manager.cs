using InControl;
using UnityEngine;
using System.Collections;

public class HUD_Manager : MonoBehaviour {

	public Spaceship spaceship;

	public bool pressingStart;
	public bool releasedStart;

	private bool pressedStartLastFrame;
	private Level currentLevel;

	private bool isTutorialLevel;

	public GameObject tutorialGui;
	public GameObject menuGui;
	public GameObject raceOverGui;
	


	// Use this for initialization
	void Start () {
		currentLevel = LevelManager.GetLoadedLevel();

		isTutorialLevel = currentLevel.name.Equals("Tutorial");
		tutorialGui.SetActive(isTutorialLevel);

		menuGui.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		pressedStartLastFrame = pressingStart;
		pressingStart = Input.GetKey(KeyCode.Escape);
		releasedStart = !pressingStart && pressedStartLastFrame;


		if (releasedStart) {
			if (menuGui.activeInHierarchy) {
				HideMenu();
			}
			else {
				DisplayMenu();
			}
		}
	}


	public void DisplayMenu() {
		spaceship.enabled = false;
		menuGui.SetActive(true);
	}


	public void HideMenu() {
		spaceship.enabled = true;
		menuGui.SetActive(false);
	}


	public void DisplayRaceOver() {
		spaceship.enabled = false;
		raceOverGui.SetActive(true);
	}
	
	
	public void HideRaceOver() {
		spaceship.enabled = true;
		raceOverGui.SetActive(false);
	}


	public void ReplayTrack() {
		LevelManager.ReloadLevel();
	}


	public void LoadMainMenu() {
		LevelManager.LoadMainMenu();
	}

	
}
